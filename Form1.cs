using lab1.View._3D;
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
        private Obj _obj;
        private View3d _view3d;
        private Viewport _viewport;
        private Vector4[] _vectors;
        private Bitmap _initImage;
        private Pen _pen = new Pen(Color.Black);

        public Form1()
        {
            InitializeComponent();
            _viewport = new Viewport()
            {
                Width = pbViewport.Width,
                Height = pbViewport.Height,
            };
            _initImage = new Bitmap(pbViewport.Width, pbViewport.Height);
            pbViewport.Image = _initImage;
        }

        private async void btnLoadObj_Click(object sender, EventArgs e)
        {
            if (dlgOpenObjFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _obj = new Obj();
                    _obj.LoadObj(dlgOpenObjFile.FileName);
                    _vectors = _obj.Verticies.Select(v => v.Vector).ToArray();
                    _view3d = new View3d(_viewport)
                    {
                        ModelTransform = Matrix4x4.CreateTranslation(0, 0, 1)
                    };
                    _view3d.Recalculate(_vectors);
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
            using (var currentImage = new Bitmap(_initImage))
            using (var graphics = Graphics.FromImage(currentImage))
            {
                foreach (var face in _obj.Faces)
                {
                    for (int i = 0; i < face.VertexIndicies.Length - 1; i++)
                    {
                        var startVertexIndex = face.VertexIndicies[i] - 1; // array starts from 0, obj index starts from 1
                        var endVertexIndex = face.VertexIndicies[i + 1] - 1;
                        var startVector = _vectors[startVertexIndex]; // bad - relying on _view3d.Recalculate and .Select(v => v.Vector).ToArray() do not change order 
                        var endVector = _vectors[endVertexIndex];

                        //currentImage.DrawDdaLine((int)startVector.X, (int)startVector.Y, (int)endVector.X, (int)endVector.Y);
                        try
                        {
                            graphics.DrawLine(_pen, (int)startVector.X, (int)startVector.Y, (int)endVector.X, (int)endVector.Y);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                //_graphics.DrawImageUnscaled()
                pbViewport.Image = currentImage;
                pbViewport.Refresh();
            }
        }
    }
}
