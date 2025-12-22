using UnityEngine;
using UnityEngine.UI;
using EarthUnderFreelancer.Systems;

namespace EarthUnderFreelancer.UI
{
    /// <summary>
    /// UI for skill tree display and interaction
    /// </summary>
    public class SkillTreeUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject skillTreePanel;
        [SerializeField] private Transform skillTreeContainer;
        [SerializeField] private GameObject skillNodePrefab;

        [Header("Tree Buttons")]
        [SerializeField] private Button combatTreeButton;
        [SerializeField] private Button defenseTreeButton;
        [SerializeField] private Button pilotingTreeButton;
        [SerializeField] private Button tradingTreeButton;

        [Header("Info Panel")]
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private Text skillNameText;
        [SerializeField] private Text skillDescriptionText;
        [SerializeField] private Text skillRankText;
        [SerializeField] private Text skillCostText;
        [SerializeField] private Text skillRequirementsText;
        [SerializeField] private Button unlockButton;

        [Header("Header")]
        [SerializeField] private Text availablePointsText;
        [SerializeField] private Text treeNameText;
        [SerializeField] private Button closeButton;

        [Header("Colors")]
        [SerializeField] private Color unlockedColor = Color.green;
        [SerializeField] private Color availableColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;
        [SerializeField] private Color maxedColor = Color.yellow;

        private string currentTreeId = "combat";
        private Skill selectedSkill;

        private void Start()
        {
            InitializeButtons();
            
            if (skillTreePanel != null)
                skillTreePanel.SetActive(false);
        }

        private void InitializeButtons()
        {
            if (combatTreeButton != null)
                combatTreeButton.onClick.AddListener(() => ShowTree("combat"));
            if (defenseTreeButton != null)
                defenseTreeButton.onClick.AddListener(() => ShowTree("defense"));
            if (pilotingTreeButton != null)
                pilotingTreeButton.onClick.AddListener(() => ShowTree("piloting"));
            if (tradingTreeButton != null)
                tradingTreeButton.onClick.AddListener(() => ShowTree("trading"));

            if (unlockButton != null)
                unlockButton.onClick.AddListener(UnlockSelectedSkill);

            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
        }

        public void Show()
        {
            if (skillTreePanel != null)
                skillTreePanel.SetActive(true);

            ShowTree(currentTreeId);
            UpdatePointsDisplay();
        }

        public void Hide()
        {
            if (skillTreePanel != null)
                skillTreePanel.SetActive(false);
        }

        private void ShowTree(string treeId)
        {
            currentTreeId = treeId;

            if (SkillTreeSystem.Instance == null) return;

            SkillTree tree = SkillTreeSystem.Instance.GetSkillTree(treeId);
            if (tree == null) return;

            if (treeNameText != null)
                treeNameText.text = tree.treeName;

            ClearSkillNodes();
            CreateSkillNodes(tree);

            // Highlight active button
            UpdateTreeButtons();

            // Clear selection
            selectedSkill = null;
            if (infoPanel != null)
                infoPanel.SetActive(false);
        }

        private void ClearSkillNodes()
        {
            if (skillTreeContainer == null) return;

            foreach (Transform child in skillTreeContainer)
            {
                Destroy(child.gameObject);
            }
        }

        private void CreateSkillNodes(SkillTree tree)
        {
            if (skillTreeContainer == null || skillNodePrefab == null) return;

            float nodeSpacing = 120f;
            float tierSpacing = 150f;

            foreach (var skill in tree.skills)
            {
                GameObject nodeObj = Instantiate(skillNodePrefab, skillTreeContainer);
                
                RectTransform rect = nodeObj.GetComponent<RectTransform>();
                if (rect != null)
                {
                    // Position based on tier
                    float x = (skill.tier - 2) * tierSpacing;
                    float y = -tree.skills.IndexOf(skill) * nodeSpacing + 200f;
                    rect.anchoredPosition = new Vector2(x, y);
                }

                // Setup node visuals
                SkillNodeUI nodeUI = nodeObj.GetComponent<SkillNodeUI>();
                if (nodeUI != null)
                {
                    nodeUI.Setup(skill, this);
                }
                else
                {
                    // Fallback setup
                    Button button = nodeObj.GetComponent<Button>();
                    Text text = nodeObj.GetComponentInChildren<Text>();
                    Image image = nodeObj.GetComponent<Image>();

                    if (text != null)
                        text.text = skill.skillName;

                    if (button != null)
                    {
                        Skill capturedSkill = skill;
                        button.onClick.AddListener(() => SelectSkill(capturedSkill));
                    }

                    // Color based on state
                    if (image != null)
                    {
                        image.color = GetSkillColor(skill);
                    }
                }
            }
        }

        private Color GetSkillColor(Skill skill)
        {
            if (SkillTreeSystem.Instance == null) return lockedColor;

            if (SkillTreeSystem.Instance.IsSkillMaxed(skill.skillId))
                return maxedColor;
            if (SkillTreeSystem.Instance.IsSkillUnlocked(skill.skillId))
                return unlockedColor;
            if (SkillTreeSystem.Instance.ArePrerequisitesMet(skill))
                return availableColor;
            return lockedColor;
        }

        public void SelectSkill(Skill skill)
        {
            selectedSkill = skill;
            ShowSkillInfo(skill);
        }

        private void ShowSkillInfo(Skill skill)
        {
            if (infoPanel != null)
                infoPanel.SetActive(true);

            if (skillNameText != null)
                skillNameText.text = skill.skillName;

            if (skillDescriptionText != null)
                skillDescriptionText.text = skill.description;

            if (SkillTreeSystem.Instance != null)
            {
                int currentRank = SkillTreeSystem.Instance.GetSkillRank(skill.skillId);

                if (skillRankText != null)
                    skillRankText.text = $"Rank: {currentRank}/{skill.maxRank}";

                if (skillCostText != null)
                    skillCostText.text = $"Cost: {skill.pointCost} SP";

                // Show requirements
                if (skillRequirementsText != null)
                {
                    if (skill.prerequisites != null && skill.prerequisites.Count > 0)
                    {
                        string reqText = "Requires: ";
                        foreach (var prereq in skill.prerequisites)
                        {
                            reqText += prereq + ", ";
                        }
                        skillRequirementsText.text = reqText.TrimEnd(',', ' ');
                    }
                    else
                    {
                        skillRequirementsText.text = "";
                    }
                }

                // Update unlock button
                if (unlockButton != null)
                {
                    bool canUnlock = currentRank < skill.maxRank &&
                                    SkillTreeSystem.Instance.AvailableSkillPoints >= skill.pointCost &&
                                    SkillTreeSystem.Instance.ArePrerequisitesMet(skill);
                    unlockButton.interactable = canUnlock;

                    Text buttonText = unlockButton.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        if (currentRank >= skill.maxRank)
                            buttonText.text = "Maxed";
                        else
                            buttonText.text = "Unlock";
                    }
                }
            }
        }

        private void UnlockSelectedSkill()
        {
            if (selectedSkill == null) return;
            if (SkillTreeSystem.Instance == null) return;

            if (SkillTreeSystem.Instance.UnlockSkill(selectedSkill.skillId))
            {
                UpdatePointsDisplay();
                ShowSkillInfo(selectedSkill);
                ShowTree(currentTreeId); // Refresh display
            }
        }

        private void UpdatePointsDisplay()
        {
            if (availablePointsText != null && SkillTreeSystem.Instance != null)
            {
                availablePointsText.text = $"Available Points: {SkillTreeSystem.Instance.AvailableSkillPoints}";
            }
        }

        private void UpdateTreeButtons()
        {
            // Highlight current tree button
            SetButtonHighlight(combatTreeButton, currentTreeId == "combat");
            SetButtonHighlight(defenseTreeButton, currentTreeId == "defense");
            SetButtonHighlight(pilotingTreeButton, currentTreeId == "piloting");
            SetButtonHighlight(tradingTreeButton, currentTreeId == "trading");
        }

        private void SetButtonHighlight(Button button, bool isActive)
        {
            if (button == null) return;

            ColorBlock colors = button.colors;
            colors.normalColor = isActive ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            button.colors = colors;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (skillTreePanel != null && skillTreePanel.activeSelf)
                    Hide();
                else
                    Show();
            }
        }
    }

    /// <summary>
    /// Individual skill node in the skill tree
    /// </summary>
    public class SkillNodeUI : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private Text nameText;
        [SerializeField] private Text rankText;
        [SerializeField] private Button button;

        private Skill skill;
        private SkillTreeUI treeUI;

        public void Setup(Skill skill, SkillTreeUI treeUI)
        {
            this.skill = skill;
            this.treeUI = treeUI;

            if (nameText != null)
                nameText.text = skill.skillName;

            if (icon != null && skill.icon != null)
                icon.sprite = skill.icon;

            if (button != null)
                button.onClick.AddListener(OnClick);

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (SkillTreeSystem.Instance == null) return;

            int rank = SkillTreeSystem.Instance.GetSkillRank(skill.skillId);

            if (rankText != null)
                rankText.text = $"{rank}/{skill.maxRank}";

            if (background != null)
            {
                if (rank >= skill.maxRank)
                    background.color = Color.yellow;
                else if (rank > 0)
                    background.color = Color.green;
                else if (SkillTreeSystem.Instance.ArePrerequisitesMet(skill))
                    background.color = Color.white;
                else
                    background.color = Color.gray;
            }
        }

        private void OnClick()
        {
            if (treeUI != null)
                treeUI.SelectSkill(skill);
        }
    }
}
