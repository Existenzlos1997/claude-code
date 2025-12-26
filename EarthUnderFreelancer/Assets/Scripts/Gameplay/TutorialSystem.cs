using UnityEngine;
using System.Collections.Generic;
using System;

namespace EarthUnderFreelancer.Gameplay
{
    /// <summary>
    /// Interactive tutorial system for new players
    /// Implements VERBESSERUNGSPLAN Priority 3 - Tutorial System
    /// Provides step-by-step flight, combat, and trading tutorials
    /// </summary>
    public class TutorialSystem : MonoBehaviour
    {
        [Header("Tutorial Settings")]
        [SerializeField] private bool autoStartTutorial = false;
        [SerializeField] private TutorialType startingTutorial = TutorialType.FlightBasics;
        [SerializeField] private bool skipCompletedTutorials = true;

        [Header("UI Settings")]
        [SerializeField] private bool showTutorialUI = true;
        [SerializeField] private float messageDisplayTime = 5f;

        public enum TutorialType
        {
            FlightBasics,       // Learn basic flight controls
            CombatBasics,       // Learn weapons and targeting
            TradingBasics,      // Learn trading system
            MissionSystem,      // Learn mission mechanics
            Advanced            // Advanced techniques
        }

        private TutorialStep currentStep;
        private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
        private int currentStepIndex = 0;
        private bool tutorialActive = false;
        private bool tutorialComplete = false;

        private float messageTimer = 0f;
        private string currentMessage = "";
        private Rect tutorialWindowRect = new Rect(Screen.width - 420, 10, 400, 200);

        // Tutorial completion tracking
        private HashSet<TutorialType> completedTutorials = new HashSet<TutorialType>();

        private class TutorialStep
        {
            public string title;
            public string description;
            public string instruction;
            public Func<bool> completionCondition;
            public Action onStepStart;
            public Action onStepComplete;
            public bool isComplete = false;

            public TutorialStep(string title, string description, string instruction, Func<bool> condition = null)
            {
                this.title = title;
                this.description = description;
                this.instruction = instruction;
                this.completionCondition = condition ?? (() => false);
            }
        }

        private void Start()
        {
            if (autoStartTutorial)
            {
                StartTutorial(startingTutorial);
            }
        }

        private void Update()
        {
            if (!tutorialActive || tutorialComplete) return;

            // Update message timer
            if (messageTimer > 0)
            {
                messageTimer -= Time.deltaTime;
            }

            // Check current step completion
            if (currentStep != null && !currentStep.isComplete)
            {
                if (currentStep.completionCondition())
                {
                    CompleteCurrentStep();
                }
            }

            // Debug controls
            if (Input.GetKeyDown(KeyCode.N))
            {
                NextStep();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log($"Tutorial: {currentStepIndex + 1}/{tutorialSteps.Count} - {currentStep?.title}");
            }
        }

        /// <summary>
        /// Start a specific tutorial
        /// </summary>
        public void StartTutorial(TutorialType tutorialType)
        {
            if (skipCompletedTutorials && completedTutorials.Contains(tutorialType))
            {
                Debug.Log($"Tutorial {tutorialType} already completed. Skipping.");
                return;
            }

            tutorialSteps.Clear();
            currentStepIndex = 0;
            tutorialActive = true;
            tutorialComplete = false;

            Debug.Log($"Starting tutorial: {tutorialType}");

            switch (tutorialType)
            {
                case TutorialType.FlightBasics:
                    CreateFlightBasicsTutorial();
                    break;
                case TutorialType.CombatBasics:
                    CreateCombatBasicsTutorial();
                    break;
                case TutorialType.TradingBasics:
                    CreateTradingBasicsTutorial();
                    break;
                case TutorialType.MissionSystem:
                    CreateMissionSystemTutorial();
                    break;
                case TutorialType.Advanced:
                    CreateAdvancedTutorial();
                    break;
            }

            if (tutorialSteps.Count > 0)
            {
                StartStep(0);
            }
        }

        private void CreateFlightBasicsTutorial()
        {
            tutorialSteps.Add(new TutorialStep(
                "Welcome to EarthUnder Freelancer!",
                "This tutorial will teach you basic flight controls.",
                "Press any key to continue...",
                () => Input.anyKeyDown
            ));

            tutorialSteps.Add(new TutorialStep(
                "Throttle Control",
                "W increases throttle, S decreases throttle.",
                "Press W to increase throttle",
                () => Input.GetKey(KeyCode.W)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Pitch and Roll",
                "Move your mouse to control pitch (up/down) and roll (tilt).",
                "Move your mouse around to control the aircraft",
                () => Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0
            ));

            tutorialSteps.Add(new TutorialStep(
                "Yaw Control",
                "A and D control yaw (left/right turn).",
                "Press A or D to turn",
                () => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Boost",
                "Hold SHIFT for afterburner/boost.",
                "Hold SHIFT to activate boost",
                () => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Braking",
                "Press SPACE to brake/slow down quickly.",
                "Press SPACE to brake",
                () => Input.GetKey(KeyCode.Space)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Flight Basics Complete!",
                "You've learned the basic flight controls.",
                "Tutorial complete. Press N for next tutorial or ESC to exit",
                () => false // Manual completion
            ));
        }

        private void CreateCombatBasicsTutorial()
        {
            tutorialSteps.Add(new TutorialStep(
                "Combat Tutorial",
                "Learn to fight and survive in aerial combat.",
                "Press any key to begin..."
            ));

            tutorialSteps.Add(new TutorialStep(
                "Targeting",
                "Press TAB to cycle through nearby targets.",
                "Press TAB to target an enemy",
                () => Input.GetKeyDown(KeyCode.Tab)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Primary Weapon",
                "Left mouse button fires your primary weapon (guns).",
                "Press LEFT MOUSE BUTTON to fire",
                () => Input.GetMouseButton(0)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Secondary Weapon",
                "Right mouse button fires missiles/rockets.",
                "Press RIGHT MOUSE BUTTON to fire missile",
                () => Input.GetMouseButton(1)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Weapon Switching",
                "Press R to switch between weapon types.",
                "Press R to change weapons",
                () => Input.GetKeyDown(KeyCode.R)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Combat Complete!",
                "You've learned basic combat controls.",
                "Practice makes perfect! Press N to continue",
                () => false
            ));
        }

        private void CreateTradingBasicsTutorial()
        {
            tutorialSteps.Add(new TutorialStep(
                "Trading Tutorial",
                "Learn how to make money through trading.",
                "Press any key to begin..."
            ));

            tutorialSteps.Add(new TutorialStep(
                "Docking",
                "Press F near a station to dock.",
                "Press F when near a station",
                () => Input.GetKeyDown(KeyCode.F)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Inventory",
                "Press I to open your inventory.",
                "Press I to open inventory",
                () => Input.GetKeyDown(KeyCode.I)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Trading Complete!",
                "You've learned the basics of trading.",
                "Buy low, sell high! Press N to continue",
                () => false
            ));
        }

        private void CreateMissionSystemTutorial()
        {
            tutorialSteps.Add(new TutorialStep(
                "Mission System",
                "Learn how to accept and complete missions.",
                "Press any key to begin..."
            ));

            tutorialSteps.Add(new TutorialStep(
                "Mission Log",
                "Press M to open the mission log.",
                "Press M to view missions",
                () => Input.GetKeyDown(KeyCode.M)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Mission System Complete!",
                "You can now accept and complete missions.",
                "Check stations for available missions!",
                () => false
            ));
        }

        private void CreateAdvancedTutorial()
        {
            tutorialSteps.Add(new TutorialStep(
                "Advanced Techniques",
                "Master advanced flight and combat techniques.",
                "Press any key to begin..."
            ));

            tutorialSteps.Add(new TutorialStep(
                "Skill Tree",
                "Press K to open your skill tree.",
                "Press K to view skills",
                () => Input.GetKeyDown(KeyCode.K)
            ));

            tutorialSteps.Add(new TutorialStep(
                "Achievements",
                "Press J to view achievements.",
                "Press J to view achievements",
                () => Input.GetKeyDown(KeyCode.J)
            ));

            tutorialSteps.Add(new TutorialStep(
                "All Tutorials Complete!",
                "You're ready to become a legend!",
                "Good luck out there, pilot!",
                () => false
            ));
        }

        private void StartStep(int stepIndex)
        {
            if (stepIndex >= tutorialSteps.Count)
            {
                CompleteTutorial();
                return;
            }

            currentStepIndex = stepIndex;
            currentStep = tutorialSteps[stepIndex];
            currentStep.isComplete = false;

            ShowMessage($"{currentStep.title}\n{currentStep.description}\n\n{currentStep.instruction}");

            currentStep.onStepStart?.Invoke();

            Debug.Log($"[Tutorial] Step {stepIndex + 1}/{tutorialSteps.Count}: {currentStep.title}");
        }

        private void CompleteCurrentStep()
        {
            if (currentStep == null || currentStep.isComplete) return;

            currentStep.isComplete = true;
            currentStep.onStepComplete?.Invoke();

            ShowMessage($"✓ {currentStep.title} Complete!");

            Debug.Log($"[Tutorial] Step complete: {currentStep.title}");

            // Auto-advance after a short delay
            Invoke(nameof(NextStep), 2f);
        }

        private void NextStep()
        {
            StartStep(currentStepIndex + 1);
        }

        private void CompleteTutorial()
        {
            tutorialComplete = true;
            tutorialActive = false;

            ShowMessage("Tutorial Complete! Well done!");

            Debug.Log("[Tutorial] Tutorial completed!");
        }

        private void ShowMessage(string message)
        {
            currentMessage = message;
            messageTimer = messageDisplayTime;
        }

        private void OnGUI()
        {
            if (!showTutorialUI || !tutorialActive) return;

            // Tutorial window
            GUI.Window(2, tutorialWindowRect, DrawTutorialWindow, "Tutorial");

            // Current message overlay (if timer active)
            if (messageTimer > 0 && !string.IsNullOrEmpty(currentMessage))
            {
                Rect messageRect = new Rect(
                    Screen.width / 2 - 200,
                    Screen.height - 150,
                    400,
                    100
                );

                GUI.Box(messageRect, "");
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 14;
                style.wordWrap = true;
                GUI.Label(messageRect, currentMessage, style);
            }
        }

        private void DrawTutorialWindow(int windowID)
        {
            GUILayout.Label($"Step {currentStepIndex + 1} of {tutorialSteps.Count}", EditorStyles.boldLabel);
            
            if (currentStep != null)
            {
                GUILayout.Label(currentStep.title, EditorStyles.boldLabel);
                GUILayout.Space(5);
                GUILayout.Label(currentStep.description);
                GUILayout.Space(10);
                GUILayout.Label(currentStep.instruction, EditorStyles.helpBox);
            }

            GUILayout.Space(10);
            
            // Progress bar
            float progress = tutorialSteps.Count > 0 ? (float)currentStepIndex / tutorialSteps.Count : 0f;
            Rect progressRect = GUILayoutUtility.GetRect(380, 20);
            EditorGUI.ProgressBar(progressRect, progress, $"{(progress * 100f):F0}%");

            GUILayout.Space(10);
            GUILayout.Label("Controls: N = Next step, T = Info, ESC = Skip", GUI.skin.label);

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        // Public API
        public void SkipTutorial()
        {
            tutorialActive = false;
            tutorialComplete = false;
            Debug.Log("[Tutorial] Tutorial skipped");
        }

        public bool IsTutorialActive() => tutorialActive;
        public bool IsTutorialComplete() => tutorialComplete;
        public int GetCurrentStepIndex() => currentStepIndex;
        public int GetTotalSteps() => tutorialSteps.Count;
    }

    // Simple EditorStyles replacement for runtime
    public static class EditorStyles
    {
        public static GUIStyle boldLabel = new GUIStyle { fontStyle = FontStyle.Bold };
        public static GUIStyle helpBox = new GUIStyle(GUI.skin.box);
    }

    // Simple EditorGUI replacement for runtime
    public static class EditorGUI
    {
        public static void ProgressBar(Rect rect, float value, string text)
        {
            GUI.Box(rect, "");
            Rect fillRect = new Rect(rect.x + 2, rect.y + 2, (rect.width - 4) * value, rect.height - 4);
            GUI.Box(fillRect, "");
            
            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(rect, text, textStyle);
        }
    }
}
