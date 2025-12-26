using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.UI
{
    // Iteration 86-88: Notification and alert system
    public class NotificationSystem : MonoBehaviour
    {
        private Queue<string> notifications = new Queue<string>();
        
        public void ShowNotification(string message)
        {
            notifications.Enqueue(message);
            Debug.Log($"[Notification] {message}");
        }
    }
}
