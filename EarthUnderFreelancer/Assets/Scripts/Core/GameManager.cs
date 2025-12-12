using UnityEngine;
using System;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Main game manager singleton that controls overall game state and flow
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState
        {
            MainMenu,
            Lobby,
            Loading,
            Playing,
            Paused,
            GameOver,
            Victory
        }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        public GameState CurrentState => currentState;

        [Header("Player Data")]
        public PlayerData playerData;

        [Header("Game Settings")]
        public GameSettings gameSettings;

        public event Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeGame()
        {
            playerData = SaveManager.LoadPlayerData();
            if (playerData == null)
            {
                playerData = new PlayerData();
            }

            gameSettings = SaveManager.LoadGameSettings();
            if (gameSettings == null)
            {
                gameSettings = new GameSettings();
            }

            Application.targetFrameRate = 60;
        }

        public void SetGameState(GameState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                OnGameStateChanged?.Invoke(newState);
                HandleStateChange(newState);
            }
        }

        private void HandleStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;
            }
        }

        public void SaveGame()
        {
            SaveManager.SavePlayerData(playerData);
            SaveManager.SaveGameSettings(gameSettings);
        }

        public void LoadGame()
        {
            playerData = SaveManager.LoadPlayerData();
            gameSettings = SaveManager.LoadGameSettings();
        }

        public void ResetProgress()
        {
            playerData = new PlayerData();
            SaveManager.SavePlayerData(playerData);
        }

        public void QuitGame()
        {
            SaveGame();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}
