/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SciImage.Core;
using SciImage.Core.Selection;
using SciImage.SciResources;

namespace SciImage.Plugins.Tools.InternalTools
{
    public class EllipseTool
        : ShapeTool 
    {
        private ImageResource ellipseToolIcon;
        private string statusTextFormat = "Bounding rectangle size: {0}{1} x {2}{3}, covered area: {4} {5} in pixels";
        private Cursor ellipseToolCursor;

        protected override List<PointF> TrimShapePath(List<PointF> points)
        {
            List<PointF> array = new List<PointF>();

            if (points.Count > 0)
            {
                array.Add(points[0]);

                if (points.Count > 1)
                {
                    array.Add(points[points.Count - 1]);
                }
            }

            return array;
        }

        public override PixelOffsetMode GetPixelOffsetMode()
        {
            return PixelOffsetMode.None;
        }

        protected override RectangleF[] GetOptimizedShapeOutlineRegion(PointF[] points, PdnGraphicsPath path)
        {
            return Utility.SimplifyTrace(path.PathPoints);
        }

        protected override PdnGraphicsPath CreateShapePath(PointF[] points)
        {
            PointF a = points[0];
            PointF b = points[points.Length - 1];
            RectangleF rect;

            if ((ModifierKeys & Keys.Shift) != 0)
            {
                PointF dir = new PointF(b.X - a.X, b.Y - a.Y);
                float len = (float)Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y);
                PointF center = new PointF((a.X + b.X) / 2.0f, (a.Y + b.Y) / 2.0f);
                float radius = len / 2.0f;
                rect = Utility.RectangleFromCenter(center, radius);
            }
            else
            {
                rect = Utility.PointsToRectangle(a, b);
            }

            if (rect.Width == 0 || rect.Height == 0)
            {
                return null;
            }

            PdnGraphicsPath path = new PdnGraphicsPath();
            path.AddEllipse(rect);
            path.Flatten(Utility.IdentityMatrix, 0.10f);

            MeasurementUnit units = AppEnvironment.Environment .Units;

            double widthPhysical = Math.Abs(Document.PixelToPhysicalX(rect.Width, units));
            double heightPhysical = Math.Abs(Document.PixelToPhysicalY(rect.Height, units));
            double areaPhysical = Math.PI * (widthPhysical / 2.0) * (heightPhysical / 2.0);
            
            string numberFormat;
            string unitsAbbreviation;

            if (units != MeasurementUnit.Pixel)
            {
                string unitsAbbreviationName = units.ToString();// //MeasurementUnitDetails.Abbreviation( units);
                unitsAbbreviation =unitsAbbreviationName;
                numberFormat = "F2";
            }
            else
            {
                unitsAbbreviation = string.Empty;
                numberFormat = "F0";
            }

            string unitsString = units.ToString() + "s";

            string statusText = string.Format(
                this.statusTextFormat,
                widthPhysical.ToString(numberFormat),
                unitsAbbreviation,
                heightPhysical.ToString(numberFormat),
                unitsAbbreviation,
                areaPhysical.ToString(numberFormat),
                unitsString);

            this.SetStatus(this.ellipseToolIcon, statusText);
            return path;
        }

        protected override void OnActivate()
        {
            this.ellipseToolCursor = new Cursor(SciResources.SciResources.GetResourceStream("Cursors.EllipseToolCursor.cur"));
            this.ellipseToolIcon = this.Image;
            this.Cursor = this.ellipseToolCursor;
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            if (this.ellipseToolCursor != null)
            {
                this.ellipseToolCursor.Dispose();
                this.ellipseToolCursor = null;
            }

            base.OnDeactivate();
        }

        public EllipseTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciResources.SciResources.GetImageResource("Icons.EllipseToolIcon.png"),
                   "Ellipse and Circle",
                   "Use shift to force the drawing to be a circle")
        {
        }
    }
}

