using lab1.World;
using ObjParser.Types;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

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

        public Pen Pen = new Pen(System.Drawing.Color.Black);
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
            var vectors = (Vector4[])_model.Vectors.Clone();
            TransformToViewport(vectors);
            foreach (var face in _model.Faces)
            {
                for (int i = 0; i < face.VertexIndicies.Length; i++)
                {
                    var startVertexIndex = face.VertexIndicies[i];
                    var endVertexIndex = face.VertexIndicies[(i + 1) % face.VertexIndicies.Length];
                    var startVector = vectors[startVertexIndex];
                    var endVector = vectors[endVertexIndex];

                    if (startVector.Z < 0 || startVector.Z > 1 || endVector.Z < 0 || endVector.Z > 1)
                        continue;
                    try
                    {
                        _image.DrawDdaLine((int)startVector.X, (int)startVector.Y, (int)endVector.X, (int)endVector.Y);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Exception while drawing {startVector} --> {endVector}");
                    }
                }
            });
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
