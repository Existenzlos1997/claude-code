using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Gameplay
{
    /// <summary>
    /// Spawns aircraft at specific locations with configurable settings
    /// Can be used for player spawning, AI spawning, or scenario setup
    /// </summary>
    public class AircraftSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private SpawnMode spawnMode = SpawnMode.OnStart;
        [SerializeField] private float spawnDelay = 0f;
        [SerializeField] private bool spawnOnlyOnce = false;

        [Header("Aircraft Configuration")]
        [SerializeField] private GameObject aircraftPrefab;
        [SerializeField] private string aircraftType = "P-51D Mustang";
        [SerializeField] private TeamType team = TeamType.Friendly;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private bool randomizeSpawnPoint = true;

        [Header("AI Configuration")]
        [SerializeField] private bool isAIControlled = true;
        [SerializeField] private AIBehaviorType aiBehavior = AIBehaviorType.Patrol;
        [SerializeField] private float patrolRadius = 1000f;

        [Header("Advanced")]
        [SerializeField] private int maxConcurrentSpawns = 10;
        [SerializeField] private float respawnDelay = 30f;

        private List<GameObject> spawnedAircraft = new List<GameObject>();
        private bool hasSpawned = false;

        public enum SpawnMode { OnStart, OnTrigger, Continuous, WavesBased }
        public enum TeamType { Player, Friendly, Enemy, Neutral }
        public enum AIBehaviorType { Patrol, Guard, Attack, Escort, Intercept }

        private void Start()
        {
            if (spawnMode == SpawnMode.OnStart)
            {
                if (spawnDelay > 0) Invoke(nameof(SpawnAircraft), spawnDelay);
                else SpawnAircraft();
            }
            else if (spawnMode == SpawnMode.Continuous)
            {
                InvokeRepeating(nameof(SpawnAircraft), spawnDelay, respawnDelay);
            }

            if (spawnPoints == null || spawnPoints.Length == 0)
                spawnPoints = new Transform[] { transform };
        }

        private void Update()
        {
            spawnedAircraft.RemoveAll(aircraft => aircraft == null);
        }

        public GameObject SpawnAircraft()
        {
            if (spawnOnlyOnce && hasSpawned) return null;
            if (spawnedAircraft.Count >= maxConcurrentSpawns)
            {
                Debug.LogWarning($"AircraftSpawner: Max concurrent spawns ({maxConcurrentSpawns}) reached");
                return null;
            }

            Transform spawnPoint = GetSpawnPoint();
            GameObject aircraft = CreateAircraft(spawnPoint);

            if (aircraft != null)
            {
                spawnedAircraft.Add(aircraft);
                hasSpawned = true;
                ConfigureAircraft(aircraft);
            }

            return aircraft;
        }

        private Transform GetSpawnPoint()
        {
            if (spawnPoints.Length == 0) return transform;
            if (randomizeSpawnPoint) return spawnPoints[Random.Range(0, spawnPoints.Length)];
            return spawnPoints[spawnedAircraft.Count % spawnPoints.Length];
        }

        private GameObject CreateAircraft(Transform spawnPoint)
        {
            if (aircraftPrefab != null)
                return Instantiate(aircraftPrefab, spawnPoint.position, spawnPoint.rotation);
            return CreatePlaceholderAircraft(spawnPoint.position, spawnPoint.rotation);
        }

        private GameObject CreatePlaceholderAircraft(Vector3 position, Quaternion rotation)
        {
            GameObject aircraft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            aircraft.name = $"Aircraft_{aircraftType}_{spawnedAircraft.Count}";
            aircraft.transform.position = position;
            aircraft.transform.rotation = rotation;
            aircraft.transform.localScale = new Vector3(10f, 3f, 15f);

            Rigidbody rb = aircraft.AddComponent<Rigidbody>();
            rb.mass = 1000f;
            rb.drag = 0.5f;
            rb.angularDrag = 5f;
            rb.useGravity = false;

            aircraft.tag = team == TeamType.Player ? "Player" : 
                           team == TeamType.Enemy ? "Enemy" : 
                           team == TeamType.Friendly ? "Friendly" : "Untagged";

            return aircraft;
        }

        private void ConfigureAircraft(GameObject aircraft)
        {
            aircraft.name = $"{aircraftType}_{team}_{spawnedAircraft.Count}";
            if (isAIControlled && team != TeamType.Player)
                Debug.Log($"AircraftSpawner: AI configuration would be applied to {aircraft.name}");
            Debug.Log($"AircraftSpawner: Spawned {aircraft.name} at {aircraft.transform.position}");
        }

        public void TriggerSpawn()
        {
            if (spawnMode == SpawnMode.OnTrigger) SpawnAircraft();
        }

        public void DespawnAll()
        {
            foreach (GameObject aircraft in spawnedAircraft)
                if (aircraft != null) Destroy(aircraft);
            spawnedAircraft.Clear();
            hasSpawned = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = team == TeamType.Enemy ? Color.red : Color.green;
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                foreach (Transform point in spawnPoints)
                {
                    if (point != null)
                    {
                        Gizmos.DrawWireSphere(point.position, 50f);
                        Gizmos.DrawLine(point.position, point.position + point.forward * 100f);
                    }
                }
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, 50f);
            }

            if (aiBehavior == AIBehaviorType.Patrol)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, patrolRadius);
            }
        }
    }
}
