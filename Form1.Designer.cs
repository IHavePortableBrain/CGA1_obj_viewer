namespace lab1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dlgOpenObjFile = new System.Windows.Forms.OpenFileDialog();
            this.pbViewport = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbViewport)).BeginInit();
            this.SuspendLayout();
            // 
            // dlgOpenObjFile
            // 
            this.dlgOpenObjFile.FileName = "openFileDialog1";
            // 
            // pbViewport
            // 
            this.pbViewport.Location = new System.Drawing.Point(0, 0);
            this.pbViewport.Name = "pbViewport";
            this.pbViewport.Size = new System.Drawing.Size(1920, 1080);
            this.pbViewport.TabIndex = 1;
            this.pbViewport.TabStop = false;
            this.pbViewport.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbViewport_MouseDown);
            this.pbViewport.MouseLeave += new System.EventHandler(this.pbViewport_MouseLeave);
            this.pbViewport.MouseHover += new System.EventHandler(this.pbViewport_MouseHover);
            this.pbViewport.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbViewport_MouseMove);
            this.pbViewport.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbViewport_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.pbViewport);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbViewport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dlgOpenObjFile;
        private System.Windows.Forms.PictureBox pbViewport;
    }
}

