using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace lab1.View._3D
{
    public class Camera
    {
        public float Speed = 0.3f;
        
        public Vector3 Eye = new Vector3(0, 0, 0);
        public Vector3 Target = new Vector3(0, 0, 5);
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
    }
}
