/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////


using System.Drawing;
using System.Windows.Forms;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.Layers;
using SciImage.Plugins.Tools.FloodTools;

namespace SciImage.Plugins.Tools.InternalTools
{
    public class PaintBucketTool
        : FloodToolBase
    {
        private Cursor cursorMouseUp;
        private Brush brush;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            brush = ToolEnvironment.CreateBrush((e.Button != MouseButtons.Left));
            Cursor = Cursors.WaitCursor;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Cursor = cursorMouseUp;
            base.OnMouseUp (e);
        }

        protected override void OnFillRegionComputed(Point[][] polygonSet)
        {
            using (PdnGraphicsPath path = new PdnGraphicsPath())
            {
                path.AddPolygons(polygonSet);

                using (SciRegion fillRegion = new SciRegion(path))
                {
                    Rectangle boundingBox = fillRegion.GetBoundsInt();

                    Surface surface = ((Layer)ActiveLayer).Surface;
                    RenderArgs ra = new RenderArgs(surface);
                    HistoryMemento ha;

                    using (SciRegion affected = Utility.SimplifyAndInflateRegion(fillRegion))
                    {
                        ha = new BitmapHistoryMemento(Name, Image, DocumentWorkspace, DocumentWorkspace.ActiveLayerIndex, affected);
                    }

                    ra.Graphics.CompositingMode = ToolEnvironment.GetCompositingMode();
                    ra.Graphics.FillRegion(brush, fillRegion.GetRegionReadOnly());

                    HistoryStack.PushNewMemento(ha);
                    ActiveLayer.Invalidate(boundingBox);
                    Update();
                }
            }
        }

        protected override void OnActivate()
        {
            // cursor-transitions
            cursorMouseUp = new Cursor(SciResources.SciResources.GetResourceStream("Cursors.PaintBucketToolCursor.cur"));
            Cursor = cursorMouseUp;

            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            if (cursorMouseUp != null)
            {
                cursorMouseUp.Dispose();
                cursorMouseUp = null;
            }

            base.OnDeactivate();
        }


        public PaintBucketTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciResources.SciResources.GetImageResource("Icons.PaintBucketIcon.png"),
                   "Fill Tool",
                   "Left mouse button fills the area with primary color, right with secondary color",
                   'f',
                   false,
                   ToolBarConfigItems.Brush | ToolBarConfigItems.Antialiasing | ToolBarConfigItems.AlphaBlending)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose (disposing);

            if (disposing)
            {
                if (brush != null)
                {
                    brush.Dispose();
                    brush = null;
                }
            }
        }
    }
}
