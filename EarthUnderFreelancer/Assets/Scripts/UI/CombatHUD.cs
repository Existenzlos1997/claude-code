using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Complete HUD system for aircraft combat
    /// Shows speed, altitude, weapons, radar, damage, objectives
    /// </summary>
    
    public class CombatHUD : MonoBehaviour
    {
        public static CombatHUD Instance { get; private set; }
        
        [Header("Flight Info")]
        [SerializeField] private UnityEngine.UI.Text speedText;
        [SerializeField] private UnityEngine.UI.Text altitudeText;
        [SerializeField] private UnityEngine.UI.Text headingText;
        [SerializeField] private UnityEngine.UI.Text gForceText;
        [SerializeField] private UnityEngine.UI.Text throttleText;
        [SerializeField] private UnityEngine.UI.Slider throttleSlider;
        
        [Header("Aircraft Status")]
        [SerializeField] private UnityEngine.UI.Slider healthBar;
        [SerializeField] private UnityEngine.UI.Slider fuelBar;
        [SerializeField] private UnityEngine.UI.Text fuelText;
        [SerializeField] private UnityEngine.UI.Image engineStatusIcon;
        [SerializeField] private UnityEngine.UI.Image leftWingIcon;
        [SerializeField] private UnityEngine.UI.Image rightWingIcon;
        [SerializeField] private UnityEngine.UI.Image tailIcon;
        
        [Header("Weapons")]
        [SerializeField] private UnityEngine.UI.Text primaryAmmoText;
        [SerializeField] private UnityEngine.UI.Text secondaryAmmoText;
        [SerializeField] private UnityEngine.UI.Text selectedWeaponText;
        [SerializeField] private UnityEngine.UI.Image[] weaponSlotIcons;
        [SerializeField] private UnityEngine.UI.Slider weaponHeatBar;
        [SerializeField] private UnityEngine.UI.Text bombsText;
        [SerializeField] private UnityEngine.UI.Text rocketsText;
        
        [Header("Targeting")]
        [SerializeField] private RectTransform targetIndicator;
        [SerializeField] private RectTransform leadIndicator;
        [SerializeField] private UnityEngine.UI.Text targetNameText;
        [SerializeField] private UnityEngine.UI.Text targetDistanceText;
        [SerializeField] private UnityEngine.UI.Slider targetHealthBar;
        [SerializeField] private UnityEngine.UI.Text lockOnText;
        [SerializeField] private RectTransform missileWarningIndicator;
        
        [Header("Radar")]
        [SerializeField] private RectTransform radarContainer;
        [SerializeField] private GameObject radarBlipPrefab;
        [SerializeField] private float radarRange = 5000f;
        [SerializeField] private Color friendlyColor = Color.green;
        [SerializeField] private Color enemyColor = Color.red;
        [SerializeField] private Color neutralColor = Color.yellow;
        
        [Header("Objective")]
        [SerializeField] private UnityEngine.UI.Text objectiveText;
        [SerializeField] private RectTransform objectiveMarker;
        [SerializeField] private UnityEngine.UI.Text objectiveDistanceText;
        
        [Header("Messages")]
        [SerializeField] private UnityEngine.UI.Text killFeedText;
        [SerializeField] private UnityEngine.UI.Text alertText;
        [SerializeField] private float messageDuration = 3f;
        
        [Header("Score")]
        [SerializeField] private UnityEngine.UI.Text scoreText;
        [SerializeField] private UnityEngine.UI.Text killsText;
        [SerializeField] private UnityEngine.UI.Text assistsText;
        
        private Camera mainCamera;
        private Transform playerTransform;
        private List<GameObject> radarBlips = new List<GameObject>();
        private Queue<string> killFeedQueue = new Queue<string>();
        private float alertTimer = 0f;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        private void Start()
        {
            mainCamera = Camera.main;
            if (alertText != null) alertText.gameObject.SetActive(false);
            if (missileWarningIndicator != null) missileWarningIndicator.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            UpdateAlertTimer();
        }
        
        public void SetPlayerTransform(Transform player)
        {
            playerTransform = player;
        }
        
        public void UpdateFlightData(float speed, float altitude, float heading, float gForce, float throttle)
        {
            if (speedText != null) speedText.text = $"{speed:F0} km/h";
            if (altitudeText != null) altitudeText.text = $"{altitude:F0} m";
            if (headingText != null) headingText.text = $"{heading:F0}°";
            if (gForceText != null)
            {
                gForceText.text = $"{gForce:F1}G";
                gForceText.color = gForce > 7f ? Color.red : (gForce > 5f ? Color.yellow : Color.white);
            }
            if (throttleText != null) throttleText.text = $"{throttle * 100:F0}%";
            if (throttleSlider != null) throttleSlider.value = throttle;
        }
        
        public void UpdateHealth(float current, float max)
        {
            if (healthBar != null)
            {
                healthBar.value = current / max;
                var fill = healthBar.fillRect?.GetComponent<UnityEngine.UI.Image>();
                if (fill != null)
                {
                    float ratio = current / max;
                    fill.color = ratio > 0.5f ? Color.green : (ratio > 0.25f ? Color.yellow : Color.red);
                }
            }
        }
        
        public void UpdateFuel(float current, float max)
        {
            if (fuelBar != null) fuelBar.value = current / max;
            if (fuelText != null)
            {
                fuelText.text = $"{current:F0}L";
                fuelText.color = current / max < 0.2f ? Color.red : Color.white;
            }
        }
        
        public void UpdateDamageIndicators(float engine, float leftWing, float rightWing, float tail)
        {
            UpdateDamageIcon(engineStatusIcon, engine);
            UpdateDamageIcon(leftWingIcon, leftWing);
            UpdateDamageIcon(rightWingIcon, rightWing);
            UpdateDamageIcon(tailIcon, tail);
        }
        
        private void UpdateDamageIcon(UnityEngine.UI.Image icon, float health)
        {
            if (icon == null) return;
            icon.color = health > 0.7f ? Color.green : (health > 0.3f ? Color.yellow : Color.red);
        }
        
        public void UpdateWeapons(string weaponName, int primaryAmmo, int secondaryAmmo, float heat)
        {
            if (selectedWeaponText != null) selectedWeaponText.text = weaponName;
            if (primaryAmmoText != null) primaryAmmoText.text = $"{primaryAmmo}";
            if (secondaryAmmoText != null) secondaryAmmoText.text = $"{secondaryAmmo}";
            if (weaponHeatBar != null) weaponHeatBar.value = heat;
        }
        
        public void UpdateOrdnance(int bombs, int rockets)
        {
            if (bombsText != null) bombsText.text = $"Bombs: {bombs}";
            if (rocketsText != null) rocketsText.text = $"Rockets: {rockets}";
        }
        
        public void SetTarget(Transform target, string name, float distance, float health, float maxHealth, bool isLocked)
        {
            if (targetIndicator != null && target != null && mainCamera != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
                if (screenPos.z > 0)
                {
                    targetIndicator.gameObject.SetActive(true);
                    targetIndicator.position = screenPos;
                }
                else
                {
                    targetIndicator.gameObject.SetActive(false);
                }
            }
            
            if (targetNameText != null) targetNameText.text = name;
            if (targetDistanceText != null) targetDistanceText.text = $"{distance:F0}m";
            if (targetHealthBar != null) targetHealthBar.value = health / maxHealth;
            if (lockOnText != null)
            {
                lockOnText.gameObject.SetActive(isLocked);
                lockOnText.text = "LOCKED";
            }
        }
        
        public void SetLeadIndicator(Vector3 worldPosition, bool visible)
        {
            if (leadIndicator == null || mainCamera == null) return;
            
            if (visible)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
                if (screenPos.z > 0)
                {
                    leadIndicator.gameObject.SetActive(true);
                    leadIndicator.position = screenPos;
                }
                else
                {
                    leadIndicator.gameObject.SetActive(false);
                }
            }
            else
            {
                leadIndicator.gameObject.SetActive(false);
            }
        }
        
        public void ClearTarget()
        {
            if (targetIndicator != null) targetIndicator.gameObject.SetActive(false);
            if (leadIndicator != null) leadIndicator.gameObject.SetActive(false);
            if (targetNameText != null) targetNameText.text = "";
            if (targetDistanceText != null) targetDistanceText.text = "";
            if (lockOnText != null) lockOnText.gameObject.SetActive(false);
        }
        
        public void ShowMissileWarning(bool show, Vector3 direction)
        {
            if (missileWarningIndicator != null)
            {
                missileWarningIndicator.gameObject.SetActive(show);
                if (show && playerTransform != null)
                {
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    angle -= playerTransform.eulerAngles.y;
                    missileWarningIndicator.localRotation = Quaternion.Euler(0, 0, -angle);
                }
            }
        }
        
        public void UpdateRadar(List<RadarContact> contacts)
        {
            // Clear old blips
            foreach (var blip in radarBlips) Destroy(blip);
            radarBlips.Clear();
            
            if (radarContainer == null || radarBlipPrefab == null || playerTransform == null) return;
            
            foreach (var contact in contacts)
            {
                Vector3 relativePos = contact.position - playerTransform.position;
                float distance = relativePos.magnitude;
                
                if (distance > radarRange) continue;
                
                float normalizedX = relativePos.x / radarRange;
                float normalizedZ = relativePos.z / radarRange;
                
                // Rotate to player heading
                float heading = playerTransform.eulerAngles.y * Mathf.Deg2Rad;
                float rotatedX = normalizedX * Mathf.Cos(heading) - normalizedZ * Mathf.Sin(heading);
                float rotatedZ = normalizedX * Mathf.Sin(heading) + normalizedZ * Mathf.Cos(heading);
                
                var blip = Instantiate(radarBlipPrefab, radarContainer);
                var rect = blip.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = new Vector2(rotatedX * 50f, rotatedZ * 50f);
                }
                
                var image = blip.GetComponent<UnityEngine.UI.Image>();
                if (image != null)
                {
                    image.color = contact.isFriendly ? friendlyColor : enemyColor;
                }
                
                radarBlips.Add(blip);
            }
        }
        
        public void SetObjective(string text, Vector3? worldPosition = null)
        {
            if (objectiveText != null) objectiveText.text = text;
            
            if (objectiveMarker != null && worldPosition.HasValue && mainCamera != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition.Value);
                if (screenPos.z > 0)
                {
                    objectiveMarker.gameObject.SetActive(true);
                    objectiveMarker.position = screenPos;
                    
                    if (objectiveDistanceText != null && playerTransform != null)
                    {
                        float dist = Vector3.Distance(playerTransform.position, worldPosition.Value);
                        objectiveDistanceText.text = $"{dist:F0}m";
                    }
                }
                else
                {
                    objectiveMarker.gameObject.SetActive(false);
                }
            }
        }
        
        public void AddKillFeedMessage(string message)
        {
            killFeedQueue.Enqueue(message);
            if (killFeedQueue.Count > 5) killFeedQueue.Dequeue();
            
            if (killFeedText != null)
            {
                killFeedText.text = string.Join("\n", killFeedQueue.ToArray());
            }
        }
        
        public void ShowAlert(string message, Color color)
        {
            if (alertText != null)
            {
                alertText.text = message;
                alertText.color = color;
                alertText.gameObject.SetActive(true);
                alertTimer = messageDuration;
            }
        }
        
        private void UpdateAlertTimer()
        {
            if (alertTimer > 0)
            {
                alertTimer -= Time.deltaTime;
                if (alertTimer <= 0 && alertText != null)
                {
                    alertText.gameObject.SetActive(false);
                }
            }
        }
        
        public void UpdateScore(int score, int kills, int assists)
        {
            if (scoreText != null) scoreText.text = $"Score: {score}";
            if (killsText != null) killsText.text = $"Kills: {kills}";
            if (assistsText != null) assistsText.text = $"Assists: {assists}";
        }
        
        public void ShowStallWarning(bool show)
        {
            ShowAlert(show ? "STALL WARNING" : "", Color.red);
        }
        
        public void ShowOverspeedWarning(bool show)
        {
            ShowAlert(show ? "OVERSPEED" : "", Color.yellow);
        }
        
        public void ShowGearIndicator(bool deployed)
        {
            // Visual indicator for landing gear
        }
        
        public void ShowFlapsIndicator(int flapSetting)
        {
            // Visual indicator for flaps
        }
    }
    
    [Serializable]
    public class RadarContact
    {
        public string id;
        public Vector3 position;
        public bool isFriendly;
        public string type;
    }
}
