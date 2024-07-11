using SciImage.SystemLayer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SciImage
{
    public class IntensitySurface2 : Surface ,ICloneable 
    {

        public override Surface Clone()
        {
            IntensitySurface2 ins=new IntensitySurface2(width,height,RStride,RBPP,RlSize,RScan0.Pointer,RMaxContrast,RMinContrast);
            return (Surface)ins;
        }

        public int RBPP;
        public int RStride;
        public int RlSize;
        public long RMaxContrast=-1;
        public long RMinContrast=-1;
        private MemoryBlock RScan0 ;

        public IntPtr RawDataBlock
        {

            get
            {
                unsafe
                {
                    
                   return  (IntPtr)RScan0.VoidStar;
                }


            }
        }
        public void SetRawData( int Stride, int BPP, int lSize, IntPtr RawData,long  MaxContrast,long  MinContrast)
        {
            RMaxContrast = MaxContrast;
            RMinContrast = MinContrast;
            RStride = Stride;
            RBPP = BPP;
            RlSize = lSize;
            RScan0 = new MemoryBlock(RawData, RlSize);
            DrawSurfaceWithLUT();
        }
        public void GetRawData(out int Stride, out int BPP, out int lSize, out IntPtr RawData)
        {
            unsafe
            {
                Stride = RStride;
                BPP = RBPP;
                lSize = RlSize;
                RawData = (IntPtr)RScan0.VoidStar;
            }

        }
        public bool GetHistogram(out int[] histogram, out long MaxValue, out long MinValue, out long MaxPossible)
        {
            long Bins;
            if ((RBPP   == 16))
            {
                Bins = 1000;
                MaxPossible = (long)Math.Pow(2, 16) + 1;
                double BinSize = (double)(MaxPossible) / (double)Bins;
                histogram = new int[Bins];
                MaxValue = 0;
                MinValue = 100000;
                
                unsafe
                {
                    int wStart = (int)(width * 0.2);
                    int hStart=(int)( height*0.2);
                    int nHeight = (int)(.8 * height);
                    int nWidth = (int)(.8 * width);
                    for (int y =hStart ; y < nHeight ; y++)
                    {
                        ushort* inData = (ushort*)((byte*)RScan0.VoidStar   + (y * RStride  ));
                        // System.Diagnostics.Debug.Print(y.ToString());
                        for (int x = wStart ; x < nWidth; x++)
                        {

                            ushort gray = (ushort)inData[x];
                            if (gray > MaxValue) MaxValue = gray;
                            if (gray < MinValue) MinValue = gray;
                            gray = (ushort)(gray / BinSize);
                            histogram[gray]++;
                        }
                    }
                }

            }
            else
            {
                Bins = 257;
                
                histogram = new int[Bins - 1];
                MaxValue = 0;
                MinValue = 100000;
                MaxPossible = 257;
                unsafe
                {

                    for (int y = 0; y < height; y++)
                    {
                        byte* inData = (byte*)RScan0.VoidStar   + (y * RStride );

                        // System.Diagnostics.Debug.Print(y.ToString());
                        for (int x = 0; x < width; x++)
                        {
                            byte gray = inData[x];
                            if (gray > MaxValue) MaxValue = gray;
                            if (gray < MinValue) MinValue = gray;
                            histogram[gray]++;

                        }

                    }
                }
            }

            return true;
        }

        private void DrawSurfaceWithLUT()
        {
                //uint* srcPtr = (uint*)((byte*)bData.Scan0.ToPointer() + (y * bData.Stride));
                //ColorBgra* dstPtr = layer.Surface.GetRowAddress(y);
               double MaxContrastValue ;
               double MinContrastValue ;
               long MaxPossible = (long)Math.Pow(2,RBPP)-2;
                if (RMaxContrast == -1 && RMinContrast == -1)
                {
                    int[] hist;
                    
                    GetHistogram(out hist, out  RMaxContrast, out  RMinContrast, out  MaxPossible);
                    
                }
                MaxContrastValue = (double)RMaxContrast / (double ) MaxPossible;
                MinContrastValue = (double)RMinContrast / (double)MaxPossible;
                
                int PixelSize = 4;
                if (RBPP  == 16)
                {
                    double ValueLength = (RMaxContrast - RMinContrast);
                    if (ValueLength == 0) ValueLength = 1;
                    double b = RMinContrast;
                    unsafe
                    {
                        for (int y = 0; y < height; y++)
                        {
                            ushort* inData = (ushort*)((byte*)RScan0.VoidStar  + (y * RStride ));

                            //IntPtr lineP = FreeImage.GetScanLine(dib, y);
                            //ushort* inData = (ushort*)(lineP);
                            //ColorBgra* dstPtr = layer.Surface.GetRowAddress(y);
                            byte* row = (byte*)GetRowAddress(y);

                            // System.Diagnostics.Debug.Print(y.ToString());
                            for (int x = 0; x < width; x++)
                            {
                                long   gray = (long )(inData[x]);
                                long grayT = (long)((gray - RMinContrast) / ValueLength * MaxPossible);
                                if (grayT > 65535) grayT = 65535;
                                if (grayT < 0) grayT = 0;
                                
                                byte GrayB = (byte)(grayT/257);

                              
                                row[x * PixelSize] = GrayB ;
                                row[x * PixelSize + 1] = GrayB ;
                                row[x * PixelSize + 2] = GrayB;
                                row[x * PixelSize + 3] = 255;
                                
                                
                                

                            }
                        }
                    }
            }
            else 
            {
                
                //double slope = (1 / ((double)RMaxContrast - (double)RMinContrast));
                //double b = -1 * slope * MinContrastValue*257 ;
                double ValueLength = (RMaxContrast  - RMinContrast );
                if (ValueLength == 0) ValueLength = 1;
                double b = RMinContrast ;

                unsafe
                {
                    
                    for (int y = 0; y < height; y++)
                    {
                        byte* inData = (byte*)RScan0.VoidStar  + (y * RStride );
                        byte* row = (byte*)GetRowAddress(y);


                        for (int x = 0; x < Width; x++)
                        {
                            byte gray = (byte )(inData[x]);
                            long grayT=(long)( (gray - RMinContrast )/ValueLength  * MaxPossible );
                            if (grayT > 255) grayT = (byte)255;
                            if (grayT < 0) grayT = 0;
                            gray = (byte)grayT;

                            
                            row[x * PixelSize ] =  gray;
                            row[x * PixelSize + 1] = gray;
                            row[x * PixelSize + 2] =  gray;
                            row[x * PixelSize + 3] = 255;
                            
                        }

                    }
                }
            }
                
        }
        public void RefreshVisibleImage()
        {
            DrawSurfaceWithLUT();
        }

        private unsafe int GetRawPointUnchecked(int x, int y)
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
            int Intensity = 0;
            if (RBPP == 16)
            {
                ushort* address=unchecked(x + (ushort*)(((byte*)RScan0.VoidStar) + (y *RStride)));
                Intensity = (int)(address[0]);
                 return Intensity;
            }
            else
            {
                byte * address = unchecked(x + (byte *)(((byte*)RScan0.VoidStar) + (y *RStride)));
                Intensity = (int)(address[0]);
                return Intensity;
            }
            
        }
        public int  getIntensity(int x, int y)
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
                    return GetRawPointUnchecked(x, y);
                }
            

           /* set
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
            }*/
        }
        public IntensitySurface2(int width, int height, int RawStride, int RawBPP, int RawlSize, IntPtr RawData, long MaxContrast,long MinContrast )
            : base(width, height)
        {

            
            SetRawData(RawStride, RawBPP, RawlSize, RawData, MaxContrast ,MinContrast );

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
        public virtual unsafe long GetRawRowByteOffsetUnchecked(int y)
        {
#if DEBUG
            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return (long)y * (long)RStride;
        }

        /// <summary>
        /// Gets the offset, in bytes, of the requested row from the start of the RGB32_Surface.
        /// </summary>
        /// <param name="y">The row.</param>
        /// <returns>The number of bytes between (0,0) and (0,y).</returns>
        public long GetRawRowByteOffset(int y)
        {
            if (y < 0 || y >= height)
            {
                throw new ArgumentOutOfRangeException("y", "Out of bounds: y=" + y.ToString());
            }

            return (long)y * (long)RStride ;
        }
        /// <summary>
        /// Gets a pointer to the beginning of the requested row in the RGB32_Surface.
        /// </summary>
        /// <param name="y">The row</param>
        /// <returns>A pointer that references (0,y) in this RGB32_Surface.</returns>
        /// <remarks>Since this returns a pointer, it is potentially unsafe to use.</remarks>
        public virtual unsafe void * GetRawRowAddress(int y)
        {
            return (byte *)(((byte*)RScan0.VoidStar ) + GetRawRowByteOffset(y));
        }


        public override void ReplaceImage(Image image)
        {
            base.ReplaceImage(image);
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
        public virtual unsafe void * GetRawRowAddressUnchecked(int y)
        {
#if DEBUG
            if (y < 0 || y >= this.height)
            {
                Tracing.Ping("y=" + y.ToString() + " is out of bounds of [0, " + this.height.ToString() + ")");
            }
#endif

            return (void *)(((byte*)scan0.VoidStar) + GetRawRowByteOffsetUnchecked(y));
        }
        /// <summary>
        /// Creates a new instance of the Surface class.
        /// </summary>
        /// <param name="size">The size, in pixels, of the new Surface.</param>
        public IntensitySurface2(System.Drawing.Size size)
            : base(size.Width, size.Height)
        {
        }
        /// <summary>
        /// Creates a new instance of the Surface class.
        /// </summary>
        /// <param name="width">The width, in pixels, of the new Surface.</param>
        /// <param name="height">The height, in pixels, of the new Surface.</param>
        public IntensitySurface2(int width, int height)
        {
            int stride;
            long bytes;

            try
            {
                stride = checked(width * ColorBgra.SizeOf);
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
        /// Creates a new instance of the Surface class that reuses a block of memory that was previously allocated.
        /// </summary>
        /// <param name="width">The width, in pixels, for the Surface.</param>
        /// <param name="height">The height, in pixels, for the Surface.</param>
        /// <param name="stride">The stride, in bytes, for the Surface.</param>
        /// <param name="scan0">The MemoryBlock to use. The beginning of this buffer defines the upper left (0, 0) pixel of the Surface.</param>
        private IntensitySurface2(int width, int height, int stride, MemoryBlock scan0)
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

        ~IntensitySurface2()
        {
            Dispose(false);
            try
            {
                RScan0.Dispose();
            }
            catch { }

        }

    }
}
