/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.SystemLayer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps;
using SciImage.SystemLayer.System;

namespace SciImage.Core.Surfaces
{
    /// <summary>
    /// This is our Surface type. We allocate our own blocks of memory for this,
    /// and provide ways to create a GDI+ Bitmap object that aliases our RGB32_Surface.
    /// That way we can do everything fast, in memory and have complete control,
    /// and still have the ability to use GDI+ for drawing and rendering where
    /// appropriate.
    /// </summary>
    [Serializable]
    public class Surface
        : IDisposable,
          ICloneable
    {
        protected MemoryBlock scan0;
        protected int width;
        protected int height;
        protected int stride=4;
        protected bool disposed = false;
        protected PixelFormat _PixelFormat = PixelFormat.Format32bppArgb;
        protected ColorPixelBase _ColorPixelBase;

        #region ImageAttributes
        public int PixelFormatSizeOf 
        {
            get { return 4; }
        }
        
        public ColorPixelBase ColorPixelBase
        {
            get { return _ColorPixelBase; }
        }
       
    

        /// <summary>
        /// Gets a MemoryBlock which is the buffer holding the pixels associated
        /// with this Surface.
        /// </summary>
        public MemoryBlock Scan0
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Surface");
                }

                return this.scan0;
            }
        }
       

        /// <summary>
        /// Gets the width, in pixels, of this Surface.
        /// </summary>
        /// <remarks>
        /// This property will never throw an ObjectDisposedException.
        /// </remarks>
        public int Width
        {
            get
            {
                return this.width;
            }
        }

        /// <summary>
        /// Gets the height, in pixels, of this Surface.
        /// </summary>
        /// <remarks>
        /// This property will never throw an ObjectDisposedException.
        /// </remarks>
        public int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        /// Gets the stride, in bytes, for this Surface.
        /// </summary>
        /// <remarks>
        /// Stride is defined as the number of bytes between the beginning of a row and
        /// the beginning of the next row. Thus, in loose C notation: stride = (byte *)&this[0, 1] - (byte *)&this[0, 0].
        /// Stride will always be equal to <b>or greater than</b> Width * PixelFormatSizeOf.
        /// This property will never throw an ObjectDisposedException.
        /// </remarks>
        public int Stride
        {
            get
            {
                return this.stride;
            }
        }

        /// <summary>
        /// Gets the size, in pixels, of this Surface.
        /// </summary>
        /// <remarks>
        /// This is a convenience function that creates a new Size instance based
        /// on the values of the Width and Height properties.
        /// This property will never throw an ObjectDisposedException.
        /// </remarks>
        public Size Size
        {
            get
            {
                return new Size(this.width, this.height);
            }
        }

        /// <summary>
        /// Gets the GDI+ PixelFormat of this Surface.
        /// </summary>
        /// <remarks>
        /// This property always returns PixelFormat.Format32bppArgb.
        /// This property will never throw an ObjectDisposedException.
        /// </remarks>
        public PixelFormat PixelFormat
        {
            get
            {
                return PixelFormat.Format32bppArgb;
            }
        }

        /// <summary>
        /// Gets the bounds of this Surface, in pixels.
        /// </summary>
        /// <remarks>
        /// This is a convenience function that returns Rectangle(0, 0, Width, Height).
        /// This property will never throw an ObjectDisposedException.
        /// </remarks>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, width, height);
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of the Surface class.
        /// </summary>
        /// <param name="size">The size, in pixels, of the new Surface.</param>
        public Surface(Size size, PixelFormat DefaultFormat)
            : this( size.Width, size.Height , DefaultFormat  )
        {
        }

        public Surface(Size size, ColorPixelBase  DefaultFormat)
            : this(size.Width, size.Height, DefaultFormat)
        {
        }
        protected Surface()
        { }

        /// <summary>
        /// Creates a new instance of the Surface class.
        /// </summary>
        /// <param name="width">The width, in pixels, of the new Surface.</param>
        /// <param name="height">The height, in pixels, of the new Surface.</param>
        public Surface(int width, int height, PixelFormat DefaultFormat )
        {
            if (DefaultFormat == PixelFormat.Format32bppArgb || DefaultFormat == PixelFormat.Format8bppIndexed || DefaultFormat==PixelFormat.Format24bppRgb )
            {
                _ColorPixelBase = new ColorBgra();
                _PixelFormat = PixelFormat.Format32bppArgb;
            }
            else
                _PixelFormat = DefaultFormat;
            int stride;
            long bytes;
            
            try
            {
                stride = checked(width * PixelFormatSizeOf);
                bytes = (long)height * (long)stride;
            }

            catch (OverflowException ex)
            {
                throw new OutOfMemoryException("Dimensions are too large - not enough memory, width=" + width.ToString() + ", height=" + height.ToString(), ex);
            }

            MemoryBlock scan0 = new MemoryBlock(width, height);
            Create(width, height, stride, scan0);
        }

        /// <summary>
        /// Creates a new instance of the Surface class.
        /// </summary>
        /// <param name="width">The width, in pixels, of the new Surface.</param>
        /// <param name="height">The height, in pixels, of the new Surface.</param>
        public Surface(int width, int height, ColorPixelBase DefaultFormat)
        {
            _ColorPixelBase = DefaultFormat;
            int stride;
            long bytes;
            _PixelFormat = DefaultFormat.pixelFormat ;
            try
            {
                stride = checked(width * PixelFormatSizeOf);
                bytes = (long)height * (long)stride;
            }

            catch (OverflowException ex)
            {
                throw new OutOfMemoryException("Dimensions are too large - not enough memory, width=" + width.ToString() + ", height=" + height.ToString(), ex);
            }

            MemoryBlock scan0 = new MemoryBlock(width, height);
            Create(width, height, stride, scan0);
        }

        /// <summary>
        /// Creates a RGB32_Surface from an Image
        /// </summary>
        /// <param name="image">The RGB32_Surface will contain this image</param>
        public Surface(Image image):this(image.Width,image.Height,image.PixelFormat  )
        {
            
            ReplaceImage(image);
            
        }

        public  void ReplaceImage(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            //throw new Exception("Not Implimented yet");
          /*  Bitmap b2;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                b2 = new Bitmap(image.Size.Width, image.Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(b2);
                g.DrawImage(image, new Point(0, 0));
                g.Dispose();
            }
            else
                b2 =(Bitmap) image;*/
            
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
                            Int32 * dstPtr = GetRowAddress(y);

                            for (int x = 0; x < bData.Width; ++x)
                            {
                                *dstPtr = (int)*srcPtr;
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
                            Int32 * dstPtr = GetRowAddress(y);

                            for (int x = 0; x < bData.Width; ++x)
                            {
                                byte b = *srcPtr;
                                byte g = *(srcPtr + 1);
                                byte r = *(srcPtr + 2);
                                byte a = 255;

                                *dstPtr = _ColorPixelBase.FromBgra(b, g, r, a).ToInt32();

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
            else if (asBitmap != null && asBitmap.PixelFormat == PixelFormat.Format16bppGrayScale )
            {
                unsafe
                {
                    BitmapData bData = asBitmap.LockBits(new Rectangle(0, 0, asBitmap.Width, asBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppGrayScale);

                    try
                    {
                        for (int y = 0; y < bData.Height; ++y)
                        {
                            UInt16* srcPtr = (UInt16*)((byte*)bData.Scan0.ToPointer() + (y * bData.Stride));
                            Int32* dstPtr = GetRowAddress(y);
                            for (int x = 0; x < bData.Width; ++x)
                            {
                                UInt16 i = srcPtr[0];
                                byte* clr = (byte*)dstPtr;
                                *(( ushort *) clr) = i;  //ColorIntensity.sFromIntensity(i, 0, 255).ToInt32();
                                *(clr + 3) = 255;
                                srcPtr++;
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
                using (RenderArgs args = new RenderArgs(this))
                {
                    args.Graphics.CompositingMode = CompositingMode.SourceCopy;
                    args.Graphics.SmoothingMode = SmoothingMode.None;
                    args.Graphics.DrawImage(image, args.Bounds, args.Bounds, GraphicsUnit.Pixel);
                }
            }


        }

        /// <summary>
        /// Creates a new instance of the Surface class that reuses a block of memory that was previously allocated.
        /// </summary>
        /// <param name="width">The width, in pixels, for the Surface.</param>
        /// <param name="height">The height, in pixels, for the Surface.</param>
        /// <param name="stride">The stride, in bytes, for the Surface.</param>
        /// <param name="scan0">The MemoryBlock to use. The beginning of this buffer defines the upper left (0, 0) pixel of the Surface.</param>
        private Surface(int width, int height, int stride, MemoryBlock scan0, PixelFormat DefaultFormat)
        {
            _PixelFormat = DefaultFormat;
            if (DefaultFormat == PixelFormat.Format32bppArgb)
            {
                _ColorPixelBase = new ColorBgra();
            }
           
            Create(width, height, stride, scan0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "width")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "height")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "stride")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "scan0")]
        private void Create(int width, int height, int stride, MemoryBlock scan0)
        {
            this.width = width;
            this.height = height;
            this.stride = stride;
            this.scan0 = scan0;
        }

        ~Surface()
        {
            Dispose(false);
        }
        #endregion
        
        #region CreateWindow
        /// <summary>
        /// Creates a Surface that aliases a portion of this Surface.
        /// </summary>
        /// <param name="bounds">The portion of this Surface that will be aliased.</param>
        /// <remarks>The upper left corner of the new Surface will correspond to the 
        /// upper left corner of this rectangle in the original Surface.</remarks>
        /// <returns>A Surface that aliases the requested portion of this Surface.</returns>
        public Surface CreateWindow(Rectangle bounds)
        {

            return CreateWindow(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        public Surface CreateWindow(int x, int y, int windowWidth, int windowHeight)
        {
           
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }

            if (windowHeight == 0)
            {
                throw new ArgumentOutOfRangeException("windowHeight", "must be greater than zero");
            }

            Rectangle original = this.Bounds;
            Rectangle sub = new Rectangle(x, y, windowWidth, windowHeight);
            Rectangle clipped = Rectangle.Intersect(original, sub);

            if (clipped != sub)
            {
                throw new ArgumentOutOfRangeException("bounds", new Rectangle(x, y, windowWidth, windowHeight),
                    "bounds parameters must be a subset of this Surface's bounds");
            }

            long offset = ((long)stride * (long)y) + ((long)PixelFormatSizeOf  * (long)x);
            long length = ((windowHeight - 1) * (long)stride) + (long)windowWidth * (long)PixelFormatSizeOf;
            MemoryBlock block = new MemoryBlock(this.scan0, offset, length);
            Surface s=new Surface(windowWidth, windowHeight, this.stride, block, _PixelFormat );
            
            return s;
        }

        #endregion
        
        #region DirectMemoryManipulation

        /// <summary>
        /// Gets the offset, in bytes, of the requested row from the start of the RGB32_Surface.
        /// </summary>
        /// <param name="y">The row.</param>
        /// <returns>The number of bytes between (0,0) and (0,y).</returns>
        public long GetRowByteOffset(int y)
        {
            if (y < 0 || y >= height)
            {
                throw new ArgumentOutOfRangeException("y", "Out of bounds: y=" + y.ToString());
            }
           
            return (long)y * (long)stride;
        }

        /// <summary>
        /// Gets the offset, in bytes, of the requested row from the start of the RGB32_Surface.
        /// </summary>
        /// <param name="y">The row.</param>
        /// <returns>The number of bytes between (0,0) and (0,y)</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetRowByteOffset().
        /// </remarks>
        public unsafe long GetRowByteOffsetUnchecked(int y)
        {
#if DEBUG
            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return (long)y * (long)stride;
        }

        /// <summary>
        /// Gets a pointer to the beginning of the requested row in the RGB32_Surface.
        /// </summary>
        /// <param name="y">The row</param>
        /// <returns>A pointer that references (0,y) in this RGB32_Surface.</returns>
        /// <remarks>Since this returns a pointer, it is potentially unsafe to use.</remarks>
        protected unsafe Int32*  GetRowAddress(int y)
        {
            return (Int32*)(((byte*)scan0.VoidStar) + GetRowByteOffset(y));
        }

        /// <summary>
        /// Gets a pointer to the beginning of the requested row in the RGB32_Surface.
        /// </summary>
        /// <param name="y">The row</param>
        /// <returns>A pointer that references (0,y) in this RGB32_Surface.</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetRowAddress().
        /// </remarks>
        protected unsafe Int32* GetRowAddressUnchecked(int y)
        {
#if DEBUG
            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return (Int32 *)(((byte*)scan0.VoidStar) + GetRowByteOffsetUnchecked(y));
        }

        /// <summary>
        /// Gets the number of bytes from the beginning of a row to the requested column.
        /// </summary>
        /// <param name="x">The column.</param>
        /// <returns>
        /// The number of bytes between (0,n) and (x,n) where n is in the range [0, Height).
        /// </returns>
        public long GetColumnByteOffset(int x)
        {
            if (x < 0 || x >= this.width)
            {
                throw new ArgumentOutOfRangeException("x", x, "Out of bounds");
            }

            return (long)x * (long)this.PixelFormatSizeOf ;
        }

        /// <summary>
        /// Gets the number of bytes from the beginning of a row to the requested column.
        /// </summary>
        /// <param name="x">The column.</param>
        /// <returns>
        /// The number of bytes between (0,n) and (x,n) where n is in the range [0, Height).
        /// </returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetColumnByteOffset().
        /// </remarks>
        public long GetColumnByteOffsetUnchecked(int x)
        {
#if DEBUG
            if (x < 0 || x >= this.width)
            {
                Tracing.Ping("x=" + x.ToString() + " is out of bounds of [0, " + this.width.ToString() + ")");
            }
#endif

            return (long)x * (long)PixelFormatSizeOf;
        }

        /// <summary>
        /// Gets the number of bytes from the beginning of the RGB32_Surface's buffer to
        /// the requested point.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns>
        /// The number of bytes between (0,0) and (x,y).
        /// </returns>
        public long  GetPointByteOffset(int x, int y)
        {
            return GetRowByteOffset(y) + GetColumnByteOffset(x);
        }

        /// <summary>
        /// Gets the number of bytes from the beginning of the RGB32_Surface's buffer to
        /// the requested point.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns>
        /// The number of bytes between (0,0) and (x,y).
        /// </returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetPointByteOffset().
        /// </remarks>
        public long GetPointByteOffsetUnchecked(int x, int y)
        {
#if DEBUG
            if (x < 0 || x >= this.width)
            {
                Tracing.Ping("x=" + x.ToString() + " is out of bounds of [0, " + this.width.ToString() + ")");
            }

            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return GetRowByteOffsetUnchecked(y) + GetColumnByteOffsetUnchecked(x);
        }

        /// <summary>
        /// Gets the color at a specified point in the RGB32_Surface.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns>The color at the requested location.</returns>
        public ColorPixelBase GetPoint(int x, int y, ColorPixelBase RequiredFormat)
        {
            return RequiredFormat.TranslateColor( GetPoint(x, y) );
        }
        public ColorPixelBase  GetPoint(int x, int y)
        {
            return _ColorPixelBase.TranslateColor( this[x, y] );
        }
        public void SetPoint(int x,int y, ColorPixelBase color)
        {
            if (color.GetType() == _ColorPixelBase.GetType())
                this[x, y] = color.ToInt32();
            else 
                this[x, y] = this.ColorPixelBase.TranslateColor(color).ToInt32();

        }
        /// <summary>
        /// Gets the color at a specified point in the Surface.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns>The color at the requested location.</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetPoint().
        /// </remarks>
        public unsafe ColorPixelBase GetPointUnchecked(int x, int y, ColorPixelBase RequiredFormat)
        {
#if DEBUG
            if (x < 0 || x >= this.width)
            {
                Tracing.Ping("x=" + x.ToString() + " is out of bounds of [0, " + this.width.ToString() + ")");
            }

            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return RequiredFormat.TranslateColor( *(x + (Int32*)(((byte*)scan0.VoidStar) + (y * stride))) );
        }

       

        /// <summary>
        /// Gets the color at a specified point in the Surface.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns>The color at the requested location.</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetPoint().
        /// </remarks>
        public unsafe  Int32   GetPointUnchecked(int x, int y)
        {
#if DEBUG
            if (x < 0 || x >= this.width)
            {
                Tracing.Ping("x=" + x.ToString() + " is out of bounds of [0, " + this.width.ToString() + ")");
            }

            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return *(x + (Int32*)(((byte*)scan0.VoidStar) + (y * stride)));
        }

        /// <summary>
        /// Gets the color at a specified point in the RGB32_Surface.
        /// </summary>
        /// <param name="pt">The point to retrieve.</param>
        /// <returns>The color at the requested location.</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetPoint().
        /// </remarks>
        public ColorPixelBase GetPointUnchecked(Point pt, ColorPixelBase  RequiredFormat)
        {
            return RequiredFormat.TranslateColor(  GetPointUnchecked(pt.X, pt.Y ) );
        }
      

         /// <summary>
         /// Gets the address in memory of the requested point.
         /// </summary>
         /// <param name="x">The x offset.</param>
         /// <param name="y">The y offset.</param>
         /// <returns>A pointer to the requested point in the RGB32_Surface.</returns>
         /// <remarks>Since this method returns a pointer, it is potentially unsafe to use.</remarks>
         protected unsafe Int32* GetPointAddress(int x, int y)
         {
             if (x < 0 || x >= Width)
             {
                 throw new ArgumentOutOfRangeException("x", "Out of bounds: x=" + x.ToString());
             }

             return GetRowAddress(y) + x;
         }

         /// <summary>
         /// Gets the address in memory of the requested point.
         /// </summary>
         /// <param name="pt">The point to retrieve.</param>
         /// <returns>A pointer to the requested point in the RGB32_Surface.</returns>
         /// <remarks>Since this method returns a pointer, it is potentially unsafe to use.</remarks>
         protected unsafe Int32* GetPointAddress(Point pt)
         {
             return GetPointAddress(pt.X, pt.Y);
         }

        /// <summary>
        /// Gets the address in memory of the requested point.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns>A pointer to the requested point in the RGB32_Surface.</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetPointAddress().
        /// </remarks>
        protected virtual unsafe Int32* GetPointAddressUnchecked(int x, int y)
        {
#if DEBUG
            if (x < 0 || x >= this.width)
            {
                Tracing.Ping("x=" + x.ToString() + " is out of bounds of [0, " + this.width.ToString() + ")");
            }

            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return unchecked(x + (Int32 *)(((byte*)scan0.VoidStar) + (y * stride)));
        }

        /// <summary>
        /// Gets the address in memory of the requested point.
        /// </summary>
        /// <param name="pt">The point to retrieve.</param>
        /// <returns>A pointer to the requested point in the RGB32_Surface.</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetPointAddress().
        /// </remarks>
        protected virtual unsafe Int32* GetPointAddressUnchecked(Point pt)
        {
            return GetPointAddressUnchecked(pt.X, pt.Y);
        }
        public virtual unsafe Int32* hGetPointAddressUnchecked(Point pt)
        {
            return GetPointAddressUnchecked(pt.X, pt.Y);
        }
        #endregion
        
        #region RowFunctions
        /// <summary>
        /// Gets a MemoryBlock that references the row requested.
        /// </summary>
        /// <param name="y">The row.</param>
        /// <returns>A MemoryBlock that gives access to the bytes in the specified row.</returns>
        /// <remarks>This method is the safest to use for direct memory access to a row's pixel data.</remarks>
        protected  MemoryBlock GetRow(int y,bool dummy)
        {
            return new MemoryBlock(scan0, GetRowByteOffset(y), (long)width * (long)this.PixelFormatSizeOf );
        }

        
        /// <summary>
        /// Gets a MemoryBlock that references the row requested.
        /// </summary>
        /// <param name="y">The row.</param>
        /// <returns>A MemoryBlock that gives access to the bytes in the specified row.</returns>
        /// <remarks>This method is the safest to use for direct memory access to a row's pixel data.</remarks>
        public MemoryBlock GetRow(int y,ColorPixelBase RequestedFormat)
        {
            //todo: this does not return the requested format.  It should convert the pixels before return
            return new MemoryBlock(scan0, GetRowByteOffset(y), (long)width * (long)this.PixelFormatSizeOf);
        }
        public void  SetRow(int y, MemoryBlock Row, bool CorrectFormat)
        {
            
        }
        #endregion

        #region IsAttributes
        public bool IsDisposed
        {
            get
            {
                return this.disposed;
            }
        }
        public bool IsContiguousMemoryRegion(Rectangle bounds)
        {
            bool oneRow = (bounds.Height == 1);
            bool manyRows = (this.Stride == (this.Width * this.PixelFormatSizeOf ) &&
                this.Width == bounds.Width);

            return oneRow || manyRows;
        }

        /// <summary>
        /// Determines if the requested pixel coordinate is within bounds.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>true if (x,y) is in bounds, false if it's not.</returns>
        public bool IsVisible(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        /// <summary>
        /// Determines if the requested pixel coordinate is within bounds.
        /// </summary>
        /// <param name="pt">The coordinate.</param>
        /// <returns>true if (pt.X, pt.Y) is in bounds, false if it's not.</returns>
        public bool IsVisible(Point pt)
        {
            return IsVisible(pt.X, pt.Y);
        }

        /// <summary>
        /// Determines if the requested row offset is within bounds.
        /// </summary>
        /// <param name="y">The row.</param>
        /// <returns>true if y &gt;= 0 and y &lt; height, otherwise false</returns>
        public bool IsRowVisible(int y)
        {
            return y >= 0 && y < Height;
        }

        /// <summary>
        /// Determines if the requested column offset is within bounds.
        /// </summary>
        /// <param name="x">The column.</param>
        /// <returns>true if x &gt;= 0 and x &lt; width, otherwise false.</returns>
        public bool IsColumnVisible(int x)
        {
            return x >= 0 && x < Width;
        }
#endregion

        #region BilinearStuff

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using super sampling. If the source RGB32_Surface is less wide
        /// or less tall than this RGB32_Surface (i.e. magnification), bicubic resampling is used instead. If either
        /// the source or destination has a dimension that is only 1 pixel, nearest neighbor is used.
        /// </summary>
        /// <param name="source">The Surface to read pixels from.</param>
        /// <remarks>This method was implemented with correctness, not performance, in mind.</remarks>
        public void SuperSamplingFitSurface(Surface source)
        {
            SuperSamplingFitSurface(source, this.Bounds);
        }

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using super sampling. If the source RGB32_Surface is less wide
        /// or less tall than this RGB32_Surface (i.e. magnification), bicubic resampling is used instead. If either
        /// the source or destination has a dimension that is only 1 pixel, nearest neighbor is used.
        /// </summary>
        /// <param name="source">The RGB32_Surface to read pixels from.</param>
        /// <param name="dstRoi">The rectangle to clip rendering to.</param>
        /// <remarks>This method was implemented with correctness, not performance, in mind.</remarks>
        public virtual void SuperSamplingFitSurface(Surface uSource, Rectangle dstRoi)
        {
            Surface source = Core.Surfaces.SurfaceFactory.ConvertSurface(uSource, _ColorPixelBase);
            if (source.Width == Width && source.Height == Height)
            {
                CopySurface(source);
            }
            else if (source.Width <= Width || source.Height <= Height)
            {
                if (source.width < 2 || source.height < 2 || this.width < 2 || this.height < 2)
                {
                    this.NearestNeighborFitSurface(source, dstRoi);
                }
                else
                {
                    this.BicubicFitSurface(source, dstRoi);
                }
            }
            else unsafe
                {
                    Rectangle dstRoi2 = Rectangle.Intersect(dstRoi, this.Bounds);

                    for (int dstY = dstRoi2.Top; dstY < dstRoi2.Bottom; ++dstY)
                    {
                        double srcTop = (double)(dstY * source.height) / (double)height;
                        double srcTopFloor = Math.Floor(srcTop);
                        double srcTopWeight = 1 - (srcTop - srcTopFloor);
                        int srcTopInt = (int)srcTopFloor;

                        double srcBottom = (double)((dstY + 1) * source.height) / (double)height;
                        double srcBottomFloor = Math.Floor(srcBottom - 0.00001);
                        double srcBottomWeight = srcBottom - srcBottomFloor;
                        int srcBottomInt = (int)srcBottomFloor;

                        Int32* dstPtr = this.GetPointAddressUnchecked(dstRoi2.Left, dstY);

                        for (int dstX = dstRoi2.Left; dstX < dstRoi2.Right; ++dstX)
                        {
                            double srcLeft = (double)(dstX * source.width) / (double)width;
                            double srcLeftFloor = Math.Floor(srcLeft);
                            double srcLeftWeight = 1 - (srcLeft - srcLeftFloor);
                            int srcLeftInt = (int)srcLeftFloor;

                            double srcRight = (double)((dstX + 1) * source.width) / (double)width;
                            double srcRightFloor = Math.Floor(srcRight - 0.00001);
                            double srcRightWeight = srcRight - srcRightFloor;
                            int srcRightInt = (int)srcRightFloor;

                            /* double blueSum = 0;
                             double greenSum = 0;
                             double redSum = 0;*/
                            double alphaSum = 0;
                            double[] ColorChannels = new double[_ColorPixelBase.NumChannels];

                          
                            int tempY =(int) (srcTopInt + 1);
                            
                            for (int srcY = srcTopInt + 1; srcY < srcBottomInt; ++srcY)
                            {
                                _ColorPixelBase = source.GetPoint(srcLeftInt, tempY);// _ColorPixelBase.TranslateColor(source[(int)srcLeftInt, (int)tempY]);
                                double a = _ColorPixelBase.alpha;
                                for (int jj = 0; jj < _ColorPixelBase.NumChannels; jj++)
                                    ColorChannels[jj] += srcLeftWeight * _ColorPixelBase.GetChannel(jj) * a;
                                
                                alphaSum += a * srcLeftWeight;
                                tempY++;
                                //srcLeftPtr = (ColorPixelBase*)((byte*)srcLeftPtr + source.stride);
                            }

                            // right fractional edge
                            //ColorPixelBase* srcRightPtr = source.GetPointAddressUnchecked(srcRightInt, srcTopInt + 1);
                            
                            tempY = (int)(srcTopInt + 1);
                            for (int srcY = srcTopInt + 1; srcY < srcBottomInt; ++srcY)
                            {
                                _ColorPixelBase = source.GetPoint(srcRightInt, tempY);// _ColorPixelBase.TranslateColor(source[(int)srcRightInt, (int)tempY]);
                                double a = _ColorPixelBase.alpha;
                                for (int jj = 0; jj < _ColorPixelBase.NumChannels; jj++)
                                    ColorChannels[jj] += srcRightWeight * _ColorPixelBase.GetChannel(jj) * a;

                                alphaSum += a * srcRightWeight;
                                tempY++;
                            }

                            // top fractional edge
                            //ColorPixelBase* srcTopPtr = source.GetPointAddressUnchecked(srcLeftInt + 1, srcTopInt);
                            
                            int TempX = srcLeftInt + 1;
                            tempY = srcTopInt;
                            for (int srcX = srcLeftInt + 1; srcX < srcRightInt; ++srcX)
                            {
                                _ColorPixelBase = source.GetPoint(TempX, tempY);
                                double a = _ColorPixelBase.alpha;
                                for (int jj = 0; jj < _ColorPixelBase.NumChannels; jj++)
                                    ColorChannels[jj] += srcTopWeight * _ColorPixelBase.GetChannel(jj) * a;

                                alphaSum += a * srcTopWeight;
                                TempX++;
                            }

                            // bottom fractional edge
                            //ColorPixelBase* srcBottomPtr = source.GetPointAddressUnchecked(srcLeftInt + 1, srcBottomInt);
                            tempY = srcBottomInt;
                            TempX = srcLeftInt + 1;
                            for (int srcX = srcLeftInt + 1; srcX < srcRightInt; ++srcX)
                            {
                                _ColorPixelBase = source.GetPoint(TempX, tempY);
                                double a = _ColorPixelBase.alpha;
                                for (int jj = 0; jj < _ColorPixelBase.NumChannels; jj++)
                                    ColorChannels[jj] += srcTopWeight * _ColorPixelBase.GetChannel(jj) * a;

                                alphaSum += a * srcTopWeight;
                                TempX++;
                            }

                            // center area
                            for (int srcY = srcTopInt + 1; srcY < srcBottomInt; ++srcY)
                            {
                                

                                for (int srcX = srcLeftInt + 1; srcX < srcRightInt; ++srcX)
                                {
                                    _ColorPixelBase = source.GetPoint(srcX, srcY);
                                    double a = _ColorPixelBase.alpha;
                                    for (int jj = 0; jj < _ColorPixelBase.NumChannels; jj++)
                                        ColorChannels[jj] += srcTopWeight * _ColorPixelBase.GetChannel(jj) * a;

                                    alphaSum += a * srcTopWeight;
                                }
                            }

                            // four corner pixels
                            _ColorPixelBase  = source.GetPoint(srcLeftInt, srcTopInt);
                            double TRa = _ColorPixelBase.alpha;
                            for (int jj = 0; jj < _ColorPixelBase.NumChannels; jj++)
                                ColorChannels[jj] += srcTopWeight * _ColorPixelBase.GetChannel(jj) * TRa;

                            alphaSum += TRa * srcTopWeight;

                            _ColorPixelBase = source.GetPoint(srcRightInt, srcTopInt);
                            TRa = _ColorPixelBase.alpha;
                            for (int jj = 0; jj < _ColorPixelBase.NumChannels; jj++)
                                ColorChannels[jj] += srcTopWeight * _ColorPixelBase.GetChannel(jj) * TRa;

                            alphaSum += TRa * srcTopWeight;

                            _ColorPixelBase = source.GetPoint(srcLeftInt, srcBottomInt);
                            TRa = _ColorPixelBase.alpha;
                            for (int jj = 0; jj < _ColorPixelBase.NumChannels; jj++)
                                ColorChannels[jj] += srcTopWeight * _ColorPixelBase.GetChannel(jj) * TRa;

                            alphaSum += TRa * srcTopWeight;

                            _ColorPixelBase = source.GetPoint(srcRightInt, srcBottomInt);
                            TRa = _ColorPixelBase.alpha;
                            for (int jj = 0; jj < _ColorPixelBase.NumChannels; jj++)
                                ColorChannels[jj] += srcTopWeight * _ColorPixelBase.GetChannel(jj) * TRa;

                            alphaSum += TRa * srcTopWeight;

                            double area = (srcRight - srcLeft) * (srcBottom - srcTop);

                            double alpha = alphaSum / area;
                            double[] FinalChannels = new double[source.ColorPixelBase.NumChannels];
                           

                            if (alpha == 0)
                            {
                                for (int i = 0; i < FinalChannels.Length; i++)
                                    FinalChannels[i] = 0;
                            }
                            else
                            {
                                for (int i = 0; i < FinalChannels.Length; i++)
                                    FinalChannels[i] = ColorChannels[i] / alphaSum; 
                               
                            }

                            // add 0.5 so that rounding goes in the direction we want it to
                            for (int i = 0; i < FinalChannels.Length; i++)
                            {
                                FinalChannels[i] += .5;
                                _ColorPixelBase.SetChannel(i, (Int32 )FinalChannels[i]);
                            }
                            _ColorPixelBase.alpha =(byte)( alpha + 0.5);

                            
                            *dstPtr = _ColorPixelBase.ToInt32();
                            ++dstPtr;
                        }
                    }
                }
        }

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using nearest neighbor resampling.
        /// </summary>
        /// <param name="source">The RGB32_Surface to read pixels from.</param>
        public void NearestNeighborFitSurface(Surface source)
        {
            NearestNeighborFitSurface(source, this.Bounds);
        }

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using nearest neighbor resampling.
        /// </summary>
        /// <param name="source">The RGB32_Surface to read pixels from.</param>
        /// <param name="dstRoi">The rectangle to clip rendering to.</param>
        public virtual void NearestNeighborFitSurface(Surface uSource, Rectangle dstRoi)
        {
            Surface source = Core.Surfaces.SurfaceFactory.ConvertSurface(uSource, _ColorPixelBase);
            Rectangle roi = Rectangle.Intersect(dstRoi, this.Bounds);

            unsafe
            {
                for (int dstY = roi.Top; dstY < roi.Bottom; ++dstY)
                {
                    int srcY = (dstY * source.height) / height;
                    Int32 * srcRow = source.GetRowAddressUnchecked(srcY);
                    Int32 * dstPtr = this.GetPointAddressUnchecked(roi.Left, dstY);

                    for (int dstX = roi.Left; dstX < roi.Right; ++dstX)
                    {
                        int srcX = (dstX * source.width) / width;
                        *dstPtr = *(srcRow + srcX);
                        ++dstPtr;
                    }
                }
            }
        }

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using bicubic interpolation.
        /// </summary>
        /// <param name="source">The Surface to read pixels from.</param>
        /// <remarks>
        /// This method was implemented with correctness, not performance, in mind. 
        /// Based on: "Bicubic Interpolation for Image Scaling" by Paul Bourke,
        ///           http://astronomy.swin.edu.au/%7Epbourke/colour/bicubic/
        /// </remarks>
        public void BicubicFitSurface(Surface source)
        {
            BicubicFitSurface(source, this.Bounds);
        }

        private double CubeClamped(double x)
        {
            if (x >= 0)
            {
                return x * x * x;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Implements R() as defined at http://astronomy.swin.edu.au/%7Epbourke/colour/bicubic/
        /// </summary>
        protected double R(double x)
        {
            return (CubeClamped(x + 2) - (4 * CubeClamped(x + 1)) + (6 * CubeClamped(x)) - (4 * CubeClamped(x - 1))) / 6;
        }

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using bicubic interpolation.
        /// </summary>
        /// <param name="source">The Surface to read pixels from.</param>
        /// <param name="dstRoi">The rectangle to clip rendering to.</param>
        /// <remarks>
        /// This method was implemented with correctness, not performance, in mind. 
        /// Based on: "Bicubic Interpolation for Image Scaling" by Paul Bourke,
        ///           http://astronomy.swin.edu.au/%7Epbourke/colour/bicubic/
        /// </remarks>
        public virtual void BicubicFitSurface(Surface source, Rectangle dstRoi)
        {
            float leftF = (1 * (float)(width - 1)) / (float)(source.width - 1);
            float topF = (1 * (height - 1)) / (float)(source.height - 1);
            float rightF = ((float)(source.width - 3) * (float)(width - 1)) / (float)(source.width - 1);
            float bottomF = ((float)(source.Height - 3) * (float)(height - 1)) / (float)(source.height - 1);

            int left = (int)Math.Ceiling((double)leftF);
            int top = (int)Math.Ceiling((double)topF);
            int right = (int)Math.Floor((double)rightF);
            int bottom = (int)Math.Floor((double)bottomF);

            Rectangle[] rois = new Rectangle[] {
                                                   Rectangle.FromLTRB(left, top, right, bottom),
                                                   new Rectangle(0, 0, width, top),
                                                   new Rectangle(0, top, left, height - top),
                                                   new Rectangle(right, top, width - right, height - top),
                                                   new Rectangle(left, bottom, right - left, height - bottom)
                                               };

            for (int i = 0; i < rois.Length; ++i)
            {
                rois[i].Intersect(dstRoi);

                if (rois[i].Width > 0 && rois[i].Height > 0)
                {
                    if (i == 0)
                    {
                        BicubicFitSurfaceUnchecked(source, rois[i]);
                    }
                    else
                    {
                        BicubicFitSurfaceChecked(source, rois[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Implements bicubic filtering with bounds checking at every pixel.
        /// </summary>
        protected virtual void BicubicFitSurfaceChecked(Surface uSource, Rectangle dstRoi)
        {
            Surface source = Core.Surfaces.SurfaceFactory.ConvertSurface(uSource, _ColorPixelBase);
            if (this.width < 2 || this.height < 2 || source.width < 2 || source.height < 2)
            {
                SuperSamplingFitSurface(source, dstRoi);
            }
            else
            {
                unsafe
                {
                    Rectangle roi = Rectangle.Intersect(dstRoi, this.Bounds);
                    Rectangle roiIn = Rectangle.Intersect(dstRoi, new Rectangle(1, 1, width - 1, height - 1));

                    IntPtr rColCacheIP = Memory.Allocate(4 * (ulong)roi.Width * (ulong)sizeof(double));
                    double* rColCache = (double*)rColCacheIP.ToPointer();

                    // Precompute and then cache the value of R() for each column
                    for (int dstX = roi.Left; dstX < roi.Right; ++dstX)
                    {
                        double srcColumn = (double)(dstX * (source.width - 1)) / (double)(width - 1);
                        double srcColumnFloor = Math.Floor(srcColumn);
                        double srcColumnFrac = srcColumn - srcColumnFloor;
                        int srcColumnInt = (int)srcColumn;

                        for (int m = -1; m <= 2; ++m)
                        {
                            int index = (m + 1) + ((dstX - roi.Left) * 4);
                            double x = m - srcColumnFrac;
                            rColCache[index] = R(x);
                        }
                    }

                    // Set this up so we can cache the R()'s for every row
                    double* rRowCache = stackalloc double[4];

                    for (int dstY = roi.Top; dstY < roi.Bottom; ++dstY)
                    {
                        double srcRow = (double)(dstY * (source.height - 1)) / (double)(height - 1);
                        double srcRowFloor = (double)Math.Floor(srcRow);
                        double srcRowFrac = srcRow - srcRowFloor;
                        int srcRowInt = (int)srcRow;
                        Int32 * dstPtr = this.GetPointAddressUnchecked(roi.Left, dstY);

                        // Compute the R() values for this row
                        for (int n = -1; n <= 2; ++n)
                        {
                            double x = srcRowFrac - n;
                            rRowCache[n + 1] = R(x);
                        }

                        // See Perf Note below
                        //int nFirst = Math.Max(-srcRowInt, -1);
                        //int nLast = Math.Min(source.height - srcRowInt - 1, 2);

                        for (int dstX = roi.Left; dstX < roi.Right; dstX++)
                        {
                            double srcColumn = (double)(dstX * (source.width - 1)) / (double)(width - 1);
                            double srcColumnFloor = Math.Floor(srcColumn);
                            double srcColumnFrac = srcColumn - srcColumnFloor;
                            int srcColumnInt = (int)srcColumn;

                            double[] ColorChannels=new double[_ColorPixelBase.NumChannels ];
                            double alphaSum = 0;
                            double totalWeight = 0;

                            // See Perf Note below
                            //int mFirst = Math.Max(-srcColumnInt, -1);
                            //int mLast = Math.Min(source.width - srcColumnInt - 1, 2);

                            //ColorPixelBase* srcPtr = source.GetPointAddressUnchecked(srcColumnInt - 1, srcRowInt - 1);
                            int Tempx = srcColumnInt - 1;
                            for (int n = -1; n <= 2; ++n)
                            {
                                int srcY = srcRowInt + n;
                                for (int m = -1; m <= 2; ++m)
                                {
                                    // Perf Note: It actually benchmarks faster on my system to do
                                    // a bounds check for every (m,n) than it is to limit the loop
                                    // to nFirst-Last and mFirst-mLast.
                                    // I'm leaving the code above, albeit commented out, so that
                                    // benchmarking between these two can still be performed.
                                    if (source.IsVisible(srcColumnInt + m, srcY))
                                    {
                                        double w0 = rColCache[(m + 1) + (4 * (dstX - roi.Left))];
                                        double w1 = rRowCache[n + 1];
                                        double w = w0 * w1;
                                        _ColorPixelBase = source.GetPoint(Tempx, srcY);

                                        double a = _ColorPixelBase.alpha;
                                        for (int i = 0; i < ColorChannels.Length; i++)
                                            ColorChannels[i] += _ColorPixelBase.GetChannel(i) * w * a;
                                        alphaSum += a * w;

                                        totalWeight += w;
                                    }

                                    Tempx++;
                                }

                                //srcPtr = (ColorPixelBase*)((byte*)(srcPtr - 4) + source.stride);
                            }
                            double[] FinalChannels = new double[ColorChannels.Length];
                            double alpha = alphaSum / totalWeight;
                            if (alpha == 0)
                            {
                                for (int i = 0; i < FinalChannels.Length; i++)
                                    FinalChannels[i] = 0;
                            }
                            else
                            {
                                for (int i = 0; i < FinalChannels.Length; i++)
                                    FinalChannels[i] = ColorChannels[i] / alphaSum;

                            }

                            // add 0.5 so that rounding goes in the direction we want it to
                            for (int i = 0; i < FinalChannels.Length; i++)
                            {
                                FinalChannels[i] += .5;
                                _ColorPixelBase.SetChannel(i, (Int32)FinalChannels[i]);
                            }
                            _ColorPixelBase.alpha = (byte)(alpha + 0.5);

                            
                            *dstPtr = _ColorPixelBase.ToInt32();
                            ++dstPtr;
                        } // for (dstX...
                    } // for (dstY...

                    Memory.Free(rColCacheIP);
                } // unsafe
            }
        }

        /// <summary>
        /// Implements bicubic filtering with NO bounds checking at any pixel.
        /// </summary>
        public virtual void BicubicFitSurfaceUnchecked(Surface uSource, Rectangle dstRoi)
        {
            Surface source = Core.Surfaces.SurfaceFactory.ConvertSurface(uSource, _ColorPixelBase);
            if (this.width < 2 || this.height < 2 || source.width < 2 || source.height < 2)
            {
                SuperSamplingFitSurface(source, dstRoi);
            }
            else
            {
                unsafe
                {
                    Rectangle roi = Rectangle.Intersect(dstRoi, this.Bounds);
                    Rectangle roiIn = Rectangle.Intersect(dstRoi, new Rectangle(1, 1, width - 1, height - 1));

                    IntPtr rColCacheIP = Memory.Allocate(4 * (ulong)roi.Width * (ulong)sizeof(double));
                    double* rColCache = (double*)rColCacheIP.ToPointer();

                    // Precompute and then cache the value of R() for each column
                    for (int dstX = roi.Left; dstX < roi.Right; ++dstX)
                    {
                        double srcColumn = (double)(dstX * (source.width - 1)) / (double)(width - 1);
                        double srcColumnFloor = Math.Floor(srcColumn);
                        double srcColumnFrac = srcColumn - srcColumnFloor;
                        int srcColumnInt = (int)srcColumn;

                        for (int m = -1; m <= 2; ++m)
                        {
                            int index = (m + 1) + ((dstX - roi.Left) * 4);
                            double x = m - srcColumnFrac;
                            rColCache[index] = R(x);
                        }
                    }

                    // Set this up so we can cache the R()'s for every row
                    double* rRowCache = stackalloc double[4];

                    for (int dstY = roi.Top; dstY < roi.Bottom; ++dstY)
                    {
                        double srcRow = (double)(dstY * (source.height - 1)) / (double)(height - 1);
                        double srcRowFloor = Math.Floor(srcRow);
                        double srcRowFrac = srcRow - srcRowFloor;
                        int srcRowInt = (int)srcRow;
                        Int32* dstPtr = this.GetPointAddressUnchecked(roi.Left, dstY);

                        // Compute the R() values for this row
                        for (int n = -1; n <= 2; ++n)
                        {
                            double x = srcRowFrac - n;
                            rRowCache[n + 1] = R(x);
                        }

                        rColCache = (double*)rColCacheIP.ToPointer();
                        //ColorPixelBase* srcRowPtr = source.GetRowAddressUnchecked(srcRowInt - 1);

                        for (int dstX = roi.Left; dstX < roi.Right; dstX++)
                        {
                            double srcColumn = (double)(dstX * (source.width - 1)) / (double)(width - 1);
                            double srcColumnFloor = Math.Floor(srcColumn);
                            double srcColumnFrac = srcColumn - srcColumnFloor;
                            int srcColumnInt = (int)srcColumn;

                            double[] ColorChannels = new double[_ColorPixelBase.NumChannels];// blueSum = 0;
                            
                            double alphaSum = 0;
                            double totalWeight = 0;

                            //ColorPixelBase* srcPtr = srcRowPtr + srcColumnInt - 1;
                            for (int n = 0; n <= 3; ++n)
                            {
                                double w0 = rColCache[0] * rRowCache[n];
                                double w1 = rColCache[1] * rRowCache[n];
                                double w2 = rColCache[2] * rRowCache[n];
                                double w3 = rColCache[3] * rRowCache[n];

                                ColorPixelBase[] Cs = new ColorPixelBase[4];
                                Cs[0] = source.GetPoint(srcColumnInt - 1, srcRowInt - 1 + n);
                                Cs[1] = source.GetPoint(srcColumnInt , srcRowInt - 1 + n);
                                Cs[2] = source.GetPoint(srcColumnInt +1, srcRowInt - 1 + n);
                                Cs[3] = source.GetPoint(srcColumnInt +2, srcRowInt - 1 + n);


                                double a0 = Cs[0].alpha; //srcPtr[0].A;
                                double a1 = Cs[1].alpha; //srcPtr[1].A;
                                double a2 = Cs[2].alpha; //srcPtr[2].A;
                                double a3 = Cs[3].alpha; //srcPtr[3].A;

                                alphaSum += (a0 * w0) + (a1 * w1) + (a2 * w2) + (a3 * w3);
                                totalWeight += w0 + w1 + w2 + w3;

                                for (int i = 0; i < ColorChannels.Length; i++)
                                {
                                    ColorChannels[i] += (a0 * Cs[0].GetChannel(i) * w0) + (a1 * Cs[1].GetChannel(i) * w1) + (a2 * Cs[2].GetChannel(i) * w2) + (a3 * Cs[3].GetChannel(i) * w3);
                                }
                                //srcPtr = (ColorPixelBase*)((byte*)srcPtr + source.stride);
                            }

                            double alpha = alphaSum / totalWeight;

                            double[] FinalChannels = new double[_ColorPixelBase.NumChannels];// blue;
                           

                            if (alpha == 0)
                            {
                                for (int i = 0; i < FinalChannels.Length; i++)
                                    FinalChannels[i] = 0;
                            }
                            else
                            {
                                for (int i = 0; i < FinalChannels.Length; i++)
                                {
                                    FinalChannels[i] = ColorChannels[i] / alphaSum + .5;
                                    _ColorPixelBase.SetChannel(i,(int) FinalChannels[i]);
                                }
                                
                                // add 0.5 to ensure truncation to uint results in rounding
                                alpha += 0.5;
                                _ColorPixelBase.alpha = (byte)alpha;
                            }

                            *dstPtr = _ColorPixelBase.ToInt32();
                            ++dstPtr;
                            rColCache += 4;
                        } 
                    } 

                    Memory.Free(rColCacheIP);
                } // unsafe
            }
        }

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using bilinear interpolation.
        /// </summary>
        /// <param name="source">The RGB32_Surface to read pixels from.</param>
        /// <remarks>This method was implemented with correctness, not performance, in mind.</remarks>
        public void BilinearFitSurface(Surface source)
        {
            BilinearFitSurface(source, this.Bounds);
        }

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using bilinear interpolation.
        /// </summary>
        /// <param name="source">The RGB32_Surface to read pixels from.</param>
        /// <param name="dstRoi">The rectangle to clip rendering to.</param>
        /// <remarks>This method was implemented with correctness, not performance, in mind.</remarks>
        public virtual void BilinearFitSurface(Surface uSource, Rectangle dstRoi)
        {
            Surface source = Core.Surfaces.SurfaceFactory.ConvertSurface(uSource, _ColorPixelBase);
            if (dstRoi.Width < 2 || dstRoi.Height < 2 || this.width < 2 || this.height < 2)
            {
                SuperSamplingFitSurface(source, dstRoi);
            }
            else
            {
                unsafe
                {
                    Rectangle roi = Rectangle.Intersect(dstRoi, this.Bounds);

                    for (int dstY = roi.Top; dstY < roi.Bottom; ++dstY)
                    {
                        Int32* dstRowPtr = this.GetRowAddressUnchecked(dstY);
                        float srcRow = (float)(dstY * (source.height - 1)) / (float)(height - 1);

                        for (int dstX = roi.Left; dstX < roi.Right; dstX++)
                        {
                            float srcColumn = (float)(dstX * (source.width - 1)) / (float)(width - 1);
                            *dstRowPtr = source.GetBilinearSample(srcColumn, srcRow,source._ColorPixelBase ).ToInt32();
                            ++dstRowPtr;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using the given algorithm.
        /// </summary>
        /// <param name="algorithm">The RGB32_Surface to copy pixels from.</param>
        /// <param name="source">The algorithm to use.</param>
        public void FitSurface(ResamplingAlgorithm algorithm, Surface source)
        {
            FitSurface(algorithm, source, this.Bounds);
        }

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using the given algorithm.
        /// </summary>
        /// <param name="algorithm">The RGB32_Surface to copy pixels from.</param>
        /// <param name="dstRoi">The rectangle to clip rendering to.</param>
        /// <param name="source">The algorithm to use.</param>
        public void FitSurface(ResamplingAlgorithm algorithm, Surface source, Rectangle dstRoi)
        {
            switch (algorithm)
            {
                case ResamplingAlgorithm.Bicubic:
                    BicubicFitSurface(source, dstRoi);
                    break;

                case ResamplingAlgorithm.Bilinear:
                    BilinearFitSurface(source, dstRoi);
                    break;

                case ResamplingAlgorithm.NearestNeighbor:
                    NearestNeighborFitSurface(source, dstRoi);
                    break;

                case ResamplingAlgorithm.SuperSampling:
                    SuperSamplingFitSurface(source, dstRoi);
                    break;

                default:
                    throw new InvalidEnumArgumentException("algorithm");
            }
        }


        public virtual  ColorPixelBase   GetBilinearSampleWrapped(float x, float y , ColorPixelBase RequestedFormat )
        {
            if (!Utility.IsNumber(x) || !Utility.IsNumber(y))
            {
                return RequestedFormat.TransparentColor()  ;
            }

            float u = x;
            float v = y;

            unchecked
            {
                int iu = (int)Math.Floor(u);
                uint sxfrac = (uint)(256 * (u - (float)iu));
                uint sxfracinv = 256 - sxfrac;

                int iv = (int)Math.Floor(v);
                uint syfrac = (uint)(256 * (v - (float)iv));
                uint syfracinv = 256 - syfrac;

                uint wul = (uint)(sxfracinv * syfracinv);
                uint wur = (uint)(sxfrac * syfracinv);
                uint wll = (uint)(sxfracinv * syfrac);
                uint wlr = (uint)(sxfrac * syfrac);

                int sx = iu;
                if (sx < 0)
                {
                    sx = (width - 1) + ((sx + 1) % width);
                }
                else if (sx > (width - 1))
                {
                    sx = sx % width;
                }

                int sy = iv;
                if (sy < 0)
                {
                    sy = (height - 1) + ((sy + 1) % height);
                }
                else if (sy > (height - 1))
                {
                    sy = sy % height;
                }

                int sleft = sx;
                int sright;

                if (sleft == (width - 1))
                {
                    sright = 0;
                }
                else
                {
                    sright = sleft + 1;
                }

                int stop = sy;
                int sbottom;

                if (stop == (height - 1))
                {
                    sbottom = 0;
                }
                else
                {
                    sbottom = stop + 1;
                }

                Int32  cul = GetPointUnchecked(sleft, stop);
                Int32  cur = GetPointUnchecked(sright, stop);
                Int32  cll = GetPointUnchecked(sleft, sbottom);
                Int32  clr = GetPointUnchecked(sright, sbottom);

                _ColorPixelBase = this._ColorPixelBase.BlendColors4W16IP(cul, wul, cur, wur, cll, wll, clr, wlr);

                return RequestedFormat.TranslateColor(_ColorPixelBase);
            }
        }

        public virtual  ColorPixelBase GetBilinearSample(float x, float y, ColorPixelBase  RequestedFormat)
        {
            if (!Utility.IsNumber(x) || !Utility.IsNumber(y))
            {
                return RequestedFormat.TransparentColor();
            }

            float u = x;
            float v = y;

            if (u >= 0 && v >= 0 && u < width && v < height)
            {
                unchecked
                {
                    int iu = (int)Math.Floor(u);
                    uint sxfrac = (uint)(256 * (u - (float)iu));
                    uint sxfracinv = 256 - sxfrac;

                    int iv = (int)Math.Floor(v);
                    uint syfrac = (uint)(256 * (v - (float)iv));
                    uint syfracinv = 256 - syfrac;

                    uint wul = (uint)(sxfracinv * syfracinv);
                    uint wur = (uint)(sxfrac * syfracinv);
                    uint wll = (uint)(sxfracinv * syfrac);
                    uint wlr = (uint)(sxfrac * syfrac);

                    int sx = iu;
                    int sy = iv;
                    int sleft = sx;
                    int sright;

                    if (sleft == (width - 1))
                    {
                        sright = sleft;
                    }
                    else
                    {
                        sright = sleft + 1;
                    }

                    int stop = sy;
                    int sbottom;

                    if (stop == (height - 1))
                    {
                        sbottom = stop;
                    }
                    else
                    {
                        sbottom = stop + 1;
                    }
                    unsafe{
                    Int32* cul = GetPointAddressUnchecked(sleft, stop);
                    Int32* cur = cul + (sright - sleft);
                    Int32* cll = GetPointAddressUnchecked(sleft, sbottom);
                    Int32* clr = cll + (sright - sleft);

                    _ColorPixelBase  = this._ColorPixelBase  .BlendColors4W16IP(*cul, wul, *cur, wur, *cll, wll, *clr, wlr);
                    }
                    return RequestedFormat.TranslateColor(_ColorPixelBase) ;
                }
            }
            else
            {
                return RequestedFormat.BlackColor() ;
            }
        }

        public virtual ColorPixelBase GetBilinearSampleClamped(float x, float y, ColorPixelBase RequestedFormat)
        {
            if (!Utility.IsNumber(x) || !Utility.IsNumber(y))
            {
                return RequestedFormat.TransparentColor ();
            }

            float u = x;
            float v = y;

            if (u < 0)
            {
                u = 0;
            }
            else if (u > this.Width - 1)
            {
                u = this.Width - 1;
            }

            if (v < 0)
            {
                v = 0;
            }
            else if (v > this.Height - 1)
            {
                v = this.Height - 1;
            }

            unchecked
            {
                int iu = (int)Math.Floor(u);
                uint sxfrac = (uint)(256 * (u - (float)iu));
                uint sxfracinv = 256 - sxfrac;

                int iv = (int)Math.Floor(v);
                uint syfrac = (uint)(256 * (v - (float)iv));
                uint syfracinv = 256 - syfrac;

                uint wul = (uint)(sxfracinv * syfracinv);
                uint wur = (uint)(sxfrac * syfracinv);
                uint wll = (uint)(sxfracinv * syfrac);
                uint wlr = (uint)(sxfrac * syfrac);

                int sx = iu;
                int sy = iv;
                int sleft = sx;
                int sright;

                if (sleft == (width - 1))
                {
                    sright = sleft;
                }
                else
                {
                    sright = sleft + 1;
                }

                int stop = sy;
                int sbottom;

                if (stop == (height - 1))
                {
                    sbottom = stop;
                }
                else
                {
                    sbottom = stop + 1;
                }
                unsafe
                {
                    Int32* cul = GetPointAddressUnchecked(sleft, stop);
                    Int32* cur = cul + (sright - sleft);
                    Int32* cll = GetPointAddressUnchecked(sleft, sbottom);
                    Int32* clr = cll + (sright - sleft);

                    _ColorPixelBase = this._ColorPixelBase .BlendColors4W16IP(*cul, wul, *cur, wur, *cll, wll, *clr, wlr);
                }
                return RequestedFormat.TranslateColor(_ColorPixelBase);
            }
        }
        #endregion

        #region this_ArrayLikeStuff
        /// <summary>
        /// Gets or sets the pixel value at the requested offset.
        /// </summary>
        /// <remarks>
        /// This property is implemented with correctness and error checking in mind. If performance
        /// is a concern, do not use it.
        /// </remarks>
        public Int32 this[int x, int y]
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("Surface");
                }

                if (x < 0 || y < 0 || x >= this.width || y >= this.height)
                {
                    throw new ArgumentOutOfRangeException("(x,y)", new Point(x, y), "Coordinates out of range, max=" + new Size(width - 1, height - 1).ToString());
                }

                unsafe
                {
                    return *GetPointAddressUnchecked(x, y);
                }
            }

            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("Surface");
                }

                if (x < 0 || y < 0 || x >= this.width || y >= this.height)
                {
                    throw new ArgumentOutOfRangeException("(x,y)", new Point(x, y), "Coordinates out of range, max=" + new Size(width - 1, height - 1).ToString());
                }

                unsafe
                {
                    *GetPointAddressUnchecked(x, y) = value;
                }
            }
        }

        public ColorPixelBase  this[int x, int y,ColorPixelBase RequestedFormat]
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("Surface");
                }

                if (x < 0 || y < 0 || x >= this.width || y >= this.height)
                {
                    throw new ArgumentOutOfRangeException("(x,y)", new Point(x, y), "Coordinates out of range, max=" + new Size(width - 1, height - 1).ToString());
                }

                unsafe
                {
                    return RequestedFormat.TranslateColor( *GetPointAddressUnchecked(x, y) );
                }
            }

            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("Surface");
                }

                if (x < 0 || y < 0 || x >= this.width || y >= this.height)
                {
                    throw new ArgumentOutOfRangeException("(x,y)", new Point(x, y), "Coordinates out of range, max=" + new Size(width - 1, height - 1).ToString());
                }

                unsafe
                {
                    *GetPointAddressUnchecked(x, y) = _ColorPixelBase.TranslateColor ( value ).ToInt32();
                }
            }
        }

        /// <summary>
        /// Gets or sets the pixel value at the requested offset.
        /// </summary>
        /// <remarks>
        /// This property is implemented with correctness and error checking in mind. If performance
        /// is a concern, do not use it.
        /// </remarks>
        protected Int32  this[Point pt]
        {
            get
            {
                return this[pt.X, pt.Y];
            }

            set
            {
                this[pt.X, pt.Y] = value;
            }
        }

        /// <summary>
        /// Gets or sets the pixel value at the requested offset.
        /// </summary>
        /// <remarks>
        /// This property is implemented with correctness and error checking in mind. If performance
        /// is a concern, do not use it.
        /// </remarks>
        public ColorPixelBase  this[Point pt, ColorPixelBase RequestedFormat]
        {
            get
            {
                return this[pt.X, pt.Y,RequestedFormat ];
            }

            set
            {
                this[pt.X, pt.Y,RequestedFormat ] = value;
            }
        }
        #endregion

        #region AliasedBitmap
        /// <summary>
        /// Helper function. Same as calling CreateAliasedBounds(Bounds).
        /// </summary>
        /// <returns>A GDI+ Bitmap that aliases the entire Surface.</returns>
        public Bitmap CreateAliasedBitmap()
        {
            return CreateAliasedBitmap(this.Bounds);
        }

        /// <summary>
        /// Helper function. Same as calling CreateAliasedBounds(bounds, true).
        /// </summary>
        /// <returns>A GDI+ Bitmap that aliases the entire Surface.</returns>
        public Bitmap CreateAliasedBitmap(Rectangle bounds)
        {
            return CreateAliasedBitmap(bounds, true);
        }

        /// <summary>
        /// Creates a GDI+ Bitmap object that aliases the same memory that this Surface does.
        /// Then you can use GDI+ to draw on to this RGB32_Surface.
        /// Note: Since the Bitmap does not hold a reference to this Surface object, nor to
        /// the MemoryBlock that it contains, you must hold a reference to the Surface object
        /// for as long as you wish to use the aliased Bitmap. Otherwise the memory may be
        /// freed and the Bitmap will look corrupt or cause other errors. You may use the
        /// RenderArgs class to help manage this lifetime instead.
        /// </summary>
        /// <param name="bounds">The rectangle of interest within this Surface that you wish to alias.</param>
        /// <param name="alpha">If true, the returned bitmap will use PixelFormat.Format32bppArgb. 
        /// If false, the returned bitmap will use PixelFormat.Format32bppRgb.</param>
        /// <returns>A GDI+ Bitmap that aliases the requested portion of the Surface.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><b>bounds</b> was not entirely within the boundaries of the Surface</exception>
        /// <exception cref="ObjectDisposedException">This Surface instance is already disposed.</exception>
        public virtual Bitmap CreateAliasedBitmap(Rectangle bounds, bool alpha)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }

            if (bounds.IsEmpty)
            {
                throw new ArgumentOutOfRangeException();
            }

            Rectangle clipped = Rectangle.Intersect(this.Bounds, bounds);

            if (clipped != bounds)
            {
                throw new ArgumentOutOfRangeException();
            }

            unsafe
            {
                return new Bitmap(bounds.Width, bounds.Height, stride, alpha ? this.PixelFormat : PixelFormat.Format64bppArgb ,
                    new IntPtr((void*)((byte*)scan0.VoidStar + GetPointByteOffsetUnchecked(bounds.X, bounds.Y))));
            }
        }

        #endregion

        #region CopySurface
        /// <summary>
        /// Creates a new Surface and copies the pixels from a Bitmap to it.
        /// </summary>
        /// <param name="bitmap">The Bitmap to duplicate.</param>
        /// <returns>A new Surface that is the same size as the given Bitmap and that has the same pixel values.</returns>
        public static Surface CopyFromBitmap(Bitmap bitmap)
        {
            Surface surface = new Surface(bitmap.Width, bitmap.Height,bitmap.PixelFormat );
            BitmapData bd = bitmap.LockBits(surface.Bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                for (int y = 0; y < bd.Height; ++y)
                {
                    Memory.Copy((void*)surface.GetRowAddress(y),
                        (byte*)bd.Scan0.ToPointer() + (y * bd.Stride), (ulong)(bd.Width * surface.PixelFormatSizeOf ));
                }
            }

            bitmap.UnlockBits(bd);
            return surface;
        }

        /// <summary>
        /// Copies the contents of the given RGB32_Surface to the upper left corner of this RGB32_Surface.
        /// </summary>
        /// <param name="source">The RGB32_Surface to copy pixels from.</param>
        /// <remarks>
        /// The source RGB32_Surface does not need to have the same dimensions as this RGB32_Surface. Clipping
        /// will be handled automatically. No resizing will be done.
        /// </remarks>
        public void CopySurface(Surface source)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }
            
            if (this.stride == source.stride &&
                (this.width * source.PixelFormatSizeOf ) == this.stride &&
                this.width == source.width &&
                this.height == source.height)
            {
                unsafe
                {
                    Memory.Copy(this.scan0.VoidStar,
                                source.scan0.VoidStar,
                                ((ulong)(height - 1) * (ulong)stride) + ((ulong)width * (ulong)source.PixelFormatSizeOf));
                }
            }
            else
            {
                int copyWidth = Math.Min(width, source.width);
                int copyHeight = Math.Min(height, source.height);

                unsafe
                {
                    for (int y = 0; y < copyHeight; ++y)
                    {
                        Memory.Copy(GetRowAddressUnchecked(y), source.GetRowAddressUnchecked(y), (ulong)copyWidth * (ulong)source.PixelFormatSizeOf);
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of the given RGB32_Surface to a location within this RGB32_Surface.
        /// </summary>
        /// <param name="source">The RGB32_Surface to copy pixels from.</param>
        /// <param name="dstOffset">
        /// The offset within this RGB32_Surface to start copying pixels to. This will map to (0,0) in the source.
        /// </param>
        /// <remarks>
        /// The source RGB32_Surface does not need to have the same dimensions as this RGB32_Surface. Clipping
        /// will be handled automatically. No resizing will be done.
        /// </remarks>
        public void CopySurface(Surface source, Point dstOffset)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }
           

            Rectangle dstRect = new Rectangle(dstOffset, source.Size);
            dstRect.Intersect(Bounds);

            if (dstRect.Width == 0 || dstRect.Height == 0)
            {
                return;
            }

            Point sourceOffset = new Point(dstRect.Location.X - dstOffset.X, dstRect.Location.Y - dstOffset.Y);
            Rectangle sourceRect = new Rectangle(sourceOffset, dstRect.Size);
            Surface sourceWindow = source.CreateWindow(sourceRect);
            Surface dstWindow = this.CreateWindow(dstRect);
            dstWindow.CopySurface(sourceWindow);

            dstWindow.Dispose();
            sourceWindow.Dispose();
        }

        /// <summary>
        /// Copies the contents of the given RGB32_Surface to the upper left of this RGB32_Surface.
        /// </summary>
        /// <param name="source">The RGB32_Surface to copy pixels from.</param>
        /// <param name="sourceRoi">
        /// The region of the source to copy from. The upper left of this rectangle
        /// will be mapped to (0,0) on this RGB32_Surface.
        /// The source RGB32_Surface does not need to have the same dimensions as this RGB32_Surface. Clipping
        /// will be handled automatically. No resizing will be done.
        /// </param>
        public virtual void CopySurface(Surface source, Rectangle sourceRoi)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }
           
            sourceRoi.Intersect(source.Bounds);
            int copiedWidth = Math.Min(this.width, sourceRoi.Width);
            int copiedHeight = Math.Min(this.Height, sourceRoi.Height);

            if (copiedWidth == 0 || copiedHeight == 0)
            {
                return;
            }

            using (Surface src = source.CreateWindow(sourceRoi ))
            {
                CopySurface(src);
            }
        }

        /// <summary>
        /// Copies a rectangular region of the given RGB32_Surface to a specific location on this RGB32_Surface.
        /// </summary>
        /// <param name="source">The RGB32_Surface to copy pixels from.</param>
        /// <param name="dstOffset">The location on this RGB32_Surface to start copying pixels to.</param>
        /// <param name="sourceRoi">The region of the source RGB32_Surface to copy pixels from.</param>
        /// <remarks>
        /// sourceRoi.Location will be mapped to dstOffset.Location.
        /// The source RGB32_Surface does not need to have the same dimensions as this RGB32_Surface. Clipping
        /// will be handled automatically. No resizing will be done.
        /// </remarks>
        public virtual void CopySurface(Surface source, Point dstOffset, Rectangle sourceRoi)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }
           
            Rectangle dstRoi = new Rectangle(dstOffset, sourceRoi.Size);
            dstRoi.Intersect(Bounds);

            if (dstRoi.Height == 0 || dstRoi.Width == 0)
            {
                return;
            }

            sourceRoi.X += dstRoi.X - dstOffset.X;
            sourceRoi.Y += dstRoi.Y - dstOffset.Y;
            sourceRoi.Width = dstRoi.Width;
            sourceRoi.Height = dstRoi.Height;

            using (Surface src = source.CreateWindow(sourceRoi ))
            {
                CopySurface(src, dstOffset);
            }
        }

        /// <summary>
        /// Copies a region of the given RGB32_Surface to this RGB32_Surface.
        /// </summary>
        /// <param name="source">The RGB32_Surface to copy pixels from.</param>
        /// <param name="region">The region to clip copying to.</param>
        /// <remarks>
        /// The upper left corner of the source RGB32_Surface will be mapped to the upper left of this
        /// RGB32_Surface, and only those pixels that are defined by the region will be copied.
        /// The source RGB32_Surface does not need to have the same dimensions as this RGB32_Surface. Clipping
        /// will be handled automatically. No resizing will be done.
        /// </remarks>
        public virtual void CopySurface(Surface source, SciRegion region)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }
           
            Rectangle[] scans = region.GetRegionScansReadOnlyInt();
            for (int i = 0; i < scans.Length; ++i)
            {
                Rectangle rect = scans[i];

                rect.Intersect(this.Bounds);
                rect.Intersect(source.Bounds);

                if (rect.Width == 0 || rect.Height == 0)
                {
                    continue;
                }

                unsafe
                {
                    for (int y = rect.Top; y < rect.Bottom; ++y)
                    {
                        Int32* dst = this.GetPointAddressUnchecked(rect.Left, y);
                        Int32* src = source.GetPointAddressUnchecked(rect.Left, y);
                        Memory.Copy(dst, src, (ulong)rect.Width * (ulong)this.PixelFormatSizeOf );
                    }
                }
            }
        }

        /// <summary>
        /// Copies a region of the given RGB32_Surface to this RGB32_Surface.
        /// </summary>
        /// <param name="source">The RGB32_Surface to copy pixels from.</param>
        /// <param name="region">The region to clip copying to.</param>
        /// <remarks>
        /// The upper left corner of the source RGB32_Surface will be mapped to the upper left of this
        /// RGB32_Surface, and only those pixels that are defined by the region will be copied.
        /// The source RGB32_Surface does not need to have the same dimensions as this RGB32_Surface. Clipping
        /// will be handled automatically. No resizing will be done.
        /// </remarks>
        public virtual void CopySurface(Surface source, Rectangle[] region, int startIndex, int length)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }
           
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Rectangle rect = region[i];

                rect.Intersect(this.Bounds);
                rect.Intersect(source.Bounds);

                if (rect.Width == 0 || rect.Height == 0)
                {
                    continue;
                }
                if (this.ColorPixelBase.GetType() == source.ColorPixelBase.GetType())
                {
                    unsafe
                    {
                        for (int y = rect.Top; y < rect.Bottom; ++y)
                        {
                            Int32* dst = this.GetPointAddressUnchecked(rect.Left, y);
                            Int32* src = source.GetPointAddressUnchecked(rect.Left, y);
                            Memory.Copy(dst, src, (ulong)rect.Width * (ulong)PixelFormatSizeOf);
                        }
                    }
                }
                else
                {
                    for (int y = rect.Top; y < rect.Bottom; ++y)
                    {
                        for (int x = rect.Left; x < rect.Right; ++x)
                        {
                            this.SetPoint(x, y, source.GetPoint(x, y));
                        }
                    }

                }
            }
        }

        public virtual void CopySurface(Surface source, Rectangle[] region)
        {
            CopySurface(source, region, 0, region.Length);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a new RGB32_Surface with the same dimensions and pixel values as this one.
        /// </summary>
        /// <returns>A new RGB32_Surface that is a clone of the current one.</returns>
        public virtual Surface Clone()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }

            Surface ret = new Surface(this.Size,this.ColorPixelBase  );
            
                ret.CopySurface(this);
           
                return ret;
        }
        #endregion

        #region ClearSurface
        /// <summary>
        /// Clears the RGB32_Surface to all-white (BGRA = [255,255,255,255]).
        /// </summary>
        public void Clear()
        {
            Clear(_ColorPixelBase.ClearColor());
        }

        /// <summary>
        /// Clears the RGB32_Surface to the given color value.
        /// </summary>
        /// <param name="color">The color value to fill the RGB32_Surface with.</param>
        public virtual void Clear(ColorPixelBase  color)
        {
            new Constant(color).Apply(this, this.Bounds);
        }

     
        /// <summary>
        /// Clears the given rectangular region within the RGB32_Surface to the given color value.
        /// </summary>
        /// <param name="color">The color value to fill the rectangular region with.</param>
        /// <param name="rect">The rectangular region to fill.</param>
        public virtual  void Clear(Rectangle rect, ColorPixelBase color)
        {
            Rectangle rect2 = Rectangle.Intersect(this.Bounds, rect);

            if (rect2 != rect)
            {
                throw new ArgumentOutOfRangeException("rectangle is out of bounds");
            }

            new Constant(color).Apply(this, rect);
        }

        public virtual void Clear(SciRegion region, ColorPixelBase color)
        {
            foreach (Rectangle rect in region.GetRegionScansReadOnlyInt())
            {
                Clear(rect, color);
            }
        }

        public virtual  void ClearWithCheckboardPattern()
        {
            unsafe
            {
                for (int y = 0; y < this.height; ++y)
                {
                    Int32 * dstPtr = GetRowAddressUnchecked(y);

                    for (int x = 0; x < this.width; ++x)
                    {
                        byte v = (byte)((((x ^ y) & 8) * 8) + 191);
                        *dstPtr =_ColorPixelBase.FromBgra(v, v, v, 255).ToInt32();
                        ++dstPtr;
                    }
                }
            }
        }
        #endregion

        private MemoryBlock GetRootMemoryBlock(MemoryBlock block)
        {
            MemoryBlock p = block;

            while (p.Parent != null)
            {
                p = p.Parent;
            }

            return p;
        }

        public void GetDrawBitmapInfo(out IntPtr bitmapHandle, out Point childOffset, out Size parentSize)
        {
            MemoryBlock rootBlock = GetRootMemoryBlock(this.scan0);
            long childOffsetBytes = this.scan0.Pointer.ToInt64() - rootBlock.Pointer.ToInt64();
            int childY = (int)(childOffsetBytes / this.stride);
            int childX = (int)((childOffsetBytes - (childY * this.stride)) / PixelFormatSizeOf );
            childOffset = new Point(childX, childY);
            parentSize = new Size(this.stride / PixelFormatSizeOf, childY + this.height);
            bitmapHandle = rootBlock.BitmapHandle;
        }

        /// <summary>
        /// Releases all resources held by this Surface object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    scan0.Dispose();
                    scan0 = null;
                }
            }
        }
    }
}
