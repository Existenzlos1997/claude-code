using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EarthUnderFreelancer.World
{
    /// <summary>
    /// Procedural terrain generator with chunk-based streaming, multiple biomes, and LOD system.
    /// Generates massive worlds efficiently using Perlin noise and async loading.
    /// </summary>
    public class ProceduralTerrainGenerator : MonoBehaviour
    {
        [Header("World Settings")]
        public int seed = 12345;
        public float worldScale = 1000f; // km
        public float heightScale = 500f; // max height in meters
        
        [Header("Chunk Settings")]
        public int chunkSize = 256; // meters per chunk
        public int viewDistance = 4; // chunks
        public int maxChunksPerFrame = 2;
        
        [Header("Terrain Generation")]
        public float noiseScale = 0.01f;
        public int octaves = 4;
        public float persistence = 0.5f;
        public float lacunarity = 2f;
        
        [Header("Biomes")]
        public BiomeDefinition[] biomes = new BiomeDefinition[]
        {
            new BiomeDefinition { name = "Ocean", minHeight = 0f, maxHeight = 0.3f, color = new Color(0.2f, 0.4f, 0.8f) },
            new BiomeDefinition { name = "Plains", minHeight = 0.3f, maxHeight = 0.5f, color = new Color(0.4f, 0.7f, 0.3f) },
            new BiomeDefinition { name = "Forest", minHeight = 0.5f, maxHeight = 0.6f, color = new Color(0.2f, 0.5f, 0.2f) },
            new BiomeDefinition { name = "Mountains", minHeight = 0.6f, maxHeight = 0.8f, color = new Color(0.5f, 0.5f, 0.5f) },
            new BiomeDefinition { name = "Snow Peaks", minHeight = 0.8f, maxHeight = 1f, color = Color.white }
        };
        
        [Header("LOD Levels")]
        public int[] lodLevels = new int[] { 1, 2, 4, 8 }; // Vertex skip amounts
        
        // Internal data
        private Dictionary<Vector2Int, TerrainChunk> activeChunks = new Dictionary<Vector2Int, TerrainChunk>();
        private Queue<Vector2Int> chunksToGenerate = new Queue<Vector2Int>();
        private Transform playerTransform;
        private Vector2Int currentPlayerChunk;
        
        [System.Serializable]
        public class BiomeDefinition
        {
            public string name;
            public float minHeight;
            public float maxHeight;
            public Color color;
        }
        
        private class TerrainChunk
        {
            public GameObject gameObject;
            public MeshFilter meshFilter;
            public MeshRenderer meshRenderer;
            public MeshCollider meshCollider;
            public Vector2Int coordinates;
            public int currentLOD;
        }
        
        private void Start()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                playerTransform = Camera.main.transform;
            }
            
            Random.InitState(seed);
        }
        
        private void Update()
        {
            if (playerTransform == null) return;
            
            // Calculate player's current chunk
            Vector2Int playerChunk = GetChunkCoordinate(playerTransform.position);
            
            // Update chunks if player moved to new chunk
            if (playerChunk != currentPlayerChunk)
            {
                currentPlayerChunk = playerChunk;
                UpdateChunks();
            }
            
            // Generate chunks from queue
            GenerateQueuedChunks();
            
            // Update LOD for existing chunks
            UpdateChunkLOD();
        }
        
        /// <summary>
        /// Get chunk coordinate from world position
        /// </summary>
        private Vector2Int GetChunkCoordinate(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / chunkSize);
            int z = Mathf.FloorToInt(worldPosition.z / chunkSize);
            return new Vector2Int(x, z);
        }
        
        /// <summary>
        /// Update visible chunks around player
        /// </summary>
        private void UpdateChunks()
        {
            HashSet<Vector2Int> requiredChunks = new HashSet<Vector2Int>();
            
            // Determine which chunks should be visible
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                for (int z = -viewDistance; z <= viewDistance; z++)
                {
                    Vector2Int chunkCoord = currentPlayerChunk + new Vector2Int(x, z);
                    requiredChunks.Add(chunkCoord);
                    
                    // Queue for generation if not exists
                    if (!activeChunks.ContainsKey(chunkCoord) && !chunksToGenerate.Contains(chunkCoord))
                    {
                        chunksToGenerate.Enqueue(chunkCoord);
                    }
                }
            }
            
            // Remove chunks outside view distance
            List<Vector2Int> chunksToRemove = new List<Vector2Int>();
            foreach (var kvp in activeChunks)
            {
                if (!requiredChunks.Contains(kvp.Key))
                {
                    chunksToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var coord in chunksToRemove)
            {
                DestroyChunk(coord);
            }
        }
        
        /// <summary>
        /// Generate chunks from queue (limit per frame)
        /// </summary>
        private void GenerateQueuedChunks()
        {
            int generated = 0;
            while (chunksToGenerate.Count > 0 && generated < maxChunksPerFrame)
            {
                Vector2Int coord = chunksToGenerate.Dequeue();
                GenerateChunk(coord);
                generated++;
            }
        }
        
        /// <summary>
        /// Generate a terrain chunk
        /// </summary>
        private void GenerateChunk(Vector2Int coordinates)
        {
            if (activeChunks.ContainsKey(coordinates)) return;
            
            // Create chunk GameObject
            GameObject chunkObj = new GameObject($"Chunk_{coordinates.x}_{coordinates.y}");
            chunkObj.transform.parent = transform;
            chunkObj.transform.position = new Vector3(coordinates.x * chunkSize, 0, coordinates.y * chunkSize);
            
            // Add components
            MeshFilter meshFilter = chunkObj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = chunkObj.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = chunkObj.AddComponent<MeshCollider>();
            
            // Create chunk data
            TerrainChunk chunk = new TerrainChunk
            {
                gameObject = chunkObj,
                meshFilter = meshFilter,
                meshRenderer = meshRenderer,
                meshCollider = meshCollider,
                coordinates = coordinates,
                currentLOD = 0
            };
            
            // Generate mesh (start at highest LOD)
            GenerateChunkMesh(chunk, 0);
            
            // Simple material
            Material mat = new Material(Shader.Find("Standard"));
            meshRenderer.sharedMaterial = mat;
            
            activeChunks[coordinates] = chunk;
        }
        
        /// <summary>
        /// Generate mesh for chunk at specific LOD
        /// </summary>
        private void GenerateChunkMesh(TerrainChunk chunk, int lodLevel)
        {
            int resolution = chunkSize + 1;
            int skip = lodLevels[Mathf.Min(lodLevel, lodLevels.Length - 1)];
            int vertexCount = (resolution / skip) + 1;
            
            Vector3[] vertices = new Vector3[vertexCount * vertexCount];
            Color[] colors = new Color[vertices.Length];
            int[] triangles = new int[(vertexCount - 1) * (vertexCount - 1) * 6];
            Vector2[] uvs = new Vector2[vertices.Length];
            
            int vertIndex = 0;
            int triIndex = 0;
            
            // Generate vertices
            for (int z = 0; z < vertexCount; z++)
            {
                for (int x = 0; x < vertexCount; x++)
                {
                    float worldX = chunk.coordinates.x * chunkSize + (x * skip);
                    float worldZ = chunk.coordinates.y * chunkSize + (z * skip);
                    
                    float height = GenerateHeight(worldX, worldZ);
                    vertices[vertIndex] = new Vector3(x * skip, height * heightScale, z * skip);
                    
                    // Assign biome color
                    colors[vertIndex] = GetBiomeColor(height);
                    
                    uvs[vertIndex] = new Vector2((float)x / (vertexCount - 1), (float)z / (vertexCount - 1));
                    
                    // Generate triangles (skip edges)
                    if (x < vertexCount - 1 && z < vertexCount - 1)
                    {
                        int topLeft = vertIndex;
                        int topRight = vertIndex + 1;
                        int bottomLeft = vertIndex + vertexCount;
                        int bottomRight = vertIndex + vertexCount + 1;
                        
                        triangles[triIndex++] = topLeft;
                        triangles[triIndex++] = bottomLeft;
                        triangles[triIndex++] = topRight;
                        
                        triangles[triIndex++] = topRight;
                        triangles[triIndex++] = bottomLeft;
                        triangles[triIndex++] = bottomRight;
                    }
                    
                    vertIndex++;
                }
            }
            
            // Create mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors = colors;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            
            chunk.meshFilter.mesh = mesh;
            chunk.meshCollider.sharedMesh = mesh;
            chunk.currentLOD = lodLevel;
        }
        
        /// <summary>
        /// Generate height value using multi-octave Perlin noise
        /// </summary>
        private float GenerateHeight(float x, float z)
        {
            float height = 0f;
            float amplitude = 1f;
            float frequency = 1f;
            
            for (int i = 0; i < octaves; i++)
            {
                float sampleX = x * noiseScale * frequency;
                float sampleZ = z * noiseScale * frequency;
                
                float perlinValue = Mathf.PerlinNoise(sampleX + seed, sampleZ + seed);
                height += perlinValue * amplitude;
                
                amplitude *= persistence;
                frequency *= lacunarity;
            }
            
            // Normalize to 0-1
            height = Mathf.Clamp01(height / 2f);
            return height;
        }
        
        /// <summary>
        /// Get biome color based on height
        /// </summary>
        private Color GetBiomeColor(float height)
        {
            foreach (var biome in biomes)
            {
                if (height >= biome.minHeight && height < biome.maxHeight)
                {
                    return biome.color;
                }
            }
            return Color.gray;
        }
        
        /// <summary>
        /// Update LOD for chunks based on distance
        /// </summary>
        private void UpdateChunkLOD()
        {
            foreach (var kvp in activeChunks)
            {
                TerrainChunk chunk = kvp.Value;
                float distance = Vector2.Distance(
                    new Vector2(playerTransform.position.x, playerTransform.position.z),
                    new Vector2(chunk.coordinates.x * chunkSize, chunk.coordinates.y * chunkSize)
                );
                
                int targetLOD = GetLODForDistance(distance);
                if (targetLOD != chunk.currentLOD)
                {
                    GenerateChunkMesh(chunk, targetLOD);
                }
            }
        }
        
        /// <summary>
        /// Determine LOD level based on distance
        /// </summary>
        private int GetLODForDistance(float distance)
        {
            if (distance < chunkSize * 2) return 0;
            if (distance < chunkSize * 4) return 1;
            if (distance < chunkSize * 6) return 2;
            return 3;
        }
        
        /// <summary>
        /// Destroy a chunk
        /// </summary>
        private void DestroyChunk(Vector2Int coordinates)
        {
            if (activeChunks.TryGetValue(coordinates, out TerrainChunk chunk))
            {
                Destroy(chunk.gameObject);
                activeChunks.Remove(coordinates);
            }
        }
    }
}
