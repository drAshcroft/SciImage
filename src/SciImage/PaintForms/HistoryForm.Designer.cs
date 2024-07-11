namespace PaintDotNet
{
    partial class HistoryForm
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
            this.HistoryControl = new PaintDotNet.HistoryFormControl();
            this.SuspendLayout();
            // 
            // HistoryControl
            // 
            this.HistoryControl.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.HistoryControl.BackColor = System.Drawing.Color.White;
            this.HistoryControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HistoryControl.Location = new System.Drawing.Point(0, 0);
            this.HistoryControl.MinimumSize = new System.Drawing.Size(165, 158);
            this.HistoryControl.Name = "HistoryControl";
            this.HistoryControl.Size = new System.Drawing.Size(292, 262);
            this.HistoryControl.TabIndex = 0;
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 262);
            this.Controls.Add(this.HistoryControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "HistoryForm";
            //this.TabText = "HistoryForm";
            this.Text = "HistoryForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HistoryForm_FormClosing);
            this.ResumeLayout(false);

        }
        public  HistoryFormControl HistoryControl;
        #endregion
    }
}