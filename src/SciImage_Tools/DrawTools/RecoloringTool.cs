/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SciImage;
using SciImage.Core;
using SciImage.Core.BitVector;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UserBlendOps;
using SciImage.Core.Surfaces.Layers;
using SciImage.Plugins.Tools;

namespace SciImage_Tools.DrawTools
{
    public class RecolorTool 
        : Tool
    {
        private bool mouseDown;

        private Point lastMouseXY;
        private MouseButtons mouseButton;
        private RenderArgs renderArgs;
        private Layer bitmapLayer;
        private ArrayList savedSurfaces;
        private BrushPreviewRenderer previewRenderer;

        private float penWidth;
        private int ceilingPenWidth;
        private int halfPenWidth;
        private ColorPixelBase colorToReplace;
        private ColorPixelBase colorReplacing;
        private Cursor cursorMouseDown;
        private Cursor cursorMouseUp;
        private Cursor cursorMouseDownPickColor;
        private Cursor cursorMouseDownAdjustColor;

        // private ColorPixelBase replacementDiff;
        private static ColorPixelBase colorToleranceBasis =ColorBgra.sFromBgra(0x20, 0x20, 0x20, 0x00);
        private SciRegion clipRegion;
        private NormalBlendOp blendOp = new NormalBlendOp();
        private bool hasDrawn;
        private Keys modifierDown;

        // AA stuff
        private BitVector2D isPointAlreadyAA;
        private Surface aaPoints;

        public ColorPixelBase AAPoints(int x, int y)
        {
            return aaPoints.ColorPixelBase.TranslateColor( aaPoints[x, y] );
        }

        public void AAPointsAdd(int x, int y, ColorPixelBase color)
        {
            aaPoints[x, y] = aaPoints.ColorPixelBase.TranslateColor( color ).ToInt32();
            isPointAlreadyAA[x, y] = true;
        }

        public void AAPointsRemove(int x, int y)
        {
            isPointAlreadyAA[x, y] = false;
        }

        private bool IsPointAlreadyAntiAliased(int x, int y)
        {
            return isPointAlreadyAA[x, y];
        }

        private bool IsPointAlreadyAntiAliased(Point pt)
        {
            return IsPointAlreadyAntiAliased(pt.X, pt.Y);
        }

        // RenderArgs specifically for a brush mask
        private RenderArgs brushRenderArgs;

        private int myTolerance;

        public SciRegion ClipRegion
        {
            get
            {
                return clipRegion;
            }

            set
            {
                clipRegion = value;
            }
        }

        private bool IsColorInTolerance(ColorPixelBase colorA, ColorPixelBase colorB)
        {
            return Utility.ColorDifference(colorA, colorB) <= myTolerance;
        }

        private void RestrictTolerance()
        {
            int difference = Utility.ColorDifference(colorReplacing, colorToReplace);

            if (myTolerance > difference)
            {
                myTolerance = difference;
            }
        }

        protected override void OnMouseEnter()
        {
            this.previewRenderer.Visible = true;
            base.OnMouseEnter();
        }

        protected override void OnMouseLeave()
        {
            this.previewRenderer.Visible = false;
            base.OnMouseLeave();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            // initialize any state information you need
            cursorMouseUp = new Cursor(SciImage.SciResources.SciResources.GetResourceStream("Cursors.RecoloringToolCursor.cur"));
            cursorMouseDown = new Cursor(SciImage.SciResources.SciResources.GetResourceStream("Cursors.GenericToolCursorMouseDown.cur"));
            cursorMouseDownPickColor = new Cursor(SciImage.SciResources.SciResources.GetResourceStream("Cursors.RecoloringToolCursorPickColor.cur"));
            cursorMouseDownAdjustColor = new Cursor(SciImage.SciResources.SciResources.GetResourceStream("Cursors.RecoloringToolCursorAdjustColor.cur"));

            this.previewRenderer = new BrushPreviewRenderer(this.RendererList);
            this.RendererList.Add(this.previewRenderer, false);

            Cursor = cursorMouseUp;
            mouseDown = false;

            // fetch colors from workspace palette
            this.colorToReplace = this.ToolEnvironment.PrimaryColor;
            this.colorReplacing = this.ToolEnvironment.SecondaryColor;

            this.aaPoints = this.ScratchSurface;
            this.isPointAlreadyAA = new BitVector2D(aaPoints.Width, aaPoints.Height);

            if (savedSurfaces != null)
            {
                foreach (PlacedSurface ps in savedSurfaces)
                {
                    ps.Dispose();
                }
            }

            savedSurfaces = new ArrayList();

            if (ActiveLayer != null)
            {
                bitmapLayer = (Layer)ActiveLayer;
                renderArgs = new RenderArgs(bitmapLayer.Surface);
            }
            else
            {
                bitmapLayer = null;
                renderArgs = null;
            }
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            if (mouseDown)
            {
                OnMouseUp(new MouseEventArgs(mouseButton, 0, lastMouseXY.X, lastMouseXY.Y, 0));
            }

            this.RendererList.Remove(this.previewRenderer);
            this.previewRenderer.Dispose();
            this.previewRenderer = null;

            if (savedSurfaces != null)
            {
                if (savedSurfaces != null)
                {
                    foreach (PlacedSurface ps in savedSurfaces)
                    {
                        ps.Dispose();
                    }
                }

                savedSurfaces.Clear();
                savedSurfaces = null;
            }

            renderArgs.Dispose();;
            renderArgs = null;

            aaPoints = null;
            renderArgs = null;
            bitmapLayer = null;

            if (ClipRegion != null)
            {
                ClipRegion.Dispose();
                ClipRegion = null;
            }


            if (cursorMouseUp != null)
            {
                cursorMouseUp.Dispose();
                cursorMouseUp = null;
            }

            if (cursorMouseDown != null)
            {
                cursorMouseDown.Dispose();
                cursorMouseDown = null;
            }

            if (cursorMouseDownPickColor != null)
            {
                cursorMouseDownPickColor.Dispose();
                cursorMouseDownPickColor = null;
            }

            if (cursorMouseDownAdjustColor != null)
            {
                cursorMouseDownAdjustColor.Dispose();
                cursorMouseDownAdjustColor = null;
            }
        }

        private ColorPixelBase LiftColor(int x, int y)
        {
            return ((Layer)ActiveLayer).Surface.ColorPixelBase.TranslateColor(((Layer)ActiveLayer).Surface[x, y] );
        }

        /// <summary>
        /// Picks up the color under the mouse and assigns to the forecolor (or backcolor).
        /// If assigning to the forecolor, the backcolor will be adjusted respective to the
        /// difference of the old forecolor versus the new forecolor.
        /// </summary>
        /// <param name="e"></param>
        private void AdjustDrawingColor(MouseEventArgs e)
        {
            ColorPixelBase oldColor;

            if (BtnDownMouseLeft(e))
            {
                oldColor = this.ToolEnvironment.PrimaryColor;
                PickColor(e);

                this.ToolEnvironment.SecondaryColor = AdjustColorDifference(oldColor, 
                    this.ToolEnvironment.PrimaryColor, this.ToolEnvironment.SecondaryColor);
            }

            if (BtnDownMouseRight(e))
            {
                oldColor = this.ToolEnvironment.SecondaryColor;
                PickColor(e);

                this.ToolEnvironment.PrimaryColor = AdjustColorDifference(oldColor, 
                    this.ToolEnvironment.SecondaryColor, this.ToolEnvironment.PrimaryColor);
            }
        }

        private byte AdjustColorByte(byte oldByte, byte newByte, byte basisByte)
        {
            if (oldByte > newByte)
            {
                return Utility.ClampToByte(basisByte - (oldByte - newByte));
            }
            else
            {
                return Utility.ClampToByte(basisByte + (newByte - oldByte));
            }
        }

        /// <summary>
        /// Returns a ColorPixelBase shift by the difference between oldcolor and newcolor but using 
        /// basisColor as the basis.
        /// </summary>
        /// <param name="oldcolor"></param>
        /// <param name="newcolor"></param>
        /// <param name="shiftColor"></param>
        /// <returns></returns>
        private ColorPixelBase AdjustColorDifference(ColorPixelBase oldColor, ColorPixelBase newColor, ColorPixelBase basisColor) 
        {
            ColorPixelBase returnColor = basisColor;
            ColorPixelBase o = returnColor.TranslateColor(oldColor);
            ColorPixelBase n = returnColor.TranslateColor(newColor);
            for (int i = 0; i < returnColor.NumChannels; i++)
                returnColor.SetChannel(i, AdjustColorByte((byte)o.GetChannel(i), (byte)n.GetChannel(i), (byte)basisColor.GetChannel(i)));

            return returnColor;
        }

        private void PickColor(MouseEventArgs e)
        {
            if (!DocumentWorkspace.Document.Bounds.Contains(e.X, e.Y))
            {
                return;
            }

            // if we managed to get here without any mouse buttons down
            // we return promptly.
            if (BtnDownMouseLeft(e) || BtnDownMouseRight(e))
            {
                // since the above statement exits if one or the other
                if (BtnDownMouseLeft(e))
                {
                    colorReplacing = LiftColor(e.X, e.Y);
                    colorReplacing.alpha = this.ToolEnvironment.PrimaryColor.alpha;
                    this.ToolEnvironment.PrimaryColor = colorReplacing;
                }
                else
                {
                    colorToReplace = LiftColor(e.X, e.Y);
                    colorToReplace.alpha = this.ToolEnvironment.SecondaryColor.alpha ;
                    this.ToolEnvironment.SecondaryColor = colorToReplace;
                }
            }
            else
            {
                return;
            }

            // before assigned the newly lifted color, we preserve the
            // alpha from the user's selected color.
        }

        private RenderArgs RenderCircleBrush()
        {
            // create pen mask RGB32_Surface
            Surface brushSurface = new Surface(ceilingPenWidth, ceilingPenWidth,new ColorBgra());
            brushSurface.Clear(ColorBgra.Zero );
            RenderArgs brush = new RenderArgs(brushSurface);

            if (ToolEnvironment.AntiAliasing)
            {
                brush.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }
            else
            {
                brush.Graphics.SmoothingMode = SmoothingMode.None;
            }

            if (ToolEnvironment.AntiAliasing)
            {
                if (penWidth > 2)
                {
                    penWidth = penWidth - 1.0f;
                }
                else
                {
                    penWidth = penWidth / 2;
                }
            }
            else
            {
                if (penWidth <= 1.0f)
                {
                    brush.Surface[1, 1] = ColorBgra.Black.ToInt32();
                }
                else
                {
                    penWidth = (float)Math.Round(penWidth + 1.0f);
                }
            }

            using (Brush testBrush = new SolidBrush(System.Drawing.Color.Black))
            {
                brush.Graphics.FillEllipse(testBrush, 0.0f, 0.0f, penWidth, penWidth);
            }

            return brush;
        }

        private unsafe void DrawOverPoints(Point start, Point finish, ColorPixelBase colorToReplaceWith, ColorPixelBase colorBeingReplaced)
        {
            ColorPixelBase colorAdjusted = colorBeingReplaced.FromColor(Color.Empty);
            byte dstAlpha;
            ColorPixelBase colorLifted;
            Rectangle[] rectSelRegions;
            Rectangle rectBrushArea;
            Rectangle rectBrushRelativeOffset = new Rectangle(0, 0, 0, 0);
            
            // special condition for a canvas with no active selection
            // create an array of rectangles with a single rectangle 
            // specifying the size of the canvas
            if (Selection.IsEmpty)
            {
                rectSelRegions = new Rectangle[] { DocumentWorkspace.Document.Bounds };
            }
            else
            {
                rectSelRegions = ClipRegion.GetRegionScansReadOnlyInt();
            }

            // code ripped off from clone stamp tool
            Point direction = new Point(finish.X - start.X, finish.Y - start.Y);
            float length = Utility.Magnitude(direction);
            float bw = ToolEnvironment.PenInfo.Width / 2;

            float fInc;
            if (length == 0.0f)
            {
                fInc = float.PositiveInfinity;
            }
            else
            {
                fInc = (float)Math.Sqrt(bw) / length;
            }

            // iterate through all points in the linear stroke
            for (float f = 0; f < 1; f += fInc) 
            {
                PointF q = new PointF(finish.X * (1 - f) + f * start.X, 
                                      finish.Y * (1 - f) + f * start.Y);

                Point p = Point.Round(q);

                // iterate through all rectangles
                foreach (Rectangle rectSel in rectSelRegions)
                {
                    // set the perimeter values for the rectBrushRegion rectangle
                    // so the area can be intersected with the active
                    // selection individual recSelRegion rectangle.
                    rectBrushArea = new Rectangle(p.X - halfPenWidth, p.Y - halfPenWidth, ceilingPenWidth, ceilingPenWidth);

                    // test the intersection...
                    // the perimeter values of rectBrushRegion (above)
                    // may calculate negative but
                    // *should* always be clipped to acceptable values by
                    // by the following intersection.
                    if (rectBrushArea.IntersectsWith(rectSel))
                    {
                        // a valid intersection was found.
                        // prune the brush rectangle to fit the intersection.
                        rectBrushArea.Intersect(rectSel);
                        for (int y = rectBrushArea.Top; y < rectBrushArea.Bottom; y++)
                        {
                            // create a new rectangle for an offset relative to the 
                            // the brush mask
                            rectBrushRelativeOffset.X = Math.Max(rectSel.X - (p.X - halfPenWidth), 0); 
                            rectBrushRelativeOffset.Y = Math.Max(rectSel.Y - (p.Y - halfPenWidth), 0);
                            rectBrushRelativeOffset.Size = rectBrushArea.Size;
                            
                            ColorPixelBase srcBgra;
                            ColorPixelBase dstBgra;


                            int xx = 0;
                            for (int x = rectBrushArea.Left; x < rectBrushArea.Right; x++)
                            {
                                    // get the source address of the first pixel from the brush mask.
                                    srcBgra = brushRenderArgs.Surface.GetPoint(rectBrushRelativeOffset.Left+xx,
                                        rectBrushRelativeOffset.Y + (y - rectBrushArea.Y));

                                    // get the address of the pixel we want to change on the canvas.
                                    dstBgra = renderArgs.Surface.GetPoint(x, y);
                              
                                if (srcBgra.alpha  != 0)
                                {
                                    colorLifted = dstBgra;

                                    // hasDrawn is set if a pixel endures color replacement so that 
                                    // the placed RGB32_Surface will be left alone, otherwise, the placed
                                    // RGB32_Surface will be discarded
                                    // adjust the channel color up and down based on the difference calculated
                                    // from the source.  These values are clamped to a byte.  It's possible
                                    // that the new color is too dark or too bright to take the whole range 

                                    bool boolCIT = this.IsColorInTolerance(colorLifted, colorBeingReplaced);
                                    bool boolPAAA = false;

                                    if (ToolEnvironment.AntiAliasing)
                                    {
                                        boolPAAA = this.IsPointAlreadyAntiAliased(x, y);
                                    }

                                    if (boolCIT || boolPAAA)
                                    {
                                        if (boolPAAA)
                                        {
                                            colorAdjusted = (ColorPixelBase)AAPoints(x, y);

                                            if (penWidth < 2.0f)
                                            {
                                                colorAdjusted[0] = Utility.ClampToByte(colorToReplaceWith[0] + (colorAdjusted[0] - colorBeingReplaced[0]));
                                                colorAdjusted[1] = Utility.ClampToByte(colorToReplaceWith[1] + (colorAdjusted[1] - colorBeingReplaced[1]));
                                                colorAdjusted[2] = Utility.ClampToByte(colorToReplaceWith[2] + (colorAdjusted[2] - colorBeingReplaced[2]));
                                                colorAdjusted.alpha = Utility.ClampToByte(colorToReplaceWith.alpha + (colorAdjusted.alpha - colorBeingReplaced.alpha));
                                            }
                                        }
                                        else
                                        {
                                            colorAdjusted[0]  = Utility.ClampToByte(colorLifted[0]  + (colorToReplaceWith[0]  - colorBeingReplaced[0] ));
                                            colorAdjusted[1]  = Utility.ClampToByte(colorLifted[1]  + (colorToReplaceWith[1]  - colorBeingReplaced[1] ));
                                            colorAdjusted[2]  = Utility.ClampToByte(colorLifted[2]  + (colorToReplaceWith[2]  - colorBeingReplaced[2] ));
                                            colorAdjusted.alpha  = Utility.ClampToByte(colorLifted.alpha  + (colorToReplaceWith.alpha  - colorBeingReplaced.alpha ));
                                        }

                                        if ((srcBgra.alpha  != 255) && ToolEnvironment.AntiAliasing)
                                        {
                                            colorAdjusted.alpha  = srcBgra.alpha ;
                                            dstAlpha = dstBgra.alpha ;
                                            renderArgs.Surface.SetPoint(x,y, blendOp.Apply(dstBgra, colorAdjusted).NewAlpha( dstAlpha ));
                                            
                                            if (!this.IsPointAlreadyAntiAliased(x, y))
                                            {
                                                AAPointsAdd(x, y, colorAdjusted);
                                            }
                                        }
                                        else
                                        {
                                            colorAdjusted.alpha  = (dstBgra).alpha ;
                                            renderArgs.Surface.SetPoint(x, y,  colorAdjusted );

                                            if (boolPAAA)
                                            {
                                               AAPointsRemove(x, y);
                                            }
                                        }
                                        
                                        hasDrawn = true;
                                    }
                                }

                                ++xx;
                            }
                        }
                    }
                }
            }
        }

        private bool KeyDownShiftOnly()
        {
            return ModifierKeys == Keys.Shift;
        }

        private bool KeyDownControlOnly()
        {
            return ModifierKeys == Keys.Control;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((modifierDown == Keys.Control) || 
                (modifierDown == Keys.Shift))
            {
                return;
            }
            else
            {
                if (!mouseDown)
                {
                    if (KeyDownControlOnly())
                    {
                        Cursor = cursorMouseDownPickColor;
                    }
                    else if (KeyDownShiftOnly())
                    {
                        Cursor = cursorMouseDownAdjustColor;
                    }
                    else
                    {
                        base.OnKeyDown(e);
                    }
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!KeyDownControlOnly() && !KeyDownShiftOnly())
            {
                if (!mouseDown)
                {
                    modifierDown = 0;
                    Cursor = cursorMouseUp;
                }
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Button down mouse left.  Returns true if only the left mouse button is depressed.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool BtnDownMouseLeft(MouseEventArgs e)
        {
            return(e.Button == MouseButtons.Left);
        }

        /// <summary>
        /// Button down mouse right.  Returns true if only the right mouse is depressed.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool BtnDownMouseRight(MouseEventArgs e)
        {
            return(e.Button == MouseButtons.Right);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (mouseDown)
            {
                return;
            }

            if (BtnDownMouseLeft(e) || BtnDownMouseRight(e))
            {
                this.previewRenderer.Visible = false;

                mouseDown = true;
                Cursor = cursorMouseDown;
                
                if ((!KeyDownControlOnly()) && (!KeyDownShiftOnly()))
                {
                    mouseButton = e.Button;
                
                    lastMouseXY.X = e.X;
                    lastMouseXY.Y = e.Y;

                    // parses and establishes the active selection area
                    if (ClipRegion != null)
                    {
                        ClipRegion.Dispose();
                        ClipRegion = null;
                    }

                    ClipRegion = Selection.CreateRegion();
                    renderArgs.Graphics.SetClip(ClipRegion.GetRegionReadOnly(), CombineMode.Replace);

                    // find the replacement color and the color to replace
                    colorReplacing = ToolEnvironment.PrimaryColor;
                    colorToReplace = ToolEnvironment.SecondaryColor;
                    penWidth = ToolEnvironment.PenInfo.Width;

                    // get the pen width find the ceiling integer of half of the pen width
                    ceilingPenWidth = (int)Math.Max(Math.Ceiling(penWidth), 3);

                    // used only for cursor positioning
                    halfPenWidth    = (int)Math.Ceiling(penWidth / 2.0f);

                    // set hasDrawn to false since nothing has been drawn       
                    hasDrawn = false;

                    // render the circle via GDI+ so the AA techniques can precisely
                    // mimic GDI+.
                    this.brushRenderArgs = RenderCircleBrush(); 

                    // establish tolerance
                    myTolerance = (int)(ToolEnvironment.Tolerance * 256);

                    // restrict tolerance so no overlap is permitted
                    RestrictTolerance();
                    OnMouseMove(e);
                }
                else
                {
                    modifierDown = ModifierKeys;
                    OnMouseMove(e);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!KeyDownShiftOnly() && !KeyDownControlOnly())
            {
                Cursor = cursorMouseUp;
            }

            if (mouseDown)
            {
                this.previewRenderer.Visible = true;

                OnMouseMove(e);

                if (savedSurfaces.Count > 0)
                {
                    SciRegion saveMeRegion = new SciRegion();
                    saveMeRegion.MakeEmpty();

                    foreach (PlacedSurface pi1 in savedSurfaces)
                    {
                        saveMeRegion.Union(pi1.Bounds);
                    }

                    SciRegion simplifiedRegion = Utility.SimplifyAndInflateRegion(saveMeRegion);

                    using (IrregularSurface weDrewThis = new IrregularSurface(renderArgs.Surface, simplifiedRegion))
                    {
                        for (int i = savedSurfaces.Count - 1; i >= 0; --i)
                        {
                            PlacedSurface ps = (PlacedSurface)savedSurfaces[i];
                            ps.Draw(renderArgs.Surface);
                            ps.Dispose();
                        }

                        savedSurfaces.Clear();

                        if (hasDrawn)
                        {
                            HistoryMemento ha = new BitmapHistoryMemento(Name, Image, DocumentWorkspace, 
                                ActiveLayerIndex, simplifiedRegion);

                            weDrewThis.Draw(bitmapLayer.Surface);
                            HistoryStack.PushNewMemento(ha);
                        }
                    }
                }

                mouseDown = false;
                modifierDown = 0;
            }

            if (brushRenderArgs != null)
            {
                if (brushRenderArgs.Surface != null)
                {
                    brushRenderArgs.Surface.Dispose();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.previewRenderer.BrushLocation = new Point(e.X, e.Y);
            this.previewRenderer.BrushSize = ToolEnvironment.PenInfo.Width / 2.0f;

            if (mouseDown)
            {
                if (BtnDownMouseLeft(e) || BtnDownMouseRight(e))
                {
                    if (modifierDown == 0)
                    {
                        // if the primary and secondary colors are identical,
                        // return...there's no point in committing any action
                        if (colorReplacing == colorToReplace)
                        {
                            return;
                        }

                        // get our start and end coordinates, since we need
                        // to trace along an action line -- the user will expect this behavior
                        // if we don't, it'll look like a tin can riddled with bullet holes
                        Point pointStartCorner = lastMouseXY;          // start point
                        Point pointEndCorner = new Point(e.X, e.Y);  // end point

                        // create the rectangle with the 'a' and 'b' points above
                        Rectangle inspectionRect = 
                            Utility.PointsToRectangle(pointStartCorner, pointEndCorner);

                        // inflate the region to address account for the pen width
                        // then intersect with the Workspace to "clip" the boundary
                        // the total area of the clipped rectangle includes the
                        // width of the pen surrounding the points limited by either
                        // the canvas perimeter or the selection outline
                        inspectionRect.Inflate(1 + ceilingPenWidth / 2, 1 + ceilingPenWidth / 2);
                        inspectionRect.Intersect(ActiveLayer.Bounds);

                        // Enforce the selection area restrictions.
                        // If within the selection area restrictions, build an image history
                        bool gotWidth = inspectionRect.Width  > 0;
                        bool gotHeight = inspectionRect.Height > 0;
                        bool isInClip = renderArgs.Graphics.IsVisible(inspectionRect);

                        if ((gotWidth) && (gotHeight) && (isInClip))
                        {
                            PlacedSurface savedPS = new PlacedSurface(renderArgs.Surface, inspectionRect);
                            savedSurfaces.Add(savedPS);

                            renderArgs.Graphics.CompositingMode = CompositingMode.SourceOver;
                    
                            // check the mouse buttons and if we've made it this far, at least
                            // one of the mouse buttons (left|right) was depressed
                            if (BtnDownMouseLeft(e))
                            {
                                this.DrawOverPoints(pointStartCorner, pointEndCorner, colorReplacing, colorToReplace);
                            }
                            else
                            {
                                this.DrawOverPoints(pointStartCorner, pointEndCorner, colorToReplace, colorReplacing);
                            }

                            bitmapLayer.Invalidate(inspectionRect);
                            Update();
                        }

                        // update the lastMouseXY so we know how to "connect the dots"
                        lastMouseXY = pointEndCorner;
                    }
                    else
                    {
                        switch (modifierDown & (Keys.Control | Keys.Shift))
                        {
                            case Keys.Control: 
                                PickColor(e);
                                break;

                            case Keys.Shift: 
                                AdjustDrawingColor(e);
                                break;

                            default: 
                                break;
                        }
                    }
                }
            }
        }

        public RecolorTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciImage.SciResources.SciResources.GetImageResource("Icons.glyphicons-484-recolor.png"),
                   "Switch Colors", 
                   "Replaces the primary color with the secondary",
                   'r',7,
                   false,
                   ToolBarConfigItems.Pen | ToolBarConfigItems.Antialiasing | ToolBarConfigItems.Tolerance)
        {
        }
    }
}