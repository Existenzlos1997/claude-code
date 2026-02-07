using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Story-driven quest system with branching narratives
    /// </summary>
    public class QuestSystem : MonoBehaviour
    {
        public static QuestSystem Instance { get; private set; }

        [Header("Quest Data")]
        [SerializeField] private List<QuestChain> questChains = new List<QuestChain>();
        [SerializeField] private List<string> completedQuests = new List<string>();
        [SerializeField] private List<ActiveQuest> activeQuests = new List<ActiveQuest>();

        [Header("Settings")]
        [SerializeField] private int maxActiveQuests = 5;

        public List<ActiveQuest> ActiveQuests => activeQuests;
        public List<string> CompletedQuests => completedQuests;

        public event System.Action<Quest> OnQuestAccepted;
        public event System.Action<Quest> OnQuestCompleted;
        public event System.Action<Quest> OnQuestFailed;
        public event System.Action<QuestObjective> OnObjectiveProgress;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeQuestChains();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeQuestChains()
        {
            // Main Story Quest Chain
            QuestChain mainStory = new QuestChain
            {
                chainId = "main_story",
                chainName = "The Freelancer's Path",
                description = "Your journey from rookie pilot to legendary freelancer."
            };

            // Quest 1: Tutorial/Introduction
            mainStory.quests.Add(new Quest
            {
                questId = "ms_01_newcomer",
                questName = "The Newcomer",
                description = "You've just arrived in the sector. Meet with the station commander to get your bearings.",
                questType = QuestType.Story,
                difficulty = QuestDifficulty.Easy,
                creditReward = 500,
                experienceReward = 100,
                prerequisites = new List<string>(),
                objectives = new List<QuestObjective>
                {
                    new QuestObjective
                    {
                        objectiveId = "dock_station",
                        description = "Dock at the starting station",
                        type = QuestObjectiveType.DockAtStation,
                        targetId = "station_alpha",
                        targetCount = 1
                    },
                    new QuestObjective
                    {
                        objectiveId = "talk_commander",
                        description = "Speak with Commander Hayes",
                        type = QuestObjectiveType.TalkToNPC,
                        targetId = "npc_commander_hayes",
                        targetCount = 1
                    }
                }
            });

            // Quest 2: First Combat
            mainStory.quests.Add(new Quest
            {
                questId = "ms_02_first_blood",
                questName = "First Blood",
                description = "Pirates have been spotted in the nearby asteroid field. Eliminate them.",
                questType = QuestType.Story,
                difficulty = QuestDifficulty.Easy,
                creditReward = 1000,
                experienceReward = 200,
                prerequisites = new List<string> { "ms_01_newcomer" },
                objectives = new List<QuestObjective>
                {
                    new QuestObjective
                    {
                        objectiveId = "kill_pirates",
                        description = "Destroy pirate ships",
                        type = QuestObjectiveType.KillEnemies,
                        targetId = "faction_pirates",
                        targetCount = 3
                    },
                    new QuestObjective
                    {
                        objectiveId = "return_station",
                        description = "Return to Station Alpha",
                        type = QuestObjectiveType.DockAtStation,
                        targetId = "station_alpha",
                        targetCount = 1
                    }
                }
            });

            // Quest 3: Trading Introduction
            mainStory.quests.Add(new Quest
            {
                questId = "ms_03_supply_run",
                questName = "Supply Run",
                description = "The station needs supplies from Station Beta. Complete a trade run.",
                questType = QuestType.Story,
                difficulty = QuestDifficulty.Easy,
                creditReward = 1500,
                experienceReward = 250,
                prerequisites = new List<string> { "ms_02_first_blood" },
                objectives = new List<QuestObjective>
                {
                    new QuestObjective
                    {
                        objectiveId = "buy_supplies",
                        description = "Purchase medical supplies",
                        type = QuestObjectiveType.PurchaseItem,
                        targetId = "medical_supplies",
                        targetCount = 10
                    },
                    new QuestObjective
                    {
                        objectiveId = "deliver_supplies",
                        description = "Deliver supplies to Station Alpha",
                        type = QuestObjectiveType.DeliverItem,
                        targetId = "medical_supplies",
                        targetCount = 10
                    }
                }
            });

            questChains.Add(mainStory);

            // Side Quest Chain: Bounty Hunter
            QuestChain bountyHunter = new QuestChain
            {
                chainId = "bounty_hunter",
                chainName = "Bounty Hunter",
                description = "Take on bounties and become a feared hunter."
            };

            bountyHunter.quests.Add(new Quest
            {
                questId = "bh_01_small_fry",
                questName = "Small Fry",
                description = "Take on your first bounty contract.",
                questType = QuestType.Side,
                difficulty = QuestDifficulty.Easy,
                creditReward = 750,
                experienceReward = 150,
                objectives = new List<QuestObjective>
                {
                    new QuestObjective
                    {
                        objectiveId = "kill_target",
                        description = "Eliminate the bounty target",
                        type = QuestObjectiveType.KillTarget,
                        targetId = "bounty_small_fry",
                        targetCount = 1
                    }
                }
            });

            questChains.Add(bountyHunter);

            // Side Quest Chain: Trader
            QuestChain traderPath = new QuestChain
            {
                chainId = "trader_path",
                chainName = "The Merchant's Way",
                description = "Build your trading empire."
            };

            traderPath.quests.Add(new Quest
            {
                questId = "tp_01_humble_beginnings",
                questName = "Humble Beginnings",
                description = "Make your first profitable trade.",
                questType = QuestType.Side,
                difficulty = QuestDifficulty.Easy,
                creditReward = 500,
                experienceReward = 100,
                objectives = new List<QuestObjective>
                {
                    new QuestObjective
                    {
                        objectiveId = "make_profit",
                        description = "Earn 1000 credits from trading",
                        type = QuestObjectiveType.EarnCredits,
                        targetCount = 1000
                    }
                }
            });

            questChains.Add(traderPath);
        }

        public bool AcceptQuest(string questId)
        {
            if (activeQuests.Count >= maxActiveQuests) return false;

            Quest quest = FindQuest(questId);
            if (quest == null) return false;

            // Check prerequisites
            foreach (var prereq in quest.prerequisites)
            {
                if (!completedQuests.Contains(prereq))
                {
                    return false;
                }
            }

            // Check if already active
            foreach (var active in activeQuests)
            {
                if (active.questId == questId) return false;
            }

            ActiveQuest activeQuest = new ActiveQuest
            {
                questId = quest.questId,
                questName = quest.questName,
                startTime = Time.time,
                objectives = new List<QuestObjectiveProgress>()
            };

            foreach (var obj in quest.objectives)
            {
                activeQuest.objectives.Add(new QuestObjectiveProgress
                {
                    objectiveId = obj.objectiveId,
                    currentCount = 0,
                    isCompleted = false
                });
            }

            activeQuests.Add(activeQuest);
            OnQuestAccepted?.Invoke(quest);

            return true;
        }

        public void UpdateObjective(string questId, string objectiveId, int progress = 1)
        {
            ActiveQuest active = GetActiveQuest(questId);
            if (active == null) return;

            Quest quest = FindQuest(questId);
            if (quest == null) return;

            for (int i = 0; i < active.objectives.Count; i++)
            {
                if (active.objectives[i].objectiveId == objectiveId)
                {
                    active.objectives[i].currentCount += progress;

                    QuestObjective questObj = quest.objectives[i];
                    if (active.objectives[i].currentCount >= questObj.targetCount)
                    {
                        active.objectives[i].isCompleted = true;
                    }

                    OnObjectiveProgress?.Invoke(questObj);
                    break;
                }
            }

            // Check if quest is complete
            CheckQuestCompletion(questId);
        }

        public void UpdateObjectiveByType(QuestObjectiveType type, string targetId, int progress = 1)
        {
            for (int i = activeQuests.Count - 1; i >= 0; i--)
            {
                ActiveQuest active = activeQuests[i];
                Quest quest = FindQuest(active.questId);
                if (quest == null) continue;

                for (int j = 0; j < quest.objectives.Count; j++)
                {
                    QuestObjective obj = quest.objectives[j];
                    if (obj.type == type && 
                        (string.IsNullOrEmpty(obj.targetId) || obj.targetId == targetId))
                    {
                        if (!active.objectives[j].isCompleted)
                        {
                            active.objectives[j].currentCount += progress;
                            if (active.objectives[j].currentCount >= obj.targetCount)
                            {
                                active.objectives[j].isCompleted = true;
                            }
                            OnObjectiveProgress?.Invoke(obj);
                        }
                    }
                }

                CheckQuestCompletion(active.questId);
            }
        }

        private void CheckQuestCompletion(string questId)
        {
            ActiveQuest active = GetActiveQuest(questId);
            if (active == null) return;

            bool allComplete = true;
            foreach (var obj in active.objectives)
            {
                if (!obj.isCompleted)
                {
                    allComplete = false;
                    break;
                }
            }

            if (allComplete)
            {
                CompleteQuest(questId);
            }
        }

        public void CompleteQuest(string questId)
        {
            ActiveQuest active = GetActiveQuest(questId);
            if (active == null) return;

            Quest quest = FindQuest(questId);
            if (quest == null) return;

            // Grant rewards
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddCredits(quest.creditReward);
            }

            if (ProgressionManager.Instance != null)
            {
                ProgressionManager.Instance.AddExperience(quest.experienceReward);
            }

            if (FactionManager.Instance != null && !string.IsNullOrEmpty(quest.reputationFactionId))
            {
                FactionManager.Instance.ModifyReputation(quest.reputationFactionId, quest.reputationReward);
            }

            // Grant item rewards
            if (InventoryManager.Instance != null && quest.itemRewards != null)
            {
                foreach (var item in quest.itemRewards)
                {
                    InventoryManager.Instance.AddItem(item.itemId, item.quantity);
                }
            }

            completedQuests.Add(questId);
            activeQuests.Remove(active);

            OnQuestCompleted?.Invoke(quest);

            // Update achievements
            if (AchievementSystem.Instance != null)
            {
                AchievementSystem.Instance.UpdateStatistic("quests_completed", 1);
            }
        }

        public void FailQuest(string questId)
        {
            ActiveQuest active = GetActiveQuest(questId);
            if (active == null) return;

            Quest quest = FindQuest(questId);
            activeQuests.Remove(active);

            OnQuestFailed?.Invoke(quest);
        }

        public void AbandonQuest(string questId)
        {
            ActiveQuest active = GetActiveQuest(questId);
            if (active != null)
            {
                activeQuests.Remove(active);
            }
        }

        private Quest FindQuest(string questId)
        {
            foreach (var chain in questChains)
            {
                foreach (var quest in chain.quests)
                {
                    if (quest.questId == questId)
                        return quest;
                }
            }
            return null;
        }

        private ActiveQuest GetActiveQuest(string questId)
        {
            foreach (var active in activeQuests)
            {
                if (active.questId == questId)
                    return active;
            }
            return null;
        }

        public bool IsQuestCompleted(string questId)
        {
            return completedQuests.Contains(questId);
        }

        public bool IsQuestActive(string questId)
        {
            return GetActiveQuest(questId) != null;
        }

        public List<Quest> GetAvailableQuests()
        {
            List<Quest> available = new List<Quest>();

            foreach (var chain in questChains)
            {
                foreach (var quest in chain.quests)
                {
                    if (completedQuests.Contains(quest.questId)) continue;
                    if (IsQuestActive(quest.questId)) continue;

                    bool prereqsMet = true;
                    foreach (var prereq in quest.prerequisites)
                    {
                        if (!completedQuests.Contains(prereq))
                        {
                            prereqsMet = false;
                            break;
                        }
                    }

                    if (prereqsMet)
                    {
                        available.Add(quest);
                    }
                }
            }

            return available;
        }

        public QuestChain GetQuestChain(string chainId)
        {
            foreach (var chain in questChains)
            {
                if (chain.chainId == chainId)
                    return chain;
            }
            return null;
        }
    }

    [System.Serializable]
    public class QuestChain
    {
        public string chainId;
        public string chainName;
        public string description;
        public List<Quest> quests = new List<Quest>();
    }

    [System.Serializable]
    public class Quest
    {
        public string questId;
        public string questName;
        [TextArea(2, 4)]
        public string description;
        public QuestType questType;
        public QuestDifficulty difficulty;

        public int creditReward;
        public int experienceReward;
        public string reputationFactionId;
        public int reputationReward;
        public List<ItemReward> itemRewards;

        public List<string> prerequisites;
        public List<QuestObjective> objectives;
        public float timeLimit; // 0 = no limit
    }

    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveId;
        public string description;
        public QuestObjectiveType type;
        public string targetId;
        public int targetCount;
        public bool isOptional;
    }

    [System.Serializable]
    public class ActiveQuest
    {
        public string questId;
        public string questName;
        public float startTime;
        public float timeRemaining;
        public List<QuestObjectiveProgress> objectives;
    }

    [System.Serializable]
    public class QuestObjectiveProgress
    {
        public string objectiveId;
        public int currentCount;
        public bool isCompleted;
    }

    [System.Serializable]
    public class ItemReward
    {
        public string itemId;
        public int quantity;
    }

    public enum QuestType
    {
        Story,
        Side,
        Daily,
        Repeatable
    }

    public enum QuestDifficulty
    {
        Easy,
        Normal,
        Hard,
        Extreme,
        Legendary
    }

    public enum QuestObjectiveType
    {
        KillEnemies,
        KillTarget,
        DockAtStation,
        TalkToNPC,
        DeliverItem,
        PurchaseItem,
        SellItem,
        CollectItem,
        EarnCredits,
        ReachLocation,
        ScanObject,
        EscortShip,
        ProtectTarget,
        SurviveTime,
        MineAsteroid
    }
}
