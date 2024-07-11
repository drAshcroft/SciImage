/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Menus;
using SciImage.Menus.Strips;

using SciImage.PaintForms.ColorPickers;
using SciImage.PaintForms.HistoryForm;
using SciImage.PaintForms.LayerForm;
using SciImage.PaintForms.ToolsForm;
using SciImage.SystemLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using SciImage.Plugins.Actions;
using SciImage.SystemLayer.Base;
using SciImage.SystemLayer.Forms;

namespace SciImage
{
    #region Delegates_and_Events
    public delegate void MoveLeftEvent(object sender);
    public delegate void MoveRightEvent(object sender);
    public delegate void MoveUpEvent(object sender);
    public delegate void MoveDownEvent(object sender);
    public delegate void MoveArbitraryEvent(object sender, long x, long y);
    public delegate void PauseMovieUpdatesEvent(object sender);
    public delegate void MakeNewMDISubFormEvent(object sender, Form NewForm);
    public delegate Form MakeFormFromUserControl(UserControl Control);
    public delegate bool CmdKeysEventHandler(object sender, ref Message msg, Keys keyData);
    #endregion
    public class AppWorkspace
        : UserControl
    {
       

        public event MoveLeftEvent MoveLeft;
        public event MoveRightEvent MoveRight;
        public event MoveDownEvent MoveDown;
        public event MoveUpEvent MoveUp;
       

        private Panel workspacePanel;
        private PictureBox MoveUpButton;
        private PictureBox MoveDownButton;
        private PictureBox MoveRightButton;
        private PictureBox MoveLeftButton;

        protected override void OnLoad(EventArgs e)
        {
            if (DocumentManager.Manager.ActiveDocumentWorkspace != null)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.Select();
            }

            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (ParentForm != null && DocumentManager.Manager.ActiveDocumentWorkspace != null)
            {
                if (ParentForm.WindowState == FormWindowState.Minimized)
                {
                    DocumentManager.Manager.ActiveDocumentWorkspace.EnableToolPulse = false;
                }
                else
                {
                    DocumentManager.Manager.ActiveDocumentWorkspace.EnableToolPulse = true;
                }
            }
            try
            {
                int ll = 0;
                this.MoveLeftButton.Left = ll;
                this.MoveLeftButton.Top = (this.Height + this.MoveRightButton.Height) / 2;

                this.MoveRightButton.Left = this.Width - this.MoveRightButton.Width;
                this.MoveRightButton.Top = (this.Height  + this.MoveRightButton.Height) / 2;

                this.MoveDownButton.Left = (this.Width - ll - MoveDownButton.Width) / 2 + ll;
                this.MoveDownButton.Top = this.Height - this.MoveDownButton.Height;

                this.workspacePanel.Top =this.MoveUpButton.Height;
                this.workspacePanel.Left = this.MoveRightButton.Width + ll;
                this.workspacePanel.Height = this.Height - ( this.MoveUpButton.Height + this.MoveDownButton.Height);
                this.workspacePanel.Width = this.Width - (this.MoveRightButton.Width + this.MoveLeftButton.Width);
                this.workspacePanel.Visible = false;

                this.MoveUpButton.Top = 0;
                this.MoveUpButton.Left = (this.Width - ll - MoveUpButton.Width) / 2 + ll;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }

        }

     
      
     

        private C_API _UserInterface = null;

        public  C_API UserInterface
        {
            get
            {
                return _UserInterface;
            }
        }

        public AppWorkspace()
        {
           FormsManager. BaseForm = this;
            SuspendLayout();

            InitializeComponent();

            ResumeLayout();
            PerformLayout();
            _UserInterface = new C_API(this);
        }

        protected override void Dispose(bool disposing)
        {

            UserInterface.BlockMovieUpdates = true;
            if (disposing)
            {
               ActionFactory. PerformAction( "CloseAllWorkspacesAction");

                if (DocumentManager.Manager.ActiveDocumentWorkspace != null)
                {
                    DocumentManager.Manager.ActiveDocumentWorkspace.SetTool(null);
                }
            }
            foreach (Control c in Controls)
            {
                c.Dispose();
            }
            //try
            //{
            //    mainToolBarForm.Dispose();
            //}
            //catch { }
            //try
            //{
            //    layerForm.Dispose();
            //}
            //catch { }
            //try { historyForm.Dispose(); }
            //catch { }
            //try { colorsForm.Dispose(); }
            //catch { }
            base.Dispose(disposing);

        }

        protected void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppWorkspace));
            this.workspacePanel = new System.Windows.Forms.Panel();
            this.MoveUpButton = new System.Windows.Forms.PictureBox();
            this.MoveDownButton = new System.Windows.Forms.PictureBox();
            this.MoveRightButton = new System.Windows.Forms.PictureBox();
            this.MoveLeftButton = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MoveUpButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveDownButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveRightButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveLeftButton)).BeginInit();
            this.SuspendLayout();
            // 
            // workspacePanel
            // 
            this.workspacePanel.Location = new System.Drawing.Point(156, 65);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Size = new System.Drawing.Size(457, 307);
            this.workspacePanel.TabIndex = 0;
            // 
            // MoveUpButton
            // 
            this.MoveUpButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveUpButton.Image")));
            this.MoveUpButton.Location = new System.Drawing.Point(396, 0);
            this.MoveUpButton.Name = "MoveUpButton";
            this.MoveUpButton.Size = new System.Drawing.Size(45, 50);
            this.MoveUpButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MoveUpButton.TabIndex = 7;
            this.MoveUpButton.TabStop = false;
            this.MoveUpButton.Visible = false;
            this.MoveUpButton.Click += new System.EventHandler(this.MoveUpButton_Click);
            // 
            // MoveDownButton
            // 
            this.MoveDownButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveDownButton.Image")));
            this.MoveDownButton.Location = new System.Drawing.Point(384, 587);
            this.MoveDownButton.Name = "MoveDownButton";
            this.MoveDownButton.Size = new System.Drawing.Size(45, 50);
            this.MoveDownButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MoveDownButton.TabIndex = 8;
            this.MoveDownButton.TabStop = false;
            this.MoveDownButton.Visible = false;
            this.MoveDownButton.Click += new System.EventHandler(this.MoveDownButton_Click);
            // 
            // MoveRightButton
            // 
            this.MoveRightButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveRightButton.Image")));
            this.MoveRightButton.Location = new System.Drawing.Point(824, 237);
            this.MoveRightButton.Name = "MoveRightButton";
            this.MoveRightButton.Size = new System.Drawing.Size(45, 50);
            this.MoveRightButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MoveRightButton.TabIndex = 9;
            this.MoveRightButton.TabStop = false;
            this.MoveRightButton.Visible = false;
            this.MoveRightButton.Click += new System.EventHandler(this.MoveRightButton_Click);
            // 
            // MoveLeftButton
            // 
            this.MoveLeftButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveLeftButton.Image")));
            this.MoveLeftButton.Location = new System.Drawing.Point(3, 252);
            this.MoveLeftButton.Name = "MoveLeftButton";
            this.MoveLeftButton.Size = new System.Drawing.Size(45, 50);
            this.MoveLeftButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MoveLeftButton.TabIndex = 10;
            this.MoveLeftButton.TabStop = false;
            this.MoveLeftButton.Visible = false;
            this.MoveLeftButton.Click += new System.EventHandler(this.MoveLeftButton_Click);
            // 
            // AppWorkspace
            // 
            this.Controls.Add(this.MoveRightButton);
            this.Controls.Add(this.MoveLeftButton);
            this.Controls.Add(this.workspacePanel);
            this.Controls.Add(this.MoveDownButton);
            this.Controls.Add(this.MoveUpButton);
            this.Name = "AppWorkspace";
            this.Size = new System.Drawing.Size(872, 640);
            ((System.ComponentModel.ISupportInitialize)(this.MoveUpButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveDownButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveRightButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveLeftButton)).EndInit();
            this.ResumeLayout(false);

        }

        #region MoveButtons //This was an idea from when this was a microscopy frontend
        private void MoveUpButon_Click(object sender, EventArgs e)
        {
            if (MoveUp != null) MoveUp(this);
        }

        private void MoveLeftButton_Click(object sender, EventArgs e)
        {
            if (MoveLeft != null) MoveLeft(this);
        }

        private void MoveRightButton_Click(object sender, EventArgs e)
        {
            if (MoveRight != null) MoveRight(this);
        }

        private void MoveDownButton_Click(object sender, EventArgs e)
        {
            if (MoveDown != null) MoveDown(this);
        }

        void MoveUpButton_Click(object sender, EventArgs e)
        {
            if (MoveUp != null) MoveUp(this);
        }

        void MoveButtonLeft_Click(object sender, EventArgs e)
        {
            if (MoveLeft != null) MoveLeft(this);

        }
        void MoveButtonRight_Click(object sender, EventArgs e)
        {
            if (MoveRight != null) MoveRight(this);
        }
        void MoveButtonUp_Click(object sender, EventArgs e)
        {
        }
        void MoveButtonDown_Click(object sender, EventArgs e)
        {
            if (MoveDown != null) MoveDown(this);
        }
        #endregion
    }
}
