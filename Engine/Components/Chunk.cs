using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Engine.Components
{
    public class Chunk
    {
        public const int CHUNK_SIZE = 32;
        
        public int chunkX { get; private set; }
        public int chunkY { get; private set; }
        public Vector2 worldPosition { get; private set; }
        public Vector2 worldBounds { get; private set; } // Size of chunk in world units
        
        private Tile[,] _tiles;
        private bool _isVisible;
        
        public Chunk(int chunkX, int chunkY, float tileSize)
        {
            this.chunkX = chunkX;
            this.chunkY = chunkY;
            
            // Calculate world position (bottom-left corner of chunk)
            worldPosition = new Vector2(chunkX * CHUNK_SIZE * tileSize, chunkY * CHUNK_SIZE * tileSize);
            worldBounds = new Vector2(CHUNK_SIZE * tileSize, CHUNK_SIZE * tileSize);
            
            _tiles = new Tile[CHUNK_SIZE, CHUNK_SIZE];
            _isVisible = false;
        }
        
        public void SetTile(int localX, int localY, Tile tile)
        {
            if (IsValidLocalPosition(localX, localY))
            {
                _tiles[localX, localY] = tile;
            }
        }
        
        public Tile GetTile(int localX, int localY)
        {
            if (IsValidLocalPosition(localX, localY))
            {
                return _tiles[localX, localY];
            }
            return null;
        }
        
        public bool IsValidLocalPosition(int localX, int localY)
        {
            return localX >= 0 && localX < CHUNK_SIZE && localY >= 0 && localY < CHUNK_SIZE;
        }
        
        public void SetVisible(bool visible)
        {
            _isVisible = visible;
        }
        
        public bool IsVisible()
        {
            return _isVisible;
        }
        
        /// <summary>
        /// Check if this chunk intersects with a given rectangular area
        /// </summary>
        public bool IntersectsWithRect(Vector2 rectMin, Vector2 rectMax)
        {
            Vector2 chunkMin = worldPosition;
            Vector2 chunkMax = worldPosition + worldBounds;
            
            return !(rectMax.X < chunkMin.X || rectMin.X > chunkMax.X || 
                     rectMax.Y < chunkMin.Y || rectMin.Y > chunkMax.Y);
        }
        
        /// <summary>
        /// Get all visible tiles in this chunk for rendering
        /// </summary>
        public List<Tile> GetVisibleTiles()
        {
            var visibleTiles = new List<Tile>();
            
            if (!_isVisible)
                return visibleTiles;
            
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    var tile = _tiles[x, y];
                    if (tile != null && tile.isVisible)
                    {
                        visibleTiles.Add(tile);
                    }
                }
            }
            
            return visibleTiles;
        }
        
        /// <summary>
        /// Render all visible tiles in this chunk
        /// </summary>
        public void Render()
        {
            if (!_isVisible)
                return;
            
            var visibleTiles = GetVisibleTiles();
            
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
        
        /// <summary>
        /// Convert world coordinates to local chunk coordinates
        /// </summary>
        public Vector2 WorldToLocalPosition(Vector2 worldPos)
        {
            return worldPos - worldPosition;
        }
        
        /// <summary>
        /// Convert local chunk coordinates to world coordinates
        /// </summary>
        public Vector2 LocalToWorldPosition(Vector2 localPos)
        {
            return worldPosition + localPos;
        }
    }
} 