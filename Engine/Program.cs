using System;
using Engine;
using Engine.Components;
using System.Numerics;

namespace Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("2D Engine Demo with ECS");
            Console.WriteLine("=======================");

            using var engine = new SimpleEngine();
            
            if (!engine.Initialize(800, 600, "2D Engine - ECS Demo"))
            {
                Console.WriteLine("Failed to initialize engine!");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            var (width, height) = engine.GetWindowSize();
            Console.WriteLine($"Window size: {width}x{height}");

            // Create a demo scene
            CreateDemoScene(engine);

            Console.WriteLine("Tile world created. Window should now be visible. Close it to exit.");
            Console.WriteLine("Top: Air tiles | Surface: Grass tiles | Underground: Dirt tiles");
            Console.WriteLine();
            Console.WriteLine("Camera Controls:");
            Console.WriteLine("  WASD         - Move camera (speed scales with zoom)");
            Console.WriteLine("  Mouse Scroll - Smooth zoom in/out (towards mouse cursor)");
            Console.WriteLine();
            
            // Run the main loop
            engine.Run();

            Console.WriteLine("Engine demo completed.");
        }
        
        static void CreateDemoScene(SimpleEngine engine)
        {
            // Create a new scene
            var scene = engine.sceneManager.CreateScene("TileWorldScene");
            
            // Create camera object
            var cameraObject = scene.CreateGameObject("MainCamera");
            
            // Set camera transform to view the center of the 100x100 tile world
            cameraObject.transform.position = new Vector3(50.0f, 50.0f, 10.0f);
            
            // Add camera component
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographicSize = 30.0f; // Zoom out to see more of the world
            camera.backgroundColor = new Vector4(0.53f, 0.81f, 0.92f, 1.0f); // Sky blue background
            
            // Add camera controller for WASD movement and mouse scroll zoom
            var cameraController = cameraObject.AddComponent<CameraController>();
            cameraController.moveSpeed = 10.0f; // Faster movement for large world
            cameraController.zoomSpeed = 0.15f; // Smooth percentage-based zoom (15% per scroll)
            cameraController.minZoom = 1.0f;
            cameraController.maxZoom = 100.0f; // Allow more zoom out for large world
            
            // Set world boundaries to match the tile world
            // Sprites are centered at their position, so tiles span from -0.5 to 99.5
            cameraController.worldMinX = -0.5f;
            cameraController.worldMinY = -0.5f;
            cameraController.worldMaxX = 99.5f; // Last tile at (99,99) extends to 99.5
            cameraController.worldMaxY = 99.5f; // Last tile at (99,99) extends to 99.5
            
            // Create world manager object
            var worldObject = scene.CreateGameObject("WorldManager");
            var worldManager = worldObject.AddComponent<WorldManager>();
            worldManager.worldWidth = 100;
            worldManager.worldHeight = 100;
            worldManager.tileSize = 1.0f;
            
            Console.WriteLine("Tile world scene created!");
        }
    }
} 