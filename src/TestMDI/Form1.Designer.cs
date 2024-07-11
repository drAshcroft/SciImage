using SciImage.Core;

namespace Test
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
            this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.appWorkspace1 = new SciImage.AppWorkspace();
            this.SuspendLayout();
            // 
            // DockPanel
            // 
            this.DockPanel.ActiveAutoHideContent = null;
            this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DockPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.DockPanel.Location = new System.Drawing.Point(0, 104);
            this.DockPanel.Name = "DockPanel";
            this.DockPanel.ShowDocumentIcon = true;
            this.DockPanel.Size = new System.Drawing.Size(660, 246);
            this.DockPanel.TabIndex = 30;
            this.DockPanel.ActiveContentChanged += new System.EventHandler(this.DockPanel_ActiveContentChanged);
            this.DockPanel.ActiveDocumentChanged += new System.EventHandler(this.DockPanel_ActiveDocumentChanged);
            // 
            // appWorkspace1
            // 
          
           
            this.appWorkspace1.Dock = System.Windows.Forms.DockStyle.Top;
            
            this.appWorkspace1.Location = new System.Drawing.Point(0, 0);
            
            this.appWorkspace1.Name = "appWorkspace1";
           
            this.appWorkspace1.Size = new System.Drawing.Size(660, 104);
            this.appWorkspace1.TabIndex = 27;
           
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 350);
            this.Controls.Add(this.DockPanel);
            this.Controls.Add(this.appWorkspace1);
            this.IsMdiContainer = true;
            this.Name = "Form1";
            this.Text = "SciImage";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        
        private SciImage.AppWorkspace appWorkspace1;
        internal WeifenLuo.WinFormsUI.Docking.DockPanel DockPanel;
    }
}

