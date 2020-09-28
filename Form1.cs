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
        public const float ScaleSpeed = 0.0_001f;
        public const float CameraRotationSpeed = 2 * 100 * (float) Math.PI / 180; // rotation degree count per 1 % viewport width dragged;
        private bool _isChangingTarget;
        private bool _freeView;
        private float _lastX;
        private float _lastY;

        private readonly Model _model;
        private readonly View3d _view3d;

        public Form1()
        {
            InitializeComponent();
            pbViewport.MouseWheel += PbViewport_MouseWheel;
            pbViewport.PreviewKeyDown += PbViewport_PreviewKeyDown;

            _model = new Model(new Obj())
            {
                MoveTranslation = Matrix4x4.CreateTranslation(Constant.SpawnPosition),
            };
            _view3d = new View3d(_model, new Viewport());
        }

        private void RedrawViewport()
        {
            try
            {
                pbViewport.Image = _view3d.Redraw();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
            _model.Update();
            RedrawViewport();
        }

        private void pbViewport_MouseDown(object sender, MouseEventArgs e)
        {
            if (e is MouseEventArgs me)
            {
                _isChangingTarget = true;
                _lastX = me.X;
                _lastY = me.Y;
            }
        }

        private void pbViewport_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isChangingTarget)
            {
                _isChangingTarget = false;
            }
        }

        private void pbViewport_MouseHover(object sender, EventArgs e)
        {
            pbViewport.Focus();
        }

        private void pbViewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (_freeView || _isChangingTarget)
            {
                var rotationY = ((_lastX - e.X) / _view3d.Viewport.Width * CameraRotationSpeed) % (float)(2 * Math.PI);
                var rotationX = ((_lastY - e.Y) / _view3d.Viewport.Height * CameraRotationSpeed) % (float)(2 * Math.PI);
                _view3d.Cam.Yaw += rotationY;
                _view3d.Cam.Pitch += rotationX;
                _lastX = e.X;
                _lastY = e.Y;
                
                RedrawViewport();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O && dlgOpenObjFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _model.Obj.Load(dlgOpenObjFile.FileName);
                    _model.Reload();
                    RedrawViewport();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                        $"Details:\n\n{ex.StackTrace}");
                }
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                _view3d.Cam.ResetEyeAndTarget();
                RedrawViewport();
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                _freeView = !_freeView;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _view3d.Viewport.Width = Width;
            _view3d.Viewport.Height = Height;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (_view3d?.Viewport != null)
            {
                _view3d.Viewport.Width = Width;
                _view3d.Viewport.Height = Height;
                RedrawViewport();
            }
        }
        #endregion
    }
}
