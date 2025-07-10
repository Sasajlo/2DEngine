using System;
using System.Numerics;

namespace Engine.Components
{
    /// <summary>
    /// A simple component that rotates a GameObject over time using Time.deltaTime
    /// </summary>
    public class Rotate : Component
    {
        public float speed = 45.0f; // Degrees per second
        private float _lastDebugTime = 0.0f;
        
        public override void Update()
        {
            // Debug output every 2 seconds
            _lastDebugTime += Time.deltaTime;
            if (_lastDebugTime >= 2.0f)
            {
                _lastDebugTime = 0.0f;
            }
            
            // Rotate the transform using Time.deltaTime for smooth, frame-rate independent rotation
            var currentRotation = gameObject.transform.rotation;
            var rotationChange = speed * Time.deltaTime;
            
            // Add rotation around Z-axis (2D rotation)
            var newRotation = new Vector3(
                currentRotation.X,
                currentRotation.Y,
                currentRotation.Z + rotationChange
            );
            
            gameObject.transform.rotation = newRotation;
        }
    }
} 