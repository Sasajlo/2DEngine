using System;
using System.Threading;
using Engine.Components;

namespace Engine
{
    public class SimpleEngine : IDisposable
    {
        private bool _initialized = false;
        private bool _disposed = false;
        
        public SceneManager sceneManager => SceneManager.Instance;
        
        public bool Initialize(int width, int height, string title)
        {
            if (_initialized)
            {
                Console.WriteLine("Engine already initialized");
                return true;
            }

            Console.WriteLine($"Initializing engine: {width}x{height} - {title}");
            
            _initialized = EngineCore.InitializeEngine(width, height, title);
            
            if (_initialized)
            {
                Console.WriteLine("Engine initialized successfully");
                
                // Initialize identity matrices for camera (will be updated by SceneManager)
                var identityMatrix = new float[16]
                {
                    1.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 1.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f
                };
                
                EngineCore.SetCameraMatrices(identityMatrix, identityMatrix);
                
                // Initialize time system
                Time.Reset();
            }
            else
            {
                Console.WriteLine("Failed to initialize engine");
            }

            return _initialized;
        }

        public void Run()
        {
            if (!_initialized)
            {
                Console.WriteLine("Engine not initialized");
                return;
            }

            Console.WriteLine("Starting main loop...");
            
            // Unity-style lifecycle: Awake → Start → Update
            sceneManager.AwakeAll();
            sceneManager.activeScene?.Start();

            // FPS display variables
            float fpsUpdateTimer = 0.0f;
            int frameCount = 0;
            const float fpsUpdateInterval = 1.0f; // Update FPS display every second
            string baseTitle = "2D Engine - ECS Demo";

            while (!EngineCore.ShouldClose())
            {
                // Update delta time
                Time.Update();
                
                EngineCore.PollEvents();
                
                // Update game logic (components use Time.deltaTime)
                sceneManager.Update();
                
                // Render sprites
                sceneManager.Render();
                
                // Render frame
                EngineCore.Render();
                
                // Update FPS display in window title
                fpsUpdateTimer += Time.deltaTime;
                ++frameCount;
                if (fpsUpdateTimer >= fpsUpdateInterval)
                {
                    var fps = frameCount / fpsUpdateTimer;
                    var newTitle = $"{baseTitle} - FPS: {fps:F1}";
                    EngineCore.SetWindowTitle(newTitle);
                    fpsUpdateTimer = 0.0f;
                    frameCount = 0;
                }
                
                // No more Thread.Sleep - deltaTime handles timing naturally
            }

            Console.WriteLine("Main loop ended");
        }

        public void SetTitle(string title)
        {
            if (_initialized)
            {
                EngineCore.SetWindowTitle(title);
            }
        }

        public (int width, int height) GetWindowSize()
        {
            if (_initialized)
            {
                EngineCore.GetWindowSize(out int width, out int height);
                return (width, height);
            }
            return (0, 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_initialized)
                {
                    Console.WriteLine("Shutting down engine...");
                    try
                    {
                        EngineCore.ShutdownEngine();
                        Console.WriteLine("Engine shutdown completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during engine shutdown: {ex.Message}");
                    }
                    _initialized = false;
                }
                _disposed = true;
            }
        }

        ~SimpleEngine()
        {
            Dispose(false);
        }
    }
} 