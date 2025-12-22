using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Customization
{
    /// <summary>
    /// Aircraft customization system for paint, decals, and modifications
    /// </summary>
    public class AircraftCustomizationSystem : MonoBehaviour
    {
        public static AircraftCustomizationSystem Instance { get; private set; }

        private Dictionary<string, AircraftCustomization> playerCustomizations = new Dictionary<string, AircraftCustomization>();

        public event Action<AircraftCustomization> OnCustomizationChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public AircraftCustomization GetCustomization(string aircraftId)
        {
            if (!playerCustomizations.ContainsKey(aircraftId))
            {
                playerCustomizations[aircraftId] = new AircraftCustomization { AircraftId = aircraftId };
            }
            return playerCustomizations[aircraftId];
        }

        public void SetCustomization(string aircraftId, AircraftCustomization customization)
        {
            playerCustomizations[aircraftId] = customization;
            OnCustomizationChanged?.Invoke(customization);
        }

        public List<CustomizationItem> GetAvailablePaintSchemes()
        {
            return new List<CustomizationItem>
            {
                new CustomizationItem("paint_default", "Factory Default", CustomizationType.Paint, 0, true),
                new CustomizationItem("paint_olive", "Olive Drab", CustomizationType.Paint, 500, true),
                new CustomizationItem("camo_woodland", "Woodland Camo", CustomizationType.Paint, 2000, false),
                new CustomizationItem("camo_desert", "Desert Camo", CustomizationType.Paint, 2000, false),
                new CustomizationItem("camo_digital", "Digital Camo", CustomizationType.Paint, 3000, false)
            };
        }

        public List<CustomizationItem> GetAvailableDecals()
        {
            return new List<CustomizationItem>
            {
                new CustomizationItem("decal_roundel_us", "USAAF Roundel", CustomizationType.Decal, 0, true),
                new CustomizationItem("decal_roundel_uk", "RAF Roundel", CustomizationType.Decal, 0, true),
                new CustomizationItem("decal_kills_star", "Kill Stars", CustomizationType.Decal, 500, false),
                new CustomizationItem("decal_squadron_shark", "Shark Mouth", CustomizationType.Decal, 1500, false)
            };
        }
    }

    [Serializable]
    public class AircraftCustomization
    {
        public string AircraftId;
        public Color PrimaryColor = Color.gray;
        public Color SecondaryColor = Color.white;
        public string CamoPatternId;
        public List<DecalPlacement> Decals = new List<DecalPlacement>();
        public string NoseArtId;
        public string TailNumber;
        public float Weathering = 0.3f;
        public float Shine = 0.5f;
    }

    [Serializable]
    public class DecalPlacement
    {
        public string DecalId;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector2 Scale;
        public bool Mirrored;
    }

    [Serializable]
    public class CustomizationItem
    {
        public string Id;
        public string Name;
        public CustomizationType Type;
        public int Price;
        public bool IsDefault;

        public CustomizationItem(string id, string name, CustomizationType type, int price, bool isDefault)
        {
            Id = id;
            Name = name;
            Type = type;
            Price = price;
            IsDefault = isDefault;
        }
    }

    public enum CustomizationType { Paint, Decal, NoseArt, Modification }
}
