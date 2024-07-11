namespace PaintDotNet
{
    partial class LayerForm
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
            this.layerFormControl1 = new PaintDotNet.LayerFormControl();
            this.SuspendLayout();
            // 
            // layerFormControl1
            // 
            this.layerFormControl1.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.layerFormControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layerFormControl1.Location = new System.Drawing.Point(0, 0);
            this.layerFormControl1.MinimumSize = new System.Drawing.Size(165, 158);
            this.layerFormControl1.Name = "layerFormControl1";
            this.layerFormControl1.Size = new System.Drawing.Size(292, 262);
            this.layerFormControl1.TabIndex = 0;
            // 
            // LayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 262);
            this.Controls.Add(this.layerFormControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LayerForm";
            //this.TabText = "LayerForm";
            this.Text = "LayerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LayerForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public  PaintDotNet.LayerFormControl layerFormControl1;

    }
}