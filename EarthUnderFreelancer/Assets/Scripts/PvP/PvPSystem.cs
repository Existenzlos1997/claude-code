using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EarthUnderFreelancer.PvP
{
    /// <summary>
    /// Complete PvP System with Arenas, Battlegrounds, and Rankings
    /// </summary>
    public class PvPSystem : MonoBehaviour
    {
        public static PvPSystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int arenaSeasonLength = 90; // days
        [SerializeField] private int ratingResetValue = 1500;
        [SerializeField] private float matchmakingRatingRange = 200f;

        public PvPStats PlayerStats { get; private set; }
        public MatchData CurrentMatch { get; private set; }
        public bool IsInMatch => CurrentMatch != null;
        public bool IsInQueue { get; private set; }

        public event Action<MatchData> OnMatchFound;
        public event Action<MatchResult> OnMatchEnded;
        public event Action<int> OnRatingChanged;
        public event Action<PvPRank> OnRankChanged;
        public event Action<string> OnQueueUpdate;

        #region Data Structures

        [Serializable]
        public class PvPStats
        {
            public int arenaRating;
            public int highestArenaRating;
            public PvPRank currentRank;
            public int wins;
            public int losses;
            public int draws;
            public int killsTotal;
            public int deathsTotal;
            public int assistsTotal;
            public float kdRatio;
            public int winStreak;
            public int bestWinStreak;
            public int honorPoints;
            public int conquestPoints;
            public int seasonWins;
            public List<string> unlockedTitles;
            public List<string> earnedRewards;
        }

        public enum PvPRank
        {
            Unranked,
            Bronze_I,
            Bronze_II,
            Bronze_III,
            Silver_I,
            Silver_II,
            Silver_III,
            Gold_I,
            Gold_II,
            Gold_III,
            Platinum_I,
            Platinum_II,
            Platinum_III,
            Diamond_I,
            Diamond_II,
            Diamond_III,
            Master,
            Grandmaster,
            Legend
        }

        [Serializable]
        public class MatchData
        {
            public string matchId;
            public PvPMode mode;
            public ArenaType arenaType;
            public BattlegroundType battlegroundType;
            public List<MatchPlayer> team1;
            public List<MatchPlayer> team2;
            public int team1Score;
            public int team2Score;
            public int playerTeam;
            public DateTime startTime;
            public float maxDuration;
            public MatchState state;
            public MapData map;
        }

        [Serializable]
        public class MatchPlayer
        {
            public string playerId;
            public string playerName;
            public int rating;
            public string aircraftType;
            public int kills;
            public int deaths;
            public int assists;
            public int objectiveScore;
            public float damageDealt;
            public float damageTaken;
            public bool isAlive;
        }

        [Serializable]
        public class MatchResult
        {
            public string matchId;
            public bool isVictory;
            public bool isDraw;
            public int ratingChange;
            public int honorGained;
            public int conquestGained;
            public List<string> rewards;
            public MatchStats stats;
        }

        [Serializable]
        public class MatchStats
        {
            public int kills;
            public int deaths;
            public int assists;
            public float damageDealt;
            public float damageTaken;
            public int objectiveScore;
            public float matchDuration;
        }

        [Serializable]
        public class MapData
        {
            public string mapId;
            public string mapName;
            public Vector3 team1Spawn;
            public Vector3 team2Spawn;
            public List<ObjectivePoint> objectives;
        }

        [Serializable]
        public class ObjectivePoint
        {
            public string objectiveId;
            public ObjectiveType type;
            public Vector3 position;
            public int controllingTeam;
        }

        public enum ObjectiveType
        {
            ControlPoint,
            Flag,
            Bomb,
            Payload,
            KillZone
        }

        public enum PvPMode
        {
            Arena1v1,
            Arena2v2,
            Arena3v3,
            Arena5v5,
            Battleground,
            OpenWorld,
            Duel,
            WarGame
        }

        public enum ArenaType
        {
            Skirmish,
            Ranked,
            Tournament
        }

        public enum BattlegroundType
        {
            TeamDeathmatch,
            Domination,
            CaptureTheFlag,
            Assault,
            Payload,
            FreeForAll
        }

        public enum MatchState
        {
            Preparing,
            InProgress,
            Ending,
            Completed
        }

        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeStats();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeStats()
        {
            PlayerStats = new PvPStats
            {
                arenaRating = ratingResetValue,
                highestArenaRating = ratingResetValue,
                currentRank = PvPRank.Unranked,
                unlockedTitles = new List<string>(),
                earnedRewards = new List<string>()
            };
        }

        #region Queue System

        public async Task<bool> JoinQueue(PvPMode mode, ArenaType arenaType = ArenaType.Ranked)
        {
            if (IsInQueue || IsInMatch)
            {
                Debug.LogError("[PvP] Already in queue or match!");
                return false;
            }

            IsInQueue = true;
            OnQueueUpdate?.Invoke($"Searching for {mode} match...");

            // Simulate matchmaking
            int waitTime = UnityEngine.Random.Range(5, 30);
            for (int i = 0; i < waitTime; i++)
            {
                await Task.Delay(1000);
                if (!IsInQueue) return false; // Cancelled
                OnQueueUpdate?.Invoke($"In queue: {i + 1}s");
            }

            // Match found!
            IsInQueue = false;
            CurrentMatch = CreateMatch(mode, arenaType);
            OnMatchFound?.Invoke(CurrentMatch);
            
            Debug.Log($"[PvP] Match found! Mode: {mode}");
            return true;
        }

        public void LeaveQueue()
        {
            if (IsInQueue)
            {
                IsInQueue = false;
                OnQueueUpdate?.Invoke("Left queue");
                Debug.Log("[PvP] Left queue");
            }
        }

        public async Task<bool> JoinBattleground(BattlegroundType bgType)
        {
            return await JoinQueue(PvPMode.Battleground, ArenaType.Skirmish);
        }

        private MatchData CreateMatch(PvPMode mode, ArenaType arenaType)
        {
            int teamSize = GetTeamSize(mode);

            var match = new MatchData
            {
                matchId = Guid.NewGuid().ToString(),
                mode = mode,
                arenaType = arenaType,
                team1 = new List<MatchPlayer>(),
                team2 = new List<MatchPlayer>(),
                team1Score = 0,
                team2Score = 0,
                playerTeam = 1,
                startTime = DateTime.UtcNow,
                maxDuration = GetMatchDuration(mode),
                state = MatchState.Preparing,
                map = GetRandomMap(mode)
            };

            // Add player to team 1
            match.team1.Add(new MatchPlayer
            {
                playerId = "local_player",
                playerName = "Player",
                rating = PlayerStats.arenaRating,
                isAlive = true
            });

            // Fill with AI/simulated players
            for (int i = 1; i < teamSize; i++)
            {
                match.team1.Add(CreateAIPlayer(PlayerStats.arenaRating));
            }
            for (int i = 0; i < teamSize; i++)
            {
                match.team2.Add(CreateAIPlayer(PlayerStats.arenaRating));
            }

            return match;
        }

        private MatchPlayer CreateAIPlayer(int baseRating)
        {
            return new MatchPlayer
            {
                playerId = Guid.NewGuid().ToString(),
                playerName = GetRandomPlayerName(),
                rating = baseRating + UnityEngine.Random.Range(-100, 100),
                isAlive = true
            };
        }

        private string GetRandomPlayerName()
        {
            string[] prefixes = { "Ace", "Sky", "Storm", "Iron", "Steel", "Dark", "Red", "Blue" };
            string[] suffixes = { "Hawk", "Eagle", "Wolf", "Bear", "Dragon", "Phoenix", "Knight", "Pilot" };
            return $"{prefixes[UnityEngine.Random.Range(0, prefixes.Length)]}{suffixes[UnityEngine.Random.Range(0, suffixes.Length)]}{UnityEngine.Random.Range(1, 999)}";
        }

        private int GetTeamSize(PvPMode mode)
        {
            return mode switch
            {
                PvPMode.Arena1v1 => 1,
                PvPMode.Arena2v2 => 2,
                PvPMode.Arena3v3 => 3,
                PvPMode.Arena5v5 => 5,
                PvPMode.Battleground => 15,
                PvPMode.Duel => 1,
                _ => 5
            };
        }

        private float GetMatchDuration(PvPMode mode)
        {
            return mode switch
            {
                PvPMode.Arena1v1 => 300f,  // 5 min
                PvPMode.Arena2v2 => 420f,  // 7 min
                PvPMode.Arena3v3 => 600f,  // 10 min
                PvPMode.Arena5v5 => 900f,  // 15 min
                PvPMode.Battleground => 1200f, // 20 min
                PvPMode.Duel => 180f, // 3 min
                _ => 600f
            };
        }

        private MapData GetRandomMap(PvPMode mode)
        {
            return new MapData
            {
                mapId = "arena_01",
                mapName = "The Crucible",
                team1Spawn = new Vector3(-500, 0, 0),
                team2Spawn = new Vector3(500, 0, 0)
            };
        }

        #endregion

        #region Match Events

        public void StartMatch()
        {
            if (!IsInMatch) return;
            CurrentMatch.state = MatchState.InProgress;
            Debug.Log("[PvP] Match started!");
        }

        public void RegisterKill(string killerId, string victimId)
        {
            if (!IsInMatch) return;

            var killer = FindPlayer(killerId);
            var victim = FindPlayer(victimId);

            if (killer != null) killer.kills++;
            if (victim != null)
            {
                victim.deaths++;
                victim.isAlive = false;
            }

            // Update team score
            if (killer != null && CurrentMatch.mode != PvPMode.Battleground)
            {
                if (CurrentMatch.team1.Contains(killer)) CurrentMatch.team1Score++;
                else CurrentMatch.team2Score++;
            }

            CheckMatchEnd();
        }

        public void RegisterAssist(string assisterId)
        {
            if (!IsInMatch) return;
            var player = FindPlayer(assisterId);
            if (player != null) player.assists++;
        }

        public void RegisterDamage(string dealerId, float damage)
        {
            if (!IsInMatch) return;
            var player = FindPlayer(dealerId);
            if (player != null) player.damageDealt += damage;
        }

        public void RegisterObjectiveScore(string playerId, int points)
        {
            if (!IsInMatch) return;
            var player = FindPlayer(playerId);
            if (player != null) player.objectiveScore += points;
        }

        public void Respawn(string playerId)
        {
            if (!IsInMatch) return;
            var player = FindPlayer(playerId);
            if (player != null) player.isAlive = true;
        }

        private MatchPlayer FindPlayer(string playerId)
        {
            var player = CurrentMatch.team1.Find(p => p.playerId == playerId);
            if (player != null) return player;
            return CurrentMatch.team2.Find(p => p.playerId == playerId);
        }

        private void CheckMatchEnd()
        {
            bool team1Eliminated = CurrentMatch.team1.TrueForAll(p => !p.isAlive);
            bool team2Eliminated = CurrentMatch.team2.TrueForAll(p => !p.isAlive);

            if (team1Eliminated || team2Eliminated)
            {
                EndMatch();
            }
        }

        public void EndMatch()
        {
            if (!IsInMatch) return;

            CurrentMatch.state = MatchState.Completed;

            bool isVictory = CurrentMatch.team1Score > CurrentMatch.team2Score;
            bool isDraw = CurrentMatch.team1Score == CurrentMatch.team2Score;

            // Calculate rating change
            int ratingChange = CalculateRatingChange(isVictory, isDraw);

            // Update stats
            PlayerStats.arenaRating += ratingChange;
            if (isVictory)
            {
                PlayerStats.wins++;
                PlayerStats.seasonWins++;
                PlayerStats.winStreak++;
                if (PlayerStats.winStreak > PlayerStats.bestWinStreak)
                {
                    PlayerStats.bestWinStreak = PlayerStats.winStreak;
                }
            }
            else if (!isDraw)
            {
                PlayerStats.losses++;
                PlayerStats.winStreak = 0;
            }
            else
            {
                PlayerStats.draws++;
            }

            if (PlayerStats.arenaRating > PlayerStats.highestArenaRating)
            {
                PlayerStats.highestArenaRating = PlayerStats.arenaRating;
            }

            // Update rank
            UpdateRank();

            // Get player stats
            var localPlayer = CurrentMatch.team1.Find(p => p.playerId == "local_player") ??
                              CurrentMatch.team2.Find(p => p.playerId == "local_player");

            var result = new MatchResult
            {
                matchId = CurrentMatch.matchId,
                isVictory = isVictory,
                isDraw = isDraw,
                ratingChange = ratingChange,
                honorGained = CalculateHonor(isVictory, localPlayer),
                conquestGained = CalculateConquest(isVictory, CurrentMatch.arenaType),
                stats = new MatchStats
                {
                    kills = localPlayer?.kills ?? 0,
                    deaths = localPlayer?.deaths ?? 0,
                    assists = localPlayer?.assists ?? 0,
                    damageDealt = localPlayer?.damageDealt ?? 0,
                    matchDuration = (float)(DateTime.UtcNow - CurrentMatch.startTime).TotalSeconds
                },
                rewards = new List<string>()
            };

            PlayerStats.honorPoints += result.honorGained;
            PlayerStats.conquestPoints += result.conquestGained;

            OnMatchEnded?.Invoke(result);
            OnRatingChanged?.Invoke(ratingChange);

            CurrentMatch = null;

            Debug.Log($"[PvP] Match ended! {(isVictory ? "Victory" : isDraw ? "Draw" : "Defeat")} Rating: {ratingChange:+#;-#;0}");
        }

        private int CalculateRatingChange(bool victory, bool draw)
        {
            if (draw) return 0;

            int baseChange = victory ? 25 : -20;

            // Modify based on opponent rating difference
            // ... (simplified)

            return baseChange;
        }

        private int CalculateHonor(bool victory, MatchPlayer player)
        {
            int baseHonor = victory ? 200 : 50;
            int killBonus = (player?.kills ?? 0) * 10;
            int objectiveBonus = (player?.objectiveScore ?? 0) * 5;
            return baseHonor + killBonus + objectiveBonus;
        }

        private int CalculateConquest(bool victory, ArenaType type)
        {
            if (type != ArenaType.Ranked) return 0;
            return victory ? 180 : 0;
        }

        private void UpdateRank()
        {
            PvPRank oldRank = PlayerStats.currentRank;
            PlayerStats.currentRank = GetRankForRating(PlayerStats.arenaRating);

            if (PlayerStats.currentRank != oldRank)
            {
                OnRankChanged?.Invoke(PlayerStats.currentRank);
            }
        }

        private PvPRank GetRankForRating(int rating)
        {
            if (rating < 1000) return PvPRank.Bronze_I;
            if (rating < 1100) return PvPRank.Bronze_II;
            if (rating < 1200) return PvPRank.Bronze_III;
            if (rating < 1300) return PvPRank.Silver_I;
            if (rating < 1400) return PvPRank.Silver_II;
            if (rating < 1500) return PvPRank.Silver_III;
            if (rating < 1600) return PvPRank.Gold_I;
            if (rating < 1700) return PvPRank.Gold_II;
            if (rating < 1800) return PvPRank.Gold_III;
            if (rating < 1900) return PvPRank.Platinum_I;
            if (rating < 2000) return PvPRank.Platinum_II;
            if (rating < 2100) return PvPRank.Platinum_III;
            if (rating < 2200) return PvPRank.Diamond_I;
            if (rating < 2300) return PvPRank.Diamond_II;
            if (rating < 2400) return PvPRank.Diamond_III;
            if (rating < 2700) return PvPRank.Master;
            if (rating < 3000) return PvPRank.Grandmaster;
            return PvPRank.Legend;
        }

        #endregion

        #region Dueling

        public bool RequestDuel(string targetPlayerId)
        {
            if (IsInMatch || IsInQueue)
            {
                Debug.LogError("[PvP] Cannot duel while in match or queue!");
                return false;
            }

            Debug.Log($"[PvP] Duel request sent to {targetPlayerId}");
            return true;
        }

        public bool AcceptDuel(string challengerId)
        {
            // Would start duel match
            return true;
        }

        public bool DeclineDuel(string challengerId)
        {
            Debug.Log($"[PvP] Declined duel from {challengerId}");
            return true;
        }

        #endregion

        #region Leaderboards

        public async Task<List<LeaderboardEntry>> GetLeaderboard(LeaderboardType type, int count = 100)
        {
            await Task.Delay(300);

            var entries = new List<LeaderboardEntry>();
            for (int i = 0; i < count; i++)
            {
                entries.Add(new LeaderboardEntry
                {
                    rank = i + 1,
                    playerId = $"player_{i}",
                    playerName = GetRandomPlayerName(),
                    rating = 3000 - (i * 15),
                    wins = 500 - (i * 5),
                    losses = 100 + (i * 2)
                });
            }

            return entries;
        }

        [Serializable]
        public class LeaderboardEntry
        {
            public int rank;
            public string playerId;
            public string playerName;
            public string guildName;
            public int rating;
            public int wins;
            public int losses;
        }

        public enum LeaderboardType
        {
            Arena2v2,
            Arena3v3,
            Arena5v5,
            Battleground,
            Overall
        }

        #endregion
    }
}
