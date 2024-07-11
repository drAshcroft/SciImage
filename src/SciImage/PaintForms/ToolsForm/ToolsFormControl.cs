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
using SciImage.PaintForms.UserControls.ColorPickers;
using SciImage.Plugins.Tools;

namespace SciImage.PaintForms.ToolsForm
{
    public class ToolsFormControl
        : UserControl , IToolPicker
    {
        private ToolsControl toolsControl = null;

        /// <summary>
        /// event fired by toolenvironment that is fired when the outside control changes the tool
        /// </summary>
        /// <param name="tool"></param>
        public void Environment_ToolChosen(Type tool)
        {
            toolsControl.SelectTool(tool);
        }

      

        public void SetTools(ToolMenuInfo[] toolInfos)
        {
            toolsControl.SetTools(toolInfos);
        }

        public void SelectTool(Type toolType)
        {
            toolsControl. SelectTool(toolType, true);
        }

        //public ToolsControl ToolsControl
        //{

        //    get
        //    {
        //        return this.toolsControl;
        //    }
        //}

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public event EventHandler OnResize;
        public void ForceResize()
        {
            this.ClientSize = new Size(toolsControl.Width, toolsControl.Height );
            this.Width =toolsControl.Width;
            this.Height = toolsControl.Height;
          
            if (OnResize != null) OnResize(this, EventArgs.Empty);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ClientSize = new Size(toolsControl.Width, toolsControl.Height);
           
        }

        public ToolsFormControl()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
           
            this.Width = toolsControl.Width;
            this.Height = toolsControl.Height ;
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
            this.toolsControl = new SciImage.PaintForms.ToolsForm.ToolsControl();
            this.SuspendLayout();
            // 
            // toolsControl
            // 
            this.toolsControl.BackColor = System.Drawing.SystemColors.Control;
            this.toolsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolsControl.Location = new System.Drawing.Point(0, 0);
            this.toolsControl.Name = "toolsControl";
            this.toolsControl.Size = new System.Drawing.Size(50, 273);
            this.toolsControl.TabIndex = 0;
            this.toolsControl.RelinquishFocus += new System.EventHandler(this.ToolsControl_RelinquishFocus);
            this.toolsControl.OnResize += new System.EventHandler(this.toolsControl_OnResize);
            // 
            // ToolsFormControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.toolsControl);
            this.Name = "ToolsFormControl";
            this.Size = new System.Drawing.Size(50, 273);
            this.ResumeLayout(false);

        }

        void toolsControl_OnResize(object sender, EventArgs e)
        {
            ForceResize();
        }
        #endregion

        private void ToolsControl_RelinquishFocus(object sender, EventArgs e)
        {
            //OnRelinquishFocus();
        }
    }
}
