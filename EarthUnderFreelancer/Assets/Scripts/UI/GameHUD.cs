using UnityEngine;
using UnityEngine.UI;
using EarthUnderFreelancer.Core;
using EarthUnderFreelancer.Vehicles;
using EarthUnderFreelancer.Combat;
using EarthUnderFreelancer.Missions;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// In-game HUD controller
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Health & Shield")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider shieldBar;
        [SerializeField] private Text healthText;
        [SerializeField] private Text shieldText;
        [SerializeField] private Image healthBarFill;
        [SerializeField] private Image shieldBarFill;

        [Header("Speed & Throttle")]
        [SerializeField] private Text speedText;
        [SerializeField] private Slider throttleBar;
        [SerializeField] private Image boostIndicator;

        [Header("Weapons")]
        [SerializeField] private Text primaryWeaponText;
        [SerializeField] private Text secondaryWeaponText;
        [SerializeField] private Text missileCountText;
        [SerializeField] private Image targetLockIndicator;
        [SerializeField] private Slider lockOnBar;

        [Header("Targeting")]
        [SerializeField] private GameObject targetPanel;
        [SerializeField] private Text targetNameText;
        [SerializeField] private Slider targetHealthBar;
        [SerializeField] private Text targetDistanceText;
        [SerializeField] private RectTransform targetReticle;

        [Header("Mission")]
        [SerializeField] private Text objectiveText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text timerText;

        [Header("Notifications")]
        [SerializeField] private Text notificationText;
        [SerializeField] private GameObject damageIndicator;

        [Header("Minimap")]
        [SerializeField] private RawImage minimapImage;
        [SerializeField] private Camera minimapCamera;

        private HealthSystem playerHealth;
        private VehicleController playerVehicle;
        private WeaponController playerWeapons;
        private float notificationTimer;

        private void Start()
        {
            FindPlayer();
            SubscribeToEvents();
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<HealthSystem>();
                playerVehicle = player.GetComponent<VehicleController>();
                playerWeapons = player.GetComponent<WeaponController>();

                if (playerHealth != null)
                {
                    playerHealth.OnHealthChanged += UpdateHealth;
                    playerHealth.OnShieldChanged += UpdateShield;
                }
            }
        }

        private void SubscribeToEvents()
        {
            EventManager.Subscribe(GameEvents.OBJECTIVE_UPDATED, OnObjectiveUpdated);
            EventManager.Subscribe(GameEvents.SHOW_NOTIFICATION, OnShowNotification);
            EventManager.Subscribe(GameEvents.DAMAGE_TAKEN, OnDamageTaken);
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdateHealth;
                playerHealth.OnShieldChanged -= UpdateShield;
            }

            EventManager.Unsubscribe(GameEvents.OBJECTIVE_UPDATED, OnObjectiveUpdated);
            EventManager.Unsubscribe(GameEvents.SHOW_NOTIFICATION, OnShowNotification);
            EventManager.Unsubscribe(GameEvents.DAMAGE_TAKEN, OnDamageTaken);
        }

        private void Update()
        {
            UpdateSpeedDisplay();
            UpdateWeaponDisplay();
            UpdateTargetDisplay();
            UpdateMissionDisplay();
            UpdateNotification();
        }

        private void UpdateHealth(float current, float max)
        {
            if (healthBar != null) healthBar.value = current / max;
            if (healthText != null) healthText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            
            if (healthBarFill != null)
            {
                float percent = current / max;
                healthBarFill.color = percent > 0.5f ? Color.green : percent > 0.25f ? Color.yellow : Color.red;
            }
        }

        private void UpdateShield(float current, float max)
        {
            if (shieldBar != null) shieldBar.value = max > 0 ? current / max : 0;
            if (shieldText != null) shieldText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
        }

        private void UpdateSpeedDisplay()
        {
            if (playerVehicle == null) return;

            if (speedText != null)
                speedText.text = $"{Mathf.RoundToInt(playerVehicle.CurrentSpeed)} m/s";
            if (throttleBar != null)
                throttleBar.value = playerVehicle.GetThrottlePercent();
            if (boostIndicator != null)
                boostIndicator.enabled = playerVehicle.IsBoosting;
        }

        private void UpdateWeaponDisplay()
        {
            if (playerWeapons == null) return;

            if (missileCountText != null)
                missileCountText.text = $"Missiles: {playerWeapons.MissileCount}";
            if (lockOnBar != null)
                lockOnBar.value = playerWeapons.LockOnProgress;
            if (targetLockIndicator != null)
                targetLockIndicator.enabled = playerWeapons.IsLockedOn;
        }

        private void UpdateTargetDisplay()
        {
            if (playerWeapons == null || playerWeapons.CurrentTarget == null)
            {
                targetPanel?.SetActive(false);
                if (targetReticle != null) targetReticle.gameObject.SetActive(false);
                return;
            }

            targetPanel?.SetActive(true);

            Transform target = playerWeapons.CurrentTarget;
            HealthSystem targetHealth = target.GetComponent<HealthSystem>();

            if (targetNameText != null)
                targetNameText.text = target.name;
            if (targetHealthBar != null && targetHealth != null)
                targetHealthBar.value = targetHealth.HealthPercent;
            if (targetDistanceText != null)
            {
                float distance = Vector3.Distance(playerVehicle.transform.position, target.position);
                targetDistanceText.text = $"{Mathf.RoundToInt(distance)}m";
            }

            // Update reticle position
            if (targetReticle != null && Camera.main != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
                if (screenPos.z > 0)
                {
                    targetReticle.gameObject.SetActive(true);
                    targetReticle.position = screenPos;
                }
                else
                {
                    targetReticle.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateMissionDisplay()
        {
            if (MissionManager.Instance == null || !MissionManager.Instance.IsMissionActive) return;

            if (scoreText != null)
                scoreText.text = $"Score: {MissionManager.Instance.CurrentScore}";
            
            if (timerText != null)
            {
                float time = MissionManager.Instance.MissionTimer;
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }

            if (objectiveText != null && MissionManager.Instance.ActiveObjectives.Count > 0)
            {
                var objective = MissionManager.Instance.ActiveObjectives[0];
                objectiveText.text = $"{objective.description}: {objective.currentCount}/{objective.targetCount}";
            }
        }

        private void UpdateNotification()
        {
            if (notificationTimer > 0)
            {
                notificationTimer -= Time.deltaTime;
                if (notificationTimer <= 0 && notificationText != null)
                    notificationText.text = "";
            }
        }

        private void OnObjectiveUpdated(object data)
        {
            if (data is MissionObjective objective)
            {
                ShowNotification($"Objective: {objective.description} - {objective.currentCount}/{objective.targetCount}", 3f);
            }
        }

        private void OnShowNotification(object data)
        {
            if (data is NotificationEventData notif)
            {
                ShowNotification(notif.message, notif.duration);
            }
        }

        private void OnDamageTaken(object data)
        {
            if (damageIndicator != null)
            {
                damageIndicator.SetActive(true);
                Invoke(nameof(HideDamageIndicator), 0.2f);
            }
        }

        private void HideDamageIndicator()
        {
            damageIndicator?.SetActive(false);
        }

        public void ShowNotification(string message, float duration = 3f)
        {
            if (notificationText != null)
            {
                notificationText.text = message;
                notificationTimer = duration;
            }
        }

        public void TogglePause()
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
                    GameManager.Instance.SetGameState(GameManager.GameState.Paused);
                else if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
                    GameManager.Instance.SetGameState(GameManager.GameState.Playing);
            }
        }
    }
}
