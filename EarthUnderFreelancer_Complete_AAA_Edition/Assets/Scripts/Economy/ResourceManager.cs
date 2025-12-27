using UnityEngine;

namespace EarthUnderFreelancer.Economy
{
    // Iterations 15-16: Resource and economy management
    public class ResourceManager : MonoBehaviour
    {
        private int credits = 10000;
        private int fuel = 100;
        private int ammo = 100;
        private int repairs = 100;
        
        public bool SpendCredits(int amount)
        {
            if (credits >= amount) { credits -= amount; return true; }
            return false;
        }
        
        public void EarnCredits(int amount) => credits += amount;
        public bool UseFuel(int amount) { if (fuel >= amount) { fuel -= amount; return true; } return false; }
        public bool UseAmmo(int amount) { if (ammo >= amount) { ammo -= amount; return true; } return false; }
        public void Refuel(int amount) => fuel = Mathf.Min(100, fuel + amount);
        public void Rearm(int amount) => ammo = Mathf.Min(100, ammo + amount);
        public int GetCredits() => credits;
        public int GetFuel() => fuel;
        public int GetAmmo() => ammo;
    }
}
