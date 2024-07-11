using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces;

namespace SciImage.Plugins.Tools.InternalTools
{
    public class LineProfileTool : SelectionTool
    {
        private Graph pg;
        protected override List<System.Drawing.PointF> CreateShape(List<System.Drawing.Point> tracePoints)
        {
          
            
            Point a = tracePoints[0];
            Point b = tracePoints[tracePoints.Count - 1];

            PdnGraphicsPath path = new PdnGraphicsPath();

            path.AddLine(a, b);

            PointF[] pointsF = path.PathPoints;
            path.Dispose();

            return new List<PointF>(pointsF);
            
        }
        Point P0;
        Point P1;
        bool MouseActive = false;
        System.Timers.Timer ProfileTimer = new System.Timers.Timer();
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseActive = true;
            P0 = new Point(e.X, e.Y);
            P1 = new Point(e.X + 1, e.Y);
            DoProfile(P0,P1);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseActive = false;
            P1 = new Point(e.X, e.Y);
            DoProfile(P0, P1);
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {

            if (MouseActive)
            {
                P1 = new Point(e.X, e.Y);
              //  DoProfile(P0,P1);
                ProfileTimer.Enabled = true;
            
                
            }
           
            base.OnMouseMove(e);
        }

        private bool DoingProfile = false;
        private void DoProfile(Point StartLine,Point EndLine)
        {
            if (DoingProfile == false)
            {
                DoingProfile = true;
                if (pg == null)
                {
                    pg = (Graph)AppWorkspace.BaseForm.ShowForm("Profile", new Graph());
                    pg.FormClosed += new FormClosedEventHandler(pg_FormClosed);
                }

                Surface dst = new Surface(this.DocumentWorkspace.ActiveLayer.Width, this.DocumentWorkspace.ActiveLayer.Height, this.DocumentWorkspace.ActiveLayer.Surface.ColorPixelBase );

                dst.Clear();

                using (RenderArgs renderArgs = new RenderArgs(dst))
                {
                    this.Document.Render(renderArgs, true);
                }

                Surface s = dst;// ((BitmapLayer)DocumentWorkspace.ActiveLayer).Surface;
                int x0 = StartLine.X;
                int x1 = EndLine.X;
                int y0 = StartLine.Y;
                int y1 = EndLine.Y;

                double Ux = x1 - x0;
                double Uy = y1 - y0;
                double d = Math.Sqrt(Ux * Ux + Uy * Uy);
                int[,] LineDots = new int[2, (int)d + 1];

                
                    for (int i = 0; i < d; i++)
                    {
                        int x = (int)(Ux * i / d + x0);
                        int y = (int)(Uy * i / d + y0);
                        LineDots[0, i] = i;
                        if (x > 0 && y > 0)
                        {
                            LineDots[1, i] = (int)(s.GetPoint(x, y).GetIntensity() * 1000);
                        }
                        else
                            LineDots[1, i] = 0;

                    }
                


                if (pg!=null) pg.SetData(LineDots, "Profile");
                System.Windows.Forms.Application.DoEvents();
            }
            DoingProfile = false;
        }
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

        

        protected override void OnActivate()
        {
            SetCursors(
                "Cursors.EllipseSelectToolCursor.cur",
                "Cursors.EllipseSelectToolCursorMinus.cur",
                "Cursors.EllipseSelectToolCursorPlus.cur",
                "Cursors.EllipseSelectToolCursorMouseDown.cur");

          //  pg = (PaintDNetWindow.Graph)this.DocumentWorkspace.AppWorkspace.ShowForm("Profile", new PaintDNetWindow.Graph());
          //  pg.FormClosed += new FormClosedEventHandler(pg_FormClosed);
            base.OnActivate();
        }

        void pg_FormClosed(object sender, FormClosedEventArgs e)
        {
            pg = null;
            ProfileTimer.Enabled = false;
        }

        void ProfileTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (P1 != P0)
            {
                DoProfile(P0, P1);

            }
        }

        protected override void OnDeactivate()
        {
            ProfileTimer.Enabled = false;
            base.OnDeactivate();
        }
        public LineProfileTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciResources.SciResources.GetImageResource("Icons.stock-tool-curves-16.png"),
                   "Profile tool",
                   "",
                   'l',
                   ToolBarConfigItems.None)
        {
            ProfileTimer.Elapsed += new System.Timers.ElapsedEventHandler(ProfileTimer_Elapsed);
            ProfileTimer.Interval = 200;
        }
    }
}
