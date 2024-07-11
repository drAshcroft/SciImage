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
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Threading;
using SciImage.Layers;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SciImage
{
    [Serializable]
    public class IntensityLayer
        : Layer,
          IDeserializationCallback
    {

        public override Surface RenderThumbnail(int maxEdgeLength)
        {
            Size thumbSize = Utility.ComputeThumbnailSize(this.Size, maxEdgeLength);
            Surface thumb = new Surface(thumbSize);

            thumb.SuperSamplingFitSurface(this.Intensity_Surface );

            Surface thumb2 = new Surface(thumbSize);
            thumb2.ClearWithCheckboardPattern();
            UserBlendOps.NormalBlendOp nbop = new UserBlendOps.NormalBlendOp();
            nbop.Apply(thumb2, thumb);

            thumb.Dispose();
            thumb = null;

            return thumb2;
        }

        public override bool CopyAction(PdnRegion selectionRegion, PdnGraphicsPath selectionOutline)
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
                Surface copySurface = new Surface(selectionBounds.Width, selectionBounds.Height);
                Bitmap copyBitmap = copySurface.CreateAliasedBitmap();
                Bitmap copyOpaqueBitmap = new Bitmap(copySurface.Width, copySurface.Height, PixelFormat.Format32bppArgb);

                using (Graphics copyBitmapGraphics = Graphics.FromImage(copyBitmap))
                {
                    copyBitmapGraphics.Clear(Color.White);
                }

                maskedSurface.Draw(copySurface, -selectionBounds.X, -selectionBounds.Y);

                using (Graphics copyOpaqueBitmapGraphics = Graphics.FromImage(copyOpaqueBitmap))
                {
                    copyOpaqueBitmapGraphics.Clear(Color.White);
                    copyOpaqueBitmapGraphics.DrawImage(copyBitmap, 0, 0);
                }

                DataObject dataObject = new DataObject();

                dataObject.SetData(DataFormats.Bitmap, copyOpaqueBitmap);
                //dataObject.SetData(copySurface);
                dataObject.SetData(surfaceForClipboard);

                int retryCount = 2;

                while (retryCount >= 0)
                {
                    try
                    {
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
        public override bool PasteAction()
        {
            bool Success = true;

            return Success;
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
                        
                        if (Intensity_Surface != null)
                        {
                            Intensity_Surface.Dispose();
                            Intensity_Surface = null;
                        }
                    }
                }

                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        public long MaxContrast
        {
            get {return (long)GetPropertyValue("maxcontrast") ;}
            set {

                DoPropertyChange("MaxContrast", value);
                
            
            }

        }
        private void DoPropertyChange(string PropertyName, object PropertyValue)
        {
            object  oldValue = GetPropertyValue(PropertyName );

            if (oldValue != PropertyValue)
            {
                OnPropertyChanging(PropertyName );
                SetPropertyValue(PropertyName ,PropertyValue);
                OnPropertyChanged(PropertyName );
                Invalidate();
            }    
        }
        public long MinContrast
        {
            get { return (long)GetPropertyValue("mincontrast"); }
            set
            {
                DoPropertyChange("MinContrast", value);
            }
        }

        /// <summary>
        /// Creates a blayer from an Image
        /// </summary>
        /// <param name="image">The Image to make a copy of that will be the first blayer ("Background") in the document.</param>
        public static IntensityLayer FromImage(int width, int height, int RawStride, int RawBPP, int RawlSize, IntPtr RawData, long MaxContrast, long MinContrast)
        {
            Int32 MaxContrasti = -1;
            Int32 MinContrasti = Int16.MaxValue;
            Surface surf = new Surface(width, height);
            surf.DataType = "Intensity";
            // Copy pixels
            if (RawBPP  == 32)
            {
                unsafe
                {
                    try
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            uint* srcPtr = (uint*)((byte*)RawData + (y * RawStride));
                            ColorPixelBase* dstPtr = surf.GetRowAddress(y);

                            for (int x = 0; x < width; ++x)
                            {
                                byte* inPixel = (byte*)srcPtr;

                                Int32 PixelIntensity = (Int32)ColorPixelBase.GetIntensity16Bit(inPixel[0], inPixel[1], inPixel[2]);
                                if (PixelIntensity > MaxContrasti) MaxContrasti = PixelIntensity;
                                if (PixelIntensity < MinContrasti) MinContrasti = PixelIntensity;
                                *((Int32*)dstPtr) = PixelIntensity;

                                ++srcPtr;
                                ++dstPtr;
                            }
                        }
                    }
                    catch { }
                   
                }
            }
            else if (RawBPP  == 24)
            {
                unsafe
                {
                    try
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            byte* srcPtr = (byte*)RawData  + (y * RawStride);
                            ColorPixelBase* dstPtr = surf.GetRowAddress(y);

                            for (int x = 0; x < width; ++x)
                            {
                                byte b = *srcPtr;
                                byte g = *(srcPtr + 1);
                                byte r = *(srcPtr + 2);

                                Int32 PixelIntensity = (Int32)ColorPixelBase.GetIntensity16Bit(b, g, r);
                                if (PixelIntensity > MaxContrasti) MaxContrasti = PixelIntensity;
                                if (PixelIntensity < MinContrasti) MinContrasti = PixelIntensity;
                                *((Int32*)dstPtr) = PixelIntensity;

                                srcPtr += 3;
                                ++dstPtr;
                            }
                        }
                    }

                    finally
                    {
                     
                    }
                }
            }
            else if (RawBPP == 16)
            {
                unsafe
                {
                    
                    try
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            Int16* srcPtr = (Int16*)( (byte*)RawData  + (y * RawStride) );
                            ColorPixelBase* dstPtr = surf.GetRowAddress(y);

                            for (int x = 0; x < width; ++x)
                            {
                                Int32 PixelIntensity = (Int32) (*srcPtr);
                                if (PixelIntensity > MaxContrasti) MaxContrasti = PixelIntensity;
                                if (PixelIntensity < MinContrasti) MinContrasti = PixelIntensity;
                                *((Int32*)dstPtr) = PixelIntensity;

                                ++srcPtr;
                                ++dstPtr;
                            }
                        }
                    }

                    finally
                    {
                       
                    }
                }
            }
            else if (RawBPP  == 8)
            {
                unsafe
                {
                   
                    try
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            byte* srcPtr = (byte*)RawData  + (y * RawStride);
                            ColorPixelBase* dstPtr = surf.GetRowAddress(y);

                            for (int x = 0; x < width; ++x)
                            {

                                Int32 PixelIntensity = (Int32)(*srcPtr);
                                if (PixelIntensity > MaxContrasti) MaxContrasti = PixelIntensity;
                                if (PixelIntensity < MinContrasti) MinContrasti = PixelIntensity;
                                *((Int32*)dstPtr) = PixelIntensity;
                                
                                ++srcPtr;
                                ++dstPtr;
                            }
                        }
                    }

                    finally
                    {
                       
                    }
                }
            }


            IntensityLayer il = new IntensityLayer(surf,true );
            il.MaxContrast = MaxContrasti;
            il.MinContrast = MinContrasti;

            return il;

        }

        /// <summary>
        /// Creates a blayer from an Image
        /// </summary>
        /// <param name="image">The Image to make a copy of that will be the first blayer ("Background") in the document.</param>
        public static IntensityLayer FromImage(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            Int32 MaxContrasti = -1;
            Int32 MinContrasti = Int16.MaxValue;
            
            Surface surf = new Surface(image.Width, image.Height);
            surf.DataType = "Intensity";
            Bitmap asBitmap = image as Bitmap;

            // Copy pixels
            if (asBitmap != null && asBitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                unsafe
                {
                    BitmapData bData = asBitmap.LockBits(new Rectangle(0, 0, asBitmap.Width, asBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    try
                    {
                        for (int y = 0; y < bData.Height; ++y)
                        {
                            uint* srcPtr = (uint*)((byte*)bData.Scan0.ToPointer() + (y * bData.Stride));
                            ColorPixelBase* dstPtr = surf.GetRowAddress(y);

                            for (int x = 0; x < bData.Width; ++x)
                            {
                                byte* inPixel= (byte*)srcPtr;
                                Int32 PixelIntensity = (Int32)(Int32)ColorPixelBase.GetIntensity16Bit(inPixel[0], inPixel[1], inPixel[2]);
                                if (PixelIntensity > MaxContrasti) MaxContrasti = PixelIntensity;
                                if (PixelIntensity < MinContrasti) MinContrasti = PixelIntensity;
                                ++srcPtr;
                                ++dstPtr;
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
                            ColorPixelBase* dstPtr = surf.GetRowAddress(y);

                            for (int x = 0; x < bData.Width; ++x)
                            {
                                byte b = *srcPtr;
                                byte g = *(srcPtr + 1);
                                byte r = *(srcPtr + 2);

                                Int32 PixelIntensity = (Int32)(Int32)ColorPixelBase.GetIntensity16Bit(b,g,r );
                                if (PixelIntensity > MaxContrasti) MaxContrasti = PixelIntensity;
                                if (PixelIntensity < MinContrasti) MinContrasti = PixelIntensity;
                               
                                srcPtr += 3;
                                ++dstPtr;
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
            else if (asBitmap != null && asBitmap.PixelFormat == PixelFormat.Format16bppGrayScale)
            {
                unsafe
                {
                    BitmapData bData = asBitmap.LockBits(new Rectangle(0, 0, asBitmap.Width, asBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    try
                    {
                        for (int y = 0; y < bData.Height; ++y)
                        {
                            Int16 * srcPtr = (Int16 *)bData.Scan0.ToPointer() + (y * bData.Stride);
                            ColorPixelBase* dstPtr = surf.GetRowAddress(y);

                            for (int x = 0; x < bData.Width; ++x)
                            {

                                Int32 PixelIntensity = (Int32)(*srcPtr);
                                if (PixelIntensity > MaxContrasti) MaxContrasti = PixelIntensity;
                                if (PixelIntensity < MinContrasti) MinContrasti = PixelIntensity;
                                *((Int32*)dstPtr) = PixelIntensity;
                                
                                ++srcPtr ;
                                ++dstPtr;
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
            else if (asBitmap != null && asBitmap.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                unsafe
                {
                    BitmapData bData = asBitmap.LockBits(new Rectangle(0, 0, asBitmap.Width, asBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    try
                    {
                        for (int y = 0; y < bData.Height; ++y)
                        {
                            byte* srcPtr = (byte *)bData.Scan0.ToPointer() + (y * bData.Stride);
                            ColorPixelBase* dstPtr = surf.GetRowAddress(y);

                            for (int x = 0; x < bData.Width; ++x)
                            {

                                Int32 PixelIntensity = (Int32)(*srcPtr);
                                if (PixelIntensity > MaxContrasti) MaxContrasti = PixelIntensity;
                                if (PixelIntensity < MinContrasti) MinContrasti = PixelIntensity;
                                *((Int32*)dstPtr) = PixelIntensity;
                                
                                ++srcPtr;
                                ++dstPtr;
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
                using (RenderArgs args = new RenderArgs(surf))
                {
                    args.Graphics.CompositingMode = CompositingMode.SourceCopy;
                    args.Graphics.SmoothingMode = SmoothingMode.None;
                    args.Graphics.DrawImage(image, args.Bounds, args.Bounds, GraphicsUnit.Pixel);
                }
            }



            IntensityLayer il = new IntensityLayer(surf,true );
            il.MaxContrast = MaxContrasti;
            il.MinContrast = MinContrasti;

            return il;
        }


        [NonSerialized]
        private BinaryPixelOp compiledBlendOp = null;

        private void CompileBlendOp()
        {
            UserBlendOp ubo=(UserBlendOp ) GetPropertyValue("blendOp");
            bool isDefaultOp = (ubo.GetType() == UserBlendOps.GetDefaultBlendOp());

            if (this.Opacity == 255)
            {
                this.compiledBlendOp = ubo;
            }
            else
            {
                this.compiledBlendOp = ubo.CreateWithOpacity(this.Opacity);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            compiledBlendOp = null;
            base.OnPropertyChanged(propertyName);
        }

        /*[Serializable]
        public sealed class IntensityLayerProperties
            : ICloneable,
              ISerializable
        {
            public UserBlendOp blendOp;
            public int opacity; // this is ONLY used when loading older version PDN files! should normally equal -1

            private const string blendOpTag = "blendOp";
            private const string opacityTag = "opacity";

            public static string BlendOpName
            {
                get
                {
                    return PdnResources.GetString("BitmapLayer.Properties.BlendOp.Name");
                }
            }

            public IntensityLayerProperties(UserBlendOp blendOp)
            {
                this.blendOp = blendOp;
                this.opacity = -1;
            }

            public IntensityLayerProperties(IntensityLayerProperties cloneMe)
            {
                this.blendOp = cloneMe.blendOp;
                this.opacity = -1;
            }

            public object Clone()
            {
                return new IntensityLayerProperties(this);
            }

            public IntensityLayerProperties(SerializationInfo info, StreamingContext context)
            {
                this.blendOp = (UserBlendOp)info.GetValue(blendOpTag, typeof(UserBlendOp));

                // search for 'opacity' and load it if it exists
                this.opacity = -1;

                foreach (SerializationEntry entry in info)
                {
                    if (entry.Name == opacityTag)
                    {
                        this.opacity = (int)((byte)entry.Value);
                        break;
                    }
                }
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(blendOpTag, this.blendOp);
            }
        }
        */
        //private IntensityLayerProperties properties;
        private Surface Intensity_Surface;
        

        
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
            UserBlendOp blendOp =(UserBlendOp ) GetPropertyValue("blendOp");
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
            if (blendOp.GetType() != oldValue.GetType() )
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

                return Intensity_Surface;
            }
        }

        public UserBlendOp BlendOp
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IntensityLayer");
                }

                return (UserBlendOp ) GetPropertyValue("blendOp");
            }
        }

        public IntensityLayer(int width, int height)
            : this(width, height, ColorPixelBase.FromBgra(255, 255, 255, 0))
        {
        }

        public IntensityLayer(int width, int height, ColorPixelBase fillColor)
            : base(width, height)
        {
            this.Intensity_Surface  = new Surface(width, height);
            // clear to see-through white, 0x00ffffff
            uint I = fillColor.GetIntensity16Bit();
            this.Surface.Clear( ColorPixelBase.FromUInt32(I) );
            SetPropertyValue("blendOp", UserBlendOps.CreateDefaultBlendOp());
            SetPropertyValue("MinContrast", -1);
            SetPropertyValue("MaxContrast", -1);
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


        private unsafe Surface ConvertSurfaceToIntensity(Surface s)
        {
            if (s.DataType == "Intensity")
            {

                return s;
            }
            else
            {
                Surface nSurface = new Surface(new Size(s.Width, s.Height));
                nSurface.DataType = "Intensity";
                for (int y = 0; y < s.Height; ++y)
                {
                    ColorPixelBase* srcPtr = s.GetRowAddressUnchecked(y);
                    ColorPixelBase* dstPtr = nSurface.GetRowAddressUnchecked(y);

                    for (int x = 0; x < s.Width; ++x)
                    {
                        dstPtr->Bgra =(srcPtr[x].GetIntensity16Bit());
                        ++dstPtr;
                    }
                }
                return nSurface;
                
            }
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
            //todo: need to convert a normal surface to an intensity surface at this
            //point
            if (takeOwnership)
            {
               this.Intensity_Surface = ConvertSurfaceToIntensity( surface );
            }
            else
            {
               this.Intensity_Surface = ConvertSurfaceToIntensity( surface.Clone() );
            }

            SetPropertyValue("blendOp", UserBlendOps.CreateDefaultBlendOp());
            SetPropertyValue("MinContrast", -1);
            SetPropertyValue("MaxContrast", -1);
        }
        public void ReplaceSurface(Surface surface)
        {
            this.Intensity_Surface  = ConvertSurfaceToIntensity(  surface );

        }
        protected IntensityLayer(IntensityLayer copyMe)
            : base(copyMe)
        {
            
            SetPropertyValue("blendOp", UserBlendOps.CreateDefaultBlendOp());
            
        }

        int rendercount = 0;
        protected unsafe override void RenderImpl(RenderArgs args, Rectangle roi)
        {
            System.Diagnostics.Debug.Print("Rendering" + rendercount.ToString() );
            ++rendercount;
            if (disposed)
            {
                throw new ObjectDisposedException("IntesityLayer");
            }

            if (Opacity == 0)
            {
                return;
            }

            if (compiledBlendOp == null)
            {
                CompileBlendOp();
            }

            byte[] Convert = new byte[roi.Width * 4];
            fixed (byte* bArptr = Convert)
            {
                for (int y = roi.Top; y < roi.Bottom; ++y)
                {
                    ColorPixelBase* dstPtr = args.Surface.GetPointAddressUnchecked(roi.Left, y);
                    ColorPixelBase* srcPtr = this.Intensity_Surface.GetPointAddressUnchecked(roi.Left, y);
                    byte* L = (byte*)bArptr;
                    for (int x = 0; x < roi.Width; x++)
                    {
                        Int32 v1 = *((Int32*)srcPtr);

                        double v = (double)v1;

                        v = (v - MinContrast) / (MaxContrast - MinContrast);
                        if (v > 1)
                            v = 1;
                        if (v < 0)
                            v = 0;
                        byte Intensity = (byte)(v * 255);
                        if (Intensity > 255)
                            System.Diagnostics.Debug.Print("");
                        *L = Intensity;
                        *(L + 1) = Intensity;
                        *(L + 2) = Intensity;
                        *(L + 3) = 255;
                        // ColorPixelBase cb = *((ColorPixelBase*)L);

                        L += 4;
                        ++srcPtr;
                    }


                    this.compiledBlendOp.Apply(dstPtr, (ColorPixelBase*)bArptr, roi.Width);

                    //this.compiledBlendOp.Apply(dstPtr , srcPtr, roi.Width);
                }
            }
        }

        protected unsafe override void RenderImpl(RenderArgs args, Rectangle[] rois, int startIndex, int length)
        {
            System.Diagnostics.Debug.Print("Rendering" + rendercount.ToString());
            ++rendercount;
            if (disposed)
            {
                throw new ObjectDisposedException("IntensityLayer");
            }

            if (Opacity == 0)
            {
                return;
            }

            if (compiledBlendOp == null)
            {
                CompileBlendOp();
            }

            for (int i = startIndex; i < startIndex + length; ++i)
            {

                Rectangle roi = rois[i];
                RenderImpl(args, roi);
            }
        }

        public override PdnBaseForm CreateConfigDialog()
        {
            IntensityPropertiesDialog2 blpd = new IntensityPropertiesDialog2();
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
