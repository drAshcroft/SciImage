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
using SciImage;
using SciImage.Core;
using SciImage.Core.Selection;

namespace SciImage_Tools.ShapeTools
{
    public class FreeformShapeTool
        : ShapeTool 
    {
        private Cursor freeformShapeToolCursor;

        protected override RectangleF[] GetOptimizedShapeOutlineRegion(PointF[] points, PdnGraphicsPath path)
        {
            return Utility.SimplifyTrace(path.PathPoints);
        }

        protected override PdnGraphicsPath CreateShapePath(PointF[] points)
        {
            // make sure we don't screw them up
            if (points.Length < 2)
            {
                return null;
            }

            // make sure the shape has an area of at least 1
            // we can determine this by making sure that all the Points in points are not all the same
            bool allTheSame = true;
            foreach (PointF pt in points)
            {
                if (pt != points[0])
                {
                    allTheSame = false;
                    break;
                }
            }

            if (allTheSame)
            {
                return null;
            }

            PdnGraphicsPath path = new PdnGraphicsPath();
            path.AddLines(points);
            path.AddLine(points[points.Length - 1], points[0]);
            path.CloseAllFigures();
            return path;
        }

        protected override void OnActivate()
        {
            this.freeformShapeToolCursor = new Cursor(SciImage.SciResources.SciResources.GetResourceStream("Cursors.FreeformShapeToolCursor.cur"));
            this.Cursor = this.freeformShapeToolCursor;
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            if (this.freeformShapeToolCursor != null)
            {
                this.freeformShapeToolCursor.Dispose();
                this.freeformShapeToolCursor = null;
            }

            base.OnDeactivate();
        }

        public FreeformShapeTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciImage.SciResources.SciResources.GetImageResource("Icons.glyphicons-99-vector-path-curve.png"),
                   "Freeform Polygon shape",
                   "Click to make lines, adjust with the control points")
        {
        }
    }
}
