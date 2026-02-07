using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Modding
{
    // Iteration 67-69: Mod loading system
    public class ModLoader : MonoBehaviour
    {
        private List<string> loadedMods = new List<string>();
        
        public void LoadMod(string modPath)
        {
            loadedMods.Add(modPath);
            Debug.Log($"[Mods] Loaded: {modPath}");
        }
        
        public List<string> GetLoadedMods() => loadedMods;
    }
}
