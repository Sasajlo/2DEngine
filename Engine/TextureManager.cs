using System;
using System.Collections.Generic;

namespace Engine
{
    /// <summary>
    /// Manages texture loading and provides texture indices for efficient rendering
    /// </summary>
    public static class TextureManager
    {
        private static Dictionary<string, uint> _textureIndices = new Dictionary<string, uint>();
        private static uint _nextTextureIndex = 0;
        
        /// <summary>
        /// Load a texture and return its GPU index. If already loaded, returns existing index.
        /// </summary>
        public static uint LoadTexture(string texturePath)
        {
            if (string.IsNullOrEmpty(texturePath))
                return uint.MaxValue;
                
            // Check if texture is already loaded
            if (_textureIndices.TryGetValue(texturePath, out uint existingIndex))
            {
                return existingIndex;
            }
            
            // Load texture via C++ and get index
            uint textureIndex = EngineCore.LoadTexture(texturePath);
            
            if (textureIndex != uint.MaxValue)
            {
                // Store the mapping
                _textureIndices[texturePath] = textureIndex;
                _nextTextureIndex = Math.Max(_nextTextureIndex, textureIndex + 1);
                
                Console.WriteLine($"Loaded texture: {texturePath} -> Index {textureIndex}");
            }
            else
            {
                Console.WriteLine($"Failed to load texture: {texturePath}");
            }
            
            return textureIndex;
        }
        
        /// <summary>
        /// Get texture index for a path (must be loaded first)
        /// </summary>
        public static uint GetTextureIndex(string texturePath)
        {
            if (_textureIndices.TryGetValue(texturePath, out uint index))
            {
                return index;
            }
            return uint.MaxValue;
        }
        
        /// <summary>
        /// Get total number of loaded textures
        /// </summary>
        public static int GetLoadedTextureCount()
        {
            return _textureIndices.Count;
        }
        
        /// <summary>
        /// Clear all texture mappings (for cleanup)
        /// </summary>
        public static void Clear()
        {
            _textureIndices.Clear();
            _nextTextureIndex = 0;
        }
        
        /// <summary>
        /// Get debug info about loaded textures
        /// </summary>
        public static string GetDebugInfo()
        {
            return $"Loaded textures: {_textureIndices.Count}";
        }
    }
} 