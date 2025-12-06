using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Interactive world map UI showing territories, factions, events, and player locations
    /// </summary>
    
    [Serializable]
    public class MapMarker
    {
        public string markerId;
        public string markerName;
        public MapMarkerType type;
        public Vector2 mapPosition;
        public Color markerColor;
        public bool isVisible;
        public bool isInteractable;
        public string tooltipText;
        public GameObject uiElement;
        
        public MapMarker(string id, string name, MapMarkerType t, Vector2 pos)
        {
            markerId = id;
            markerName = name;
            type = t;
            mapPosition = pos;
            markerColor = Color.white;
            isVisible = true;
            isInteractable = true;
            tooltipText = "";
        }
    }
    
    [Serializable]
    public enum MapMarkerType
    {
        Player,
        FriendlyPlayer,
        EnemyPlayer,
        Airbase,
        City,
        Port,
        Factory,
        Radar,
        AAGun,
        Objective,
        Event,
        TradeRoute,
        FrontLine,
        ResourceNode
    }
    
    [Serializable]
    public class TerritoryDisplay
    {
        public string territoryId;
        public string territoryName;
        public string controllingFactionId;
        public Color factionColor;
        public Vector2[] borderPoints;
        public Vector2 centerPosition;
        public float influenceLevel;
        public bool isContested;
        public bool isUnderAttack;
    }
    
    public class WorldMapUI : MonoBehaviour
    {
        public static WorldMapUI Instance { get; private set; }
        
        [Header("Map Settings")]
        [SerializeField] private RectTransform mapContainer;
        [SerializeField] private RectTransform markersContainer;
        [SerializeField] private Image mapBackground;
        [SerializeField] private float minZoom = 0.5f;
        [SerializeField] private float maxZoom = 5f;
        [SerializeField] private float zoomSpeed = 0.5f;
        [SerializeField] private float panSpeed = 500f;
        
        [Header("World Bounds")]
        [SerializeField] private float worldMinX = -15000f;
        [SerializeField] private float worldMaxX = 15000f;
        [SerializeField] private float worldMinZ = -15000f;
        [SerializeField] private float worldMaxZ = 15000f;
        
        [Header("Marker Prefabs")]
        [SerializeField] private GameObject playerMarkerPrefab;
        [SerializeField] private GameObject airbaseMarkerPrefab;
        [SerializeField] private GameObject cityMarkerPrefab;
        [SerializeField] private GameObject eventMarkerPrefab;
        [SerializeField] private GameObject objectiveMarkerPrefab;
        
        [Header("UI Elements")]
        [SerializeField] private Text coordinatesText;
        [SerializeField] private Text selectedInfoText;
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private Text tooltipText;
        [SerializeField] private Dropdown filterDropdown;
        [SerializeField] private Toggle showGridToggle;
        [SerializeField] private Toggle showTradeRoutesToggle;
        [SerializeField] private Toggle showEventsToggle;
        [SerializeField] private Button centerOnPlayerButton;
        [SerializeField] private Button setWaypointButton;
        [SerializeField] private Slider zoomSlider;
        
        [Header("Territory Display")]
        [SerializeField] private Material territoryMaterial;
        [SerializeField] private Color neutralColor = Color.gray;
        [SerializeField] private Color contestedColor = Color.yellow;
        
        [Header("Faction Colors")]
        [SerializeField] private Color usaColor = new Color(0.2f, 0.4f, 0.8f);
        [SerializeField] private Color germanyColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] private Color ukColor = new Color(0.9f, 0.5f, 0.2f);
        [SerializeField] private Color ussrColor = new Color(0.8f, 0.2f, 0.2f);
        [SerializeField] private Color japanColor = new Color(0.9f, 0.9f, 0.9f);
        
        private float currentZoom = 1f;
        private Vector2 currentPan = Vector2.zero;
        private Dictionary<string, MapMarker> markers = new Dictionary<string, MapMarker>();
        private List<TerritoryDisplay> territories = new List<TerritoryDisplay>();
        private MapMarker selectedMarker;
        private Vector2 lastMousePosition;
        private bool isDragging = false;
        private Vector2 playerWaypoint;
        private bool hasWaypoint = false;
        
        public event Action<Vector2> OnWaypointSet;
        public event Action<MapMarker> OnMarkerSelected;
        public event Action<TerritoryDisplay> OnTerritorySelected;
        
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
            InitializeUI();
            InitializeTerritories();
            CreateStaticMarkers();
        }
        
        private void InitializeUI()
        {
            if (centerOnPlayerButton != null)
                centerOnPlayerButton.onClick.AddListener(CenterOnPlayer);
            
            if (setWaypointButton != null)
                setWaypointButton.onClick.AddListener(EnableWaypointMode);
            
            if (zoomSlider != null)
            {
                zoomSlider.minValue = minZoom;
                zoomSlider.maxValue = maxZoom;
                zoomSlider.value = currentZoom;
                zoomSlider.onValueChanged.AddListener(SetZoom);
            }
            
            if (showGridToggle != null)
                showGridToggle.onValueChanged.AddListener(ToggleGrid);
            
            if (showTradeRoutesToggle != null)
                showTradeRoutesToggle.onValueChanged.AddListener(ToggleTradeRoutes);
            
            if (showEventsToggle != null)
                showEventsToggle.onValueChanged.AddListener(ToggleEvents);
            
            if (filterDropdown != null)
            {
                filterDropdown.ClearOptions();
                filterDropdown.AddOptions(new List<string> 
                { 
                    "All", "Airbases", "Cities", "Events", "Players", "Objectives" 
                });
                filterDropdown.onValueChanged.AddListener(ApplyFilter);
            }
        }
        
        private void InitializeTerritories()
        {
            // Create initial territory data
            // This would normally come from WorldFactionSystem
            territories = new List<TerritoryDisplay>
            {
                CreateTerritory("europe_west", "Western Europe", "usa", new Vector2(0, 0), 3000f),
                CreateTerritory("europe_central", "Central Europe", "germany", new Vector2(2000, 500), 2500f),
                CreateTerritory("europe_east", "Eastern Europe", "ussr", new Vector2(4000, 1000), 4000f),
                CreateTerritory("britain", "British Isles", "uk", new Vector2(-2000, 1500), 1500f),
                CreateTerritory("pacific_west", "Western Pacific", "japan", new Vector2(8000, -2000), 3500f),
                CreateTerritory("north_africa", "North Africa", "neutral", new Vector2(1000, -3000), 3000f),
                CreateTerritory("middle_east", "Middle East", "neutral", new Vector2(3500, -2000), 2000f),
                CreateTerritory("russia", "Russia", "ussr", new Vector2(5000, 3000), 5000f),
                CreateTerritory("scandinavia", "Scandinavia", "neutral", new Vector2(1500, 4000), 2000f),
                CreateTerritory("mediterranean", "Mediterranean", "contested", new Vector2(1500, -1500), 2500f)
            };
        }
        
        private TerritoryDisplay CreateTerritory(string id, string name, string faction, Vector2 center, float size)
        {
            var territory = new TerritoryDisplay
            {
                territoryId = id,
                territoryName = name,
                controllingFactionId = faction,
                centerPosition = center,
                influenceLevel = 1f,
                isContested = faction == "contested",
                isUnderAttack = false
            };
            
            // Create simple hexagonal border
            int sides = 6;
            territory.borderPoints = new Vector2[sides];
            for (int i = 0; i < sides; i++)
            {
                float angle = i * (360f / sides) * Mathf.Deg2Rad;
                territory.borderPoints[i] = center + new Vector2(
                    Mathf.Cos(angle) * size,
                    Mathf.Sin(angle) * size
                );
            }
            
            territory.factionColor = GetFactionColor(faction);
            
            return territory;
        }
        
        private void CreateStaticMarkers()
        {
            // Major airbases
            CreateMarker("airbase_london", "RAF Northolt", MapMarkerType.Airbase, new Vector2(-2000, 1500));
            CreateMarker("airbase_paris", "Orly Airfield", MapMarkerType.Airbase, new Vector2(-500, 500));
            CreateMarker("airbase_berlin", "Tempelhof", MapMarkerType.Airbase, new Vector2(2200, 800));
            CreateMarker("airbase_moscow", "Zhukovsky", MapMarkerType.Airbase, new Vector2(5500, 2500));
            CreateMarker("airbase_tokyo", "Yokota", MapMarkerType.Airbase, new Vector2(9000, -1500));
            CreateMarker("airbase_washington", "Andrews AFB", MapMarkerType.Airbase, new Vector2(-8000, 1000));
            
            // Major cities
            CreateMarker("city_london", "London", MapMarkerType.City, new Vector2(-2100, 1400));
            CreateMarker("city_paris", "Paris", MapMarkerType.City, new Vector2(-600, 400));
            CreateMarker("city_berlin", "Berlin", MapMarkerType.City, new Vector2(2100, 700));
            CreateMarker("city_moscow", "Moscow", MapMarkerType.City, new Vector2(5400, 2400));
            CreateMarker("city_tokyo", "Tokyo", MapMarkerType.City, new Vector2(8900, -1600));
            
            // Ports
            CreateMarker("port_rotterdam", "Rotterdam", MapMarkerType.Port, new Vector2(-100, 1200));
            CreateMarker("port_hamburg", "Hamburg", MapMarkerType.Port, new Vector2(1500, 1500));
            CreateMarker("port_marseille", "Marseille", MapMarkerType.Port, new Vector2(0, -800));
            
            // Factories
            CreateMarker("factory_ruhr", "Ruhr Industrial", MapMarkerType.Factory, new Vector2(1000, 1000));
            CreateMarker("factory_ural", "Ural Factories", MapMarkerType.Factory, new Vector2(7000, 2000));
        }
        
        private void Update()
        {
            HandleInput();
            UpdateMarkerPositions();
            UpdateCoordinatesDisplay();
            UpdateTooltip();
        }
        
        private void HandleInput()
        {
            // Zoom with scroll wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                currentZoom = Mathf.Clamp(currentZoom + scroll * zoomSpeed, minZoom, maxZoom);
                if (zoomSlider != null)
                    zoomSlider.value = currentZoom;
                ApplyZoom();
            }
            
            // Pan with middle mouse or right click drag
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            
            if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                isDragging = false;
            }
            
            if (isDragging)
            {
                Vector2 delta = (Vector2)Input.mousePosition - lastMousePosition;
                currentPan += delta / currentZoom;
                lastMousePosition = Input.mousePosition;
                ApplyPan();
            }
            
            // Click to select
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick(Input.mousePosition);
            }
            
            // Keyboard shortcuts
            if (Input.GetKeyDown(KeyCode.M))
            {
                CenterOnPlayer();
            }
            
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                SetZoom(currentZoom + 0.2f);
            }
            
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                SetZoom(currentZoom - 0.2f);
            }
        }
        
        private void HandleClick(Vector2 screenPosition)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapContainer, screenPosition, null, out localPoint);
            
            // Check for marker clicks
            foreach (var kvp in markers)
            {
                var marker = kvp.Value;
                if (!marker.isVisible || !marker.isInteractable) continue;
                
                Vector2 markerScreenPos = WorldToMapPosition(new Vector3(marker.mapPosition.x, 0, marker.mapPosition.y));
                float distance = Vector2.Distance(localPoint, markerScreenPos);
                
                if (distance < 20f) // Click radius
                {
                    SelectMarker(marker);
                    return;
                }
            }
            
            // Check for territory clicks
            Vector2 worldPos = MapToWorldPosition(localPoint);
            foreach (var territory in territories)
            {
                if (IsPointInTerritory(worldPos, territory))
                {
                    SelectTerritory(territory);
                    return;
                }
            }
            
            // Click on empty space - deselect
            DeselectAll();
        }
        
        private bool IsPointInTerritory(Vector2 point, TerritoryDisplay territory)
        {
            float distance = Vector2.Distance(point, territory.centerPosition);
            // Simple distance check - in a real implementation, use proper polygon containment
            return distance < 2000f;
        }
        
        private void SelectMarker(MapMarker marker)
        {
            selectedMarker = marker;
            OnMarkerSelected?.Invoke(marker);
            
            if (selectedInfoText != null)
            {
                selectedInfoText.text = $"{marker.markerName}\n{marker.type}\n{marker.tooltipText}";
            }
        }
        
        private void SelectTerritory(TerritoryDisplay territory)
        {
            OnTerritorySelected?.Invoke(territory);
            
            if (selectedInfoText != null)
            {
                string status = territory.isContested ? " (CONTESTED)" : "";
                selectedInfoText.text = $"{territory.territoryName}{status}\n" +
                                       $"Controlled by: {territory.controllingFactionId}\n" +
                                       $"Influence: {territory.influenceLevel:P0}";
            }
        }
        
        private void DeselectAll()
        {
            selectedMarker = null;
            if (selectedInfoText != null)
            {
                selectedInfoText.text = "";
            }
        }
        
        private void ApplyZoom()
        {
            if (mapContainer != null)
            {
                mapContainer.localScale = Vector3.one * currentZoom;
            }
        }
        
        private void ApplyPan()
        {
            if (mapContainer != null)
            {
                mapContainer.anchoredPosition = currentPan;
            }
        }
        
        private void SetZoom(float zoom)
        {
            currentZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            if (zoomSlider != null)
                zoomSlider.value = currentZoom;
            ApplyZoom();
        }
        
        private void UpdateMarkerPositions()
        {
            foreach (var kvp in markers)
            {
                var marker = kvp.Value;
                if (marker.uiElement != null)
                {
                    Vector2 mapPos = WorldToMapPosition(new Vector3(marker.mapPosition.x, 0, marker.mapPosition.y));
                    marker.uiElement.GetComponent<RectTransform>().anchoredPosition = mapPos;
                    marker.uiElement.SetActive(marker.isVisible);
                }
            }
        }
        
        private void UpdateCoordinatesDisplay()
        {
            if (coordinatesText == null) return;
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapContainer, Input.mousePosition, null, out localPoint);
            
            Vector2 worldPos = MapToWorldPosition(localPoint);
            coordinatesText.text = $"X: {worldPos.x:F0} Z: {worldPos.y:F0}";
        }
        
        private void UpdateTooltip()
        {
            if (tooltipPanel == null) return;
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapContainer, Input.mousePosition, null, out localPoint);
            
            MapMarker hoveredMarker = null;
            foreach (var kvp in markers)
            {
                var marker = kvp.Value;
                if (!marker.isVisible) continue;
                
                Vector2 markerPos = WorldToMapPosition(new Vector3(marker.mapPosition.x, 0, marker.mapPosition.y));
                if (Vector2.Distance(localPoint, markerPos) < 15f)
                {
                    hoveredMarker = marker;
                    break;
                }
            }
            
            if (hoveredMarker != null)
            {
                tooltipPanel.SetActive(true);
                tooltipPanel.transform.position = Input.mousePosition + new Vector3(15, -15, 0);
                
                if (tooltipText != null)
                {
                    tooltipText.text = hoveredMarker.markerName + 
                        (string.IsNullOrEmpty(hoveredMarker.tooltipText) ? "" : "\n" + hoveredMarker.tooltipText);
                }
            }
            else
            {
                tooltipPanel.SetActive(false);
            }
        }
        
        // Coordinate conversion
        
        private Vector2 WorldToMapPosition(Vector3 worldPos)
        {
            float mapWidth = 1000f; // Map image width in pixels
            float mapHeight = 1000f;
            
            float normalizedX = (worldPos.x - worldMinX) / (worldMaxX - worldMinX);
            float normalizedZ = (worldPos.z - worldMinZ) / (worldMaxZ - worldMinZ);
            
            return new Vector2(
                normalizedX * mapWidth - mapWidth / 2,
                normalizedZ * mapHeight - mapHeight / 2
            );
        }
        
        private Vector2 MapToWorldPosition(Vector2 mapPos)
        {
            float mapWidth = 1000f;
            float mapHeight = 1000f;
            
            float normalizedX = (mapPos.x + mapWidth / 2) / mapWidth;
            float normalizedZ = (mapPos.y + mapHeight / 2) / mapHeight;
            
            return new Vector2(
                normalizedX * (worldMaxX - worldMinX) + worldMinX,
                normalizedZ * (worldMaxZ - worldMinZ) + worldMinZ
            );
        }
        
        // Marker management
        
        public void CreateMarker(string id, string name, MapMarkerType type, Vector2 worldPosition)
        {
            var marker = new MapMarker(id, name, type, worldPosition);
            marker.markerColor = GetMarkerColor(type);
            
            // Create UI element
            GameObject prefab = GetMarkerPrefab(type);
            if (prefab != null && markersContainer != null)
            {
                marker.uiElement = Instantiate(prefab, markersContainer);
                marker.uiElement.name = $"Marker_{id}";
                
                var image = marker.uiElement.GetComponent<Image>();
                if (image != null)
                {
                    image.color = marker.markerColor;
                }
            }
            
            markers[id] = marker;
        }
        
        public void UpdateMarker(string id, Vector2 newPosition)
        {
            if (markers.ContainsKey(id))
            {
                markers[id].mapPosition = newPosition;
            }
        }
        
        public void RemoveMarker(string id)
        {
            if (markers.ContainsKey(id))
            {
                if (markers[id].uiElement != null)
                {
                    Destroy(markers[id].uiElement);
                }
                markers.Remove(id);
            }
        }
        
        public void SetMarkerVisibility(string id, bool visible)
        {
            if (markers.ContainsKey(id))
            {
                markers[id].isVisible = visible;
            }
        }
        
        private GameObject GetMarkerPrefab(MapMarkerType type)
        {
            switch (type)
            {
                case MapMarkerType.Player:
                case MapMarkerType.FriendlyPlayer:
                case MapMarkerType.EnemyPlayer:
                    return playerMarkerPrefab;
                case MapMarkerType.Airbase:
                    return airbaseMarkerPrefab;
                case MapMarkerType.City:
                case MapMarkerType.Port:
                case MapMarkerType.Factory:
                    return cityMarkerPrefab;
                case MapMarkerType.Event:
                    return eventMarkerPrefab;
                case MapMarkerType.Objective:
                    return objectiveMarkerPrefab;
                default:
                    return cityMarkerPrefab;
            }
        }
        
        private Color GetMarkerColor(MapMarkerType type)
        {
            switch (type)
            {
                case MapMarkerType.Player: return Color.cyan;
                case MapMarkerType.FriendlyPlayer: return Color.green;
                case MapMarkerType.EnemyPlayer: return Color.red;
                case MapMarkerType.Airbase: return new Color(0.5f, 0.8f, 1f);
                case MapMarkerType.City: return Color.white;
                case MapMarkerType.Port: return new Color(0.3f, 0.6f, 1f);
                case MapMarkerType.Factory: return new Color(1f, 0.8f, 0.3f);
                case MapMarkerType.Radar: return new Color(0.8f, 0.8f, 0.2f);
                case MapMarkerType.AAGun: return new Color(1f, 0.5f, 0.5f);
                case MapMarkerType.Objective: return new Color(1f, 0.8f, 0f);
                case MapMarkerType.Event: return new Color(1f, 0.5f, 1f);
                default: return Color.white;
            }
        }
        
        private Color GetFactionColor(string factionId)
        {
            switch (factionId.ToLower())
            {
                case "usa": return usaColor;
                case "germany": return germanyColor;
                case "uk": return ukColor;
                case "ussr": return ussrColor;
                case "japan": return japanColor;
                case "contested": return contestedColor;
                default: return neutralColor;
            }
        }
        
        // Player functions
        
        public void UpdatePlayerPosition(Vector3 worldPosition)
        {
            if (!markers.ContainsKey("player_local"))
            {
                CreateMarker("player_local", "You", MapMarkerType.Player, new Vector2(worldPosition.x, worldPosition.z));
            }
            else
            {
                UpdateMarker("player_local", new Vector2(worldPosition.x, worldPosition.z));
            }
        }
        
        public void CenterOnPlayer()
        {
            if (markers.ContainsKey("player_local"))
            {
                Vector2 playerMapPos = WorldToMapPosition(new Vector3(
                    markers["player_local"].mapPosition.x, 0, 
                    markers["player_local"].mapPosition.y));
                
                currentPan = -playerMapPos * currentZoom;
                ApplyPan();
            }
        }
        
        public void CenterOnPosition(Vector2 worldPosition)
        {
            Vector2 mapPos = WorldToMapPosition(new Vector3(worldPosition.x, 0, worldPosition.y));
            currentPan = -mapPos * currentZoom;
            ApplyPan();
        }
        
        // Waypoint
        
        private bool isSettingWaypoint = false;
        
        private void EnableWaypointMode()
        {
            isSettingWaypoint = true;
        }
        
        public void SetWaypoint(Vector2 worldPosition)
        {
            playerWaypoint = worldPosition;
            hasWaypoint = true;
            
            if (!markers.ContainsKey("waypoint"))
            {
                CreateMarker("waypoint", "Waypoint", MapMarkerType.Objective, worldPosition);
            }
            else
            {
                UpdateMarker("waypoint", worldPosition);
            }
            
            OnWaypointSet?.Invoke(worldPosition);
        }
        
        public void ClearWaypoint()
        {
            hasWaypoint = false;
            RemoveMarker("waypoint");
        }
        
        public Vector2 GetWaypoint()
        {
            return playerWaypoint;
        }
        
        public bool HasWaypoint()
        {
            return hasWaypoint;
        }
        
        // Filter and toggle functions
        
        private void ApplyFilter(int filterIndex)
        {
            string[] filters = { "All", "Airbases", "Cities", "Events", "Players", "Objectives" };
            string filter = filters[filterIndex];
            
            foreach (var kvp in markers)
            {
                var marker = kvp.Value;
                bool visible = filter == "All";
                
                switch (filter)
                {
                    case "Airbases":
                        visible = marker.type == MapMarkerType.Airbase;
                        break;
                    case "Cities":
                        visible = marker.type == MapMarkerType.City || 
                                 marker.type == MapMarkerType.Port || 
                                 marker.type == MapMarkerType.Factory;
                        break;
                    case "Events":
                        visible = marker.type == MapMarkerType.Event;
                        break;
                    case "Players":
                        visible = marker.type == MapMarkerType.Player || 
                                 marker.type == MapMarkerType.FriendlyPlayer || 
                                 marker.type == MapMarkerType.EnemyPlayer;
                        break;
                    case "Objectives":
                        visible = marker.type == MapMarkerType.Objective;
                        break;
                }
                
                marker.isVisible = visible;
            }
        }
        
        private void ToggleGrid(bool show)
        {
            // Implement grid overlay
        }
        
        private void ToggleTradeRoutes(bool show)
        {
            foreach (var kvp in markers)
            {
                if (kvp.Value.type == MapMarkerType.TradeRoute)
                {
                    kvp.Value.isVisible = show;
                }
            }
        }
        
        private void ToggleEvents(bool show)
        {
            foreach (var kvp in markers)
            {
                if (kvp.Value.type == MapMarkerType.Event)
                {
                    kvp.Value.isVisible = show;
                }
            }
        }
        
        // Territory updates
        
        public void UpdateTerritory(string territoryId, string newFactionId, float influence, bool contested)
        {
            var territory = territories.Find(t => t.territoryId == territoryId);
            if (territory != null)
            {
                territory.controllingFactionId = newFactionId;
                territory.factionColor = GetFactionColor(newFactionId);
                territory.influenceLevel = influence;
                territory.isContested = contested;
            }
        }
        
        public void SetTerritoryUnderAttack(string territoryId, bool underAttack)
        {
            var territory = territories.Find(t => t.territoryId == territoryId);
            if (territory != null)
            {
                territory.isUnderAttack = underAttack;
            }
        }
        
        // Event markers
        
        public void AddEventMarker(string eventId, string eventName, Vector2 position, string tooltip)
        {
            string markerId = $"event_{eventId}";
            CreateMarker(markerId, eventName, MapMarkerType.Event, position);
            markers[markerId].tooltipText = tooltip;
        }
        
        public void RemoveEventMarker(string eventId)
        {
            RemoveMarker($"event_{eventId}");
        }
        
        // Other player markers
        
        public void AddPlayerMarker(string playerId, string playerName, Vector2 position, bool isFriendly)
        {
            string markerId = $"player_{playerId}";
            var type = isFriendly ? MapMarkerType.FriendlyPlayer : MapMarkerType.EnemyPlayer;
            CreateMarker(markerId, playerName, type, position);
        }
        
        public void UpdatePlayerMarker(string playerId, Vector2 position)
        {
            UpdateMarker($"player_{playerId}", position);
        }
        
        public void RemovePlayerMarker(string playerId)
        {
            RemoveMarker($"player_{playerId}");
        }
        
        // Show/Hide map
        
        public void ShowMap()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f; // Pause game while map is open
        }
        
        public void HideMap()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
        
        public void ToggleMap()
        {
            if (gameObject.activeSelf)
                HideMap();
            else
                ShowMap();
        }
        
        // Draw territories (would use LineRenderer or UI elements in real implementation)
        private void OnGUI()
        {
            if (!gameObject.activeSelf) return;
            
            // This is a placeholder - real implementation would use proper UI elements
            // Draw territory borders and fills using UI Image components with proper shaders
        }
    }
}
