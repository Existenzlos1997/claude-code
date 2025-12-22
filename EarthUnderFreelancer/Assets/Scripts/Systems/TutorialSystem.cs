using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Interactive tutorial system for new players
    /// </summary>
    public class TutorialSystem : MonoBehaviour
    {
        public static TutorialSystem Instance { get; private set; }

        [Header("Tutorial State")]
        [SerializeField] private bool tutorialEnabled = true;
        [SerializeField] private bool tutorialCompleted = false;
        [SerializeField] private int currentStepIndex = 0;

        [Header("Tutorial Steps")]
        [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

        [Header("UI References")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private UnityEngine.UI.Text titleText;
        [SerializeField] private UnityEngine.UI.Text descriptionText;
        [SerializeField] private UnityEngine.UI.Text progressText;
        [SerializeField] private UnityEngine.UI.Button skipButton;
        [SerializeField] private UnityEngine.UI.Button nextButton;

        [Header("Highlight")]
        [SerializeField] private GameObject highlightPrefab;
        private GameObject currentHighlight;

        public bool TutorialCompleted => tutorialCompleted;
        public TutorialStep CurrentStep => currentStepIndex < tutorialSteps.Count ? tutorialSteps[currentStepIndex] : null;

        public event System.Action<TutorialStep> OnStepStarted;
        public event System.Action<TutorialStep> OnStepCompleted;
        public event System.Action OnTutorialCompleted;
        public event System.Action OnTutorialSkipped;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeTutorial();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Load tutorial state
            tutorialCompleted = PlayerPrefs.GetInt("tutorial_completed", 0) == 1;

            if (!tutorialCompleted && tutorialEnabled)
            {
                StartTutorial();
            }
        }

        private void InitializeTutorial()
        {
            // Define tutorial steps
            tutorialSteps.Add(new TutorialStep
            {
                stepId = "welcome",
                title = "Welcome to EarthUnderFreelancer!",
                description = "You are about to embark on an epic space adventure. This tutorial will guide you through the basics.",
                requiresAction = false,
                autoAdvanceDelay = 5f
            });

            tutorialSteps.Add(new TutorialStep
            {
                stepId = "movement",
                title = "Flight Controls",
                description = "Use WASD or left joystick to control your ship.\nW/S = Throttle\nA/D = Roll\nMouse = Aim",
                requiresAction = true,
                requiredInputs = new string[] { "Horizontal", "Vertical" },
                inputThreshold = 0.5f
            });

            tutorialSteps.Add(new TutorialStep
            {
                stepId = "boost",
                title = "Boost",
                description = "Hold SHIFT to activate boost for increased speed.\nBoost consumes energy that regenerates over time.",
                requiresAction = true,
                requiredKey = KeyCode.LeftShift
            });

            tutorialSteps.Add(new TutorialStep
            {
                stepId = "brake",
                title = "Braking",
                description = "Press SPACE to brake and slow down quickly.",
                requiresAction = true,
                requiredKey = KeyCode.Space
            });

            tutorialSteps.Add(new TutorialStep
            {
                stepId = "combat_intro",
                title = "Combat",
                description = "Left-click to fire your primary weapons.\nRight-click for secondary weapons like missiles.",
                requiresAction = false,
                autoAdvanceDelay = 4f
            });

            tutorialSteps.Add(new TutorialStep
            {
                stepId = "targeting",
                title = "Targeting",
                description = "Press TAB to cycle through targets.\nThe lead indicator shows where to aim.",
                requiresAction = true,
                requiredKey = KeyCode.Tab
            });

            tutorialSteps.Add(new TutorialStep
            {
                stepId = "inventory",
                title = "Inventory",
                description = "Press I to open your inventory.\nManage cargo, equipment and upgrades here.",
                requiresAction = true,
                requiredKey = KeyCode.I
            });

            tutorialSteps.Add(new TutorialStep
            {
                stepId = "mission_log",
                title = "Missions",
                description = "Press M to view available and active missions.\nComplete missions to earn credits and reputation.",
                requiresAction = true,
                requiredKey = KeyCode.M
            });

            tutorialSteps.Add(new TutorialStep
            {
                stepId = "stations",
                title = "Space Stations",
                description = "Approach stations to dock.\nStations offer trading, missions, repairs and upgrades.",
                requiresAction = false,
                autoAdvanceDelay = 4f
            });

            tutorialSteps.Add(new TutorialStep
            {
                stepId = "complete",
                title = "Tutorial Complete!",
                description = "You're ready to explore the galaxy!\n\nGood luck, Freelancer!",
                requiresAction = false,
                autoAdvanceDelay = 3f
            });
        }

        public void StartTutorial()
        {
            if (tutorialCompleted && !tutorialEnabled) return;

            currentStepIndex = 0;
            ShowCurrentStep();
        }

        private void ShowCurrentStep()
        {
            if (currentStepIndex >= tutorialSteps.Count)
            {
                CompleteTutorial();
                return;
            }

            TutorialStep step = tutorialSteps[currentStepIndex];

            // Update UI
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }

            if (titleText != null)
            {
                titleText.text = step.title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = step.description;
            }

            if (progressText != null)
            {
                progressText.text = $"Step {currentStepIndex + 1} of {tutorialSteps.Count}";
            }

            // Show highlight if specified
            if (!string.IsNullOrEmpty(step.highlightObjectName))
            {
                ShowHighlight(step.highlightObjectName);
            }
            else
            {
                HideHighlight();
            }

            OnStepStarted?.Invoke(step);

            // Auto-advance if no action required
            if (!step.requiresAction && step.autoAdvanceDelay > 0)
            {
                Invoke(nameof(AdvanceStep), step.autoAdvanceDelay);
            }
        }

        private void Update()
        {
            if (tutorialCompleted || !tutorialEnabled) return;
            if (currentStepIndex >= tutorialSteps.Count) return;

            TutorialStep step = tutorialSteps[currentStepIndex];

            if (step.requiresAction)
            {
                if (CheckStepCondition(step))
                {
                    AdvanceStep();
                }
            }
        }

        private bool CheckStepCondition(TutorialStep step)
        {
            // Check required key
            if (step.requiredKey != KeyCode.None)
            {
                if (Input.GetKeyDown(step.requiredKey))
                {
                    return true;
                }
            }

            // Check required inputs
            if (step.requiredInputs != null && step.requiredInputs.Length > 0)
            {
                foreach (var inputName in step.requiredInputs)
                {
                    if (Mathf.Abs(Input.GetAxis(inputName)) > step.inputThreshold)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AdvanceStep()
        {
            CancelInvoke(nameof(AdvanceStep));

            TutorialStep completedStep = tutorialSteps[currentStepIndex];
            OnStepCompleted?.Invoke(completedStep);

            currentStepIndex++;
            ShowCurrentStep();
        }

        public void SkipTutorial()
        {
            CancelInvoke(nameof(AdvanceStep));
            tutorialCompleted = true;
            HideTutorial();
            
            OnTutorialSkipped?.Invoke();
            SaveTutorialState();
        }

        private void CompleteTutorial()
        {
            tutorialCompleted = true;
            HideTutorial();
            
            OnTutorialCompleted?.Invoke();
            SaveTutorialState();

            // Grant completion reward
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddCredits(500);
            }

            if (AchievementSystem.Instance != null)
            {
                AchievementSystem.Instance.UnlockAchievement("tutorial_complete");
            }
        }

        private void HideTutorial()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
            HideHighlight();
        }

        private void ShowHighlight(string objectName)
        {
            GameObject target = GameObject.Find(objectName);
            if (target == null) return;

            if (highlightPrefab != null)
            {
                HideHighlight();
                currentHighlight = Instantiate(highlightPrefab, target.transform);
            }
        }

        private void HideHighlight()
        {
            if (currentHighlight != null)
            {
                Destroy(currentHighlight);
                currentHighlight = null;
            }
        }

        private void SaveTutorialState()
        {
            PlayerPrefs.SetInt("tutorial_completed", tutorialCompleted ? 1 : 0);
            PlayerPrefs.SetInt("tutorial_step", currentStepIndex);
            PlayerPrefs.Save();
        }

        public void ResetTutorial()
        {
            tutorialCompleted = false;
            currentStepIndex = 0;
            SaveTutorialState();
        }

        public void EnableTutorial(bool enabled)
        {
            tutorialEnabled = enabled;
        }
    }

    [System.Serializable]
    public class TutorialStep
    {
        public string stepId;
        public string title;
        [TextArea(2, 4)]
        public string description;
        
        public bool requiresAction;
        public KeyCode requiredKey = KeyCode.None;
        public string[] requiredInputs;
        public float inputThreshold = 0.5f;
        
        public float autoAdvanceDelay = 0f;
        public string highlightObjectName;
        
        public UnityEvent onStepStart;
        public UnityEvent onStepComplete;
    }
}
