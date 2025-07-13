using System;
using System.Numerics;

namespace Engine.Components
{
    /// <summary>
    /// Debug system for managing debug mode and debug rendering
    /// </summary>
    public static class DebugSystem
    {
        private static bool _isDebugMode = false;
        private static double _lastChunkLogTime = 0;
        
        /// <summary>
        /// Whether debug mode is currently enabled
        /// </summary>
        public static bool IsDebugMode => _isDebugMode;
        
        /// <summary>
        /// Update debug system - call this every frame
        /// </summary>
        public static void Update()
        {   
            // Toggle debug mode on H key press (not hold)
            if (Input.IsKeyPressed(Input.KeyCode.F12))
            {
                _isDebugMode = !_isDebugMode;
                Console.WriteLine($"Debug mode: {(_isDebugMode ? "ON" : "OFF")}");
            }
        }
        
        /// <summary>
        /// Force set debug mode state
        /// </summary>
        public static void SetDebugMode(bool enabled)
        {
            _isDebugMode = enabled;
            Console.WriteLine($"Debug mode: {(_isDebugMode ? "ON" : "OFF")}");
        }
        
        /// <summary>
        /// Render debug line between two points
        /// </summary>
        public static void DrawLine(Vector2 start, Vector2 end, Vector4 color, float thickness = 1.0f)
        {
            if (!_isDebugMode)
                return;
            
            // Calculate line direction and length
            Vector2 direction = end - start;
            float length = direction.Length();
            
            if (length == 0)
                return;
            
            // Normalize direction
            direction = Vector2.Normalize(direction);
            
            // Calculate rotation angle
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            
            // Create transform matrix for the line
            var scaleMatrix = Matrix4x4.CreateScale(length, thickness, 1.0f);
            var rotationMatrix = Matrix4x4.CreateRotationZ(angle);
            var translationMatrix = Matrix4x4.CreateTranslation(start.X + direction.X * length * 0.5f, start.Y + direction.Y * length * 0.5f, 0);
            
            var transform = scaleMatrix * rotationMatrix * translationMatrix;
            
            // Convert Matrix4x4 to float array
            var matrixArray = new float[16]
            {
                transform.M11, transform.M12, transform.M13, transform.M14,
                transform.M21, transform.M22, transform.M23, transform.M24,
                transform.M31, transform.M32, transform.M33, transform.M34,
                transform.M41, transform.M42, transform.M43, transform.M44
            };
            
            // Convert color to float array
            var colorArray = new float[4] { color.X, color.Y, color.Z, color.W };
            
            // Set size
            var sizeArray = new float[2] { 1.0f, 1.0f };
            
            // Render using default square texture
            EngineCore.RenderSprite(
                "Assets/Textures/Default/square.png",
                matrixArray,
                colorArray,
                sizeArray,
                1000, // High sorting order for debug graphics
                false, // flipX
                false  // flipY
            );
        }
        
        /// <summary>
        /// Render debug rectangle outline
        /// </summary>
        public static void DrawRectangle(Vector2 position, Vector2 size, Vector4 color, float thickness = 1.0f)
        {
            if (!_isDebugMode)
                return;
            
            Vector2 topLeft = position;
            Vector2 topRight = position + new Vector2(size.X, 0);
            Vector2 bottomLeft = position + new Vector2(0, size.Y);
            Vector2 bottomRight = position + size;
            
            // Draw rectangle outline
            DrawLine(topLeft, topRight, color, thickness);     // Top edge
            DrawLine(topRight, bottomRight, color, thickness); // Right edge
            DrawLine(bottomRight, bottomLeft, color, thickness); // Bottom edge
            DrawLine(bottomLeft, topLeft, color, thickness);   // Left edge
        }
        
        /// <summary>
        /// Render debug grid
        /// </summary>
        public static void DrawGrid(Vector2 start, Vector2 end, Vector2 cellSize, Vector4 color, float thickness = 1.0f)
        {
            if (!_isDebugMode)
                return;
            
            // Draw vertical lines
            for (float x = start.X; x <= end.X; x += cellSize.X)
            {
                DrawLine(new Vector2(x, start.Y), new Vector2(x, end.Y), color, thickness);
            }
            
            // Draw horizontal lines
            for (float y = start.Y; y <= end.Y; y += cellSize.Y)
            {
                DrawLine(new Vector2(start.X, y), new Vector2(end.X, y), color, thickness);
            }
        }
        
        /// <summary>
        /// Log chunk information when debug mode is enabled
        /// </summary>
        public static void LogChunkInfo(int visibleChunks, int totalChunks)
        {
            if (!_isDebugMode)
                return;
            
            // Log chunk information every 2 seconds to avoid spam
            if (Time.time - _lastChunkLogTime >= 2.0)
            {
                double renderingPercentage = (double)visibleChunks / totalChunks * 100.0;
                Console.WriteLine($"Chunks: {visibleChunks}/{totalChunks} visible ({renderingPercentage:F1}%)");
                _lastChunkLogTime = Time.time;
            }
        }
        
        /// <summary>
        /// Render chunk grid using WorldManager data
        /// </summary>
        public static void RenderChunkGrid(WorldManager worldManager)
        {
            if (!_isDebugMode)
                return;
            
            var camera = Camera.mainCamera;
            if (camera == null)
                return;
            
            var cameraTransform = camera.gameObject.transform;
            if (cameraTransform == null)
                return;
            
            // Get viewport bounds to determine which chunk grid lines to draw
            var (viewportMin, viewportMax) = ViewportCuller.GetViewportBounds(camera, cameraTransform);
            
            // Add some margin to ensure grid lines are visible at viewport edges
            float chunkWorldSize = worldManager.GetChunkWorldSize();
            float margin = chunkWorldSize;
            viewportMin -= new Vector2(margin, margin);
            viewportMax += new Vector2(margin, margin);
            
            // Calculate the range of grid lines to draw
            // Grid lines should be at chunk boundaries, which are between tiles
            float tileSize = worldManager.GetTileSize();
            float halfTile = tileSize * 0.5f;
            
            float startX = (float)Math.Floor(viewportMin.X / chunkWorldSize) * chunkWorldSize;
            float endX = (float)Math.Ceiling(viewportMax.X / chunkWorldSize) * chunkWorldSize;
            float startY = (float)Math.Floor(viewportMin.Y / chunkWorldSize) * chunkWorldSize;
            float endY = (float)Math.Ceiling(viewportMax.Y / chunkWorldSize) * chunkWorldSize;
            
            // Yellow color for chunk grid
            Vector4 chunkGridColor = new Vector4(1.0f, 1.0f, 0.0f, 0.8f); // Yellow with some transparency
            
            // Scale line thickness with camera zoom to maintain consistent screen appearance
            float baseThickness = 0.02f; // Reduced base thickness
            float zoomFactor = camera.orthographicSize / 5.0f; // Normalize to zoom level (5.0f = default zoom)
            float gridLineThickness = Math.Max(0.01f, Math.Min(0.15f, baseThickness * zoomFactor)); // Thinner range
            
            // Draw vertical lines (chunk boundaries)
            // Boundaries are at: chunkWorldSize - 0.5, chunkWorldSize * 2 - 0.5, etc.
            for (float x = startX + chunkWorldSize - halfTile; x <= endX; x += chunkWorldSize)
            {
                DrawLine(
                    new Vector2(x, startY),
                    new Vector2(x, endY),
                    chunkGridColor,
                    gridLineThickness
                );
            }
            
            // Draw horizontal lines (chunk boundaries)
            for (float y = startY + chunkWorldSize - halfTile; y <= endY; y += chunkWorldSize)
            {
                DrawLine(
                    new Vector2(startX, y),
                    new Vector2(endX, y),
                    chunkGridColor,
                    gridLineThickness
                );
            }
        }
    }
} 