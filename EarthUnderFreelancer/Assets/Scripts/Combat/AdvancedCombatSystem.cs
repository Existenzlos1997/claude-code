using UnityEngine;

namespace EarthUnderFreelancer.Combat
{
    public enum AmmoType { Standard, ArmorPiercing, HighExplosive, Incendiary }
    public enum WeakPoint { Engine, Cockpit, Wings, Tail, Fuel }
    
    public class AdvancedCombatSystem : MonoBehaviour
    {
        [Header("Weapon System")]
        public AmmoType currentAmmoType = AmmoType.Standard;
        public int ammoCount = 500;
        
        [Header("Electronic Warfare")]
        public bool radarJamming = false;
        public float jammingRadius = 1000f;
        
        [Header("Countermeasures")]
        public int flaresRemaining = 10;
        public int chaffRemaining = 10;
        
        public void FireWeapon(Transform target)
        {
            if (ammoCount <= 0) return;
            
            float damageMultiplier = GetAmmoMultiplier(currentAmmoType);
            float baseDamage = 10f;
            
            // Check for weak point hit
            WeakPoint hitPoint = DetermineHitPoint();
            float weakPointMultiplier = GetWeakPointMultiplier(hitPoint);
            
            float totalDamage = baseDamage * damageMultiplier * weakPointMultiplier;
            
            // Apply damage
            var targetHealth = target.GetComponent<FlightPhysicsEngine>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(totalDamage / 100f);
                Debug.Log($"Hit {hitPoint} with {currentAmmoType} for {totalDamage} damage");
            }
            
            ammoCount--;
        }
        
        private float GetAmmoMultiplier(AmmoType type)
        {
            switch (type)
            {
                case AmmoType.ArmorPiercing: return 1.5f;
                case AmmoType.HighExplosive: return 2.0f;
                case AmmoType.Incendiary: return 1.3f;
                default: return 1.0f;
            }
        }
        
        private WeakPoint DetermineHitPoint()
        {
            float roll = Random.value;
            if (roll < 0.1f) return WeakPoint.Engine;
            if (roll < 0.15f) return WeakPoint.Cockpit;
            if (roll < 0.3f) return WeakPoint.Wings;
            if (roll < 0.4f) return WeakPoint.Tail;
            if (roll < 0.5f) return WeakPoint.Fuel;
            return WeakPoint.Wings; // Default
        }
        
        private float GetWeakPointMultiplier(WeakPoint point)
        {
            switch (point)
            {
                case WeakPoint.Engine: return 3.0f;
                case WeakPoint.Cockpit: return 5.0f;
                case WeakPoint.Fuel: return 4.0f;
                default: return 1.0f;
            }
        }
        
        public void DeployFlares()
        {
            if (flaresRemaining > 0)
            {
                flaresRemaining--;
                Debug.Log("Flares deployed! Missiles diverted.");
            }
        }
        
        public void DeployChaff()
        {
            if (chaffRemaining > 0)
            {
                chaffRemaining--;
                Debug.Log("Chaff deployed! Radar lock broken.");
            }
        }
        
        public void ToggleJamming()
        {
            radarJamming = !radarJamming;
            if (radarJamming)
                Debug.Log($"Radar jamming active in {jammingRadius}m radius");
        }
    }
}
