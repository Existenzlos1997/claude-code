using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace EarthUnderFreelancer.AutoUpdate
{
    /// <summary>
    /// Automatic game update system with delta patching
    /// </summary>
    public class AutoUpdateSystem : MonoBehaviour
    {
        public static AutoUpdateSystem Instance { get; private set; }
        
        [Header("Configuration")]
        [SerializeField] private string updateServerUrl = "https://updates.earthunderfreelancer.com";
        [SerializeField] private string manifestEndpoint = "/manifest.json";
        [SerializeField] private string patchEndpoint = "/patches/";
        [SerializeField] private bool checkOnStart = true;
        [SerializeField] private float checkInterval = 3600f; // 1 hour
        
        [Header("Events")]
        public event Action<UpdateInfo> OnUpdateAvailable;
        public event Action<float, string> OnDownloadProgress;
        public event Action<string> OnUpdateComplete;
        public event Action<string> OnUpdateError;
        
        private string localManifestPath;
        private string downloadPath;
        private UpdateManifest currentManifest;
        private UpdateManifest serverManifest;
        private bool isUpdating;
        
        [Serializable]
        public class UpdateManifest
        {
            public string version;
            public string buildDate;
            public long buildNumber;
            public List<FileEntry> files;
            public List<PatchInfo> patches;
        }
        
        [Serializable]
        public class FileEntry
        {
            public string path;
            public string hash;
            public long size;
            public bool compressed;
        }
        
        [Serializable]
        public class PatchInfo
        {
            public string fromVersion;
            public string toVersion;
            public string patchFile;
            public string hash;
            public long size;
        }
        
        [Serializable]
        public class UpdateInfo
        {
            public string currentVersion;
            public string newVersion;
            public long downloadSize;
            public string changelog;
            public bool requiresRestart;
            public bool forceUpdate;
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                localManifestPath = Path.Combine(Application.persistentDataPath, "manifest.json");
                downloadPath = Path.Combine(Application.persistentDataPath, "Downloads");
                
                LoadLocalManifest();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            if (checkOnStart)
            {
                StartCoroutine(CheckForUpdatesRoutine());
            }
            
            StartCoroutine(PeriodicUpdateCheck());
        }
        
        private IEnumerator PeriodicUpdateCheck()
        {
            while (true)
            {
                yield return new WaitForSeconds(checkInterval);
                yield return StartCoroutine(CheckForUpdatesRoutine());
            }
        }
        
        private void LoadLocalManifest()
        {
            if (File.Exists(localManifestPath))
            {
                try
                {
                    string json = File.ReadAllText(localManifestPath);
                    currentManifest = JsonUtility.FromJson<UpdateManifest>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load local manifest: {e.Message}");
                    currentManifest = CreateDefaultManifest();
                }
            }
            else
            {
                currentManifest = CreateDefaultManifest();
            }
        }
        
        private UpdateManifest CreateDefaultManifest()
        {
            return new UpdateManifest
            {
                version = Application.version,
                buildDate = DateTime.Now.ToString("yyyy-MM-dd"),
                buildNumber = 1,
                files = new List<FileEntry>(),
                patches = new List<PatchInfo>()
            };
        }
        
        public void CheckForUpdates()
        {
            if (!isUpdating)
            {
                StartCoroutine(CheckForUpdatesRoutine());
            }
        }
        
        private IEnumerator CheckForUpdatesRoutine()
        {
            string manifestUrl = updateServerUrl + manifestEndpoint;
            
            using (UnityWebRequest www = UnityWebRequest.Get(manifestUrl))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        serverManifest = JsonUtility.FromJson<UpdateManifest>(www.downloadHandler.text);
                        
                        if (IsUpdateAvailable())
                        {
                            UpdateInfo info = CreateUpdateInfo();
                            OnUpdateAvailable?.Invoke(info);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to parse server manifest: {e.Message}");
                        OnUpdateError?.Invoke($"Failed to check for updates: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Update check failed: {www.error}");
                }
            }
        }
        
        private bool IsUpdateAvailable()
        {
            if (serverManifest == null) return false;
            
            try
            {
                Version current = new Version(currentManifest.version);
                Version server = new Version(serverManifest.version);
                return server > current;
            }
            catch
            {
                return serverManifest.buildNumber > currentManifest.buildNumber;
            }
        }
        
        private UpdateInfo CreateUpdateInfo()
        {
            long downloadSize = CalculateDownloadSize();
            
            return new UpdateInfo
            {
                currentVersion = currentManifest.version,
                newVersion = serverManifest.version,
                downloadSize = downloadSize,
                changelog = "", // Would fetch from server
                requiresRestart = true,
                forceUpdate = false
            };
        }
        
        private long CalculateDownloadSize()
        {
            // Check if delta patch available
            var patch = FindPatch(currentManifest.version, serverManifest.version);
            if (patch != null)
            {
                return patch.size;
            }
            
            // Calculate full download size
            long size = 0;
            foreach (var file in serverManifest.files)
            {
                var localFile = currentManifest.files.Find(f => f.path == file.path);
                if (localFile == null || localFile.hash != file.hash)
                {
                    size += file.size;
                }
            }
            return size;
        }
        
        private PatchInfo FindPatch(string from, string to)
        {
            if (serverManifest?.patches == null) return null;
            return serverManifest.patches.Find(p => p.fromVersion == from && p.toVersion == to);
        }
        
        public void StartUpdate()
        {
            if (!isUpdating && serverManifest != null)
            {
                StartCoroutine(UpdateRoutine());
            }
        }
        
        private IEnumerator UpdateRoutine()
        {
            isUpdating = true;
            
            // Create download directory
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            
            // Check for delta patch
            var patch = FindPatch(currentManifest.version, serverManifest.version);
            
            if (patch != null)
            {
                yield return StartCoroutine(ApplyDeltaPatch(patch));
            }
            else
            {
                yield return StartCoroutine(FullUpdate());
            }
            
            isUpdating = false;
        }
        
        private IEnumerator ApplyDeltaPatch(PatchInfo patch)
        {
            OnDownloadProgress?.Invoke(0f, "Downloading patch...");
            
            string patchUrl = updateServerUrl + patchEndpoint + patch.patchFile;
            string patchPath = Path.Combine(downloadPath, patch.patchFile);
            
            using (UnityWebRequest www = UnityWebRequest.Get(patchUrl))
            {
                var operation = www.SendWebRequest();
                
                while (!operation.isDone)
                {
                    OnDownloadProgress?.Invoke(www.downloadProgress * 0.5f, $"Downloading patch: {www.downloadProgress * 100:F0}%");
                    yield return null;
                }
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    // Verify hash
                    byte[] data = www.downloadHandler.data;
                    string hash = ComputeHash(data);
                    
                    if (hash == patch.hash)
                    {
                        File.WriteAllBytes(patchPath, data);
                        
                        OnDownloadProgress?.Invoke(0.5f, "Applying patch...");
                        
                        // Apply patch (would use binary diff library)
                        yield return StartCoroutine(ApplyPatchFile(patchPath));
                        
                        // Cleanup
                        File.Delete(patchPath);
                        
                        // Update manifest
                        currentManifest = serverManifest;
                        SaveLocalManifest();
                        
                        OnDownloadProgress?.Invoke(1f, "Update complete!");
                        OnUpdateComplete?.Invoke(serverManifest.version);
                    }
                    else
                    {
                        OnUpdateError?.Invoke("Patch verification failed. Hash mismatch.");
                    }
                }
                else
                {
                    OnUpdateError?.Invoke($"Failed to download patch: {www.error}");
                }
            }
        }
        
        private IEnumerator ApplyPatchFile(string patchPath)
        {
            // In a real implementation, this would use a binary diff library
            // like bsdiff or HDiffPatch to apply the delta patch
            
            OnDownloadProgress?.Invoke(0.75f, "Extracting files...");
            yield return null;
            
            // Simulate patch application
            yield return new WaitForSeconds(1f);
        }
        
        private IEnumerator FullUpdate()
        {
            List<FileEntry> filesToDownload = new List<FileEntry>();
            
            // Find files that need updating
            foreach (var file in serverManifest.files)
            {
                var localFile = currentManifest.files.Find(f => f.path == file.path);
                if (localFile == null || localFile.hash != file.hash)
                {
                    filesToDownload.Add(file);
                }
            }
            
            long totalSize = 0;
            foreach (var file in filesToDownload)
            {
                totalSize += file.size;
            }
            
            long downloadedSize = 0;
            
            foreach (var file in filesToDownload)
            {
                string fileUrl = updateServerUrl + "/files/" + file.path;
                string localPath = Path.Combine(Application.dataPath, file.path);
                
                using (UnityWebRequest www = UnityWebRequest.Get(fileUrl))
                {
                    var operation = www.SendWebRequest();
                    
                    while (!operation.isDone)
                    {
                        float fileProgress = www.downloadProgress * file.size;
                        float totalProgress = (downloadedSize + fileProgress) / totalSize;
                        OnDownloadProgress?.Invoke(totalProgress, $"Downloading: {file.path}");
                        yield return null;
                    }
                    
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        byte[] data = www.downloadHandler.data;
                        string hash = ComputeHash(data);
                        
                        if (hash == file.hash)
                        {
                            // Ensure directory exists
                            string dir = Path.GetDirectoryName(localPath);
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                            
                            // Decompress if needed
                            if (file.compressed)
                            {
                                data = Decompress(data);
                            }
                            
                            File.WriteAllBytes(localPath, data);
                            downloadedSize += file.size;
                        }
                        else
                        {
                            OnUpdateError?.Invoke($"File verification failed: {file.path}");
                            yield break;
                        }
                    }
                    else
                    {
                        OnUpdateError?.Invoke($"Failed to download: {file.path}");
                        yield break;
                    }
                }
            }
            
            // Update manifest
            currentManifest = serverManifest;
            SaveLocalManifest();
            
            OnDownloadProgress?.Invoke(1f, "Update complete!");
            OnUpdateComplete?.Invoke(serverManifest.version);
        }
        
        private void SaveLocalManifest()
        {
            string json = JsonUtility.ToJson(currentManifest, true);
            File.WriteAllText(localManifestPath, json);
        }
        
        private string ComputeHash(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        
        private byte[] Decompress(byte[] data)
        {
            // Would use GZip or other compression
            // For now, return as-is
            return data;
        }
        
        public string GetCurrentVersion()
        {
            return currentManifest?.version ?? Application.version;
        }
        
        public bool IsUpdating()
        {
            return isUpdating;
        }
    }
    
    /// <summary>
    /// Integrity verification system
    /// </summary>
    public class IntegrityChecker : MonoBehaviour
    {
        [SerializeField] private bool checkOnStart = true;
        [SerializeField] private bool repairOnFailure = true;
        
        public event Action<int, int> OnProgress;
        public event Action<List<string>> OnComplete;
        public event Action<string> OnError;
        
        public void VerifyIntegrity()
        {
            StartCoroutine(VerifyRoutine());
        }
        
        private IEnumerator VerifyRoutine()
        {
            List<string> corruptedFiles = new List<string>();
            
            string manifestPath = Path.Combine(Application.persistentDataPath, "manifest.json");
            if (!File.Exists(manifestPath))
            {
                OnComplete?.Invoke(corruptedFiles);
                yield break;
            }
            
            string json = File.ReadAllText(manifestPath);
            var manifest = JsonUtility.FromJson<AutoUpdateSystem.UpdateManifest>(json);
            
            int checked_count = 0;
            int total = manifest.files.Count;
            
            foreach (var file in manifest.files)
            {
                string localPath = Path.Combine(Application.dataPath, file.path);
                
                if (!File.Exists(localPath))
                {
                    corruptedFiles.Add(file.path);
                }
                else
                {
                    byte[] data = File.ReadAllBytes(localPath);
                    string hash = ComputeHash(data);
                    
                    if (hash != file.hash)
                    {
                        corruptedFiles.Add(file.path);
                    }
                }
                
                checked_count++;
                OnProgress?.Invoke(checked_count, total);
                
                // Yield periodically
                if (checked_count % 10 == 0)
                {
                    yield return null;
                }
            }
            
            if (corruptedFiles.Count > 0 && repairOnFailure)
            {
                // Trigger repair
                AutoUpdateSystem.Instance?.StartUpdate();
            }
            
            OnComplete?.Invoke(corruptedFiles);
        }
        
        private string ComputeHash(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
