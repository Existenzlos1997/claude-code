using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Performance
{
    /// <summary>
    /// Advanced LOD (Level of Detail) management system for MMO-scale performance
    /// Handles dynamic LOD switching, object pooling integration, and culling
    /// </summary>
    public class LODManager : MonoBehaviour
    {
        public static LODManager Instance { get; private set; }

        [Header("LOD Settings")]
        [SerializeField] private float[] lodDistances = { 50f, 150f, 400f, 1000f };
        [SerializeField] private float lodBias = 1f;
        [SerializeField] private float updateInterval = 0.1f;
        [SerializeField] private int maxUpdatesPerFrame = 50;

        [Header("Culling Settings")]
        [SerializeField] private float cullingDistance = 2000f;
        [SerializeField] private bool useOcclusionCulling = true;
        [SerializeField] private bool useFrustumCulling = true;
        [SerializeField] private LayerMask cullingLayers = -1;

        [Header("Quality Scaling")]
        [SerializeField] private bool enableDynamicQuality = true;
        [SerializeField] private float targetFrameRate = 60f;
        [SerializeField] private float qualityAdjustmentSpeed = 0.5f;
        [SerializeField] private float minQualityScale = 0.5f;
        [SerializeField] private float maxQualityScale = 1.5f;

        [Header("Object Streaming")]
        [SerializeField] private float streamingDistance = 1500f;
        [SerializeField] private int maxStreamedObjects = 500;
        [SerializeField] private float streamingUpdateInterval = 1f;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo;
        [SerializeField] private bool showCullingBounds;

        // Registered LOD objects
        private List<LODObject> registeredObjects = new List<LODObject>();
        private Queue<LODObject> updateQueue = new Queue<LODObject>();
        private Dictionary<int, List<LODObject>> lodGroups = new Dictionary<int, List<LODObject>>();

        // Camera reference
        private Camera mainCamera;
        private Transform cameraTransform;
        private Plane[] frustumPlanes = new Plane[6];

        // Quality scaling
        private float currentQualityScale = 1f;
        private float[] frameTimes = new float[30];
        private int frameTimeIndex;

        // Streaming
        private List<StreamableObject> streamableObjects = new List<StreamableObject>();
        private float lastStreamingUpdate;

        // Statistics
        private int visibleObjects;
        private int culledObjects;
        private int currentLOD0Count;
        private int currentLOD1Count;
        private int currentLOD2Count;
        private int currentLOD3Count;

        private float lastUpdateTime;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }

            // Initialize LOD groups
            for (int i = 0; i < 4; i++)
            {
                lodGroups[i] = new List<LODObject>();
            }
        }

        private void Update()
        {
            if (cameraTransform == null)
            {
                mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    cameraTransform = mainCamera.transform;
                }
                return;
            }

            // Update frustum planes for culling
            if (useFrustumCulling && mainCamera != null)
            {
                GeometryUtility.CalculateFrustumPlanes(mainCamera, frustumPlanes);
            }

            // Process LOD updates
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                lastUpdateTime = Time.time;
                ProcessLODUpdates();
            }

            // Dynamic quality adjustment
            if (enableDynamicQuality)
            {
                UpdateDynamicQuality();
            }

            // Object streaming
            if (Time.time - lastStreamingUpdate >= streamingUpdateInterval)
            {
                lastStreamingUpdate = Time.time;
                UpdateObjectStreaming();
            }
        }

        private void ProcessLODUpdates()
        {
            Vector3 cameraPos = cameraTransform.position;
            int updatesThisFrame = 0;

            // Reset statistics
            visibleObjects = 0;
            culledObjects = 0;
            currentLOD0Count = 0;
            currentLOD1Count = 0;
            currentLOD2Count = 0;
            currentLOD3Count = 0;

            // Add all registered objects to queue if empty
            if (updateQueue.Count == 0)
            {
                foreach (var obj in registeredObjects)
                {
                    if (obj != null && obj.IsActive)
                    {
                        updateQueue.Enqueue(obj);
                    }
                }
            }

            // Process queue
            while (updateQueue.Count > 0 && updatesThisFrame < maxUpdatesPerFrame)
            {
                var obj = updateQueue.Dequeue();
                if (obj != null && obj.IsActive)
                {
                    UpdateLODObject(obj, cameraPos);
                    updatesThisFrame++;
                }
            }
        }

        private void UpdateLODObject(LODObject obj, Vector3 cameraPos)
        {
            float distance = Vector3.Distance(obj.Position, cameraPos);
            float adjustedDistance = distance / (lodBias * currentQualityScale);

            // Check culling
            if (adjustedDistance > cullingDistance)
            {
                obj.SetCulled(true);
                culledObjects++;
                return;
            }

            // Frustum culling
            if (useFrustumCulling && obj.Bounds.size.magnitude > 0)
            {
                if (!GeometryUtility.TestPlanesAABB(frustumPlanes, obj.Bounds))
                {
                    obj.SetCulled(true);
                    culledObjects++;
                    return;
                }
            }

            // Determine LOD level
            int lodLevel = 3;
            for (int i = 0; i < lodDistances.Length; i++)
            {
                if (adjustedDistance < lodDistances[i])
                {
                    lodLevel = i;
                    break;
                }
            }

            obj.SetCulled(false);
            obj.SetLODLevel(lodLevel);
            visibleObjects++;

            // Track LOD counts
            switch (lodLevel)
            {
                case 0: currentLOD0Count++; break;
                case 1: currentLOD1Count++; break;
                case 2: currentLOD2Count++; break;
                case 3: currentLOD3Count++; break;
            }
        }

        private void UpdateDynamicQuality()
        {
            // Track frame times
            frameTimes[frameTimeIndex] = Time.unscaledDeltaTime;
            frameTimeIndex = (frameTimeIndex + 1) % frameTimes.Length;

            // Calculate average frame rate
            float avgFrameTime = 0f;
            for (int i = 0; i < frameTimes.Length; i++)
            {
                avgFrameTime += frameTimes[i];
            }
            avgFrameTime /= frameTimes.Length;
            float currentFPS = 1f / avgFrameTime;

            // Adjust quality scale
            float targetScale;
            if (currentFPS < targetFrameRate * 0.9f)
            {
                // Below target - reduce quality
                targetScale = currentQualityScale - qualityAdjustmentSpeed * Time.deltaTime;
            }
            else if (currentFPS > targetFrameRate * 1.1f)
            {
                // Above target - increase quality
                targetScale = currentQualityScale + qualityAdjustmentSpeed * Time.deltaTime;
            }
            else
            {
                targetScale = currentQualityScale;
            }

            currentQualityScale = Mathf.Clamp(targetScale, minQualityScale, maxQualityScale);

            // Apply quality scale to LOD distances
            for (int i = 0; i < lodDistances.Length; i++)
            {
                float baseDist = i == 0 ? 50f : i == 1 ? 150f : i == 2 ? 400f : 1000f;
                lodDistances[i] = baseDist * currentQualityScale;
            }
        }

        private void UpdateObjectStreaming()
        {
            if (cameraTransform == null) return;

            Vector3 cameraPos = cameraTransform.position;
            int loadedCount = 0;

            foreach (var streamable in streamableObjects)
            {
                if (streamable == null) continue;

                float distance = Vector3.Distance(streamable.Position, cameraPos);

                if (distance <= streamingDistance)
                {
                    if (!streamable.IsLoaded && loadedCount < maxStreamedObjects)
                    {
                        streamable.Load();
                        loadedCount++;
                    }
                }
                else if (streamable.IsLoaded)
                {
                    streamable.Unload();
                }

                if (streamable.IsLoaded)
                {
                    loadedCount++;
                }
            }
        }

        public void RegisterLODObject(LODObject obj)
        {
            if (!registeredObjects.Contains(obj))
            {
                registeredObjects.Add(obj);
            }
        }

        public void UnregisterLODObject(LODObject obj)
        {
            registeredObjects.Remove(obj);
        }

        public void RegisterStreamableObject(StreamableObject obj)
        {
            if (!streamableObjects.Contains(obj))
            {
                streamableObjects.Add(obj);
            }
        }

        public void UnregisterStreamableObject(StreamableObject obj)
        {
            streamableObjects.Remove(obj);
        }

        public float GetCurrentQualityScale()
        {
            return currentQualityScale;
        }

        public void SetLODBias(float bias)
        {
            lodBias = Mathf.Clamp(bias, 0.5f, 2f);
        }

        public LODStats GetStats()
        {
            return new LODStats
            {
                VisibleObjects = visibleObjects,
                CulledObjects = culledObjects,
                LOD0Count = currentLOD0Count,
                LOD1Count = currentLOD1Count,
                LOD2Count = currentLOD2Count,
                LOD3Count = currentLOD3Count,
                QualityScale = currentQualityScale,
                RegisteredCount = registeredObjects.Count
            };
        }

        private void OnGUI()
        {
            if (!showDebugInfo) return;

            var stats = GetStats();
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"LOD Manager Statistics:");
            GUILayout.Label($"Visible: {stats.VisibleObjects} | Culled: {stats.CulledObjects}");
            GUILayout.Label($"LOD0: {stats.LOD0Count} | LOD1: {stats.LOD1Count}");
            GUILayout.Label($"LOD2: {stats.LOD2Count} | LOD3: {stats.LOD3Count}");
            GUILayout.Label($"Quality Scale: {stats.QualityScale:F2}");
            GUILayout.Label($"FPS: {1f / Time.unscaledDeltaTime:F1}");
            GUILayout.EndArea();
        }
    }

    public struct LODStats
    {
        public int VisibleObjects;
        public int CulledObjects;
        public int LOD0Count;
        public int LOD1Count;
        public int LOD2Count;
        public int LOD3Count;
        public float QualityScale;
        public int RegisteredCount;
    }

    /// <summary>
    /// Component for objects that support LOD switching
    /// </summary>
    public class LODObject : MonoBehaviour
    {
        [SerializeField] private GameObject[] lodLevels;
        [SerializeField] private bool autoRegister = true;
        [SerializeField] private bool useCustomBounds;
        [SerializeField] private Vector3 boundsSize = Vector3.one * 10f;

        private int currentLOD = -1;
        private bool isCulled;
        private Bounds bounds;

        public Vector3 Position => transform.position;
        public Bounds Bounds => bounds;
        public bool IsActive => gameObject.activeInHierarchy;
        public int CurrentLOD => currentLOD;

        private void Awake()
        {
            CalculateBounds();
        }

        private void OnEnable()
        {
            if (autoRegister && LODManager.Instance != null)
            {
                LODManager.Instance.RegisterLODObject(this);
            }
        }

        private void OnDisable()
        {
            if (LODManager.Instance != null)
            {
                LODManager.Instance.UnregisterLODObject(this);
            }
        }

        private void CalculateBounds()
        {
            if (useCustomBounds)
            {
                bounds = new Bounds(transform.position, boundsSize);
            }
            else
            {
                var renderers = GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    bounds = renderers[0].bounds;
                    for (int i = 1; i < renderers.Length; i++)
                    {
                        bounds.Encapsulate(renderers[i].bounds);
                    }
                }
                else
                {
                    bounds = new Bounds(transform.position, Vector3.one);
                }
            }
        }

        public void SetLODLevel(int level)
        {
            if (level == currentLOD) return;

            currentLOD = Mathf.Clamp(level, 0, lodLevels.Length - 1);

            for (int i = 0; i < lodLevels.Length; i++)
            {
                if (lodLevels[i] != null)
                {
                    lodLevels[i].SetActive(i == currentLOD);
                }
            }
        }

        public void SetCulled(bool culled)
        {
            if (culled == isCulled) return;

            isCulled = culled;

            foreach (var lod in lodLevels)
            {
                if (lod != null)
                {
                    lod.SetActive(!culled && (Array.IndexOf(lodLevels, lod) == currentLOD));
                }
            }
        }

        private void LateUpdate()
        {
            // Update bounds position
            bounds.center = transform.position;
        }
    }

    /// <summary>
    /// Component for streamable objects that load/unload based on distance
    /// </summary>
    public class StreamableObject : MonoBehaviour
    {
        [SerializeField] private string assetPath;
        [SerializeField] private bool autoRegister = true;

        private GameObject loadedAsset;
        private bool isLoaded;

        public Vector3 Position => transform.position;
        public bool IsLoaded => isLoaded;

        private void OnEnable()
        {
            if (autoRegister && LODManager.Instance != null)
            {
                LODManager.Instance.RegisterStreamableObject(this);
            }
        }

        private void OnDisable()
        {
            if (LODManager.Instance != null)
            {
                LODManager.Instance.UnregisterStreamableObject(this);
            }
        }

        public void Load()
        {
            if (isLoaded) return;

            if (!string.IsNullOrEmpty(assetPath))
            {
                var prefab = Resources.Load<GameObject>(assetPath);
                if (prefab != null)
                {
                    loadedAsset = Instantiate(prefab, transform);
                }
            }

            isLoaded = true;
        }

        public void Unload()
        {
            if (!isLoaded) return;

            if (loadedAsset != null)
            {
                Destroy(loadedAsset);
                loadedAsset = null;
            }

            isLoaded = false;
        }
    }

    /// <summary>
    /// Instancing manager for efficient rendering of many similar objects
    /// </summary>
    public class InstancedRenderer : MonoBehaviour
    {
        [SerializeField] private Mesh instanceMesh;
        [SerializeField] private Material instanceMaterial;
        [SerializeField] private int maxInstances = 1000;
        [SerializeField] private float lodDistance = 100f;
        [SerializeField] private bool castShadows = true;

        private Matrix4x4[] instanceMatrices;
        private List<Vector3> instancePositions = new List<Vector3>();
        private List<Quaternion> instanceRotations = new List<Quaternion>();
        private List<Vector3> instanceScales = new List<Vector3>();
        private int instanceCount;
        private MaterialPropertyBlock propertyBlock;

        private void Awake()
        {
            instanceMatrices = new Matrix4x4[maxInstances];
            propertyBlock = new MaterialPropertyBlock();
        }

        public void AddInstance(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (instanceCount >= maxInstances) return;

            instancePositions.Add(position);
            instanceRotations.Add(rotation);
            instanceScales.Add(scale);
            instanceMatrices[instanceCount] = Matrix4x4.TRS(position, rotation, scale);
            instanceCount++;
        }

        public void ClearInstances()
        {
            instancePositions.Clear();
            instanceRotations.Clear();
            instanceScales.Clear();
            instanceCount = 0;
        }

        private void Update()
        {
            if (instanceCount == 0 || instanceMesh == null || instanceMaterial == null) return;

            // Render instances in batches of 1023 (Unity's limit)
            int batchSize = 1023;
            for (int i = 0; i < instanceCount; i += batchSize)
            {
                int count = Mathf.Min(batchSize, instanceCount - i);
                var batchMatrices = new Matrix4x4[count];
                Array.Copy(instanceMatrices, i, batchMatrices, 0, count);

                UnityEngine.Graphics.DrawMeshInstanced(
                    instanceMesh,
                    0,
                    instanceMaterial,
                    batchMatrices,
                    count,
                    propertyBlock,
                    castShadows ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off
                );
            }
        }
    }
}
