using UnityEngine;

namespace EarthUnderFreelancer.Systems
{
    // Iteration 100: Master game state manager
    public class GameStateManager : MonoBehaviour
    {
        public enum GameState { MainMenu, Loading, InGame, Paused, GameOver }
        
        private GameState currentState = GameState.MainMenu;
        
        public void SetState(GameState newState)
        {
            currentState = newState;
            Debug.Log($"[GameState] Changed to: {newState}");
            
            switch (newState)
            {
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.InGame:
                    Time.timeScale = 1f;
                    break;
            }
        }
        
        public GameState GetState() => currentState;
    }
}
