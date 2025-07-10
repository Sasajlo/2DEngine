using System;
using System.Numerics;

namespace Engine.Components
{
    public class Transform : Component
    {
        public Vector3 position { get; set; } = Vector3.Zero;
        public Vector3 rotation { get; set; } = Vector3.Zero;
        public Vector3 scale { get; set; } = Vector3.One;
        
        public Transform parent { get; set; }
        
        public Matrix4x4 GetWorldMatrix()
        {
            var translationMatrix = Matrix4x4.CreateTranslation(position);
            
            // Convert degrees to radians for CreateFromYawPitchRoll
            var rotationRadians = new Vector3(
                rotation.X * MathF.PI / 180.0f,
                rotation.Y * MathF.PI / 180.0f,
                rotation.Z * MathF.PI / 180.0f
            );
            
            var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(rotationRadians.Y, rotationRadians.X, rotationRadians.Z);
            var scaleMatrix = Matrix4x4.CreateScale(scale);
            
            var localMatrix = scaleMatrix * rotationMatrix * translationMatrix;
            
            if (parent != null)
            {
                return localMatrix * parent.GetWorldMatrix();
            }
            
            return localMatrix;
        }
        
        public Vector3 GetWorldPosition()
        {
            if (parent != null)
            {
                return Vector3.Transform(position, parent.GetWorldMatrix());
            }
            return position;
        }
    }
} 