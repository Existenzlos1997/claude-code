using UnityEngine;
using System.Collections.Generic;
using EarthUnderFreelancer.Core;

namespace EarthUnderFreelancer.Networking
{
    /// <summary>
    /// Manages lobby functionality
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        [Header("Lobby Settings")]
        [SerializeField] private int minPlayersToStart = 2;
        [SerializeField] private float countdownTime = 10f;
        [SerializeField] private bool autoStart = true;

        [Header("State")]
        [SerializeField] private bool isCountingDown = false;
        [SerializeField] private float currentCountdown = 0f;

        public bool IsCountingDown => isCountingDown;
        public float CurrentCountdown => currentCountdown;

        public event System.Action<float> OnCountdownTick;
        public event System.Action OnCountdownStarted;
        public event System.Action OnCountdownCancelled;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnPlayerJoined += OnPlayerJoined;
                NetworkManager.Instance.OnPlayerLeft += OnPlayerLeft;
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnPlayerJoined -= OnPlayerJoined;
                NetworkManager.Instance.OnPlayerLeft -= OnPlayerLeft;
            }
        }

        private void Update()
        {
            if (isCountingDown)
            {
                currentCountdown -= Time.deltaTime;
                OnCountdownTick?.Invoke(currentCountdown);

                if (currentCountdown <= 0)
                {
                    isCountingDown = false;
                    StartMatch();
                }
            }
            else if (autoStart && CanStartMatch())
            {
                StartCountdown();
            }
        }

        private void OnPlayerJoined(NetworkPlayer player)
        {
            if (autoStart && CanStartMatch()) StartCountdown();
        }

        private void OnPlayerLeft(NetworkPlayer player)
        {
            if (isCountingDown && !CanStartMatch()) CancelCountdown();
        }

        public bool CanStartMatch()
        {
            if (NetworkManager.Instance == null || !NetworkManager.Instance.IsInRoom) return false;
            if (NetworkManager.Instance.PlayerCount < minPlayersToStart) return false;

            List<NetworkPlayer> players = NetworkManager.Instance.GetAllPlayers();
            foreach (var player in players) if (!player.isReady) return false;
            return true;
        }

        public void StartCountdown()
        {
            if (isCountingDown || !CanStartMatch()) return;
            isCountingDown = true;
            currentCountdown = countdownTime;
            OnCountdownStarted?.Invoke();
        }

        public void CancelCountdown()
        {
            if (!isCountingDown) return;
            isCountingDown = false;
            currentCountdown = 0f;
            OnCountdownCancelled?.Invoke();
        }

        private void StartMatch() => NetworkManager.Instance?.StartMatch();
        public void SetPlayerReady(bool ready) => NetworkManager.Instance?.SetReady(ready);
        public void SelectTeam(int team) => NetworkManager.Instance?.SetTeam(team);

        public void SelectVehicle(string vehicleId)
        {
            if (NetworkManager.Instance?.LocalPlayer != null)
                NetworkManager.Instance.LocalPlayer.vehicleId = vehicleId;
        }

        public void LeaveLobby()
        {
            CancelCountdown();
            NetworkManager.Instance?.LeaveRoom();
            SceneLoader.Instance?.LoadMainMenu();
        }

        public void KickPlayer(int playerId)
        {
            if (NetworkManager.Instance?.IsHost != true) return;
            Debug.Log($"Kicking player: {playerId}");
        }

        public void ChangeMap(string mapName)
        {
            if (NetworkManager.Instance?.IsHost != true) return;
            Debug.Log($"Changing map to: {mapName}");
        }

        public void ChangeGameMode(GameMode mode)
        {
            if (NetworkManager.Instance?.IsHost != true) return;
            Debug.Log($"Changing game mode to: {mode}");
        }
    }
}
