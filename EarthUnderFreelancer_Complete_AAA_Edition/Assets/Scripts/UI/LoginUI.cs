using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EarthUnderFreelancer.Account;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Complete Login/Registration UI with all account management features
    /// </summary>
    public class LoginUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject forgotPasswordPanel;
        [SerializeField] private GameObject profilePanel;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject errorPanel;

        [Header("Login Fields")]
        [SerializeField] private TMP_InputField loginUsernameField;
        [SerializeField] private TMP_InputField loginPasswordField;
        [SerializeField] private Toggle rememberMeToggle;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button guestButton;
        [SerializeField] private Button toRegisterButton;
        [SerializeField] private Button forgotPasswordButton;

        [Header("Register Fields")]
        [SerializeField] private TMP_InputField registerUsernameField;
        [SerializeField] private TMP_InputField registerEmailField;
        [SerializeField] private TMP_InputField registerPasswordField;
        [SerializeField] private TMP_InputField registerConfirmPasswordField;
        [SerializeField] private TMP_InputField registerDisplayNameField;
        [SerializeField] private Toggle termsToggle;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button toLoginButton;

        [Header("Forgot Password Fields")]
        [SerializeField] private TMP_InputField forgotEmailField;
        [SerializeField] private Button sendResetButton;
        [SerializeField] private Button backToLoginButton;

        [Header("Profile Fields")]
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text displayNameText;
        [SerializeField] private TMP_Text emailText;
        [SerializeField] private TMP_Text accountLevelText;
        [SerializeField] private Image avatarImage;
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button editProfileButton;
        [SerializeField] private Button playButton;

        [Header("Error/Message")]
        [SerializeField] private TMP_Text errorText;
        [SerializeField] private TMP_Text successText;
        [SerializeField] private Button closeErrorButton;

        [Header("Password Requirements Display")]
        [SerializeField] private Image lengthCheckIcon;
        [SerializeField] private Image numberCheckIcon;
        [SerializeField] private Image specialCheckIcon;
        [SerializeField] private Color validColor = Color.green;
        [SerializeField] private Color invalidColor = Color.red;

        [Header("Settings")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string gameScene = "GameScene";

        private AccountSystem accountSystem;

        private void Awake()
        {
            // Create UI elements if not assigned
            CreateUIIfNeeded();
        }

        private void Start()
        {
            accountSystem = AccountSystem.Instance;
            
            if (accountSystem == null)
            {
                // Create AccountSystem if it doesn't exist
                GameObject accountObj = new GameObject("AccountSystem");
                accountSystem = accountObj.AddComponent<AccountSystem>();
            }

            SetupEventListeners();
            SetupAccountSystemEvents();

            // Check if already logged in
            if (accountSystem.IsLoggedIn)
            {
                ShowProfilePanel();
            }
            else
            {
                ShowLoginPanel();
            }
        }

        private void CreateUIIfNeeded()
        {
            if (loginPanel == null)
            {
                // Create basic UI structure programmatically
                CreateUIStructure();
            }
        }

        private void CreateUIStructure()
        {
            // Create Canvas if needed
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                gameObject.AddComponent<CanvasScaler>();
                gameObject.AddComponent<GraphicRaycaster>();
            }

            // Create main container
            GameObject container = CreatePanel("LoginContainer", transform);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.sizeDelta = Vector2.zero;

            // Create Login Panel
            loginPanel = CreatePanel("LoginPanel", container.transform);
            SetupLoginPanel();

            // Create Register Panel
            registerPanel = CreatePanel("RegisterPanel", container.transform);
            SetupRegisterPanel();
            registerPanel.SetActive(false);

            // Create Forgot Password Panel
            forgotPasswordPanel = CreatePanel("ForgotPasswordPanel", container.transform);
            SetupForgotPasswordPanel();
            forgotPasswordPanel.SetActive(false);

            // Create Profile Panel
            profilePanel = CreatePanel("ProfilePanel", container.transform);
            SetupProfilePanel();
            profilePanel.SetActive(false);

            // Create Loading Panel
            loadingPanel = CreatePanel("LoadingPanel", container.transform);
            SetupLoadingPanel();
            loadingPanel.SetActive(false);

            // Create Error Panel
            errorPanel = CreatePanel("ErrorPanel", container.transform);
            SetupErrorPanel();
            errorPanel.SetActive(false);
        }

        private GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            return panel;
        }

        private void SetupLoginPanel()
        {
            // Title
            CreateText("EarthUnderFreelancer", loginPanel.transform, new Vector2(0, 200), 48);
            CreateText("Anmelden", loginPanel.transform, new Vector2(0, 120), 32);

            // Username field
            loginUsernameField = CreateInputField("Benutzername / E-Mail", loginPanel.transform, new Vector2(0, 50));

            // Password field
            loginPasswordField = CreateInputField("Passwort", loginPanel.transform, new Vector2(0, -20), true);

            // Remember me toggle
            rememberMeToggle = CreateToggle("Angemeldet bleiben", loginPanel.transform, new Vector2(-50, -70));

            // Login button
            loginButton = CreateButton("Anmelden", loginPanel.transform, new Vector2(0, -130), new Color(0.2f, 0.6f, 0.2f));

            // Guest button
            guestButton = CreateButton("Als Gast spielen", loginPanel.transform, new Vector2(0, -190), new Color(0.3f, 0.3f, 0.5f));

            // Register link
            toRegisterButton = CreateButton("Neues Konto erstellen", loginPanel.transform, new Vector2(0, -250), new Color(0.2f, 0.4f, 0.6f));

            // Forgot password link
            forgotPasswordButton = CreateButton("Passwort vergessen?", loginPanel.transform, new Vector2(0, -300), new Color(0.4f, 0.4f, 0.4f));
        }

        private void SetupRegisterPanel()
        {
            // Title
            CreateText("Konto erstellen", registerPanel.transform, new Vector2(0, 220), 32);

            // Username field
            registerUsernameField = CreateInputField("Benutzername", registerPanel.transform, new Vector2(0, 150));

            // Email field
            registerEmailField = CreateInputField("E-Mail-Adresse", registerPanel.transform, new Vector2(0, 80));

            // Display name field
            registerDisplayNameField = CreateInputField("Anzeigename (optional)", registerPanel.transform, new Vector2(0, 10));

            // Password field
            registerPasswordField = CreateInputField("Passwort", registerPanel.transform, new Vector2(0, -60), true);

            // Confirm password field
            registerConfirmPasswordField = CreateInputField("Passwort bestätigen", registerPanel.transform, new Vector2(0, -130), true);

            // Terms toggle
            termsToggle = CreateToggle("Ich akzeptiere die Nutzungsbedingungen", registerPanel.transform, new Vector2(0, -180));

            // Register button
            registerButton = CreateButton("Registrieren", registerPanel.transform, new Vector2(0, -240), new Color(0.2f, 0.6f, 0.2f));

            // Back to login
            toLoginButton = CreateButton("Zurück zur Anmeldung", registerPanel.transform, new Vector2(0, -300), new Color(0.4f, 0.4f, 0.4f));
        }

        private void SetupForgotPasswordPanel()
        {
            // Title
            CreateText("Passwort zurücksetzen", forgotPasswordPanel.transform, new Vector2(0, 100), 32);

            // Email field
            forgotEmailField = CreateInputField("E-Mail-Adresse", forgotPasswordPanel.transform, new Vector2(0, 20));

            // Send button
            sendResetButton = CreateButton("Link senden", forgotPasswordPanel.transform, new Vector2(0, -50), new Color(0.2f, 0.6f, 0.2f));

            // Back button
            backToLoginButton = CreateButton("Zurück", forgotPasswordPanel.transform, new Vector2(0, -110), new Color(0.4f, 0.4f, 0.4f));
        }

        private void SetupProfilePanel()
        {
            // Title
            CreateText("Profil", profilePanel.transform, new Vector2(0, 200), 32);

            // Username
            usernameText = CreateText("Username", profilePanel.transform, new Vector2(0, 100), 24);

            // Display name
            displayNameText = CreateText("Display Name", profilePanel.transform, new Vector2(0, 60), 28);

            // Email
            emailText = CreateText("email@example.com", profilePanel.transform, new Vector2(0, 20), 18);

            // Account level
            accountLevelText = CreateText("Level 1", profilePanel.transform, new Vector2(0, -20), 20);

            // Play button
            playButton = CreateButton("SPIELEN", profilePanel.transform, new Vector2(0, -100), new Color(0.2f, 0.7f, 0.2f));
            
            // Expand play button
            RectTransform playRect = playButton.GetComponent<RectTransform>();
            playRect.sizeDelta = new Vector2(300, 80);

            // Edit profile button
            editProfileButton = CreateButton("Profil bearbeiten", profilePanel.transform, new Vector2(0, -180), new Color(0.3f, 0.4f, 0.5f));

            // Logout button
            logoutButton = CreateButton("Abmelden", profilePanel.transform, new Vector2(0, -240), new Color(0.6f, 0.2f, 0.2f));
        }

        private void SetupLoadingPanel()
        {
            Image bg = loadingPanel.GetComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);

            CreateText("Bitte warten...", loadingPanel.transform, new Vector2(0, 0), 28);
        }

        private void SetupErrorPanel()
        {
            Image bg = errorPanel.GetComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.9f);

            errorText = CreateText("Fehler", errorPanel.transform, new Vector2(0, 20), 24);
            closeErrorButton = CreateButton("OK", errorPanel.transform, new Vector2(0, -50), new Color(0.5f, 0.3f, 0.3f));
        }

        private TMP_Text CreateText(string text, Transform parent, Vector2 position, int fontSize)
        {
            GameObject textObj = new GameObject("Text_" + text);
            textObj.transform.SetParent(parent);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(500, 60);

            TMP_Text tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = text;
            tmpText.fontSize = fontSize;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.white;

            return tmpText;
        }

        private TMP_InputField CreateInputField(string placeholder, Transform parent, Vector2 position, bool isPassword = false)
        {
            GameObject fieldObj = new GameObject("InputField_" + placeholder);
            fieldObj.transform.SetParent(parent);

            RectTransform rect = fieldObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(350, 50);

            Image bg = fieldObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.25f);

            // Text area
            GameObject textArea = new GameObject("Text Area");
            textArea.transform.SetParent(fieldObj.transform);
            RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.sizeDelta = new Vector2(-20, -10);
            textAreaRect.anchoredPosition = Vector2.zero;

            // Placeholder
            GameObject placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(textArea.transform);
            RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.sizeDelta = Vector2.zero;
            placeholderRect.anchoredPosition = Vector2.zero;

            TMP_Text placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderText.text = placeholder;
            placeholderText.fontSize = 18;
            placeholderText.fontStyle = FontStyles.Italic;
            placeholderText.color = new Color(0.6f, 0.6f, 0.6f);
            placeholderText.alignment = TextAlignmentOptions.MidlineLeft;

            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(textArea.transform);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            TMP_Text inputText = textObj.AddComponent<TextMeshProUGUI>();
            inputText.fontSize = 20;
            inputText.color = Color.white;
            inputText.alignment = TextAlignmentOptions.MidlineLeft;

            TMP_InputField inputField = fieldObj.AddComponent<TMP_InputField>();
            inputField.textViewport = textAreaRect;
            inputField.textComponent = inputText;
            inputField.placeholder = placeholderText;
            
            if (isPassword)
            {
                inputField.contentType = TMP_InputField.ContentType.Password;
            }

            return inputField;
        }

        private Button CreateButton(string text, Transform parent, Vector2 position, Color color)
        {
            GameObject buttonObj = new GameObject("Button_" + text);
            buttonObj.transform.SetParent(parent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(250, 50);

            Image bg = buttonObj.AddComponent<Image>();
            bg.color = color;

            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = bg;

            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            TMP_Text buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = text;
            buttonText.fontSize = 20;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;

            return button;
        }

        private Toggle CreateToggle(string label, Transform parent, Vector2 position)
        {
            GameObject toggleObj = new GameObject("Toggle_" + label);
            toggleObj.transform.SetParent(parent);

            RectTransform rect = toggleObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(300, 30);

            Toggle toggle = toggleObj.AddComponent<Toggle>();

            // Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(toggleObj.transform);
            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.5f);
            bgRect.anchorMax = new Vector2(0, 0.5f);
            bgRect.anchoredPosition = new Vector2(15, 0);
            bgRect.sizeDelta = new Vector2(24, 24);

            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.35f);

            // Checkmark
            GameObject checkObj = new GameObject("Checkmark");
            checkObj.transform.SetParent(bgObj.transform);
            RectTransform checkRect = checkObj.AddComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.sizeDelta = new Vector2(-6, -6);
            checkRect.anchoredPosition = Vector2.zero;

            Image checkImage = checkObj.AddComponent<Image>();
            checkImage.color = new Color(0.3f, 0.8f, 0.3f);

            toggle.targetGraphic = bgImage;
            toggle.graphic = checkImage;

            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(toggleObj.transform);
            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(1, 0.5f);
            labelRect.anchoredPosition = new Vector2(20, 0);
            labelRect.sizeDelta = new Vector2(-40, 30);

            TMP_Text labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 16;
            labelText.alignment = TextAlignmentOptions.MidlineLeft;
            labelText.color = Color.white;

            return toggle;
        }

        private void SetupEventListeners()
        {
            // Login panel
            if (loginButton != null)
                loginButton.onClick.AddListener(OnLoginClicked);
            if (guestButton != null)
                guestButton.onClick.AddListener(OnGuestClicked);
            if (toRegisterButton != null)
                toRegisterButton.onClick.AddListener(ShowRegisterPanel);
            if (forgotPasswordButton != null)
                forgotPasswordButton.onClick.AddListener(ShowForgotPasswordPanel);

            // Register panel
            if (registerButton != null)
                registerButton.onClick.AddListener(OnRegisterClicked);
            if (toLoginButton != null)
                toLoginButton.onClick.AddListener(ShowLoginPanel);

            // Forgot password panel
            if (sendResetButton != null)
                sendResetButton.onClick.AddListener(OnSendResetClicked);
            if (backToLoginButton != null)
                backToLoginButton.onClick.AddListener(ShowLoginPanel);

            // Profile panel
            if (logoutButton != null)
                logoutButton.onClick.AddListener(OnLogoutClicked);
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);

            // Error panel
            if (closeErrorButton != null)
                closeErrorButton.onClick.AddListener(HideErrorPanel);

            // Password validation
            if (registerPasswordField != null)
                registerPasswordField.onValueChanged.AddListener(ValidatePasswordRequirements);
        }

        private void SetupAccountSystemEvents()
        {
            if (accountSystem == null) return;

            accountSystem.OnLoginSuccess += OnLoginSuccess;
            accountSystem.OnLoginFailed += OnLoginFailed;
            accountSystem.OnLogout += OnLogoutSuccess;
            accountSystem.OnRegistrationSuccess += OnRegistrationSuccess;
            accountSystem.OnRegistrationFailed += OnRegistrationFailed;
            accountSystem.OnPasswordResetSent += OnPasswordResetSuccess;
            accountSystem.OnPasswordResetFailed += OnPasswordResetFailed;
        }

        private void OnDestroy()
        {
            if (accountSystem != null)
            {
                accountSystem.OnLoginSuccess -= OnLoginSuccess;
                accountSystem.OnLoginFailed -= OnLoginFailed;
                accountSystem.OnLogout -= OnLogoutSuccess;
                accountSystem.OnRegistrationSuccess -= OnRegistrationSuccess;
                accountSystem.OnRegistrationFailed -= OnRegistrationFailed;
                accountSystem.OnPasswordResetSent -= OnPasswordResetSuccess;
                accountSystem.OnPasswordResetFailed -= OnPasswordResetFailed;
            }
        }

        #region Panel Navigation

        private void ShowLoginPanel()
        {
            HideAllPanels();
            if (loginPanel != null) loginPanel.SetActive(true);
        }

        private void ShowRegisterPanel()
        {
            HideAllPanels();
            if (registerPanel != null) registerPanel.SetActive(true);
        }

        private void ShowForgotPasswordPanel()
        {
            HideAllPanels();
            if (forgotPasswordPanel != null) forgotPasswordPanel.SetActive(true);
        }

        private void ShowProfilePanel()
        {
            HideAllPanels();
            if (profilePanel != null)
            {
                profilePanel.SetActive(true);
                UpdateProfileDisplay();
            }
        }

        private void ShowLoading()
        {
            if (loadingPanel != null) loadingPanel.SetActive(true);
        }

        private void HideLoading()
        {
            if (loadingPanel != null) loadingPanel.SetActive(false);
        }

        private void ShowError(string message)
        {
            if (errorPanel != null && errorText != null)
            {
                errorText.text = message;
                errorPanel.SetActive(true);
            }
        }

        private void HideErrorPanel()
        {
            if (errorPanel != null) errorPanel.SetActive(false);
        }

        private void HideAllPanels()
        {
            if (loginPanel != null) loginPanel.SetActive(false);
            if (registerPanel != null) registerPanel.SetActive(false);
            if (forgotPasswordPanel != null) forgotPasswordPanel.SetActive(false);
            if (profilePanel != null) profilePanel.SetActive(false);
        }

        #endregion

        #region Button Handlers

        private void OnLoginClicked()
        {
            string username = loginUsernameField?.text ?? "";
            string password = loginPasswordField?.text ?? "";
            bool remember = rememberMeToggle?.isOn ?? true;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Bitte Benutzername und Passwort eingeben");
                return;
            }

            ShowLoading();
            accountSystem?.Login(username, password, remember);
        }

        private void OnGuestClicked()
        {
            ShowLoading();
            accountSystem?.LoginAsGuest();
        }

        private void OnRegisterClicked()
        {
            string username = registerUsernameField?.text ?? "";
            string email = registerEmailField?.text ?? "";
            string password = registerPasswordField?.text ?? "";
            string confirmPassword = registerConfirmPasswordField?.text ?? "";
            string displayName = registerDisplayNameField?.text ?? "";

            if (password != confirmPassword)
            {
                ShowError("Passwörter stimmen nicht überein");
                return;
            }

            if (termsToggle != null && !termsToggle.isOn)
            {
                ShowError("Bitte akzeptieren Sie die Nutzungsbedingungen");
                return;
            }

            ShowLoading();
            accountSystem?.Register(username, email, password, displayName);
        }

        private void OnSendResetClicked()
        {
            string email = forgotEmailField?.text ?? "";

            if (string.IsNullOrEmpty(email))
            {
                ShowError("Bitte E-Mail-Adresse eingeben");
                return;
            }

            ShowLoading();
            accountSystem?.RequestPasswordReset(email);
        }

        private void OnLogoutClicked()
        {
            accountSystem?.Logout();
        }

        private void OnPlayClicked()
        {
            // Load game scene
            if (!string.IsNullOrEmpty(gameScene))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(gameScene);
            }
        }

        private void ValidatePasswordRequirements(string password)
        {
            if (lengthCheckIcon != null)
                lengthCheckIcon.color = password.Length >= 8 ? validColor : invalidColor;

            if (numberCheckIcon != null)
                numberCheckIcon.color = System.Text.RegularExpressions.Regex.IsMatch(password, @"\d") ? validColor : invalidColor;

            if (specialCheckIcon != null)
                specialCheckIcon.color = System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]") ? validColor : invalidColor;
        }

        #endregion

        #region Account System Event Handlers

        private void OnLoginSuccess(UserSession session)
        {
            HideLoading();
            ShowProfilePanel();
        }

        private void OnLoginFailed(string error)
        {
            HideLoading();
            ShowError(error);
        }

        private void OnLogoutSuccess()
        {
            ShowLoginPanel();
        }

        private void OnRegistrationSuccess(string message)
        {
            HideLoading();
            
            if (accountSystem.IsLoggedIn)
            {
                ShowProfilePanel();
            }
            else
            {
                ShowLoginPanel();
                // Show success message
            }
        }

        private void OnRegistrationFailed(string error)
        {
            HideLoading();
            ShowError(error);
        }

        private void OnPasswordResetSuccess()
        {
            HideLoading();
            ShowLoginPanel();
            // Show success message
        }

        private void OnPasswordResetFailed(string error)
        {
            HideLoading();
            ShowError(error);
        }

        #endregion

        private void UpdateProfileDisplay()
        {
            if (accountSystem?.CurrentSession == null) return;

            var session = accountSystem.CurrentSession;

            if (usernameText != null)
                usernameText.text = $"@{session.username}";

            if (displayNameText != null)
                displayNameText.text = session.displayName;

            if (emailText != null)
                emailText.text = session.email ?? "";

            if (accountLevelText != null)
                accountLevelText.text = $"Level {session.accountLevel}";
        }
    }
}
