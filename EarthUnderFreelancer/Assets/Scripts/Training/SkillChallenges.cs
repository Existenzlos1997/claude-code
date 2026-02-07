using UnityEngine;

namespace EarthUnderFreelancer.Training
{
    // Iteration 19-20: Skill-based challenges
    public class SkillChallenges : MonoBehaviour
    {
        public enum ChallengeType { TimeAttack, Accuracy, Survival, Formation }
        
        private ChallengeType currentChallenge;
        private float challengeTimer = 0f;
        private bool challengeActive = false;
        
        public void StartChallenge(ChallengeType type)
        {
            currentChallenge = type;
            challengeActive = true;
            challengeTimer = 0f;
            Debug.Log($"[Challenge] Started: {type}");
        }
        
        private void Update()
        {
            if (challengeActive)
            {
                challengeTimer += Time.deltaTime;
            }
        }
        
        public void CompleteChallenge()
        {
            challengeActive = false;
            Debug.Log($"[Challenge] Completed in {challengeTimer:F2}s");
        }
    }
}
