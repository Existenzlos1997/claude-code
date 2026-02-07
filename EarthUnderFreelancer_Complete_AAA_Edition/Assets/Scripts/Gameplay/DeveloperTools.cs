using UnityEngine;

namespace EarthUnderFreelancer.Gameplay
{
    /// <summary>
    /// Developer tools for debugging and testing
    /// Press F1 to toggle the tools menu
    /// </summary>
    public class DeveloperTools : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool enabledOnStart = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.F1;

        [Header("Features")]
        [SerializeField] private bool showGUI = true;
        [SerializeField] private bool enableCheatCodes = true;

        private bool guiVisible = false;
        private Rect windowRect = new Rect(20, 20, 300, 400);
        private Vector2 scrollPosition;

        // Stats
        private float fps = 0f;
        private int frameCount = 0;
        private float elapsed = 0f;

        private void Start()
        {
            if (!enabledOnStart)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            // Toggle GUI
            if (Input.GetKeyDown(toggleKey))
            {
                guiVisible = !guiVisible;
                Debug.Log($"Developer Tools: {(guiVisible ? "Enabled" : "Disabled")}");
            }

            // Calculate FPS
            frameCount++;
            elapsed += Time.deltaTime;
            if (elapsed >= 1f)
            {
                fps = frameCount / elapsed;
                frameCount = 0;
                elapsed = 0f;
            }

            // Cheat codes (only if enabled)
            if (enableCheatCodes)
            {
                ProcessCheatCodes();
            }
        }

        private void ProcessCheatCodes()
        {
            // Time manipulation
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    Time.timeScale = Mathf.Min(Time.timeScale + 0.5f, 5f);
                    Debug.Log($"Time scale: {Time.timeScale}x");
                }
                if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                {
                    Time.timeScale = Mathf.Max(Time.timeScale - 0.5f, 0.1f);
                    Debug.Log($"Time scale: {Time.timeScale}x");
                }
                if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
                {
                    Time.timeScale = 1f;
                    Debug.Log("Time scale: Normal");
                }
            }

            // God mode toggle (make player invincible)
            if (Input.GetKeyDown(KeyCode.G) && Input.GetKey(KeyCode.LeftControl))
            {
                Debug.Log("Developer Tools: God mode toggled (not implemented yet)");
            }

            // Teleport to coordinates
            if (Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftControl))
            {
                Debug.Log("Developer Tools: Teleport menu (not implemented yet)");
            }
        }

        private void OnGUI()
        {
            if (!showGUI || !guiVisible) return;

            windowRect = GUI.Window(0, windowRect, DrawToolsWindow, "Developer Tools");
        }

        private void DrawToolsWindow(int windowID)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Performance Stats
            GUILayout.Label("=== Performance ===", GUI.skin.box);
            GUILayout.Label($"FPS: {fps:F1}");
            GUILayout.Label($"Time Scale: {Time.timeScale:F2}x");
            GUILayout.Label($"Frame: {Time.frameCount}");

            GUILayout.Space(10);

            // Scene Info
            GUILayout.Label("=== Scene ===", GUI.skin.box);
            GUILayout.Label($"Objects: {FindObjectsOfType<GameObject>().Length}");
            
            Camera cam = Camera.main;
            if (cam != null)
            {
                GUILayout.Label($"Camera: {cam.transform.position:F1}");
            }

            GUILayout.Space(10);

            // Time Controls
            GUILayout.Label("=== Time Controls ===", GUI.skin.box);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0.5x")) Time.timeScale = 0.5f;
            if (GUILayout.Button("1.0x")) Time.timeScale = 1f;
            if (GUILayout.Button("2.0x")) Time.timeScale = 2f;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pause")) Time.timeScale = 0f;
            if (GUILayout.Button("Resume")) Time.timeScale = 1f;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Quick Actions
            GUILayout.Label("=== Quick Actions ===", GUI.skin.box);
            if (GUILayout.Button("Spawn Enemy"))
            {
                Debug.Log("Developer Tools: Spawn enemy (not fully implemented)");
                // Could instantiate an enemy here
            }

            if (GUILayout.Button("Clear All Enemies"))
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    Destroy(enemy);
                }
                Debug.Log($"Developer Tools: Destroyed {enemies.Length} enemies");
            }

            if (GUILayout.Button("Reload Scene"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }

            GUILayout.Space(10);

            // Info
            GUILayout.Label("=== Controls ===", GUI.skin.box);
            GUILayout.Label($"Toggle Menu: {toggleKey}");
            GUILayout.Label("Time +: Ctrl + Plus");
            GUILayout.Label("Time -: Ctrl + Minus");
            GUILayout.Label("Time Reset: Ctrl + 0");

            GUILayout.EndScrollView();

            // Make window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        // Public methods for external use
        public void SetTimeScale(float scale)
        {
            Time.timeScale = Mathf.Clamp(scale, 0f, 10f);
        }

        public void ToggleGUI()
        {
            guiVisible = !guiVisible;
        }
    }
}
