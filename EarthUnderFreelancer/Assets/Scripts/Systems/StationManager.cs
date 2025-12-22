using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Space station docking and services
    /// </summary>
    public class StationManager : MonoBehaviour
    {
        public static StationManager Instance { get; private set; }

        [Header("Current Station")]
        [SerializeField] private SpaceStation currentStation;
        [SerializeField] private bool isDocked;

        [Header("Docking Settings")]
        [SerializeField] private float dockingRange = 100f;
        [SerializeField] private float dockingSpeed = 5f;

        public SpaceStation CurrentStation => currentStation;
        public bool IsDocked => isDocked;
        public float DockingRange => dockingRange;

        public event System.Action<SpaceStation> OnDocked;
        public event System.Action OnUndocked;
        public event System.Action<SpaceStation> OnStationInRange;
        public event System.Action OnStationOutOfRange;

        private List<SpaceStation> stations = new List<SpaceStation>();
        private SpaceStation nearestStation;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (!isDocked)
            {
                CheckNearbyStations();
            }
        }

        private void CheckNearbyStations()
        {
            Transform player = GetPlayerTransform();
            if (player == null) return;

            SpaceStation nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var station in stations)
            {
                if (station == null) continue;

                float dist = Vector3.Distance(player.position, station.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = station;
                }
            }

            if (nearest != null && nearestDist <= dockingRange)
            {
                if (nearestStation != nearest)
                {
                    nearestStation = nearest;
                    OnStationInRange?.Invoke(nearest);
                }
            }
            else if (nearestStation != null)
            {
                nearestStation = null;
                OnStationOutOfRange?.Invoke();
            }
        }

        private Transform GetPlayerTransform()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            return player?.transform;
        }

        public bool CanDock()
        {
            if (isDocked) return false;
            if (nearestStation == null) return false;

            // Check faction reputation
            if (FactionManager.Instance != null)
            {
                if (!FactionManager.Instance.CanDock(nearestStation.FactionId))
                    return false;
            }

            return true;
        }

        public bool Dock()
        {
            if (!CanDock()) return false;

            isDocked = true;
            currentStation = nearestStation;

            // Disable player controls
            DisablePlayerMovement();

            OnDocked?.Invoke(currentStation);
            return true;
        }

        public bool Undock()
        {
            if (!isDocked) return false;

            isDocked = false;
            SpaceStation previousStation = currentStation;
            currentStation = null;

            // Enable player controls
            EnablePlayerMovement();

            // Move player outside station
            MovePlayerOutside(previousStation);

            OnUndocked?.Invoke();
            return true;
        }

        private void DisablePlayerMovement()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var controller = player.GetComponent<Player.PlayerController>();
                if (controller != null)
                {
                    controller.enabled = false;
                }

                var rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
            }
        }

        private void EnablePlayerMovement()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var controller = player.GetComponent<Player.PlayerController>();
                if (controller != null)
                {
                    controller.enabled = true;
                }

                var rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
        }

        private void MovePlayerOutside(SpaceStation station)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && station != null)
            {
                Vector3 exitPosition = station.transform.position + station.transform.forward * (dockingRange + 50f);
                player.transform.position = exitPosition;
                player.transform.rotation = Quaternion.LookRotation(-station.transform.forward);
            }
        }

        public void RegisterStation(SpaceStation station)
        {
            if (!stations.Contains(station))
            {
                stations.Add(station);
            }
        }

        public void UnregisterStation(SpaceStation station)
        {
            stations.Remove(station);
        }

        public SpaceStation GetNearestStation()
        {
            return nearestStation;
        }

        public List<SpaceStation> GetAllStations()
        {
            return new List<SpaceStation>(stations);
        }

        public float GetDistanceToStation(SpaceStation station)
        {
            Transform player = GetPlayerTransform();
            if (player == null || station == null) return float.MaxValue;

            return Vector3.Distance(player.position, station.transform.position);
        }
    }

    public class SpaceStation : MonoBehaviour
    {
        [Header("Station Info")]
        [SerializeField] private string stationId;
        [SerializeField] private string stationName;
        [SerializeField] private string factionId;
        [SerializeField] private StationType stationType;

        [Header("Services")]
        [SerializeField] private bool hasTrading = true;
        [SerializeField] private bool hasRepairs = true;
        [SerializeField] private bool hasMissions = true;
        [SerializeField] private bool hasCrafting = true;
        [SerializeField] private bool hasRefuel = true;

        [Header("Economy")]
        [SerializeField] private List<TradeItem> availableItems = new List<TradeItem>();

        public string StationId => stationId;
        public string StationName => stationName;
        public string FactionId => factionId;
        public StationType Type => stationType;
        public bool HasTrading => hasTrading;
        public bool HasRepairs => hasRepairs;
        public bool HasMissions => hasMissions;
        public bool HasCrafting => hasCrafting;
        public bool HasRefuel => hasRefuel;
        public List<TradeItem> AvailableItems => availableItems;

        private void Start()
        {
            if (StationManager.Instance != null)
            {
                StationManager.Instance.RegisterStation(this);
            }

            if (string.IsNullOrEmpty(stationId))
            {
                stationId = System.Guid.NewGuid().ToString();
            }
        }

        private void OnDestroy()
        {
            if (StationManager.Instance != null)
            {
                StationManager.Instance.UnregisterStation(this);
            }
        }

        public int GetRepairCost(float damagePercent)
        {
            return Mathf.RoundToInt(damagePercent * 100f);
        }

        public bool RepairShip(float damagePercent)
        {
            int cost = GetRepairCost(damagePercent);
            
            if (EconomyManager.Instance != null && EconomyManager.Instance.SpendCredits(cost))
            {
                // Find player health system and repair
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    var health = player.GetComponent<Combat.HealthSystem>();
                    if (health != null)
                    {
                        health.Heal(health.MaxHealth);
                        return true;
                    }
                }
            }
            return false;
        }

        public int GetRefuelCost(float fuelNeeded)
        {
            return Mathf.RoundToInt(fuelNeeded * 2f);
        }
    }

    public enum StationType
    {
        Trading,
        Military,
        Industrial,
        Mining,
        Research,
        Pirate
    }

    [System.Serializable]
    public class TradeItem
    {
        public string itemId;
        public string itemName;
        public int basePrice;
        public int stock;
        public int demand;
    }
}
