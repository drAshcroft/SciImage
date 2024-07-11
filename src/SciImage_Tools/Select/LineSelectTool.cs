﻿using System.Collections.Generic;
using System.Drawing;
using SciImage;
using SciImage.Core.Selection;
using SciImage.Plugins.Tools;

namespace SciImage_Tools.Select
{
    public  class LineSelectTool : SelectionTool
    {
       
        
        protected override List<Point> TrimShapePath(List<Point> tracePoints)
        {
            List<Point> array = new List<Point>();

            if (tracePoints.Count > 0)
            {
                array.Add(tracePoints[0]);

                if (tracePoints.Count > 1)
                {
                    array.Add(tracePoints[tracePoints.Count - 1]);
                }
            }

            return array;
        }

        protected override List<PointF> CreateShape(List<Point> tracePoints)
        {
            Point a = tracePoints[0];
            Point b = tracePoints[tracePoints.Count - 1];
           
            PdnGraphicsPath path = new PdnGraphicsPath();

            path.AddLine(a, b);
            
            PointF[] pointsF = path.PathPoints;
            path.Dispose();

            return new List<PointF>(pointsF);
        }

        protected override void OnActivate()
        {
            SetCursors(
                "Cursors.EllipseSelectToolCursor.cur",
                "Cursors.EllipseSelectToolCursorMinus.cur",
                "Cursors.EllipseSelectToolCursorPlus.cur",
                "Cursors.EllipseSelectToolCursorMouseDown.cur");

            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }

        public LineSelectTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciImage.SciResources.SciResources.GetImageResource("Icons.glyphicons-98-lineselect.png"),
                   "Line selection tool",
                   "",
                   'l',
                   ToolBarConfigItems.None)
        {
        }

    }
}