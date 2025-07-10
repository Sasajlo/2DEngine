using System;
using System.Numerics;

namespace Engine.Components
{
    public class Sprite : Component
    {
        public string texturePath { get; set; } = "Assets/Textures/square.png";
        public Vector4 color { get; set; } = Vector4.One; // White color (R, G, B, A)
        public Vector2 size { get; set; } = Vector2.One;
        
        public Sprite()
        {
        }
        
        public Sprite(string texturePath)
        {
            this.texturePath = texturePath;
        }
        
        public Sprite(string texturePath, Vector4 color)
        {
            this.texturePath = texturePath;
            this.color = color;
        }
    }
} 