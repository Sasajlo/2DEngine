using System;

namespace Engine
{
    /// <summary>
    /// Unity-style Time class for accessing deltaTime and time values
    /// </summary>
    public static class Time
    {
        /// <summary>
        /// The time in seconds it took to complete the last frame (Read Only)
        /// </summary>
        public static float deltaTime => EngineCore.GetDeltaTime();
        
        /// <summary>
        /// The time at the beginning of this frame (Read Only)
        /// </summary>
        public static double time => EngineCore.GetTime();
        
        /// <summary>
        /// The time in seconds since the engine started (Read Only)
        /// </summary>
        public static double timeSinceStartup => EngineCore.GetTime();
        
        /// <summary>
        /// The fixed time interval in seconds at which physics and other fixed frame rate updates are performed
        /// </summary>
        public static float fixedDeltaTime { get; set; } = 0.02f; // Default to 50 FPS for physics
        
        /// <summary>
        /// The time scale at which time passes. This can be used for slow motion effects
        /// </summary>
        public static float timeScale { get; set; } = 1.0f;
        
        /// <summary>
        /// The scaled deltaTime (deltaTime * timeScale)
        /// </summary>
        public static float scaledDeltaTime => deltaTime * timeScale;
        
        /// <summary>
        /// The unscaled deltaTime (not affected by timeScale)
        /// </summary>
        public static float unscaledDeltaTime => deltaTime;
        
        /// <summary>
        /// The frame rate in frames per second (Read Only)
        /// </summary>
        public static float frameRate => deltaTime > 0 ? 1.0f / deltaTime : 0.0f;
        
        /// <summary>
        /// Reset the time to zero (used internally by the engine)
        /// </summary>
        internal static void Reset()
        {
            EngineCore.ResetTime();
        }
        
        /// <summary>
        /// Update the delta time calculation (used internally by the engine)
        /// </summary>
        internal static void Update()
        {
            EngineCore.UpdateDeltaTime();
        }
    }
} 