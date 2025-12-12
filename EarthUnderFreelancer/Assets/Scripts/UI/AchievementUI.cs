using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EarthUnderFreelancer.Systems;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// UI for displaying achievements
    /// </summary>
    public class AchievementUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject achievementPanel;
        [SerializeField] private Transform achievementListContainer;
        [SerializeField] private GameObject achievementItemPrefab;

        [Header("Category Buttons")]
        [SerializeField] private Button allButton;
        [SerializeField] private Button combatButton;
        [SerializeField] private Button tradingButton;
        [SerializeField] private Button missionsButton;
        [SerializeField] private Button explorationButton;
        [SerializeField] private Button progressionButton;

        [Header("Info")]
        [SerializeField] private Text totalProgressText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private Button closeButton;

        [Header("Popup")]
        [SerializeField] private GameObject achievementPopup;
        [SerializeField] private Text popupTitleText;
        [SerializeField] private Text popupDescriptionText;
        [SerializeField] private Image popupIcon;
        [SerializeField] private float popupDuration = 3f;

        [Header("Colors")]
        [SerializeField] private Color unlockedColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color lockedColor = new Color(0.4f, 0.4f, 0.4f);
        [SerializeField] private Color progressColor = new Color(0.8f, 0.8f, 0.2f);

        private AchievementCategory? currentFilter = null;
        private List<GameObject> achievementItems = new List<GameObject>();

        private void Start()
        {
            InitializeButtons();

            if (achievementPanel != null)
                achievementPanel.SetActive(false);

            if (achievementPopup != null)
                achievementPopup.SetActive(false);

            // Subscribe to achievement events
            if (AchievementSystem.Instance != null)
            {
                AchievementSystem.Instance.OnAchievementUnlocked += ShowAchievementPopup;
            }
        }

        private void OnDestroy()
        {
            if (AchievementSystem.Instance != null)
            {
                AchievementSystem.Instance.OnAchievementUnlocked -= ShowAchievementPopup;
            }
        }

        private void InitializeButtons()
        {
            if (allButton != null)
                allButton.onClick.AddListener(() => SetFilter(null));
            if (combatButton != null)
                combatButton.onClick.AddListener(() => SetFilter(AchievementCategory.Combat));
            if (tradingButton != null)
                tradingButton.onClick.AddListener(() => SetFilter(AchievementCategory.Trading));
            if (missionsButton != null)
                missionsButton.onClick.AddListener(() => SetFilter(AchievementCategory.Missions));
            if (explorationButton != null)
                explorationButton.onClick.AddListener(() => SetFilter(AchievementCategory.Exploration));
            if (progressionButton != null)
                progressionButton.onClick.AddListener(() => SetFilter(AchievementCategory.Progression));

            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
        }

        public void Show()
        {
            if (achievementPanel != null)
                achievementPanel.SetActive(true);

            RefreshList();
            UpdateTotalProgress();
        }

        public void Hide()
        {
            if (achievementPanel != null)
                achievementPanel.SetActive(false);
        }

        public void Toggle()
        {
            if (achievementPanel != null && achievementPanel.activeSelf)
                Hide();
            else
                Show();
        }

        private void SetFilter(AchievementCategory? category)
        {
            currentFilter = category;
            RefreshList();
        }

        private void RefreshList()
        {
            ClearList();

            if (AchievementSystem.Instance == null) return;

            List<Achievement> achievements;
            
            if (currentFilter.HasValue)
            {
                achievements = AchievementSystem.Instance.GetAchievementsByCategory(currentFilter.Value);
            }
            else
            {
                achievements = AchievementSystem.Instance.AllAchievements;
            }

            foreach (var achievement in achievements)
            {
                // Skip hidden achievements that aren't unlocked
                if (achievement.isHidden && !achievement.isUnlocked)
                    continue;

                CreateAchievementItem(achievement);
            }
        }

        private void ClearList()
        {
            foreach (var item in achievementItems)
            {
                Destroy(item);
            }
            achievementItems.Clear();
        }

        private void CreateAchievementItem(Achievement achievement)
        {
            if (achievementListContainer == null) return;

            GameObject itemObj;
            
            if (achievementItemPrefab != null)
            {
                itemObj = Instantiate(achievementItemPrefab, achievementListContainer);
            }
            else
            {
                // Create simple item
                itemObj = new GameObject(achievement.achievementId);
                itemObj.transform.SetParent(achievementListContainer);

                RectTransform rect = itemObj.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(400, 80);

                Image bg = itemObj.AddComponent<Image>();
                bg.color = achievement.isUnlocked ? unlockedColor : lockedColor;

                // Add text
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(itemObj.transform);
                Text text = textObj.AddComponent<Text>();
                text.text = $"{achievement.title}\n{achievement.description}";
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                text.fontSize = 14;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleLeft;

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = new Vector2(10, 5);
                textRect.offsetMax = new Vector2(-10, -5);
            }

            AchievementItemUI itemUI = itemObj.GetComponent<AchievementItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(achievement);
            }

            achievementItems.Add(itemObj);
        }

        private void UpdateTotalProgress()
        {
            if (AchievementSystem.Instance == null) return;

            int unlocked = AchievementSystem.Instance.GetUnlockedCount();
            int total = AchievementSystem.Instance.GetTotalCount();

            if (totalProgressText != null)
            {
                totalProgressText.text = $"Achievements: {unlocked} / {total}";
            }

            if (progressBar != null)
            {
                progressBar.value = (float)unlocked / total;
            }
        }

        private void ShowAchievementPopup(Achievement achievement)
        {
            if (achievementPopup == null) return;

            if (popupTitleText != null)
                popupTitleText.text = achievement.title;

            if (popupDescriptionText != null)
                popupDescriptionText.text = achievement.description;

            if (popupIcon != null && achievement.icon != null)
                popupIcon.sprite = achievement.icon;

            achievementPopup.SetActive(true);

            // Auto-hide
            CancelInvoke(nameof(HidePopup));
            Invoke(nameof(HidePopup), popupDuration);
        }

        private void HidePopup()
        {
            if (achievementPopup != null)
                achievementPopup.SetActive(false);
        }

        private void Update()
        {
            // Toggle with J key
            if (Input.GetKeyDown(KeyCode.J))
            {
                Toggle();
            }
        }
    }

    /// <summary>
    /// Individual achievement item display
    /// </summary>
    public class AchievementItemUI : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private Text progressText;
        [SerializeField] private GameObject unlockedBadge;

        public void Setup(Achievement achievement)
        {
            if (titleText != null)
                titleText.text = achievement.title;

            if (descriptionText != null)
                descriptionText.text = achievement.description;

            if (icon != null && achievement.icon != null)
                icon.sprite = achievement.icon;

            if (unlockedBadge != null)
                unlockedBadge.SetActive(achievement.isUnlocked);

            // Show progress for incomplete achievements
            if (!achievement.isUnlocked && AchievementSystem.Instance != null)
            {
                float progress = AchievementSystem.Instance.GetAchievementProgress(achievement.achievementId);
                
                if (progressBar != null)
                {
                    progressBar.gameObject.SetActive(true);
                    progressBar.value = progress;
                }

                if (progressText != null)
                {
                    int current = AchievementSystem.Instance.Statistics.GetStatistic(achievement.statisticKey);
                    progressText.text = $"{current}/{achievement.requirement}";
                }
            }
            else
            {
                if (progressBar != null)
                    progressBar.gameObject.SetActive(false);
                if (progressText != null)
                    progressText.text = "";
            }

            // Color based on state
            if (background != null)
            {
                background.color = achievement.isUnlocked ? 
                    new Color(0.2f, 0.6f, 0.2f, 0.8f) : 
                    new Color(0.3f, 0.3f, 0.3f, 0.8f);
            }
        }
    }
}
