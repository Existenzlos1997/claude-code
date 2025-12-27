using UnityEngine;
using UnityEngine.UI;
using EarthUnderFreelancer.Vehicles;
using EarthUnderFreelancer.Combat;
using EarthUnderFreelancer.Systems;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Enhanced game HUD with all flight, combat and status information
    /// </summary>
    public class EnhancedGameHUD : MonoBehaviour
    {
        [Header("Player References")]
        [SerializeField] private VehicleController playerVehicle;
        [SerializeField] private HealthSystem playerHealth;
        [SerializeField] private WeaponController playerWeapons;
        [SerializeField] private TargetingSystem targetingSystem;

        [Header("Health Bars")]
        [SerializeField] private Slider hullBar;
        [SerializeField] private Slider shieldBar;
        [SerializeField] private Text hullText;
        [SerializeField] private Text shieldText;
        [SerializeField] private Image hullBarFill;
        [SerializeField] private Image shieldBarFill;

        [Header("Speed & Throttle")]
        [SerializeField] private Slider speedBar;
        [SerializeField] private Slider throttleBar;
        [SerializeField] private Slider boostBar;
        [SerializeField] private Text speedText;
        [SerializeField] private Text throttleText;
        [SerializeField] private Image boostFill;

        [Header("Weapons")]
        [SerializeField] private Text weaponNameText;
        [SerializeField] private Text ammoText;
        [SerializeField] private Slider weaponHeatBar;
        [SerializeField] private Slider missileCountBar;
        [SerializeField] private Image weaponIcon;
        [SerializeField] private Image crosshair;

        [Header("Target Display")]
        [SerializeField] private GameObject targetPanel;
        [SerializeField] private Text targetNameText;
        [SerializeField] private Text targetDistanceText;
        [SerializeField] private Slider targetHullBar;
        [SerializeField] private Slider targetShieldBar;
        [SerializeField] private Text targetHullText;
        [SerializeField] private Text targetShieldText;
        [SerializeField] private Image targetIcon;
        [SerializeField] private RectTransform leadIndicator;

        [Header("Mission Info")]
        [SerializeField] private GameObject missionPanel;
        [SerializeField] private Text missionNameText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private Text missionTimerText;

        [Header("Status Info")]
        [SerializeField] private Text creditsText;
        [SerializeField] private Text locationText;
        [SerializeField] private GameObject warningPanel;
        [SerializeField] private Text warningText;
        [SerializeField] private GameObject damageIndicator;

        [Header("Radar")]
        [SerializeField] private RadarSystem radarSystem;
        [SerializeField] private Text radarRangeText;
        [SerializeField] private Text enemyCountText;

        [Header("Notifications")]
        [SerializeField] private Text notificationText;
        [SerializeField] private float notificationDuration = 3f;

        [Header("Colors")]
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private Color shieldColor = Color.cyan;

        private float notificationTimer;
        private float lastHullValue;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            FindPlayerReferences();
            InitializeUI();
        }

        private void FindPlayerReferences()
        {
            if (playerVehicle == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerVehicle = player.GetComponent<VehicleController>();
                    playerHealth = player.GetComponent<HealthSystem>();
                    playerWeapons = player.GetComponent<WeaponController>();
                    targetingSystem = player.GetComponent<TargetingSystem>();
                }
            }
        }

        private void InitializeUI()
        {
            if (targetPanel != null)
                targetPanel.SetActive(false);
            if (warningPanel != null)
                warningPanel.SetActive(false);
            if (damageIndicator != null)
                damageIndicator.SetActive(false);
            if (notificationText != null)
                notificationText.gameObject.SetActive(false);
        }

        private void Update()
        {
            UpdateHealthDisplay();
            UpdateSpeedDisplay();
            UpdateWeaponDisplay();
            UpdateTargetDisplay();
            UpdateMissionDisplay();
            UpdateStatusDisplay();
            UpdateNotifications();
            UpdateWarnings();
        }

        private void UpdateHealthDisplay()
        {
            if (playerHealth == null) return;

            float hullPercent = playerHealth.CurrentHealth / playerHealth.MaxHealth;
            float shieldPercent = playerHealth.CurrentShield / playerHealth.MaxShield;

            if (hullBar != null)
            {
                hullBar.value = hullPercent;
            }

            if (shieldBar != null)
            {
                shieldBar.value = shieldPercent;
            }

            if (hullText != null)
            {
                hullText.text = $"{playerHealth.CurrentHealth:F0}/{playerHealth.MaxHealth:F0}";
            }

            if (shieldText != null)
            {
                shieldText.text = $"{playerHealth.CurrentShield:F0}/{playerHealth.MaxShield:F0}";
            }

            // Color based on health
            if (hullBarFill != null)
            {
                if (hullPercent > 0.5f)
                    hullBarFill.color = healthyColor;
                else if (hullPercent > 0.25f)
                    hullBarFill.color = warningColor;
                else
                    hullBarFill.color = criticalColor;
            }

            // Damage flash
            if (playerHealth.CurrentHealth < lastHullValue)
            {
                ShowDamageIndicator();
            }
            lastHullValue = playerHealth.CurrentHealth;
        }

        private void UpdateSpeedDisplay()
        {
            if (playerVehicle == null) return;

            float speedPercent = playerVehicle.GetSpeedPercent();
            float throttlePercent = playerVehicle.GetThrottlePercent();

            if (speedBar != null)
            {
                speedBar.value = speedPercent;
            }

            if (throttleBar != null)
            {
                throttleBar.value = throttlePercent;
            }

            if (speedText != null)
            {
                speedText.text = $"{playerVehicle.CurrentSpeed:F0} m/s";
            }

            if (throttleText != null)
            {
                throttleText.text = $"{throttlePercent * 100:F0}%";
            }

            if (boostBar != null)
            {
                boostBar.value = playerVehicle.BoostAmount;
            }

            if (boostFill != null)
            {
                boostFill.color = playerVehicle.IsBoosting ? Color.yellow : Color.white;
            }
        }

        private void UpdateWeaponDisplay()
        {
            if (playerWeapons == null) return;

            if (weaponNameText != null)
            {
                weaponNameText.text = playerWeapons.CurrentWeaponName;
            }

            if (ammoText != null)
            {
                if (playerWeapons.HasUnlimitedAmmo)
                {
                    ammoText.text = "∞";
                }
                else
                {
                    ammoText.text = $"{playerWeapons.CurrentAmmo}/{playerWeapons.MaxAmmo}";
                }
            }

            if (weaponHeatBar != null)
            {
                weaponHeatBar.value = playerWeapons.HeatLevel;
            }
        }

        private void UpdateTargetDisplay()
        {
            if (targetingSystem == null)
            {
                if (targetPanel != null)
                    targetPanel.SetActive(false);
                return;
            }

            Transform target = targetingSystem.CurrentTarget;
            
            if (target == null)
            {
                if (targetPanel != null)
                    targetPanel.SetActive(false);
                if (leadIndicator != null)
                    leadIndicator.gameObject.SetActive(false);
                return;
            }

            if (targetPanel != null)
                targetPanel.SetActive(true);

            // Target name
            if (targetNameText != null)
            {
                targetNameText.text = target.name;
            }

            // Distance
            if (targetDistanceText != null && playerVehicle != null)
            {
                float distance = Vector3.Distance(playerVehicle.transform.position, target.position);
                if (distance > 1000f)
                {
                    targetDistanceText.text = $"{distance / 1000f:F1} km";
                }
                else
                {
                    targetDistanceText.text = $"{distance:F0} m";
                }
            }

            // Target health
            HealthSystem targetHealth = target.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                if (targetHullBar != null)
                {
                    targetHullBar.value = targetHealth.CurrentHealth / targetHealth.MaxHealth;
                }
                if (targetShieldBar != null)
                {
                    targetShieldBar.value = targetHealth.CurrentShield / targetHealth.MaxShield;
                }
                if (targetHullText != null)
                {
                    targetHullText.text = $"{targetHealth.CurrentHealth:F0}";
                }
                if (targetShieldText != null)
                {
                    targetShieldText.text = $"{targetHealth.CurrentShield:F0}";
                }
            }

            // Lead indicator
            UpdateLeadIndicator(target);
        }

        private void UpdateLeadIndicator(Transform target)
        {
            if (leadIndicator == null || mainCamera == null || playerWeapons == null) return;

            Vector3 leadPos = targetingSystem.GetLeadPosition();
            
            // Check if lead position is in front of camera
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(leadPos);
            
            if (viewportPos.z > 0 && viewportPos.x >= 0 && viewportPos.x <= 1 && 
                viewportPos.y >= 0 && viewportPos.y <= 1)
            {
                leadIndicator.gameObject.SetActive(true);
                Vector3 screenPos = mainCamera.WorldToScreenPoint(leadPos);
                leadIndicator.position = screenPos;
            }
            else
            {
                leadIndicator.gameObject.SetActive(false);
            }
        }

        private void UpdateMissionDisplay()
        {
            if (MissionSystem.Instance == null)
            {
                if (missionPanel != null)
                    missionPanel.SetActive(false);
                return;
            }

            var activeMissions = MissionSystem.Instance.ActiveMissions;
            
            if (activeMissions.Count == 0)
            {
                if (missionPanel != null)
                    missionPanel.SetActive(false);
                return;
            }

            var mission = activeMissions[0];

            if (missionPanel != null)
                missionPanel.SetActive(true);

            if (missionNameText != null)
            {
                missionNameText.text = mission.missionName;
            }

            if (objectiveText != null && mission.objectives.Count > 0)
            {
                var obj = mission.objectives[0];
                objectiveText.text = $"{obj.description}: {obj.currentCount}/{obj.targetCount}";
            }

            if (missionTimerText != null)
            {
                if (mission.timeLimit > 0)
                {
                    int minutes = Mathf.FloorToInt(mission.timeRemaining / 60);
                    int seconds = Mathf.FloorToInt(mission.timeRemaining % 60);
                    missionTimerText.text = $"{minutes:00}:{seconds:00}";
                    missionTimerText.color = mission.timeRemaining < 60 ? criticalColor : Color.white;
                }
                else
                {
                    missionTimerText.text = "";
                }
            }
        }

        private void UpdateStatusDisplay()
        {
            if (creditsText != null && EconomyManager.Instance != null)
            {
                creditsText.text = $"${EconomyManager.Instance.Credits:N0}";
            }

            if (radarSystem != null && enemyCountText != null)
            {
                int enemyCount = radarSystem.GetContactCount(RadarContactType.Enemy);
                enemyCountText.text = $"Hostiles: {enemyCount}";
                enemyCountText.color = enemyCount > 0 ? warningColor : healthyColor;
            }
        }

        private void UpdateWarnings()
        {
            if (playerHealth == null || warningPanel == null) return;

            float hullPercent = playerHealth.CurrentHealth / playerHealth.MaxHealth;

            if (hullPercent < 0.25f)
            {
                warningPanel.SetActive(true);
                if (warningText != null)
                {
                    warningText.text = "CRITICAL DAMAGE";
                }
            }
            else if (playerHealth.CurrentShield <= 0 && playerHealth.MaxShield > 0)
            {
                warningPanel.SetActive(true);
                if (warningText != null)
                {
                    warningText.text = "SHIELDS DOWN";
                }
            }
            else
            {
                warningPanel.SetActive(false);
            }
        }

        private void UpdateNotifications()
        {
            if (notificationText == null) return;

            if (notificationTimer > 0)
            {
                notificationTimer -= Time.deltaTime;
                if (notificationTimer <= 0)
                {
                    notificationText.gameObject.SetActive(false);
                }
            }
        }

        public void ShowNotification(string message, Color? color = null)
        {
            if (notificationText == null) return;

            notificationText.text = message;
            notificationText.color = color ?? Color.white;
            notificationText.gameObject.SetActive(true);
            notificationTimer = notificationDuration;
        }

        private void ShowDamageIndicator()
        {
            if (damageIndicator == null) return;

            damageIndicator.SetActive(true);
            Invoke(nameof(HideDamageIndicator), 0.2f);
        }

        private void HideDamageIndicator()
        {
            if (damageIndicator != null)
            {
                damageIndicator.SetActive(false);
            }
        }

        public void SetPlayer(GameObject player)
        {
            if (player != null)
            {
                playerVehicle = player.GetComponent<VehicleController>();
                playerHealth = player.GetComponent<HealthSystem>();
                playerWeapons = player.GetComponent<WeaponController>();
                targetingSystem = player.GetComponent<TargetingSystem>();
            }
        }
    }
}
