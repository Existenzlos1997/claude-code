using UnityEngine;

namespace EarthUnderFreelancer.Training
{
    // Iteration 18: Training dummy for target practice
    public class TrainingDummy : MonoBehaviour
    {
        private float health = 100f;
        private int hits = 0;
        
        public void TakeDamage(float amount)
        {
            health -= amount;
            hits++;
            
            if (health <= 0)
            {
                health = 100f;
                Debug.Log($"[Training] Dummy destroyed! Hits: {hits}");
                hits = 0;
            }
        }
    }
}
