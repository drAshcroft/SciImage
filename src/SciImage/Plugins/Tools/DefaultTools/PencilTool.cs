/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.BinaryPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UserBlendOps;
using SciImage.Core.Surfaces.Layers;
using SciImage.SciResources;

namespace SciImage.Plugins.Tools.DefaultTools
{
    public class PencilTool
        : Tool
    {
        protected bool mouseDown = false;
        private ColorPixelBase pencilColor;
        private MouseButtons mouseButton;
        private Layer bitmapLayer;
        private RenderArgs renderArgs;
        private List<Point> tracePoints;
        private List<Rectangle> savedRects;
        private SciRegion clipRegion;
        private Point lastPoint;
        private Point difference;
        private Cursor pencilToolCursor;
        private BinaryPixelOp blendOp = new NormalBlendOp();
        private BinaryPixelOp copyOp = new AssignFromRhs();

        protected override void OnActivate()
        {
            base.OnActivate();

            this.pencilToolCursor = new Cursor(SciResources.SciResources.GetResourceStream("Cursors.PencilToolCursor.cur"));
            this.Cursor = this.pencilToolCursor;

            this.savedRects = new List<Rectangle>();

            if (ActiveLayer != null)
            {
                bitmapLayer = (Layer)ActiveLayer;
                renderArgs = new RenderArgs(bitmapLayer.Surface);
                tracePoints = new List<Point>();
            }
            else
            {
                bitmapLayer = null;

                if (renderArgs != null)
                {
                    renderArgs.Dispose();
                    renderArgs = null;
                }
            }
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            if (this.pencilToolCursor != null)
            {
                this.pencilToolCursor.Dispose();
                this.pencilToolCursor = null;
            }

            if (mouseDown)
            {
                Point lastTracePoint = (Point)tracePoints[tracePoints.Count - 1];
                OnMouseUp(new MouseEventArgs(mouseButton, 0, lastTracePoint.X, lastTracePoint.Y, 0));
            }

            this.savedRects = null;
            this.tracePoints = null;
            this.bitmapLayer = null;

            if (this.renderArgs != null)
            {
                this.renderArgs.Dispose();
                this.renderArgs = null;
            }

            this.mouseDown = false;

            if (clipRegion != null)
            {
                clipRegion.Dispose();
                clipRegion = null;
            }
        }

        // Draws a point, but first intersects it with the selection
        protected virtual void DrawPoint(RenderArgs ra, Point p, ColorPixelBase color)
        {
            if (ra.Surface.Bounds.Contains(p))
            {
                if (ra.Graphics.IsVisible(p))
                {
                    BinaryPixelOp op = ToolEnvironment.AlphaBlending ? blendOp : copyOp;
                    ra.Surface[p.X, p.Y] = op.Apply(ra.Surface.GetPoint(p.X, p.Y), ra.Surface.ColorPixelBase.TranslateColor(color)).ToInt32();
                }
            }
        }

        protected virtual void DrawLines(RenderArgs ra, List<Point> points, int startIndex, int length, ColorPixelBase color)
        {
            // Draw a point in the line
            if (points.Count == 0)
            {
                return;
            }
            else if (points.Count == 1)
            {
                Point p = (Point)points[0];

                if (ra.Surface.Bounds.Contains(p))
                {
                    DrawPoint(ra, p, color);
                }
            }
            else
            {
                for (int i = startIndex + 1; i < startIndex + length; ++i)
                {
                    Point[] linePoints = Utility.GetLinePoints(points[i - 1], points[i]);
                    int startPoint = 0;

                    if (i != 1)
                    {
                        startPoint = 1;
                    }

                    for (int pi = startPoint; pi < linePoints.Length; ++pi)
                    {
                        Point p = linePoints[pi];
                        DrawPoint(ra, p, color);
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (mouseDown)
            {
                return;
            }

            if (((e.Button & MouseButtons.Left) == MouseButtons.Left) ||
                ((e.Button & MouseButtons.Right) == MouseButtons.Right))
            {
                mouseDown = true;
                mouseButton = e.Button;
                tracePoints = new List<Point>();
                if (bitmapLayer != ActiveLayer)
                {
                    bitmapLayer = (Layer)ActiveLayer;
                    renderArgs = new RenderArgs(bitmapLayer.Surface);
                }

                if (clipRegion != null)
                {
                    clipRegion.Dispose();
                    clipRegion = null;
                }

                clipRegion = Selection.CreateRegion();
                renderArgs.Graphics.SetClip(clipRegion.GetRegionReadOnly(), CombineMode.Replace);
                OnMouseMove(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (mouseDown && ((e.Button & mouseButton) != MouseButtons.None))
            {
                Point mouseXY = new Point(e.X, e.Y);

                if (lastPoint == Point.Empty)
                {
                    lastPoint = mouseXY;
                }

                difference = new Point(mouseXY.X - lastPoint.X, mouseXY.Y - lastPoint.Y);

                if (tracePoints.Count > 0)
                {
                    Point lastMouseXY = (Point)tracePoints[tracePoints.Count - 1];
                    if (lastMouseXY == mouseXY)
                    {
                        return;
                    }
                }

                if ((mouseButton & MouseButtons.Left) == MouseButtons.Left)
                {
                    this.pencilColor = ToolEnvironment.PrimaryColor;
                }
                else // if ((mouseButton & MouseButtons.Right) == MouseButtons.Right)
                {
                    // right mouse button = swap primary/secondary
                    this.pencilColor = ToolEnvironment.SecondaryColor;
                }

                if (!(tracePoints.Count > 0 && mouseXY == (Point)tracePoints[tracePoints.Count - 1]))
                {
                    tracePoints.Add(mouseXY);
                }

                if (ActiveLayer.Surface != null)
                {
                    Rectangle saveRect;

                    if (tracePoints.Count == 1)
                    {
                        saveRect = Utility.PointsToRectangle(mouseXY, mouseXY);
                    }
                    else
                    {
                        // >1 points
                        saveRect = Utility.PointsToRectangle((Point)tracePoints[tracePoints.Count - 1], (Point)tracePoints[tracePoints.Count - 2]);
                    }

                    saveRect.Inflate(2, 2);
                    saveRect.Intersect(ActiveLayer.Bounds);

                    // drawing outside of the canvas is a no-op, so don't do anything in that case!
                    // also make sure it's within the clipping bounds
                    if (saveRect.Width > 0 && saveRect.Height > 0 && renderArgs.Graphics.IsVisible(saveRect))
                    {
                        SaveRegion(null, saveRect, ActiveLayer.Surface.ColorPixelBase);
                        this.savedRects.Add(saveRect);

                        int startIndex;
                        int length;

                        if (tracePoints.Count == 1)
                        {
                            startIndex = 0;
                            length = 1;
                        }
                        else
                        {
                            startIndex = tracePoints.Count - 2;
                            length = 2;
                        }

                        DrawLines(this.renderArgs, tracePoints, startIndex, length, pencilColor);

                        bitmapLayer.Invalidate(saveRect);
                        Update();
                    }
                }
                else
                {
                    // will have to do something here if we add other blayer types besides BitmapLayer
                }

                lastPoint = mouseXY;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (mouseDown)
            {
                OnMouseMove(e);
                mouseDown = false;

                if (savedRects.Count > 0)
                {
                    Rectangle[] savedScans = this.savedRects.ToArray();
                    SciRegion saveMeRegion = Utility.RectanglesToRegion(savedScans);

                    HistoryMemento ha = new BitmapHistoryMemento(Name, Image, DocumentWorkspace,
                        ActiveLayerIndex, saveMeRegion, ScratchSurface);

                    HistoryStack.PushNewMemento(ha);
                    saveMeRegion.Dispose();
                    this.savedRects.Clear();
                    ClearSavedMemory();
                }

                tracePoints = null;
            }
        }

        public PencilTool(DocumentWorkspace documentWorkspace,
                   ImageResource toolBarImage,
                   string name,
                   string helpText,
                   char hotKey,
                   int order,
                   bool skipIfActiveOnHotKey,
                   ToolBarConfigItems toolBarConfigItems):base (documentWorkspace,toolBarImage,name,helpText,hotKey,order,skipIfActiveOnHotKey, toolBarConfigItems)
        {
            mouseDown = false;
        }

        public PencilTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciResources.SciResources.GetImageResource("Icons.glyphicons-31-pencil.png"),
                   "Simple drawing tool",
                   "Left for Primary color, right click for secondary",
                   'p', 3,
                   true,
                   ToolBarConfigItems.AlphaBlending)
        {
            // initialize any state information you need
            mouseDown = false;
        }
    }
}
