using System;
using System.Numerics;

namespace Engine
{
    /// <summary>
    /// Unity-style Input class for handling keyboard and mouse input
    /// </summary>
    public static class Input
    {
        // GLFW Key codes (commonly used ones)
        public static class KeyCode
        {
            public const int W = 87;
            public const int A = 65;
            public const int S = 83;
            public const int D = 68;
            public const int Q = 81;
            public const int E = 69;
            public const int Space = 32;
            public const int LeftShift = 340;
            public const int LeftControl = 341;
            public const int Escape = 256;
            public const int Enter = 257;
            public const int Up = 265;
            public const int Down = 264;
            public const int Left = 263;
            public const int Right = 262;
        }
        
        /// <summary>
        /// Returns true during the frame the user starts pressing down the key
        /// </summary>
        public static bool GetKeyDown(int keyCode)
        {
            return EngineCore.IsKeyPressed(keyCode);
        }
        
        /// <summary>
        /// Returns true while the user holds down the key
        /// </summary>
        public static bool GetKey(int keyCode)
        {
            return EngineCore.IsKeyHeld(keyCode);
        }
        
        /// <summary>
        /// Returns true during the frame the user releases the key
        /// </summary>
        public static bool GetKeyUp(int keyCode)
        {
            return EngineCore.IsKeyReleased(keyCode);
        }
        
        /// <summary>
        /// Returns the mouse scroll wheel delta for this frame
        /// </summary>
        public static float GetMouseScrollDelta()
        {
            return EngineCore.GetMouseScrollDelta();
        }
        
        /// <summary>
        /// Resets the mouse scroll delta (called internally by the engine)
        /// </summary>
        internal static void ResetMouseScrollDelta()
        {
            EngineCore.ResetMouseScrollDelta();
        }
        
        /// <summary>
        /// Returns the current mouse position in screen coordinates
        /// </summary>
        public static Vector2 GetMousePosition()
        {
            EngineCore.GetMousePosition(out float x, out float y);
            return new Vector2(x, y);
        }
        
        // Convenience methods for common keys
        public static bool GetKeyDown(string keyName) => GetKeyDown(GetKeyCodeFromName(keyName));
        public static bool GetKey(string keyName) => GetKey(GetKeyCodeFromName(keyName));
        public static bool GetKeyUp(string keyName) => GetKeyUp(GetKeyCodeFromName(keyName));
        
        private static int GetKeyCodeFromName(string keyName)
        {
            return keyName.ToLower() switch
            {
                "w" => KeyCode.W,
                "a" => KeyCode.A,
                "s" => KeyCode.S,
                "d" => KeyCode.D,
                "q" => KeyCode.Q,
                "e" => KeyCode.E,
                "space" => KeyCode.Space,
                "shift" => KeyCode.LeftShift,
                "ctrl" or "control" => KeyCode.LeftControl,
                "escape" => KeyCode.Escape,
                "enter" => KeyCode.Enter,
                "up" => KeyCode.Up,
                "down" => KeyCode.Down,
                "left" => KeyCode.Left,
                "right" => KeyCode.Right,
                _ => throw new ArgumentException($"Unknown key name: {keyName}")
            };
        }
    }
} 