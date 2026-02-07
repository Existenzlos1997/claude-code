using UnityEngine;

namespace EarthUnderFreelancer.Systems
{
    // Iteration 91-93: Save and load system
    public class SaveLoadSystem : MonoBehaviour
    {
        public void SaveGame(string slotName)
        {
            PlayerPrefs.SetString($"Save_{slotName}", System.DateTime.Now.ToString());
            PlayerPrefs.Save();
            Debug.Log($"[Save] Game saved to slot: {slotName}");
        }
        
        public void LoadGame(string slotName)
        {
            string saveData = PlayerPrefs.GetString($"Save_{slotName}", "");
            Debug.Log($"[Load] Loading from slot: {slotName}");
        }
    }
}
