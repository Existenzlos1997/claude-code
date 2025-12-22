using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Multiplayer
{
    /// <summary>
    /// Advanced multiplayer synchronization with interpolation and lag compensation
    /// </summary>
    public class MultiplayerSyncSystem : MonoBehaviour
    {
        public static MultiplayerSyncSystem Instance { get; private set; }

        [SerializeField] private float sendRate = 20f;
        [SerializeField] private float interpolationDelay = 0.1f;
        [SerializeField] private bool enableLagCompensation = true;
        [SerializeField] private float maxLagCompensation = 0.2f;

        private Dictionary<string, NetworkSyncObject> syncObjects = new Dictionary<string, NetworkSyncObject>();
        private float averageLatency;

        public event Action<string, Vector3, Quaternion> OnObjectMoved;
        public event Action<float> OnLatencyUpdated;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            foreach (var kvp in syncObjects)
            {
                var syncObj = kvp.Value;
                if (syncObj != null && !syncObj.IsLocalAuthority)
                {
                    syncObj.InterpolateToTarget(interpolationDelay);
                }
            }
        }

        public void RegisterSyncObject(NetworkSyncObject syncObj)
        {
            if (string.IsNullOrEmpty(syncObj.NetworkId))
            {
                syncObj.NetworkId = Guid.NewGuid().ToString();
            }
            syncObjects[syncObj.NetworkId] = syncObj;
        }

        public void UnregisterSyncObject(NetworkSyncObject syncObj)
        {
            syncObjects.Remove(syncObj.NetworkId);
        }

        public float GetAverageLatency() => averageLatency;

        public void UpdateLatency(float latency)
        {
            averageLatency = Mathf.Lerp(averageLatency, latency, 0.1f);
            OnLatencyUpdated?.Invoke(averageLatency);
        }
    }

    public class NetworkSyncObject : MonoBehaviour
    {
        [SerializeField] private string networkId;
        [SerializeField] private bool isLocalAuthority;
        [SerializeField] private float interpolationSpeed = 10f;

        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private Vector3 targetVelocity;

        public string NetworkId { get => networkId; set => networkId = value; }
        public bool IsLocalAuthority => isLocalAuthority;
        public Vector3 Velocity { get; private set; }

        private void Start()
        {
            MultiplayerSyncSystem.Instance?.RegisterSyncObject(this);
            targetPosition = transform.position;
            targetRotation = transform.rotation;
        }

        private void OnDestroy()
        {
            MultiplayerSyncSystem.Instance?.UnregisterSyncObject(this);
        }

        public void SetNetworkTarget(Vector3 position, Quaternion rotation, Vector3 velocity, float timestamp)
        {
            targetPosition = position;
            targetRotation = rotation;
            targetVelocity = velocity;
        }

        public void InterpolateToTarget(float delay)
        {
            if (isLocalAuthority) return;

            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance > 10f)
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
                return;
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * interpolationSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * interpolationSpeed);
        }
    }
}
