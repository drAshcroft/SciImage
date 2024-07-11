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
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Forms;
using SciImage.Plugins.Actions;

namespace SciImage.Core.Surfaces.Layers.IntensityLayers
{
    [Serializable]
    public class IntensityLayer
        : Layer,
          IDeserializationCallback
    {
        public long MaxIntensity = ushort.MaxValue;
        public long MinIntensity = 0;
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
                throw new ObjectDisposedException("IntensityLayer");
            }

            return (object)new IntensityLayer(this);
        }

        public override  Surface Surface
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IntensityLayer");
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


            Surface s = IntensityLayer.CreateGrayScaleSurface(image);
            //todo:  need to decide how to convert the surface or to switch to bitmap layer
            IntensityLayer layer = new IntensityLayer(s);

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


           
            //todo:  need to decide how to convert the surface or to switch to bitmap layer
            IntensityLayer layer = new IntensityLayer(image);

            return layer;
        }

        private static  Surface CreateGrayScaleSurface(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            
            Surface DstSurf = new Surface(image.Width, image.Height, new ColorIntensity());
            Bitmap asBitmap = image as Bitmap;

            // Copy pixels
            if (asBitmap != null && asBitmap.PixelFormat == PixelFormat.Format16bppGrayScale  )
            {
                unsafe
                {
                    BitmapData bData = asBitmap.LockBits(new Rectangle(0, 0, asBitmap.Width, asBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppGrayScale );

                    try
                    {
                        for (int y = 0; y < bData.Height; ++y)
                        {
                            UInt16 * srcPtr = (UInt16*)((byte*)bData.Scan0.ToPointer() + (y * bData.Stride));
                            for (int x = 0; x < bData.Width; ++x)
                            {
                                UInt16  i= srcPtr[0];
                                
                                DstSurf[x, y] = ColorIntensity.sFromIntensity( i,0,255) .ToInt32();
                                srcPtr++;
                            }
                        }
                    }

                    finally
                    {
                        asBitmap.UnlockBits(bData);
                        bData = null;
                    }
                }
            }
            else if (asBitmap != null && asBitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                unsafe
                {
                    BitmapData bData = asBitmap.LockBits(new Rectangle(0, 0, asBitmap.Width, asBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    try
                    {
                        for (int y = 0; y < bData.Height; ++y)
                        {
                            byte* srcPtr = ((byte*)bData.Scan0.ToPointer() + (y * bData.Stride));
                            for (int x = 0; x < bData.Width; ++x)
                            {
                                byte r= srcPtr[0];
                                byte g= srcPtr[1];
                                byte b= srcPtr[2];
                                byte a= srcPtr[3];
                                DstSurf[x, y] = ColorIntensity.sFromBgra(b, g, r, a).ToInt32();
                                srcPtr+=4;
                            }
                        }
                    }

                    finally
                    {
                        asBitmap.UnlockBits(bData);
                        bData = null;
                    }
                }
            }
            else if (asBitmap != null && asBitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                unsafe
                {
                    BitmapData bData = asBitmap.LockBits(new Rectangle(0, 0, asBitmap.Width, asBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    try
                    {
                        for (int y = 0; y < bData.Height; ++y)
                        {
                            byte* srcPtr = (byte*)bData.Scan0.ToPointer() + (y * bData.Stride);
                            
                            for (int x = 0; x < bData.Width; ++x)
                            {
                                byte b = *srcPtr;
                                byte g = *(srcPtr + 1);
                                byte r = *(srcPtr + 2);
                                byte a = 255;

                                DstSurf[x, y] = ColorIntensity.sFromBgra(b, g, r, a).ToInt32();

                                srcPtr += 3;
                            }
                        }
                    }

                    finally
                    {
                        asBitmap.UnlockBits(bData);
                        bData = null;
                    }
                }
            }
            else
            {
               
            }
            return DstSurf;

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

        
        public IntensityLayer(int width, int height, ColorPixelBase fillColor)
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
        public IntensityLayer(Surface surface)
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
        public IntensityLayer(Surface surface, bool takeOwnership)
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
        protected IntensityLayer(IntensityLayer copyMe)
            : base(copyMe)
        {
            this.surface = copyMe.Surface.Clone();
            compiledBlendOp = UserBlendOpsMaster.CreateDefaultBlendOp();
            SetPropertyValue("blendOp", UserBlendOpsMaster.CreateDefaultBlendOp());
        }

        protected IntensityLayer(BitmapLayer copyMe)
            : base(copyMe)
        {
            //todo: convert the layer to an intensity layer
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
                double Length = MaxIntensity - MinIntensity;
                if (Length == 0) Length = 1;
                ColorBgra b = new ColorBgra();
                for (int i = startIndex; i < startIndex + length; ++i)
                {
                    Rectangle roi = rois[i];

                    for (int y = roi.Top; y < roi.Bottom; ++y)
                    {
                        for (int x = roi.Left; x < roi.Right; ++x)
                        {
                            ColorPixelBase dstPtr = args.Surface.GetPoint(x, y);
                            ColorIntensity srcPtr = ( ColorIntensity  ) this.surface.GetPoint(x, y);
                            //if (srcPtr.Intensity == 255) 
                              //  srcPtr.Intensity =(ushort) MaxIntensity;
                            long Clr =(long)(254* ((double)(srcPtr.Intensity - MinIntensity)) / Length);
                         //   if (srcPtr.Intensity != 0)
                           //     System.Diagnostics.Debug.Print("");
                            if (Clr < 0) Clr = 0;
                            if (Clr > 254) Clr = 254;
                            b.R =(byte) Clr;
                            b.G =(byte) Clr;
                            b.B =(byte) Clr;
                            b.alpha = srcPtr.alpha;
                            args.Surface[x, y] = (this.compiledBlendOp.Apply(dstPtr, b)).ToInt32();
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
                            // if (x< (roi.Left+roi.Right)/2 )
                            //   srcPtr = ColorBgra.White;
                            //else
                            //    srcPtr = ColorBgra.Black;

                            args.Surface[x, y] = (this.compiledBlendOp.Apply(dstPtr, srcPtr)).ToInt32();
                        }
                    }
                }
            }
        }

        public override SciBaseForm CreateConfigDialog()
        {
            IntensityLayerPropertiesDialog blpd = new IntensityLayerPropertiesDialog();
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
