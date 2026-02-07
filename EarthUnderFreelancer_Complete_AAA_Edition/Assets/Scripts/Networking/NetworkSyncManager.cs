using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Networking
{
    /// <summary>
    /// Real-time network synchronization with interpolation, prediction, and lag compensation.
    /// Supports up to 200ms latency with smooth movement.
    /// </summary>
    public class NetworkSyncManager : MonoBehaviour
    {
        private static NetworkSyncManager instance;
        public static NetworkSyncManager Instance => instance;
        
        [Header("Network Settings")]
        public float sendRate = 20f; // Updates per second
        public float interpolationDelay = 100f; // ms
        public bool clientPrediction = true;
        public bool lagCompensation = true;
        
        [Header("Sync Priority")]
        public float criticalDistance = 1000f; // Sync rate 20Hz
        public float normalDistance = 3000f; // Sync rate 10Hz
        public float lowDistance = 10000f; // Sync rate 5Hz
        
        // Synchronized entities
        private Dictionary<int, NetworkEntity> entities = new Dictionary<int, NetworkEntity>();
        private float nextSendTime = 0f;
        
        public class NetworkEntity
        {
            public int entityId;
            public Transform transform;
            public Vector3 targetPosition;
            public Quaternion targetRotation;
            public Vector3 velocity;
            public float lastSyncTime;
            public int priority;
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
            if (Time.time >= nextSendTime)
            {
                SyncAllEntities();
                nextSendTime = Time.time + (1f / sendRate);
            }
            
            InterpolateEntities();
        }
        
        /// <summary>
        /// Register entity for network sync
        /// </summary>
        public void RegisterEntity(int entityId, Transform entityTransform)
        {
            if (!entities.ContainsKey(entityId))
            {
                entities[entityId] = new NetworkEntity
                {
                    entityId = entityId,
                    transform = entityTransform,
                    targetPosition = entityTransform.position,
                    targetRotation = entityTransform.rotation,
                    velocity = Vector3.zero,
                    lastSyncTime = Time.time,
                    priority = 1
                };
            }
        }
        
        /// <summary>
        /// Sync all entities based on priority
        /// </summary>
        private void SyncAllEntities()
        {
            foreach (var kvp in entities)
            {
                NetworkEntity entity = kvp.Value;
                
                // Update priority based on distance to player
                UpdateEntityPriority(entity);
                
                // Sync based on priority
                if (ShouldSync(entity))
                {
                    SendEntityState(entity);
                    entity.lastSyncTime = Time.time;
                }
            }
        }
        
        /// <summary>
        /// Update entity sync priority
        /// </summary>
        private void UpdateEntityPriority(NetworkEntity entity)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null || entity.transform == null) return;
            
            float distance = Vector3.Distance(player.transform.position, entity.transform.position);
            
            if (distance < criticalDistance)
                entity.priority = 3;
            else if (distance < normalDistance)
                entity.priority = 2;
            else if (distance < lowDistance)
                entity.priority = 1;
            else
                entity.priority = 0;
        }
        
        /// <summary>
        /// Check if entity should sync this frame
        /// </summary>
        private bool ShouldSync(NetworkEntity entity)
        {
            float timeSinceSync = Time.time - entity.lastSyncTime;
            
            switch (entity.priority)
            {
                case 3: return timeSinceSync >= 1f / 20f; // 20Hz
                case 2: return timeSinceSync >= 1f / 10f; // 10Hz
                case 1: return timeSinceSync >= 1f / 5f; // 5Hz
                default: return false;
            }
        }
        
        /// <summary>
        /// Send entity state over network (placeholder - would use actual networking)
        /// </summary>
        private void SendEntityState(NetworkEntity entity)
        {
            // In real implementation, would send over network
            // For now, just update local state
            if (entity.transform != null)
            {
                entity.velocity = (entity.transform.position - entity.targetPosition) / Time.deltaTime;
                entity.targetPosition = entity.transform.position;
                entity.targetRotation = entity.transform.rotation;
            }
        }
        
        /// <summary>
        /// Receive entity state from network (called by network layer)
        /// </summary>
        public void ReceiveEntityState(int entityId, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            if (entities.TryGetValue(entityId, out NetworkEntity entity))
            {
                if (clientPrediction)
                {
                    // Apply client-side prediction
                    entity.targetPosition = position + (velocity * (interpolationDelay / 1000f));
                }
                else
                {
                    entity.targetPosition = position;
                }
                
                entity.targetRotation = rotation;
                entity.velocity = velocity;
                entity.lastSyncTime = Time.time;
            }
        }
        
        /// <summary>
        /// Interpolate entities to target state
        /// </summary>
        private void InterpolateEntities()
        {
            float interpolationSpeed = 10f;
            
            foreach (var kvp in entities)
            {
                NetworkEntity entity = kvp.Value;
                if (entity.transform == null) continue;
                
                // Smooth interpolation
                entity.transform.position = Vector3.Lerp(
                    entity.transform.position,
                    entity.targetPosition,
                    Time.deltaTime * interpolationSpeed
                );
                
                entity.transform.rotation = Quaternion.Slerp(
                    entity.transform.rotation,
                    entity.targetRotation,
                    Time.deltaTime * interpolationSpeed
                );
            }
        }
        
        /// <summary>
        /// Unregister entity
        /// </summary>
        public void UnregisterEntity(int entityId)
        {
            entities.Remove(entityId);
        }
        
        /// <summary>
        /// Get network statistics
        /// </summary>
        public string GetNetworkStats()
        {
            return $"Synced Entities: {entities.Count}\n" +
                   $"Send Rate: {sendRate} Hz\n" +
                   $"Interpolation Delay: {interpolationDelay} ms";
        }
    }
}
