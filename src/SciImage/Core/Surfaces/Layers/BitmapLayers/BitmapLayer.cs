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
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UserBlendOps;
using SciImage.Forms;
using SciImage.Plugins.Actions;

namespace SciImage.Core.Surfaces.Layers.BitmapLayers
{
    [Serializable]
    public class BitmapLayer
        : Layer,
          IDeserializationCallback
    {
        public override Surface RenderThumbnail(int maxEdgeLength)
        {
            Size thumbSize = Utility.ComputeThumbnailSize(this.Size, maxEdgeLength);
            Surface thumb = new Surface(thumbSize,PixelFormat.Format32bppArgb );

            thumb.SuperSamplingFitSurface(this.surface);

            Surface thumb2 = new Surface(thumbSize, PixelFormat.Format32bppArgb);
            thumb2.ClearWithCheckboardPattern();
            NormalBlendOp nbop = new NormalBlendOp();
            nbop.Apply(thumb2, thumb);

            thumb.Dispose();
            thumb = null;

            return thumb2;
        }

        private bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                try
                {
                    if (disposing)
                    {
                        if (surface != null)
                        {
                            surface.Dispose();
                            surface = null;
                        }
                    }
                }

                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        [NonSerialized]
        private BinaryPixelOp compiledBlendOp = null;

        private void CompileBlendOp()
        {
            UserBlendOp ubo = (UserBlendOp)GetPropertyValue("blendOp");
            bool isDefaultOp = (ubo.GetType() == UserBlendOpsMaster.GetDefaultBlendOp());

            if (this.Opacity == 255)
            {
                this.compiledBlendOp = ubo;
            }
            else
            {
                this.compiledBlendOp = ubo.CreateWithOpacity(this.Opacity);
            }
        }


        public override bool CopyAction(SciRegion selectionRegion,  PdnGraphicsPath selectionOutline )
        {
            bool Success = true;
            Layer activeLayer = this;
            //BitmapLayer activeLayer = (BitmapLayer)documentWorkspace.ActiveLayer;
            RenderArgs renderArgs = new RenderArgs(activeLayer.Surface);
            MaskedSurface maskedSurface = new MaskedSurface(renderArgs.Surface, selectionOutline);
            SurfaceForClipboard surfaceForClipboard = new SurfaceForClipboard(maskedSurface);
            Rectangle selectionBounds = Utility.GetRegionBounds(selectionRegion);

            if (selectionBounds.Width > 0 && selectionBounds.Height > 0)
            {
                Surface copySurface = new Surface(selectionBounds.Width, selectionBounds.Height,PixelFormat.Format32bppArgb );
                
                Bitmap copyOpaqueBitmap = new Bitmap(copySurface.Width, copySurface.Height, PixelFormat.Format32bppArgb);

                Bitmap copyBitmap = copySurface.CreateAliasedBitmap();
                using (Graphics copyBitmapGraphics = Graphics.FromImage(copyBitmap))
                {
                    copyBitmapGraphics.Clear(Color.Transparent);
                }

                maskedSurface.Draw(copySurface, -selectionBounds.X, -selectionBounds.Y);
                
                

                using (Graphics copyOpaqueBitmapGraphics = Graphics.FromImage(copyOpaqueBitmap))
                {
                    copyOpaqueBitmapGraphics.Clear(Color.White);
                    copyOpaqueBitmapGraphics.DrawImage(copyBitmap, 0, 0);
                }

                DataObject dataObject = new DataObject();

                dataObject.SetData(DataFormats.Bitmap, copyOpaqueBitmap);
                dataObject.SetData(copySurface);
                dataObject.SetData(surfaceForClipboard);

                int retryCount = 2;

                while (retryCount >= 0)
                {
                    try
                    {
                        Clipboard.SetImage(copyBitmap  ); 
                        Clipboard.SetDataObject(dataObject, true);
                        break;
                    }
                    catch
                    {
                        if (retryCount == 0)
                        {
                            Success = false;
                        }
                        else
                        {
                            Thread.Sleep(200);
                        }
                    }

                    finally
                    {
                        --retryCount;
                    }
                }

                copySurface.Dispose();
                copyBitmap.Dispose();
                copyOpaqueBitmap.Dispose();
                renderArgs.Dispose();
                maskedSurface.Dispose();
            }
            return Success;
        }

        public override  bool PasteAction()
        {
            bool Success = true;

            return Success;
        }
        protected override void OnPropertyChanged(string propertyName)
        {
            compiledBlendOp = null;
            base.OnPropertyChanged(propertyName);
        }

     
        private Surface surface;

        public override void LoadProperties(object oldState, bool suppressEvents)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("IntensityLayer");
            }

            Dictionary<string, object> lp = (Dictionary<string, object>)oldState;

            // Have the base class load its properties
            base.LoadProperties(lp, suppressEvents);

            // Now load our properties, and announce them to the world
            bool raiseBlendOp = false;
            UserBlendOp blendOp = (UserBlendOp)GetPropertyValue("blendOp");
            if (blendOp.GetType() != blendOp.GetType())
            {
                if (!suppressEvents)
                {
                    raiseBlendOp = true;
                    OnPropertyChanging("BlendOp");
                }
            }


            this.compiledBlendOp = null;

            Invalidate();

            if (raiseBlendOp)
            {
                OnPropertyChanged("BlendOp");
            }
        }
        public void SetBlendOp(UserBlendOp blendOp)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("IntensityLayer");
            }
            UserBlendOp oldValue = (UserBlendOp)GetPropertyValue("blendOp");
            if (blendOp.GetType() != oldValue.GetType())
            {
                OnPropertyChanging("BlendOp");
                SetPropertyValue("blendOp", blendOp);
                compiledBlendOp = null;
                Invalidate();
                OnPropertyChanged("BlendOp");
            }
        }

        public override object Clone()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("BitmapLayer");
            }

            return (object)new BitmapLayer(this);
        }

        public override  Surface Surface
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("BitmapLayer");
                }

                return surface;
            }
        }

        /// <summary>
        /// Creates a blayer from an Image
        /// </summary>
        /// <param name="image">The Image to make a copy of that will be the first blayer ("Background") in the document.</param>
        public static Layer FromImage(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            //if ( image.PixelFormat == PixelFormat.Format16bppGrayScale)
            //    return IntensityLayer.FromImage(image);

            Surface s = new Surface(image);

            BitmapLayer layer = new BitmapLayer(s);

            return layer;
        }
        /// <summary>
        /// Creates a blayer from an Image
        /// </summary>
        /// <param name="image">The Image to make a copy of that will be the first blayer ("Background") in the document.</param>
        public static Layer FromImage(Surface image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            //if ( image.PixelFormat == PixelFormat.Format16bppGrayScale)
            //    return IntensityLayer.FromImage(image);

           

            BitmapLayer layer = new BitmapLayer(image);

            return layer;
        }

        public UserBlendOp BlendOp
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IntensityLayer");
                }

                return (UserBlendOp)GetPropertyValue("blendOp");
            }
        }

        
        public BitmapLayer(int width, int height, ColorPixelBase fillColor)
            : base(width, height)
        {
            this.surface = new Surface(width, height, fillColor);
            // clear to see-through white, 0x00ffffff
            this.Surface.Clear(fillColor);
            compiledBlendOp = UserBlendOpsMaster.CreateDefaultBlendOp();
            SetPropertyValue("blendOp", UserBlendOpsMaster.CreateDefaultBlendOp());
        }

        /// <summary>
        /// Creates a new BitmapLayer of the same size as the given Surface, and copies the 
        /// pixels from the given Surface.
        /// </summary>
        /// <param name="RGB32_Surface">The Surface to copy pixels from.</param>
        public BitmapLayer(Surface surface)
            : this(surface, false)
        {
        }

        /// <summary>
        /// Creates a new BitmapLayer of the same size as the given Surface, and either
        /// copies the pixels of the given Surface or takes ownership of it.
        /// </summary>
        /// <param name="RGB32_Surface">The Surface.</param>
        /// <param name="takeOwnership">
        /// true to take ownership of the RGB32_Surface (make sure to Dispose() it yourself), or
        /// false to copy its pixels
        /// </param>
        public BitmapLayer(Surface surface, bool takeOwnership)
            : base(surface.Width, surface.Height)
        {
            if (takeOwnership)
            {
                this.surface = surface;
            }
            else
            {
                this.surface = surface.Clone();
            }
            compiledBlendOp = UserBlendOpsMaster.CreateDefaultBlendOp();
            SetPropertyValue("blendOp", UserBlendOpsMaster.CreateDefaultBlendOp());
        }
        public void ReplaceSurface(Surface surface)
        {
            this.surface = surface;

        }
        protected BitmapLayer(BitmapLayer copyMe)
            : base(copyMe)
        {
            this.surface = copyMe.Surface.Clone();
            compiledBlendOp = UserBlendOpsMaster.CreateDefaultBlendOp();
            SetPropertyValue("blendOp", UserBlendOpsMaster.CreateDefaultBlendOp());
        }

        protected unsafe override void RenderImpl(RenderArgs args, Rectangle roi)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("BitmapLayer");
            }

            if (Opacity == 0)
            {
                return;
            }

            if (compiledBlendOp == null)
            {
                CompileBlendOp();
            }

            for (int y = roi.Top; y < roi.Bottom; ++y)
            {
                for (int x = roi.Left; x < roi.Right; ++x)
                {
                    ColorPixelBase dstPtr = args.Surface.GetPoint(x, y);
                    ColorPixelBase srcPtr = this.surface.GetPoint(x, y,dstPtr);

                    args.Surface[x,y]= (this.compiledBlendOp.Apply(dstPtr, srcPtr)).ToInt32();
                }
            }
        }

        protected unsafe override void RenderImpl(RenderArgs args, Rectangle[] rois, int startIndex, int length)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("BitmapLayer");
            }

            if (Opacity == 0)
            {
                return;
            }

            if (compiledBlendOp == null)
            {
                CompileBlendOp();
            }

            if (args.Surface.ColorPixelBase.GetType() == typeof(ColorBgra))
            {
                ColorBgra clr=new ColorBgra();
                ColorBgra clr2=new ColorBgra();
                for (int i = startIndex; i < startIndex + length; ++i)
                {
                    Rectangle roi = rois[i];

                    for (int y = roi.Top; y < roi.Bottom; ++y)
                    {
                        for (int x = roi.Left; x < roi.Right; ++x)
                        {
                            clr.Bgra  =(uint) args.Surface[x, y];
                            clr2.Bgra =(uint) this.surface[x, y];

                            args.Surface[x, y] = (this.compiledBlendOp.Apply(clr , clr2)).ToInt32();
                        }
                    }
                }
            }
            else
            {
                for (int i = startIndex; i < startIndex + length; ++i)
                {
                    Rectangle roi = rois[i];

                    for (int y = roi.Top; y < roi.Bottom; ++y)
                    {
                        for (int x = roi.Left; x < roi.Right; ++x)
                        {
                            ColorPixelBase dstPtr = args.Surface.GetPoint(x, y);
                            ColorPixelBase srcPtr = this.surface.GetPoint(x, y, dstPtr);

                            args.Surface[x, y] = (this.compiledBlendOp.Apply(dstPtr, srcPtr)).ToInt32();
                        }
                    }
                }
            }
        }

        public override SciBaseForm CreateConfigDialog()
        {
            BitmapLayerPropertiesDialog2 blpd = new BitmapLayerPropertiesDialog2();
            blpd.Layer = this;
            return blpd;
        }

        public void OnDeserialization(object sender)
        {
            /*if (this.properties.opacity != -1)
            {
                this.PushSuppressPropertyChanged();
                base.Opacity = (byte)this.properties.opacity;
                this.properties.opacity = -1;
                this.PopSuppressPropertyChanged();
            }*/

            this.compiledBlendOp = null;
        }
    }
}
