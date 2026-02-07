using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Gameplay
{
    // Iteration 11: Replay system for recording and playback
    public class ReplaySystem : MonoBehaviour
    {
        private struct RecordFrame
        {
            public Vector3 position;
            public Quaternion rotation;
            public float timestamp;
        }
        
        private List<RecordFrame> recording = new List<RecordFrame>();
        private bool isRecording = false;
        private bool isPlaying = false;
        private int playbackIndex = 0;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                if (!isRecording) StartRecording();
                else StopRecording();
            }
            
            if (Input.GetKeyDown(KeyCode.F10) && recording.Count > 0)
            {
                StartPlayback();
            }
            
            if (isRecording)
            {
                RecordFrame();
            }
            
            if (isPlaying)
            {
                PlaybackFrame();
            }
        }
        
        private void StartRecording()
        {
            recording.Clear();
            isRecording = true;
            Debug.Log("[Replay] Recording started");
        }
        
        private void StopRecording()
        {
            isRecording = false;
            Debug.Log($"[Replay] Recording stopped. {recording.Count} frames recorded");
        }
        
        private void RecordFrame()
        {
            recording.Add(new RecordFrame
            {
                position = transform.position,
                rotation = transform.rotation,
                timestamp = Time.time
            });
        }
        
        private void StartPlayback()
        {
            playbackIndex = 0;
            isPlaying = true;
            Debug.Log("[Replay] Playback started");
        }
        
        private void PlaybackFrame()
        {
            if (playbackIndex >= recording.Count)
            {
                isPlaying = false;
                Debug.Log("[Replay] Playback complete");
                return;
            }
            
            RecordFrame frame = recording[playbackIndex];
            transform.position = frame.position;
            transform.rotation = frame.rotation;
            playbackIndex++;
        }
    }
}
