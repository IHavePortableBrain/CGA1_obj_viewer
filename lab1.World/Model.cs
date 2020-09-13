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
        //public Quaternion Quaternion { get; set; }

        public readonly Obj Obj; // IModelSource
        public Matrix4x4 MoveTranslation;
        private Matrix4x4 ModelTransform => MoveTranslation * Matrix4x4.CreateScale(Scale); //SRT - scale rotate then translate  Matrix4x4.CreateFromQuaternion(Quaternion) *
        public Vector4[] Vectors = Array.Empty<Vector4>();
        public Face[] Faces = Array.Empty<Face>();

        public Model(Obj obj) // todo declare IModelSource, and use instead obj
        {
            Obj = obj;
        }

        public void Reload()
        {
            Scale = 1;
            // Quaternion = new Quaternion(0, 0, 0, 1);
            Vectors = Obj.Verticies.Select(v => v.Vector).ToArray();
            Faces = Obj.Faces.ToArray();
            Update();
        }

        public void Update()
        {
            for (int i = 0; i < Vectors.Length; i++)
            {
                Vectors[i] = Vector4.Transform(Vectors[i], Matrix4x4.CreateScale(Scale));
                Vectors[i] = Vector4.Transform(Vectors[i], MoveTranslation);
            }
            ResetTransforms();
        }

        private void ResetTransforms()
        {
            MoveTranslation = Matrix4x4.Identity;
            Scale = 1;
            // Quaternion = new Quaternion(0, 0, 0, 1);
        }
    }
}
