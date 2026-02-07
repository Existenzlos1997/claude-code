using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Object pooling system for efficient object management
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int initialSize;
            public bool expandable = true;
        }

        [SerializeField] private List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<string, Pool> poolConfigs;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            poolConfigs = new Dictionary<string, Pool>();

            if (pools == null) return;

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                poolConfigs[pool.tag] = pool;

                for (int i = 0; i < pool.initialSize; i++)
                {
                    GameObject obj = CreateNewObject(pool.prefab);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        private GameObject CreateNewObject(GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            return obj;
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            Queue<GameObject> pool = poolDictionary[tag];
            GameObject objectToSpawn;

            if (pool.Count == 0)
            {
                if (poolConfigs[tag].expandable)
                {
                    objectToSpawn = CreateNewObject(poolConfigs[tag].prefab);
                }
                else
                {
                    Debug.LogWarning($"Pool {tag} is empty and not expandable.");
                    return null;
                }
            }
            else
            {
                objectToSpawn = pool.Dequeue();
            }

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.transform.SetParent(null);

            IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
            pooledObject?.OnObjectSpawn();

            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                Destroy(obj);
                return;
            }

            IPooledObject pooledObject = obj.GetComponent<IPooledObject>();
            pooledObject?.OnObjectDespawn();

            obj.SetActive(false);
            obj.transform.SetParent(transform);
            poolDictionary[tag].Enqueue(obj);
        }

        public void CreatePool(string tag, GameObject prefab, int initialSize, bool expandable = true)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} already exists.");
                return;
            }

            Pool newPool = new Pool
            {
                tag = tag,
                prefab = prefab,
                initialSize = initialSize,
                expandable = expandable
            };

            Queue<GameObject> objectPool = new Queue<GameObject>();
            poolConfigs[tag] = newPool;

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = CreateNewObject(prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(tag, objectPool);
        }

        public void ClearPool(string tag)
        {
            if (!poolDictionary.ContainsKey(tag)) return;

            Queue<GameObject> pool = poolDictionary[tag];
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Destroy(obj);
            }
        }

        public void ClearAllPools()
        {
            foreach (var kvp in poolDictionary)
            {
                while (kvp.Value.Count > 0)
                {
                    GameObject obj = kvp.Value.Dequeue();
                    Destroy(obj);
                }
            }
        }
    }

    public interface IPooledObject
    {
        void OnObjectSpawn();
        void OnObjectDespawn();
    }
}
