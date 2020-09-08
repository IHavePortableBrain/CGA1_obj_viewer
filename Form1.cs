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
        public const float ScaleSpeed = 10f;
        public const float RotationSpeed = 0.3f;
        private bool _isRotating = false;
        private float _lastX;
        private float _lastY;

        private Obj _obj = new Obj();
        private Model _model;
        private View3d _view3d;
        private Viewport _viewport;

        public Form1()
        {
            InitializeComponent();
            pbViewport.MouseWheel += PbViewport_MouseWheel; 
            _model = new Model(_obj);
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
            if (_obj != null)
            {
                _model.Scale += e.Delta * ScaleSpeed;
                _model.Update();
                RedrawViewport();
            }
        }

        private void pbViewport_DoubleClick(object sender, EventArgs e)
        {
            _isRotating = true;
            if (e is MouseEventArgs me)
            {
                _lastX = me.X;
                _lastY = me.Y;
            }
        }

        private void pbViewport_DragOver(object sender, DragEventArgs e)
        {
            _isRotating = false;
        }

        private void pbViewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isRotating)
            {
                var q = _model.Quaternion;
                q.X = ((q.X + (e.X - _lastX) * RotationSpeed)) % (float)(2 * Math.PI);
                q.Y = ((q.Y + (e.Y - _lastY) * RotationSpeed)) % (float)(2 * Math.PI);
                _model.Quaternion = q;
                _lastX = e.X;
                _lastY = e.Y;
                _model.Update();
                RedrawViewport();
            }
        }

        #endregion
    }
}
