using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

namespace EarthUnderFreelancer.Account
{
    /// <summary>
    /// Complete Account System with Registration, Login, Password Reset, Email Verification
    /// Handles all user authentication and account management
    /// </summary>
    public class AccountSystem : MonoBehaviour
    {
        public static AccountSystem Instance { get; private set; }

        [Header("Server Configuration")]
        [SerializeField] private string authServerURL = "https://api.earthunderfreelancer.com/auth";
        [SerializeField] private float requestTimeout = 30f;
        [SerializeField] private int maxRetries = 3;

        [Header("Session Settings")]
        [SerializeField] private float sessionRefreshInterval = 300f; // 5 minutes
        [SerializeField] private float tokenExpiryTime = 3600f; // 1 hour
        [SerializeField] private bool rememberLogin = true;

        [Header("Security")]
        [SerializeField] private bool useSecureConnection = true;
        [SerializeField] private int minPasswordLength = 8;
        [SerializeField] private bool requireSpecialCharacter = true;
        [SerializeField] private bool requireNumber = true;

        // Current session
        private UserSession currentSession;
        private string authToken;
        private DateTime tokenExpiry;
        private Coroutine sessionRefreshCoroutine;

        // Events
        public event Action<UserSession> OnLoginSuccess;
        public event Action<string> OnLoginFailed;
        public event Action OnLogout;
        public event Action<UserSession> OnSessionRefreshed;
        public event Action<string> OnRegistrationSuccess;
        public event Action<string> OnRegistrationFailed;
        public event Action OnPasswordResetSent;
        public event Action<string> OnPasswordResetFailed;
        public event Action OnEmailVerified;
        public event Action<string> OnEmailVerificationFailed;
        public event Action<UserProfile> OnProfileUpdated;

        // Properties
        public bool IsLoggedIn => currentSession != null && !string.IsNullOrEmpty(authToken);
        public UserSession CurrentSession => currentSession;
        public string AuthToken => authToken;
        public string Username => currentSession?.username ?? "Guest";
        public string DisplayName => currentSession?.displayName ?? "Guest";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSavedSession();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                SaveSession();
            }
        }

        #region Registration

        /// <summary>
        /// Register a new account
        /// </summary>
        public void Register(string username, string email, string password, string displayName = null, Action<bool, string> callback = null)
        {
            // Validate input
            string validationError = ValidateRegistration(username, email, password);
            if (!string.IsNullOrEmpty(validationError))
            {
                OnRegistrationFailed?.Invoke(validationError);
                callback?.Invoke(false, validationError);
                return;
            }

            StartCoroutine(RegisterCoroutine(username, email, password, displayName ?? username, callback));
        }

        private IEnumerator RegisterCoroutine(string username, string email, string password, string displayName, Action<bool, string> callback)
        {
            var registrationData = new RegistrationRequest
            {
                username = username,
                email = email,
                passwordHash = HashPassword(password),
                displayName = displayName,
                clientVersion = Application.version,
                platform = Application.platform.ToString()
            };

            string jsonData = JsonUtility.ToJson(registrationData);
            
            using (UnityWebRequest request = new UnityWebRequest($"{authServerURL}/register", "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = (int)requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<RegistrationResponse>(request.downloadHandler.text);
                    
                    if (response.success)
                    {
                        OnRegistrationSuccess?.Invoke(response.message);
                        callback?.Invoke(true, response.message);
                        
                        // Auto-login after registration if enabled
                        if (response.autoLogin && !string.IsNullOrEmpty(response.token))
                        {
                            ProcessLoginResponse(response.token, response.session);
                        }
                    }
                    else
                    {
                        OnRegistrationFailed?.Invoke(response.error);
                        callback?.Invoke(false, response.error);
                    }
                }
                else
                {
                    // Offline mode - create local account
                    CreateOfflineAccount(username, email, displayName);
                    OnRegistrationSuccess?.Invoke("Account erstellt (Offline-Modus)");
                    callback?.Invoke(true, "Offline account created");
                }
            }
        }

        private string ValidateRegistration(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || username.Length < 3)
                return "Benutzername muss mindestens 3 Zeichen haben";

            if (username.Length > 20)
                return "Benutzername darf maximal 20 Zeichen haben";

            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                return "Benutzername darf nur Buchstaben, Zahlen und Unterstriche enthalten";

            if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
                return "Ungültige E-Mail-Adresse";

            if (string.IsNullOrEmpty(password) || password.Length < minPasswordLength)
                return $"Passwort muss mindestens {minPasswordLength} Zeichen haben";

            if (requireNumber && !System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
                return "Passwort muss mindestens eine Zahl enthalten";

            if (requireSpecialCharacter && !System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
                return "Passwort muss mindestens ein Sonderzeichen enthalten";

            return null;
        }

        #endregion

        #region Login

        /// <summary>
        /// Login with username/email and password
        /// </summary>
        public void Login(string usernameOrEmail, string password, bool remember = true, Action<bool, string> callback = null)
        {
            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            {
                OnLoginFailed?.Invoke("Benutzername und Passwort erforderlich");
                callback?.Invoke(false, "Username and password required");
                return;
            }

            rememberLogin = remember;
            StartCoroutine(LoginCoroutine(usernameOrEmail, password, callback));
        }

        private IEnumerator LoginCoroutine(string usernameOrEmail, string password, Action<bool, string> callback)
        {
            var loginData = new LoginRequest
            {
                usernameOrEmail = usernameOrEmail,
                passwordHash = HashPassword(password),
                clientVersion = Application.version,
                platform = Application.platform.ToString(),
                deviceId = SystemInfo.deviceUniqueIdentifier
            };

            string jsonData = JsonUtility.ToJson(loginData);

            using (UnityWebRequest request = new UnityWebRequest($"{authServerURL}/login", "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = (int)requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                    if (response.success)
                    {
                        ProcessLoginResponse(response.token, response.session);
                        OnLoginSuccess?.Invoke(currentSession);
                        callback?.Invoke(true, "Login erfolgreich!");
                    }
                    else
                    {
                        OnLoginFailed?.Invoke(response.error);
                        callback?.Invoke(false, response.error);
                    }
                }
                else
                {
                    // Offline mode - try local login
                    if (TryOfflineLogin(usernameOrEmail, password))
                    {
                        OnLoginSuccess?.Invoke(currentSession);
                        callback?.Invoke(true, "Offline-Login erfolgreich!");
                    }
                    else
                    {
                        OnLoginFailed?.Invoke("Verbindung zum Server fehlgeschlagen");
                        callback?.Invoke(false, "Server connection failed");
                    }
                }
            }
        }

        /// <summary>
        /// Login as guest (no account required)
        /// </summary>
        public void LoginAsGuest(Action<bool, string> callback = null)
        {
            string guestId = $"Guest_{UnityEngine.Random.Range(10000, 99999)}";
            
            currentSession = new UserSession
            {
                userId = SystemInfo.deviceUniqueIdentifier,
                username = guestId,
                displayName = guestId,
                isGuest = true,
                loginTime = DateTime.UtcNow,
                accountLevel = 0
            };

            authToken = GenerateLocalToken();
            tokenExpiry = DateTime.UtcNow.AddHours(24);

            OnLoginSuccess?.Invoke(currentSession);
            callback?.Invoke(true, "Als Gast angemeldet");
        }

        private void ProcessLoginResponse(string token, UserSession session)
        {
            authToken = token;
            currentSession = session;
            tokenExpiry = DateTime.UtcNow.AddSeconds(tokenExpiryTime);

            if (rememberLogin)
            {
                SaveSession();
            }

            // Start session refresh
            if (sessionRefreshCoroutine != null)
            {
                StopCoroutine(sessionRefreshCoroutine);
            }
            sessionRefreshCoroutine = StartCoroutine(SessionRefreshLoop());
        }

        #endregion

        #region Logout

        /// <summary>
        /// Logout current user
        /// </summary>
        public void Logout(Action callback = null)
        {
            StartCoroutine(LogoutCoroutine(callback));
        }

        private IEnumerator LogoutCoroutine(Action callback)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                using (UnityWebRequest request = new UnityWebRequest($"{authServerURL}/logout", "POST"))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.timeout = 10;

                    yield return request.SendWebRequest();
                    // We don't care about the result, just clearing locally
                }
            }

            ClearSession();
            OnLogout?.Invoke();
            callback?.Invoke();
        }

        private void ClearSession()
        {
            currentSession = null;
            authToken = null;
            tokenExpiry = DateTime.MinValue;

            if (sessionRefreshCoroutine != null)
            {
                StopCoroutine(sessionRefreshCoroutine);
                sessionRefreshCoroutine = null;
            }

            PlayerPrefs.DeleteKey("EUF_Session");
            PlayerPrefs.DeleteKey("EUF_Token");
            PlayerPrefs.Save();
        }

        #endregion

        #region Password Reset

        /// <summary>
        /// Request password reset email
        /// </summary>
        public void RequestPasswordReset(string email, Action<bool, string> callback = null)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                OnPasswordResetFailed?.Invoke("Ungültige E-Mail-Adresse");
                callback?.Invoke(false, "Invalid email");
                return;
            }

            StartCoroutine(PasswordResetCoroutine(email, callback));
        }

        private IEnumerator PasswordResetCoroutine(string email, Action<bool, string> callback)
        {
            var resetData = new PasswordResetRequest { email = email };
            string jsonData = JsonUtility.ToJson(resetData);

            using (UnityWebRequest request = new UnityWebRequest($"{authServerURL}/reset-password", "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = (int)requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<GenericResponse>(request.downloadHandler.text);

                    if (response.success)
                    {
                        OnPasswordResetSent?.Invoke();
                        callback?.Invoke(true, "E-Mail gesendet");
                    }
                    else
                    {
                        OnPasswordResetFailed?.Invoke(response.error);
                        callback?.Invoke(false, response.error);
                    }
                }
                else
                {
                    OnPasswordResetFailed?.Invoke("Server nicht erreichbar");
                    callback?.Invoke(false, "Server unreachable");
                }
            }
        }

        /// <summary>
        /// Change password (requires current password)
        /// </summary>
        public void ChangePassword(string currentPassword, string newPassword, Action<bool, string> callback = null)
        {
            string validationError = ValidatePassword(newPassword);
            if (!string.IsNullOrEmpty(validationError))
            {
                callback?.Invoke(false, validationError);
                return;
            }

            StartCoroutine(ChangePasswordCoroutine(currentPassword, newPassword, callback));
        }

        private IEnumerator ChangePasswordCoroutine(string currentPassword, string newPassword, Action<bool, string> callback)
        {
            var changeData = new ChangePasswordRequest
            {
                currentPasswordHash = HashPassword(currentPassword),
                newPasswordHash = HashPassword(newPassword)
            };

            string jsonData = JsonUtility.ToJson(changeData);

            using (UnityWebRequest request = new UnityWebRequest($"{authServerURL}/change-password", "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = (int)requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<GenericResponse>(request.downloadHandler.text);
                    callback?.Invoke(response.success, response.success ? "Passwort geändert" : response.error);
                }
                else
                {
                    callback?.Invoke(false, "Server nicht erreichbar");
                }
            }
        }

        private string ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < minPasswordLength)
                return $"Passwort muss mindestens {minPasswordLength} Zeichen haben";

            if (requireNumber && !System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
                return "Passwort muss mindestens eine Zahl enthalten";

            if (requireSpecialCharacter && !System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
                return "Passwort muss mindestens ein Sonderzeichen enthalten";

            return null;
        }

        #endregion

        #region Email Verification

        /// <summary>
        /// Resend verification email
        /// </summary>
        public void ResendVerificationEmail(Action<bool, string> callback = null)
        {
            if (!IsLoggedIn)
            {
                callback?.Invoke(false, "Nicht eingeloggt");
                return;
            }

            StartCoroutine(ResendVerificationCoroutine(callback));
        }

        private IEnumerator ResendVerificationCoroutine(Action<bool, string> callback)
        {
            using (UnityWebRequest request = new UnityWebRequest($"{authServerURL}/resend-verification", "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = (int)requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<GenericResponse>(request.downloadHandler.text);
                    callback?.Invoke(response.success, response.success ? "E-Mail gesendet" : response.error);
                }
                else
                {
                    callback?.Invoke(false, "Server nicht erreichbar");
                }
            }
        }

        /// <summary>
        /// Verify email with code
        /// </summary>
        public void VerifyEmail(string code, Action<bool, string> callback = null)
        {
            StartCoroutine(VerifyEmailCoroutine(code, callback));
        }

        private IEnumerator VerifyEmailCoroutine(string code, Action<bool, string> callback)
        {
            var verifyData = new EmailVerificationRequest { code = code };
            string jsonData = JsonUtility.ToJson(verifyData);

            using (UnityWebRequest request = new UnityWebRequest($"{authServerURL}/verify-email", "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = (int)requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<GenericResponse>(request.downloadHandler.text);
                    
                    if (response.success)
                    {
                        if (currentSession != null)
                            currentSession.emailVerified = true;
                        OnEmailVerified?.Invoke();
                        callback?.Invoke(true, "E-Mail verifiziert");
                    }
                    else
                    {
                        OnEmailVerificationFailed?.Invoke(response.error);
                        callback?.Invoke(false, response.error);
                    }
                }
                else
                {
                    callback?.Invoke(false, "Server nicht erreichbar");
                }
            }
        }

        #endregion

        #region Profile Management

        /// <summary>
        /// Update user profile
        /// </summary>
        public void UpdateProfile(UserProfile profile, Action<bool, string> callback = null)
        {
            if (!IsLoggedIn)
            {
                callback?.Invoke(false, "Nicht eingeloggt");
                return;
            }

            StartCoroutine(UpdateProfileCoroutine(profile, callback));
        }

        private IEnumerator UpdateProfileCoroutine(UserProfile profile, Action<bool, string> callback)
        {
            string jsonData = JsonUtility.ToJson(profile);

            using (UnityWebRequest request = new UnityWebRequest($"{authServerURL}/profile", "PUT"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = (int)requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<ProfileResponse>(request.downloadHandler.text);
                    
                    if (response.success)
                    {
                        OnProfileUpdated?.Invoke(response.profile);
                        callback?.Invoke(true, "Profil aktualisiert");
                    }
                    else
                    {
                        callback?.Invoke(false, response.error);
                    }
                }
                else
                {
                    callback?.Invoke(false, "Server nicht erreichbar");
                }
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        public void GetProfile(Action<UserProfile> callback)
        {
            if (!IsLoggedIn)
            {
                callback?.Invoke(null);
                return;
            }

            StartCoroutine(GetProfileCoroutine(callback));
        }

        private IEnumerator GetProfileCoroutine(Action<UserProfile> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{authServerURL}/profile"))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = (int)requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<ProfileResponse>(request.downloadHandler.text);
                    callback?.Invoke(response.success ? response.profile : null);
                }
                else
                {
                    callback?.Invoke(null);
                }
            }
        }

        #endregion

        #region Session Management

        private IEnumerator SessionRefreshLoop()
        {
            while (IsLoggedIn)
            {
                yield return new WaitForSeconds(sessionRefreshInterval);

                if (DateTime.UtcNow >= tokenExpiry.AddMinutes(-5))
                {
                    yield return RefreshSession();
                }
            }
        }

        private IEnumerator RefreshSession()
        {
            using (UnityWebRequest request = new UnityWebRequest($"{authServerURL}/refresh", "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                request.timeout = (int)requestTimeout;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                    
                    if (response.success)
                    {
                        authToken = response.token;
                        tokenExpiry = DateTime.UtcNow.AddSeconds(tokenExpiryTime);
                        
                        if (response.session != null)
                        {
                            currentSession = response.session;
                        }
                        
                        OnSessionRefreshed?.Invoke(currentSession);
                        
                        if (rememberLogin)
                        {
                            SaveSession();
                        }
                    }
                }
            }
        }

        private void SaveSession()
        {
            if (currentSession != null && rememberLogin)
            {
                string sessionJson = JsonUtility.ToJson(currentSession);
                string encryptedSession = EncryptString(sessionJson);
                PlayerPrefs.SetString("EUF_Session", encryptedSession);
                
                string encryptedToken = EncryptString(authToken);
                PlayerPrefs.SetString("EUF_Token", encryptedToken);
                
                PlayerPrefs.SetString("EUF_TokenExpiry", tokenExpiry.ToBinary().ToString());
                PlayerPrefs.Save();
            }
        }

        private void LoadSavedSession()
        {
            if (PlayerPrefs.HasKey("EUF_Session") && PlayerPrefs.HasKey("EUF_Token"))
            {
                try
                {
                    string encryptedSession = PlayerPrefs.GetString("EUF_Session");
                    string sessionJson = DecryptString(encryptedSession);
                    currentSession = JsonUtility.FromJson<UserSession>(sessionJson);

                    string encryptedToken = PlayerPrefs.GetString("EUF_Token");
                    authToken = DecryptString(encryptedToken);

                    if (PlayerPrefs.HasKey("EUF_TokenExpiry"))
                    {
                        long binary = long.Parse(PlayerPrefs.GetString("EUF_TokenExpiry"));
                        tokenExpiry = DateTime.FromBinary(binary);

                        if (DateTime.UtcNow >= tokenExpiry)
                        {
                            // Token expired, need to re-login
                            ClearSession();
                            return;
                        }
                    }

                    // Start session refresh
                    sessionRefreshCoroutine = StartCoroutine(SessionRefreshLoop());
                }
                catch
                {
                    ClearSession();
                }
            }
        }

        #endregion

        #region Offline Mode

        private void CreateOfflineAccount(string username, string email, string displayName)
        {
            currentSession = new UserSession
            {
                userId = SystemInfo.deviceUniqueIdentifier,
                username = username,
                email = email,
                displayName = displayName,
                isGuest = false,
                isOffline = true,
                loginTime = DateTime.UtcNow,
                accountLevel = 1
            };

            authToken = GenerateLocalToken();
            tokenExpiry = DateTime.UtcNow.AddDays(30);
            
            // Save offline account
            SaveOfflineAccount(username, email, displayName);
        }

        private bool TryOfflineLogin(string usernameOrEmail, string password)
        {
            string savedData = PlayerPrefs.GetString("EUF_OfflineAccount", "");
            if (string.IsNullOrEmpty(savedData))
                return false;

            try
            {
                var offlineAccount = JsonUtility.FromJson<OfflineAccount>(DecryptString(savedData));
                
                if ((offlineAccount.username == usernameOrEmail || offlineAccount.email == usernameOrEmail) &&
                    offlineAccount.passwordHash == HashPassword(password))
                {
                    currentSession = new UserSession
                    {
                        userId = SystemInfo.deviceUniqueIdentifier,
                        username = offlineAccount.username,
                        email = offlineAccount.email,
                        displayName = offlineAccount.displayName,
                        isGuest = false,
                        isOffline = true,
                        loginTime = DateTime.UtcNow,
                        accountLevel = offlineAccount.level
                    };

                    authToken = GenerateLocalToken();
                    tokenExpiry = DateTime.UtcNow.AddDays(30);
                    return true;
                }
            }
            catch { }

            return false;
        }

        private void SaveOfflineAccount(string username, string email, string displayName)
        {
            // Note: In real implementation, we wouldn't store password hash locally
            // This is just for offline demo mode
        }

        private string GenerateLocalToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        #endregion

        #region Security Helpers

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Add salt for security
                string salted = password + "EarthUnderFreelancer_Salt_2024";
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(salted));
                return Convert.ToBase64String(bytes);
            }
        }

        private string EncryptString(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            
            // Simple XOR encryption for local storage
            // In production, use proper encryption
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] key = Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier);
            
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            
            return Convert.ToBase64String(data);
        }

        private string DecryptString(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            
            byte[] data = Convert.FromBase64String(input);
            byte[] key = Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier);
            
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            
            return Encoding.UTF8.GetString(data);
        }

        #endregion
    }

    #region Data Classes

    [Serializable]
    public class UserSession
    {
        public string userId;
        public string username;
        public string email;
        public string displayName;
        public string avatarUrl;
        public int accountLevel;
        public bool isGuest;
        public bool isOffline;
        public bool emailVerified;
        public bool isPremium;
        public DateTime loginTime;
        public DateTime createdAt;
        public string[] roles;
        public string country;
        public string language;
    }

    [Serializable]
    public class UserProfile
    {
        public string displayName;
        public string bio;
        public string avatarUrl;
        public string country;
        public string language;
        public bool showOnlineStatus;
        public bool allowFriendRequests;
        public bool allowMessages;
    }

    [Serializable]
    public class RegistrationRequest
    {
        public string username;
        public string email;
        public string passwordHash;
        public string displayName;
        public string clientVersion;
        public string platform;
    }

    [Serializable]
    public class RegistrationResponse
    {
        public bool success;
        public string message;
        public string error;
        public bool autoLogin;
        public string token;
        public UserSession session;
    }

    [Serializable]
    public class LoginRequest
    {
        public string usernameOrEmail;
        public string passwordHash;
        public string clientVersion;
        public string platform;
        public string deviceId;
    }

    [Serializable]
    public class LoginResponse
    {
        public bool success;
        public string token;
        public UserSession session;
        public string error;
    }

    [Serializable]
    public class PasswordResetRequest
    {
        public string email;
    }

    [Serializable]
    public class ChangePasswordRequest
    {
        public string currentPasswordHash;
        public string newPasswordHash;
    }

    [Serializable]
    public class EmailVerificationRequest
    {
        public string code;
    }

    [Serializable]
    public class GenericResponse
    {
        public bool success;
        public string message;
        public string error;
    }

    [Serializable]
    public class ProfileResponse
    {
        public bool success;
        public UserProfile profile;
        public string error;
    }

    [Serializable]
    public class OfflineAccount
    {
        public string username;
        public string email;
        public string displayName;
        public string passwordHash;
        public int level;
    }

    #endregion
}
