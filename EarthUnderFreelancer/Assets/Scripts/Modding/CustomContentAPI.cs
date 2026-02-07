using UnityEngine;

namespace EarthUnderFreelancer.Modding
{
    // Iteration 70-72: API for custom content
    public class CustomContentAPI : MonoBehaviour
    {
        public void RegisterCustomAircraft(GameObject aircraft)
        {
            Debug.Log($"[Mods] Registered aircraft: {aircraft.name}");
        }
        
        public void RegisterCustomWeapon(GameObject weapon)
        {
            Debug.Log($"[Mods] Registered weapon: {weapon.name}");
        }
    }
}
