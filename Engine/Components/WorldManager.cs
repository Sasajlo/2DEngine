using System;
using System.Collections.Generic;
using System.Numerics;

namespace Engine.Components
{
    public class WorldManager : Component
    {
        public int worldWidth { get; set; } = 100;
        public int worldHeight { get; set; } = 100;
        public float tileSize { get; set; } = 1.0f;
        
        private GameObject[,] _tiles;
        private Scene _scene;
        
        public override void Awake()
        {
            _scene = gameObject.scene;
        }

        public override void Start()
        {
            GenerateWorld();
        }
        
        private void GenerateWorld()
        {
            Console.WriteLine($"Generating world: {worldWidth}x{worldHeight} tiles...");
            
            _tiles = new GameObject[worldWidth, worldHeight];
            
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
            // Create tile GameObject
            var tileObject = _scene.CreateGameObject($"Tile_{gridX}_{gridY}");
            
            // Position the tile in world space
            // Using screen coordinates: (0,0) at top-left, with Y increasing downward
            float worldX = gridX * tileSize;
            float worldY = gridY * tileSize;
            tileObject.transform.position = new Vector3(worldX, worldY, 0);
            tileObject.transform.scale = new Vector3(tileSize, tileSize, 1);
            
            // Add required components
            var sprite = tileObject.AddComponent<Sprite>();
            var spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
            var tile = tileObject.AddComponent<Tile>();
            
            // Set tile grid position
            tile.gridX = gridX;
            tile.gridY = gridY;
            
            // Determine tile type based on position
            // In screen coordinates: Y=0 is at top, Y increases downward
            // Top portion = Air (sky)
            // Surface layer = Grass (ground surface)
            // Underground = Dirt (below surface)
            string tileType;
            int surfaceLevel = (int)Math.Floor(worldHeight / 2.0);
            
            if (gridY < surfaceLevel)
            {
                tileType = "Air";
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
            
            // Only render visible tiles to optimize performance
            if (!tile.IsVisible())
            {
                spriteRenderer.enabled = false;
            }
            
            tileObject.AwakeAll();
            tileObject.Start();
            _tiles[gridX, gridY] = tileObject;
        }
        
        public GameObject GetTileAt(int gridX, int gridY)
        {
            if (gridX >= 0 && gridX < worldWidth && gridY >= 0 && gridY < worldHeight)
            {
                return _tiles[gridX, gridY];
            }
            return null;
        }
        
        public Tile GetTileComponentAt(int gridX, int gridY)
        {
            var tileObject = GetTileAt(gridX, gridY);
            return tileObject?.GetComponent<Tile>();
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
    }
} 