using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Streaming
{
    /// <summary>
    /// Dynamic asset streaming manager for large open worlds.
    /// Manages memory budget, priority-based loading, and background streaming.
    /// </summary>
    public class AssetStreamingManager : MonoBehaviour
    {
        private static AssetStreamingManager instance;
        public static AssetStreamingManager Instance => instance;
        
        [Header("Memory Budget")]
        public float maxAssetMemoryMB = 2048f; // 2GB
        public float currentMemoryUsageMB = 0f;
        
        [Header("Streaming Settings")]
        public float streamingDistance = 5000f; // meters
        public float unloadDistance = 7000f; // meters
        public int maxAssetsPerFrame = 3;
        
        [Header("Priority Zones")]
        public Vector3 playerPosition;
        public float criticalZone = 1000f; // Load immediately
        public float highPriorityZone = 2500f;
        public float normalPriorityZone = 5000f;
        
        // Streaming data
        private Dictionary<string, StreamedAsset> loadedAssets = new Dictionary<string, StreamedAsset>();
        private Queue<AssetRequest> loadQueue = new Queue<AssetRequest>();
        private List<AssetRequest> priorityQueue = new List<AssetRequest>();
        
        private class StreamedAsset
        {
            public string assetId;
            public GameObject gameObject;
            public float memorySize;
            public float lastAccessTime;
            public Vector3 position;
            public int priority;
        }
        
        private class AssetRequest
        {
            public string assetId;
            public string assetPath;
            public Vector3 position;
            public int priority;
            public System.Action<GameObject> onLoaded;
        }
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Update()
        {
            // Update player position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerPosition = player.transform.position;
            }
            
            // Process streaming
            UpdateAssetStreaming();
            ProcessLoadQueue();
            UnloadDistantAssets();
        }
        
        /// <summary>
        /// Request asset to be loaded
        /// </summary>
        public void RequestAsset(string assetId, string assetPath, Vector3 position, System.Action<GameObject> onLoaded = null)
        {
            // Check if already loaded
            if (loadedAssets.ContainsKey(assetId))
            {
                loadedAssets[assetId].lastAccessTime = Time.time;
                onLoaded?.Invoke(loadedAssets[assetId].gameObject);
                return;
            }
            
            // Calculate priority based on distance
            float distance = Vector3.Distance(playerPosition, position);
            int priority = GetPriorityForDistance(distance);
            
            AssetRequest request = new AssetRequest
            {
                assetId = assetId,
                assetPath = assetPath,
                position = position,
                priority = priority,
                onLoaded = onLoaded
            };
            
            // Add to appropriate queue
            if (priority >= 3)
            {
                priorityQueue.Add(request);
                priorityQueue.Sort((a, b) => b.priority.CompareTo(a.priority));
            }
            else
            {
                loadQueue.Enqueue(request);
            }
        }
        
        /// <summary>
        /// Process asset load queue
        /// </summary>
        private void ProcessLoadQueue()
        {
            int loaded = 0;
            
            // Process priority queue first
            while (priorityQueue.Count > 0 && loaded < maxAssetsPerFrame)
            {
                if (currentMemoryUsageMB >= maxAssetMemoryMB) break;
                
                AssetRequest request = priorityQueue[0];
                priorityQueue.RemoveAt(0);
                LoadAsset(request);
                loaded++;
            }
            
            // Process normal queue
            while (loadQueue.Count > 0 && loaded < maxAssetsPerFrame)
            {
                if (currentMemoryUsageMB >= maxAssetMemoryMB) break;
                
                AssetRequest request = loadQueue.Dequeue();
                LoadAsset(request);
                loaded++;
            }
        }
        
        /// <summary>
        /// Load an asset
        /// </summary>
        private void LoadAsset(AssetRequest request)
        {
            // In real implementation, would use Resources.LoadAsync or AssetBundle
            // For now, create placeholder
            GameObject asset = CreatePlaceholderAsset(request.assetId);
            asset.transform.position = request.position;
            
            // Estimate memory size (would be actual size in real implementation)
            float memorySize = EstimateAssetSize(request.assetId);
            
            StreamedAsset streamedAsset = new StreamedAsset
            {
                assetId = request.assetId,
                gameObject = asset,
                memorySize = memorySize,
                lastAccessTime = Time.time,
                position = request.position,
                priority = request.priority
            };
            
            loadedAssets[request.assetId] = streamedAsset;
            currentMemoryUsageMB += memorySize;
            
            request.onLoaded?.Invoke(asset);
        }
        
        /// <summary>
        /// Unload assets that are too far away
        /// </summary>
        private void UnloadDistantAssets()
        {
            List<string> assetsToUnload = new List<string>();
            
            foreach (var kvp in loadedAssets)
            {
                float distance = Vector3.Distance(playerPosition, kvp.Value.position);
                
                // Unload if beyond unload distance
                if (distance > unloadDistance)
                {
                    assetsToUnload.Add(kvp.Key);
                }
            }
            
            foreach (string assetId in assetsToUnload)
            {
                UnloadAsset(assetId);
            }
        }
        
        /// <summary>
        /// Unload specific asset
        /// </summary>
        public void UnloadAsset(string assetId)
        {
            if (loadedAssets.TryGetValue(assetId, out StreamedAsset asset))
            {
                Destroy(asset.gameObject);
                currentMemoryUsageMB -= asset.memorySize;
                loadedAssets.Remove(assetId);
            }
        }
        
        /// <summary>
        /// Update asset streaming based on player movement
        /// </summary>
        private void UpdateAssetStreaming()
        {
            // Update priorities for loaded assets
            foreach (var kvp in loadedAssets)
            {
                float distance = Vector3.Distance(playerPosition, kvp.Value.position);
                kvp.Value.priority = GetPriorityForDistance(distance);
            }
        }
        
        /// <summary>
        /// Get priority based on distance from player
        /// </summary>
        private int GetPriorityForDistance(float distance)
        {
            if (distance < criticalZone) return 4; // Critical
            if (distance < highPriorityZone) return 3; // High
            if (distance < normalPriorityZone) return 2; // Normal
            return 1; // Low
        }
        
        /// <summary>
        /// Create placeholder asset (would be replaced with actual loading)
        /// </summary>
        private GameObject CreatePlaceholderAsset(string assetId)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = assetId;
            return obj;
        }
        
        /// <summary>
        /// Estimate asset memory size (would use actual size in real implementation)
        /// </summary>
        private float EstimateAssetSize(string assetId)
        {
            // Placeholder estimation
            if (assetId.Contains("aircraft")) return 50f; // MB
            if (assetId.Contains("building")) return 20f;
            if (assetId.Contains("tree")) return 1f;
            return 5f;
        }
        
        /// <summary>
        /// Force garbage collection if memory is low
        /// </summary>
        public void ForceGarbageCollection()
        {
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }
        
        /// <summary>
        /// Get memory usage statistics
        /// </summary>
        public string GetMemoryStats()
        {
            return $"Asset Memory: {currentMemoryUsageMB:F1}/{maxAssetMemoryMB:F1} MB\n" +
                   $"Loaded Assets: {loadedAssets.Count}\n" +
                   $"Queue Size: {loadQueue.Count + priorityQueue.Count}";
        }
    }
}
