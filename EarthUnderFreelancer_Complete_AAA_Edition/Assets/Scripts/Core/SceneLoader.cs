using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Handles scene loading and transitions
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("Loading Screen")]
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private UnityEngine.UI.Slider progressBar;
        [SerializeField] private UnityEngine.UI.Text progressText;
        [SerializeField] private UnityEngine.UI.Text tipText;

        public const string MAIN_MENU = "MainMenu";
        public const string LOBBY = "Lobby";
        public const string HANGAR = "Hangar";
        public const string MISSION = "Mission";
        public const string PVP_ARENA = "PVPArena";
        public const string TRADING_POST = "TradingPost";

        private string[] loadingTips = new string[]
        {
            "Upgrade your shields to survive longer in combat.",
            "Trade goods between stations for profit.",
            "Complete missions to earn reputation with factions.",
            "Watch rewarded ads to earn bonus credits.",
            "Customize your ship in the hangar.",
            "Team up with other players for challenging missions.",
            "Higher tier weapons deal more damage but cost more.",
            "Some factions will attack you if your reputation is too low."
        };

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        public void LoadSceneWithDelay(string sceneName, float delay)
        {
            StartCoroutine(LoadSceneWithDelayCoroutine(sceneName, delay));
        }

        private IEnumerator LoadSceneWithDelayCoroutine(string sceneName, float delay)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(true);
                
                if (tipText != null)
                {
                    tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
                }
            }

            yield return new WaitForSeconds(0.1f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                
                if (progressBar != null)
                    progressBar.value = progress;
                
                if (progressText != null)
                    progressText.text = $"Loading... {(int)(progress * 100)}%";

                if (asyncLoad.progress >= 0.9f)
                {
                    if (progressText != null)
                        progressText.text = "Press any key to continue...";
                    
                    yield return new WaitForSeconds(0.5f);
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }

            if (loadingScreen != null)
            {
                loadingScreen.SetActive(false);
            }
        }

        public void LoadMainMenu() => LoadScene(MAIN_MENU);
        public void LoadLobby() => LoadScene(LOBBY);
        public void LoadHangar() => LoadScene(HANGAR);
        public void LoadPVP() => LoadScene(PVP_ARENA);

        public void LoadMission(string missionId)
        {
            PlayerPrefs.SetString("CurrentMission", missionId);
            LoadScene(MISSION);
        }

        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
