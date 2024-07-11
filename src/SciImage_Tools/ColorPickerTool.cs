/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Windows.Forms;
using SciImage;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.Layers;
using SciImage.PaintForms.UserControls.ColorPickers;
using SciImage.Plugins.Tools;
using SciImage.Plugins.Tools.DefaultTools;
using SciImage.Plugins.Tools.Enums;

namespace SciImage_Tools
{
    public class ColorPickerTool : Tool
    {
        private bool mouseDown;
        private Cursor colorPickerToolCursor;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (mouseDown)
            {
                PickColor(e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDown = true;
        
            PickColor(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseDown = false;

            switch (ToolEnvironment.ColorPickerClickBehavior)
            {
                case ColorPickerClickBehavior.NoToolSwitch:
                    break;

                case ColorPickerClickBehavior.SwitchToLastTool:
                    DocumentWorkspace.SetToolFromType(DocumentWorkspace.PreviousActiveToolType);
                    break;

                case ColorPickerClickBehavior.SwitchToPencilTool:
                    DocumentWorkspace.SetToolFromType(typeof(PencilTool));
                    break;

                default:
                    throw new System.ComponentModel.InvalidEnumArgumentException();
            }
        }

        private ColorPixelBase LiftColor(int x, int y)
        {
            ColorPixelBase newColor;
            newColor = ((Layer)ActiveLayer).Surface.GetPoint(x, y);
            return newColor;
        }

        private void PickColor(MouseEventArgs e)
        {
            if (!Document.Bounds.Contains(e.X, e.Y))
            {
                return;
            }

            ColorPixelBase color;
            color = LiftColor(e.X, e.Y);

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this.ToolEnvironment.PrimaryColor = color;
            }
            else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {   
                this.ToolEnvironment.SecondaryColor = color;
            }
        }

        protected override void OnActivate()
        {
            this.colorPickerToolCursor = new Cursor(SciImage.SciResources.SciResources.GetResourceStream("Cursors.ColorPickerToolCursor.cur"));
            this.Cursor = this.colorPickerToolCursor;
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            if (this.colorPickerToolCursor != null)
            {
                this.colorPickerToolCursor.Dispose();
                this.colorPickerToolCursor = null;
            }

            base.OnDeactivate();
        }

        public ColorPickerTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciImage.SciResources.SciResources.GetImageResource("Icons.glyphicons-91-eyedropper.png"),
                   "Color sampler",
                   "Left for copied primary color, right click for copying the secondary color",
                   'k',5,
                   true,
                   ToolBarConfigItems.ColorPickerBehavior)
        {
            // initialize any state information you need
            mouseDown = false;
        }
    }
}