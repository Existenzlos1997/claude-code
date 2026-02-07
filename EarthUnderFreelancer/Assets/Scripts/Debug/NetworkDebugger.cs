using UnityEngine;

namespace EarthUnderFreelancer.Debug
{
    // Iteration 78-79: Network debugging tools
    public class NetworkDebugger : MonoBehaviour
    {
        private float ping = 0f;
        private int packetsSent = 0;
        private int packetsReceived = 0;
        
        public void RecordPing(float ms) => ping = ms;
        public void RecordPacket(bool sent)
        {
            if (sent) packetsSent++;
            else packetsReceived++;
        }
        
        private void OnGUI()
        {
            GUI.Label(new Rect(Screen.width - 150, 10, 140, 60), 
                $"Ping: {ping:F0}ms\nSent: {packetsSent}\nRecv: {packetsReceived}");
        }
    }
}
