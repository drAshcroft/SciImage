namespace PaintDotNet
{
    partial class PaintCanvas
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.appWorkspace1 = new PaintDotNet.AppWorkspace();
            this.SuspendLayout();
            // 
            // appWorkspace1
            // 
            this.appWorkspace1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appWorkspace1.Location = new System.Drawing.Point(0, 0);
            this.appWorkspace1.Name = "appWorkspace1";
            this.appWorkspace1.RulersEnabled = true;
            this.appWorkspace1.Size = new System.Drawing.Size(640, 473);
            this.appWorkspace1.TabIndex = 0;
            this.appWorkspace1.Units = PaintDotNet.MeasurementUnit.Pixel;
            // 
            // PaintCanvas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.appWorkspace1);
            this.Name = "PaintCanvas";
            this.Size = new System.Drawing.Size(640, 473);
            this.ResumeLayout(false);

        }

        #endregion

        private PaintDotNet.AppWorkspace appWorkspace1;
    }
}
