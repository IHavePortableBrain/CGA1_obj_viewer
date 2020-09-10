using lab1.View._3D;
using lab1.World;
using ObjParser;
using System;
using System.Numerics;
using System.Security;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {
        public const float ScaleSpeed = 0.7f;
        public const float MinScale = 0.1f;
        public const float MaxScale = 10f;
        public const float RotationSpeed = 15 * (float)Math.PI / 180; // rotation degree count per 1 % viewport width dragged;
        private bool _isRotating = false;
        private bool _isChangingTarget = true;
        private float _lastX;
        private float _lastY;

        private readonly Obj _obj = new Obj();
        private readonly Model _model;
        private readonly View3d _view3d;
        private readonly Viewport _viewport;

        public Form1()
        {
            InitializeComponent();
            pbViewport.MouseWheel += PbViewport_MouseWheel;
            pbViewport.LostFocus += PbViewport_LostFocus;
            pbViewport.PreviewKeyDown += PbViewport_PreviewKeyDown;
            var q = new Quaternion(0, (float)Math.PI / 12, 0, 1);
            var test = Matrix4x4.CreateFromQuaternion(q);
            _model = new Model(_obj)
            {
                MoveTranslation = Matrix4x4.Multiply(
                    test,
                    Matrix4x4.CreateTranslation(0, 0, 7)), //(float)Math.PI/4
            };
            _viewport = new Viewport()
            {
                Width = pbViewport.Width,
                Height = pbViewport.Height,
            };
            _view3d = new View3d(_model, _viewport);
        }

        private void RedrawViewport()
        {
            pbViewport.Image = _view3d.Redraw();
            pbViewport.Refresh();
        }

        #region ui
        private void PbViewport_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _view3d.Cam.MoveForward();
                    RedrawViewport();
                    break;
                case Keys.S:
                    _view3d.Cam.MoveBackward();
                    RedrawViewport();
                    break;
                case Keys.A:
                    _view3d.Cam.MoveLeft();
                    RedrawViewport();
                    break;
                case Keys.D:
                    _view3d.Cam.MoveRight();
                    RedrawViewport();
                    break;
            }
        }

        private void PbViewport_MouseWheel(object sender, MouseEventArgs e)
        {
            _model.Scale = 1 + e.Delta * ScaleSpeed;
            _model.Scale = _model.Scale > MaxScale ? MaxScale : _model.Scale;
            _model.Scale = _model.Scale < MinScale ? MinScale : _model.Scale;
            _model.Update();
            RedrawViewport();
        }

        private void pbViewport_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine($"Start rotating {_isRotating}");
            if (e is MouseEventArgs me)
            {
                _isRotating = true;
                _lastX = me.X;
                _lastY = me.Y;
            }
        }

        private void pbViewport_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isRotating)
            {
                Console.WriteLine($"End rotating {_isRotating}");
                _isRotating = false;
            }
        }

        private void pbViewport_MouseHover(object sender, EventArgs e)
        {
            Console.WriteLine($"Focus");
            pbViewport.Focus();
        }

        private void pbViewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isRotating)
            {
                var q = _model.Quaternion;
                q.X = (q.X + (e.X - _lastX) / _viewport.Width * RotationSpeed) % (float)(2 * Math.PI);
                q.Y = (q.Y + (e.Y - _lastY) / _viewport.Width * RotationSpeed) % (float)(2 * Math.PI);
                _model.Quaternion = q;
                _lastX = e.X;
                _lastY = e.Y;
                _model.Update();
                RedrawViewport();
            }
            if (_isChangingTarget)
            {
                //_view3d.UpdateCameraTarget(e.X, e.Y);
                //RedrawViewport();
            }
        }

        private void PbViewport_LostFocus(object sender, EventArgs e)
        {
            Console.WriteLine("Lost focus");
            _isRotating = false;
        }

        private void pbViewport_MouseLeave(object sender, EventArgs e)
        {
            Console.WriteLine("Mouse leave");
            _isRotating = false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O && dlgOpenObjFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _obj.LoadObj(dlgOpenObjFile.FileName);
                    _model.Reload();
                    RedrawViewport();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                        $"Details:\n\n{ex.StackTrace}");
                }
            }
        }
        #endregion
    }
}
