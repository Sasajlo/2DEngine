using System;
using System.Collections.Generic;
using Engine.Components;

namespace Engine
{
    public class SceneManager
    {
        private static SceneManager _instance;
        public static SceneManager Instance => _instance ??= new SceneManager();
        
        private Scene _activeScene;
        private readonly Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();
        
        public Scene activeScene => _activeScene;
        
        public Scene CreateScene(string name)
        {
            if (_scenes.ContainsKey(name))
            {
                throw new ArgumentException($"Scene '{name}' already exists");
            }
            
            var scene = new Scene(name);
            _scenes[name] = scene;
            
            if (_activeScene == null)
            {
                _activeScene = scene;
            }
            
            return scene;
        }
        
        public void LoadScene(string name)
        {
            if (_scenes.TryGetValue(name, out Scene scene))
            {
                _activeScene = scene;
            }
            else
            {
                throw new ArgumentException($"Scene '{name}' not found");
            }
        }
        
        public void AwakeAll()
        {
            if (_activeScene != null)
            {
                _activeScene.AwakeAll();
            }
        }
        
        public void Update()
        {
            if (_activeScene != null)
            {
                _activeScene.Update();
            }
        }
        
        public void Render()
        {
            if (_activeScene != null)
            {
                // Update camera if available
                var camera = _activeScene.FindObjectOfType<Camera>();
                if (camera != null)
                {
                    UpdateCamera(camera);
                }
                
                // Render the scene
                _activeScene.Render();
            }
        }
        
        private void UpdateCamera(Camera camera)
        {
            // Get window size for aspect ratio
            EngineCore.GetWindowSize(out int width, out int height);
            float aspectRatio = (float)width / height;
            
            // Get camera matrices
            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = camera.GetProjectionMatrix(aspectRatio);
            
            // Convert to float arrays
            var viewArray = new float[16]
            {
                viewMatrix.M11, viewMatrix.M12, viewMatrix.M13, viewMatrix.M14,
                viewMatrix.M21, viewMatrix.M22, viewMatrix.M23, viewMatrix.M24,
                viewMatrix.M31, viewMatrix.M32, viewMatrix.M33, viewMatrix.M34,
                viewMatrix.M41, viewMatrix.M42, viewMatrix.M43, viewMatrix.M44
            };
            
            var projectionArray = new float[16]
            {
                projectionMatrix.M11, projectionMatrix.M12, projectionMatrix.M13, projectionMatrix.M14,
                projectionMatrix.M21, projectionMatrix.M22, projectionMatrix.M23, projectionMatrix.M24,
                projectionMatrix.M31, projectionMatrix.M32, projectionMatrix.M33, projectionMatrix.M34,
                projectionMatrix.M41, projectionMatrix.M42, projectionMatrix.M43, projectionMatrix.M44
            };
            
            // Set camera matrices in C++
            EngineCore.SetCameraMatrices(viewArray, projectionArray);
        }
    }
} 