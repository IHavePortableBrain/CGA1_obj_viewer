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
            this.btnLoadObj = new System.Windows.Forms.Button();
            this.pbViewport = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbViewport)).BeginInit();
            this.SuspendLayout();
            // 
            // dlgOpenObjFile
            // 
            this.dlgOpenObjFile.FileName = "openFileDialog1";
            // 
            // btnLoadObj
            // 
            this.btnLoadObj.Location = new System.Drawing.Point(1830, 12);
            this.btnLoadObj.Name = "btnLoadObj";
            this.btnLoadObj.Size = new System.Drawing.Size(75, 23);
            this.btnLoadObj.TabIndex = 0;
            this.btnLoadObj.Text = "Load obj";
            this.btnLoadObj.UseVisualStyleBackColor = true;
            this.btnLoadObj.Click += new System.EventHandler(this.btnLoadObj_Click);
            // 
            // pbViewport
            // 
            this.pbViewport.Location = new System.Drawing.Point(0, 0);
            this.pbViewport.Name = "pbViewport";
            this.pbViewport.Size = new System.Drawing.Size(1820, 1080);
            this.pbViewport.TabIndex = 1;
            this.pbViewport.TabStop = false;
            this.pbViewport.DragOver += new System.Windows.Forms.DragEventHandler(this.pbViewport_DragOver);
            this.pbViewport.DoubleClick += new System.EventHandler(this.pbViewport_DoubleClick);
            this.pbViewport.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbViewport_MouseMove);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.pbViewport);
            this.Controls.Add(this.btnLoadObj);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pbViewport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dlgOpenObjFile;
        private System.Windows.Forms.Button btnLoadObj;
        private System.Windows.Forms.PictureBox pbViewport;
    }
}

