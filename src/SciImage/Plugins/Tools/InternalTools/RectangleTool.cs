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
    public class RectangleTool
        : ShapeTool 
    {
        private ImageResource rectangleToolIcon;
        private string statusTextFormat = "Coordinates {0}{1} x {2}{3}, Covers: {4} {5} pixels";
        private Cursor rectangleToolCursor;

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
            if (ToolEnvironment.PenInfo.Width == 1.0f)
            {
                return PixelOffsetMode.None;
            }

            return base.GetPixelOffsetMode();
        }

        protected override PdnGraphicsPath CreateShapePath(PointF[] points)
        {
            PointF a = points[0];
            PointF b = points[points.Length - 1];
            RectangleF rect;

            if ((ModifierKeys & Keys.Shift) != 0)
            {
                rect = Utility.PointsToConstrainedRectangle(a, b);
            }
            else
            {
                rect = Utility.PointsToRectangle(a, b);
            }

            PdnGraphicsPath path = new PdnGraphicsPath();
            path.AddRectangle(rect);
            path.CloseFigure();
            path.Reverse();

            MeasurementUnit units = AppEnvironment.Environment.Units;
            double widthPhysical = Math.Abs(Document.PixelToPhysicalX(rect.Width, units));
            double heightPhysical = Math.Abs(Document.PixelToPhysicalY(rect.Height, units));
            double areaPhysical = widthPhysical * heightPhysical;
            
            string numberFormat;
            string unitsAbbreviation;

            if (units != MeasurementUnit.Pixel)
            {
                string unitsAbbreviationName = MeasurementUnitDetails.Abbreviation( units);
                unitsAbbreviation = SciResources.SciResources.GetString(unitsAbbreviationName);
                numberFormat = "F2";
            }
            else
            {
                unitsAbbreviation = string.Empty;
                numberFormat = "F0";
            }

            string unitsString = MeasurementUnitDetails.Plural(units);

            string statusText = string.Format(
                this.statusTextFormat,
                widthPhysical.ToString(numberFormat),
                unitsAbbreviation,
                heightPhysical.ToString(numberFormat),
                unitsAbbreviation,
                areaPhysical.ToString(numberFormat),
                unitsString);

            this.SetStatus(this.rectangleToolIcon, statusText);
            return path;
        }

        protected override void OnActivate()
        {
            rectangleToolCursor = new Cursor(SciResources.SciResources.GetResourceStream("Cursors.RectangleToolCursor.cur"));
            this.rectangleToolIcon = this.Image;
            this.Cursor = rectangleToolCursor;
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            if (this.rectangleToolCursor != null)
            {
                this.rectangleToolCursor.Dispose();
                this.rectangleToolCursor = null;
            }

            base.OnDeactivate();
        }

        public RectangleTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciResources.SciResources.GetImageResource("Icons.RectangleToolIcon.png"),
                   "Rectangle Drawing",
                   "Hold shift to force the drawing to remain square.")
        {
        }
    }
}
