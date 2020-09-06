using ObjParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {
        private Obj _obj;
        
        public Form1()
        {
            InitializeComponent();

        }

        private void btnLoadObj_Click(object sender, EventArgs e)
        {
            if (dlgOpenObjFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _obj = new Obj();
                    _obj.LoadObj(dlgOpenObjFile.FileName);
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }
    }
}
