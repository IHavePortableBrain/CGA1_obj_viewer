using System;
using System.Numerics;

namespace lab1.View._3D
{
    public class Camera
    {
        public readonly float MaxYaw = 2 * (float)Math.PI;
        public readonly float MaxPitch = (float)Math.PI / 2;
        public float Speed = 0.1f;

        public Vector3 Eye;
        public Vector3 Target { get; private set; }
        public Vector3 Up = new Vector3(0, 1, 0);
        public Vector3 YAxis => Vector3.Normalize(Vector3.Cross(XAxis, ZAxis));
        public Vector3 ZAxis => Vector3.Normalize(-Target); // +Eye 
        public Vector3 XAxis => Vector3.Normalize(Vector3.Cross(ZAxis, Up));

        private float _yaw; // rad
        public float Yaw { get { return _yaw; } set{ _yaw = value % MaxYaw; UpdateTarget(); } }
        private float _pitch; // rad
        public float Pitch { get { return _pitch; } set { _pitch = value % MaxPitch; UpdateTarget(); } }

        public Camera()
        {
            ResetEyeAndTarget();
            UpdateTarget();
        }

        public void ResetEyeAndTarget()
        {
            Eye = Constant.SpawnPosition - new Vector3(3, 0, 0);
            Yaw = Constant.InitYaw;
            Pitch = Constant.InitPitch;
        }

        public Vector3 MoveForward(float time = 1f)
        {
            var delta = Target * Speed * time;
            Eye += delta;
            return Eye;
        }

        public Vector3 MoveBackward(float time = 1f)
        {
            var delta = Target * Speed * time;
            Eye -= delta;
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

        private void UpdateTarget()
        {
            var x = (float)(Math.Cos(_yaw) * Math.Cos(_pitch));
            var y = (float)Math.Sin(_pitch);
            var z = (float)(Math.Sin(_yaw) * Math.Cos(_pitch));
            Target = Vector3.Normalize(new Vector3(x, y, z));
        }
    }
}
