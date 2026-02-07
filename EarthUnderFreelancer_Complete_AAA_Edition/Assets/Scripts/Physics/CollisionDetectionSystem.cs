using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Physics
{
    public class CollisionDetectionSystem : MonoBehaviour
    {
        private static CollisionDetectionSystem instance;
        public static CollisionDetectionSystem Instance => instance;
        
        public int gridSize = 100;
        public float cellSize = 100f;
        private Dictionary<Vector2Int, List<Collider>> spatialGrid = new Dictionary<Vector2Int, List<Collider>>();
        
        private void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
        
        public void RegisterCollider(Collider col)
        {
            Vector2Int cell = GetCell(col.transform.position);
            if (!spatialGrid.ContainsKey(cell)) spatialGrid[cell] = new List<Collider>();
            if (!spatialGrid[cell].Contains(col)) spatialGrid[cell].Add(col);
        }
        
        public List<Collider> GetNearbyColliders(Vector3 position, float radius)
        {
            List<Collider> nearby = new List<Collider>();
            Vector2Int center = GetCell(position);
            int range = Mathf.CeilToInt(radius / cellSize);
            
            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    Vector2Int cell = center + new Vector2Int(x, z);
                    if (spatialGrid.ContainsKey(cell))
                    {
                        foreach (var col in spatialGrid[cell])
                        {
                            if (col != null && Vector3.Distance(position, col.transform.position) <= radius)
                                nearby.Add(col);
                        }
                    }
                }
            }
            return nearby;
        }
        
        private Vector2Int GetCell(Vector3 pos) { return new Vector2Int(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.z / cellSize)); }
    }
}
