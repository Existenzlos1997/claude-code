using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Customization
{
    // Iteration 33-34: Loadout preset system
    public class LoadoutPresets : MonoBehaviour
    {
        [System.Serializable]
        public class Preset
        {
            public string name;
            public string[] weapons;
        }
        
        private List<Preset> presets = new List<Preset>();
        
        public void SavePreset(string name, string[] weapons)
        {
            presets.Add(new Preset { name = name, weapons = weapons });
            Debug.Log($"[Presets] Saved: {name}");
        }
        
        public Preset GetPreset(string name)
        {
            return presets.Find(p => p.name == name);
        }
    }
}
