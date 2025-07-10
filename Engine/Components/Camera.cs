using System;
using System.Numerics;

namespace Engine.Components
{
    public class Camera : Component
    {
        public float orthographicSize { get; set; } = 5.0f;
        public float near { get; set; } = 0.1f;
        public float far { get; set; } = 100.0f;
        public Vector4 backgroundColor { get; set; } = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
        
        public static Camera mainCamera { get; private set; }
        
        private Transform _transform;
        
        public override void Awake()
        {
            _transform = gameObject.transform; // Unity-style access
            
            // Set as main camera if none exists
            if (mainCamera == null)
            {
                mainCamera = this;
            }
        }
        
        public override void OnDestroy()
        {
            if (mainCamera == this)
            {
                mainCamera = null;
            }
        }
        
        public Matrix4x4 GetViewMatrix()
        {
            if (_transform == null)
                return Matrix4x4.Identity;
                
            var worldMatrix = _transform.GetWorldMatrix();
            Matrix4x4.Invert(worldMatrix, out var viewMatrix);
            return viewMatrix;
        }
        
        public Matrix4x4 GetProjectionMatrix(float aspectRatio)
        {
            float height = orthographicSize * 2.0f;
            float width = height * aspectRatio;
            
            return Matrix4x4.CreateOrthographic(width, height, near, far);
        }
        
        public void SetAsMainCamera()
        {
            mainCamera = this;
        }
    }
} 