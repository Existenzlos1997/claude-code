using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Radar/Minimap system for showing nearby objects
    /// </summary>
    public class RadarSystem : MonoBehaviour
    {
        public static RadarSystem Instance { get; private set; }

        [Header("Radar Settings")]
        [SerializeField] private float radarRange = 1000f;
        [SerializeField] private float updateInterval = 0.1f;
        [SerializeField] private float radarSize = 200f;

        [Header("UI References")]
        [SerializeField] private RectTransform radarContainer;
        [SerializeField] private RectTransform playerBlip;

        [Header("Blip Prefabs")]
        [SerializeField] private GameObject enemyBlipPrefab;
        [SerializeField] private GameObject friendlyBlipPrefab;
        [SerializeField] private GameObject neutralBlipPrefab;
        [SerializeField] private GameObject stationBlipPrefab;
        [SerializeField] private GameObject asteroidBlipPrefab;
        [SerializeField] private GameObject missionBlipPrefab;
        [SerializeField] private GameObject lootBlipPrefab;

        [Header("Colors")]
        [SerializeField] private Color enemyColor = Color.red;
        [SerializeField] private Color friendlyColor = Color.green;
        [SerializeField] private Color neutralColor = Color.yellow;
        [SerializeField] private Color stationColor = Color.cyan;
        [SerializeField] private Color asteroidColor = Color.gray;
        [SerializeField] private Color missionColor = Color.magenta;
        [SerializeField] private Color lootColor = Color.yellow;

        [Header("Tracking")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private List<RadarContact> contacts = new List<RadarContact>();

        private Dictionary<Transform, RadarBlip> activeBlips = new Dictionary<Transform, RadarBlip>();
        private float lastUpdateTime;
        private ObjectPool blipPool;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Find player if not assigned
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }

            // Create default blip prefab if not assigned
            CreateDefaultBlips();
        }

        private void CreateDefaultBlips()
        {
            if (enemyBlipPrefab == null)
            {
                enemyBlipPrefab = CreateBlipPrefab("EnemyBlip", enemyColor);
            }
            if (friendlyBlipPrefab == null)
            {
                friendlyBlipPrefab = CreateBlipPrefab("FriendlyBlip", friendlyColor);
            }
            if (neutralBlipPrefab == null)
            {
                neutralBlipPrefab = CreateBlipPrefab("NeutralBlip", neutralColor);
            }
            if (stationBlipPrefab == null)
            {
                stationBlipPrefab = CreateBlipPrefab("StationBlip", stationColor, true);
            }
        }

        private GameObject CreateBlipPrefab(string name, Color color, bool isSquare = false)
        {
            GameObject blip = new GameObject(name);
            blip.SetActive(false);

            RectTransform rect = blip.AddComponent<RectTransform>();
            rect.sizeDelta = isSquare ? new Vector2(8, 8) : new Vector2(6, 6);

            UnityEngine.UI.Image image = blip.AddComponent<UnityEngine.UI.Image>();
            image.color = color;

            return blip;
        }

        private void Update()
        {
            if (playerTransform == null) return;

            if (Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateRadar();
                lastUpdateTime = Time.time;
            }

            // Update blip positions every frame for smooth movement
            UpdateBlipPositions();
        }

        private void UpdateRadar()
        {
            // Find all radar-visible objects
            ScanForContacts();

            // Update active blips
            UpdateBlips();
        }

        private void ScanForContacts()
        {
            contacts.Clear();

            // Scan for enemies
            ScanObjects("Enemy", RadarContactType.Enemy);

            // Scan for friendly ships
            ScanObjects("Friendly", RadarContactType.Friendly);

            // Scan for stations
            ScanObjects("Station", RadarContactType.Station);

            // Scan for asteroids
            ScanObjects("Asteroid", RadarContactType.Asteroid);

            // Scan for loot
            ScanObjects("Loot", RadarContactType.Loot);

            // Scan for mission objectives
            ScanObjects("Objective", RadarContactType.MissionObjective);
        }

        private void ScanObjects(string tag, RadarContactType type)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (var obj in objects)
            {
                if (obj.transform == playerTransform) continue;

                float distance = Vector3.Distance(playerTransform.position, obj.transform.position);
                if (distance <= radarRange)
                {
                    contacts.Add(new RadarContact
                    {
                        target = obj.transform,
                        contactType = type,
                        distance = distance
                    });
                }
            }
        }

        private void UpdateBlips()
        {
            // Mark all existing blips as potentially unused
            HashSet<Transform> currentContacts = new HashSet<Transform>();

            foreach (var contact in contacts)
            {
                currentContacts.Add(contact.target);

                if (!activeBlips.ContainsKey(contact.target))
                {
                    // Create new blip
                    RadarBlip blip = CreateBlip(contact);
                    if (blip != null)
                    {
                        activeBlips[contact.target] = blip;
                    }
                }
            }

            // Remove blips that are no longer in range
            List<Transform> toRemove = new List<Transform>();
            foreach (var kvp in activeBlips)
            {
                if (!currentContacts.Contains(kvp.Key) || kvp.Key == null)
                {
                    if (kvp.Value.blipObject != null)
                    {
                        Destroy(kvp.Value.blipObject);
                    }
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var key in toRemove)
            {
                activeBlips.Remove(key);
            }
        }

        private RadarBlip CreateBlip(RadarContact contact)
        {
            GameObject prefab = GetBlipPrefab(contact.contactType);
            if (prefab == null) return null;

            GameObject blipObj = Instantiate(prefab, radarContainer);
            blipObj.SetActive(true);

            RectTransform rect = blipObj.GetComponent<RectTransform>();

            return new RadarBlip
            {
                blipObject = blipObj,
                rectTransform = rect,
                contactType = contact.contactType
            };
        }

        private GameObject GetBlipPrefab(RadarContactType type)
        {
            switch (type)
            {
                case RadarContactType.Enemy: return enemyBlipPrefab;
                case RadarContactType.Friendly: return friendlyBlipPrefab;
                case RadarContactType.Neutral: return neutralBlipPrefab;
                case RadarContactType.Station: return stationBlipPrefab;
                case RadarContactType.Asteroid: return asteroidBlipPrefab;
                case RadarContactType.MissionObjective: return missionBlipPrefab;
                case RadarContactType.Loot: return lootBlipPrefab;
                default: return neutralBlipPrefab;
            }
        }

        private void UpdateBlipPositions()
        {
            if (playerTransform == null) return;

            foreach (var contact in contacts)
            {
                if (activeBlips.TryGetValue(contact.target, out RadarBlip blip))
                {
                    if (blip.blipObject == null || contact.target == null) continue;

                    // Calculate relative position
                    Vector3 relativePos = contact.target.position - playerTransform.position;

                    // Rotate to match player's facing direction
                    float playerYaw = playerTransform.eulerAngles.y;
                    Vector3 rotatedPos = Quaternion.Euler(0, -playerYaw, 0) * relativePos;

                    // Normalize to radar size
                    float normalizedX = (rotatedPos.x / radarRange) * (radarSize / 2);
                    float normalizedY = (rotatedPos.z / radarRange) * (radarSize / 2);

                    // Clamp to radar bounds
                    Vector2 blipPos = new Vector2(normalizedX, normalizedY);
                    if (blipPos.magnitude > radarSize / 2)
                    {
                        blipPos = blipPos.normalized * (radarSize / 2);
                    }

                    blip.rectTransform.anchoredPosition = blipPos;

                    // Fade based on distance
                    float alpha = 1f - (contact.distance / radarRange) * 0.5f;
                    var image = blip.blipObject.GetComponent<UnityEngine.UI.Image>();
                    if (image != null)
                    {
                        Color c = image.color;
                        c.a = alpha;
                        image.color = c;
                    }
                }
            }
        }

        public void SetRadarRange(float range)
        {
            radarRange = range;
        }

        public void SetPlayerTransform(Transform player)
        {
            playerTransform = player;
        }

        public List<RadarContact> GetContactsOfType(RadarContactType type)
        {
            List<RadarContact> result = new List<RadarContact>();
            foreach (var contact in contacts)
            {
                if (contact.contactType == type)
                {
                    result.Add(contact);
                }
            }
            return result;
        }

        public RadarContact GetNearestContact(RadarContactType type)
        {
            RadarContact nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var contact in contacts)
            {
                if (contact.contactType == type && contact.distance < nearestDist)
                {
                    nearest = contact;
                    nearestDist = contact.distance;
                }
            }

            return nearest;
        }

        public int GetContactCount(RadarContactType type)
        {
            int count = 0;
            foreach (var contact in contacts)
            {
                if (contact.contactType == type)
                    count++;
            }
            return count;
        }
    }

    [System.Serializable]
    public class RadarContact
    {
        public Transform target;
        public RadarContactType contactType;
        public float distance;
    }

    [System.Serializable]
    public class RadarBlip
    {
        public GameObject blipObject;
        public RectTransform rectTransform;
        public RadarContactType contactType;
    }

    public enum RadarContactType
    {
        Enemy,
        Friendly,
        Neutral,
        Station,
        Asteroid,
        MissionObjective,
        Loot,
        Waypoint
    }
}
