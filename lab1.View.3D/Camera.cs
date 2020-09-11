using System.Numerics;

namespace lab1.View._3D
{
    public class Camera
    {
        public float Speed = 0.1f;
        
        public Vector3 Eye = new Vector3(0, 0, 0);
        public Vector3 Target = Constant.SpawnPosition;
        public Vector3 Up = new Vector3(0, 1, 0);
        public Vector3 YAxis => Up;
        public Vector3 ZAxis => Vector3.Normalize(Eye - Target);
        public Vector3 XAxis => Vector3.Normalize(Vector3.Cross(Up, ZAxis));

        public Vector3 MoveForward(float time = 1f)
        {
            Eye += Vector3.Normalize(Target) * Speed * time;
            return Eye;
        }

        public Vector3 MoveBackward(float time = 1f)
        {
            Eye -= Vector3.Normalize(Target) * Speed * time;
            return Eye;
        }

        public Vector3 MoveLeft(float time = 1f)
        {
            Eye -= XAxis * Speed * time;
            return Eye;
        }

        public Vector3 MoveRight(float time = 1f)
        {
            Eye += XAxis * Speed * time;
            return Eye;
        }

        public void RotateX(float radian)
        {
            var rotation = XAxis.CreateRotation(radian);
            Target = Vector3.Transform(Target, rotation);
        }

        public void RotateY(float radian)
        {
            var rotation = YAxis.CreateRotation(radian);
            Target = Vector3.Transform(Target, rotation);
        }
    }
}
