using System;
using System.Numerics;

namespace Engine.Components
{
    /// <summary>
    /// Component that renders a hover texture over the tile currently under the mouse cursor
    /// </summary>
    public class TileHoverEffect : Component
    {
        [Header("Hover Settings")]
        public string hoverTexturePath = "Assets/Textures/Tiles/hovered.png";
        public Vector4 hoverTint = new Vector4(1.0f, 1.0f, 1.0f, 0.8f); // Slightly transparent
        public int hoverSortingOrder = 1000; // Render on top of everything
        
        private Camera _camera;
        private WorldManager _worldManager;
        private Vector2 _lastHoveredGrid = new Vector2(-1, -1);
        
        public override void Awake()
        {
            // Find the main camera
            _camera = Camera.mainCamera;
            if (_camera == null)
            {
                Console.WriteLine("TileHoverEffect: No main camera found!");
            }
            
            // Find the world manager in the scene
            _worldManager = gameObject.scene.FindObjectOfType<WorldManager>();
            if (_worldManager == null)
            {
                Console.WriteLine("TileHoverEffect: No WorldManager found in scene!");
            }
        }
        
        public override void Update()
        {
            if (_camera == null || _worldManager == null)
                return;
                
            // Get mouse position and convert to world coordinates
            var mouseScreenPos = Input.GetMousePosition();
            var worldPos = ScreenToWorldPoint(mouseScreenPos);
            
            // Convert world position to tile grid coordinates
            var gridPos = _worldManager.WorldToGrid(worldPos);
            int gridX = (int)gridPos.X;
            int gridY = (int)gridPos.Y;
            
            // Check if this is a valid tile position
            if (_worldManager.IsValidPosition(gridX, gridY))
            {
                _lastHoveredGrid = new Vector2(gridX, gridY);
            }
            else
            {
                _lastHoveredGrid = new Vector2(-1, -1); // Invalid position
            }
        }
        
        public void Render()
        {
            if (_camera == null || _worldManager == null)
                return;
                
            // Only render if we have a valid hovered tile
            if (_lastHoveredGrid.X < 0 || _lastHoveredGrid.Y < 0)
                return;
                
            int gridX = (int)_lastHoveredGrid.X;
            int gridY = (int)_lastHoveredGrid.Y;
            
            // Get the tile at the hovered position
            var hoveredTile = _worldManager.GetTileAt(gridX, gridY);
            if (hoveredTile == null || !hoveredTile.isVisible)
                return;
                
            // Create transform matrix for the hover effect (same as tile position and size)
            var hoverPosition = _worldManager.GridToWorld(gridX, gridY);
            var hoverScale = new Vector3(_worldManager.tileSize, _worldManager.tileSize, 1.0f);
            
            var transform = Matrix4x4.CreateScale(hoverScale) * 
                           Matrix4x4.CreateTranslation(hoverPosition);
            
            // Convert Matrix4x4 to float array
            var matrixArray = new float[16]
            {
                transform.M11, transform.M12, transform.M13, transform.M14,
                transform.M21, transform.M22, transform.M23, transform.M24,
                transform.M31, transform.M32, transform.M33, transform.M34,
                transform.M41, transform.M42, transform.M43, transform.M44
            };
            
            // Convert tint color to float array
            var colorArray = new float[4] { hoverTint.X, hoverTint.Y, hoverTint.Z, hoverTint.W };
            
            // Set default size (1x1 for tiles)
            var sizeArray = new float[2] { 1.0f, 1.0f };
            
            // Render the hover effect
            EngineCore.RenderSprite(
                hoverTexturePath,
                matrixArray,
                colorArray,
                sizeArray,
                hoverSortingOrder,
                false, // flipX
                false  // flipY
            );
        }
        
        /// <summary>
        /// Convert screen coordinates to world coordinates using the main camera
        /// </summary>
        private Vector3 ScreenToWorldPoint(Vector2 screenPoint)
        {
            if (_camera == null)
                return Vector3.Zero;
                
            var cameraTransform = _camera.gameObject.transform;
            if (cameraTransform == null)
                return Vector3.Zero;
            
            // Get window size
            EngineCore.GetWindowSize(out int width, out int height);
            
            // Convert screen coordinates to normalized device coordinates (-1 to 1)
            float normalizedX = (screenPoint.X / width) * 2.0f - 1.0f;
            float normalizedY = (screenPoint.Y / height) * 2.0f - 1.0f;
            
            // Calculate world size based on camera's orthographic size
            float worldHeight = _camera.orthographicSize * 2.0f;
            float worldWidth = worldHeight * (float)width / height;
            
            // Convert to world coordinates relative to camera
            float worldX = normalizedX * (worldWidth * 0.5f);
            float worldY = normalizedY * (worldHeight * 0.5f);
            
            // Add camera position to get absolute world position
            return new Vector3(
                cameraTransform.position.X + worldX,
                cameraTransform.position.Y + worldY,
                0.0f
            );
        }
        
        /// <summary>
        /// Get the currently hovered tile grid coordinates
        /// </summary>
        public Vector2 GetHoveredGridPosition()
        {
            return _lastHoveredGrid;
        }
        
        /// <summary>
        /// Get the currently hovered tile object
        /// </summary>
        public Tile GetHoveredTile()
        {
            if (_worldManager == null || _lastHoveredGrid.X < 0 || _lastHoveredGrid.Y < 0)
                return null;
                
            return _worldManager.GetTileAt((int)_lastHoveredGrid.X, (int)_lastHoveredGrid.Y);
        }
    }
} 