using lab1.View._3D;
using lab1.World;
using ObjParser;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {
        public const float ScaleSpeed = 0.01f;
        public const float MinScale = 0.1f;
        public const float MaxScale = 10f;
        public const float RotationSpeed = 15 * (float)Math.PI / 180; // rotation degree count per 1 % viewport width dragged;
        private bool _isRotating = false;
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
            _model = new Model(_obj)
            {
                MoveTranslation = Matrix4x4.CreateTranslation(0, 0, 1),
            };
            _viewport = new Viewport()
            {
                Width = pbViewport.Width,
                Height = pbViewport.Height,
            };
            _view3d = new View3d(_model, _viewport);
        }

        private async void btnLoadObj_Click(object sender, EventArgs e)
        {
            if (dlgOpenObjFile.ShowDialog() == DialogResult.OK)
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
        
        private void RedrawViewport()
        {
            pbViewport.Image = _view3d.Redraw();
            pbViewport.Refresh();
        }

        #region ui

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
                //var q = _model.Quaternion;
                //q.X = (q.X + (e.X - _lastX) / _viewport.Width * RotationSpeed) % (float)(2 * Math.PI);
                //q.Y = (q.Y + (e.Y - _lastY) / _viewport.Width * RotationSpeed) % (float)(2 * Math.PI);
                //// Console.WriteLine($"X quaternation: {q.X}, Y: {q.Y}");
                //_model.Quaternion = q;
                //_lastX = e.X;
                //_lastY = e.Y;
                //_model.Update();
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

        #endregion
    }
}
