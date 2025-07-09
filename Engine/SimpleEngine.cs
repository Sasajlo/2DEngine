namespace Engine
{
    public class SimpleEngine : IDisposable
    {
        private bool _initialized = false;
        private bool _disposed = false;

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

            while (!EngineCore.ShouldClose())
            {
                EngineCore.PollEvents();
                EngineCore.Render();
                
                // Small delay to prevent 100% CPU usage in this simple example
                Thread.Sleep(16); // ~60 FPS
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