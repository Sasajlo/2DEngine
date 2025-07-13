using System;
using System.Numerics;

namespace Engine.Components
{
    /// <summary>
    /// Component for controlling camera movement with WASD and zoom with mouse scroll
    /// </summary>
    public class CameraController : Component
    {
        [Header("Movement")]
        public float moveSpeed = 0.2f;
        
        [Header("Zoom")]
        public float zoomSpeed = 0.1f; // Zoom factor per scroll (10% per scroll)
        public float minZoom = 1.0f;
        public float maxZoom = 20.0f;
        
        [Header("World Boundaries")]
        public float worldMinX = -0.5f;
        public float worldMinY = -0.5f;
        public float worldMaxX = 99.5f;
        public float worldMaxY = 99.5f;
        
        private Camera _camera;
        private Transform _transform;
        
        public override void Awake()
        {
            _camera = gameObject.GetComponent<Camera>();
            _transform = gameObject.transform;
            
            if (_camera == null)
            {
                // Console.WriteLine("CameraController: No Camera component found on GameObject!");
            }
        }
        
        public override void Start()
        {
            // Ensure camera starts within world boundaries
            if (_camera != null && _transform != null)
            {
                // // First, constrain the zoom level to fit within world boundaries
                // EngineCore.GetWindowSize(out int width, out int height);
                // float aspectRatio = (float)width / height;
                // float worldWidth = worldMaxX - worldMinX;
                // float worldHeight = worldMaxY - worldMinY;
                
                // // Calculate maximum allowed orthographicSize to fit world
                // float maxZoomForWidth = worldWidth / (2.0f * aspectRatio);
                // float maxZoomForHeight = worldHeight / 2.0f;
                // float dynamicMaxZoom = Math.Min(maxZoomForWidth, maxZoomForHeight);
                // dynamicMaxZoom = Math.Min(dynamicMaxZoom, maxZoom);
                
                // Constrain initial orthographicSize
                _camera.orthographicSize = minZoom * 5.0f;
                
                // Then constrain position
                _transform.position = ClampCameraPosition(_transform.position);
            }
        }
        
        public override void Update()
        {
            if (_camera == null || _transform == null)
                return;
            
            HandleMovement();
            HandleZoom();
        }
        
        private void HandleMovement()
        {
            var moveVector = Vector3.Zero;
            
            // WASD movement (camera movement to show desired view direction)
            if (Input.IsKeyHeld(Input.KeyCode.W)) // W - Show up (move camera down)
                moveVector.Y -= 1.0f;
            if (Input.IsKeyHeld(Input.KeyCode.S)) // S - Show down (move camera up)  
                moveVector.Y += 1.0f;
            if (Input.IsKeyHeld(Input.KeyCode.A)) // A - Show left (move camera left)
                moveVector.X -= 1.0f;
            if (Input.IsKeyHeld(Input.KeyCode.D)) // D - Show right (move camera right)
                moveVector.X += 1.0f;
            
            // Apply movement with speed and deltaTime
            if (moveVector != Vector3.Zero)
            {
                moveVector = Vector3.Normalize(moveVector); // Normalize for consistent diagonal movement
                
                // Scale movement speed based on zoom level for constant apparent movement
                float zoomScaledSpeed = moveSpeed * _camera.orthographicSize;
                var movement = moveVector * zoomScaledSpeed * Time.deltaTime;
                
                var newPosition = _transform.position + movement;
                
                // Clamp camera position to world boundaries
                newPosition = ClampCameraPosition(newPosition);
                
                _transform.position = newPosition;
            }
        }
        
        private void HandleZoom()
        {
            float scrollDelta = Input.GetMouseScrollDelta();
            
            if (scrollDelta != 0.0f)
            {
                // Get mouse position before zoom
                var mousePos = Input.GetMousePosition();
                var worldPointBeforeZoom = ScreenToWorldPoint(mousePos);
                
                // Apply smooth percentage-based zoom
                // Positive scroll = zoom in (smaller orthographic size)
                // Negative scroll = zoom out (larger orthographic size)
                float zoomFactor = 1.0f + (scrollDelta * zoomSpeed);
                float oldZoom = _camera.orthographicSize;
                
                // Calculate dynamic max zoom to ensure camera view doesn't exceed world bounds
                EngineCore.GetWindowSize(out int width, out int height);
                float aspectRatio = (float)width / height;
                float worldWidth = worldMaxX - worldMinX;
                float worldHeight = worldMaxY - worldMinY;
                
                // Maximum zoom should never allow camera viewport to exceed world bounds
                // Camera height = orthographicSize * 2
                // Camera width = Camera height * aspectRatio = orthographicSize * 2 * aspectRatio
                float maxZoomForWidth = worldWidth / (2.0f * aspectRatio);
                float maxZoomForHeight = worldHeight / 2.0f;
                float dynamicMaxZoom = Math.Min(maxZoomForWidth, maxZoomForHeight);
                dynamicMaxZoom = Math.Min(dynamicMaxZoom, maxZoom);
                
                float newZoom = Math.Clamp(oldZoom / zoomFactor, minZoom, dynamicMaxZoom);
                _camera.orthographicSize = newZoom;
                
                // Calculate world point after zoom (same screen position)
                var worldPointAfterZoom = ScreenToWorldPoint(mousePos);
                
                // Adjust camera position to keep the world point under the mouse
                var worldPointDelta = worldPointBeforeZoom - worldPointAfterZoom;
                var newPosition = _transform.position + worldPointDelta;
                
                // Clamp camera position to world boundaries after zoom
                newPosition = ClampCameraPosition(newPosition);
                
                _transform.position = newPosition;
            }
        }
        
        private Vector3 ClampCameraPosition(Vector3 position)
        {
            // Get window size for aspect ratio
            EngineCore.GetWindowSize(out int width, out int height);
            float aspectRatio = (float)width / height;
            
            // Calculate camera view bounds
            float cameraHeight = _camera.orthographicSize * 2.0f;
            float cameraWidth = cameraHeight * aspectRatio;
            
            float halfWidth = cameraWidth * 0.5f;
            float halfHeight = cameraHeight * 0.5f;
            
            // Calculate world bounds
            float worldWidth = worldMaxX - worldMinX;
            float worldHeight = worldMaxY - worldMinY;
            float worldCenterX = (worldMaxX + worldMinX) * 0.5f;
            float worldCenterY = (worldMaxY + worldMinY) * 0.5f;
            
            // If camera view is larger than world, center it
            if (cameraWidth >= worldWidth)
            {
                position.X = worldCenterX;
            }
            else
            {
                // Clamp camera position so view stays within world bounds
                float minX = worldMinX + halfWidth;
                float maxX = worldMaxX - halfWidth;
                position.X = Math.Clamp(position.X, minX, maxX);
            }
            
            if (cameraHeight >= worldHeight)
            {
                position.Y = worldCenterY;
            }
            else
            {
                // Clamp camera position so view stays within world bounds
                float minY = worldMinY + halfHeight;
                float maxY = worldMaxY - halfHeight;
                position.Y = Math.Clamp(position.Y, minY, maxY);
            }
            
            return position;
        }
        
        private Vector3 ScreenToWorldPoint(Vector2 screenPoint)
        {
            // Get window size
            EngineCore.GetWindowSize(out int width, out int height);
            
            // Convert screen coordinates to normalized device coordinates (-1 to 1)
            float normalizedX = (screenPoint.X / width) * 2.0f - 1.0f;
            float normalizedY = (screenPoint.Y / height) * 2.0f - 1.0f; // No Y flip - keep screen coordinate system
            
            // Calculate world size based on camera's orthographic size
            float worldHeight = _camera.orthographicSize * 2.0f;
            float worldWidth = worldHeight * (float)width / height;
            
            // Convert to world coordinates relative to camera
            float worldX = normalizedX * (worldWidth * 0.5f);
            float worldY = normalizedY * (worldHeight * 0.5f);
            
            // Add camera position to get absolute world position
            return new Vector3(
                _transform.position.X + worldX,
                _transform.position.Y + worldY,
                0.0f
            );
        }
    }
    
    /// <summary>
    /// Attribute for organizing inspector fields (Unity-style)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class HeaderAttribute : Attribute
    {
        public string title { get; }
        
        public HeaderAttribute(string title)
        {
            this.title = title;
        }
    }
} 