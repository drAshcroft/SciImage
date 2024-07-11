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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UserBlendOps;
using SciImage.Core.Surfaces.Layers;
using SciImage.PaintForms.UserControls;
using SciImage.PaintForms.UserControls.Buttons;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;
using SciImage.Plugins.Tools.DefaultTools;
using SciImage.Plugins.Tools.Enums;
using SciImage.SystemLayer.Forms;
using SciImage.SystemLayer.System;
using SciImage_Actions.Actions.Image_Actions.CanvasSize;
using Clipboard = System.Windows.Forms.Clipboard;

namespace SciImage_Actions.Actions.Edit_Actions
{
    public sealed class PasteAction:PluginAction 
    {
        
        private sealed class IntensityMaskOp
            : BinaryPixelOp
        {
            public override ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs)
            {
                byte intensity = rhs.GetIntensityByte();
                ColorPixelBase result = lhs.FromBgra((byte)lhs[0], (byte)lhs[1], (byte)lhs[2], (byte)Utility.FastScaleByteByByte(intensity, lhs.alpha));
                return result;
            }
        }

        public override string Name
        {
            get
            {
                return "Paste";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return "Edit"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }

        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 2; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 3; }
        }

        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null)
            {
                return ActionDisplayOptions.Visible;
            }
            if (documentWorkspace.ActiveLayer.Surface!=null)
            {
                return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
            }
            return ActionDisplayOptions.Hidden;
        }

        private SurfaceForClipboard CreateSurfacefromImage(Image image)
        {
            // Sometimes we get weird errors if we're in, say, 16-bit mode but the image was copied
            // to the clipboard in 32-bit mode
            if (image == null)
            {
               
                return null;
            }

            MaskedSurface maskedSurface = null;

            try
            {
                Utility.GCFullCollect();
                Bitmap bitmap;
                Surface surface = null;

                if (image is Bitmap)
                {
                    bitmap = (Bitmap)image;
                    image = null;
                }
                else
                {
                    bitmap = new Bitmap(image);
                    image.Dispose();
                    image = null;
                }

                surface = Surface.CopyFromBitmap(bitmap);
                bitmap.Dispose();
                bitmap = null;

                maskedSurface = new MaskedSurface(surface, new SciRegion(surface.Bounds));

                surface.Dispose();
                surface = null;
            }

            catch (Exception)
            {
                return null;
            }

            return  new SurfaceForClipboard(maskedSurface);

        }
        private bool GetClipBoardData(DocumentWorkspace documentWorkspace, Layer ActiveLayer,out SurfaceForClipboard surfaceForClipboard)
        {
            surfaceForClipboard = null;
            IDataObject clipData = null;
            try
            {
                Utility.GCFullCollect();
                clipData = Clipboard.GetDataObject();
            }
            catch (ExternalException)
            {
                Utility.ErrorBox(documentWorkspace, SciImage.SciResources.SciResources.GetString("PasteAction.Error.TransferFromClipboard"));
                return false;
            }
            catch (OutOfMemoryException)
            {
                Utility.ErrorBox(documentWorkspace, SciImage.SciResources.SciResources.GetString("PasteAction.Error.OutOfMemory"));
                return false;
            }
            finally
            {
                if (clipData == null)
                {
                    try
                    {
                        Image clipImage = Clipboard.GetImage();
                        surfaceForClipboard = CreateSurfacefromImage(clipImage);
                        
                    }
                    catch
                    {

                    }
                }
            }
            //need to leave procedure if the image has been loaded.
            if (surfaceForClipboard != null)
                return true;


            if (clipData.GetDataPresent(typeof(SurfaceForClipboard)))
            {
                try
                {
                    Utility.GCFullCollect();
                    surfaceForClipboard = clipData.GetData(typeof(SurfaceForClipboard)) as SurfaceForClipboard;
                    return true;
                }
                catch (OutOfMemoryException)
                {
                    Utility.ErrorBox(documentWorkspace, SciImage.SciResources.SciResources.GetString("PasteAction.Error.OutOfMemory"));
                    return false;
                }
            }

            if (surfaceForClipboard != null && surfaceForClipboard.MaskedSurface.IsDisposed)
            {
                // Have been getting crash reports where sfc contains a disposed MaskedSurface ...
                surfaceForClipboard = null;
                return false;
            }

            if (surfaceForClipboard == null &&
                (clipData.GetDataPresent(DataFormats.Bitmap, true) || clipData.GetDataPresent(DataFormats.EnhancedMetafile, true)))
            {
                Image image;
                try
                {
                    Utility.GCFullCollect();
                    image = clipData.GetData(DataFormats.Bitmap, true) as Image;

                    if (image == null)
                    {
                        image = SciImage.SystemLayer.System.Clipboard.GetEmfFromClipboard(documentWorkspace);
                    }
                }

                catch (OutOfMemoryException)
                {
                    Utility.ErrorBox(documentWorkspace, SciImage.SciResources.SciResources.GetString("PasteAction.Error.OutOfMemory"));
                    return false;
                }
                surfaceForClipboard= CreateSurfacefromImage(image);
                return !(surfaceForClipboard == null);
            }

            if (surfaceForClipboard == null || surfaceForClipboard.MaskedSurface == null)
            {
                // silently fail: like what if a program overwrote the clipboard in between the time
                // we enabled the "Paste" menu item and the user actually clicked paste?
                // it could happen!
                Utility.ErrorBox(documentWorkspace, SciImage.SciResources.SciResources.GetString("PasteAction.Error.NoImage"));
                return false;
            }
            return true;

        }

        private bool CheckPasteSize(DocumentWorkspace documentWorkspace, int layerIndex, SurfaceForClipboard surfaceForClipboard, ref List<HistoryMemento> OptionalHistoryRecord)
        {
            // If the image is larger than the document, ask them if they'd like to make the image larger first
            Rectangle bounds = surfaceForClipboard.Bounds;
            HistoryMemento rdha = null;
            if (bounds.Width > documentWorkspace.Document.Width ||
                bounds.Height > documentWorkspace.Document.Height)
            {
                Surface thumb;

                try
                {
                    using (new WaitCursorChanger(documentWorkspace))
                    {
                        thumb = CreateThumbnail(surfaceForClipboard);
                    }
                }

                catch (OutOfMemoryException)
                {
                    thumb = null;
                }

                DialogResult dr = ShowExpandCanvasTaskDialog(documentWorkspace, thumb);


                switch (dr)
                {
                    case DialogResult.Yes:
                        Size newSize = new Size(Math.Max(bounds.Width, documentWorkspace.Document.Width),
                                                Math.Max(bounds.Height, documentWorkspace.Document.Height));

                        Document newDoc = CanvasSizeAction.ResizeDocument(
                            documentWorkspace.Document,
                            newSize,
                            AnchorEdge.TopLeft,
                            ToolEnvironment.Environment.SecondaryColor);

                        if (newDoc == null)
                        {
                            return false; // user clicked cancel!
                        }
                        else
                        {
                            rdha = new ReplaceDocumentHistoryMemento(
                                CanvasSizeAction.StaticName,
                                CanvasSizeAction.StaticImage,
                                documentWorkspace);

                            documentWorkspace.Document = newDoc;
                            if (OptionalHistoryRecord == null)
                                documentWorkspace.History.PushNewMemento(rdha);
                            else
                                OptionalHistoryRecord.Add(rdha);
                            documentWorkspace.ActiveLayer = (Layer)documentWorkspace.Document.Layers[layerIndex];
                        }

                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        return false;

                    default:
                        throw new InvalidEnumArgumentException("Internal error: DialogResult was neither Yes, No, nor Cancel");
                }
            }
            return true;
        }
        /// <summary>
        /// Pastes from the clipboard into the document.
        /// </summary>
        /// <returns>true if the paste operation completed, false if there was an error or if it was cancelled for some reason</returns>
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            SurfaceForClipboard surfaceForClipboard = null;
            IDataObject clipData = null;
            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;
            documentWorkspace.ActiveLayer = (Layer)documentWorkspace.Document.Layers[index];


            // First "ask" the current tool if it wants to handle it
            bool handledByTool = false;
            if (documentWorkspace.Tool != null)
            {
                documentWorkspace.Tool.PerformPaste(clipData, out handledByTool);
                if (handledByTool)
                {
                    return true;
                }

            }

            bool ret= GetClipBoardData(documentWorkspace, documentWorkspace.ActiveLayer, out surfaceForClipboard);
            if (ret == false)
                return false;

            ret = CheckPasteSize(documentWorkspace, index, surfaceForClipboard, ref OptionalHistoryRecord);
            if (ret == false)
                return false;

            Rectangle bounds = surfaceForClipboard.Bounds;
            // Decide where to paste to: If the paste is within bounds of the document, do as normal
            // Otherwise, center it.
            Rectangle docBounds = documentWorkspace.Document.Bounds;
            Rectangle intersect1 = Rectangle.Intersect(docBounds, bounds);
            bool doMove = intersect1 != bounds; //intersect1.IsEmpty;

            Point pasteOffset;

            if (doMove)
            {
                pasteOffset = new Point(-bounds.X + (docBounds.Width / 2) - (bounds.Width / 2),
                                        -bounds.Y + (docBounds.Height / 2) - (bounds.Height / 2));
            }
            else
            {
                pasteOffset = new Point(0, 0);
            }

            // Paste to the place it was originally copied from (for PDN-to-PDN transfers)
            // and then if its not pasted within the viewable rectangle we pan to that location
            RectangleF visibleDocRectF = documentWorkspace.VisibleDocumentRectangleF;
            Rectangle visibleDocRect = Utility.RoundRectangle(visibleDocRectF);
            Rectangle bounds2 = new Rectangle(new Point(bounds.X + pasteOffset.X, bounds.Y + pasteOffset.Y), bounds.Size);
            Rectangle intersect2 = Rectangle.Intersect(bounds2, visibleDocRect);
            bool doPan = intersect2.IsEmpty;

            documentWorkspace.SetTool(null);
            documentWorkspace.SetToolFromType(typeof(MoveTool));

            ((MoveTool)documentWorkspace.Tool).PasteMouseDown(surfaceForClipboard, pasteOffset);

            if (doPan)
            {
                Point centerPtView = new Point(visibleDocRect.Left + (visibleDocRect.Width / 2),
                                               visibleDocRect.Top + (visibleDocRect.Height / 2));

                Point centerPtPasted = new Point(bounds2.Left + (bounds2.Width / 2),
                                                 bounds2.Top + (bounds2.Height / 2));

                Size delta = new Size(centerPtPasted.X - centerPtView.X,
                                      centerPtPasted.Y - centerPtView.Y);

                PointF docScrollPos = documentWorkspace.DocumentScrollPositionF;

                PointF newDocScrollPos = new PointF(docScrollPos.X + delta.Width,
                                                    docScrollPos.Y + delta.Height);

                documentWorkspace.DocumentScrollPositionF = newDocScrollPos;
            }

            return true;
        }

        private static Surface CreateThumbnail(SurfaceForClipboard surfaceForClipboard)
        {
            const int thumbLength96dpi = 120;
            int thumbSizeOurDpi = UI.ScaleWidth(thumbLength96dpi);

            Surface surface = surfaceForClipboard.MaskedSurface.SurfaceReadOnly;
            PdnGraphicsPath maskPath = surfaceForClipboard.MaskedSurface.CreatePath();
            Rectangle bounds = surfaceForClipboard.Bounds;

            Surface thumb = CreateThumbnail(surface, maskPath, bounds, thumbSizeOurDpi);

            maskPath.Dispose();

            return thumb;
        }

        public static Surface CreateThumbnail(Surface sourceSurface, PdnGraphicsPath maskPath, Rectangle bounds, int thumbSideLength)
        {
            Size thumbSize = Utility.ComputeThumbnailSize(bounds.Size, thumbSideLength);

            Surface thumb = new Surface(Math.Max(5, thumbSize.Width + 4), Math.Max(5, thumbSize.Height + 4),new ColorBgra() );
            thumb.Clear(thumb.ColorPixelBase.TransparentColor() );
            thumb.Clear(new Rectangle(1, 1, thumb.Width - 2, thumb.Height - 2), thumb.ColorPixelBase.BlackColor());

            Rectangle insetRect = new Rectangle(2, 2, thumb.Width - 4, thumb.Height - 4);

            Surface thumbInset = thumb.CreateWindow(insetRect);
            thumbInset.Clear(sourceSurface.ColorPixelBase.TransparentColor());

            float scaleX = (float)thumbInset.Width / (float)bounds.Width;
            float scaleY = (float)thumbInset.Height / (float)bounds.Height;

            Matrix scaleMatrix = new Matrix();
            scaleMatrix.Translate(-bounds.X, -bounds.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
            scaleMatrix.Scale(scaleX, scaleY, System.Drawing.Drawing2D.MatrixOrder.Append);

            thumbInset.SuperSamplingFitSurface(sourceSurface);

            Surface maskInset = new Surface(thumbInset.Size, sourceSurface.ColorPixelBase);
            maskInset.Clear(sourceSurface.ColorPixelBase.BlackColor());
            using (RenderArgs maskInsetRA = new RenderArgs(maskInset))
            {
                maskInsetRA.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                maskInsetRA.Graphics.Transform = scaleMatrix;
                maskInsetRA.Graphics.FillPath(Brushes.White, maskPath);
                maskInsetRA.Graphics.DrawPath(Pens.White, maskPath);
            }

            scaleMatrix.Dispose();
            scaleMatrix = null;

            IntensityMaskOp maskOp = new IntensityMaskOp();
            maskOp.Apply(maskInset, thumbInset, maskInset);

            NormalBlendOp normalOp = new NormalBlendOp();
            thumbInset.ClearWithCheckboardPattern();
            normalOp.Apply(thumbInset, thumbInset, maskInset);

            maskInset.Dispose();
            maskInset = null;

            thumbInset.Dispose();
            thumbInset = null;

            using (RenderArgs thumbRA = new RenderArgs(thumb))
            {
                Utility.DrawDropShadow1px(thumbRA.Graphics, thumb.Bounds);
            }

            return thumb;
        }

        private static DialogResult ShowExpandCanvasTaskDialog(IWin32Window owner, Surface thumbnail)
        {
            DialogResult result;

            Icon formIcon = Utility.ImageToIcon(SciImage.SciResources.SciResources.GetImageResource("Icons.MenuEditPasteIcon.png").Reference);
            string formTitle = SciImage.SciResources.SciResources.GetString("ExpandCanvasQuestion.Title");

            RenderArgs taskImageRA = new RenderArgs(thumbnail);
            Image taskImage = taskImageRA.Bitmap;
            string introText = SciImage.SciResources.SciResources.GetString("ExpandCanvasQuestion.IntroText");

            TaskButton yesTB = new TaskButton(
                SciImage.SciResources.SciResources.GetImageResource("Icons.ExpandCanvasQuestion.YesTB.Image.png").Reference,
                SciImage.SciResources.SciResources.GetString("ExpandCanvasQuestion.YesTB.ActionText"),
                SciImage.SciResources.SciResources.GetString("ExpandCanvasQuestion.YesTB.ExplanationText"));

            TaskButton noTB = new TaskButton(
                SciImage.SciResources.SciResources.GetImageResource("Icons.ExpandCanvasQuestion.NoTB.Image.png").Reference,
                SciImage.SciResources.SciResources.GetString("ExpandCanvasQuestion.NoTB.ActionText"),
                SciImage.SciResources.SciResources.GetString("ExpandCanvasQuestion.NoTB.ExplanationText"));

            TaskButton cancelTB = new TaskButton(
                TaskButton.Cancel.Image,
                SciImage.SciResources.SciResources.GetString("ExpandCanvasQuestion.CancelTB.ActionText"),
                SciImage.SciResources.SciResources.GetString("ExpandCanvasQuestion.CancelTB.ExplanationText"));

            int width96dpi = (TaskDialog.DefaultPixelWidth96Dpi * 3) / 2;

            TaskButton clickedTB = TaskDialog.Show(
                owner,
                formIcon,
                formTitle,
                taskImage,
                false, 
                introText,
                new TaskButton[] { yesTB, noTB, cancelTB },
                yesTB,
                cancelTB,
                width96dpi);

            if (clickedTB == yesTB)
            {
                result = DialogResult.Yes;
            }
            else if (clickedTB == noTB)
            {
                result = DialogResult.No;
            }
            else
            {
                result = DialogResult.Cancel;
            }

            taskImageRA.Dispose();
            taskImageRA = null;

            return result;
        }

        public PasteAction()
        {
            Tracing.LogFeature("PasteAction");
            
        }
    }
}
