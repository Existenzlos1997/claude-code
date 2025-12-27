using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Optimization
{
    // Iteration 46-48: Object pooling for performance
    public class ObjectPooling : MonoBehaviour
    {
        private Queue<GameObject> pool = new Queue<GameObject>();
        
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 10;
        
        private void Start()
        {
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
        
        public GameObject Get()
        {
            if (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            return Instantiate(prefab);
        }
        
        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}
