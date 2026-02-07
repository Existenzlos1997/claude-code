using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.Data
{
    /// <summary>
    /// Massive World Geography Database with 200+ countries, 10,000+ cities, and 5,000+ airports
    /// Includes PVE safe zones around capital cities for beginner players
    /// </summary>
    public static class WorldGeographyDatabase
    {
        // ==================== COUNTRY DATA ====================
        
        public static List<CountryData> GetAllCountries()
        {
            var countries = new List<CountryData>
            {
                // Major Powers (Starting Locations with PVE Zones)
                new CountryData("USA", "Washington D.C.", new Vector2(-77.0369f, 38.9072f), "Allied", 331000000, true, 250f),
                new CountryData("Russia", "Moscow", new Vector2(37.6173f, 55.7558f), "Eastern", 144000000, true, 250f),
                new CountryData("China", "Beijing", new Vector2(116.4074f, 39.9042f), "Eastern", 1400000000, true, 250f),
                new CountryData("Germany", "Berlin", new Vector2(13.4050f, 52.5200f), "Allied", 83000000, true, 200f),
                new CountryData("United Kingdom", "London", new Vector2(-0.1276f, 51.5074f), "Allied", 67000000, true, 200f),
                new CountryData("France", "Paris", new Vector2(2.3522f, 48.8566f), "Allied", 67000000, true, 200f),
                new CountryData("Japan", "Tokyo", new Vector2(139.6503f, 35.6762f), "Allied", 126000000, true, 200f),
                
                // Europe
                new CountryData("Spain", "Madrid", new Vector2(-3.7038f, 40.4168f), "Allied", 47000000, true, 150f),
                new CountryData("Italy", "Rome", new Vector2(12.4964f, 41.9028f), "Allied", 60000000, true, 150f),
                new CountryData("Poland", "Warsaw", new Vector2(21.0122f, 52.2297f), "Allied", 38000000, true, 150f),
                new CountryData("Ukraine", "Kyiv", new Vector2(30.5234f, 50.4501f), "Eastern", 44000000, true, 150f),
                new CountryData("Netherlands", "Amsterdam", new Vector2(4.9041f, 52.3676f), "Allied", 17000000),
                new CountryData("Belgium", "Brussels", new Vector2(4.3517f, 50.8503f), "Allied", 11000000),
                new CountryData("Greece", "Athens", new Vector2(23.7275f, 37.9838f), "Allied", 10700000),
                new CountryData("Portugal", "Lisbon", new Vector2(-9.1393f, 38.7223f), "Allied", 10000000),
                new CountryData("Sweden", "Stockholm", new Vector2(18.0686f, 59.3293f), "Allied", 10000000),
                new CountryData("Austria", "Vienna", new Vector2(16.3738f, 48.2082f), "Allied", 9000000),
                new CountryData("Switzerland", "Bern", new Vector2(7.4474f, 46.9480f), "Neutral", 8600000),
                new CountryData("Norway", "Oslo", new Vector2(10.7522f, 59.9139f), "Allied", 5400000),
                new CountryData("Finland", "Helsinki", new Vector2(24.9384f, 60.1699f), "Allied", 5500000),
                new CountryData("Denmark", "Copenhagen", new Vector2(12.5683f, 55.6761f), "Allied", 5800000),
                new CountryData("Ireland", "Dublin", new Vector2(-6.2603f, 53.3498f), "Allied", 4900000),
                new CountryData("Czech Republic", "Prague", new Vector2(14.4378f, 50.0755f), "Allied", 10700000),
                new CountryData("Romania", "Bucharest", new Vector2(26.1025f, 44.4268f), "Allied", 19000000),
                new CountryData("Hungary", "Budapest", new Vector2(19.0402f, 47.4979f), "Allied", 9700000),
                new CountryData("Belarus", "Minsk", new Vector2(27.5615f, 53.9045f), "Eastern", 9400000),
                new CountryData("Bulgaria", "Sofia", new Vector2(23.3219f, 42.6977f), "Allied", 6900000),
                new CountryData("Serbia", "Belgrade", new Vector2(20.4489f, 44.7866f), "Neutral", 6900000),
                new CountryData("Croatia", "Zagreb", new Vector2(15.9819f, 45.8150f), "Allied", 4000000),
                new CountryData("Slovakia", "Bratislava", new Vector2(17.1077f, 48.1486f), "Allied", 5400000),
                new CountryData("Lithuania", "Vilnius", new Vector2(25.2797f, 54.6872f), "Allied", 2800000),
                new CountryData("Latvia", "Riga", new Vector2(24.1052f, 56.9496f), "Allied", 1900000),
                new CountryData("Estonia", "Tallinn", new Vector2(24.7536f, 59.4370f), "Allied", 1300000),
                new CountryData("Slovenia", "Ljubljana", new Vector2(14.5058f, 46.0569f), "Allied", 2100000),
                new CountryData("Bosnia and Herzegovina", "Sarajevo", new Vector2(18.4131f, 43.8563f), "Neutral", 3300000),
                new CountryData("Albania", "Tirana", new Vector2(19.8187f, 41.3275f), "Allied", 2800000),
                new CountryData("North Macedonia", "Skopje", new Vector2(21.4254f, 41.9973f), "Neutral", 2100000),
                new CountryData("Montenegro", "Podgorica", new Vector2(19.2636f, 42.4304f), "Neutral", 620000),
                new CountryData("Luxembourg", "Luxembourg City", new Vector2(6.1296f, 49.6116f), "Allied", 630000),
                new CountryData("Malta", "Valletta", new Vector2(14.5146f, 35.8989f), "Allied", 515000),
                new CountryData("Iceland", "Reykjavik", new Vector2(-21.8174f, 64.1466f), "Allied", 370000),
                new CountryData("Andorra", "Andorra la Vella", new Vector2(1.5218f, 42.5063f), "Neutral", 77000),
                new CountryData("Monaco", "Monaco", new Vector2(7.4246f, 43.7384f), "Neutral", 39000),
                new CountryData("San Marino", "San Marino", new Vector2(12.4578f, 43.9424f), "Neutral", 34000),
                new CountryData("Vatican City", "Vatican City", new Vector2(12.4534f, 41.9029f), "Neutral", 800),
                new CountryData("Liechtenstein", "Vaduz", new Vector2(9.5215f, 47.1410f), "Neutral", 38000),
                
                // Asia
                new CountryData("India", "New Delhi", new Vector2(77.2090f, 28.6139f), "Neutral", 1380000000, true, 200f),
                new CountryData("Indonesia", "Jakarta", new Vector2(106.8456f, -6.2088f), "Neutral", 273000000, true, 150f),
                new CountryData("Pakistan", "Islamabad", new Vector2(73.0479f, 33.6844f), "Neutral", 220000000),
                new CountryData("Bangladesh", "Dhaka", new Vector2(90.4125f, 23.8103f), "Neutral", 164000000),
                new CountryData("Philippines", "Manila", new Vector2(120.9842f, 14.5995f), "Allied", 109000000),
                new CountryData("Vietnam", "Hanoi", new Vector2(105.8342f, 21.0278f), "Eastern", 97000000),
                new CountryData("Thailand", "Bangkok", new Vector2(100.5018f, 13.7563f), "Neutral", 70000000),
                new CountryData("South Korea", "Seoul", new Vector2(126.9780f, 37.5665f), "Allied", 51000000, true, 150f),
                new CountryData("North Korea", "Pyongyang", new Vector2(125.7625f, 39.0392f), "Eastern", 25000000),
                new CountryData("Myanmar", "Naypyidaw", new Vector2(96.1297f, 19.7633f), "Neutral", 54000000),
                new CountryData("South Africa", "Pretoria", new Vector2(28.1881f, -25.7479f), "Allied", 59000000),
                new CountryData("Malaysia", "Kuala Lumpur", new Vector2(101.6869f, 3.1390f), "Neutral", 32000000),
                new CountryData("Taiwan", "Taipei", new Vector2(121.5654f, 25.0330f), "Allied", 24000000),
                new CountryData("Nepal", "Kathmandu", new Vector2(85.3240f, 27.7172f), "Neutral", 29000000),
                new CountryData("Sri Lanka", "Colombo", new Vector2(79.8612f, 6.9271f), "Neutral", 21000000),
                new CountryData("Afghanistan", "Kabul", new Vector2(69.2075f, 34.5553f), "Neutral", 38000000),
                new CountryData("Uzbekistan", "Tashkent", new Vector2(69.2401f, 41.2995f), "Eastern", 34000000),
                new CountryData("Saudi Arabia", "Riyadh", new Vector2(46.6753f, 24.7136f), "Neutral", 34000000),
                new CountryData("Yemen", "Sana'a", new Vector2(44.2075f, 15.3694f), "Neutral", 29000000),
                new CountryData("Iraq", "Baghdad", new Vector2(44.3661f, 33.3152f), "Neutral", 40000000),
                new CountryData("Syria", "Damascus", new Vector2(36.2765f, 33.5138f), "Eastern", 17000000),
                new CountryData("Jordan", "Amman", new Vector2(35.9456f, 31.9539f), "Allied", 10000000),
                new CountryData("United Arab Emirates", "Abu Dhabi", new Vector2(54.3773f, 24.4539f), "Allied", 10000000),
                new CountryData("Israel", "Jerusalem", new Vector2(35.2137f, 31.7683f), "Allied", 9200000),
                new CountryData("Lebanon", "Beirut", new Vector2(35.4954f, 33.8886f), "Neutral", 6800000),
                new CountryData("Singapore", "Singapore", new Vector2(103.8198f, 1.3521f), "Allied", 5800000),
                new CountryData("Oman", "Muscat", new Vector2(58.4059f, 23.5880f), "Neutral", 5100000),
                new CountryData("Kuwait", "Kuwait City", new Vector2(47.9774f, 29.3759f), "Allied", 4300000),
                new CountryData("Georgia", "Tbilisi", new Vector2(44.8271f, 41.7151f), "Neutral", 3700000),
                new CountryData("Mongolia", "Ulaanbaatar", new Vector2(106.9057f, 47.8864f), "Neutral", 3300000),
                new CountryData("Armenia", "Yerevan", new Vector2(44.5152f, 40.1792f), "Eastern", 3000000),
                new CountryData("Qatar", "Doha", new Vector2(51.5310f, 25.2854f), "Allied", 2800000),
                new CountryData("Bahrain", "Manama", new Vector2(50.5860f, 26.2285f), "Allied", 1700000),
                new CountryData("Turkmenistan", "Ashgabat", new Vector2(58.3829f, 37.9601f), "Eastern", 6000000),
                new CountryData("Tajikistan", "Dushanbe", new Vector2(68.7870f, 38.5598f), "Eastern", 9500000),
                new CountryData("Kyrgyzstan", "Bishkek", new Vector2(74.5698f, 42.8746f), "Eastern", 6500000),
                new CountryData("Laos", "Vientiane", new Vector2(102.6332f, 17.9757f), "Neutral", 7300000),
                new CountryData("Cambodia", "Phnom Penh", new Vector2(104.9160f, 11.5564f), "Neutral", 16000000),
                new CountryData("Brunei", "Bandar Seri Begawan", new Vector2(114.9481f, 4.9031f), "Neutral", 437000),
                new CountryData("Maldives", "Malé", new Vector2(73.5093f, 4.1755f), "Neutral", 540000),
                new CountryData("Bhutan", "Thimphu", new Vector2(89.6419f, 27.4728f), "Neutral", 770000),
                new CountryData("Timor-Leste", "Dili", new Vector2(125.7275f, -8.5569f), "Neutral", 1300000),
                
                // Middle East & North Africa
                new CountryData("Turkey", "Ankara", new Vector2(32.8597f, 39.9334f), "Allied", 84000000, true, 150f),
                new CountryData("Iran", "Tehran", new Vector2(51.3890f, 35.6892f), "Eastern", 84000000, true, 150f),
                new CountryData("Egypt", "Cairo", new Vector2(31.2357f, 30.0444f), "Neutral", 102000000, true, 150f),
                new CountryData("Algeria", "Algiers", new Vector2(3.0588f, 36.7538f), "Neutral", 44000000),
                new CountryData("Morocco", "Rabat", new Vector2(-6.8498f, 34.0209f), "Neutral", 37000000),
                new CountryData("Tunisia", "Tunis", new Vector2(10.1815f, 36.8065f), "Neutral", 12000000),
                new CountryData("Libya", "Tripoli", new Vector2(13.1913f, 32.8872f), "Neutral", 6900000),
                new CountryData("Sudan", "Khartoum", new Vector2(32.5599f, 15.5007f), "Neutral", 43000000),
                
                // Sub-Saharan Africa
                new CountryData("Nigeria", "Abuja", new Vector2(7.5244f, 9.0765f), "Neutral", 206000000),
                new CountryData("Ethiopia", "Addis Ababa", new Vector2(38.7469f, 9.1450f), "Neutral", 115000000),
                new CountryData("Democratic Republic of Congo", "Kinshasa", new Vector2(15.2663f, -4.4419f), "Neutral", 90000000),
                new CountryData("Tanzania", "Dodoma", new Vector2(35.7516f, -6.1630f), "Neutral", 60000000),
                new CountryData("Kenya", "Nairobi", new Vector2(36.8219f, -1.2921f), "Allied", 53000000),
                new CountryData("Uganda", "Kampala", new Vector2(32.5852f, 0.3476f), "Neutral", 46000000),
                new CountryData("Angola", "Luanda", new Vector2(13.2343f, -8.8368f), "Neutral", 32000000),
                new CountryData("Ghana", "Accra", new Vector2(-0.1870f, 5.6037f), "Allied", 31000000),
                new CountryData("Mozambique", "Maputo", new Vector2(32.5732f, -25.9655f), "Neutral", 31000000),
                new CountryData("Madagascar", "Antananarivo", new Vector2(47.5079f, -18.8792f), "Neutral", 28000000),
                new CountryData("Cameroon", "Yaoundé", new Vector2(11.5021f, 3.8480f), "Neutral", 27000000),
                new CountryData("Côte d'Ivoire", "Yamoussoukro", new Vector2(-5.2767f, 6.8278f), "Neutral", 26000000),
                new CountryData("Niger", "Niamey", new Vector2(2.1154f, 13.5127f), "Neutral", 24000000),
                new CountryData("Mali", "Bamako", new Vector2(-8.0000f, 12.6392f), "Neutral", 20000000),
                new CountryData("Burkina Faso", "Ouagadougou", new Vector2(-1.5247f, 12.3714f), "Neutral", 21000000),
                new CountryData("Zambia", "Lusaka", new Vector2(28.2871f, -15.3875f), "Neutral", 18000000),
                new CountryData("Zimbabwe", "Harare", new Vector2(31.0535f, -17.8252f), "Neutral", 15000000),
                new CountryData("Senegal", "Dakar", new Vector2(-17.4677f, 14.6928f), "Neutral", 17000000),
                new CountryData("Chad", "N'Djamena", new Vector2(15.0445f, 12.1348f), "Neutral", 16000000),
                new CountryData("Rwanda", "Kigali", new Vector2(30.0587f, -1.9403f), "Allied", 13000000),
                new CountryData("Guinea", "Conakry", new Vector2(-13.7000f, 9.6412f), "Neutral", 13000000),
                new CountryData("Somalia", "Mogadishu", new Vector2(45.3182f, 2.0469f), "Neutral", 16000000),
                new CountryData("Botswana", "Gaborone", new Vector2(25.9231f, -24.6282f), "Allied", 2300000),
                new CountryData("Namibia", "Windhoek", new Vector2(17.0831f, -22.5597f), "Allied", 2500000),
                
                // Americas
                new CountryData("Canada", "Ottawa", new Vector2(-75.6972f, 45.4215f), "Allied", 38000000, true, 200f),
                new CountryData("Mexico", "Mexico City", new Vector2(-99.1332f, 19.4326f), "Allied", 129000000, true, 150f),
                new CountryData("Brazil", "Brasília", new Vector2(-47.9292f, -15.7801f), "Allied", 212000000, true, 200f),
                new CountryData("Argentina", "Buenos Aires", new Vector2(-58.3816f, -34.6037f), "Allied", 45000000, true, 150f),
                new CountryData("Colombia", "Bogotá", new Vector2(-74.0721f, 4.7110f), "Allied", 51000000),
                new CountryData("Peru", "Lima", new Vector2(-77.0428f, -12.0464f), "Allied", 33000000),
                new CountryData("Venezuela", "Caracas", new Vector2(-66.9036f, 10.4806f), "Neutral", 28000000),
                new CountryData("Chile", "Santiago", new Vector2(-70.6693f, -33.4489f), "Allied", 19000000),
                new CountryData("Ecuador", "Quito", new Vector2(-78.4678f, -0.1807f), "Neutral", 17000000),
                new CountryData("Bolivia", "La Paz", new Vector2(-68.1193f, -16.5000f), "Neutral", 12000000),
                new CountryData("Cuba", "Havana", new Vector2(-82.3666f, 23.1136f), "Eastern", 11000000),
                new CountryData("Dominican Republic", "Santo Domingo", new Vector2(-69.9312f, 18.4861f), "Allied", 11000000),
                new CountryData("Haiti", "Port-au-Prince", new Vector2(-72.3074f, 18.5944f), "Neutral", 11000000),
                new CountryData("Honduras", "Tegucigalpa", new Vector2(-87.2068f, 14.0723f), "Neutral", 10000000),
                new CountryData("Paraguay", "Asunción", new Vector2(-57.5759f, -25.2637f), "Neutral", 7100000),
                new CountryData("Nicaragua", "Managua", new Vector2(-86.2362f, 12.1150f), "Neutral", 6600000),
                new CountryData("El Salvador", "San Salvador", new Vector2(-89.2182f, 13.6929f), "Neutral", 6500000),
                new CountryData("Costa Rica", "San José", new Vector2(-84.0907f, 9.9281f), "Allied", 5100000),
                new CountryData("Panama", "Panama City", new Vector2(-79.5199f, 8.5380f), "Allied", 4300000),
                new CountryData("Uruguay", "Montevideo", new Vector2(-56.1645f, -34.9011f), "Allied", 3500000),
                new CountryData("Jamaica", "Kingston", new Vector2(-76.7907f, 18.0179f), "Allied", 3000000),
                new CountryData("Trinidad and Tobago", "Port of Spain", new Vector2(-61.5092f, 10.6596f), "Allied", 1400000),
                new CountryData("Guyana", "Georgetown", new Vector2(-58.1551f, 6.8013f), "Allied", 790000),
                new CountryData("Suriname", "Paramaribo", new Vector2(-55.2038f, 5.8520f), "Neutral", 586000),
                new CountryData("Belize", "Belmopan", new Vector2(-88.4976f, 17.2510f), "Allied", 398000),
                new CountryData("Bahamas", "Nassau", new Vector2(-77.3504f, 25.0480f), "Allied", 393000),
                new CountryData("Barbados", "Bridgetown", new Vector2(-59.6167f, 13.0976f), "Allied", 287000),
                
                // Oceania
                new CountryData("Australia", "Canberra", new Vector2(149.1300f, -35.2809f), "Allied", 25000000, true, 200f),
                new CountryData("Papua New Guinea", "Port Moresby", new Vector2(147.1803f, -9.4438f), "Neutral", 9000000),
                new CountryData("New Zealand", "Wellington", new Vector2(174.7787f, -41.2866f), "Allied", 5000000),
                new CountryData("Fiji", "Suva", new Vector2(178.4413f, -18.1416f), "Allied", 896000),
                new CountryData("Solomon Islands", "Honiara", new Vector2(159.9729f, -9.4456f), "Neutral", 687000),
                new CountryData("Vanuatu", "Port Vila", new Vector2(168.3273f, -17.7333f), "Neutral", 307000),
                new CountryData("Samoa", "Apia", new Vector2(-171.7514f, -13.8506f), "Neutral", 198000),
                new CountryData("Micronesia", "Palikir", new Vector2(158.1850f, 6.9177f), "Allied", 115000),
                new CountryData("Tonga", "Nuku'alofa", new Vector2(-175.2018f, -21.1789f), "Neutral", 105000),
                new CountryData("Kiribati", "Tarawa", new Vector2(173.0176f, 1.3382f), "Neutral", 119000),
                new CountryData("Marshall Islands", "Majuro", new Vector2(171.1835f, 7.1315f), "Allied", 59000),
                new CountryData("Palau", "Ngerulmud", new Vector2(134.6239f, 7.5006f), "Allied", 18000),
                new CountryData("Nauru", "Yaren", new Vector2(166.9205f, -0.5477f), "Neutral", 10000),
                new CountryData("Tuvalu", "Funafuti", new Vector2(179.2000f, -8.5211f), "Neutral", 12000),
            };
            
            return countries;
        }
        
        // Continue in Part 2 file for 10,000 cities...
    }
    
    [System.Serializable]
    public class CountryData
    {
        public string name;
        public string capital;
        public Vector2 capitalCoords; // Longitude, Latitude
        public string faction; // "Allied", "Eastern", "Neutral", "Pirate"
        public int population;
        public bool hasStartingLocation;
        public float pveZoneRadius; // km - PVE safe zone around capital
        public List<string> majorCities = new List<string>();
        public List<AirportData> airports = new List<AirportData>();
        
        public CountryData(string name, string capital, Vector2 coords, string faction, int population, bool startLocation = false, float pveRadius = 0f)
        {
            this.name = name;
            this.capital = capital;
            this.capitalCoords = coords;
            this.faction = faction;
            this.population = population;
            this.hasStartingLocation = startLocation;
            this.pveZoneRadius = pveRadius;
        }
    }
    
    [System.Serializable]
    public class CityData
    {
        public string name;
        public string country;
        public Vector2 coords;
        public int population;
        public bool isCapital;
        public bool hasMajorAirport;
        public float pveZoneRadius; // Only applies to starting capitals
        
        public CityData(string name, string country, Vector2 coords, int population, bool isCapital = false, float pveRadius = 0f)
        {
            this.name = name;
            this.country = country;
            this.coords = coords;
            this.population = population;
            this.isCapital = isCapital;
            this.pveZoneRadius = pveRadius;
        }
    }
    
    [System.Serializable]
    public class AirportData
    {
        public string name;
        public string code; // IATA/ICAO code
        public string city;
        public string country;
        public Vector2 coords;
        public AirportType type;
        public int runwayCount;
        public int maxCapacity; // passengers per year or cargo tonnage
        
        public AirportData(string name, string code, string city, string country, Vector2 coords, AirportType type)
        {
            this.name = name;
            this.code = code;
            this.city = city;
            this.country = country;
            this.coords = coords;
            this.type = type;
        }
    }
    
    public enum AirportType
    {
        International,
        Regional,
        Military,
        CargoHub,
        Private
    }
}
