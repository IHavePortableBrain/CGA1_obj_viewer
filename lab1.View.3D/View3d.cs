using lab1.World;
using System;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace lab1.View._3D
{
    public class View3d
    {
        private const float _FOV = (float)(110 * Math.PI / 180);
        private const float _aspect = 2.39f;
        private const float _zFar = 1000;
        private const float _zNear = 0.1f;

        private readonly Model _model;
        private readonly Viewport _viewport;
        private Bitmap _image;

        public Pen Pen = new Pen(Color.Black);
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
                    1 / (_aspect * (float)Math.Tan(_FOV / 2)), 0, 0, 0,
                    0, 1 / (float)Math.Tan(_FOV / 2), 0, 0,
                    0, 0, _zFar / (_zNear - _zFar), _zNear * _zFar / (_zNear - _zFar),
                    0, 0, -1, 0));
        private Matrix4x4 ToViewportSpaceTransform =>
            Matrix4x4.Transpose(
                new Matrix4x4(
                    _viewport.Width / 2, 0, 0, _viewport.MinX + _viewport.Width / 2,
                    0, - _viewport.Height / 2, 0, _viewport.MinY + _viewport.Height / 2,
                    0, 0, 1, 0,
                    0, 0, 0, 1));

        public View3d(Model model, Viewport viewport)
        {
            _model = model;
            _viewport = viewport;
        }

        public Image Redraw()
        {
            _image = new Bitmap(_viewport.Width, _viewport.Height);
            using (var graphics = Graphics.FromImage(_image))
            {
                var modelVectors = (Vector4[])_model.Vectors.Clone();
                var vectors = CalculateViewportVectors(modelVectors);
                Parallel.ForEach(_model.Faces, face =>
                {
                    for (int i = 0; i < face.VertexIndicies.Length - 1; i++)
                    {
                        var startVertexIndex = face.VertexIndicies[i] - 1; // array starts from 0, obj index starts from 1
                        var endVertexIndex = face.VertexIndicies[i + 1] - 1;
                        var startVector = vectors[startVertexIndex];
                        var endVector = vectors[endVertexIndex];
                        if (startVector.Z < 0 && endVector.Z < 0)
                        {
                            continue;
                        }

                        try
                        {
                            //_image.DrawDdaLine((int)startVector.X, (int)startVector.Y, (int)endVector.X, (int)endVector.Y);
                            graphics.DrawLine(Pen, (int)startVector.X, (int)startVector.Y, (int)endVector.X, (int)endVector.Y);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Exception while drawing {startVector} --> {endVector}");
                        }
                    }
                });
                //_graphics.DrawImageUnscaled()
                return _image;
            }
        }

        private Vector4[] CalculateViewportVectors(Vector4[] vectors)
        {
            // await Task.CompletedTask;
            var toObserver = ToObserverSpaceTransform;
            var toProjection = ToProjectionSpaceTransform;
            var toViewport = ToViewportSpaceTransform;
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = Vector4.Transform(vectors[i], toObserver);
                vectors[i] = Vector4.Transform(vectors[i], toProjection);
                vectors[i] = Vector4.Divide(vectors[i], vectors[i].W); //
                vectors[i] = Vector4.Transform(vectors[i], toViewport);
            }
            return vectors;
        }

        #region trash
            public void UpdateCameraTarget(int x, int y)
        {
            //var toWorld = ToWorldTransform;
            //if (toWorld.HasValue)
            //{
            var viewPortTarget = new Vector4(x, y, Cam.Eye.Z, 1);
            var target = TransformToWorld(viewPortTarget); //Vector4.Transform(viewPortTarget, toWorld.Value);
            if (target.HasValue)
            {
                var v = target.Value;
                Cam.Target = new Vector3(v.X, v.Y, v.Z);
            }

            //}
        }

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
