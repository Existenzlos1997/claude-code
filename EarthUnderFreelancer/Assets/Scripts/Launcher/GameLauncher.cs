using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace EarthUnderFreelancer.Launcher
{
    /// <summary>
    /// Professional Game Launcher with news, updates, and server status
    /// </summary>
    public class GameLauncher : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button updateButton;
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private TextMeshProUGUI serverStatusText;
        [SerializeField] private Slider downloadProgressBar;
        [SerializeField] private TextMeshProUGUI downloadStatusText;
        [SerializeField] private Transform newsContainer;
        [SerializeField] private GameObject newsItemPrefab;
        [SerializeField] private Image serverStatusIcon;
        [SerializeField] private Image backgroundImage;
        
        [Header("Settings")]
        [SerializeField] private string gameSceneName = "MainMenu";
        [SerializeField] private string newsApiUrl = "https://api.earthunderfreelancer.com/news";
        [SerializeField] private string serverStatusUrl = "https://api.earthunderfreelancer.com/status";
        [SerializeField] private string updateCheckUrl = "https://api.earthunderfreelancer.com/version";
        [SerializeField] private float refreshInterval = 60f;
        
        [Header("Colors")]
        [SerializeField] private Color onlineColor = Color.green;
        [SerializeField] private Color maintenanceColor = Color.yellow;
        [SerializeField] private Color offlineColor = Color.red;
        
        private string currentVersion;
        private string latestVersion;
        private bool updateAvailable;
        private bool isDownloading;
        private List<NewsItem> newsItems = new List<NewsItem>();
        
        [Serializable]
        public class NewsItem
        {
            public string id;
            public string title;
            public string content;
            public string imageUrl;
            public string date;
            public string category;
            public string link;
        }
        
        [Serializable]
        public class NewsResponse
        {
            public List<NewsItem> news;
        }
        
        [Serializable]
        public class ServerStatus
        {
            public string status;
            public int playerCount;
            public int maxPlayers;
            public string message;
            public List<ServerInfo> servers;
        }
        
        [Serializable]
        public class ServerInfo
        {
            public string name;
            public string region;
            public string status;
            public int players;
            public int ping;
        }
        
        [Serializable]
        public class VersionInfo
        {
            public string latest;
            public string minimum;
            public string downloadUrl;
            public string changelog;
            public long fileSize;
            public bool forceUpdate;
        }
        
        private void Awake()
        {
            currentVersion = Application.version;
            
            if (versionText != null)
                versionText.text = $"v{currentVersion}";
        }
        
        private void Start()
        {
            SetupButtons();
            StartCoroutine(InitializeLauncher());
        }
        
        private void SetupButtons()
        {
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);
                
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);
                
            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClicked);
                
            if (updateButton != null)
            {
                updateButton.onClick.AddListener(OnUpdateClicked);
                updateButton.gameObject.SetActive(false);
            }
            
            if (downloadProgressBar != null)
                downloadProgressBar.gameObject.SetActive(false);
        }
        
        private IEnumerator InitializeLauncher()
        {
            // Check for updates
            yield return StartCoroutine(CheckForUpdates());
            
            // Get server status
            yield return StartCoroutine(FetchServerStatus());
            
            // Load news
            yield return StartCoroutine(FetchNews());
            
            // Start refresh loop
            StartCoroutine(RefreshLoop());
        }
        
        private IEnumerator RefreshLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(refreshInterval);
                yield return StartCoroutine(FetchServerStatus());
            }
        }
        
        private IEnumerator CheckForUpdates()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(updateCheckUrl))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        VersionInfo versionInfo = JsonUtility.FromJson<VersionInfo>(www.downloadHandler.text);
                        latestVersion = versionInfo.latest;
                        
                        if (CompareVersions(currentVersion, latestVersion) < 0)
                        {
                            updateAvailable = true;
                            
                            if (updateButton != null)
                            {
                                updateButton.gameObject.SetActive(true);
                                
                                if (versionInfo.forceUpdate)
                                {
                                    playButton.interactable = false;
                                    ShowMessage("Update Required", $"Please update to version {latestVersion} to continue playing.");
                                }
                            }
                            
                            if (versionText != null)
                                versionText.text = $"v{currentVersion} → v{latestVersion}";
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to parse version info: {e.Message}");
                    }
                }
            }
        }
        
        private IEnumerator FetchServerStatus()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(serverStatusUrl))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        ServerStatus status = JsonUtility.FromJson<ServerStatus>(www.downloadHandler.text);
                        UpdateServerStatusUI(status);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to parse server status: {e.Message}");
                        SetServerOffline();
                    }
                }
                else
                {
                    SetServerOffline();
                }
            }
        }
        
        private void UpdateServerStatusUI(ServerStatus status)
        {
            if (playerCountText != null)
                playerCountText.text = $"{status.playerCount:N0} / {status.maxPlayers:N0} Players Online";
            
            if (serverStatusText != null)
            {
                serverStatusText.text = status.status.ToUpper();
                
                switch (status.status.ToLower())
                {
                    case "online":
                        serverStatusText.color = onlineColor;
                        if (serverStatusIcon != null) serverStatusIcon.color = onlineColor;
                        playButton.interactable = !updateAvailable || !IsForceUpdate();
                        break;
                    case "maintenance":
                        serverStatusText.color = maintenanceColor;
                        if (serverStatusIcon != null) serverStatusIcon.color = maintenanceColor;
                        playButton.interactable = false;
                        break;
                    default:
                        serverStatusText.color = offlineColor;
                        if (serverStatusIcon != null) serverStatusIcon.color = offlineColor;
                        playButton.interactable = false;
                        break;
                }
            }
        }
        
        private void SetServerOffline()
        {
            if (serverStatusText != null)
            {
                serverStatusText.text = "OFFLINE";
                serverStatusText.color = offlineColor;
            }
            
            if (playerCountText != null)
                playerCountText.text = "Server unavailable";
            
            if (serverStatusIcon != null)
                serverStatusIcon.color = offlineColor;
        }
        
        private IEnumerator FetchNews()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(newsApiUrl))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        NewsResponse response = JsonUtility.FromJson<NewsResponse>(www.downloadHandler.text);
                        newsItems = response.news;
                        PopulateNews();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to parse news: {e.Message}");
                    }
                }
            }
        }
        
        private void PopulateNews()
        {
            if (newsContainer == null || newsItemPrefab == null) return;
            
            // Clear existing items
            foreach (Transform child in newsContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Add news items
            foreach (var item in newsItems)
            {
                GameObject newsObj = Instantiate(newsItemPrefab, newsContainer);
                LauncherNewsItem newsComponent = newsObj.GetComponent<LauncherNewsItem>();
                if (newsComponent != null)
                {
                    newsComponent.Setup(item);
                }
            }
        }
        
        private void OnPlayClicked()
        {
            if (isDownloading) return;
            
            // Save launcher settings
            SaveLauncherSettings();
            
            // Load game
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }
        
        private void OnSettingsClicked()
        {
            // Open settings panel
            LauncherSettings settings = FindObjectOfType<LauncherSettings>();
            if (settings != null)
            {
                settings.Open();
            }
        }
        
        private void OnUpdateClicked()
        {
            if (isDownloading) return;
            
            StartCoroutine(DownloadUpdate());
        }
        
        private IEnumerator DownloadUpdate()
        {
            isDownloading = true;
            playButton.interactable = false;
            updateButton.interactable = false;
            
            if (downloadProgressBar != null)
                downloadProgressBar.gameObject.SetActive(true);
            
            // Get update info
            VersionInfo updateInfo = null;
            using (UnityWebRequest www = UnityWebRequest.Get(updateCheckUrl))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    updateInfo = JsonUtility.FromJson<VersionInfo>(www.downloadHandler.text);
                }
            }
            
            if (updateInfo == null || string.IsNullOrEmpty(updateInfo.downloadUrl))
            {
                ShowMessage("Update Error", "Could not retrieve update information.");
                isDownloading = false;
                updateButton.interactable = true;
                yield break;
            }
            
            // Download update
            string tempPath = Path.Combine(Application.temporaryCachePath, "update.zip");
            
            using (UnityWebRequest www = UnityWebRequest.Get(updateInfo.downloadUrl))
            {
                var operation = www.SendWebRequest();
                
                while (!operation.isDone)
                {
                    if (downloadProgressBar != null)
                        downloadProgressBar.value = www.downloadProgress;
                    
                    if (downloadStatusText != null)
                    {
                        float downloadedMB = www.downloadedBytes / (1024f * 1024f);
                        float totalMB = updateInfo.fileSize / (1024f * 1024f);
                        downloadStatusText.text = $"Downloading: {downloadedMB:F1} MB / {totalMB:F1} MB ({www.downloadProgress * 100f:F0}%)";
                    }
                    
                    yield return null;
                }
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(tempPath, www.downloadHandler.data);
                    
                    if (downloadStatusText != null)
                        downloadStatusText.text = "Installing update...";
                    
                    // Launch updater and exit
                    LaunchUpdater(tempPath);
                }
                else
                {
                    ShowMessage("Download Error", $"Failed to download update: {www.error}");
                    isDownloading = false;
                    updateButton.interactable = true;
                }
            }
        }
        
        private void LaunchUpdater(string updatePath)
        {
            string updaterPath = Path.Combine(Application.dataPath, "..", "updater.exe");
            
            if (File.Exists(updaterPath))
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = updaterPath,
                    Arguments = $"\"{updatePath}\" \"{Application.dataPath}\" \"{Application.productName}\"",
                    UseShellExecute = true
                };
                
                System.Diagnostics.Process.Start(startInfo);
                Application.Quit();
            }
            else
            {
                ShowMessage("Updater Not Found", "The updater executable was not found. Please reinstall the game.");
            }
        }
        
        private void OnExitClicked()
        {
            SaveLauncherSettings();
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
        
        private void SaveLauncherSettings()
        {
            PlayerPrefs.SetString("LastVersion", currentVersion);
            PlayerPrefs.SetString("LastLoginTime", DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
        
        private int CompareVersions(string v1, string v2)
        {
            try
            {
                Version ver1 = new Version(v1);
                Version ver2 = new Version(v2);
                return ver1.CompareTo(ver2);
            }
            catch
            {
                return string.Compare(v1, v2, StringComparison.Ordinal);
            }
        }
        
        private bool IsForceUpdate()
        {
            // Would be determined from version check
            return false;
        }
        
        private void ShowMessage(string title, string message)
        {
            Debug.Log($"{title}: {message}");
            // Would show UI dialog
        }
    }
    
    /// <summary>
    /// News item display component
    /// </summary>
    public class LauncherNewsItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI categoryText;
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private Button readMoreButton;
        
        private GameLauncher.NewsItem newsData;
        
        public void Setup(GameLauncher.NewsItem item)
        {
            newsData = item;
            
            if (titleText != null)
                titleText.text = item.title;
            
            if (dateText != null)
                dateText.text = item.date;
            
            if (categoryText != null)
            {
                categoryText.text = item.category;
                categoryText.color = GetCategoryColor(item.category);
            }
            
            if (readMoreButton != null)
                readMoreButton.onClick.AddListener(OnReadMore);
            
            if (!string.IsNullOrEmpty(item.imageUrl) && thumbnailImage != null)
            {
                StartCoroutine(LoadThumbnail(item.imageUrl));
            }
        }
        
        private Color GetCategoryColor(string category)
        {
            switch (category?.ToLower())
            {
                case "update": return Color.cyan;
                case "event": return Color.yellow;
                case "maintenance": return Color.red;
                case "news": return Color.white;
                default: return Color.gray;
            }
        }
        
        private IEnumerator LoadThumbnail(string url)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    thumbnailImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                }
            }
        }
        
        private void OnReadMore()
        {
            if (!string.IsNullOrEmpty(newsData?.link))
            {
                Application.OpenURL(newsData.link);
            }
        }
    }
    
    /// <summary>
    /// Launcher settings panel
    /// </summary>
    public class LauncherSettings : MonoBehaviour
    {
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Toggle vsyncToggle;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private TMP_Dropdown languageDropdown;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button cancelButton;
        
        private Resolution[] resolutions;
        
        private void Start()
        {
            SetupUI();
            LoadSettings();
        }
        
        private void SetupUI()
        {
            // Quality settings
            if (qualityDropdown != null)
            {
                qualityDropdown.ClearOptions();
                qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
                qualityDropdown.value = QualitySettings.GetQualityLevel();
            }
            
            // Resolutions
            if (resolutionDropdown != null)
            {
                resolutions = Screen.resolutions;
                resolutionDropdown.ClearOptions();
                
                List<string> options = new List<string>();
                int currentIndex = 0;
                
                for (int i = 0; i < resolutions.Length; i++)
                {
                    string option = $"{resolutions[i].width} x {resolutions[i].height} @ {resolutions[i].refreshRate}Hz";
                    options.Add(option);
                    
                    if (resolutions[i].width == Screen.currentResolution.width &&
                        resolutions[i].height == Screen.currentResolution.height)
                    {
                        currentIndex = i;
                    }
                }
                
                resolutionDropdown.AddOptions(options);
                resolutionDropdown.value = currentIndex;
            }
            
            // Fullscreen
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = Screen.fullScreen;
            
            // VSync
            if (vsyncToggle != null)
                vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
            
            // Volume
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            
            // Language
            if (languageDropdown != null)
            {
                languageDropdown.ClearOptions();
                languageDropdown.AddOptions(new List<string> {
                    "English", "Deutsch", "Français", "Español", "Italiano",
                    "Português", "Русский", "Polski", "日本語", "한국어",
                    "简体中文", "繁體中文", "Türkçe", "العربية"
                });
            }
            
            // Buttons
            if (applyButton != null)
                applyButton.onClick.AddListener(ApplySettings);
            
            if (cancelButton != null)
                cancelButton.onClick.AddListener(Close);
        }
        
        public void Open()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }
        
        public void Close()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }
        
        private void LoadSettings()
        {
            // Load from PlayerPrefs
        }
        
        private void ApplySettings()
        {
            // Quality
            if (qualityDropdown != null)
                QualitySettings.SetQualityLevel(qualityDropdown.value);
            
            // Resolution
            if (resolutionDropdown != null && resolutions != null)
            {
                Resolution res = resolutions[resolutionDropdown.value];
                Screen.SetResolution(res.width, res.height, fullscreenToggle?.isOn ?? true);
            }
            
            // VSync
            QualitySettings.vSyncCount = (vsyncToggle?.isOn ?? true) ? 1 : 0;
            
            // Volume
            if (masterVolumeSlider != null)
            {
                PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
                AudioListener.volume = masterVolumeSlider.value;
            }
            
            PlayerPrefs.Save();
            Close();
        }
    }
}
