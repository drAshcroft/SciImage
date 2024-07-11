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
using SciImage.Core;
using SciImage.Core.Surfaces.Layers;
using SciImage.SystemLayer.Base;
using SciImage.SystemLayer.Forms;

namespace SciImage.PaintForms.LayerForm
{
    public class LayerFormControl
        : UserControl, ILayerForm
    {
        public DocumentWorkspace ActiveDocumentWorkspace
        {
            get
            {
                return DocumentManager.Manager.ActiveDocumentWorkspace;
            }
        }

        public  void SetupNewDocument(DocumentWorkspace newDocument )
        {
            if (newDocument!=null)
             layerControl.SetupNewDocument(newDocument.Document);
        }

        public void SuspendLayerPreviewUpdates()
        {
            layerControl.SuspendLayerPreviewUpdates();
        }

        public void ResumeLayerPreviewUpdates()
        {
            layerControl.ResumeLayerPreviewUpdates();
        }

        #region Declarations
        private LayerControl layerControl;
        private System.Windows.Forms.ImageList imageList;
        private ToolStripEx toolStrip;
        private ToolStripButton addNewLayerButton;
        private ToolStripButton deleteLayerButton;
        private ToolStripButton duplicateLayerButton;
        private ToolStripButton mergeLayerDownButton;
        private ToolStripButton moveLayerUpButton;
        private ToolStripButton moveLayerDownButton;
        private ToolStripButton propertiesButton;
        private System.ComponentModel.IContainer components;
        #endregion

        #region Send Messages
        private void OnNewLayerButtonClick()
        {
            if (DocumentManager.Manager.ActiveDocumentWorkspace != null)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.PerformAction("AddNewBlankLayerAction");
            }
        }


        private void OnDeleteLayerButtonClick()
        {
            if (DocumentManager.Manager.ActiveDocumentWorkspace != null && DocumentManager.Manager.ActiveDocumentWorkspace.Document.Layers.Count > 1)
            {

                DocumentManager.Manager.ActiveDocumentWorkspace.PerformAction("DeleteLayerAction");
                //ActiveDocumentWorkspace.ExecuteFunction(new DeleteLayerAction(ActiveDocumentWorkspace.ActiveLayerIndex));
            }
        }


        private void OnDuplicateLayerButtonClick()
        {
            if (DocumentManager.Manager.ActiveDocumentWorkspace != null)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.PerformAction("DuplicateLayerAction");
            }
        }


        private void OnMergeLayerDownButtonClick()
        {
            if (DocumentManager.Manager.ActiveDocumentWorkspace != null)
            {
                if (DocumentManager.Manager.ActiveDocumentWorkspace != null &&
                    DocumentManager.Manager.ActiveDocumentWorkspace.ActiveLayerIndex > 0)
                {
                    // TODO: keep this in sync with LayersMenu. not appropriate to refactor into an Action for a 'dot' release
                    int newLayerIndex = Utility.Clamp(
                        DocumentManager.Manager.ActiveDocumentWorkspace.ActiveLayerIndex - 1,
                        0,
                        DocumentManager.Manager.ActiveDocumentWorkspace.Document.Layers.Count - 1);

                    DocumentManager.Manager.ActiveDocumentWorkspace.PerformAction("MergeLayerDownAction");
                    DocumentManager.Manager.ActiveDocumentWorkspace.ActiveLayerIndex = newLayerIndex;
                }
            }
        }


        private void OnMoveLayerUpButtonClick()
        {
            if (DocumentManager.Manager.ActiveDocumentWorkspace != null && DocumentManager.Manager.ActiveDocumentWorkspace.Document.Layers.Count >= 2)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.PerformAction("MoveActiveLayerUpAction");
            }
        }


        private void OnMoveLayerDownButtonClick()
        {
            if (DocumentManager.Manager.ActiveDocumentWorkspace != null && DocumentManager.Manager.ActiveDocumentWorkspace.Document.Layers.Count >= 2)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.PerformAction("MoveActiveLayerDownAction");
            }
        }


        private void OnPropertiesButtonClick()
        {
            if (DocumentManager.Manager.ActiveDocumentWorkspace != null)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.PerformAction("OpenActiveLayerPropertiesAction");
            }
        }
        #endregion

        #region ClickHandlers
        public void PerformNewLayerClick()
        {
            this.OnNewLayerButtonClick();
        }

        public void PerformDeleteLayerClick()
        {
            this.OnDeleteLayerButtonClick();
        }

        public void PerformDuplicateLayerClick()
        {
            this.OnDuplicateLayerButtonClick();
        }

        public void PerformMoveLayerUpClick()
        {
            this.OnMoveLayerUpButtonClick();
        }

        public void PerformMoveLayerDownClick()
        {
            this.OnMoveLayerDownButtonClick();
        }

        public void PerformPropertiesClick()
        {
            this.OnPropertiesButtonClick();
        }

        private void NewLayerButton_Click(object sender, System.EventArgs e)
        {
            OnNewLayerButtonClick();
        }

        private void DeleteLayerButton_Click(object sender, System.EventArgs e)
        {
            OnDeleteLayerButtonClick();
        }

        private void DuplicateLayerButton_Click(object sender, System.EventArgs e)
        {
            OnDuplicateLayerButtonClick();
        }

        private void MoveUpButton_Click(object sender, System.EventArgs e)
        {
            OnMoveLayerUpButtonClick();
        }

        private void MoveDownButton_Click(object sender, System.EventArgs e)
        {
            OnMoveLayerDownButtonClick();
        }

        private void PropertiesButton_Click(object sender, System.EventArgs e)
        {
            OnPropertiesButtonClick();
        }

        private void LayerControl_ClickOnLayer(object sender, EventArgs<Layer> ce)
        {

            int index = ActiveDocumentWorkspace.Document.Layers.IndexOf(ce.Data);
            try
            {
                if (index == -1)
                {
                    index = ((Layer)ce.Data).LayerIndex;
                }
            }
            catch { }
            DetermineButtonEnableStates(index);
        }

        private void LayerControl_DoubleClickedOnLayer(object sender, EventArgs<Layer> ce)
        {
            OnPropertiesButtonClick();
            //this.OnRelinquishFocus();
        }

        private void LayerControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.None)
            {
                this.OnDeleteLayerButtonClick();
                e.Handled = true;
                return;
            }
        }

        private void OnToolStripButtonClick(object sender, EventArgs e)
        {
            UI.SuspendControlPainting(this.layerControl);

            if (sender == addNewLayerButton)
            {
                OnNewLayerButtonClick();
            }
            else if (sender == deleteLayerButton)
            {
                OnDeleteLayerButtonClick();
            }
            else if (sender == duplicateLayerButton)
            {
                OnDuplicateLayerButtonClick();
            }
            else if (sender == mergeLayerDownButton)
            {
                OnMergeLayerDownButtonClick();
            }
            else if (sender == moveLayerUpButton)
            {
                OnMoveLayerUpButtonClick();
            }
            else if (sender == moveLayerDownButton)
            {
                OnMoveLayerDownButtonClick();
            }

            UI.ResumeControlPainting(this.layerControl);
            this.layerControl.Invalidate(true);

            if (sender == propertiesButton)
            {
                OnPropertiesButtonClick();
            }

            DetermineButtonEnableStates();
            //OnRelinquishFocus();
        }

        #endregion

        #region Constructors
        //public LayerControl LayerControl
        //{
        //    get
        //    {
        //        return layerControl;
        //    }
        //}

        public LayerFormControl()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            imageList.TransparentColor = Utility.TransparentKey;

            toolStrip.ImageList = this.imageList;

            int addNewLayerIndex = imageList.Images.Add(SciResources.SciResources.GetImageResource("Icons.glyphicons-410-addLayer.png").Reference, imageList.TransparentColor);
            int deleteLayerIndex = imageList.Images.Add(SciResources.SciResources.GetImageResource("Icons.glyphicons-410-deletelayer.png").Reference, imageList.TransparentColor);
            int moveLayerUpIndex = imageList.Images.Add(SciResources.SciResources.GetImageResource("Icons.glyphicons-410-MoveUp.png").Reference, imageList.TransparentColor);
            int moveLayerDownIndex = imageList.Images.Add(SciResources.SciResources.GetImageResource("Icons.glyphicons-410-MoveDown.png").Reference, imageList.TransparentColor);
            int duplicateLayerIndex = imageList.Images.Add(SciResources.SciResources.GetImageResource("Icons.glyphicons-483-bring-forward.png").Reference, imageList.TransparentColor);
            int mergeLayerDownIndex = imageList.Images.Add(SciResources.SciResources.GetImageResource("Icons.glyphicons-410-sendToBottom.png").Reference, imageList.TransparentColor);
            int propertiesIndex = imageList.Images.Add(SciResources.SciResources.GetImageResource("Icons.glyphicons-137-cogwheel.png").Reference, imageList.TransparentColor);

            addNewLayerButton.ImageIndex = addNewLayerIndex;
            deleteLayerButton.ImageIndex = deleteLayerIndex;
            moveLayerUpButton.ImageIndex = moveLayerUpIndex;
            moveLayerDownButton.ImageIndex = moveLayerDownIndex;
            duplicateLayerButton.ImageIndex = duplicateLayerIndex;
            mergeLayerDownButton.ImageIndex = mergeLayerDownIndex;
            propertiesButton.ImageIndex = propertiesIndex;

            layerControl.KeyUp += new KeyEventHandler(LayerControl_KeyUp);

            this.Text = "Layer Control";
            this.addNewLayerButton.ToolTipText = "Add Layer";
            this.deleteLayerButton.ToolTipText = "Remove Layer";
            this.duplicateLayerButton.ToolTipText = "Copy Layer";
            this.mergeLayerDownButton.ToolTipText = "Merge Down";
            this.moveLayerUpButton.ToolTipText = "Move Up";
            this.moveLayerDownButton.ToolTipText = "Move Down";
            this.propertiesButton.ToolTipText = "Layer Properties";

            this.MinimumSize = this.Size;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            if (layerControl != null)
            {
                layerControl.Size = new Size(ClientRectangle.Width, ClientRectangle.Height -
                    (this.toolStrip.Height + (ClientRectangle.Height - ClientRectangle.Bottom)));
            }
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

                layerControl.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this.layerControl = new LayerControl();
            this.imageList = new ImageList(this.components);
            this.toolStrip = new ToolStripEx();
            this.addNewLayerButton = new ToolStripButton();
            this.deleteLayerButton = new ToolStripButton();
            this.duplicateLayerButton = new ToolStripButton();
            this.mergeLayerDownButton = new ToolStripButton();
            this.moveLayerUpButton = new ToolStripButton();
            this.moveLayerDownButton = new ToolStripButton();
            this.propertiesButton = new ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // layerControl
            // 
            this.layerControl.Dock = System.Windows.Forms.DockStyle.Fill;

            this.layerControl.Location = new System.Drawing.Point(0, 0);
            this.layerControl.Name = "layerControl";
            this.layerControl.Size = new System.Drawing.Size(160, 158);
            this.layerControl.TabIndex = 5;
           
            this.layerControl.ActiveLayerChanged += this.LayerControl_ClickOnLayer;
            this.layerControl.ClickedOnLayer += this.LayerControl_ClickOnLayer;
            this.layerControl.DoubleClickedOnLayer += this.LayerControl_DoubleClickedOnLayer;
            this.layerControl.RelinquishFocus += new EventHandler(LayerControl_RelinquishFocus);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                                                                                        this.addNewLayerButton,
                                                                                        this.deleteLayerButton,
                                                                                        this.duplicateLayerButton,
                                                                                        this.mergeLayerDownButton,
                                                                                        this.moveLayerUpButton,
                                                                                        this.moveLayerDownButton,
                                                                                        this.propertiesButton
                                                                                   });
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip.Location = new System.Drawing.Point(0, 132);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(160, 26);
            this.toolStrip.TabIndex = 7;
            this.toolStrip.TabStop = true;
            this.toolStrip.RelinquishFocus += new EventHandler(ToolStrip_RelinquishFocus);
            // 
            // addNewLayerButton
            // 
            this.addNewLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addNewLayerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addNewLayerButton.Name = "addNewLayerButton";
            this.addNewLayerButton.Size = new System.Drawing.Size(23, 4);
            this.addNewLayerButton.Click += new System.EventHandler(this.OnToolStripButtonClick);
            // 
            // deleteLayerButton
            // 
            this.deleteLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteLayerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteLayerButton.Name = "deleteLayerButton";
            this.deleteLayerButton.Size = new System.Drawing.Size(23, 4);
            this.deleteLayerButton.Click += new System.EventHandler(this.OnToolStripButtonClick);
            // 
            // duplicateLayerButton
            // 
            this.duplicateLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.duplicateLayerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.duplicateLayerButton.Name = "duplicateLayerButton";
            this.duplicateLayerButton.Size = new System.Drawing.Size(23, 4);
            this.duplicateLayerButton.Click += new System.EventHandler(this.OnToolStripButtonClick);
            //
            // mergeLayerDownButton
            //
            this.mergeLayerDownButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.mergeLayerDownButton.Name = "mergeLayerDownButton";
            this.mergeLayerDownButton.Click += new EventHandler(OnToolStripButtonClick);
            // 
            // moveLayerUpButton
            // 
            this.moveLayerUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveLayerUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveLayerUpButton.Name = "moveLayerUpButton";
            this.moveLayerUpButton.Size = new System.Drawing.Size(23, 4);
            this.moveLayerUpButton.Click += new System.EventHandler(this.OnToolStripButtonClick);
            // 
            // moveLayerDownButton
            // 
            this.moveLayerDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveLayerDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveLayerDownButton.Name = "moveLayerDownButton";
            this.moveLayerDownButton.Size = new System.Drawing.Size(23, 4);
            this.moveLayerDownButton.Click += new System.EventHandler(this.OnToolStripButtonClick);
            // 
            // propertiesButton
            // 
            this.propertiesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.propertiesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.propertiesButton.Name = "propertiesButton";
            this.propertiesButton.Size = new System.Drawing.Size(23, 4);
            this.propertiesButton.Click += new System.EventHandler(this.OnToolStripButtonClick);
            // 
            // LayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(165, 158);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.layerControl);
            this.Name = "LayersForm";
            this.Controls.SetChildIndex(this.layerControl, 0);
            this.Controls.SetChildIndex(this.toolStrip, 0);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        #endregion

        #region Focus
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.Visible)
            {
                foreach (LayerElement le in this.layerControl.Layers)
                {
                    le.RefreshPreview();
                }
            }

            base.OnVisibleChanged(e);
        }

        private void LayerControl_RelinquishFocus(object sender, EventArgs e)
        {
            //OnRelinquishFocus();
        }

        private void ToolStrip_RelinquishFocus(object sender, EventArgs e)
        {
            //OnRelinquishFocus();
        }
        #endregion

        #region ButtonStates
        private void DetermineButtonEnableStates()
        {
            DetermineButtonEnableStates(this.layerControl.ActiveLayerIndex);
        }

        private void DetermineButtonEnableStates(int index)
        {
           

            // Find a reason to disable the Move Layer Down button
            if (ActiveDocumentWorkspace == null ||
                ActiveDocumentWorkspace.Document == null ||
                index == 0)
            {
                this.moveLayerDownButton.Enabled = false;
            }
            else
            {
                this.moveLayerDownButton.Enabled = true;
            }

            // Find a reason to disable the Move Layer Up button
            if (ActiveDocumentWorkspace == null ||
               ActiveDocumentWorkspace.Document == null ||
                index == (ActiveDocumentWorkspace.Document.Layers.Count - 1))
            {
                this.moveLayerUpButton.Enabled = false;
            }
            else
            {
                this.moveLayerUpButton.Enabled = true;
            }

            // Find reasons to disable the Delete Layer button
            if ( ActiveDocumentWorkspace == null ||
                 ActiveDocumentWorkspace.Document == null ||
                 ActiveDocumentWorkspace.Document.Layers.Count <= 1)
            {
                this.deleteLayerButton.Enabled = false;
            }
            else
            {
                this.deleteLayerButton.Enabled = true;
            }

            // Find reasons to disable the Merge Layer Down button
            if ( ActiveDocumentWorkspace == null ||
                 ActiveDocumentWorkspace.Document == null ||
                 ActiveDocumentWorkspace.ActiveLayerIndex == 0 ||
                 ActiveDocumentWorkspace.Document.Layers.Count < 2)
            {
                this.mergeLayerDownButton.Enabled = false;
            }
            else
            {
                this.mergeLayerDownButton.Enabled = true;
            }
        }
        #endregion
    }
}