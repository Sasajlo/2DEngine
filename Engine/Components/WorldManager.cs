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
        
        private Tile[,] _tiles;
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
            // Collect visible tiles with their sorting order
            var visibleTiles = new List<Tile>();
            
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    var tile = _tiles[x, y];
                    if (tile != null && tile.isVisible)
                    {
                        visibleTiles.Add(tile);
                    }
                }
            }
            
            // Sort tiles by sorting order for proper layering
            var sortedTiles = visibleTiles.OrderBy(tile => tile.sortingOrder).ToArray();
            
            // Render each visible tile
            foreach (var tile in sortedTiles)
            {
                RenderTile(tile);
            }
        }
        
        private void RenderTile(Tile tile)
        {
            if (tile.tileData == null)
                return;
                
            // Create transform matrix for the tile
            var transform = Matrix4x4.CreateScale(tile.scale) * 
                           Matrix4x4.CreateTranslation(tile.position);
            
            // Convert Matrix4x4 to float array
            var matrixArray = new float[16]
            {
                transform.M11, transform.M12, transform.M13, transform.M14,
                transform.M21, transform.M22, transform.M23, transform.M24,
                transform.M31, transform.M32, transform.M33, transform.M34,
                transform.M41, transform.M42, transform.M43, transform.M44
            };
            
            // Convert color to float array
            var color = tile.tileData.color.ToVector4();
            var colorArray = new float[4] { color.X, color.Y, color.Z, color.W };
            
            // Set default size (1x1 for tiles)
            var sizeArray = new float[2] { 1.0f, 1.0f };
            
            // Call C++ rendering function
            EngineCore.RenderSprite(
                tile.tileData.texture,
                matrixArray,
                colorArray,
                sizeArray,
                tile.sortingOrder,
                false, // flipX
                false  // flipY
            );
        }
        
        private void GenerateWorld()
        {
            Console.WriteLine($"Generating world: {worldWidth}x{worldHeight} tiles...");
            
            _tiles = new Tile[worldWidth, worldHeight];
            
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    CreateTile(x, y);
                }
            }
            
            Console.WriteLine("World generation completed.");
        }
        
        private void CreateTile(int gridX, int gridY)
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
            
            _tiles[gridX, gridY] = tile;
        }
        
        public Tile GetTileAt(int gridX, int gridY)
        {
            if (gridX >= 0 && gridX < worldWidth && gridY >= 0 && gridY < worldHeight)
            {
                return _tiles[gridX, gridY];
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
                
            var tile = _tiles[gridX, gridY];
            if (tile != null)
            {
                tile.LoadTileType(tileType);
            }
        }
    }
} 