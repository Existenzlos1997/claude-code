using UnityEngine;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// ScriptableObject defining space station properties
    /// </summary>
    [CreateAssetMenu(fileName = "NewStationData", menuName = "EarthUnder/Station Data")]
    public class StationData : ScriptableObject
    {
        [Header("Identity")]
        public string stationId;
        public string stationName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        public GameObject prefab;
        public StationType stationType;

        [Header("Faction")]
        public string ownerFaction;
        public int reputationRequired = -50;

        [Header("Services")]
        public bool hasRepairService = true;
        public bool hasRefuelService = true;
        public bool hasTradingPost = true;
        public bool hasMissionBoard = true;
        public bool hasShipyard = false;
        public bool hasEquipmentDealer = true;

        [Header("Prices")]
        public float repairPriceModifier = 1f;
        public float tradePriceModifier = 1f;

        [Header("Inventory")]
        public StationItem[] inventory;

        [Header("Docking")]
        public float dockingRange = 200f;
        public Vector3[] dockingPoints;

        [Header("Defense")]
        public float hullStrength = 10000f;
        public float shieldStrength = 5000f;
    }

    [System.Serializable]
    public class StationItem
    {
        public string itemId;
        public int quantity;
        public int maxQuantity;
        public float priceModifier = 1f;
    }

    public enum StationType
    {
        Trading,
        Military,
        Mining,
        Research,
        Industrial,
        Pirate,
        Shipyard
    }
}
