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
using SciImage.Core.ColorsAndPixelOps;

namespace SciImage
{
    /// <summary>
    /// This is our Surface type. We allocate our own blocks of memory for this,
    /// and provide ways to create a GDI+ Bitmap object that aliases our RGB32_Surface.
    /// That way we can do everything fast, in memory and have complete control,
    /// and still have the ability to use GDI+ for drawing and rendering where
    /// appropriate.
    /// </summary>
    [Serializable]
    public sealed class FISurface:Surface
    {


        /// <summary>
        /// Creates a new instance of the Surface class.
        /// </summary>
        /// <param name="size">The size, in pixels, of the new Surface.</param>
        public FISurface(Size size)
            : this(size.Width, size.Height)
        {
        }

        /// <summary>
        /// Creates a new instance of the Surface class.
        /// </summary>
        /// <param name="width">The width, in pixels, of the new Surface.</param>
        /// <param name="height">The height, in pixels, of the new Surface.</param>
        public FISurface(int width, int height)
        {
            int stride;
            long bytes;

            try
            {
                stride = checked(width * ColorPixelBase.SizeOf);
                bytes = (long)height * (long)stride;
            }

            catch (OverflowException ex)
            {
                throw new OutOfMemoryException("Dimensions are too large - not enough memory, width=" + width.ToString() + ", height=" + height.ToString(), ex);
            }

            MemoryBlock scan0 = new MemoryBlock(width, height,32);
            Create(width, height, stride, scan0);
        }

        /// <summary>
        /// Creates a new instance of the Surface class that reuses a block of memory that was previously allocated.
        /// </summary>
        /// <param name="width">The width, in pixels, for the Surface.</param>
        /// <param name="height">The height, in pixels, for the Surface.</param>
        /// <param name="stride">The stride, in bytes, for the Surface.</param>
        /// <param name="scan0">The MemoryBlock to use. The beginning of this buffer defines the upper left (0, 0) pixel of the Surface.</param>
        private FISurface(int width, int height, int stride, MemoryBlock scan0)
        {
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

        ~FISurface()
        {
            Dispose(false);
        }

        
      /*  public Surface CreateWindow(int x, int y, int windowWidth, int windowHeight)
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

            long offset = ((long)stride * (long)y) + ((long)ColorPixelBase.SizeOf * (long)x);
            long length = ((windowHeight - 1) * (long)stride) + (long)windowWidth * (long)ColorPixelBase.SizeOf;
            MemoryBlock block = new MemoryBlock(this.scan0, offset, length);
            return (Surface)(new FISurface(windowWidth, windowHeight, this.stride, block));
        }*/


        /// <summary>
        /// Gets the offset, in bytes, of the requested row from the start of the RGB32_Surface.
        /// </summary>
        /// <param name="y">The row.</param>
        /// <returns>The number of bytes between (0,0) and (0,y)</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetRowByteOffset().
        /// </remarks>
        /*public override unsafe long GetRowByteOffsetUnchecked(int y)
        {
#if DEBUG
            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return (long)y * (long)stride;
        }*/

        /// <summary>
        /// Gets a pointer to the beginning of the requested row in the RGB32_Surface.
        /// </summary>
        /// <param name="y">The row</param>
        /// <returns>A pointer that references (0,y) in this RGB32_Surface.</returns>
        /// <remarks>Since this returns a pointer, it is potentially unsafe to use.</remarks>
        //public override  unsafe ColorPixelBase *GetRowAddress(int y)
        //{
        //    return (ColorPixelBase *)(((byte *)scan0.VoidStar) + GetRowByteOffset(y));
        //}

        /// <summary>
        /// Gets a pointer to the beginning of the requested row in the RGB32_Surface.
        /// </summary>
        /// <param name="y">The row</param>
        /// <returns>A pointer that references (0,y) in this RGB32_Surface.</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetRowAddress().
        /// </remarks>
        /*public override  unsafe ColorPixelBase *GetRowAddressUnchecked(int y)
        {
#if DEBUG
            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return (ColorPixelBase *)(((byte *)scan0.VoidStar) + GetRowByteOffsetUnchecked(y));
        }*/

        /// <summary>
        /// Gets the number of bytes from the beginning of a row to the requested column.
        /// </summary>
        /// <param name="x">The column.</param>
        /// <returns>
        /// The number of bytes between (0,n) and (x,n) where n is in the range [0, Height).
        /// </returns>
        /*public override long GetColumnByteOffset(int x)
        {
            if (x < 0 || x >= this.width)
            {
                throw new ArgumentOutOfRangeException("x", x, "Out of bounds");
            }

            return (long)x * (long)ColorPixelBase.SizeOf;
        }*/

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
        public  override long GetColumnByteOffsetUnchecked(int x)
        {
#if DEBUG
            if (x < 0 || x >= this.width)
            {
                Tracing.Ping("x=" + x.ToString() + " is out of bounds of [0, " + this.width.ToString() + ")");
            }
#endif

            return (long)x * (long)ColorPixelBase.SizeOf;
        }


        /// <summary>
        /// Gets the color at a specified point in the RGB32_Surface.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns>The color at the requested location.</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetPoint().
        /// </remarks>
        /*public override  unsafe ColorPixelBase GetPointUnchecked(int x, int y)
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

            return *(x + (ColorPixelBase *)(((byte *)scan0.VoidStar) + (y * stride)));
        }*/




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
        /*public override unsafe ColorPixelBase *GetPointAddressUnchecked(int x, int y)
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

            return unchecked(x + (ColorPixelBase *)(((byte *)scan0.VoidStar) + (y * stride)));
        }*/

        /// <summary>
        /// Gets the address in memory of the requested point.
        /// </summary>
        /// <param name="pt">The point to retrieve.</param>
        /// <returns>A pointer to the requested point in the RGB32_Surface.</returns>
        /// <remarks>
        /// This method does not do any bounds checking and is potentially unsafe to use,
        /// but faster than GetPointAddress().
        /// </remarks>
        /*public override  unsafe ColorPixelBase *GetPointAddressUnchecked(Point pt)
        {
            return GetPointAddressUnchecked(pt.X, pt.Y);
        }*/

        /// <summary>
        /// Gets a MemoryBlock that references the row requested.
        /// </summary>
        /// <param name="y">The row.</param>
        /// <returns>A MemoryBlock that gives access to the bytes in the specified row.</returns>
        /// <remarks>This method is the safest to use for direct memory access to a row's pixel data.</remarks>
        /*public override MemoryBlock GetRow(int y)
        {
            return new MemoryBlock(scan0, GetRowByteOffset(y), (long)width * (long)ColorPixelBase.SizeOf);
        }*/


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
        public override Bitmap CreateAliasedBitmap(Rectangle bounds, bool alpha)
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
                return new Bitmap(bounds.Width, bounds.Height, stride, alpha ? this.PixelFormat : PixelFormat.Format32bppRgb, 
                    new IntPtr((void *)((byte *)scan0.VoidStar + GetPointByteOffsetUnchecked(bounds.X, bounds.Y))));
            }
        }


        /// <summary>
        /// Copies the contents of the given RGB32_Surface to the upper left corner of this RGB32_Surface.
        /// </summary>
        /// <param name="source">The RGB32_Surface to copy pixels from.</param>
        /// <remarks>
        /// The source RGB32_Surface does not need to have the same dimensions as this RGB32_Surface. Clipping
        /// will be handled automatically. No resizing will be done.
        /// </remarks>
        public override void CopySurface(Surface source)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }

            if (this.stride == source.Stride &&
                (this.width * ColorPixelBase.SizeOf) == this.stride &&
                this.width == source.Width &&
                this.height == source.Height)
            {
                unsafe
                {
                    Memory.Copy(this.scan0.VoidStar, 
                                source.Scan0.VoidStar, 
                                ((ulong)(height - 1) * (ulong)stride) + ((ulong)width * (ulong)ColorPixelBase.SizeOf));
                }
            }
            else
            {
                int copyWidth = Math.Min(width, source.Width);
                int copyHeight = Math.Min(height, source.Height);

                unsafe
                {
                    for (int y = 0; y < copyHeight; ++y)
                    {
                        Memory.Copy(GetRowAddressUnchecked(y), source.GetRowAddressUnchecked(y), (ulong)copyWidth * (ulong)ColorPixelBase.SizeOf);
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
        public override void CopySurface(Surface source, Point dstOffset)
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
        public override void CopySurface(Surface source, Rectangle sourceRoi)
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

            using (Surface src = source.CreateWindow(sourceRoi))
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
        public override void CopySurface(Surface source, Point dstOffset, Rectangle sourceRoi)
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

            using (Surface src = source.CreateWindow(sourceRoi))
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
        public override void CopySurface(Surface source, PdnRegion region)
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
                        ColorPixelBase *dst = this.GetPointAddressUnchecked(rect.Left, y);
                        ColorPixelBase *src = source.GetPointAddressUnchecked(rect.Left, y);
                        Memory.Copy(dst, src, (ulong)rect.Width * (ulong)ColorPixelBase.SizeOf);
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
        public override void CopySurface(Surface source, Rectangle[] region, int startIndex, int length)
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

                unsafe
                {
                    for (int y = rect.Top; y < rect.Bottom; ++y)
                    {
                        ColorPixelBase* dst = this.GetPointAddressUnchecked(rect.Left, y);
                        ColorPixelBase* src = source.GetPointAddressUnchecked(rect.Left, y);
                        Memory.Copy(dst, src, (ulong)rect.Width * (ulong)ColorPixelBase.SizeOf);
                    }
                }
            }
        }

        public override void CopySurface(Surface source, Rectangle[] region)
        {
            CopySurface(source, region, 0, region.Length);
        }

        /// <summary>
        /// Creates a new RGB32_Surface with the same dimensions and pixel values as this one.
        /// </summary>
        /// <returns>A new RGB32_Surface that is a clone of the current one.</returns>
        public override Surface Clone()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Surface");
            }

            Surface ret = new Surface(this.Size);
            ret.CopySurface(this);
            return ret;
        }

        /// <summary>
        /// Clears the RGB32_Surface to the given color value.
        /// </summary>
        /// <param name="color">The color value to fill the RGB32_Surface with.</param>
        public override void Clear(ColorPixelBase color)
        {
            new UnaryPixelOps.Constant(color).Apply(this, this.Bounds);
        }

        
        /// <summary>
        /// Clears the given rectangular region within the RGB32_Surface to the given color value.
        /// </summary>
        /// <param name="color">The color value to fill the rectangular region with.</param>
        /// <param name="rect">The rectangular region to fill.</param>
        public override void Clear(Rectangle rect, ColorPixelBase color)
        {
            Rectangle rect2 = Rectangle.Intersect(this.Bounds, rect);

            if (rect2 != rect)
            {
                throw new ArgumentOutOfRangeException("rectangle is out of bounds");
            }

            new UnaryPixelOps.Constant(color).Apply(this, rect);
        }

        public override void Clear(PdnRegion region, ColorPixelBase color)
        {
            foreach (Rectangle rect in region.GetRegionScansReadOnlyInt())
            {
                Clear(rect, color);
            }
        }

        public override void ClearWithCheckboardPattern()
        {
            unsafe
            {
                for (int y = 0; y < this.height; ++y)
                {
                    ColorPixelBase* dstPtr = GetRowAddressUnchecked(y);

                    for (int x = 0; x < this.width; ++x)
                    {
                        byte v = (byte)((((x ^ y) & 8) * 8) + 191);
                        *dstPtr = ColorPixelBase.FromBgra(v, v, v, 255);
                        ++dstPtr;
                    }
                }
            }
        }


        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using super sampling. If the source RGB32_Surface is less wide
        /// or less tall than this RGB32_Surface (i.e. magnification), bicubic resampling is used instead. If either
        /// the source or destination has a dimension that is only 1 pixel, nearest neighbor is used.
        /// </summary>
        /// <param name="source">The RGB32_Surface to read pixels from.</param>
        /// <param name="dstRoi">The rectangle to clip rendering to.</param>
        /// <remarks>This method was implemented with correctness, not performance, in mind.</remarks>
        public override void SuperSamplingFitSurface(Surface source, Rectangle dstRoi)
        {
            if (source.Width == Width && source.Height == Height)
            {
                CopySurface(source);
            }
            else if (source.Width <= Width || source.Height <= Height)
            {
                if (source.Width < 2 || source.Height < 2 || this.width < 2 || this.height < 2)
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
                    double srcTop = (double)(dstY * source.Height) / (double)height;
                    double srcTopFloor = Math.Floor(srcTop);
                    double srcTopWeight = 1 - (srcTop - srcTopFloor);
                    int srcTopInt = (int)srcTopFloor;

                    double srcBottom = (double)((dstY + 1) * source.Height) / (double)height;
                    double srcBottomFloor = Math.Floor(srcBottom - 0.00001);
                    double srcBottomWeight = srcBottom - srcBottomFloor;
                    int srcBottomInt = (int)srcBottomFloor;

                    ColorPixelBase *dstPtr = this.GetPointAddressUnchecked(dstRoi2.Left, dstY);

                    for (int dstX = dstRoi2.Left; dstX < dstRoi2.Right; ++dstX)
                    {
                        double srcLeft = (double)(dstX * source.Width) / (double)width;
                        double srcLeftFloor = Math.Floor(srcLeft);
                        double srcLeftWeight = 1 - (srcLeft - srcLeftFloor);
                        int srcLeftInt = (int)srcLeftFloor;

                        double srcRight = (double)((dstX + 1) * source.Width) / (double)width;
                        double srcRightFloor = Math.Floor(srcRight - 0.00001);
                        double srcRightWeight = srcRight - srcRightFloor;
                        int srcRightInt = (int)srcRightFloor;

                        double blueSum = 0;
                        double greenSum = 0;
                        double redSum = 0;
                        double alphaSum = 0;

                        // left fractional edge
                        ColorPixelBase *srcLeftPtr = source.GetPointAddressUnchecked(srcLeftInt, srcTopInt + 1);

                        for (int srcY = srcTopInt + 1; srcY < srcBottomInt; ++srcY)
                        {
                            double a = srcLeftPtr->A;
                            blueSum += srcLeftPtr->B * srcLeftWeight * a;
                            greenSum += srcLeftPtr->G * srcLeftWeight * a;
                            redSum += srcLeftPtr->R * srcLeftWeight * a;
                            alphaSum += srcLeftPtr->A * srcLeftWeight;
                            srcLeftPtr = (ColorPixelBase*)((byte*)srcLeftPtr + source.Stride);
                        }

                        // right fractional edge
                        ColorPixelBase *srcRightPtr = source.GetPointAddressUnchecked(srcRightInt, srcTopInt + 1);
                        for (int srcY = srcTopInt + 1; srcY < srcBottomInt; ++srcY)
                        {
                            double a = srcRightPtr->A;
                            blueSum += srcRightPtr->B * srcRightWeight * a;
                            greenSum += srcRightPtr->G * srcRightWeight * a;
                            redSum += srcRightPtr->R * srcRightWeight * a;
                            alphaSum += srcRightPtr->A * srcRightWeight;
                            srcRightPtr = (ColorPixelBase*)((byte*)srcRightPtr + source.Stride);
                        }

                        // top fractional edge
                        ColorPixelBase *srcTopPtr = source.GetPointAddressUnchecked(srcLeftInt + 1, srcTopInt);
                        for (int srcX = srcLeftInt + 1; srcX < srcRightInt; ++srcX)
                        {
                            double a = srcTopPtr->A;
                            blueSum += srcTopPtr->B * srcTopWeight * a;
                            greenSum += srcTopPtr->G * srcTopWeight * a;
                            redSum += srcTopPtr->R * srcTopWeight * a;
                            alphaSum += srcTopPtr->A * srcTopWeight;
                            ++srcTopPtr;
                        }

                        // bottom fractional edge
                        ColorPixelBase *srcBottomPtr = source.GetPointAddressUnchecked(srcLeftInt + 1, srcBottomInt);
                        for (int srcX = srcLeftInt + 1; srcX < srcRightInt; ++srcX)
                        {
                            double a = srcBottomPtr->A;
                            blueSum += srcBottomPtr->B * srcBottomWeight * a;
                            greenSum += srcBottomPtr->G * srcBottomWeight * a;
                            redSum += srcBottomPtr->R * srcBottomWeight * a;
                            alphaSum += srcBottomPtr->A * srcBottomWeight;
                            ++srcBottomPtr;
                        }

                        // center area
                        for (int srcY = srcTopInt + 1; srcY < srcBottomInt; ++srcY)
                        {
                            ColorPixelBase *srcPtr = source.GetPointAddressUnchecked(srcLeftInt + 1, srcY);

                            for (int srcX = srcLeftInt + 1; srcX < srcRightInt; ++srcX)
                            {
                                double a = srcPtr->A;
                                blueSum += (double)srcPtr->B * a;
                                greenSum += (double)srcPtr->G * a;
                                redSum += (double)srcPtr->R * a;
                                alphaSum += (double)srcPtr->A;
                                ++srcPtr;
                            }
                        }

                        // four corner pixels
                        ColorPixelBase srcTL = source.GetPoint(srcLeftInt, srcTopInt);
                        double srcTLA = srcTL.A;
                        blueSum += srcTL.B * (srcTopWeight * srcLeftWeight) * srcTLA;
                        greenSum += srcTL.G * (srcTopWeight * srcLeftWeight) * srcTLA;
                        redSum += srcTL.R * (srcTopWeight * srcLeftWeight) * srcTLA;
                        alphaSum += srcTL.A * (srcTopWeight * srcLeftWeight);

                        ColorPixelBase srcTR = source.GetPoint(srcRightInt, srcTopInt);
                        double srcTRA = srcTR.A;
                        blueSum += srcTR.B * (srcTopWeight * srcRightWeight) * srcTRA;
                        greenSum += srcTR.G * (srcTopWeight * srcRightWeight) * srcTRA;
                        redSum += srcTR.R * (srcTopWeight * srcRightWeight) * srcTRA;
                        alphaSum += srcTR.A * (srcTopWeight * srcRightWeight);

                        ColorPixelBase srcBL = source.GetPoint(srcLeftInt, srcBottomInt);
                        double srcBLA = srcBL.A;
                        blueSum += srcBL.B * (srcBottomWeight * srcLeftWeight) * srcBLA;
                        greenSum += srcBL.G * (srcBottomWeight * srcLeftWeight) * srcBLA;
                        redSum += srcBL.R * (srcBottomWeight * srcLeftWeight) * srcBLA;
                        alphaSum += srcBL.A * (srcBottomWeight * srcLeftWeight);

                        ColorPixelBase srcBR = source.GetPoint(srcRightInt, srcBottomInt);
                        double srcBRA = srcBR.A;
                        blueSum += srcBR.B * (srcBottomWeight * srcRightWeight) * srcBRA;
                        greenSum += srcBR.G * (srcBottomWeight * srcRightWeight) * srcBRA;
                        redSum += srcBR.R * (srcBottomWeight * srcRightWeight) * srcBRA;
                        alphaSum += srcBR.A * (srcBottomWeight * srcRightWeight);

                        double area = (srcRight - srcLeft) * (srcBottom - srcTop);

                        double alpha = alphaSum / area;
                        double blue;
                        double green;
                        double red;

                        if (alpha == 0)
                        {
                            blue = 0;
                            green = 0;
                            red = 0;
                        }
                        else
                        {
                            blue = blueSum / alphaSum;
                            green = greenSum / alphaSum;
                            red = redSum / alphaSum;
                        }

                        // add 0.5 so that rounding goes in the direction we want it to
                        blue += 0.5;
                        green += 0.5;
                        red += 0.5;
                        alpha += 0.5;

                        dstPtr->Bgra = (uint)blue + ((uint)green << 8) + ((uint)red << 16) + ((uint)alpha << 24);
                        ++dstPtr;
                    }
                }
            }
        }


        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using nearest neighbor resampling.
        /// </summary>
        /// <param name="source">The RGB32_Surface to read pixels from.</param>
        /// <param name="dstRoi">The rectangle to clip rendering to.</param>
        public override void NearestNeighborFitSurface(Surface source, Rectangle dstRoi)
        {
            Rectangle roi = Rectangle.Intersect(dstRoi, this.Bounds);

            unsafe
            {
                for (int dstY = roi.Top; dstY < roi.Bottom; ++dstY)
                {
                    int srcY = (dstY * source.Height) / height;
                    ColorPixelBase *srcRow = source.GetRowAddressUnchecked(srcY);
                    ColorPixelBase *dstPtr = this.GetPointAddressUnchecked(roi.Left, dstY);

                    for (int dstX = roi.Left; dstX < roi.Right; ++dstX)
                    {
                        int srcX = (dstX * source.Width) / width;
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
        /// <param name="dstRoi">The rectangle to clip rendering to.</param>
        /// <remarks>
        /// This method was implemented with correctness, not performance, in mind. 
        /// Based on: "Bicubic Interpolation for Image Scaling" by Paul Bourke,
        ///           http://astronomy.swin.edu.au/%7Epbourke/colour/bicubic/
        /// </remarks>
        public override void BicubicFitSurface(Surface source, Rectangle dstRoi)
        {
            float leftF = (1 * (float)(width - 1)) / (float)(source.Width - 1);
            float topF = (1 * (height - 1)) / (float)(source.Height - 1);
            float rightF = ((float)(source.Width - 3) * (float)(width - 1)) / (float)(source.Width - 1);
            float bottomF = ((float)(source.Height - 3) * (float)(height - 1)) / (float)(source.Height - 1);

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

                if (rois[i].Width > 0  && rois[i].Height > 0)
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
        protected override  void BicubicFitSurfaceChecked(Surface source, Rectangle dstRoi)
        {
            if (this.width < 2 || this.height < 2 || source.Width < 2 || source.Height < 2)
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
                        double srcColumn = (double)(dstX * (source.Width - 1)) / (double)(width - 1);
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
                        double srcRow = (double)(dstY * (source.Height - 1)) / (double)(height - 1);
                        double srcRowFloor = (double)Math.Floor(srcRow);
                        double srcRowFrac = srcRow - srcRowFloor;
                        int srcRowInt = (int)srcRow;
                        ColorPixelBase *dstPtr = this.GetPointAddressUnchecked(roi.Left, dstY);

                        // Compute the R() values for this row
                        for (int n = -1; n <= 2; ++n)
                        {
                            double x = srcRowFrac - n;
                            rRowCache[n + 1] = R(x);
                        }

                        // See Perf Note below
                        //int nFirst = Math.Max(-srcRowInt, -1);
                        //int nLast = Math.Min(source.Height - srcRowInt - 1, 2);

                        for (int dstX = roi.Left; dstX < roi.Right; dstX++)
                        {
                            double srcColumn = (double)(dstX * (source.Width - 1)) / (double)(width - 1);
                            double srcColumnFloor = Math.Floor(srcColumn);
                            double srcColumnFrac = srcColumn - srcColumnFloor;
                            int srcColumnInt = (int)srcColumn;

                            double blueSum = 0;
                            double greenSum = 0;
                            double redSum = 0;
                            double alphaSum = 0;
                            double totalWeight = 0;

                            // See Perf Note below
                            //int mFirst = Math.Max(-srcColumnInt, -1);
                            //int mLast = Math.Min(source.Width - srcColumnInt - 1, 2);

                            ColorPixelBase *srcPtr = source.GetPointAddressUnchecked(srcColumnInt - 1, srcRowInt - 1);

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

                                        blueSum += srcPtr->B * w * srcPtr->A;
                                        greenSum += srcPtr->G * w * srcPtr->A;
                                        redSum += srcPtr->R * w * srcPtr->A;
                                        alphaSum += srcPtr->A * w;

                                        totalWeight += w;
                                    }

                                    ++srcPtr;
                                }

                                srcPtr = (ColorPixelBase *)((byte *)(srcPtr - 4) + source.Stride);
                            }

                            double alpha = alphaSum / totalWeight;
                            double blue;
                            double green;
                            double red;

                            if (alpha == 0)
                            {
                                blue = 0;
                                green = 0;
                                red = 0;
                            }
                            else
                            {
                                blue = blueSum / alphaSum;
                                green = greenSum / alphaSum;
                                red = redSum / alphaSum;

                                // add 0.5 to ensure truncation to uint results in rounding
                                alpha += 0.5;
                                blue += 0.5;
                                green += 0.5;
                                red += 0.5;
                            }

                            dstPtr->Bgra = (uint)blue + ((uint)green << 8) + ((uint)red << 16) + ((uint)alpha << 24);
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
        public override void BicubicFitSurfaceUnchecked(Surface source, Rectangle dstRoi)
        {
            if (this.width < 2 || this.height < 2 || source.Width < 2 || source.Height < 2)
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
                        double srcColumn = (double)(dstX * (source.Width - 1)) / (double)(width - 1);
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
                        double srcRow = (double)(dstY * (source.Height - 1)) / (double)(height - 1);
                        double srcRowFloor = Math.Floor(srcRow);
                        double srcRowFrac = srcRow - srcRowFloor;
                        int srcRowInt = (int)srcRow;
                        ColorPixelBase *dstPtr = this.GetPointAddressUnchecked(roi.Left, dstY);

                        // Compute the R() values for this row
                        for (int n = -1; n <= 2; ++n)
                        {
                            double x = srcRowFrac - n;
                            rRowCache[n + 1] = R(x);
                        }

                        rColCache = (double*)rColCacheIP.ToPointer();
                        ColorPixelBase *srcRowPtr = source.GetRowAddressUnchecked(srcRowInt - 1);

                        for (int dstX = roi.Left; dstX < roi.Right; dstX++)
                        {
                            double srcColumn = (double)(dstX * (source.Width - 1)) / (double)(width - 1);
                            double srcColumnFloor = Math.Floor(srcColumn);
                            double srcColumnFrac = srcColumn - srcColumnFloor;
                            int srcColumnInt = (int)srcColumn;

                            double blueSum = 0;
                            double greenSum = 0;
                            double redSum = 0;
                            double alphaSum = 0;
                            double totalWeight = 0;

                            ColorPixelBase *srcPtr = srcRowPtr + srcColumnInt - 1;
                            for (int n = 0; n <= 3; ++n)
                            {
                                double w0 = rColCache[0] * rRowCache[n];
                                double w1 = rColCache[1] * rRowCache[n];
                                double w2 = rColCache[2] * rRowCache[n];
                                double w3 = rColCache[3] * rRowCache[n];

                                double a0 = srcPtr[0].A;
                                double a1 = srcPtr[1].A;
                                double a2 = srcPtr[2].A;
                                double a3 = srcPtr[3].A;

                                alphaSum += (a0 * w0) + (a1 * w1) + (a2 * w2) + (a3 * w3);
                                totalWeight += w0 + w1 + w2 + w3;

                                blueSum += (a0 * srcPtr[0].B * w0) + (a1 * srcPtr[1].B * w1) + (a2 * srcPtr[2].B * w2) + (a3 * srcPtr[3].B * w3);
                                greenSum += (a0 * srcPtr[0].G * w0) + (a1 * srcPtr[1].G * w1) + (a2 * srcPtr[2].G * w2) + (a3 * srcPtr[3].G * w3);
                                redSum += (a0 * srcPtr[0].R * w0) + (a1 * srcPtr[1].R * w1) + (a2 * srcPtr[2].R * w2) + (a3 * srcPtr[3].R * w3);

                                srcPtr = (ColorPixelBase *)((byte *)srcPtr + source.Stride);
                            }

                            double alpha = alphaSum / totalWeight;

                            double blue;
                            double green;
                            double red;

                            if (alpha == 0)
                            {
                                blue = 0;
                                green = 0;
                                red = 0;
                            }
                            else
                            {
                                blue = blueSum / alphaSum;
                                green = greenSum / alphaSum;
                                red = redSum / alphaSum;

                                // add 0.5 to ensure truncation to uint results in rounding
                                alpha += 0.5;
                                blue += 0.5;
                                green += 0.5;
                                red += 0.5;
                            }

                            dstPtr->Bgra = (uint)blue + ((uint)green << 8) + ((uint)red << 16) + ((uint)alpha << 24);
                            ++dstPtr;
                            rColCache += 4;
                        } // for (dstX...
                    } // for (dstY...

                    Memory.Free(rColCacheIP);
                } // unsafe
            }
        }
         

        /// <summary>
        /// Fits the source RGB32_Surface to this RGB32_Surface using bilinear interpolation.
        /// </summary>
        /// <param name="source">The RGB32_Surface to read pixels from.</param>
        /// <param name="dstRoi">The rectangle to clip rendering to.</param>
        /// <remarks>This method was implemented with correctness, not performance, in mind.</remarks>
        public override void BilinearFitSurface(Surface source, Rectangle dstRoi)
        {
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
                        ColorPixelBase *dstRowPtr = this.GetRowAddressUnchecked(dstY);
                        float srcRow = (float)(dstY * (source.Height - 1)) / (float)(height - 1);

                        for (int dstX = roi.Left; dstX < roi.Right; dstX++)
                        {
                            float srcColumn = (float)(dstX * (source.Width - 1)) / (float)(width - 1);
                            *dstRowPtr = source.GetBilinearSample(srcColumn, srcRow);
                            ++dstRowPtr;
                        }
                    }
                }
            }
        }


        private new void Dispose(bool disposing)
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
