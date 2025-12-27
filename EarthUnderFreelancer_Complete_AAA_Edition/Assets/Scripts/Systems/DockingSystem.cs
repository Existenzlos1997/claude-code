using UnityEngine;
using EarthUnderFreelancer.Systems;
using EarthUnderFreelancer.Vehicles;
using EarthUnderFreelancer.Player;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Handles docking with space stations
    /// </summary>
    public class DockingSystem : MonoBehaviour
    {
        public static DockingSystem Instance { get; private set; }

        [Header("Docking State")]
        [SerializeField] private bool isDocked = false;
        [SerializeField] private SpaceStation currentStation;
        [SerializeField] private Transform dockedShip;

        [Header("Docking Settings")]
        [SerializeField] private float dockingRange = 100f;
        [SerializeField] private float dockingSpeed = 20f;
        [SerializeField] private float alignmentThreshold = 15f;
        [SerializeField] private float approachSpeed = 5f;

        [Header("UI")]
        [SerializeField] private GameObject dockingPrompt;
        [SerializeField] private UnityEngine.UI.Text promptText;
        [SerializeField] private GameObject stationMenuUI;

        [Header("References")]
        [SerializeField] private Transform playerShip;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private VehicleController vehicleController;

        private SpaceStation nearestStation;
        private bool canDock = false;
        private bool isDocking = false;
        private Vector3 dockingTarget;
        private Quaternion dockingRotation;

        public bool IsDocked => isDocked;
        public SpaceStation CurrentStation => currentStation;

        public event System.Action<SpaceStation> OnDockingStarted;
        public event System.Action<SpaceStation> OnDocked;
        public event System.Action OnUndocked;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            FindPlayerReferences();

            if (dockingPrompt != null)
                dockingPrompt.SetActive(false);
            if (stationMenuUI != null)
                stationMenuUI.SetActive(false);
        }

        private void FindPlayerReferences()
        {
            if (playerShip == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerShip = player.transform;
                    playerController = player.GetComponent<PlayerController>();
                    vehicleController = player.GetComponent<VehicleController>();
                }
            }
        }

        private void Update()
        {
            if (isDocked)
            {
                HandleDockedState();
            }
            else if (isDocking)
            {
                HandleDockingApproach();
            }
            else
            {
                CheckForNearbyStations();
                HandleDockingInput();
            }
        }

        private void CheckForNearbyStations()
        {
            if (playerShip == null) return;

            nearestStation = null;
            canDock = false;
            float nearestDistance = float.MaxValue;

            // Find all stations
            SpaceStation[] stations = FindObjectsByType<SpaceStation>(FindObjectsSortMode.None);

            foreach (var station in stations)
            {
                float distance = Vector3.Distance(playerShip.position, station.transform.position);
                
                if (distance < dockingRange && distance < nearestDistance)
                {
                    nearestStation = station;
                    nearestDistance = distance;
                }
            }

            if (nearestStation != null)
            {
                // Check if aligned with docking port
                Vector3 toDock = (nearestStation.DockingPoint.position - playerShip.position).normalized;
                float angle = Vector3.Angle(playerShip.forward, toDock);

                canDock = angle < alignmentThreshold && nearestDistance < dockingRange / 2;

                UpdateDockingPrompt(nearestDistance, angle);
            }
            else
            {
                if (dockingPrompt != null)
                    dockingPrompt.SetActive(false);
            }
        }

        private void UpdateDockingPrompt(float distance, float angle)
        {
            if (dockingPrompt == null) return;

            dockingPrompt.SetActive(true);

            if (promptText != null)
            {
                if (canDock)
                {
                    promptText.text = $"Press [F] to dock at {nearestStation.StationName}";
                    promptText.color = Color.green;
                }
                else
                {
                    string reason = distance > dockingRange / 2 ? 
                        $"Get closer ({distance:F0}m)" : 
                        $"Align with dock ({angle:F0}°)";
                    promptText.text = $"{nearestStation.StationName} - {reason}";
                    promptText.color = Color.yellow;
                }
            }
        }

        private void HandleDockingInput()
        {
            if (Input.GetKeyDown(KeyCode.F) && canDock && nearestStation != null)
            {
                StartDocking(nearestStation);
            }
        }

        public void StartDocking(SpaceStation station)
        {
            if (isDocking || isDocked) return;

            isDocking = true;
            currentStation = station;
            dockingTarget = station.DockingPoint.position;
            dockingRotation = station.DockingPoint.rotation;

            // Disable player control
            if (playerController != null)
                playerController.DisableControls();

            if (vehicleController != null)
                vehicleController.SetControllable(false);

            OnDockingStarted?.Invoke(station);

            if (dockingPrompt != null)
            {
                promptText.text = "Docking...";
                promptText.color = Color.cyan;
            }
        }

        private void HandleDockingApproach()
        {
            if (playerShip == null) return;

            // Move towards docking point
            Vector3 direction = (dockingTarget - playerShip.position).normalized;
            float distance = Vector3.Distance(playerShip.position, dockingTarget);

            if (distance > 1f)
            {
                playerShip.position = Vector3.MoveTowards(
                    playerShip.position, 
                    dockingTarget, 
                    approachSpeed * Time.deltaTime
                );

                // Rotate to align
                playerShip.rotation = Quaternion.RotateTowards(
                    playerShip.rotation,
                    dockingRotation,
                    60f * Time.deltaTime
                );
            }
            else
            {
                CompleteDocking();
            }
        }

        private void CompleteDocking()
        {
            isDocking = false;
            isDocked = true;
            dockedShip = playerShip;

            // Snap to docking position
            playerShip.position = dockingTarget;
            playerShip.rotation = dockingRotation;

            // Freeze physics
            Rigidbody rb = playerShip.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // Repair ship if station offers it
            if (currentStation.OffersRepairs)
            {
                HealthSystem health = playerShip.GetComponent<HealthSystem>();
                if (health != null)
                {
                    // Auto-repair could be implemented here
                }
            }

            // Update achievements
            if (AchievementSystem.Instance != null)
            {
                AchievementSystem.Instance.UpdateStatistic("stations_visited", 1);
            }

            OnDocked?.Invoke(currentStation);
            ShowStationMenu();

            if (dockingPrompt != null)
                dockingPrompt.SetActive(false);
        }

        public void Undock()
        {
            if (!isDocked) return;

            isDocked = false;
            
            // Unfreeze physics
            if (playerShip != null)
            {
                Rigidbody rb = playerShip.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    // Give a small push away from station
                    rb.AddForce(playerShip.forward * 10f, ForceMode.VelocityChange);
                }
            }

            // Re-enable player control
            if (playerController != null)
                playerController.EnableControls();

            if (vehicleController != null)
                vehicleController.SetControllable(true);

            HideStationMenu();

            currentStation = null;
            dockedShip = null;

            OnUndocked?.Invoke();
        }

        private void HandleDockedState()
        {
            // Check for undock input
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape))
            {
                Undock();
            }
        }

        private void ShowStationMenu()
        {
            if (stationMenuUI != null)
            {
                stationMenuUI.SetActive(true);
            }

            // Show cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Pause time (optional)
            // Time.timeScale = 0f;
        }

        private void HideStationMenu()
        {
            if (stationMenuUI != null)
            {
                stationMenuUI.SetActive(false);
            }

            // Hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Resume time
            // Time.timeScale = 1f;
        }

        public void SetPlayerShip(Transform ship)
        {
            playerShip = ship;
            if (ship != null)
            {
                playerController = ship.GetComponent<PlayerController>();
                vehicleController = ship.GetComponent<VehicleController>();
            }
        }

        public bool IsNearStation()
        {
            return nearestStation != null;
        }

        public SpaceStation GetNearestStation()
        {
            return nearestStation;
        }

        public float GetDockingProgress()
        {
            if (!isDocking || playerShip == null) return 0f;

            float distance = Vector3.Distance(playerShip.position, dockingTarget);
            float startDistance = dockingRange / 2;
            return 1f - Mathf.Clamp01(distance / startDistance);
        }
    }
}

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Space station component
    /// </summary>
    public class SpaceStation : MonoBehaviour
    {
        [Header("Station Info")]
        [SerializeField] private string stationId;
        [SerializeField] private string stationName = "Unknown Station";
        [SerializeField] private string factionId = "neutral";

        [Header("Docking")]
        [SerializeField] private Transform dockingPoint;

        [Header("Services")]
        [SerializeField] private bool offersRepairs = true;
        [SerializeField] private bool offersRefuel = true;
        [SerializeField] private bool offersTrading = true;
        [SerializeField] private bool offersMissions = true;
        [SerializeField] private bool offersUpgrades = true;

        [Header("Economy")]
        [SerializeField] private float repairCostMultiplier = 1f;
        [SerializeField] private float fuelCostMultiplier = 1f;
        [SerializeField] private float tradePriceModifier = 1f;

        public string StationId => stationId;
        public string StationName => stationName;
        public string FactionId => factionId;
        public Transform DockingPoint => dockingPoint ?? transform;
        public bool OffersRepairs => offersRepairs;
        public bool OffersRefuel => offersRefuel;
        public bool OffersTrading => offersTrading;
        public bool OffersMissions => offersMissions;
        public bool OffersUpgrades => offersUpgrades;

        private void Awake()
        {
            if (dockingPoint == null)
            {
                // Create default docking point
                GameObject dockPoint = new GameObject("DockingPoint");
                dockPoint.transform.SetParent(transform);
                dockPoint.transform.localPosition = new Vector3(0, 0, 50);
                dockPoint.transform.localRotation = Quaternion.identity;
                dockingPoint = dockPoint.transform;
            }

            // Ensure station has a collider for detection
            if (GetComponent<Collider>() == null)
            {
                SphereCollider col = gameObject.AddComponent<SphereCollider>();
                col.radius = 100f;
                col.isTrigger = true;
            }
        }

        public int GetRepairCost(float damageAmount)
        {
            return Mathf.RoundToInt(damageAmount * 10 * repairCostMultiplier);
        }

        public int GetRefuelCost(float fuelAmount)
        {
            return Mathf.RoundToInt(fuelAmount * 5 * fuelCostMultiplier);
        }

        public float GetTradePriceModifier(string itemId, bool buying)
        {
            // Could implement supply/demand per station
            return tradePriceModifier;
        }

        public void RepairShip(HealthSystem health)
        {
            if (!offersRepairs || health == null) return;

            float damage = health.MaxHealth - health.CurrentHealth;
            int cost = GetRepairCost(damage);

            if (EconomyManager.Instance != null && EconomyManager.Instance.SpendCredits(cost))
            {
                health.SetHealth(health.MaxHealth);
                health.SetShield(health.MaxShield);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw docking range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 100f);

            // Draw docking point
            if (dockingPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(dockingPoint.position, 5f);
                Gizmos.DrawLine(transform.position, dockingPoint.position);
            }
        }
    }
}
