using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Engine.Components
{
    public class WorldManager : Component
    {
        public int worldWidth { get; set; } = 100;
        public int worldHeight { get; set; } = 100;
        public float tileSize { get; set; } = 1.0f;
        
        // Chunk system
        private Dictionary<Vector2, Chunk> _chunks;
        private int _chunksX;
        private int _chunksY;
        private float _cullMargin = 2.0f; // Margin for smoother culling
        
        private Scene _scene;
        
        public override void Awake()
        {
            _scene = gameObject.scene;
        }

        public override void Start()
        {
            GenerateWorld();
        }
        
        // Called by the rendering system to render tiles
        public void Render()
        {
            // Get main camera for culling
            var camera = Camera.mainCamera;
            if (camera == null)
                return;
            
            var cameraTransform = camera.gameObject.transform;
            if (cameraTransform == null)
                return;
            
            // Get visible chunks using viewport culling
            var visibleChunks = ViewportCuller.GetVisibleChunks(_chunks, camera, cameraTransform, _cullMargin);
            
            // Update chunk visibility
            foreach (var chunk in _chunks.Values)
            {
                chunk.SetVisible(false);
            }
            
            foreach (var chunk in visibleChunks)
            {
                chunk.SetVisible(true);
            }
            

            
            // Render only visible chunks
            foreach (var chunk in visibleChunks)
            {
                chunk.Render();
            }
            
            // Let DebugSystem handle debug rendering
            if (DebugSystem.IsDebugMode)
            {
                DebugSystem.RenderChunkGrid(this);
                DebugSystem.LogChunkInfo(visibleChunks.Count, _chunks.Count);
            }
        }
        
        /// <summary>
        /// Get chunk at specified chunk coordinates
        /// </summary>
        public Chunk GetChunkAt(int chunkX, int chunkY)
        {
            var key = new Vector2(chunkX, chunkY);
            return _chunks.ContainsKey(key) ? _chunks[key] : null;
        }
        
        /// <summary>
        /// Get chunk that contains the specified world position
        /// </summary>
        public Chunk GetChunkAtWorldPosition(Vector3 worldPosition)
        {
            float chunkWorldSize = Chunk.CHUNK_SIZE * tileSize;
            int chunkX = (int)Math.Floor(worldPosition.X / chunkWorldSize);
            int chunkY = (int)Math.Floor(worldPosition.Y / chunkWorldSize);
            return GetChunkAt(chunkX, chunkY);
        }
        
        private void GenerateWorld()
        {
            Console.WriteLine($"Generating world: {worldWidth}x{worldHeight} tiles...");
            
            // Calculate number of chunks needed
            _chunksX = (int)Math.Ceiling((float)worldWidth / Chunk.CHUNK_SIZE);
            _chunksY = (int)Math.Ceiling((float)worldHeight / Chunk.CHUNK_SIZE);
            
            Console.WriteLine($"Creating {_chunksX}x{_chunksY} chunks ({_chunksX * _chunksY} total)...");
            
            _chunks = new Dictionary<Vector2, Chunk>();
            
            // Generate chunks
            for (int chunkX = 0; chunkX < _chunksX; chunkX++)
            {
                for (int chunkY = 0; chunkY < _chunksY; chunkY++)
                {
                    var chunk = new Chunk(chunkX, chunkY, tileSize);
                    _chunks[new Vector2(chunkX, chunkY)] = chunk;
                    
                    // Generate tiles for this chunk
                    GenerateChunkTiles(chunk);
                }
            }
            
            Console.WriteLine("World generation completed.");
        }
        
        private void GenerateChunkTiles(Chunk chunk)
        {
            // Generate tiles for this chunk
            for (int localX = 0; localX < Chunk.CHUNK_SIZE; localX++)
            {
                for (int localY = 0; localY < Chunk.CHUNK_SIZE; localY++)
                {
                    // Calculate global grid position
                    int globalX = chunk.chunkX * Chunk.CHUNK_SIZE + localX;
                    int globalY = chunk.chunkY * Chunk.CHUNK_SIZE + localY;
                    
                    // Only create tiles that are within world bounds
                    if (globalX < worldWidth && globalY < worldHeight)
                    {
                        var tile = CreateTile(globalX, globalY);
                        chunk.SetTile(localX, localY, tile);
                    }
                }
            }
        }
        
        private Tile CreateTile(int gridX, int gridY)
        {
            // Calculate world position
            float worldX = gridX * tileSize;
            float worldY = gridY * tileSize;
            Vector3 position = new Vector3(worldX, worldY, 0);
            Vector3 scale = new Vector3(tileSize, tileSize, 1);
            
            // Create tile directly
            var tile = new Tile(gridX, gridY, position, scale);
            
            // Determine tile type based on position
            string tileType;
            int surfaceLevel = 0;

            if (gridY < surfaceLevel)
            {
                tileType = "Air";
            }
            else if (gridX == 0 || gridX == worldWidth - 1 || gridY == worldHeight - 1)
            {
                tileType = "Bedrock"; // Border tiles
            }
            else if (gridY == surfaceLevel)
            {
                tileType = "Grass";
            }
            else if (gridY <= surfaceLevel + 5)
            {
                tileType = "Dirt";
            }
            else
            {
                tileType = "Stone"; // Default tile type for deeper layers
            }
            
            // Load tile data
            if (!tile.LoadTileType(tileType))
            {
                Console.WriteLine($"Failed to load tile type {tileType} for tile at ({gridX}, {gridY})");
            }
            
            return tile;
        }
        
        public Tile GetTileAt(int gridX, int gridY)
        {
            if (gridX >= 0 && gridX < worldWidth && gridY >= 0 && gridY < worldHeight)
            {
                // Calculate chunk coordinates
                int chunkX = gridX / Chunk.CHUNK_SIZE;
                int chunkY = gridY / Chunk.CHUNK_SIZE;
                
                // Get the chunk
                var chunk = GetChunkAt(chunkX, chunkY);
                if (chunk != null)
                {
                    // Calculate local coordinates within the chunk
                    int localX = gridX % Chunk.CHUNK_SIZE;
                    int localY = gridY % Chunk.CHUNK_SIZE;
                    
                    return chunk.GetTile(localX, localY);
                }
            }
            return null;
        }
        
        public bool IsValidPosition(int gridX, int gridY)
        {
            return gridX >= 0 && gridX < worldWidth && gridY >= 0 && gridY < worldHeight;
        }
        
        public Vector2 WorldToGrid(Vector3 worldPosition)
        {
            return new Vector2(
                (int)(worldPosition.X / tileSize),
                (int)(worldPosition.Y / tileSize)
            );
        }
        
        public Vector3 GridToWorld(int gridX, int gridY)
        {
            return new Vector3(
                gridX * tileSize,
                gridY * tileSize,
                0
            );
        }
        
        // Method to set a tile at a specific position (for tile modification)
        public void SetTileAt(int gridX, int gridY, string tileType)
        {
            if (!IsValidPosition(gridX, gridY))
                return;
                
            var tile = GetTileAt(gridX, gridY);
            if (tile != null)
            {
                tile.LoadTileType(tileType);
            }
        }
        
        /// <summary>
        /// Get debug information about chunk rendering
        /// </summary>
        public string GetRenderDebugInfo()
        {
            var camera = Camera.mainCamera;
            if (camera == null)
                return "No main camera found";
            
            var cameraTransform = camera.gameObject.transform;
            if (cameraTransform == null)
                return "No camera transform found";
            
            var visibleChunks = ViewportCuller.GetVisibleChunks(_chunks, camera, cameraTransform, _cullMargin);
            
            int totalChunks = _chunks.Count;
            int visibleChunkCount = visibleChunks.Count;
            int totalTiles = totalChunks * Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE;
            int visibleTiles = visibleChunkCount * Chunk.CHUNK_SIZE * Chunk.CHUNK_SIZE;
            
            return $"Chunks: {visibleChunkCount}/{totalChunks} visible | Tiles: {visibleTiles}/{totalTiles} rendered";
        }
        
        /// <summary>
        /// Get chunk world size for debug rendering
        /// </summary>
        public float GetChunkWorldSize()
        {
            return Chunk.CHUNK_SIZE * tileSize;
        }
        
        /// <summary>
        /// Get tile size for debug rendering
        /// </summary>
        public float GetTileSize()
        {
            return tileSize;
        }
        
        /// <summary>
        /// Get all chunk positions for debug rendering
        /// </summary>
        public List<Vector2> GetChunkPositions()
        {
            var positions = new List<Vector2>();
            foreach (var chunk in _chunks.Values)
            {
                positions.Add(chunk.worldPosition);
            }
            return positions;
        }
    }
} 