using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthUnderFreelancer.Story
{
    /// <summary>
    /// Complete story campaign system with branching narratives, cinematics, and player choices
    /// Supports main story, side stories, faction stories, and dynamic world events
    /// </summary>
    public class StoryCampaignSystem : MonoBehaviour
    {
        public static StoryCampaignSystem Instance { get; private set; }

        [Header("Campaign Settings")]
        [SerializeField] private StoryChapter[] mainCampaign;
        [SerializeField] private FactionCampaign[] factionCampaigns;
        [SerializeField] private bool autoSaveOnChapterComplete = true;

        [Header("Dialogue Settings")]
        [SerializeField] private float textSpeed = 0.05f;
        [SerializeField] private float autoAdvanceDelay = 3f;
        [SerializeField] private bool skipCinematicsAllowed = true;

        // State
        private int currentChapterIndex;
        private int currentMissionIndex;
        private StoryChapter currentChapter;
        private StoryMission currentMission;
        private Dictionary<string, bool> storyFlags = new Dictionary<string, bool>();
        private Dictionary<string, int> storyVariables = new Dictionary<string, int>();
        private List<string> completedChapters = new List<string>();
        private List<string> completedMissions = new List<string>();
        private List<StoryChoice> playerChoices = new List<StoryChoice>();

        // Events
        public event Action<StoryChapter> OnChapterStarted;
        public event Action<StoryChapter> OnChapterCompleted;
        public event Action<StoryMission> OnMissionStarted;
        public event Action<StoryMission> OnMissionCompleted;
        public event Action<DialogueLine> OnDialogueStarted;
        public event Action OnDialogueEnded;
        public event Action<CinematicData> OnCinematicStarted;
        public event Action OnCinematicEnded;
        public event Action<StoryChoice> OnChoiceMade;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeCampaign();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeCampaign()
        {
            if (mainCampaign == null || mainCampaign.Length == 0)
            {
                mainCampaign = CreateDefaultCampaign();
            }

            if (factionCampaigns == null || factionCampaigns.Length == 0)
            {
                factionCampaigns = CreateDefaultFactionCampaigns();
            }
        }

        private StoryChapter[] CreateDefaultCampaign()
        {
            return new StoryChapter[]
            {
                // PROLOGUE
                new StoryChapter
                {
                    chapterId = "prologue",
                    chapterName = "Prologue: Wings of Destiny",
                    description = "The year is 1942. The world is at war, and you are a rookie pilot.",
                    missions = new StoryMission[]
                    {
                        new StoryMission
                        {
                            missionId = "prologue_01",
                            missionName = "First Flight",
                            description = "Complete basic flight training.",
                            objectives = new string[] { "Take off", "Fly through rings", "Land safely" },
                            rewardCredits = 500,
                            rewardXP = 100
                        },
                        new StoryMission
                        {
                            missionId = "prologue_02",
                            missionName = "Combat Training",
                            description = "Learn aerial combat basics.",
                            objectives = new string[] { "Destroy drones", "Evade attacks", "Score hits" },
                            rewardCredits = 750,
                            rewardXP = 150
                        },
                        new StoryMission
                        {
                            missionId = "prologue_03",
                            missionName = "The Call to War",
                            description = "First real mission - escort a convoy.",
                            objectives = new string[] { "Rendezvous with convoy", "Protect convoy", "Ensure survival" },
                            hasCinematic = true,
                            rewardCredits = 1000,
                            rewardXP = 250
                        }
                    }
                },

                // CHAPTER 1
                new StoryChapter
                {
                    chapterId = "chapter_1",
                    chapterName = "Chapter 1: Storm Over Europe",
                    description = "Face the might of the Luftwaffe in the European theater.",
                    prerequisiteChapterId = "prologue",
                    missions = new StoryMission[]
                    {
                        new StoryMission
                        {
                            missionId = "ch1_01",
                            missionName = "Channel Crossing",
                            description = "Escort bombers across the English Channel.",
                            rewardCredits = 1500,
                            rewardXP = 300
                        },
                        new StoryMission
                        {
                            missionId = "ch1_02",
                            missionName = "Ace Hunter",
                            description = "Find and defeat an enemy ace.",
                            hasChoice = true,
                            choiceId = "spare_ace",
                            rewardCredits = 2000,
                            rewardXP = 500
                        },
                        new StoryMission
                        {
                            missionId = "ch1_03",
                            missionName = "The Blitz",
                            description = "Defend London from a massive air raid.",
                            isBossMission = true,
                            rewardCredits = 3000,
                            rewardXP = 750
                        }
                    }
                },

                // CHAPTER 2
                new StoryChapter
                {
                    chapterId = "chapter_2",
                    chapterName = "Chapter 2: Pacific Storm",
                    description = "Carrier operations and island hopping in the Pacific.",
                    prerequisiteChapterId = "chapter_1",
                    missions = new StoryMission[]
                    {
                        new StoryMission
                        {
                            missionId = "ch2_01",
                            missionName = "Carrier Ops",
                            description = "Learn carrier procedures.",
                            rewardCredits = 1500,
                            rewardXP = 300
                        },
                        new StoryMission
                        {
                            missionId = "ch2_02",
                            missionName = "Island Assault",
                            description = "Provide air support for marine landing.",
                            rewardCredits = 2000,
                            rewardXP = 400
                        },
                        new StoryMission
                        {
                            missionId = "ch2_03",
                            missionName = "Divine Wind",
                            description = "Face a massive kamikaze attack.",
                            isBossMission = true,
                            rewardCredits = 4000,
                            rewardXP = 1000
                        }
                    }
                },

                // CHAPTER 3
                new StoryChapter
                {
                    chapterId = "chapter_3",
                    chapterName = "Chapter 3: Jet Age",
                    description = "Welcome to the jet age and the Korean conflict.",
                    prerequisiteChapterId = "chapter_2",
                    missions = new StoryMission[]
                    {
                        new StoryMission
                        {
                            missionId = "ch3_01",
                            missionName = "New Wings",
                            description = "Train on jet fighters.",
                            unlocksAircraft = "F-86 Sabre",
                            rewardCredits = 2000,
                            rewardXP = 400
                        },
                        new StoryMission
                        {
                            missionId = "ch3_02",
                            missionName = "MiG Alley",
                            description = "Patrol dangerous Korean skies.",
                            rewardCredits = 2500,
                            rewardXP = 500
                        },
                        new StoryMission
                        {
                            missionId = "ch3_03",
                            missionName = "Bridge Too Far",
                            description = "Strike mission to destroy supply bridge.",
                            hasChoice = true,
                            choiceId = "save_wingman",
                            rewardCredits = 3500,
                            rewardXP = 700
                        }
                    }
                },

                // CHAPTER 4
                new StoryChapter
                {
                    chapterId = "chapter_4",
                    chapterName = "Chapter 4: Modern Warriors",
                    description = "Advanced technology in modern conflicts.",
                    prerequisiteChapterId = "chapter_3",
                    missions = new StoryMission[]
                    {
                        new StoryMission
                        {
                            missionId = "ch4_01",
                            missionName = "Top Gun",
                            description = "Complete advanced fighter weapons school.",
                            unlocksAircraft = "F-16 Fighting Falcon",
                            rewardCredits = 5000,
                            rewardXP = 1000
                        },
                        new StoryMission
                        {
                            missionId = "ch4_02",
                            missionName = "Desert Storm",
                            description = "Liberation campaign operations.",
                            rewardCredits = 6000,
                            rewardXP = 1200
                        },
                        new StoryMission
                        {
                            missionId = "ch4_03",
                            missionName = "Final Showdown",
                            description = "Face the ultimate enemy in a battle for air supremacy.",
                            isBossMission = true,
                            isFinalMission = true,
                            rewardCredits = 10000,
                            rewardXP = 2500
                        }
                    }
                }
            };
        }

        private FactionCampaign[] CreateDefaultFactionCampaigns()
        {
            return new FactionCampaign[]
            {
                new FactionCampaign
                {
                    factionId = "federation",
                    factionName = "United Federation",
                    description = "Unite nations under peace and cooperation.",
                    requiredReputation = 1000
                },
                new FactionCampaign
                {
                    factionId = "empire",
                    factionName = "Imperial Dominion",
                    description = "Strength through unity and authority.",
                    requiredReputation = 1000
                },
                new FactionCampaign
                {
                    factionId = "rebels",
                    factionName = "Free Pilots Alliance",
                    description = "Fight for freedom above all.",
                    requiredReputation = 500
                },
                new FactionCampaign
                {
                    factionId = "traders",
                    factionName = "Merchant Guild",
                    description = "Commerce is civilization's lifeblood.",
                    requiredReputation = 250
                }
            };
        }

        public void StartCampaign()
        {
            if (mainCampaign != null && mainCampaign.Length > 0)
            {
                StartChapter(mainCampaign[0]);
            }
        }

        public void StartChapter(StoryChapter chapter)
        {
            if (chapter == null) return;

            if (!string.IsNullOrEmpty(chapter.prerequisiteChapterId))
            {
                if (!completedChapters.Contains(chapter.prerequisiteChapterId))
                {
                    return;
                }
            }

            currentChapter = chapter;
            currentChapterIndex = Array.IndexOf(mainCampaign, chapter);
            currentMissionIndex = 0;

            OnChapterStarted?.Invoke(chapter);

            if (chapter.missions != null && chapter.missions.Length > 0)
            {
                StartMission(chapter.missions[0]);
            }
        }

        public void StartMission(StoryMission mission)
        {
            if (mission == null) return;

            currentMission = mission;
            OnMissionStarted?.Invoke(mission);
        }

        public void CompleteMission(bool success)
        {
            if (currentMission == null) return;

            if (success)
            {
                completedMissions.Add(currentMission.missionId);
                OnMissionCompleted?.Invoke(currentMission);

                currentMissionIndex++;
                if (currentChapter != null && currentMissionIndex < currentChapter.missions.Length)
                {
                    StartMission(currentChapter.missions[currentMissionIndex]);
                }
                else
                {
                    CompleteChapter();
                }
            }
        }

        private void CompleteChapter()
        {
            if (currentChapter == null) return;

            completedChapters.Add(currentChapter.chapterId);
            OnChapterCompleted?.Invoke(currentChapter);

            if (autoSaveOnChapterComplete)
            {
                SaveProgress();
            }

            currentChapterIndex++;
            if (currentChapterIndex < mainCampaign.Length)
            {
                StartChapter(mainCampaign[currentChapterIndex]);
            }
        }

        public void MakeChoice(string choiceId, int optionIndex)
        {
            var choice = new StoryChoice
            {
                choiceId = choiceId,
                optionChosen = optionIndex,
                timestamp = DateTime.Now
            };

            playerChoices.Add(choice);
            storyFlags[$"choice_{choiceId}_{optionIndex}"] = true;
            OnChoiceMade?.Invoke(choice);
        }

        public void SetStoryFlag(string flagName, bool value)
        {
            storyFlags[flagName] = value;
        }

        public bool GetStoryFlag(string flagName)
        {
            return storyFlags.TryGetValue(flagName, out bool value) && value;
        }

        public float GetCampaignProgress()
        {
            if (mainCampaign == null || mainCampaign.Length == 0) return 0f;
            int totalMissions = mainCampaign.Sum(c => c.missions?.Length ?? 0);
            return totalMissions > 0 ? (float)completedMissions.Count / totalMissions : 0f;
        }

        public StoryChapter GetCurrentChapter() => currentChapter;
        public StoryMission GetCurrentMission() => currentMission;
        public StoryChapter[] GetAllChapters() => mainCampaign;

        public void SaveProgress()
        {
            PlayerPrefs.SetString("StoryProgress", string.Join(",", completedMissions));
            PlayerPrefs.Save();
        }

        public void LoadProgress()
        {
            if (PlayerPrefs.HasKey("StoryProgress"))
            {
                var saved = PlayerPrefs.GetString("StoryProgress");
                completedMissions = new List<string>(saved.Split(','));
            }
        }
    }

    [Serializable]
    public class StoryChapter
    {
        public string chapterId;
        public string chapterName;
        public string description;
        public string prerequisiteChapterId;
        public StoryMission[] missions;
        public bool isEpilogue;
    }

    [Serializable]
    public class StoryMission
    {
        public string missionId;
        public string missionName;
        public string description;
        public string[] objectives;
        public bool hasCinematic;
        public bool hasChoice;
        public string choiceId;
        public bool isBossMission;
        public bool isFinalMission;
        public string unlocksAircraft;
        public int rewardCredits;
        public int rewardXP;
        public int factionReputationChange;
    }

    [Serializable]
    public class DialogueLine
    {
        public string speakerId;
        public string speakerName;
        public string text;
        public DialogueEmotion emotion;
    }

    public enum DialogueEmotion
    {
        Neutral, Happy, Sad, Angry, Surprised, Scared, Serious, Determined, Encouraging
    }

    [Serializable]
    public class CinematicData
    {
        public string cinematicId;
        public float duration;
        public bool skippable;
    }

    [Serializable]
    public class FactionCampaign
    {
        public string factionId;
        public string factionName;
        public string description;
        public int requiredReputation;
        public StoryChapter[] chapters;
    }

    [Serializable]
    public class StoryChoice
    {
        public string choiceId;
        public int optionChosen;
        public DateTime timestamp;
    }
}
