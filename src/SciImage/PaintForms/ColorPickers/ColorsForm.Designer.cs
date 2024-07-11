namespace PaintDotNet.ColorPickers
{
    partial class ColorsForm
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
            this.ColorsControl1 = new PaintDotNet.ColorPickers.ColorsFormControl();
            this.SuspendLayout();
            // 
            // layerFormControl1
            // 
            this.ColorsControl1.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ColorsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ColorsControl1.Location = new System.Drawing.Point(0, 0);
            this.ColorsControl1.MinimumSize = new System.Drawing.Size(550, 158);
            this.ColorsControl1.Name = "ColorsControl1";
            
            this.ColorsControl1.Size = new System.Drawing.Size(292, 262);
            this.ColorsControl1.TabIndex = 0;
            // 
            // LayerForm
            // 
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 262);
            this.Controls.Add(this.ColorsControl1);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ColorsForm_FormClosing);
            this.Name = "ColorsForm";
            this.Text = "ColorsForm";
            this.ResumeLayout(false);
        }

        void ColorsForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        #endregion
        public  ColorsFormControl ColorsControl1;
    }
}