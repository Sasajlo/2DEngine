using System;
using System.Numerics;

namespace Engine.Components
{
    public class SpriteRenderer : Component
    {
        public int sortingOrder { get; set; } = 0;
        public bool flipX { get; set; } = false;
        public bool flipY { get; set; } = false;
        
        private Sprite _sprite;
        private Transform _transform;
        
        public override void Awake()
        {
            _sprite = gameObject.GetComponent<Sprite>();
            _transform = gameObject.transform; // Unity-style access
        }
        
        public void Render()
        {
            if (_sprite == null || _transform == null || !enabled)
                return;
                
            // Get world transform matrix
            var worldMatrix = _transform.GetWorldMatrix();
            
            // Convert Matrix4x4 to float array
            var matrixArray = new float[16]
            {
                worldMatrix.M11, worldMatrix.M12, worldMatrix.M13, worldMatrix.M14,
                worldMatrix.M21, worldMatrix.M22, worldMatrix.M23, worldMatrix.M24,
                worldMatrix.M31, worldMatrix.M32, worldMatrix.M33, worldMatrix.M34,
                worldMatrix.M41, worldMatrix.M42, worldMatrix.M43, worldMatrix.M44
            };
            
            // Convert Vector4 to float array
            var colorArray = new float[4] { _sprite.color.X, _sprite.color.Y, _sprite.color.Z, _sprite.color.W };
            
            // Convert Vector2 to float array
            var sizeArray = new float[2] { _sprite.size.X, _sprite.size.Y };
            
            // Call C++ rendering function
            EngineCore.RenderSprite(
                _sprite.texturePath,
                matrixArray,
                colorArray,
                sizeArray,
                sortingOrder,
                flipX,
                flipY
            );
        }
    }
} 