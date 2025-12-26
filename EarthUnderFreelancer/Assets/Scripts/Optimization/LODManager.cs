using UnityEngine;

namespace EarthUnderFreelancer.Optimization
{
    // Iteration 49-50: Level of Detail management
    public class LODManager : MonoBehaviour
    {
        [SerializeField] private float[] lodDistances = { 100f, 500f, 1000f };
        private int currentLOD = 0;
        
        private void Update()
        {
            float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            UpdateLOD(distance);
        }
        
        private void UpdateLOD(float distance)
        {
            int newLOD = 0;
            for (int i = 0; i < lodDistances.Length; i++)
            {
                if (distance > lodDistances[i]) newLOD = i + 1;
            }
            
            if (newLOD != currentLOD)
            {
                currentLOD = newLOD;
            }
        }
    }
}
