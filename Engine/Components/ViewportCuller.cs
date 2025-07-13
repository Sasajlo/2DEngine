using System;
using System.Collections.Generic;
using System.Numerics;

namespace Engine.Components
{
    public static class ViewportCuller
    {
        /// <summary>
        /// Calculate the viewport bounds in world space based on camera properties
        /// </summary>
        public static (Vector2 min, Vector2 max) GetViewportBounds(Camera camera, Transform cameraTransform)
        {
            if (camera == null || cameraTransform == null)
                return (Vector2.Zero, Vector2.Zero);
            
            // Get window size for aspect ratio
            EngineCore.GetWindowSize(out int width, out int height);
            float aspectRatio = (float)width / height;
            
            // Calculate viewport size in world units
            float viewportHeight = camera.orthographicSize * 2.0f;
            float viewportWidth = viewportHeight * aspectRatio;
            
            // Get camera position
            Vector3 cameraPos = cameraTransform.position;
            
            // Calculate viewport bounds
            Vector2 min = new Vector2(
                cameraPos.X - viewportWidth * 0.5f,
                cameraPos.Y - viewportHeight * 0.5f
            );
            
            Vector2 max = new Vector2(
                cameraPos.X + viewportWidth * 0.5f,
                cameraPos.Y + viewportHeight * 0.5f
            );
            
            return (min, max);
        }
        
        /// <summary>
        /// Get all chunks that are visible within the viewport
        /// </summary>
        public static List<Chunk> GetVisibleChunks(
            Dictionary<Vector2, Chunk> chunks, 
            Camera camera, 
            Transform cameraTransform,
            float margin = 0.0f)
        {
            var visibleChunks = new List<Chunk>();
            
            if (camera == null || cameraTransform == null)
                return visibleChunks;
            
            var (viewportMin, viewportMax) = GetViewportBounds(camera, cameraTransform);
            
            // Add margin to viewport bounds for smoother culling
            viewportMin -= new Vector2(margin, margin);
            viewportMax += new Vector2(margin, margin);
            
            // Test each chunk for intersection with viewport
            foreach (var chunk in chunks.Values)
            {
                if (chunk.IntersectsWithRect(viewportMin, viewportMax))
                {
                    visibleChunks.Add(chunk);
                }
            }
            
            return visibleChunks;
        }
        
        /// <summary>
        /// Calculate which chunk coordinates are potentially visible
        /// </summary>
        public static (Vector2 minChunk, Vector2 maxChunk) GetVisibleChunkRange(
            Camera camera, 
            Transform cameraTransform, 
            float tileSize,
            float margin = 0.0f)
        {
            var (viewportMin, viewportMax) = GetViewportBounds(camera, cameraTransform);
            
            // Add margin to viewport bounds
            viewportMin -= new Vector2(margin, margin);
            viewportMax += new Vector2(margin, margin);
            
            // Convert viewport bounds to chunk coordinates
            float chunkWorldSize = Chunk.CHUNK_SIZE * tileSize;
            
            Vector2 minChunk = new Vector2(
                (float)Math.Floor(viewportMin.X / chunkWorldSize),
                (float)Math.Floor(viewportMin.Y / chunkWorldSize)
            );
            
            Vector2 maxChunk = new Vector2(
                (float)Math.Ceiling(viewportMax.X / chunkWorldSize),
                (float)Math.Ceiling(viewportMax.Y / chunkWorldSize)
            );
            
            return (minChunk, maxChunk);
        }
        
        /// <summary>
        /// Check if a specific chunk coordinate is visible
        /// </summary>
        public static bool IsChunkVisible(
            int chunkX, 
            int chunkY, 
            Camera camera, 
            Transform cameraTransform, 
            float tileSize,
            float margin = 0.0f)
        {
            var (viewportMin, viewportMax) = GetViewportBounds(camera, cameraTransform);
            
            // Add margin to viewport bounds
            viewportMin -= new Vector2(margin, margin);
            viewportMax += new Vector2(margin, margin);
            
            // Calculate chunk bounds in world space
            float chunkWorldSize = Chunk.CHUNK_SIZE * tileSize;
            Vector2 chunkMin = new Vector2(chunkX * chunkWorldSize, chunkY * chunkWorldSize);
            Vector2 chunkMax = chunkMin + new Vector2(chunkWorldSize, chunkWorldSize);
            
            // Check intersection
            return !(viewportMax.X < chunkMin.X || viewportMin.X > chunkMax.X || 
                     viewportMax.Y < chunkMin.Y || viewportMin.Y > chunkMax.Y);
        }
    }
} 