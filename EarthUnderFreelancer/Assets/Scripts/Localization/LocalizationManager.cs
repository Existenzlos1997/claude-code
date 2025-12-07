using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Localization
{
    /// <summary>
    /// Complete localization system supporting multiple languages
    /// Includes text, audio, and UI localization
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private SystemLanguage defaultLanguage = SystemLanguage.English;
        [SerializeField] private bool useSystemLanguage = true;
        [SerializeField] private LocalizationData[] languageData;

        // Current state
        private SystemLanguage currentLanguage;
        private Dictionary<string, string> localizedStrings = new Dictionary<string, string>();
        private Dictionary<string, AudioClip> localizedAudio = new Dictionary<string, AudioClip>();
        private Dictionary<string, Sprite> localizedSprites = new Dictionary<string, Sprite>();

        // Events
        public event Action<SystemLanguage> OnLanguageChanged;

        // Supported languages
        public static readonly SystemLanguage[] SupportedLanguages = 
        {
            SystemLanguage.English,
            SystemLanguage.German,
            SystemLanguage.French,
            SystemLanguage.Spanish,
            SystemLanguage.Italian,
            SystemLanguage.Portuguese,
            SystemLanguage.Russian,
            SystemLanguage.Polish,
            SystemLanguage.Japanese,
            SystemLanguage.ChineseSimplified,
            SystemLanguage.ChineseTraditional,
            SystemLanguage.Korean,
            SystemLanguage.Turkish,
            SystemLanguage.Arabic
        };

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            // Determine initial language
            if (useSystemLanguage)
            {
                var systemLang = Application.systemLanguage;
                if (IsLanguageSupported(systemLang))
                {
                    currentLanguage = systemLang;
                }
                else
                {
                    currentLanguage = defaultLanguage;
                }
            }
            else
            {
                currentLanguage = defaultLanguage;
            }

            // Load saved language preference
            if (PlayerPrefs.HasKey("Language"))
            {
                int savedLang = PlayerPrefs.GetInt("Language");
                currentLanguage = (SystemLanguage)savedLang;
            }

            LoadLanguage(currentLanguage);
            
            // Initialize default strings if no data loaded
            if (localizedStrings.Count == 0)
            {
                LoadDefaultStrings();
            }
        }

        private void LoadDefaultStrings()
        {
            // UI - Main Menu
            AddString("ui.mainmenu.play", "Play", "Spielen", "Jouer", "Jugar");
            AddString("ui.mainmenu.campaign", "Campaign", "Kampagne", "Campagne", "Campaña");
            AddString("ui.mainmenu.multiplayer", "Multiplayer", "Mehrspieler", "Multijoueur", "Multijugador");
            AddString("ui.mainmenu.hangar", "Hangar", "Hangar", "Hangar", "Hangar");
            AddString("ui.mainmenu.settings", "Settings", "Einstellungen", "Paramètres", "Configuración");
            AddString("ui.mainmenu.exit", "Exit", "Beenden", "Quitter", "Salir");

            // UI - HUD
            AddString("ui.hud.speed", "Speed", "Geschwindigkeit", "Vitesse", "Velocidad");
            AddString("ui.hud.altitude", "Altitude", "Höhe", "Altitude", "Altitud");
            AddString("ui.hud.health", "Health", "Gesundheit", "Santé", "Salud");
            AddString("ui.hud.shields", "Shields", "Schilde", "Boucliers", "Escudos");
            AddString("ui.hud.ammo", "Ammo", "Munition", "Munitions", "Munición");
            AddString("ui.hud.fuel", "Fuel", "Treibstoff", "Carburant", "Combustible");
            AddString("ui.hud.target", "Target", "Ziel", "Cible", "Objetivo");

            // UI - Settings
            AddString("ui.settings.graphics", "Graphics", "Grafik", "Graphismes", "Gráficos");
            AddString("ui.settings.audio", "Audio", "Audio", "Audio", "Audio");
            AddString("ui.settings.controls", "Controls", "Steuerung", "Contrôles", "Controles");
            AddString("ui.settings.language", "Language", "Sprache", "Langue", "Idioma");
            AddString("ui.settings.quality", "Quality", "Qualität", "Qualité", "Calidad");
            AddString("ui.settings.low", "Low", "Niedrig", "Bas", "Bajo");
            AddString("ui.settings.medium", "Medium", "Mittel", "Moyen", "Medio");
            AddString("ui.settings.high", "High", "Hoch", "Élevé", "Alto");
            AddString("ui.settings.ultra", "Ultra", "Ultra", "Ultra", "Ultra");

            // UI - Trading
            AddString("ui.trading.buy", "Buy", "Kaufen", "Acheter", "Comprar");
            AddString("ui.trading.sell", "Sell", "Verkaufen", "Vendre", "Vender");
            AddString("ui.trading.price", "Price", "Preis", "Prix", "Precio");
            AddString("ui.trading.quantity", "Quantity", "Menge", "Quantité", "Cantidad");
            AddString("ui.trading.credits", "Credits", "Credits", "Crédits", "Créditos");

            // UI - Inventory
            AddString("ui.inventory.equip", "Equip", "Ausrüsten", "Équiper", "Equipar");
            AddString("ui.inventory.unequip", "Unequip", "Ablegen", "Déséquiper", "Desequipar");
            AddString("ui.inventory.drop", "Drop", "Ablegen", "Jeter", "Soltar");
            AddString("ui.inventory.use", "Use", "Benutzen", "Utiliser", "Usar");

            // UI - Mission
            AddString("ui.mission.accept", "Accept", "Annehmen", "Accepter", "Aceptar");
            AddString("ui.mission.decline", "Decline", "Ablehnen", "Refuser", "Rechazar");
            AddString("ui.mission.complete", "Complete", "Abschließen", "Terminer", "Completar");
            AddString("ui.mission.failed", "Failed", "Fehlgeschlagen", "Échoué", "Fallido");
            AddString("ui.mission.objectives", "Objectives", "Ziele", "Objectifs", "Objetivos");
            AddString("ui.mission.reward", "Reward", "Belohnung", "Récompense", "Recompensa");

            // Factions
            AddString("faction.federation", "United Federation", "Vereinigte Föderation", "Fédération Unie", "Federación Unida");
            AddString("faction.empire", "Imperial Dominion", "Imperiales Dominion", "Dominion Impérial", "Dominio Imperial");
            AddString("faction.rebels", "Free Pilots Alliance", "Freie Piloten-Allianz", "Alliance des Pilotes Libres", "Alianza de Pilotos Libres");
            AddString("faction.traders", "Merchant Guild", "Händlergilde", "Guilde des Marchands", "Gremio de Comerciantes");

            // Combat
            AddString("combat.kill", "Kill", "Abschuss", "Élimination", "Baja");
            AddString("combat.assist", "Assist", "Unterstützung", "Assistance", "Asistencia");
            AddString("combat.death", "Death", "Tod", "Mort", "Muerte");
            AddString("combat.victory", "Victory!", "Sieg!", "Victoire!", "¡Victoria!");
            AddString("combat.defeat", "Defeat", "Niederlage", "Défaite", "Derrota");

            // Aircraft
            AddString("aircraft.p51d", "P-51D Mustang", "P-51D Mustang", "P-51D Mustang", "P-51D Mustang");
            AddString("aircraft.bf109", "Bf 109", "Bf 109", "Bf 109", "Bf 109");
            AddString("aircraft.spitfire", "Spitfire", "Spitfire", "Spitfire", "Spitfire");
            AddString("aircraft.zero", "A6M Zero", "A6M Zero", "A6M Zero", "A6M Zero");
            AddString("aircraft.f86", "F-86 Sabre", "F-86 Sabre", "F-86 Sabre", "F-86 Sabre");
            AddString("aircraft.mig15", "MiG-15", "MiG-15", "MiG-15", "MiG-15");
            AddString("aircraft.f16", "F-16 Fighting Falcon", "F-16 Fighting Falcon", "F-16 Fighting Falcon", "F-16 Fighting Falcon");

            // Notifications
            AddString("notify.levelup", "Level Up!", "Level aufgestiegen!", "Niveau supérieur!", "¡Subida de nivel!");
            AddString("notify.achievement", "Achievement Unlocked", "Erfolg freigeschaltet", "Succès débloqué", "Logro desbloqueado");
            AddString("notify.newitems", "New Items Available", "Neue Gegenstände verfügbar", "Nouveaux objets disponibles", "Nuevos objetos disponibles");
            AddString("notify.missionstart", "Mission Started", "Mission gestartet", "Mission commencée", "Misión iniciada");
            AddString("notify.missioncomplete", "Mission Complete", "Mission abgeschlossen", "Mission terminée", "Misión completada");

            // Tutorial
            AddString("tutorial.welcome", "Welcome, pilot!", "Willkommen, Pilot!", "Bienvenue, pilote!", "¡Bienvenido, piloto!");
            AddString("tutorial.controls", "Learn the controls", "Lerne die Steuerung", "Apprenez les contrôles", "Aprende los controles");
            AddString("tutorial.combat", "Combat basics", "Kampf-Grundlagen", "Bases du combat", "Conceptos básicos de combate");
            AddString("tutorial.trading", "Trading guide", "Handelsführer", "Guide de commerce", "Guía de comercio");

            // Errors
            AddString("error.connection", "Connection failed", "Verbindung fehlgeschlagen", "Connexion échouée", "Conexión fallida");
            AddString("error.notenough", "Not enough credits", "Nicht genug Credits", "Pas assez de crédits", "No hay suficientes créditos");
            AddString("error.inventoryfull", "Inventory full", "Inventar voll", "Inventaire plein", "Inventario lleno");
        }

        private void AddString(string key, string en, string de = null, string fr = null, string es = null)
        {
            // This is a simplified version - in production, would load from files
            switch (currentLanguage)
            {
                case SystemLanguage.German:
                    localizedStrings[key] = de ?? en;
                    break;
                case SystemLanguage.French:
                    localizedStrings[key] = fr ?? en;
                    break;
                case SystemLanguage.Spanish:
                    localizedStrings[key] = es ?? en;
                    break;
                default:
                    localizedStrings[key] = en;
                    break;
            }
        }

        public void LoadLanguage(SystemLanguage language)
        {
            currentLanguage = language;
            localizedStrings.Clear();
            localizedAudio.Clear();
            localizedSprites.Clear();

            // Find language data
            var langData = Array.Find(languageData, d => d != null && d.language == language);
            if (langData != null)
            {
                foreach (var entry in langData.strings)
                {
                    localizedStrings[entry.key] = entry.value;
                }

                foreach (var entry in langData.audioClips)
                {
                    localizedAudio[entry.key] = entry.clip;
                }

                foreach (var entry in langData.sprites)
                {
                    localizedSprites[entry.key] = entry.sprite;
                }
            }
            else
            {
                // Load default strings for this language
                LoadDefaultStrings();
            }

            // Save preference
            PlayerPrefs.SetInt("Language", (int)language);
            PlayerPrefs.Save();

            OnLanguageChanged?.Invoke(language);
        }

        public static string GetString(string key)
        {
            if (Instance == null) return key;

            if (Instance.localizedStrings.TryGetValue(key, out string value))
            {
                return value;
            }

            Debug.LogWarning($"Localization key not found: {key}");
            return key;
        }

        public static string GetString(string key, params object[] args)
        {
            string format = GetString(key);
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }

        public static AudioClip GetAudio(string key)
        {
            if (Instance == null) return null;

            Instance.localizedAudio.TryGetValue(key, out AudioClip clip);
            return clip;
        }

        public static Sprite GetSprite(string key)
        {
            if (Instance == null) return null;

            Instance.localizedSprites.TryGetValue(key, out Sprite sprite);
            return sprite;
        }

        public SystemLanguage GetCurrentLanguage()
        {
            return currentLanguage;
        }

        public void SetLanguage(SystemLanguage language)
        {
            if (IsLanguageSupported(language))
            {
                LoadLanguage(language);
            }
        }

        public bool IsLanguageSupported(SystemLanguage language)
        {
            return Array.IndexOf(SupportedLanguages, language) >= 0;
        }

        public string GetLanguageName(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.English: return "English";
                case SystemLanguage.German: return "Deutsch";
                case SystemLanguage.French: return "Français";
                case SystemLanguage.Spanish: return "Español";
                case SystemLanguage.Italian: return "Italiano";
                case SystemLanguage.Portuguese: return "Português";
                case SystemLanguage.Russian: return "Русский";
                case SystemLanguage.Polish: return "Polski";
                case SystemLanguage.Japanese: return "日本語";
                case SystemLanguage.ChineseSimplified: return "简体中文";
                case SystemLanguage.ChineseTraditional: return "繁體中文";
                case SystemLanguage.Korean: return "한국어";
                case SystemLanguage.Turkish: return "Türkçe";
                case SystemLanguage.Arabic: return "العربية";
                default: return language.ToString();
            }
        }

        // Quick access for common strings
        public static string L(string key) => GetString(key);
    }

    [Serializable]
    public class LocalizationData : ScriptableObject
    {
        public SystemLanguage language;
        public StringEntry[] strings;
        public AudioEntry[] audioClips;
        public SpriteEntry[] sprites;
    }

    [Serializable]
    public class StringEntry
    {
        public string key;
        public string value;
    }

    [Serializable]
    public class AudioEntry
    {
        public string key;
        public AudioClip clip;
    }

    [Serializable]
    public class SpriteEntry
    {
        public string key;
        public Sprite sprite;
    }

    /// <summary>
    /// Component for automatically localizing UI text
    /// </summary>
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string localizationKey;
        [SerializeField] private bool autoUpdate = true;

        private UnityEngine.UI.Text uiText;
        private TMPro.TextMeshProUGUI tmpText;

        private void Awake()
        {
            uiText = GetComponent<UnityEngine.UI.Text>();
            tmpText = GetComponent<TMPro.TextMeshProUGUI>();
        }

        private void Start()
        {
            UpdateText();

            if (autoUpdate && LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
            }
        }

        private void OnDestroy()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
            }
        }

        private void OnLanguageChanged(SystemLanguage language)
        {
            UpdateText();
        }

        public void UpdateText()
        {
            string localizedString = LocalizationManager.GetString(localizationKey);

            if (uiText != null)
            {
                uiText.text = localizedString;
            }

            if (tmpText != null)
            {
                tmpText.text = localizedString;
            }
        }

        public void SetKey(string key)
        {
            localizationKey = key;
            UpdateText();
        }
    }
}
