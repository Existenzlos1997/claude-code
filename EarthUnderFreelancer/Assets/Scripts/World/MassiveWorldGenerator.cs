using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.World
{
    /// <summary>
    /// Massive procedural world generator creating a world larger than any competitor
    /// Generates 1000+ unique locations, 500+ cities, 200+ airports, infinite terrain
    /// </summary>
    public class MassiveWorldGenerator : MonoBehaviour
    {
        public static MassiveWorldGenerator Instance { get; private set; }

        [Header("World Scale Settings")]
        [SerializeField] private float worldScale = 1000000f; // 1 million km²
        [SerializeField] private int continentCount = 7;
        [SerializeField] private int countriesPerContinent = 25;
        [SerializeField] private int citiesPerCountry = 50;
        [SerializeField] private int airportsPerCountry = 10;
        [SerializeField] private int militaryBasesPerCountry = 5;
        [SerializeField] private int navalBasesPerCountry = 3;
        
        [Header("Terrain Settings")]
        [SerializeField] private int terrainChunkSize = 1000;
        [SerializeField] private int maxLoadedChunks = 100;
        [SerializeField] private float viewDistance = 50000f;
        
        // World Data
        private List<Continent> continents = new List<Continent>();
        private List<Country> countries = new List<Country>();
        private List<City> cities = new List<City>();
        private List<Airport> airports = new List<Airport>();
        private List<MilitaryBase> militaryBases = new List<MilitaryBase>();
        private List<NavalBase> navalBases = new List<NavalBase>();
        private List<Landmark> landmarks = new List<Landmark>();
        private List<TradeRoute> tradeRoutes = new List<TradeRoute>();
        private List<ConflictZone> conflictZones = new List<ConflictZone>();
        
        // Terrain chunks
        private Dictionary<Vector2Int, TerrainChunk> loadedChunks = new Dictionary<Vector2Int, TerrainChunk>();
        private Queue<Vector2Int> chunkLoadQueue = new Queue<Vector2Int>();
        
        // Statistics
        public WorldStatistics Statistics { get; private set; }
        
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
        
        private void Start()
        {
            GenerateWorld();
        }
        
        public void GenerateWorld()
        {
            Debug.Log("[MassiveWorldGenerator] Starting world generation...");
            
            // Generate continents
            GenerateContinents();
            
            // Generate countries
            GenerateCountries();
            
            // Generate cities
            GenerateCities();
            
            // Generate airports
            GenerateAirports();
            
            // Generate military installations
            GenerateMilitaryInstallations();
            
            // Generate landmarks
            GenerateLandmarks();
            
            // Generate trade routes
            GenerateTradeRoutes();
            
            // Generate conflict zones
            GenerateConflictZones();
            
            // Calculate statistics
            CalculateStatistics();
            
            Debug.Log($"[MassiveWorldGenerator] World generation complete! {Statistics}");
        }
        
        private void GenerateContinents()
        {
            string[] continentNames = {
                "Aurelia", "Novaterra", "Zephyria", "Kronheim", 
                "Velantis", "Drakkoria", "Crystallia"
            };
            
            for (int i = 0; i < continentCount; i++)
            {
                var continent = new Continent
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = continentNames[i % continentNames.Length],
                    Position = GetRandomContinentPosition(i),
                    Size = UnityEngine.Random.Range(100000f, 500000f),
                    Climate = (ClimateType)UnityEngine.Random.Range(0, 6),
                    BiomeDistribution = GenerateBiomeDistribution(),
                    PopulationDensity = UnityEngine.Random.Range(10f, 500f),
                    TechLevel = UnityEngine.Random.Range(1, 10),
                    PrimaryFaction = GetRandomFaction()
                };
                
                continents.Add(continent);
            }
        }
        
        private void GenerateCountries()
        {
            string[] countryPrefixes = { "New", "Greater", "United", "Federal", "Democratic", "Republic of", "Kingdom of", "Empire of" };
            string[] countryNames = {
                "Valdoria", "Krynthia", "Solheim", "Aethermoor", "Ironvale", "Stormhold", "Frostmark",
                "Sundale", "Shadowmere", "Goldcrest", "Silverwyn", "Bronzegate", "Steelhaven", "Crystalford",
                "Thunderpeak", "Windholm", "Starfall", "Moonridge", "Dawnbreak", "Duskwood", "Nightshade",
                "Brightwater", "Darkstone", "Redcliff", "Blueharbor", "Greenfield", "Whitespire", "Blackrock"
            };
            
            foreach (var continent in continents)
            {
                for (int i = 0; i < countriesPerContinent; i++)
                {
                    string prefix = UnityEngine.Random.value > 0.5f ? countryPrefixes[UnityEngine.Random.Range(0, countryPrefixes.Length)] + " " : "";
                    
                    var country = new Country
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = prefix + countryNames[UnityEngine.Random.Range(0, countryNames.Length)] + (i > 0 ? $" {ToRomanNumeral(i)}" : ""),
                        ContinentId = continent.Id,
                        Position = continent.Position + UnityEngine.Random.insideUnitSphere * continent.Size * 0.4f,
                        Size = UnityEngine.Random.Range(10000f, 100000f),
                        Population = UnityEngine.Random.Range(1000000, 500000000),
                        GDP = UnityEngine.Random.Range(1000000000f, 50000000000000f),
                        MilitaryStrength = UnityEngine.Random.Range(1, 100),
                        GovernmentType = (GovernmentType)UnityEngine.Random.Range(0, 8),
                        Faction = GetRandomFaction(),
                        Relations = new Dictionary<string, float>(),
                        Resources = GenerateResources(),
                        Industries = GenerateIndustries()
                    };
                    
                    countries.Add(country);
                }
            }
            
            // Generate relations between countries
            foreach (var country in countries)
            {
                foreach (var other in countries)
                {
                    if (country.Id != other.Id)
                    {
                        country.Relations[other.Id] = UnityEngine.Random.Range(-100f, 100f);
                    }
                }
            }
        }
        
        private void GenerateCities()
        {
            string[] cityPrefixes = { "New ", "Old ", "Port ", "Fort ", "Saint ", "Mount ", "Lake ", "East ", "West ", "North ", "South ", "Upper ", "Lower ", "Greater ", "Lesser " };
            string[] cityNames = {
                "Haven", "Vista", "Springs", "Falls", "Ridge", "Valley", "Harbor", "Port", "Bay", "Cove",
                "Hill", "Dale", "Glen", "Grove", "Wood", "Forest", "Field", "Plain", "Mesa", "Canyon",
                "Beach", "Shore", "Coast", "Island", "Point", "Cape", "Bluff", "Cliff", "Rock", "Stone",
                "Bridge", "Ford", "Cross", "Junction", "Station", "Terminal", "Central", "Plaza", "Square", "Park",
                "Tower", "Castle", "Palace", "Manor", "Estate", "Villa", "Court", "Gate", "Wall", "Keep"
            };
            
            foreach (var country in countries)
            {
                // Capital city
                var capital = new City
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = country.Name + " City",
                    CountryId = country.Id,
                    Position = country.Position,
                    Population = (int)(country.Population * 0.15f),
                    IsCapital = true,
                    CityType = CityType.Metropolis,
                    Infrastructure = UnityEngine.Random.Range(70, 100),
                    Tourism = UnityEngine.Random.Range(50, 100),
                    Industry = UnityEngine.Random.Range(60, 100),
                    Commerce = UnityEngine.Random.Range(70, 100),
                    Districts = GenerateDistricts(true),
                    Landmarks = new List<string>(),
                    Services = GenerateCityServices(true)
                };
                cities.Add(capital);
                
                // Other cities
                for (int i = 0; i < citiesPerCountry - 1; i++)
                {
                    string prefix = UnityEngine.Random.value > 0.7f ? cityPrefixes[UnityEngine.Random.Range(0, cityPrefixes.Length)] : "";
                    string name = cityNames[UnityEngine.Random.Range(0, cityNames.Length)];
                    
                    var city = new City
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = prefix + name,
                        CountryId = country.Id,
                        Position = country.Position + UnityEngine.Random.insideUnitSphere * country.Size * 0.4f,
                        Population = UnityEngine.Random.Range(10000, 5000000),
                        IsCapital = false,
                        CityType = GetCityType(UnityEngine.Random.Range(10000, 5000000)),
                        Infrastructure = UnityEngine.Random.Range(30, 90),
                        Tourism = UnityEngine.Random.Range(10, 80),
                        Industry = UnityEngine.Random.Range(20, 90),
                        Commerce = UnityEngine.Random.Range(30, 80),
                        Districts = GenerateDistricts(false),
                        Landmarks = new List<string>(),
                        Services = GenerateCityServices(false)
                    };
                    cities.Add(city);
                }
            }
        }
        
        private void GenerateAirports()
        {
            string[] airportTypes = { "International", "Regional", "Municipal", "Executive", "Cargo", "Military" };
            
            foreach (var country in countries)
            {
                var countryCities = cities.Where(c => c.CountryId == country.Id).ToList();
                
                for (int i = 0; i < airportsPerCountry && i < countryCities.Count; i++)
                {
                    var city = countryCities[i];
                    
                    var airport = new Airport
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"{city.Name} {airportTypes[UnityEngine.Random.Range(0, airportTypes.Length)]} Airport",
                        ICAOCode = GenerateICAOCode(),
                        IATACode = GenerateIATACode(),
                        CityId = city.Id,
                        CountryId = country.Id,
                        Position = city.Position + UnityEngine.Random.insideUnitSphere * 20f,
                        Elevation = UnityEngine.Random.Range(0f, 3000f),
                        Runways = GenerateRunways(),
                        Terminals = UnityEngine.Random.Range(1, 10),
                        Gates = UnityEngine.Random.Range(5, 200),
                        AnnualPassengers = UnityEngine.Random.Range(100000, 100000000),
                        AnnualCargo = UnityEngine.Random.Range(10000f, 5000000f),
                        Airlines = GenerateAirlines(),
                        Services = GenerateAirportServices(),
                        FuelTypes = GenerateFuelTypes(),
                        ATCFrequencies = GenerateATCFrequencies()
                    };
                    airports.Add(airport);
                }
            }
        }
        
        private void GenerateMilitaryInstallations()
        {
            string[] basePrefixes = { "Fort", "Camp", "Base", "Station", "Command", "Headquarters" };
            string[] baseNames = { "Thunder", "Eagle", "Falcon", "Hawk", "Phoenix", "Dragon", "Storm", "Iron", "Steel", "Titan" };
            
            foreach (var country in countries)
            {
                // Air Force bases
                for (int i = 0; i < militaryBasesPerCountry; i++)
                {
                    var militaryBase = new MilitaryBase
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"{basePrefixes[UnityEngine.Random.Range(0, basePrefixes.Length)]} {baseNames[UnityEngine.Random.Range(0, baseNames.Length)]}",
                        CountryId = country.Id,
                        Position = country.Position + UnityEngine.Random.insideUnitSphere * country.Size * 0.3f,
                        BaseType = (MilitaryBaseType)UnityEngine.Random.Range(0, 5),
                        SecurityLevel = UnityEngine.Random.Range(1, 10),
                        Personnel = UnityEngine.Random.Range(500, 50000),
                        AircraftCapacity = UnityEngine.Random.Range(20, 500),
                        DefenseRating = UnityEngine.Random.Range(50, 100),
                        RadarRange = UnityEngine.Random.Range(100f, 1000f),
                        SAMSites = UnityEngine.Random.Range(0, 20),
                        Hangars = UnityEngine.Random.Range(5, 50),
                        Runways = GenerateRunways(),
                        StationedUnits = GenerateMilitaryUnits(),
                        Equipment = GenerateMilitaryEquipment()
                    };
                    militaryBases.Add(militaryBase);
                }
                
                // Naval bases
                for (int i = 0; i < navalBasesPerCountry; i++)
                {
                    var navalBase = new NavalBase
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"Naval Station {baseNames[UnityEngine.Random.Range(0, baseNames.Length)]}",
                        CountryId = country.Id,
                        Position = country.Position + UnityEngine.Random.insideUnitSphere * country.Size * 0.4f,
                        BaseType = (NavalBaseType)UnityEngine.Random.Range(0, 4),
                        Piers = UnityEngine.Random.Range(5, 50),
                        DrydockCapacity = UnityEngine.Random.Range(1, 10),
                        SubmarinePens = UnityEngine.Random.Range(0, 20),
                        AircraftCarrierBerths = UnityEngine.Random.Range(0, 5),
                        Personnel = UnityEngine.Random.Range(1000, 30000),
                        Ships = GenerateNavalShips(),
                        CoastalDefenses = GenerateCoastalDefenses()
                    };
                    navalBases.Add(navalBase);
                }
            }
        }
        
        private void GenerateLandmarks()
        {
            string[] landmarkTypes = { "Monument", "Tower", "Bridge", "Dam", "Stadium", "Museum", "Temple", "Castle", "Palace", "Statue" };
            
            foreach (var city in cities)
            {
                int landmarkCount = city.IsCapital ? UnityEngine.Random.Range(5, 15) : UnityEngine.Random.Range(1, 5);
                
                for (int i = 0; i < landmarkCount; i++)
                {
                    var landmark = new Landmark
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"{city.Name} {landmarkTypes[UnityEngine.Random.Range(0, landmarkTypes.Length)]}",
                        CityId = city.Id,
                        Position = city.Position + UnityEngine.Random.insideUnitSphere * 5f,
                        Type = (LandmarkType)UnityEngine.Random.Range(0, 15),
                        Height = UnityEngine.Random.Range(10f, 500f),
                        YearBuilt = UnityEngine.Random.Range(1800, 2024),
                        HistoricalSignificance = UnityEngine.Random.Range(1, 10),
                        TouristRating = UnityEngine.Random.Range(1, 5),
                        DailyVisitors = UnityEngine.Random.Range(100, 100000)
                    };
                    landmarks.Add(landmark);
                    city.Landmarks.Add(landmark.Id);
                }
            }
        }
        
        private void GenerateTradeRoutes()
        {
            // Generate trade routes between airports
            for (int i = 0; i < airports.Count; i++)
            {
                int connections = UnityEngine.Random.Range(3, 15);
                for (int j = 0; j < connections; j++)
                {
                    int targetIndex = UnityEngine.Random.Range(0, airports.Count);
                    if (targetIndex != i)
                    {
                        var route = new TradeRoute
                        {
                            Id = Guid.NewGuid().ToString(),
                            OriginId = airports[i].Id,
                            DestinationId = airports[targetIndex].Id,
                            Distance = Vector3.Distance(airports[i].Position, airports[targetIndex].Position),
                            RouteType = (TradeRouteType)UnityEngine.Random.Range(0, 5),
                            Volume = UnityEngine.Random.Range(1000f, 1000000f),
                            Value = UnityEngine.Random.Range(10000f, 100000000f),
                            Frequency = UnityEngine.Random.Range(1, 100),
                            Commodities = GenerateCommodities(),
                            RiskLevel = UnityEngine.Random.Range(1, 10),
                            IsLegal = UnityEngine.Random.value > 0.1f
                        };
                        tradeRoutes.Add(route);
                    }
                }
            }
        }
        
        private void GenerateConflictZones()
        {
            // Generate conflict zones between countries with bad relations
            foreach (var country in countries)
            {
                foreach (var relation in country.Relations)
                {
                    if (relation.Value < -50f && UnityEngine.Random.value > 0.7f)
                    {
                        var otherCountry = countries.FirstOrDefault(c => c.Id == relation.Key);
                        if (otherCountry != null)
                        {
                            var conflictZone = new ConflictZone
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = $"{country.Name}-{otherCountry.Name} Conflict Zone",
                                Position = (country.Position + otherCountry.Position) / 2f,
                                Radius = UnityEngine.Random.Range(1000f, 10000f),
                                ConflictType = (ConflictType)UnityEngine.Random.Range(0, 6),
                                Intensity = UnityEngine.Random.Range(1, 10),
                                FactionA = country.Faction,
                                FactionB = otherCountry.Faction,
                                Objectives = GenerateConflictObjectives(),
                                ActiveMissions = new List<string>(),
                                Rewards = GenerateConflictRewards()
                            };
                            conflictZones.Add(conflictZone);
                        }
                    }
                }
            }
        }
        
        private void CalculateStatistics()
        {
            Statistics = new WorldStatistics
            {
                TotalContinents = continents.Count,
                TotalCountries = countries.Count,
                TotalCities = cities.Count,
                TotalAirports = airports.Count,
                TotalMilitaryBases = militaryBases.Count,
                TotalNavalBases = navalBases.Count,
                TotalLandmarks = landmarks.Count,
                TotalTradeRoutes = tradeRoutes.Count,
                TotalConflictZones = conflictZones.Count,
                TotalPopulation = countries.Sum(c => c.Population),
                TotalGDP = countries.Sum(c => c.GDP),
                WorldSize = worldScale
            };
        }
        
        // Helper methods
        private Vector3 GetRandomContinentPosition(int index)
        {
            float angle = (index / (float)continentCount) * Mathf.PI * 2f;
            return new Vector3(
                Mathf.Cos(angle) * worldScale * 0.3f,
                0,
                Mathf.Sin(angle) * worldScale * 0.3f
            );
        }
        
        private Dictionary<BiomeType, float> GenerateBiomeDistribution()
        {
            var distribution = new Dictionary<BiomeType, float>();
            float total = 0;
            foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)))
            {
                float value = UnityEngine.Random.value;
                distribution[biome] = value;
                total += value;
            }
            // Normalize
            foreach (var key in distribution.Keys.ToList())
            {
                distribution[key] /= total;
            }
            return distribution;
        }
        
        private string GetRandomFaction()
        {
            string[] factions = { "Alliance", "Federation", "Empire", "Republic", "Coalition", "Union", "Confederation", "Dominion" };
            return factions[UnityEngine.Random.Range(0, factions.Length)];
        }
        
        private string ToRomanNumeral(int number)
        {
            if (number < 1) return "";
            if (number >= 10) return "X" + ToRomanNumeral(number - 10);
            if (number >= 9) return "IX" + ToRomanNumeral(number - 9);
            if (number >= 5) return "V" + ToRomanNumeral(number - 5);
            if (number >= 4) return "IV" + ToRomanNumeral(number - 4);
            if (number >= 1) return "I" + ToRomanNumeral(number - 1);
            return "";
        }
        
        private Dictionary<ResourceType, float> GenerateResources()
        {
            var resources = new Dictionary<ResourceType, float>();
            foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
            {
                resources[resource] = UnityEngine.Random.Range(0f, 100f);
            }
            return resources;
        }
        
        private List<IndustryType> GenerateIndustries()
        {
            var industries = new List<IndustryType>();
            foreach (IndustryType industry in Enum.GetValues(typeof(IndustryType)))
            {
                if (UnityEngine.Random.value > 0.5f)
                {
                    industries.Add(industry);
                }
            }
            return industries;
        }
        
        private CityType GetCityType(int population)
        {
            if (population > 5000000) return CityType.Metropolis;
            if (population > 1000000) return CityType.LargeCity;
            if (population > 500000) return CityType.MediumCity;
            if (population > 100000) return CityType.SmallCity;
            return CityType.Town;
        }
        
        private List<District> GenerateDistricts(bool isCapital)
        {
            var districts = new List<District>();
            int count = isCapital ? UnityEngine.Random.Range(15, 30) : UnityEngine.Random.Range(3, 15);
            
            string[] districtNames = { "Downtown", "Industrial", "Residential", "Commercial", "Historic", "Arts", "Financial", "Tech", "Harbor", "University" };
            
            for (int i = 0; i < count; i++)
            {
                districts.Add(new District
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = districtNames[i % districtNames.Length] + " District",
                    Type = (DistrictType)UnityEngine.Random.Range(0, 10),
                    Population = UnityEngine.Random.Range(10000, 500000),
                    Area = UnityEngine.Random.Range(1f, 50f),
                    Prosperity = UnityEngine.Random.Range(1, 10)
                });
            }
            
            return districts;
        }
        
        private List<CityService> GenerateCityServices(bool isCapital)
        {
            var services = new List<CityService>();
            foreach (CityServiceType serviceType in Enum.GetValues(typeof(CityServiceType)))
            {
                if (isCapital || UnityEngine.Random.value > 0.3f)
                {
                    services.Add(new CityService
                    {
                        Type = serviceType,
                        Quality = UnityEngine.Random.Range(1, 10),
                        Capacity = UnityEngine.Random.Range(1000, 100000)
                    });
                }
            }
            return services;
        }
        
        private string GenerateICAOCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Range(0, 4).Select(_ => chars[UnityEngine.Random.Range(0, chars.Length)]).ToArray());
        }
        
        private string GenerateIATACode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Range(0, 3).Select(_ => chars[UnityEngine.Random.Range(0, chars.Length)]).ToArray());
        }
        
        private List<Runway> GenerateRunways()
        {
            var runways = new List<Runway>();
            int count = UnityEngine.Random.Range(1, 5);
            
            for (int i = 0; i < count; i++)
            {
                int heading = UnityEngine.Random.Range(1, 37) * 10;
                runways.Add(new Runway
                {
                    Id = $"{heading:D2}L",
                    Length = UnityEngine.Random.Range(1500f, 5000f),
                    Width = UnityEngine.Random.Range(30f, 75f),
                    Surface = (RunwaySurface)UnityEngine.Random.Range(0, 4),
                    ILSEquipped = UnityEngine.Random.value > 0.3f,
                    LightingType = (RunwayLighting)UnityEngine.Random.Range(0, 4)
                });
            }
            
            return runways;
        }
        
        private List<string> GenerateAirlines()
        {
            string[] airlines = { "Global Air", "Sky Wings", "United Express", "Pacific Airlines", "Continental", "Delta Force Air", "Eagle Airways", "Phoenix Airlines" };
            return airlines.OrderBy(_ => UnityEngine.Random.value).Take(UnityEngine.Random.Range(3, 15)).ToList();
        }
        
        private List<AirportService> GenerateAirportServices()
        {
            var services = new List<AirportService>();
            foreach (AirportServiceType serviceType in Enum.GetValues(typeof(AirportServiceType)))
            {
                if (UnityEngine.Random.value > 0.2f)
                {
                    services.Add(new AirportService
                    {
                        Type = serviceType,
                        Available = true,
                        Cost = UnityEngine.Random.Range(10f, 10000f)
                    });
                }
            }
            return services;
        }
        
        private List<FuelType> GenerateFuelTypes()
        {
            var fuels = new List<FuelType>();
            foreach (FuelType fuel in Enum.GetValues(typeof(FuelType)))
            {
                if (UnityEngine.Random.value > 0.3f)
                {
                    fuels.Add(fuel);
                }
            }
            return fuels;
        }
        
        private List<ATCFrequency> GenerateATCFrequencies()
        {
            var frequencies = new List<ATCFrequency>
            {
                new ATCFrequency { Name = "Ground", Frequency = 121.0f + UnityEngine.Random.Range(0f, 5f) },
                new ATCFrequency { Name = "Tower", Frequency = 118.0f + UnityEngine.Random.Range(0f, 5f) },
                new ATCFrequency { Name = "Approach", Frequency = 124.0f + UnityEngine.Random.Range(0f, 5f) },
                new ATCFrequency { Name = "Departure", Frequency = 125.0f + UnityEngine.Random.Range(0f, 5f) },
                new ATCFrequency { Name = "ATIS", Frequency = 127.0f + UnityEngine.Random.Range(0f, 3f) }
            };
            return frequencies;
        }
        
        private List<MilitaryUnit> GenerateMilitaryUnits()
        {
            var units = new List<MilitaryUnit>();
            string[] unitNames = { "1st Fighter Wing", "2nd Bomber Group", "3rd Transport Squadron", "4th Reconnaissance Wing", "5th Attack Squadron" };
            
            for (int i = 0; i < UnityEngine.Random.Range(2, 8); i++)
            {
                units.Add(new MilitaryUnit
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = unitNames[i % unitNames.Length],
                    Type = (MilitaryUnitType)UnityEngine.Random.Range(0, 6),
                    Personnel = UnityEngine.Random.Range(100, 2000),
                    Aircraft = UnityEngine.Random.Range(10, 100),
                    ReadinessLevel = UnityEngine.Random.Range(1, 10)
                });
            }
            
            return units;
        }
        
        private List<MilitaryEquipment> GenerateMilitaryEquipment()
        {
            var equipment = new List<MilitaryEquipment>();
            
            string[] equipmentNames = { "F-16 Fighting Falcon", "F-35 Lightning II", "B-2 Spirit", "C-130 Hercules", "AH-64 Apache", "Patriot Missile System" };
            
            for (int i = 0; i < UnityEngine.Random.Range(5, 20); i++)
            {
                equipment.Add(new MilitaryEquipment
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = equipmentNames[UnityEngine.Random.Range(0, equipmentNames.Length)],
                    Type = (EquipmentType)UnityEngine.Random.Range(0, 8),
                    Quantity = UnityEngine.Random.Range(1, 50),
                    Condition = UnityEngine.Random.Range(50, 100)
                });
            }
            
            return equipment;
        }
        
        private List<NavalShip> GenerateNavalShips()
        {
            var ships = new List<NavalShip>();
            string[] shipNames = { "Valor", "Freedom", "Justice", "Liberty", "Victory", "Thunder", "Storm", "Phoenix" };
            
            for (int i = 0; i < UnityEngine.Random.Range(5, 30); i++)
            {
                ships.Add(new NavalShip
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"USS {shipNames[UnityEngine.Random.Range(0, shipNames.Length)]}",
                    Type = (NavalShipType)UnityEngine.Random.Range(0, 10),
                    Displacement = UnityEngine.Random.Range(1000f, 100000f),
                    Crew = UnityEngine.Random.Range(50, 5000),
                    Armament = GenerateShipArmament()
                });
            }
            
            return ships;
        }
        
        private List<string> GenerateShipArmament()
        {
            string[] weapons = { "Harpoon Missiles", "Tomahawk Cruise Missiles", "5-inch Gun", "Phalanx CIWS", "Torpedo Tubes", "VLS Cells" };
            return weapons.OrderBy(_ => UnityEngine.Random.value).Take(UnityEngine.Random.Range(2, 6)).ToList();
        }
        
        private List<CoastalDefense> GenerateCoastalDefenses()
        {
            var defenses = new List<CoastalDefense>();
            
            for (int i = 0; i < UnityEngine.Random.Range(3, 15); i++)
            {
                defenses.Add(new CoastalDefense
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = (CoastalDefenseType)UnityEngine.Random.Range(0, 5),
                    Range = UnityEngine.Random.Range(10f, 200f),
                    Firepower = UnityEngine.Random.Range(1, 10)
                });
            }
            
            return defenses;
        }
        
        private List<Commodity> GenerateCommodities()
        {
            var commodities = new List<Commodity>();
            string[] commodityNames = { "Electronics", "Machinery", "Fuel", "Food", "Textiles", "Weapons", "Medical Supplies", "Raw Materials" };
            
            for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
            {
                commodities.Add(new Commodity
                {
                    Name = commodityNames[UnityEngine.Random.Range(0, commodityNames.Length)],
                    Quantity = UnityEngine.Random.Range(100, 10000),
                    Value = UnityEngine.Random.Range(100f, 100000f)
                });
            }
            
            return commodities;
        }
        
        private List<ConflictObjective> GenerateConflictObjectives()
        {
            var objectives = new List<ConflictObjective>();
            string[] objectiveNames = { "Capture Airfield", "Destroy Radar", "Escort Convoy", "Intercept Bombers", "Defend Base", "Strike Target" };
            
            for (int i = 0; i < UnityEngine.Random.Range(2, 6); i++)
            {
                objectives.Add(new ConflictObjective
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = objectiveNames[UnityEngine.Random.Range(0, objectiveNames.Length)],
                    Type = (ObjectiveType)UnityEngine.Random.Range(0, 8),
                    Priority = UnityEngine.Random.Range(1, 10),
                    RewardMultiplier = UnityEngine.Random.Range(1f, 5f)
                });
            }
            
            return objectives;
        }
        
        private ConflictRewards GenerateConflictRewards()
        {
            return new ConflictRewards
            {
                Experience = UnityEngine.Random.Range(1000, 100000),
                Money = UnityEngine.Random.Range(10000f, 10000000f),
                Reputation = UnityEngine.Random.Range(100, 10000),
                UniqueItems = new List<string> { "Rare Aircraft Skin", "Elite Pilot Badge", "Victory Medal" }
            };
        }
        
        // Terrain streaming
        private void Update()
        {
            if (Camera.main != null)
            {
                UpdateTerrainChunks(Camera.main.transform.position);
            }
        }
        
        private void UpdateTerrainChunks(Vector3 viewerPosition)
        {
            Vector2Int currentChunk = new Vector2Int(
                Mathf.FloorToInt(viewerPosition.x / terrainChunkSize),
                Mathf.FloorToInt(viewerPosition.z / terrainChunkSize)
            );
            
            // Queue nearby chunks for loading
            int viewRadius = Mathf.CeilToInt(viewDistance / terrainChunkSize);
            for (int x = -viewRadius; x <= viewRadius; x++)
            {
                for (int z = -viewRadius; z <= viewRadius; z++)
                {
                    Vector2Int chunkCoord = new Vector2Int(currentChunk.x + x, currentChunk.y + z);
                    if (!loadedChunks.ContainsKey(chunkCoord) && !chunkLoadQueue.Contains(chunkCoord))
                    {
                        chunkLoadQueue.Enqueue(chunkCoord);
                    }
                }
            }
            
            // Load one chunk per frame
            if (chunkLoadQueue.Count > 0 && loadedChunks.Count < maxLoadedChunks)
            {
                Vector2Int chunkToLoad = chunkLoadQueue.Dequeue();
                LoadChunk(chunkToLoad);
            }
            
            // Unload distant chunks
            var chunksToUnload = loadedChunks.Keys
                .Where(coord => Vector2Int.Distance(coord, currentChunk) > viewRadius + 2)
                .ToList();
            
            foreach (var coord in chunksToUnload)
            {
                UnloadChunk(coord);
            }
        }
        
        private void LoadChunk(Vector2Int coord)
        {
            var chunk = new TerrainChunk
            {
                Coordinate = coord,
                Position = new Vector3(coord.x * terrainChunkSize, 0, coord.y * terrainChunkSize),
                Size = terrainChunkSize,
                IsLoaded = true,
                HeightData = GenerateHeightData(coord),
                BiomeData = GenerateBiomeData(coord)
            };
            
            loadedChunks[coord] = chunk;
        }
        
        private void UnloadChunk(Vector2Int coord)
        {
            if (loadedChunks.TryGetValue(coord, out var chunk))
            {
                chunk.IsLoaded = false;
                loadedChunks.Remove(coord);
            }
        }
        
        private float[,] GenerateHeightData(Vector2Int coord)
        {
            int resolution = 33;
            float[,] heights = new float[resolution, resolution];
            
            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    float worldX = coord.x * terrainChunkSize + x * (terrainChunkSize / (resolution - 1));
                    float worldZ = coord.y * terrainChunkSize + z * (terrainChunkSize / (resolution - 1));
                    
                    // Multi-octave noise for realistic terrain
                    float height = 0;
                    float amplitude = 1;
                    float frequency = 0.0001f;
                    
                    for (int octave = 0; octave < 6; octave++)
                    {
                        height += Mathf.PerlinNoise(worldX * frequency, worldZ * frequency) * amplitude;
                        amplitude *= 0.5f;
                        frequency *= 2;
                    }
                    
                    heights[x, z] = height;
                }
            }
            
            return heights;
        }
        
        private BiomeType[,] GenerateBiomeData(Vector2Int coord)
        {
            int resolution = 33;
            BiomeType[,] biomes = new BiomeType[resolution, resolution];
            
            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    float worldX = coord.x * terrainChunkSize + x * (terrainChunkSize / (resolution - 1));
                    float worldZ = coord.y * terrainChunkSize + z * (terrainChunkSize / (resolution - 1));
                    
                    float temperature = Mathf.PerlinNoise(worldX * 0.00001f, worldZ * 0.00001f);
                    float moisture = Mathf.PerlinNoise(worldX * 0.00002f + 1000, worldZ * 0.00002f + 1000);
                    
                    biomes[x, z] = GetBiomeFromClimate(temperature, moisture);
                }
            }
            
            return biomes;
        }
        
        private BiomeType GetBiomeFromClimate(float temperature, float moisture)
        {
            if (temperature < 0.2f) return BiomeType.Tundra;
            if (temperature < 0.4f) return moisture > 0.5f ? BiomeType.Taiga : BiomeType.Steppe;
            if (temperature < 0.6f) return moisture > 0.6f ? BiomeType.TemperateForest : BiomeType.Grassland;
            if (temperature < 0.8f) return moisture > 0.7f ? BiomeType.TropicalForest : BiomeType.Savanna;
            return moisture < 0.3f ? BiomeType.Desert : BiomeType.TropicalForest;
        }
        
        // Public API
        public List<Airport> GetNearbyAirports(Vector3 position, float radius)
        {
            return airports.Where(a => Vector3.Distance(a.Position, position) < radius).ToList();
        }
        
        public List<City> GetNearbyCities(Vector3 position, float radius)
        {
            return cities.Where(c => Vector3.Distance(c.Position, position) < radius).ToList();
        }
        
        public List<ConflictZone> GetActiveConflicts()
        {
            return conflictZones.Where(c => c.Intensity > 5).ToList();
        }
        
        public Country GetCountryAtPosition(Vector3 position)
        {
            return countries.OrderBy(c => Vector3.Distance(c.Position, position)).FirstOrDefault();
        }
    }
    
    // Data structures
    [Serializable]
    public class WorldStatistics
    {
        public int TotalContinents;
        public int TotalCountries;
        public int TotalCities;
        public int TotalAirports;
        public int TotalMilitaryBases;
        public int TotalNavalBases;
        public int TotalLandmarks;
        public int TotalTradeRoutes;
        public int TotalConflictZones;
        public long TotalPopulation;
        public double TotalGDP;
        public float WorldSize;
        
        public override string ToString()
        {
            return $"World: {TotalContinents} continents, {TotalCountries} countries, {TotalCities} cities, {TotalAirports} airports, " +
                   $"{TotalMilitaryBases} military bases, {TotalNavalBases} naval bases, {TotalLandmarks} landmarks, " +
                   $"{TotalTradeRoutes} trade routes, {TotalConflictZones} conflict zones, Pop: {TotalPopulation:N0}, GDP: ${TotalGDP:N0}";
        }
    }
    
    [Serializable]
    public class Continent
    {
        public string Id;
        public string Name;
        public Vector3 Position;
        public float Size;
        public ClimateType Climate;
        public Dictionary<BiomeType, float> BiomeDistribution;
        public float PopulationDensity;
        public int TechLevel;
        public string PrimaryFaction;
    }
    
    [Serializable]
    public class Country
    {
        public string Id;
        public string Name;
        public string ContinentId;
        public Vector3 Position;
        public float Size;
        public long Population;
        public double GDP;
        public int MilitaryStrength;
        public GovernmentType GovernmentType;
        public string Faction;
        public Dictionary<string, float> Relations;
        public Dictionary<ResourceType, float> Resources;
        public List<IndustryType> Industries;
    }
    
    [Serializable]
    public class City
    {
        public string Id;
        public string Name;
        public string CountryId;
        public Vector3 Position;
        public int Population;
        public bool IsCapital;
        public CityType CityType;
        public int Infrastructure;
        public int Tourism;
        public int Industry;
        public int Commerce;
        public List<District> Districts;
        public List<string> Landmarks;
        public List<CityService> Services;
    }
    
    [Serializable]
    public class District
    {
        public string Id;
        public string Name;
        public DistrictType Type;
        public int Population;
        public float Area;
        public int Prosperity;
    }
    
    [Serializable]
    public class CityService
    {
        public CityServiceType Type;
        public int Quality;
        public int Capacity;
    }
    
    [Serializable]
    public class Airport
    {
        public string Id;
        public string Name;
        public string ICAOCode;
        public string IATACode;
        public string CityId;
        public string CountryId;
        public Vector3 Position;
        public float Elevation;
        public List<Runway> Runways;
        public int Terminals;
        public int Gates;
        public int AnnualPassengers;
        public float AnnualCargo;
        public List<string> Airlines;
        public List<AirportService> Services;
        public List<FuelType> FuelTypes;
        public List<ATCFrequency> ATCFrequencies;
    }
    
    [Serializable]
    public class Runway
    {
        public string Id;
        public float Length;
        public float Width;
        public RunwaySurface Surface;
        public bool ILSEquipped;
        public RunwayLighting LightingType;
    }
    
    [Serializable]
    public class AirportService
    {
        public AirportServiceType Type;
        public bool Available;
        public float Cost;
    }
    
    [Serializable]
    public class ATCFrequency
    {
        public string Name;
        public float Frequency;
    }
    
    [Serializable]
    public class MilitaryBase
    {
        public string Id;
        public string Name;
        public string CountryId;
        public Vector3 Position;
        public MilitaryBaseType BaseType;
        public int SecurityLevel;
        public int Personnel;
        public int AircraftCapacity;
        public int DefenseRating;
        public float RadarRange;
        public int SAMSites;
        public int Hangars;
        public List<Runway> Runways;
        public List<MilitaryUnit> StationedUnits;
        public List<MilitaryEquipment> Equipment;
    }
    
    [Serializable]
    public class MilitaryUnit
    {
        public string Id;
        public string Name;
        public MilitaryUnitType Type;
        public int Personnel;
        public int Aircraft;
        public int ReadinessLevel;
    }
    
    [Serializable]
    public class MilitaryEquipment
    {
        public string Id;
        public string Name;
        public EquipmentType Type;
        public int Quantity;
        public int Condition;
    }
    
    [Serializable]
    public class NavalBase
    {
        public string Id;
        public string Name;
        public string CountryId;
        public Vector3 Position;
        public NavalBaseType BaseType;
        public int Piers;
        public int DrydockCapacity;
        public int SubmarinePens;
        public int AircraftCarrierBerths;
        public int Personnel;
        public List<NavalShip> Ships;
        public List<CoastalDefense> CoastalDefenses;
    }
    
    [Serializable]
    public class NavalShip
    {
        public string Id;
        public string Name;
        public NavalShipType Type;
        public float Displacement;
        public int Crew;
        public List<string> Armament;
    }
    
    [Serializable]
    public class CoastalDefense
    {
        public string Id;
        public CoastalDefenseType Type;
        public float Range;
        public int Firepower;
    }
    
    [Serializable]
    public class Landmark
    {
        public string Id;
        public string Name;
        public string CityId;
        public Vector3 Position;
        public LandmarkType Type;
        public float Height;
        public int YearBuilt;
        public int HistoricalSignificance;
        public int TouristRating;
        public int DailyVisitors;
    }
    
    [Serializable]
    public class TradeRoute
    {
        public string Id;
        public string OriginId;
        public string DestinationId;
        public float Distance;
        public TradeRouteType RouteType;
        public float Volume;
        public float Value;
        public int Frequency;
        public List<Commodity> Commodities;
        public int RiskLevel;
        public bool IsLegal;
    }
    
    [Serializable]
    public class Commodity
    {
        public string Name;
        public int Quantity;
        public float Value;
    }
    
    [Serializable]
    public class ConflictZone
    {
        public string Id;
        public string Name;
        public Vector3 Position;
        public float Radius;
        public ConflictType ConflictType;
        public int Intensity;
        public string FactionA;
        public string FactionB;
        public List<ConflictObjective> Objectives;
        public List<string> ActiveMissions;
        public ConflictRewards Rewards;
    }
    
    [Serializable]
    public class ConflictObjective
    {
        public string Id;
        public string Name;
        public ObjectiveType Type;
        public int Priority;
        public float RewardMultiplier;
    }
    
    [Serializable]
    public class ConflictRewards
    {
        public int Experience;
        public float Money;
        public int Reputation;
        public List<string> UniqueItems;
    }
    
    [Serializable]
    public class TerrainChunk
    {
        public Vector2Int Coordinate;
        public Vector3 Position;
        public int Size;
        public bool IsLoaded;
        public float[,] HeightData;
        public BiomeType[,] BiomeData;
    }
    
    // Enums
    public enum ClimateType { Tropical, Subtropical, Temperate, Continental, Polar, Arid }
    public enum BiomeType { Desert, Grassland, Savanna, TropicalForest, TemperateForest, Taiga, Tundra, Steppe, Mediterranean, Rainforest }
    public enum GovernmentType { Democracy, Republic, Monarchy, Dictatorship, Theocracy, Federation, Confederation, Communist }
    public enum ResourceType { Oil, Gas, Coal, Iron, Gold, Silver, Copper, Uranium, Diamonds, RareEarth, Timber, Fish, Agriculture }
    public enum IndustryType { Aerospace, Automotive, Electronics, Pharmaceuticals, Steel, Shipbuilding, Agriculture, Mining, Tourism, Finance, Technology }
    public enum CityType { Metropolis, LargeCity, MediumCity, SmallCity, Town }
    public enum DistrictType { Residential, Commercial, Industrial, Historic, Financial, Entertainment, Government, Military, University, Mixed }
    public enum CityServiceType { Police, Fire, Medical, Education, Transportation, Utilities, Sanitation, Parks, Social }
    public enum RunwaySurface { Asphalt, Concrete, Grass, Gravel }
    public enum RunwayLighting { None, Basic, Standard, HighIntensity }
    public enum AirportServiceType { Fuel, Maintenance, Hangar, Customs, Catering, GroundHandling, DeIcing, Cargo }
    public enum FuelType { AvGas, JetA, JetA1, JP8 }
    public enum MilitaryBaseType { AirForce, Army, Navy, JointOperations, SpecialForces }
    public enum MilitaryUnitType { Fighter, Bomber, Transport, Reconnaissance, Attack, Support }
    public enum EquipmentType { Fighter, Bomber, Transport, Helicopter, Drone, SAM, Radar, Vehicle }
    public enum NavalBaseType { Fleet, Submarine, Shipyard, Training }
    public enum NavalShipType { Carrier, Battleship, Cruiser, Destroyer, Frigate, Submarine, Amphibious, Support, Patrol, Corvette }
    public enum CoastalDefenseType { Artillery, Missile, Radar, Minefield, Bunker }
    public enum LandmarkType { Monument, Tower, Bridge, Dam, Stadium, Museum, Temple, Castle, Palace, Statue, Park, Market, Theater, University, Hospital }
    public enum TradeRouteType { Air, Sea, Land, Rail, Pipeline }
    public enum ConflictType { War, Skirmish, Insurgency, CivilWar, Blockade, Sanctions }
    public enum ObjectiveType { Capture, Destroy, Defend, Escort, Intercept, Reconnaissance, Supply, Evacuation }
}
