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

            Console.WriteLine("Demo scene created. Window should now be visible. Close it to exit.");
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
            var scene = engine.sceneManager.CreateScene("DemoScene");
            
            // Create camera object
            var cameraObject = scene.CreateGameObject("MainCamera");
            
            // Set camera transform (Unity-style access)
            cameraObject.transform.position = new Vector3(0.0f, 0.0f, 10.0f);
            
            // Add camera component
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographicSize = 5.0f;
            camera.backgroundColor = new Vector4(0.2f, 0.3f, 0.4f, 1.0f); // Nice blue background
            
            // Add camera controller for WASD movement and mouse scroll zoom
            var cameraController = cameraObject.AddComponent<CameraController>();
            cameraController.moveSpeed = 1.0f; // Base movement speed (scaled by zoom level)
            cameraController.zoomSpeed = 0.15f; // Smooth percentage-based zoom (15% per scroll)
            cameraController.minZoom = 1.0f;
            cameraController.maxZoom = 15.0f;
            
            // Create sprite object
            var spriteObject = scene.CreateGameObject("Sprite");
            
            // Set transform (Unity-style access)
            spriteObject.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            spriteObject.transform.scale = new Vector3(2.0f, 2.0f, 1.0f);
            
            // Add sprite component
            var sprite = spriteObject.AddComponent<Sprite>();
            sprite.texturePath = "Assets/Textures/square.png";
            sprite.color = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); // Blue color
            sprite.size = new Vector2(2.0f, 1.0f);
            
            // Add sprite renderer component
            var spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 0;
            
            // Add rotation component to demonstrate deltaTime
            var rotateComponent = spriteObject.AddComponent<Rotate>();
            rotateComponent.speed = 45.0f; // 45 degrees per second
            
            // Create a second sprite with different color and position
            var spriteObject2 = scene.CreateGameObject("Sprite2");
            
            // Set transform (Unity-style access)
            spriteObject2.transform.position = new Vector3(3.0f, 1.0f, 0.0f);
            spriteObject2.transform.rotation = new Vector3(0.0f, 0.0f, 45.0f);
            spriteObject2.transform.scale = new Vector3(1.5f, 1.5f, 1.0f);
            
            var sprite2 = spriteObject2.AddComponent<Sprite>();
            sprite2.texturePath = "Assets/Textures/square.png";
            sprite2.color = new Vector4(1.0f, 0.5f, 0.0f, 1.0f); // Orange color
            sprite2.size = new Vector2(1.0f, 1.0f);
            
            var spriteRenderer2 = spriteObject2.AddComponent<SpriteRenderer>();
            spriteRenderer2.sortingOrder = 1;
        
            
            Console.WriteLine("Demo scene created!");
        }
    }
} 