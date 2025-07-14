using System;
using System.IO;
using System.Numerics;
using System.Text.Json;

namespace Engine.Components
{
    public class TileData
    {
        public string name { get; set; }
        public TileColor color { get; set; }
        public string texture { get; set; }
        public uint textureIndex { get; set; } = uint.MaxValue; // GPU texture index
    }
    
    public class TileColor
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }
        
        public Vector4 ToVector4()
        {
            return new Vector4(r, g, b, a);
        }
    }
    
    public class Tile
    {
        public TileData tileData { get; private set; }
        public int gridX { get; set; }
        public int gridY { get; set; }
        public Vector3 position { get; set; }
        public Vector3 scale { get; set; }
        public bool isVisible { get; set; } = true;
        public int sortingOrder { get; set; }
        
        public Tile(int gridX, int gridY, Vector3 position, Vector3 scale)
        {
            this.gridX = gridX;
            this.gridY = gridY;
            this.position = position;
            this.scale = scale;
            this.sortingOrder = gridY; // Lower Y values render in front
        }
        
        public bool LoadTileType(string tileTypeName)
        {
            try
            {
                string jsonPath = $"Assets/Tiles/{tileTypeName}.json";
                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine($"Tile JSON file not found: {jsonPath}");
                    return false;
                }
                
                string jsonContent = File.ReadAllText(jsonPath);
                tileData = JsonSerializer.Deserialize<TileData>(jsonContent);
                
                if (tileData != null)
                {
                    // Update visibility based on tile data
                    isVisible = IsVisible();
                    
                    // Load texture and get index
                    tileData.textureIndex = TextureManager.LoadTexture(tileData.texture);
                }
                
                return tileData != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tile type {tileTypeName}: {ex.Message}");
                return false;
            }
        }
        
        public bool IsVisible()
        {
            return tileData != null && tileData.color.a > 0.0f;
        }
    }
} 