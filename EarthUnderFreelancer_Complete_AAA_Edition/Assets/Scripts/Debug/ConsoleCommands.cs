using UnityEngine;

namespace EarthUnderFreelancer.Debug
{
    // Iteration 73-75: Debug console with commands
    public class ConsoleCommands : MonoBehaviour
    {
        private bool consoleOpen = false;
        private string command = "";
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                consoleOpen = !consoleOpen;
            }
        }
        
        private void OnGUI()
        {
            if (!consoleOpen) return;
            
            command = GUI.TextField(new Rect(10, 10, 300, 25), command);
            
            if (GUI.Button(new Rect(320, 10, 80, 25), "Execute") || Input.GetKeyDown(KeyCode.Return))
            {
                ExecuteCommand(command);
                command = "";
            }
        }
        
        private void ExecuteCommand(string cmd)
        {
            string[] parts = cmd.Split(' ');
            
            switch (parts[0].ToLower())
            {
                case "god":
                    Debug.Log("[Console] God mode toggled");
                    break;
                case "spawn":
                    if (parts.Length > 1) Debug.Log($"[Console] Spawning: {parts[1]}");
                    break;
                case "teleport":
                    Debug.Log("[Console] Teleporting...");
                    break;
                default:
                    Debug.Log($"[Console] Unknown command: {cmd}");
                    break;
            }
        }
    }
}
