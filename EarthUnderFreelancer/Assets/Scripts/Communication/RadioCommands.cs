using UnityEngine;

namespace EarthUnderFreelancer.Communication
{
    // Iteration 37-38: Quick radio commands
    public class RadioCommands : MonoBehaviour
    {
        private string[] commands = { "Roger", "Negative", "Help!", "Form Up", "Attack!", "Retreat!" };
        
        public void SendCommand(int index)
        {
            if (index < commands.Length)
            {
                Debug.Log($"[Radio] {commands[index]}");
            }
        }
    }
}
