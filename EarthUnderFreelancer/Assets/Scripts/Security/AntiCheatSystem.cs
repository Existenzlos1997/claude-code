using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Security
{
    /// <summary>
    /// Anti-cheat system for detecting and preventing common cheats
    /// Implements server authority and client validation
    /// </summary>
    public class AntiCheatSystem : MonoBehaviour
    {
        public static AntiCheatSystem Instance { get; private set; }

        [Header("Detection Settings")]
        [SerializeField] private bool enableSpeedhackDetection = true;
        [SerializeField] private bool enablePositionValidation = true;
        [SerializeField] private bool enableHealthValidation = true;
        [SerializeField] private float maxAllowedSpeed = 500f; // Max aircraft speed
        [SerializeField] private float maxPositionDelta = 100f; // Max position change per second

        [Header("Report Settings")]
        [SerializeField] private int suspicionThreshold = 5;
        [SerializeField] private bool autoKickOnDetection = false;

        private Dictionary<string, PlayerSecurityData> playerData = new Dictionary<string, PlayerSecurityData>();
        private List<CheatReport> cheatReports = new List<CheatReport>();

        public System.Action<string, CheatType> OnCheatDetected;

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

        public void RegisterPlayer(string playerId)
        {
            if (!playerData.ContainsKey(playerId))
            {
                playerData[playerId] = new PlayerSecurityData(playerId);
            }
        }

        public void UnregisterPlayer(string playerId)
        {
            playerData.Remove(playerId);
        }

        public void ValidatePosition(string playerId, Vector3 position, float deltaTime)
        {
            if (!enablePositionValidation) return;
            if (!playerData.ContainsKey(playerId)) return;

            var data = playerData[playerId];
            
            if (data.LastPosition != Vector3.zero)
            {
                float distance = Vector3.Distance(data.LastPosition, position);
                float speed = distance / deltaTime;

                if (speed > maxPositionDelta)
                {
                    ReportCheat(playerId, CheatType.Teleport, 
                        $"Suspicious position change: {distance:F2}m in {deltaTime:F2}s ({speed:F2}m/s)");
                }
            }

            data.LastPosition = position;
            data.LastUpdateTime = Time.time;
        }

        public void ValidateSpeed(string playerId, float speed)
        {
            if (!enableSpeedhackDetection) return;
            if (!playerData.ContainsKey(playerId)) return;

            if (speed > maxAllowedSpeed)
            {
                ReportCheat(playerId, CheatType.Speedhack, 
                    $"Speed exceeds maximum: {speed:F2} > {maxAllowedSpeed:F2}");
            }
        }

        public bool ValidateHealth(string playerId, float oldHealth, float newHealth, float damage)
        {
            if (!enableHealthValidation) return true;
            if (!playerData.ContainsKey(playerId)) return true;

            float expectedHealth = Mathf.Max(0, oldHealth - damage);
            float difference = Mathf.Abs(newHealth - expectedHealth);

            // Allow small floating point errors
            if (difference > 0.1f)
            {
                // Health doesn't match expected value
                if (newHealth > oldHealth)
                {
                    ReportCheat(playerId, CheatType.HealthHack, 
                        $"Health increased without healing: {oldHealth:F1} -> {newHealth:F1}");
                    return false;
                }
                else if (newHealth > expectedHealth)
                {
                    ReportCheat(playerId, CheatType.HealthHack, 
                        $"Damage reduced: Expected {expectedHealth:F1}, got {newHealth:F1}");
                    return false;
                }
            }

            return true;
        }

        public bool ValidateInput(string playerId, float inputValue, float minValue, float maxValue)
        {
            if (inputValue < minValue || inputValue > maxValue)
            {
                ReportCheat(playerId, CheatType.InvalidInput, 
                    $"Input out of range: {inputValue} (valid: {minValue}-{maxValue})");
                return false;
            }
            return true;
        }

        public void ReportCheat(string playerId, CheatType cheatType, string details)
        {
            if (!playerData.ContainsKey(playerId)) return;

            var data = playerData[playerId];
            data.SuspicionLevel++;
            data.LastViolationTime = Time.time;

            CheatReport report = new CheatReport
            {
                PlayerId = playerId,
                CheatType = cheatType,
                Details = details,
                Timestamp = System.DateTime.Now,
                SuspicionLevel = data.SuspicionLevel
            };

            cheatReports.Add(report);
            OnCheatDetected?.Invoke(playerId, cheatType);

            Debug.LogWarning($"[Anti-Cheat] Player {playerId}: {cheatType} - {details}");

            // Auto-kick if threshold exceeded
            if (autoKickOnDetection && data.SuspicionLevel >= suspicionThreshold)
            {
                KickPlayer(playerId);
            }
        }

        private void KickPlayer(string playerId)
        {
            Debug.LogWarning($"[Anti-Cheat] Kicking player {playerId} for suspicious activity");
            // In a real implementation, this would disconnect the player
            UnregisterPlayer(playerId);
        }

        public int GetPlayerSuspicionLevel(string playerId)
        {
            return playerData.ContainsKey(playerId) ? playerData[playerId].SuspicionLevel : 0;
        }

        public List<CheatReport> GetRecentReports(int count = 10)
        {
            int startIndex = Mathf.Max(0, cheatReports.Count - count);
            return cheatReports.GetRange(startIndex, Mathf.Min(count, cheatReports.Count));
        }

        public void ClearPlayerViolations(string playerId)
        {
            if (playerData.ContainsKey(playerId))
            {
                playerData[playerId].SuspicionLevel = 0;
            }
        }

        public string GetSecurityReport()
        {
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            report.AppendLine("=== Anti-Cheat Security Report ===");
            report.AppendLine($"Monitored Players: {playerData.Count}");
            report.AppendLine($"Total Reports: {cheatReports.Count}");

            var suspiciousPlayers = new List<PlayerSecurityData>();
            foreach (var data in playerData.Values)
            {
                if (data.SuspicionLevel > 0)
                {
                    suspiciousPlayers.Add(data);
                }
            }

            if (suspiciousPlayers.Count > 0)
            {
                report.AppendLine($"\nSuspicious Players: {suspiciousPlayers.Count}");
                foreach (var player in suspiciousPlayers)
                {
                    report.AppendLine($"- {player.PlayerId}: Level {player.SuspicionLevel}");
                }
            }

            if (cheatReports.Count > 0)
            {
                report.AppendLine("\nRecent Reports:");
                var recentReports = GetRecentReports(5);
                foreach (var r in recentReports)
                {
                    report.AppendLine($"- {r.PlayerId} [{r.CheatType}]: {r.Details}");
                }
            }

            return report.ToString();
        }
    }

    public enum CheatType
    {
        Speedhack,
        Teleport,
        HealthHack,
        InvalidInput,
        Wallhack,
        Aimbot,
        Other
    }

    public class PlayerSecurityData
    {
        public string PlayerId { get; private set; }
        public int SuspicionLevel { get; set; }
        public Vector3 LastPosition { get; set; }
        public float LastUpdateTime { get; set; }
        public float LastViolationTime { get; set; }

        public PlayerSecurityData(string playerId)
        {
            PlayerId = playerId;
            SuspicionLevel = 0;
            LastPosition = Vector3.zero;
            LastUpdateTime = 0f;
            LastViolationTime = 0f;
        }
    }

    public class CheatReport
    {
        public string PlayerId { get; set; }
        public CheatType CheatType { get; set; }
        public string Details { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int SuspicionLevel { get; set; }
    }
}
