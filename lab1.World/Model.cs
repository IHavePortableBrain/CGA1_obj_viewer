using ObjParser;
using ObjParser.Types;
using System;
using System.Linq;
using System.Numerics;

namespace lab1.World
{
    public class Model
    {
        public float Scale { get; set; }
        public Quaternion Quaternion { get; set; }

        public Matrix4x4 MoveTranslation;
        private Matrix4x4 ModelTransform => Matrix4x4.CreateFromQuaternion(Quaternion) * Matrix4x4.CreateScale(Scale) * MoveTranslation; // * ; // 
        private Obj _obj; // IModelSource
        public Vector4[] Vectors = Array.Empty<Vector4>(); // todo must be private set
        public Face[] Faces = Array.Empty<Face>(); // todo must be private set

        public Model(Obj obj) // todo declare IModelSource, and use instead obj
        {
            _obj = obj;
        }

        public void Reload()
        {
            Scale = 1;
            Quaternion = new Quaternion(0, 0, 0, 1);
            Vectors = _obj.Verticies.Select(v => v.Vector).ToArray();
            Faces = _obj.Faces.ToArray();
            Update();
        }

        public void Update()
        {
            for (int i = 0; i < Vectors.Length; i++)
            {
                Vectors[i] = Vector4.Transform(Vectors[i], ModelTransform);
            }
            ResetTransforms();
        }

        private void ResetTransforms()
        {
            MoveTranslation = Matrix4x4.Identity;
            Scale = 1;
            Quaternion = new Quaternion(0, 0, 0, 1);
        }
    }
}
