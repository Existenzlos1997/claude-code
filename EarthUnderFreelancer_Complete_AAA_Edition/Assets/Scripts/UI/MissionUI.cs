using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// Mission UI for viewing and accepting missions
    /// </summary>
    public class MissionUI : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private Button availableTabButton;
        [SerializeField] private Button activeTabButton;
        [SerializeField] private GameObject availablePanel;
        [SerializeField] private GameObject activePanel;

        [Header("Available Missions")]
        [SerializeField] private Transform availableMissionsList;
        [SerializeField] private GameObject missionCardPrefab;

        [Header("Active Missions")]
        [SerializeField] private Transform activeMissionsList;

        [Header("Mission Details")]
        [SerializeField] private GameObject detailsPanel;
        [SerializeField] private TextMeshProUGUI missionTitleText;
        [SerializeField] private TextMeshProUGUI missionDescriptionText;
        [SerializeField] private TextMeshProUGUI missionRewardsText;
        [SerializeField] private TextMeshProUGUI missionObjectivesText;
        [SerializeField] private TextMeshProUGUI missionFactionText;
        [SerializeField] private TextMeshProUGUI missionDifficultyText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button abandonButton;

        private List<GameObject> missionCards = new List<GameObject>();
        private Systems.GeneratedMission selectedAvailableMission;
        private Systems.ActiveMission selectedActiveMission;
        private bool showingAvailable = true;

        private void OnEnable()
        {
            ShowAvailableMissions();
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            if (Systems.MissionSystem.Instance != null)
            {
                Systems.MissionSystem.Instance.OnMissionsRefreshed += RefreshMissionList;
                Systems.MissionSystem.Instance.OnMissionAccepted += OnMissionAccepted;
                Systems.MissionSystem.Instance.OnMissionCompleted += OnMissionCompleted;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (Systems.MissionSystem.Instance != null)
            {
                Systems.MissionSystem.Instance.OnMissionsRefreshed -= RefreshMissionList;
                Systems.MissionSystem.Instance.OnMissionAccepted -= OnMissionAccepted;
                Systems.MissionSystem.Instance.OnMissionCompleted -= OnMissionCompleted;
            }
        }

        public void ShowAvailableMissions()
        {
            showingAvailable = true;
            if (availablePanel != null) availablePanel.SetActive(true);
            if (activePanel != null) activePanel.SetActive(false);
            RefreshMissionList();
        }

        public void ShowActiveMissions()
        {
            showingAvailable = false;
            if (availablePanel != null) availablePanel.SetActive(false);
            if (activePanel != null) activePanel.SetActive(true);
            RefreshMissionList();
        }

        private void RefreshMissionList()
        {
            ClearMissionCards();

            if (showingAvailable)
            {
                PopulateAvailableMissions();
            }
            else
            {
                PopulateActiveMissions();
            }
        }

        private void ClearMissionCards()
        {
            foreach (var card in missionCards)
            {
                Destroy(card);
            }
            missionCards.Clear();
        }

        private void PopulateAvailableMissions()
        {
            if (Systems.MissionSystem.Instance == null) return;

            foreach (var mission in Systems.MissionSystem.Instance.AvailableMissions)
            {
                CreateMissionCard(mission, availableMissionsList, true);
            }
        }

        private void PopulateActiveMissions()
        {
            if (Systems.MissionSystem.Instance == null) return;

            foreach (var mission in Systems.MissionSystem.Instance.ActiveMissions)
            {
                CreateMissionCard(mission, activeMissionsList, false);
            }
        }

        private void CreateMissionCard(Systems.GeneratedMission mission, Transform parent, bool isAvailable)
        {
            if (missionCardPrefab == null || parent == null) return;

            GameObject card = Instantiate(missionCardPrefab, parent);
            missionCards.Add(card);

            // Setup card visuals
            TextMeshProUGUI titleText = card.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI factionText = card.transform.Find("Faction")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI rewardText = card.transform.Find("Reward")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI difficultyText = card.transform.Find("Difficulty")?.GetComponent<TextMeshProUGUI>();
            Image typeIcon = card.transform.Find("TypeIcon")?.GetComponent<Image>();

            if (titleText != null) titleText.text = mission.missionName;
            if (factionText != null) factionText.text = mission.factionId;
            if (rewardText != null) rewardText.text = $"${mission.creditReward}";
            if (difficultyText != null) difficultyText.text = mission.difficulty.ToString();

            // Set difficulty color
            if (difficultyText != null)
            {
                switch (mission.difficulty)
                {
                    case Systems.MissionDifficulty.Easy: difficultyText.color = Color.green; break;
                    case Systems.MissionDifficulty.Normal: difficultyText.color = Color.yellow; break;
                    case Systems.MissionDifficulty.Hard: difficultyText.color = new Color(1f, 0.5f, 0f); break;
                    case Systems.MissionDifficulty.VeryHard: difficultyText.color = Color.red; break;
                    case Systems.MissionDifficulty.Extreme: difficultyText.color = new Color(0.5f, 0f, 0.5f); break;
                }
            }

            // Click handler
            Button btn = card.GetComponent<Button>();
            if (btn != null)
            {
                if (isAvailable)
                {
                    btn.onClick.AddListener(() => SelectAvailableMission(mission));
                }
                else
                {
                    btn.onClick.AddListener(() => SelectActiveMission(mission as Systems.ActiveMission));
                }
            }
        }

        private void SelectAvailableMission(Systems.GeneratedMission mission)
        {
            selectedAvailableMission = mission;
            selectedActiveMission = null;
            ShowMissionDetails(mission, true);
        }

        private void SelectActiveMission(Systems.ActiveMission mission)
        {
            selectedActiveMission = mission;
            selectedAvailableMission = null;
            ShowMissionDetails(mission, false);
        }

        private void ShowMissionDetails(Systems.GeneratedMission mission, bool canAccept)
        {
            if (detailsPanel != null) detailsPanel.SetActive(true);

            if (missionTitleText != null) missionTitleText.text = mission.missionName;
            if (missionDescriptionText != null) missionDescriptionText.text = mission.description;
            if (missionFactionText != null) missionFactionText.text = $"Faction: {mission.factionId}";
            if (missionDifficultyText != null) missionDifficultyText.text = $"Difficulty: {mission.difficulty}";

            // Rewards
            if (missionRewardsText != null)
            {
                missionRewardsText.text = $"Rewards:\n" +
                    $"  Credits: ${mission.creditReward}\n" +
                    $"  XP: {mission.experienceReward}\n" +
                    $"  Reputation: +{mission.reputationReward}";
            }

            // Objectives
            if (missionObjectivesText != null)
            {
                string objectives = "Objectives:\n";
                foreach (var obj in mission.objectives)
                {
                    string status = obj.isCompleted ? "[✓]" : $"[{obj.currentCount}/{obj.targetCount}]";
                    objectives += $"  {status} {obj.description}\n";
                }
                missionObjectivesText.text = objectives;
            }

            // Buttons
            if (acceptButton != null) acceptButton.gameObject.SetActive(canAccept);
            if (abandonButton != null) abandonButton.gameObject.SetActive(!canAccept);
        }

        public void OnAcceptClicked()
        {
            if (selectedAvailableMission == null) return;

            if (Systems.MissionSystem.Instance.AcceptMission(selectedAvailableMission.missionId))
            {
                UIManager.Instance?.ShowNotification($"Mission accepted: {selectedAvailableMission.missionName}", NotificationType.Success);
                if (detailsPanel != null) detailsPanel.SetActive(false);
                RefreshMissionList();
            }
            else
            {
                UIManager.Instance?.ShowNotification("Cannot accept more missions!", NotificationType.Error);
            }
        }

        public void OnAbandonClicked()
        {
            if (selectedActiveMission == null) return;

            Systems.MissionSystem.Instance.AbandonMission(selectedActiveMission.missionId);
            UIManager.Instance?.ShowNotification($"Mission abandoned: {selectedActiveMission.missionName}", NotificationType.Warning);
            if (detailsPanel != null) detailsPanel.SetActive(false);
            RefreshMissionList();
        }

        public void OnRefreshClicked()
        {
            Systems.MissionSystem.Instance?.RefreshAvailableMissions();
        }

        private void OnMissionAccepted(Systems.ActiveMission mission)
        {
            RefreshMissionList();
        }

        private void OnMissionCompleted(Systems.ActiveMission mission)
        {
            UIManager.Instance?.ShowNotification($"Mission complete: {mission.missionName}!", NotificationType.Success);
            RefreshMissionList();
        }

        public void OnCloseClicked()
        {
            UIManager.Instance?.ShowScreen(UIScreen.GameHUD);
        }
    }
}
