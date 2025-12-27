using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    // Iteration 97-99: Localization and translation
    public class LocalizationSystem : MonoBehaviour
    {
        private Dictionary<string, string> translations = new Dictionary<string, string>();
        private string currentLanguage = "en";
        
        public void SetLanguage(string lang)
        {
            currentLanguage = lang;
            Debug.Log($"[Localization] Language: {lang}");
        }
        
        public string GetText(string key)
        {
            return translations.ContainsKey(key) ? translations[key] : key;
        }
    }
}
