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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps
{
    /// <summary>
    /// This is our pixel format that we will work with. It is always 32-bits / 4-bytes and is
    /// always laid out in BGRA order.
    /// Generally used with the Surface class.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public class ColorIntensity : ColorPixelBase
    {

        [FieldOffset(0)]
        public UInt16 Intensity;
        
        
        [FieldOffset(1)]
        public byte LookupColor;

        [FieldOffset(3)]
        public byte A;

        /// <summary>
        /// Lets you change B, G, R, and A at the same time.
        /// </summary>
        [NonSerialized]
        [FieldOffset(0)]
        public uint Bgra;

        public byte alpha { get { return A; } set { A = value; } }

        public static List<Color> ColorLUT;

        public bool CompareColor(ColorPixelBase TestColor, int tolerance)
        {
            ColorIntensity a = (ColorIntensity)TestColor;
            int sum = 0;
            int diff;

            diff = a.Intensity   - Intensity ;
            sum += (1 + diff * diff) * a.A / Int16.MaxValue ;

            diff = a.LookupColor  - LookupColor ;
            sum += (1 + diff * diff) * a.A / 256;

            diff = a.A - A;
            sum += diff * diff;

            return (sum <= tolerance * tolerance * 4);
        }


        public int SizeOf
        {
            get { return 4; }
        }
       

        public int NumChannels { get { return 4; } }
        public Int32 GetChannel(int ChannelNumber)
        {
            return this[ChannelNumber];
        }
        public void SetChannel(int ChannelNumber, Int32 Value)
        {
            this[ChannelNumber] = (byte)Value;
        }
        public void ZeroPixel()
        {
            Bgra = 0;
        }

        public const int IntensityChannel = 0;
        public const int ColorChannel = 1;
        public const int AlphaChannel = 3;

        public const int SizeOf0 = 4;


        public ColorPixelBase InvertColor(ColorPixelBase c)
        {
            ColorIntensity color = (ColorIntensity)c;
            return FromIntensity((UInt16 )(UInt16.MaxValue  - color.Intensity ), (byte)(color.LookupColor ), (byte)color.A);
        }
        public ColorPixelBase InvertColorAndAlpha(ColorPixelBase c)
        {
            ColorIntensity color = (ColorIntensity)c;
            return FromIntensity((UInt16)(UInt16.MaxValue - color.Intensity), (byte)(color.LookupColor), (byte)(255 - color.A));
        }

        public ColorPixelBase TranslateColor(ColorPixelBase InputPixel)
        {
            return (ColorPixelBase) InputPixel.ToColorIntensity ();
        }
        public ColorPixelBase TranslateColor(Int32 inputLong)
        {
            return FromInt32(inputLong);

        }
        public ColorPixelBase TranslateColor(uint inputLong)
        {
            return FromInt32((Int32)inputLong);

        }
        public static ColorPixelBase FromInt32(Int32 color)
        {
            ColorIntensity c = new ColorIntensity();
            c.Bgra = (UInt32)color;
            return c;
        }

        public ColorPixelBase ParseHexString(string hexString)
        {
            uint value = Convert.ToUInt32(hexString, 16);
            return ColorIntensity.FromUInt32(value);
        }

        public string ToHexString()
        {
            int rgbNumber = this.Intensity ;
            string colorString = Convert.ToString(rgbNumber, 16);

            while (colorString.Length < 6)
            {
                colorString = "0" + colorString;
            }

            string alphaString = System.Convert.ToString(this.A, 16);

            while (alphaString.Length < 2)
            {
                alphaString = "0" + alphaString;
            }

            colorString = alphaString + colorString;

            return colorString.ToUpper();
        }

        /// <summary>
        /// Gets or sets the byte value of the specified color channel.
        /// </summary>
        public unsafe Int32 this[int channel]
        {
            get
            {
                if (channel < 0 || channel > 3)
                {
                    throw new ArgumentOutOfRangeException("channel", channel, "valid range is [0,3]");
                }

                if (channel ==0)
                    return Intensity ;
                else if (channel ==1)
                    return LookupColor ;
                else 
                    return A;
                
            }

            set
            {
                if (channel < 0 || channel > 3)
                {
                    throw new ArgumentOutOfRangeException("channel", channel, "valid range is [0,3]");
                }

                if (channel == 0)
                    Intensity = (UInt16)value;
                else if (channel == 1)
                    LookupColor = (byte)value;
                else
                    A = (byte)value;
            }
        }

        /// <summary>
        /// Gets the luminance intensity of the pixel based on the values of the red, green, and blue components. Alpha is ignored.
        /// </summary>
        /// <returns>A value in the range 0 to 1 inclusive.</returns>
        public double GetIntensity()
        {
            return (double)Intensity /(double)UInt16.MaxValue ;
        }
        /// <summary>
        /// Gets the luminance intensity of the pixel based on the values of the red, green, and blue components. Alpha is ignored.
        /// </summary>
        /// <returns>A value in the range 0 to 2^16 inclusive.</returns>
        public uint GetIntensity16Bit()
        {
            return (uint)(Intensity);
        }
       
        /// <summary>
        /// Gets the luminance intensity of the pixel based on the values of the red, green, and blue components. Alpha is ignored.
        /// </summary>
        /// <returns>A value in the range 0 to 255 inclusive.</returns>
        public byte GetIntensityByte()
        {
            return (byte)(GetIntensity() * 255);
        }

        public ColorPixelBase Clone()
        {
            ColorIntensity c = new ColorIntensity();
            c.A = A;
            c.LookupColor  = LookupColor ;
            c.Intensity  = Intensity ;
            return c;
        }
        public ColorPixelBase AnotherPixel()
        {
            ColorIntensity c = new ColorIntensity();
            c.A = 255;
            c.Intensity  = 0;
            c.LookupColor  = 0;
            return c;
        }
        public ColorPixelBase AverageChannels(ColorPixelBase InputColor)
        {
            ColorIntensity c = (ColorIntensity)InputColor;
           
            ColorIntensity c2 = new ColorIntensity();
            c2.A = c.A;
            c2.Intensity  = c.Intensity ;
            c2.LookupColor  = (byte)c.LookupColor ;
            
            return (c2);

        }
        /// <summary>
        /// Returns the maximum value out of the B, G, and R values. Alpha is ignored.
        /// </summary>
        /// <returns></returns>
        public byte GetMaxColorChannelValue()
        {
            return (byte) Intensity ;
        }

        /// <summary>
        /// Returns the average of the B, G, and R values. Alpha is ignored.
        /// </summary>
        /// <returns></returns>
        public byte GetAverageColorChannelValue()
        {
            return (byte)( GetIntensity()*255 );
        }

        /// <summary>
        /// Compares two ColorIntensity instance to determine if they are equal.
        /// </summary>
        public static bool operator ==(ColorIntensity lhs, ColorIntensity rhs)
        {
            return lhs.Bgra == rhs.Bgra;
        }

        /// <summary>
        /// Compares two ColorIntensity instance to determine if they are not equal.
        /// </summary>
        public static bool operator !=(ColorIntensity lhs, ColorIntensity rhs)
        {
            return lhs.Bgra != rhs.Bgra;
        }

        /// <summary>
        /// Compares two ColorIntensity instance to determine if they are equal.
        /// </summary>
        public override bool Equals(object obj)
        {

            if (obj != null && obj is ColorIntensity && ((ColorIntensity)obj).Bgra == this.Bgra)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this color value.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (int)Bgra;
            }
        }

        

        /// <summary>
        /// Returns a new ColorIntensity with the same color values but with a new alpha component value.
        /// </summary>
        public ColorPixelBase NewAlpha(byte newA)
        {
            return (ColorIntensity)FromIntensity(Intensity ,LookupColor , newA);
        }

        /// <summary>
        /// Creates a new ColorIntensity instance with the given color and alpha values.
        /// </summary>
        public static ColorIntensity sFromBgra(byte b, byte g, byte r, byte a)
        {
            ColorIntensity color = new ColorIntensity();
            color.Intensity = (UInt16 )(((0.114 * (double)b) + (0.587 * (double)g) + (0.299 * (double)r)) * UInt16.MaxValue);
            color.A = a;
            return color;
        }

        /// <summary>
        /// Creates a new ColorIntensity instance with the given color and alpha values.
        /// </summary>
        
        public ColorPixelBase FromBgra(byte b, byte g, byte r, byte a)
        {
            return sFromBgra(b, g, r, a);
        }

        /// <summary>
        /// Creates a new ColorIntensity instance with the given color and alpha values.
        /// </summary>
        public ColorPixelBase FromIntensity(int Intensity)
        {
            ColorIntensity color = new ColorIntensity();
            color.Bgra = (uint)Intensity ;
            return color;
        }
        /// <summary>
        /// Creates a new ColorIntensity instance with the given color and alpha values.
        /// </summary>
        public ColorPixelBase FromIntensity(UInt16  Intensity,byte Color, byte Alpha)
        {
            
            return ColorIntensity.sFromIntensity(Intensity,Color,Alpha);
        }
        public static  ColorPixelBase sFromIntensity(UInt16 Intensity, byte Color, byte Alpha)
        {
            ColorIntensity color = new ColorIntensity();

            color.Intensity = Intensity;
            color.LookupColor = Color;
            color.A = Alpha;
            return color;
        }
        
        public ColorPixelBase FromArray(int[] Channels, byte alpha)
        {
            return FromIntensity((UInt16)Channels[0], (byte)Channels[1], alpha);
        }
        public ColorPixelBase FromArray(long[] Channels, byte alpha)
        {
            return FromIntensity((UInt16)Channels[0], (byte)Channels[1], alpha);
        }
        public ColorPixelBase FromArray(byte[] Channels, byte alpha)
        {
            return FromIntensity((UInt16)Channels[0], (byte)Channels[1], alpha);
        }
        /// <summary>
        /// Creates a new ColorIntensity instance with the given color and alpha values.
        /// </summary>
        public ColorPixelBase FromBgraClamped(int b, int g, int r, int a)
        {
            return FromBgra(
                Utility.ClampToByte(b),
                Utility.ClampToByte(g),
                Utility.ClampToByte(r),
                Utility.ClampToByte(a));
        }

        /// <summary>
        /// Creates a new ColorIntensity instance with the given color and alpha values.
        /// </summary>
        public ColorPixelBase FromBgraClamped(float b, float g, float r, float a)
        {
            return FromBgra(
                Utility.ClampToByte(b),
                Utility.ClampToByte(g),
                Utility.ClampToByte(r),
                Utility.ClampToByte(a));
        }

        /// <summary>
        /// Packs color and alpha values into a 32-bit integer.
        /// </summary>
        public static UInt32 BgraToUInt32(byte b, byte g, byte r, byte a)
        {
            return (uint)(((0.114 * (double)b) + (0.587 * (double)g) + (0.299 * (double)r)) * UInt16.MaxValue ); 
        }
       

        /// <summary>
        /// Packs color and alpha values into a 32-bit integer.
        /// </summary>
        public static UInt32 BgraToUInt32(int b, int g, int r, int a)
        {
            return (uint)(((0.114 * (double)b) + (0.587 * (double)g) + (0.299 * (double)r)) * UInt16.MaxValue); 
        }

        /// <summary>
        /// Creates a new ColorIntensity instance with the given color values, and 255 for alpha.
        /// </summary>
        public ColorIntensity FromBgr(byte b, byte g, byte r)
        {
            return (ColorIntensity)FromBgra(b, g, r, 255);
        }

        /// <summary>
        /// Constructs a new ColorIntensity instance with the given 32-bit value.
        /// </summary>
        public static ColorIntensity FromUInt32(UInt32 bgra)
        {
            ColorIntensity color = new ColorIntensity();
            color.Bgra = bgra;
            return color;
        }

        /// <summary>
        /// Constructs a new ColorIntensity instance given a 32-bit signed integer that represents an R,G,B triple.
        /// Alpha will be initialized to 255.
        /// </summary>
        public static ColorIntensity FromOpaqueInt32(Int32 bgr)
        {
            if (bgr < 0 || bgr > 0xffffff)
            {
                throw new ArgumentOutOfRangeException("bgr", "must be in the range [0, 0xffffff]");
            }

            ColorIntensity color = new ColorIntensity();
            color.Bgra = (uint)bgr;
            color.A = 255;

            return color;
        }
        /// <summary>
        /// Gets the equivalent GDI+ PixelFormat.
        /// </summary>
        /// <remarks>
        /// This property always returns PixelFormat.Format32bppArgb.
        /// </remarks>
        public static PixelFormat PixelFormat
        {
            get
            {
                return PixelFormat.Format32bppArgb;
            }
        }
        public PixelFormat pixelFormat
        {
            get
            {
                return PixelFormat.Format32bppArgb;
            }
        }
        public static int ToOpaqueInt32(ColorIntensity color)
        {
            if (color.A != 255)
            {
                throw new InvalidOperationException("Alpha value must be 255 for this to work");
            }

            return (int)(color.Bgra & 0xffffff);
        }

        public Int64 ToInt64()
        {
            return (Int64)Bgra;
        }
        public Int32 ToInt32()
        {
            return (Int32)Bgra;
        }
        /// <summary>
        /// Constructs a new ColorIntensity instance from the values in the given Color instance.
        /// </summary>
        public ColorPixelBase FromColor(Color c)
        {
            return FromBgra(c.B, c.G, c.R, c.A);
        }

        /// <summary>
        /// Constructs a new ColorIntensity instance from the values in the given Color instance.
        /// </summary>
        public static ColorIntensity sFromColor(Color c)
        {
            return sFromBgra(c.B, c.G, c.R, c.A);
        }

        /// <summary>
        /// Converts this ColorIntensity instance to a Color instance.
        /// </summary>
        public Color ToColor()
        {
            byte R, G, B;
            Color LUT = Color.White ; //ColorLUT[LookupColor];
            double C = GetIntensity();
            R =(byte)( C * LUT.R);
            G =(byte)( C * LUT.G);
            B =(byte)( C * LUT.B);
            return Color.FromArgb(A, R, G, B);
        }


        public ColorBgra  ToColorBgra()
        {
            byte R, G, B;
            Color LUT = Color.White;
            double C = GetIntensity();
            R = (byte)(C * LUT.R);
            G = (byte)(C * LUT.G);
            B = (byte)(C * LUT.B);
            return ColorBgra.sFromBgra(B, G, R, A);
        }
        public ColorIntensity ToColorIntensity()
        {
            return this;
        }
        /// <summary>
        /// Smoothly blends between two colors.
        /// </summary>
        public ColorPixelBase Blend(ColorPixelBase ca, ColorPixelBase cb, byte cbAlpha)
        {
            uint caA = (uint)Utility.FastScaleByteByByte((byte)(255 - cbAlpha), ca.alpha);
            uint cbA = (uint)Utility.FastScaleByteByByte(cbAlpha, cb.alpha);
            uint cbAT = caA + cbA;

            uint intensity;
           
            if (cbAT == 0)
            {
                intensity = 0;
            }
            else
            {
                intensity =(uint) (((ca[0] * caA) + (cb[0] * cbA)) / cbAT);
             
            }

            return FromIntensity((UInt16  )intensity ,((ColorIntensity)ca  ).LookupColor    , (byte)cbAT);
        }

        /// <summary>
        /// Linearly interpolates between two color values.
        /// </summary>
        /// <param name="from">The color value that represents 0 on the lerp number line.</param>
        /// <param name="to">The color value that represents 1 on the lerp number line.</param>
        /// <param name="frac">A value in the range [0, 1].</param>
        /// <remarks>
        /// This method does a simple lerp on each color value and on the alpha channel. It does
        /// not properly take into account the alpha channel's effect on color blending.
        /// </remarks>
        public ColorPixelBase Lerp(ColorPixelBase from, ColorPixelBase to, float frac)
        {
            ColorIntensity ret = new ColorIntensity();

            ret.Intensity  = (byte)Utility.ClampToByte(Utility.Lerp(from[0], to[0], frac));
            ret.LookupColor =(byte) from[1];
            ret.A = (byte)Utility.ClampToByte(Utility.Lerp(from.alpha, to.alpha, frac));

            return ret;
        }

        /// <summary>
        /// Linearly interpolates between two color values.
        /// </summary>
        /// <param name="from">The color value that represents 0 on the lerp number line.</param>
        /// <param name="to">The color value that represents 1 on the lerp number line.</param>
        /// <param name="frac">A value in the range [0, 1].</param>
        /// <remarks>
        /// This method does a simple lerp on each color value and on the alpha channel. It does
        /// not properly take into account the alpha channel's effect on color blending.
        /// </remarks>
        public ColorPixelBase Lerp(ColorPixelBase from, ColorPixelBase to, double frac)
        {
            ColorIntensity ret = new ColorIntensity();

            ret.Intensity  = (byte)Utility.ClampToByte(Utility.Lerp(from[0], to[0], frac));
            ret.LookupColor = (byte)from[1];
            ret.A = (byte)Utility.ClampToByte(Utility.Lerp(from.alpha, to.alpha, frac));

            return ret;
        }


        public ColorPixelBase BlendColors4W16IP(Int32 c1, uint w1, Int32 c2, uint w2, Int32 c3, uint w3, Int32 c4, uint w4)
        {
            return BlendColors4W16IP(ColorIntensity.FromUInt32((UInt32)c1), w1, ColorIntensity.FromUInt32((UInt32)c2), w2, ColorIntensity.FromUInt32((UInt32)c3), w3, ColorIntensity.FromUInt32((UInt32)c4), w4);
        }
        /// <summary>
        /// Blends four colors together based on the given weight values.
        /// </summary>
        /// <returns>The blended color.</returns>
        /// <remarks>
        /// The weights should be 16-bit fixed point numbers that add up to 65536 ("1.0").
        /// 4W16IP means "4 colors, weights, 16-bit integer precision"
        /// </remarks>
        public ColorPixelBase BlendColors4W16IP(ColorPixelBase Cc1, uint w1, ColorPixelBase Cc2, uint w2, ColorPixelBase Cc3, uint w3, ColorPixelBase Cc4, uint w4)
        //public static ColorIntensity BlendColors4W16IP(ColorIntensity c1, uint w1, ColorIntensity c2, uint w2, ColorIntensity c3, uint w3, ColorIntensity c4, uint w4)
        {
#if DEBUG
            /*
            if ((w1 + w2 + w3 + w4) != 65536)
            {
                throw new ArgumentOutOfRangeException("w1 + w2 + w3 + w4 must equal 65536!");
            }
             * */
#endif
            ColorIntensity c1 = (ColorIntensity)Cc1;
            ColorIntensity c2 = (ColorIntensity)Cc2;
            ColorIntensity c3 = (ColorIntensity)Cc3;
            ColorIntensity c4 = (ColorIntensity)Cc4;

            const uint ww = 32768;
            uint af = (c1.A * w1) + (c2.A * w2) + (c3.A * w3) + (c4.A * w4);
            uint a = (af + ww) >> 16;

            uint b;
            uint g;
            uint r;
            ColorIntensity Cout = new ColorIntensity();
            if (a == 0)
            {
                b = 0;
                g = 0;
                r = 0;
            }
            else
            {
                Cout.Intensity  = (UInt16 )((((long)c1.A * c1[0] * w1) + ((long)c2.A * c2[0] * w2) + ((long)c3.A * c3[0] * w3) + ((long)c4.A * c4[0] * w4)) / af);
                
            }
            Cout.LookupColor = c1.LookupColor;
            Cout.A = (byte)a;
            return Cout ;
        }

        /// <summary>
        /// Blends the colors based on the given weight values.
        /// </summary>
        /// <param name="c">The array of color values.</param>
        /// <param name="w">The array of weight values.</param>
        /// <returns>
        /// The weights should be fixed point numbers. 
        /// The total summation of the weight values will be treated as "1.0".
        /// Each color will be blended in proportionally to its weight value respective to 
        /// the total summation of the weight values.
        /// </returns>
        /// <remarks>
        /// "WAIP" stands for "weights, arbitrary integer precision"</remarks>
        public static ColorIntensity BlendColorsWAIP(ColorIntensity[] c, uint[] w)
        {
            if (c.Length != w.Length)
            {
                throw new ArgumentException("c.Length != w.Length");
            }

            if (c.Length == 0)
            {
                return ColorIntensity.FromUInt32(0);
            }

            long wsum = 0;
            long asum = 0;

            for (int i = 0; i < w.Length; ++i)
            {
                wsum += w[i];
                asum += c[i].A * w[i];
            }

            uint a = (uint)((asum + (wsum >> 1)) / wsum);

            long b;
            long g;
            long r;

             
            if (a == 0)
            {
                b = 0;
                g = 0;
                r = 0;
            }
            else
            {
                b = 0;
                g = 0;
                r = 0;

                for (int i = 0; i < c.Length; ++i)
                {
                    b += (long)c[i].A * c[i][0] * w[i];
                    
                }

                b /= asum;
                g /= asum;
                r /= asum;
            }

            return (ColorIntensity) ColorIntensity.sFromIntensity((UInt16) b, (byte)c[0][1],(byte)a );
        }

        /// <summary>
        /// Blends the colors based on the given weight values.
        /// </summary>
        /// <param name="c">The array of color values.</param>
        /// <param name="w">The array of weight values.</param>
        /// <returns>
        /// Each color will be blended in proportionally to its weight value respective to 
        /// the total summation of the weight values.
        /// </returns>
        /// <remarks>
        /// "WAIP" stands for "weights, floating-point"</remarks>
        public static ColorIntensity BlendColorsWFP(ColorIntensity[] c, double[] w)
        {
            if (c.Length != w.Length)
            {
                throw new ArgumentException("c.Length != w.Length");
            }

            if (c.Length == 0)
            {
                return ColorIntensity.Transparent;
            }

            double wsum = 0;
            double asum = 0;

            for (int i = 0; i < w.Length; ++i)
            {
                wsum += w[i];
                asum += (double)c[i].A * w[i];
            }

            double a = asum / wsum;
            double aMultWsum = a * wsum;

            double b;
            double g;
            double r;

            if (asum == 0)
            {
                b = 0;
                g = 0;
                r = 0;
            }
            else
            {
                b = 0;
                g = 0;
                r = 0;

                for (int i = 0; i < c.Length; ++i)
                {
                    b += (double)c[i].A * c[i][0] * w[i];
                  
                }

                b /= aMultWsum;
               
            }

            return (ColorIntensity)ColorIntensity.sFromIntensity((UInt16)b, (byte)c[0][1], (byte)a);
        }

        public ColorPixelBase Blend(ColorPixelBase[] colors)
        {
            int count = colors.Length;
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count must be 0 or greater");
            }

            if (count == 0)
            {
                return ColorIntensity.Transparent;
            }

            ulong aSum = 0;

            for (int i = 0; i < count; ++i)
            {
                aSum += (ulong)colors[i].alpha;
            }

            byte b = 0;
            byte g = 0;
            byte r = 0;
            byte a = (byte)(aSum / (ulong)count);

            if (aSum != 0)
            {
                ulong bSum = 0;
                ulong gSum = 0;
                ulong rSum = 0;

                for (int i = 0; i < count; ++i)
                {
                    bSum += (ulong)(colors[i].alpha * colors[i][0]);
                  
                }

                b = (byte)(bSum / aSum);
                g = (byte)(gSum / aSum);
                r = (byte)(rSum / aSum);
            }

            return (ColorIntensity)ColorIntensity.sFromIntensity((UInt16)b, (byte)colors[0][1], (byte)a);
        }

        public override string ToString()
        {
            return "Intensity: " + Intensity.ToString() + ", ColorLUT: " + LookupColor.ToString() +  ", A: " + A;
        }

        /// <summary>
        /// Casts a ColorIntensity to a UInt32.
        /// </summary>
        public static explicit operator UInt32(ColorIntensity color)
        {
            return color.Bgra;
        }

        /// <summary>
        /// Casts a UInt32 to a ColorIntensity.
        /// </summary>
        public static explicit operator ColorIntensity(UInt32 uint32)
        {
            return ColorIntensity.FromUInt32(uint32);
        }


        /// <summary>
        /// Casts a ColorBGRA to a ColorIntensity.
        /// </summary>
        public static explicit operator ColorIntensity(ColorBgra  colorbgra)
        {
            return ColorIntensity.sFromBgra((byte)colorbgra[0], (byte)colorbgra[1], (byte)colorbgra[2], (byte)colorbgra.alpha);
        }

        // Colors: copied from System.Drawing.Color's list (don't worry I didn't type it in 
        // manually, I used a code generator w/ reflection ...)
        public ColorPixelBase BlackColor() { return Zero ; }
        public ColorPixelBase WhiteColor() { return White; }
        public ColorPixelBase TransparentColor() { return Transparent; }
        public ColorPixelBase ClearColor() { return White; }

        public static ColorIntensity Transparent
        {
            get
            {
                return ColorIntensity.sFromBgra(255, 255, 255, 0);
            }
        }
        public static ColorIntensity White
        {
            get
            {
                return (ColorIntensity)sFromIntensity(UInt16.MaxValue , 0, 255);
            }
        }
        public static ColorIntensity Black
        {
            get
            {
                return (ColorIntensity)sFromIntensity(0,0,255);
            }
        }
        public static ColorIntensity Zero
        {
            get
            {
                return (ColorIntensity)0;
            }
        }

        private static Dictionary<string, ColorIntensity> predefinedColors;

        /// <summary>
        /// Gets a hashtable that contains a list of all the predefined colors.
        /// These are the same color values that are defined as public static properties
        /// in System.Drawing.Color. The hashtable uses strings for the keys, and
        /// ColorIntensitys for the values.
        /// </summary>
        public static Dictionary<string, ColorIntensity> PredefinedColors
        {
            get
            {
                if (predefinedColors != null)
                {
                    Type ColorIntensityType = typeof(ColorIntensity);
                    PropertyInfo[] propInfos = ColorIntensityType.GetProperties(BindingFlags.Static | BindingFlags.Public);
                    Hashtable colors = new Hashtable();

                    foreach (PropertyInfo pi in propInfos)
                    {
                        if (pi.PropertyType == ColorIntensityType)
                        {
                            colors.Add(pi.Name, (ColorIntensity)pi.GetValue(null, null));
                        }
                    }
                }

                return new Dictionary<string, ColorIntensity>(predefinedColors);
            }
        }
    }
}
