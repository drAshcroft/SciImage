/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.Core;
using SciImage.SystemLayer;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SciImage.Plugins.Tools;
using SciImage.Plugins.Tools.ToolConfigs;
using SciImage.SystemLayer.Forms;
using System.Collections.Generic;

namespace SciImage.PaintForms.ToolsForm
{
    public class ToolsControl 
        : UserControl, 
          IToolChooser
    {
        private ToolStripEx toolStripEx;
        private ImageList imageList;
        private const int tbWidth = 2; // two buttons per line in the toolbars
        private int ignoreToolClicked = 0;
        private Control onePxSpacingLeft;
        public void ForceVisible()
        {
            this.Visible = true;
            toolStripEx.Visible = true;
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ToolsControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

       
        protected virtual void OnToolClicked(Type toolType)
        {
            if (this.ignoreToolClicked <= 0)
            {
                ToolFactory.Factory.OnToolClick(this, toolType);
            }
        }

        public void SetTools(ToolMenuInfo[] toolInfos)
        {
            if (this.toolStripEx != null)
            {
                this.toolStripEx.Items.Clear();
            }

            this.imageList = new ImageList();
            this.imageList.ColorDepth = ColorDepth.Depth32Bit;
            this.imageList.TransparentColor = Utility.TransparentKey;

            this.toolStripEx.ImageList = this.imageList;

            List<ToolStripItem> buttons = new List<ToolStripItem>();
            string toolTipFormat = "";// PdnResources.GetString("ToolsControl.ToolToolTip.Format");

            for (int i = 0; i < toolInfos.Length; ++i)
            {
                try
                {
                    ToolMenuInfo toolInfo = toolInfos[i];
                    ToolStripButton button = new ToolStripButton();
                    if (toolInfo.Image != null)
                    {
                        int imageIndex = imageList.Images.Add(
                            toolInfo.Image.Reference,
                            imageList.TransparentColor);

                        button.ImageIndex = imageIndex;
                    }
                    button.Tag = toolInfo.ToolType;
                    button.ToolTipText = string.Format(toolTipFormat, toolInfo.Name, char.ToUpperInvariant(toolInfo.HotKey).ToString());
                    buttons.Add( button);
                }
                catch { }
            }

            this.toolStripEx.Items.AddRange(buttons.ToArray());
        }

        public void SelectTool(Type toolType)
        {
            SelectTool(toolType, true);
        }

        public void SelectTool(Type toolType, bool raiseEvent)
        {
            if (!raiseEvent)
            {
                ++this.ignoreToolClicked;
            }

            try
            {
                foreach (ToolStripButton button in this.toolStripEx.Items)
                {
                    if ((Type)button.Tag == toolType)
                    {
                        this.ToolStripEx_ItemClicked(this, new ToolStripItemClickedEventArgs(button));
                        return;
                    }
                }

                //throw new ArgumentException("Tool type not found");
            }

            finally
            {
                if (!raiseEvent)
                {
                    --this.ignoreToolClicked;
                }
            }
        }

        #region Constructor
        protected override void OnLayout(LayoutEventArgs e)
        {
            int buttonWidth;

            if (this.toolStripEx.Items.Count > 0)
            {
                buttonWidth = this.toolStripEx.Items[0].Width;
            }
            else
            {
                buttonWidth = 0;
            }

            this.toolStripEx.Width =
                this.toolStripEx.Padding.Left +
                (buttonWidth * tbWidth) +
                (this.toolStripEx.Margin.Horizontal * tbWidth) +
                this.toolStripEx.Padding.Right;

            this.toolStripEx.Height = this.toolStripEx.GetPreferredSize(this.toolStripEx.Size).Height;

            if (buttonWidth != 0)
            {
                this.Width = this.toolStripEx.Width + this.onePxSpacingLeft.Width;
                this.Height = this.toolStripEx.Height;
            }

            base.OnLayout(e);
            if (OnResize != null) OnResize(this, EventArgs.Empty);
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

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripEx = new ToolStripEx();
            this.onePxSpacingLeft = new System.Windows.Forms.Control();
            this.SuspendLayout();
            // 
            // toolStripEx
            // 
            this.toolStripEx.ClickThrough = true;
            this.toolStripEx.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripEx.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStripEx.Location = new System.Drawing.Point(1, 0);
            this.toolStripEx.ManagedFocus = true;
            this.toolStripEx.Name = "toolStripEx";
            this.toolStripEx.Size = new System.Drawing.Size(47, 47);
            this.toolStripEx.TabIndex = 0;
            this.toolStripEx.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolStripEx_ItemClicked);
            this.toolStripEx.RelinquishFocus += new System.EventHandler(this.ToolStripEx_RelinquishFocus);
            // 
            // onePxSpacingLeft
            // 
            this.onePxSpacingLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.onePxSpacingLeft.Location = new System.Drawing.Point(0, 0);
            this.onePxSpacingLeft.Name = "onePxSpacingLeft";
            this.onePxSpacingLeft.Size = new System.Drawing.Size(1, 328);
            this.onePxSpacingLeft.TabIndex = 1;
            // 
            // ToolsControl
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.toolStripEx);
            this.Controls.Add(this.onePxSpacingLeft);
            this.Name = "ToolsControl";
            this.Size = new System.Drawing.Size(48, 328);
            this.Load += new System.EventHandler(this.ToolsControl_Load);
            this.Resize += new System.EventHandler(this.ToolsControl_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Events
        public event EventHandler RelinquishFocus;
        private void OnRelinquishFocus()
        {
            if (RelinquishFocus != null)
            {
                RelinquishFocus(this, EventArgs.Empty);
            }
        }

        private void ToolStripEx_RelinquishFocus(object sender, EventArgs e)
        {
            OnRelinquishFocus();
        }

        private void ToolStripEx_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripButton button in this.toolStripEx.Items)
            {
                button.Checked = (button == e.ClickedItem);
            }

            OnToolClicked((Type)e.ClickedItem.Tag);
        }

        private void ToolsControl_Load(object sender, EventArgs e)
        {
            this.Visible = true;
            toolStripEx.Visible = true;
        }

        public new event EventHandler OnResize;
        private void ToolsControl_Resize(object sender, EventArgs e)
        {
            if (OnResize != null) OnResize(this, EventArgs.Empty);
        }
        #endregion
    }
}

