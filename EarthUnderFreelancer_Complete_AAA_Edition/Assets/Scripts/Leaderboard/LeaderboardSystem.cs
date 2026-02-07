using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.Leaderboard
{
    /// <summary>
    /// Global leaderboard and ranking system
    /// Tracks player achievements, stats, and competitive rankings
    /// </summary>
    
    [Serializable]
    public enum LeaderboardType
    {
        // Combat
        TotalKills,
        AirToAirKills,
        GroundKills,
        BombingScore,
        KDRatio,
        
        // Flight
        FlightHours,
        DistanceFlown,
        LandingsCompleted,
        PerfectLandings,
        
        // Economy
        TotalEarnings,
        TradesCompleted,
        SmugglingRuns,
        CargoDelivered,
        
        // Faction
        TerritoryConquered,
        FactionInfluence,
        PvPWins,
        
        // Progression
        TotalXP,
        HighestLevel,
        AchievementsUnlocked,
        QuestsCompleted,
        
        // Weekly/Seasonal
        WeeklyKills,
        WeeklyEarnings,
        SeasonalRank
    }
    
    [Serializable]
    public enum RankTier
    {
        Unranked,
        Bronze,
        Silver,
        Gold,
        Platinum,
        Diamond,
        Master,
        Grandmaster,
        Legend
    }
    
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerId;
        public string playerName;
        public string factionId;
        public float score;
        public int rank;
        public DateTime lastUpdated;
        public string region;
        
        public LeaderboardEntry(string id, string name, float s)
        {
            playerId = id;
            playerName = name;
            factionId = "";
            score = s;
            rank = 0;
            lastUpdated = DateTime.UtcNow;
            region = "global";
        }
    }
    
    [Serializable]
    public class PlayerRanking
    {
        public string playerId;
        public string playerName;
        public RankTier tier;
        public int division; // 1-5 within tier
        public int rankPoints;
        public int peakRankPoints;
        public int wins;
        public int losses;
        public float winRate;
        public int currentStreak;
        public int bestStreak;
        public DateTime seasonStart;
        public List<string> previousSeasonRewards;
        
        public PlayerRanking(string id, string name)
        {
            playerId = id;
            playerName = name;
            tier = RankTier.Unranked;
            division = 5;
            rankPoints = 0;
            peakRankPoints = 0;
            wins = 0;
            losses = 0;
            winRate = 0f;
            currentStreak = 0;
            bestStreak = 0;
            seasonStart = DateTime.UtcNow;
            previousSeasonRewards = new List<string>();
        }
        
        public string GetRankDisplayName()
        {
            if (tier == RankTier.Unranked) return "Unranked";
            return $"{tier} {division}";
        }
    }
    
    [Serializable]
    public class PlayerStats
    {
        public string playerId;
        
        // Combat stats
        public int totalKills;
        public int airToAirKills;
        public int groundKills;
        public int deaths;
        public int assists;
        public float damageDealt;
        public float damageTaken;
        public int bombsDropped;
        public int bombHits;
        
        // Flight stats
        public float flightHours;
        public float distanceFlown;
        public int sorties;
        public int landings;
        public int perfectLandings;
        public int crashes;
        public float highestAltitude;
        public float fastestSpeed;
        
        // Economy stats
        public long totalEarnings;
        public long totalSpent;
        public int tradesCompleted;
        public int smugglingRuns;
        public float cargoDelivered;
        
        // Mission stats
        public int missionsCompleted;
        public int missionsFailed;
        public int questsCompleted;
        public int pvpWins;
        public int pvpLosses;
        
        // Progression
        public int totalXP;
        public int level;
        public int achievementsUnlocked;
        public int aircraftOwned;
        
        // Territory
        public int territoriesCaptured;
        public int territoriesLost;
        public float controlTime;
        
        public PlayerStats(string id)
        {
            playerId = id;
        }
        
        public float GetKDRatio()
        {
            return deaths > 0 ? (float)totalKills / deaths : totalKills;
        }
        
        public float GetWinRate()
        {
            int total = pvpWins + pvpLosses;
            return total > 0 ? (float)pvpWins / total : 0f;
        }
    }
    
    [Serializable]
    public class SeasonData
    {
        public int seasonNumber;
        public string seasonName;
        public DateTime startDate;
        public DateTime endDate;
        public bool isActive;
        public Dictionary<RankTier, string> tierRewards;
        
        public SeasonData(int number, string name)
        {
            seasonNumber = number;
            seasonName = name;
            startDate = DateTime.UtcNow;
            endDate = startDate.AddMonths(3);
            isActive = true;
            tierRewards = new Dictionary<RankTier, string>
            {
                { RankTier.Bronze, "bronze_medal" },
                { RankTier.Silver, "silver_medal" },
                { RankTier.Gold, "gold_medal" },
                { RankTier.Platinum, "platinum_medal" },
                { RankTier.Diamond, "diamond_medal" },
                { RankTier.Master, "master_medal" },
                { RankTier.Grandmaster, "grandmaster_medal" },
                { RankTier.Legend, "legend_medal" }
            };
        }
    }
    
    public class LeaderboardSystem : MonoBehaviour
    {
        public static LeaderboardSystem Instance { get; private set; }
        
        [Header("Ranking Settings")]
        [SerializeField] private int basePointsPerWin = 25;
        [SerializeField] private int basePointsPerLoss = 20;
        [SerializeField] private int pointsPerTier = 400;
        [SerializeField] private int divisionsPerTier = 5;
        [SerializeField] private int pointsPerDivision = 80;
        
        [Header("Season Settings")]
        [SerializeField] private int seasonDurationDays = 90;
        
        private Dictionary<LeaderboardType, List<LeaderboardEntry>> leaderboards = 
            new Dictionary<LeaderboardType, List<LeaderboardEntry>>();
        private Dictionary<string, PlayerStats> playerStats = new Dictionary<string, PlayerStats>();
        private Dictionary<string, PlayerRanking> playerRankings = new Dictionary<string, PlayerRanking>();
        private SeasonData currentSeason;
        
        public event Action<LeaderboardType, LeaderboardEntry> OnLeaderboardUpdated;
        public event Action<string, RankTier, RankTier> OnRankChanged;
        public event Action<SeasonData> OnSeasonEnded;
        public event Action<SeasonData> OnSeasonStarted;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeLeaderboards();
                InitializeSeason();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeLeaderboards()
        {
            foreach (LeaderboardType type in Enum.GetValues(typeof(LeaderboardType)))
            {
                leaderboards[type] = new List<LeaderboardEntry>();
            }
        }
        
        private void InitializeSeason()
        {
            currentSeason = new SeasonData(1, "Season of the Ace");
        }
        
        private void Update()
        {
            // Check for season end
            if (currentSeason.isActive && DateTime.UtcNow >= currentSeason.endDate)
            {
                EndSeason();
            }
        }
        
        // Stats tracking
        
        public void RecordKill(string playerId, bool isAirToAir)
        {
            var stats = GetOrCreateStats(playerId);
            stats.totalKills++;
            if (isAirToAir)
                stats.airToAirKills++;
            else
                stats.groundKills++;
            
            UpdateLeaderboard(LeaderboardType.TotalKills, playerId, stats.totalKills);
            UpdateLeaderboard(LeaderboardType.AirToAirKills, playerId, stats.airToAirKills);
            UpdateLeaderboard(LeaderboardType.KDRatio, playerId, stats.GetKDRatio());
        }
        
        public void RecordDeath(string playerId)
        {
            var stats = GetOrCreateStats(playerId);
            stats.deaths++;
            UpdateLeaderboard(LeaderboardType.KDRatio, playerId, stats.GetKDRatio());
        }
        
        public void RecordFlightTime(string playerId, float hours, float distance)
        {
            var stats = GetOrCreateStats(playerId);
            stats.flightHours += hours;
            stats.distanceFlown += distance;
            
            UpdateLeaderboard(LeaderboardType.FlightHours, playerId, stats.flightHours);
            UpdateLeaderboard(LeaderboardType.DistanceFlown, playerId, stats.distanceFlown);
        }
        
        public void RecordLanding(string playerId, bool isPerfect)
        {
            var stats = GetOrCreateStats(playerId);
            stats.landings++;
            if (isPerfect) stats.perfectLandings++;
            
            UpdateLeaderboard(LeaderboardType.LandingsCompleted, playerId, stats.landings);
            UpdateLeaderboard(LeaderboardType.PerfectLandings, playerId, stats.perfectLandings);
        }
        
        public void RecordEarnings(string playerId, long amount)
        {
            var stats = GetOrCreateStats(playerId);
            stats.totalEarnings += amount;
            
            UpdateLeaderboard(LeaderboardType.TotalEarnings, playerId, stats.totalEarnings);
        }
        
        public void RecordTrade(string playerId)
        {
            var stats = GetOrCreateStats(playerId);
            stats.tradesCompleted++;
            
            UpdateLeaderboard(LeaderboardType.TradesCompleted, playerId, stats.tradesCompleted);
        }
        
        public void RecordSmugglingRun(string playerId)
        {
            var stats = GetOrCreateStats(playerId);
            stats.smugglingRuns++;
            
            UpdateLeaderboard(LeaderboardType.SmugglingRuns, playerId, stats.smugglingRuns);
        }
        
        public void RecordTerritoryCapture(string playerId)
        {
            var stats = GetOrCreateStats(playerId);
            stats.territoriesCaptured++;
            
            UpdateLeaderboard(LeaderboardType.TerritoryConquered, playerId, stats.territoriesCaptured);
        }
        
        public void RecordXP(string playerId, int xp, int level)
        {
            var stats = GetOrCreateStats(playerId);
            stats.totalXP += xp;
            stats.level = level;
            
            UpdateLeaderboard(LeaderboardType.TotalXP, playerId, stats.totalXP);
            UpdateLeaderboard(LeaderboardType.HighestLevel, playerId, stats.level);
        }
        
        public void RecordAchievement(string playerId)
        {
            var stats = GetOrCreateStats(playerId);
            stats.achievementsUnlocked++;
            
            UpdateLeaderboard(LeaderboardType.AchievementsUnlocked, playerId, stats.achievementsUnlocked);
        }
        
        public void RecordQuestComplete(string playerId)
        {
            var stats = GetOrCreateStats(playerId);
            stats.questsCompleted++;
            
            UpdateLeaderboard(LeaderboardType.QuestsCompleted, playerId, stats.questsCompleted);
        }
        
        // PvP Ranking
        
        public void RecordPvPResult(string playerId, string playerName, bool isWin, int enemyRankPoints = 0)
        {
            var ranking = GetOrCreateRanking(playerId, playerName);
            var stats = GetOrCreateStats(playerId);
            
            RankTier previousTier = ranking.tier;
            
            if (isWin)
            {
                stats.pvpWins++;
                ranking.wins++;
                ranking.currentStreak++;
                ranking.bestStreak = Mathf.Max(ranking.bestStreak, ranking.currentStreak);
                
                int pointsGained = CalculatePointsGained(ranking.rankPoints, enemyRankPoints, true);
                ranking.rankPoints += pointsGained;
            }
            else
            {
                stats.pvpLosses++;
                ranking.losses++;
                ranking.currentStreak = 0;
                
                int pointsLost = CalculatePointsGained(ranking.rankPoints, enemyRankPoints, false);
                ranking.rankPoints = Mathf.Max(0, ranking.rankPoints - pointsLost);
            }
            
            ranking.winRate = ranking.wins + ranking.losses > 0 
                ? (float)ranking.wins / (ranking.wins + ranking.losses) 
                : 0f;
            ranking.peakRankPoints = Mathf.Max(ranking.peakRankPoints, ranking.rankPoints);
            
            UpdateRankTier(ranking);
            
            if (ranking.tier != previousTier)
            {
                OnRankChanged?.Invoke(playerId, previousTier, ranking.tier);
            }
            
            UpdateLeaderboard(LeaderboardType.PvPWins, playerId, ranking.wins);
            UpdateLeaderboard(LeaderboardType.SeasonalRank, playerId, ranking.rankPoints);
        }
        
        private int CalculatePointsGained(int playerPoints, int enemyPoints, bool isWin)
        {
            // ELO-like calculation
            float expectedWin = 1f / (1f + Mathf.Pow(10f, (enemyPoints - playerPoints) / 400f));
            
            if (isWin)
            {
                return Mathf.RoundToInt(basePointsPerWin * (1f + (1f - expectedWin)));
            }
            else
            {
                return Mathf.RoundToInt(basePointsPerLoss * expectedWin);
            }
        }
        
        private void UpdateRankTier(PlayerRanking ranking)
        {
            int points = ranking.rankPoints;
            
            // Calculate tier and division from points
            int tierIndex = points / pointsPerTier;
            tierIndex = Mathf.Clamp(tierIndex, 0, (int)RankTier.Legend - 1);
            
            ranking.tier = (RankTier)(tierIndex + 1); // +1 because Unranked is 0
            
            if (ranking.tier == RankTier.Legend)
            {
                ranking.division = 1; // Legend has no divisions
            }
            else
            {
                int pointsInTier = points % pointsPerTier;
                ranking.division = divisionsPerTier - (pointsInTier / pointsPerDivision);
                ranking.division = Mathf.Clamp(ranking.division, 1, divisionsPerTier);
            }
            
            // Promotion matches after 10 games
            if (ranking.wins + ranking.losses < 10)
            {
                ranking.tier = RankTier.Unranked;
            }
        }
        
        // Leaderboard Management
        
        private void UpdateLeaderboard(LeaderboardType type, string playerId, float score)
        {
            var board = leaderboards[type];
            
            var existingEntry = board.Find(e => e.playerId == playerId);
            if (existingEntry != null)
            {
                existingEntry.score = score;
                existingEntry.lastUpdated = DateTime.UtcNow;
            }
            else
            {
                var entry = new LeaderboardEntry(playerId, GetPlayerName(playerId), score);
                board.Add(entry);
            }
            
            // Sort and update ranks
            board.Sort((a, b) => b.score.CompareTo(a.score));
            for (int i = 0; i < board.Count; i++)
            {
                board[i].rank = i + 1;
            }
            
            var updatedEntry = board.Find(e => e.playerId == playerId);
            if (updatedEntry != null)
            {
                OnLeaderboardUpdated?.Invoke(type, updatedEntry);
            }
        }
        
        public List<LeaderboardEntry> GetLeaderboard(LeaderboardType type, int topN = 100)
        {
            if (!leaderboards.ContainsKey(type)) return new List<LeaderboardEntry>();
            
            return leaderboards[type].Take(topN).ToList();
        }
        
        public int GetPlayerRank(LeaderboardType type, string playerId)
        {
            if (!leaderboards.ContainsKey(type)) return -1;
            
            var entry = leaderboards[type].Find(e => e.playerId == playerId);
            return entry?.rank ?? -1;
        }
        
        public LeaderboardEntry GetPlayerEntry(LeaderboardType type, string playerId)
        {
            if (!leaderboards.ContainsKey(type)) return null;
            return leaderboards[type].Find(e => e.playerId == playerId);
        }
        
        public List<LeaderboardEntry> GetNearbyEntries(LeaderboardType type, string playerId, int range = 5)
        {
            if (!leaderboards.ContainsKey(type)) return new List<LeaderboardEntry>();
            
            var board = leaderboards[type];
            int playerIndex = board.FindIndex(e => e.playerId == playerId);
            
            if (playerIndex < 0) return new List<LeaderboardEntry>();
            
            int start = Mathf.Max(0, playerIndex - range);
            int end = Mathf.Min(board.Count, playerIndex + range + 1);
            
            return board.GetRange(start, end - start);
        }
        
        // Season Management
        
        private void EndSeason()
        {
            currentSeason.isActive = false;
            OnSeasonEnded?.Invoke(currentSeason);
            
            // Distribute rewards
            foreach (var kvp in playerRankings)
            {
                var ranking = kvp.Value;
                if (ranking.tier != RankTier.Unranked && currentSeason.tierRewards.ContainsKey(ranking.tier))
                {
                    ranking.previousSeasonRewards.Add(currentSeason.tierRewards[ranking.tier]);
                }
                
                // Soft reset ranks
                ranking.rankPoints = ranking.rankPoints / 2;
                ranking.wins = 0;
                ranking.losses = 0;
                ranking.currentStreak = 0;
                UpdateRankTier(ranking);
            }
            
            // Clear weekly leaderboards
            leaderboards[LeaderboardType.WeeklyKills].Clear();
            leaderboards[LeaderboardType.WeeklyEarnings].Clear();
            
            StartNewSeason();
        }
        
        private void StartNewSeason()
        {
            int newSeasonNumber = currentSeason.seasonNumber + 1;
            string[] seasonNames = { "Season of the Ace", "Season of Fire", "Season of Steel", "Season of Thunder" };
            string name = seasonNames[(newSeasonNumber - 1) % seasonNames.Length];
            
            currentSeason = new SeasonData(newSeasonNumber, name);
            OnSeasonStarted?.Invoke(currentSeason);
        }
        
        public SeasonData GetCurrentSeason()
        {
            return currentSeason;
        }
        
        public TimeSpan GetTimeUntilSeasonEnd()
        {
            return currentSeason.endDate - DateTime.UtcNow;
        }
        
        // Helpers
        
        private PlayerStats GetOrCreateStats(string playerId)
        {
            if (!playerStats.ContainsKey(playerId))
            {
                playerStats[playerId] = new PlayerStats(playerId);
            }
            return playerStats[playerId];
        }
        
        private PlayerRanking GetOrCreateRanking(string playerId, string playerName)
        {
            if (!playerRankings.ContainsKey(playerId))
            {
                playerRankings[playerId] = new PlayerRanking(playerId, playerName);
            }
            return playerRankings[playerId];
        }
        
        private string GetPlayerName(string playerId)
        {
            if (playerRankings.ContainsKey(playerId))
            {
                return playerRankings[playerId].playerName;
            }
            return playerId;
        }
        
        public PlayerStats GetPlayerStats(string playerId)
        {
            return playerStats.ContainsKey(playerId) ? playerStats[playerId] : null;
        }
        
        public PlayerRanking GetPlayerRanking(string playerId)
        {
            return playerRankings.ContainsKey(playerId) ? playerRankings[playerId] : null;
        }
        
        // Weekly reset
        
        public void ResetWeeklyLeaderboards()
        {
            leaderboards[LeaderboardType.WeeklyKills].Clear();
            leaderboards[LeaderboardType.WeeklyEarnings].Clear();
        }
        
        // Faction leaderboards
        
        public List<KeyValuePair<string, float>> GetFactionLeaderboard()
        {
            Dictionary<string, float> factionScores = new Dictionary<string, float>();
            
            foreach (var entry in leaderboards[LeaderboardType.TerritoryConquered])
            {
                if (!string.IsNullOrEmpty(entry.factionId))
                {
                    if (!factionScores.ContainsKey(entry.factionId))
                        factionScores[entry.factionId] = 0;
                    factionScores[entry.factionId] += entry.score;
                }
            }
            
            return factionScores.OrderByDescending(kvp => kvp.Value).ToList();
        }
    }
}
