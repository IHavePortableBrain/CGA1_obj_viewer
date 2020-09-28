using lab1.World;
using ObjParser.Types;
using System;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace lab1.View._3D
{
    public class View3d
    {
        private const float _FOV = (float)(60 * Math.PI / 180);
        private const float _zFar = 1000;
        private const float _zNear = 0.1f;

        public float Aspect => Viewport.Width / (float)Viewport.Height;
        public readonly Viewport Viewport;
        private readonly Model _model;
        private Bitmap _image;
        private float[] _zBuffer;

        public System.Drawing.Color Color = System.Drawing.Color.White;
        public Camera Cam = new Camera();

        private Matrix4x4 ToObserverSpaceTransform => //Matrix4x4.CreateLookAt(_eye, _target, _zAxis);
            Matrix4x4.Transpose(
                new Matrix4x4(
                    Cam.XAxis.X, Cam.XAxis.Y, Cam.XAxis.Z, -Vector3.Dot(Cam.XAxis, Cam.Eye),
                    Cam.YAxis.X, Cam.YAxis.Y, Cam.YAxis.Z, -Vector3.Dot(Cam.YAxis, Cam.Eye),
                    Cam.ZAxis.X, Cam.ZAxis.Y, Cam.ZAxis.Z, -Vector3.Dot(Cam.ZAxis, Cam.Eye),
                    0, 0, 0, 1));

        private Matrix4x4 ToProjectionSpaceTransform => //Matrix4x4.CreatePerspectiveFieldOfView(_FOV, _aspect, _zNear, _zFar);
            Matrix4x4.Transpose(
                new Matrix4x4(
                    1 / (Aspect * (float)Math.Tan(_FOV / 2)), 0, 0, 0,
                    0, 1 / (float)Math.Tan(_FOV / 2), 0, 0,
                    0, 0, _zFar / (_zNear - _zFar), _zNear * _zFar / (_zNear - _zFar),
                    0, 0, -1, 0));

        private Matrix4x4 ToViewportSpaceTransform =>
            Matrix4x4.Transpose(
                new Matrix4x4(
                    Viewport.Width / 2f, 0, 0, Viewport.MinX + Viewport.Width / 2f,
                    0, - Viewport.Height / 2f, 0, Viewport.MinY + Viewport.Height / 2f,
                    0, 0, 1, 0,
                    0, 0, 0, 1));

        public View3d(Model model, Viewport viewport)
        {
            _model = model;
            Viewport = viewport;
        }

        public Image Redraw()
        {
            _image = new Bitmap(Viewport.Width, Viewport.Height);
            _zBuffer = Enumerable.Repeat(float.MaxValue, Viewport.Width * Viewport.Height).ToArray();
            var vectors = (Vector4[])_model.Vectors.Clone();
            TransformToViewport(vectors);
            foreach (var face in _model.Faces)
            {
                var vis = face.VertexIndicies;
                var verticies = vis.Select(vi => vectors[vi]).ToArray();
                Vector4 faceNormal = Vector4.Normalize(
                    Vector4.Multiply(
                    _model.Vectors[vis[2]] - _model.Vectors[vis[0]], _model.Vectors[vis[1]] - _model.Vectors[vis[0]]));
                float intensity = Vector4.Dot(faceNormal, _model.LightDirection);
                //if (intensity > 0)
                //{
                    var color = System.Drawing.Color.FromArgb(Color.A, (byte)(Color.R * intensity), (byte)(Color.G * intensity), (byte)(Color.B * intensity));  
                    _image.RasterizeTriangle(_zBuffer, verticies, color);
                //}
            }
            return _image;
        }

        private void TransformToViewport(Vector4[] vectors)
        {
            // await Task.CompletedTask;
            var toObserver = ToObserverSpaceTransform;
            var toProjection = ToProjectionSpaceTransform;
            var toViewport = ToViewportSpaceTransform;
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = Vector4.Transform(vectors[i], toObserver);
                //if (vectors.Next and v.Curr < _znear or >_zFar)add to unobserved
                vectors[i] = Vector4.Transform(vectors[i], toProjection);
                vectors[i] = Vector4.Divide(vectors[i], vectors[i].W);
                vectors[i] = Vector4.Transform(vectors[i], toViewport);
            };
        }

        #region trash

        private Vector4? TransformToWorld(Vector4 v)
        {
            if (!Matrix4x4.Invert(ToViewportSpaceTransform, out var invert)
                || !Matrix4x4.Invert(ToProjectionSpaceTransform, out invert))
                return default;
            v = Vector4.Divide(v, v.W);
            v = Vector4.Transform(v, invert);
            if (!Matrix4x4.Invert(ToObserverSpaceTransform, out invert))
                return default;
            v = Vector4.Transform(v, invert);
            return v;
        }

        #endregion
    }
}
