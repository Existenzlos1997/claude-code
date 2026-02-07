using UnityEngine;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Power distribution and management system
    /// Allocate power between weapons, shields, engines
    /// </summary>
    public class PowerManagementSystem : MonoBehaviour
    {
        [Header("Power Allocation")]
        [SerializeField] [Range(0f, 1f)] private float weaponsPower = 0.33f;
        [SerializeField] [Range(0f, 1f)] private float shieldsPower = 0.33f;
        [SerializeField] [Range(0f, 1f)] private float enginesPower = 0.33f;
        
        [Header("Power Settings")]
        [SerializeField] private float totalPower = 100f;
        [SerializeField] private float powerRegenRate = 5f;
        [SerializeField] private bool autoBalance = true;
        
        [Header("System Effects")]
        [SerializeField] private float weaponsPowerMultiplier = 1.5f;
        [SerializeField] private float shieldsPowerMultiplier = 2f;
        [SerializeField] private float enginesPowerMultiplier = 1.5f;
        
        private float currentPower = 100f;
        
        private void Start()
        {
            currentPower = totalPower;
            if (autoBalance)
            {
                BalancePower();
            }
        }
        
        private void Update()
        {
            // Regenerate power
            if (currentPower < totalPower)
            {
                currentPower = Mathf.Min(totalPower, currentPower + powerRegenRate * Time.deltaTime);
            }
            
            // Handle power distribution inputs
            HandleInput();
        }
        
        private void HandleInput()
        {
            // Quick power presets
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetPowerPreset(PowerPreset.Weapons);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetPowerPreset(PowerPreset.Shields);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetPowerPreset(PowerPreset.Engines);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetPowerPreset(PowerPreset.Balanced);
            }
        }
        
        public enum PowerPreset
        {
            Weapons,    // Max to weapons
            Shields,    // Max to shields
            Engines,    // Max to engines
            Balanced    // Even distribution
        }
        
        public void SetPowerPreset(PowerPreset preset)
        {
            switch (preset)
            {
                case PowerPreset.Weapons:
                    weaponsPower = 0.7f;
                    shieldsPower = 0.15f;
                    enginesPower = 0.15f;
                    Debug.Log("[Power] Weapons priority");
                    break;
                    
                case PowerPreset.Shields:
                    weaponsPower = 0.15f;
                    shieldsPower = 0.7f;
                    enginesPower = 0.15f;
                    Debug.Log("[Power] Shields priority");
                    break;
                    
                case PowerPreset.Engines:
                    weaponsPower = 0.15f;
                    shieldsPower = 0.15f;
                    enginesPower = 0.7f;
                    Debug.Log("[Power] Engines priority");
                    break;
                    
                case PowerPreset.Balanced:
                    BalancePower();
                    Debug.Log("[Power] Balanced");
                    break;
            }
            
            NormalizePower();
        }
        
        private void BalancePower()
        {
            weaponsPower = 0.33f;
            shieldsPower = 0.33f;
            enginesPower = 0.33f;
        }
        
        private void NormalizePower()
        {
            float total = weaponsPower + shieldsPower + enginesPower;
            if (total > 0)
            {
                weaponsPower /= total;
                shieldsPower /= total;
                enginesPower /= total;
            }
        }
        
        public float GetWeaponsPowerLevel()
        {
            return weaponsPower * weaponsPowerMultiplier;
        }
        
        public float GetShieldsPowerLevel()
        {
            return shieldsPower * shieldsPowerMultiplier;
        }
        
        public float GetEnginesPowerLevel()
        {
            return enginesPower * enginesPowerMultiplier;
        }
        
        public bool ConsumePower(float amount)
        {
            if (currentPower >= amount)
            {
                currentPower -= amount;
                return true;
            }
            return false;
        }
        
        private void OnGUI()
        {
            // Simple power display
            float y = 150f;
            GUI.Label(new Rect(20, y, 200, 20), $"Power: {currentPower:F0}/{totalPower:F0}");
            y += 25f;
            GUI.Label(new Rect(20, y, 200, 20), $"Weapons: {weaponsPower * 100f:F0}%");
            y += 20f;
            GUI.Label(new Rect(20, y, 200, 20), $"Shields: {shieldsPower * 100f:F0}%");
            y += 20f;
            GUI.Label(new Rect(20, y, 200, 20), $"Engines: {enginesPower * 100f:F0}%");
            y += 25f;
            GUI.Label(new Rect(20, y, 200, 20), "1-4: Power presets");
        }
    }
}
