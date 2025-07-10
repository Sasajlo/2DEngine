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
    
    public class Tile : Component
    {
        public TileData tileData { get; private set; }
        public int gridX { get; set; }
        public int gridY { get; set; }
        
        private Sprite _sprite;
        private SpriteRenderer _spriteRenderer;
        
        public override void Awake()
        {
            _sprite = gameObject.GetComponent<Sprite>();
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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
                    // Get sprite component directly instead of relying on cached field
                    // This handles the case where LoadTileType is called before Awake
                    var sprite = gameObject.GetComponent<Sprite>();
                    var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    
                    if (sprite != null)
                    {
                        sprite.color = tileData.color.ToVector4();
                        sprite.texturePath = tileData.texture;
                    }
                    
                    // Set sprite renderer sorting order based on grid position
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sortingOrder = gridY; // Lower Y values render in front
                    }
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