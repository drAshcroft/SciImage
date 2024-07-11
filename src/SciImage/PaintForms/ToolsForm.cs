/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PaintDotNet
{
    public class ToolsForm
        :Form  //FloatingToolForm
    {
       // private ToolsFormControl toolsControl = null;

        /*public  ToolsFormControl ToolsControl
        {
            set
            {
                this.ToolsFormControl1 = value;
                this.ToolsFormControl1.Visible = true;
                this.ToolsFormControl1.ToolsControl.Visible = true;
                this.ToolsFormControl1.ToolsControl.ForceVisible();
                this.ToolsFormControl1.Update();
            }
            get
            {
                return this.ToolsFormControl1;
            }
        }*/

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        public  void ResetSize()
        {
            this.ClientSize = new Size(ToolsFormControl1.ToolsControl.Width+5, ToolsFormControl1.ToolsControl.Height+5);
        }
        protected override void OnLoad(EventArgs e)
        {

            ToolsFormControl1.ForceResize();
            base.OnLoad(e);
            this.ClientSize = new Size(ToolsFormControl1.Width+5, ToolsFormControl1.Height+5);
        }

        public ToolsForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.Width = ToolsControl.Width+5;
           // this.Text = PdnResources.GetString("MainToolBarForm.Text");
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
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
            this.ToolsFormControl1 = new PaintDotNet.ToolsFormControl();
            this.SuspendLayout();
            // 
            // ToolsFormControl1
            // 
            this.ToolsFormControl1.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ToolsFormControl1.BackColor = System.Drawing.SystemColors.Control;
            this.ToolsFormControl1.Location = new System.Drawing.Point(0, 0);
            this.ToolsFormControl1.MinimumSize = new System.Drawing.Size(30, 158);
            this.ToolsFormControl1.Name = "ToolsFormControl1";
            this.ToolsFormControl1.Size = new System.Drawing.Size(51, 219);
            this.ToolsFormControl1.TabIndex = 0;
            this.ToolsFormControl1.Load += new System.EventHandler(this.ToolsFormControl1_Load);
            this.ToolsFormControl1.OnResize += new System.EventHandler(this.ToolsFormControl1_OnResize);
            // 
            // ToolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(53, 262);
            this.Controls.Add(this.ToolsFormControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ToolsForm";
            //this.TabText = "Tool Form";
            this.Text = "Tool Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ToolsForm_FormClosing);
            this.ResumeLayout(false);

        }

        

        public PaintDotNet.ToolsFormControl  ToolsFormControl1;
        #endregion
        public ToolsControl ToolsControl
        {
            get { return ToolsFormControl1.ToolsControl; }

        }
        private void ToolsControl_RelinquishFocus(object sender, EventArgs e)
        {
          //  OnRelinquishFocus();
        }

        private void ToolsFormControl1_Load(object sender, EventArgs e)
        {
           // ToolsFormControl1.Visible = true;
            ToolsFormControl1.ToolsControl.ForceVisible();
        }

        private void ToolsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            //this.Visible = false;
        }

        private void ToolsFormControl1_OnResize(object sender, EventArgs e)
        {
            this.ClientSize = new Size(ToolsFormControl1.Width+5, ToolsFormControl1.Height+5);
            //Point p = this.PointToScreen(new Point(0, 0));
            //this.DesktopBounds = new Rectangle(p.X,p.Y , ToolsFormControl1.Width, ToolsFormControl1.Height); 
        }
    }
}
