using UnityEngine;

namespace EarthUnderFreelancer.Gameplay
{
    // Iteration 12: Photo mode for screenshots
    public class PhotoMode : MonoBehaviour
    {
        private bool isPhotoMode = false;
        private float savedTimeScale = 1f;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TogglePhotoMode();
            }
            
            if (isPhotoMode)
            {
                if (Input.GetKeyDown(KeyCode.F12))
                {
                    TakeScreenshot();
                }
            }
        }
        
        private void TogglePhotoMode()
        {
            isPhotoMode = !isPhotoMode;
            
            if (isPhotoMode)
            {
                savedTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                Debug.Log("[Photo Mode] Enabled - Press F12 to capture");
            }
            else
            {
                Time.timeScale = savedTimeScale;
                Debug.Log("[Photo Mode] Disabled");
            }
        }
        
        private void TakeScreenshot()
        {
            string filename = $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log($"[Photo Mode] Screenshot saved: {filename}");
        }
    }
}
