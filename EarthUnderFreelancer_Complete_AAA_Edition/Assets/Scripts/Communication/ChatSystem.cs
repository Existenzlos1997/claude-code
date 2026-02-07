using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Communication
{
    // Iteration 35-36: Text chat system
    public class ChatSystem : MonoBehaviour
    {
        private List<string> chatHistory = new List<string>();
        private string currentMessage = "";
        
        public void SendMessage(string message)
        {
            chatHistory.Add($"[{System.DateTime.Now:HH:mm}] {message}");
            Debug.Log($"[Chat] {message}");
        }
        
        public List<string> GetChatHistory() => chatHistory;
    }
}
