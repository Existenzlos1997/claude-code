using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Accessibility
{
    // Iteration 65-66: Control remapping system
    public class InputRemapping : MonoBehaviour
    {
        private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();
        
        private void Start()
        {
            keyBindings["Fire"] = KeyCode.Mouse0;
            keyBindings["Throttle Up"] = KeyCode.W;
            keyBindings["Throttle Down"] = KeyCode.S;
        }
        
        public void RemapKey(string action, KeyCode newKey)
        {
            keyBindings[action] = newKey;
            Debug.Log($"[Input] {action} remapped to {newKey}");
        }
        
        public KeyCode GetBinding(string action) => keyBindings.ContainsKey(action) ? keyBindings[action] : KeyCode.None;
    }
}
