using Engine;

namespace Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("2D Engine Demo");
            Console.WriteLine("===============");

            using var engine = new SimpleEngine();
            
            if (!engine.Initialize(800, 600, "2D Engine - Vulkan Demo"))
            {
                Console.WriteLine("Failed to initialize engine!");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            var (width, height) = engine.GetWindowSize();
            Console.WriteLine($"Window size: {width}x{height}");

            Console.WriteLine("Window should now be visible. Close it to exit.");
            
            // Run the main loop
            engine.Run();

            Console.WriteLine("Engine demo completed.");
        }
    }
} 