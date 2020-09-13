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
        public const float ScaleSpeed = 0.0001f;
        public const float RotationSpeed = 2 * (float) Math.PI / 180; // rotation degree count per 1 % viewport width dragged;
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
            _view3d = new View3d(_model, new Viewport()
            {
                Width = pbViewport.Width,
                Height = pbViewport.Height,
            });
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
            _model.Update();
            RedrawViewport();
        }

        private void pbViewport_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine($"Start rotating {_isChangingTarget}");
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
                Console.WriteLine($"End rotating {_isChangingTarget}");
                _isChangingTarget = false;
            }
        }

        private void pbViewport_MouseHover(object sender, EventArgs e)
        {
            Console.WriteLine($"Focus");
            pbViewport.Focus();
        }

        private void pbViewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (_freeView || _isChangingTarget)
            {
                var rotationY = ((e.X - _lastX) * RotationSpeed) % (float)(2 * Math.PI); // / _view3d.Viewport.Width 
                var rotationX = ((e.Y - _lastY) * RotationSpeed) % (float)(2 * Math.PI); // / _view3d.Viewport.Height
                _view3d.Cam.Yaw += rotationY;
                _view3d.Cam.Pitch += rotationX;
                Console.WriteLine($"Rot Y: {rotationY}, Rot X: {rotationX}");
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
                _view3d.Cam.Yaw = Constant.InitYaw;
                _view3d.Cam.Pitch = Constant.InitPitch;
                RedrawViewport();
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                _freeView = !_freeView;
            }
        }
        #endregion
    }
}
