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
    public struct ColorBgra : ColorPixelBase
    {
        
        [FieldOffset(0)]
        public byte B;

        [FieldOffset(1)]
        public byte G;

        [FieldOffset(2)]
        public byte R;

        [FieldOffset(3)]
        public byte A;

        /// <summary>
        /// Lets you change B, G, R, and A at the same time.
        /// </summary>
        [NonSerialized]
        [FieldOffset(0)]
        public uint Bgra;


        public bool CompareColor(ColorPixelBase TestColor, int tolerance)
        {
            ColorBgra a = (ColorBgra)TestColor;
            int sum = 0;
            int diff;

            diff = a.R - R;
            sum += (1 + diff * diff) * a.A / 256;

            diff = a.G - G;
            sum += (1 + diff * diff) * a.A / 256;

            diff = a.B - B;
            sum += (1 + diff * diff) * a.A / 256;

            diff = a.A - A;
            sum += diff * diff;

            return (sum <= tolerance * tolerance * 4);
        }


        public int  SizeOf
        {
            get { return 4; }
        }
        public byte red { get { return R; } set { R = value; } }
        public byte green { get { return G; } set { G = value; } }
        public byte blue { get { return B; } set { B = value; } }
        public byte alpha { get { return A; } set { A = value; } }

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
            this = Black;
        }

        public const int BlueChannel = 0;
        public const int GreenChannel = 1;
        public const int RedChannel = 2;
        public const int AlphaChannel = 3;

        public const int SizeOf0 = 4;

       
        public ColorPixelBase InvertColor(ColorPixelBase c)
        {
            ColorBgra color = (ColorBgra)c;
            return FromBgra((byte)(255 - color.B), (byte)(255 - color.G), (byte)(255 - color.R), (byte) color.A);
        }
        public ColorPixelBase InvertColorAndAlpha(ColorPixelBase c)
        {
            ColorBgra color = (ColorBgra)c;
            return FromBgra((byte)(255 - color.B), (byte)(255 - color.G), (byte)(255 - color.R), (byte)(255 - color.A));
        }

        public ColorPixelBase TranslateColor(ColorPixelBase InputPixel)
        {
            return (ColorPixelBase)InputPixel.ToColorBgra();
        }
        public ColorPixelBase TranslateColor(Int32 inputLong)
        {
            return FromInt32(inputLong );

        }
        public ColorPixelBase TranslateColor(uint inputLong)
        {
            return FromInt32((Int32)inputLong);

        }
        public static ColorPixelBase FromInt32(Int32 color)
        {
            ColorBgra c = new ColorBgra();
            c.Bgra = (UInt32)color;
            return c;
        }

        public ColorPixelBase  ParseHexString(string hexString)
        {
            uint value = Convert.ToUInt32(hexString, 16);
            return ColorBgra.FromUInt32(value);
        }

        public string ToHexString()
        {
            int rgbNumber = (this.R << 16) | (this.G << 8) | this.B;
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
        public unsafe Int32  this[int channel]
        {
            get
            {
                if (channel < 0 || channel > 3)
                {
                    throw new ArgumentOutOfRangeException("channel", channel, "valid range is [0,3]");
                }

                fixed (byte* p = &B)
                {
                    return p[channel];
                }
            }

            set
            {
                if (channel < 0 || channel > 3)
                {
                    throw new ArgumentOutOfRangeException("channel", channel, "valid range is [0,3]");
                }

                fixed (byte* p = &B)
                {
                    p[channel] = (byte)value;
                }
            }
        }

        /// <summary>
        /// Gets the luminance intensity of the pixel based on the values of the red, green, and blue components. Alpha is ignored.
        /// </summary>
        /// <returns>A value in the range 0 to 1 inclusive.</returns>
        public double GetIntensity()
        {
            return ((0.114 * (double)B) + (0.587 * (double)G) + (0.299 * (double)R)) / 255.0;
        }
        /// <summary>
        /// Gets the luminance intensity of the pixel based on the values of the red, green, and blue components. Alpha is ignored.
        /// </summary>
        /// <returns>A value in the range 0 to 2^16 inclusive.</returns>
        public uint GetIntensity16Bit()
        {
            return (uint)(((0.114 * (double)B) + (0.587 * (double)G) + (0.299 * (double)R)) * 255.0);
        }
        public static uint GetIntensity16Bit(byte B, byte G, byte R)
        {
            return (uint)(((0.114 * (double)B) + (0.587 * (double)G) + (0.299 * (double)R)) * 255.0);
        }
        /// <summary>
        /// Gets the luminance intensity of the pixel based on the values of the red, green, and blue components. Alpha is ignored.
        /// </summary>
        /// <returns>A value in the range 0 to 255 inclusive.</returns>
        public byte GetIntensityByte()
        {
            return (byte)((7471 * B + 38470 * G + 19595 * R) >> 16);
        }

        public ColorPixelBase Clone()
        {
            ColorBgra c = new ColorBgra();
            c.A = A;
            c.B = B;
            c.R = R;
            c.G = G;
            return c;
        }
        public ColorPixelBase AnotherPixel()
        {
            ColorBgra c = new ColorBgra();
            c.A = 255;
            c.B = 0;
            c.R = 0;
            c.G = 0;
            return c;
        }
        public ColorPixelBase  AverageChannels(ColorPixelBase InputColor)
        {
            ColorBgra c=(ColorBgra)InputColor ;
            double i=(c.A+c.B+c.G )/3;
            ColorBgra c2=new ColorBgra();
            c2.A=c.A ;
            c2.B=(byte)i;
            c2.G=(byte)i;
            c2.R=(byte)i;
            return (c2);

        }
        /// <summary>
        /// Returns the maximum value out of the B, G, and R values. Alpha is ignored.
        /// </summary>
        /// <returns></returns>
        public byte GetMaxColorChannelValue()
        {
            return Math.Max(this.B, Math.Max(this.G, this.R));
        }

        /// <summary>
        /// Returns the average of the B, G, and R values. Alpha is ignored.
        /// </summary>
        /// <returns></returns>
        public byte GetAverageColorChannelValue()
        {
            return (byte)((this.B + this.G + this.R) / 3);
        }

        /// <summary>
        /// Compares two ColorBgra instance to determine if they are equal.
        /// </summary>
        public static bool operator ==(ColorBgra lhs, ColorBgra rhs)
        {
            return lhs.Bgra == rhs.Bgra;
        }

        /// <summary>
        /// Compares two ColorBgra instance to determine if they are not equal.
        /// </summary>
        public static bool operator !=(ColorBgra lhs, ColorBgra rhs)
        {
            return lhs.Bgra != rhs.Bgra;
        }

        /// <summary>
        /// Compares two ColorBgra instance to determine if they are equal.
        /// </summary>
        public override bool Equals(object obj)
        {

            if (obj != null && obj is ColorBgra && ((ColorBgra)obj).Bgra == this.Bgra)
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

        /// <summary>
        /// Returns a new ColorBgra with the same color values but with a new alpha component value.
        /// </summary>
        public ColorPixelBase NewAlpha(byte newA)
        {
            return (ColorBgra)FromBgra(B, G, R, newA);
        }

        /// <summary>
        /// Creates a new ColorBgra instance with the given color and alpha values.
        /// </summary>
        /// <summary>
        /// Creates a new ColorBgra instance with the given color and alpha values.
        /// </summary>
        public static ColorBgra sFromBgra(byte b, byte g, byte r, byte a)
        {
            ColorBgra color = new ColorBgra();
            color.Bgra = BgraToUInt32(b, g, r, a);
            return color;
        }

        /// <summary>
        /// Creates a new ColorBgra instance with the given color and alpha values.
        /// </summary>
        /// <summary>
        /// Creates a new ColorBgra instance with the given color and alpha values.
        /// </summary>
        public ColorPixelBase FromBgra(byte b, byte g, byte r, byte a)
        {
            return sFromBgra(b, g, r, a);
        }

        public ColorPixelBase FromArray(int[] Channels, byte alpha)
        {
            ColorBgra color = new ColorBgra();
            color.Bgra = BgraToUInt32((byte)Channels[0], (byte)Channels[1], (byte)Channels[2], (byte)alpha);
            return color;
        }
        public ColorPixelBase FromArray(long[] Channels, byte alpha)
        {
            ColorBgra color = new ColorBgra();
            color.Bgra = BgraToUInt32((byte)Channels[0], (byte)Channels[1], (byte)Channels[2], (byte)alpha);
            return color;
        }
        public ColorPixelBase FromArray(byte[] Channels,byte alpha)
        {
            ColorBgra color = new ColorBgra();
            color.Bgra = BgraToUInt32(Channels[0], Channels[1], Channels[2], alpha);
            return color;
        }
        /// <summary>
        /// Creates a new ColorBgra instance with the given color and alpha values.
        /// </summary>
        public ColorPixelBase  FromBgraClamped(int b, int g, int r, int a)
        {
            return FromBgra(
                Utility.ClampToByte(b),
                Utility.ClampToByte(g),
                Utility.ClampToByte(r),
                Utility.ClampToByte(a));
        }

        /// <summary>
        /// Creates a new ColorBgra instance with the given color and alpha values.
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
            return (uint)b + ((uint)g << 8) + ((uint)r << 16) + ((uint)a << 24);
        }

        /// <summary>
        /// Packs color and alpha values into a 32-bit integer.
        /// </summary>
        public static UInt32 BgraToUInt32(int b, int g, int r, int a)
        {
            return (uint)b + ((uint)g << 8) + ((uint)r << 16) + ((uint)a << 24);
        }

        /// <summary>
        /// Creates a new ColorBgra instance with the given color values, and 255 for alpha.
        /// </summary>
        public ColorBgra FromBgr(byte b, byte g, byte r)
        {
            return (ColorBgra)FromBgra(b, g, r, 255);
        }

        /// <summary>
        /// Constructs a new ColorBgra instance with the given 32-bit value.
        /// </summary>
        public static ColorBgra FromUInt32(UInt32 bgra)
        {
            ColorBgra color = new ColorBgra();
            color.Bgra = bgra;
            return color;
        }

        /// <summary>
        /// Constructs a new ColorBgra instance given a 32-bit signed integer that represents an R,G,B triple.
        /// Alpha will be initialized to 255.
        /// </summary>
        public static ColorBgra FromOpaqueInt32(Int32 bgr)
        {
            if (bgr < 0 || bgr > 0xffffff)
            {
                throw new ArgumentOutOfRangeException("bgr", "must be in the range [0, 0xffffff]");
            }

            ColorBgra color = new ColorBgra();
            color.Bgra = (uint)bgr;
            color.A = 255;

            return color;
        }

        public PixelFormat pixelFormat 
        {
            get
            {
                return PixelFormat.Format32bppArgb;
            }
        }
        public static int ToOpaqueInt32(ColorBgra color)
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
        /// Constructs a new ColorBgra instance from the values in the given Color instance.
        /// </summary>
        public ColorPixelBase FromColor(Color c)
        {
            return FromBgra(c.B, c.G, c.R, c.A);
        }

        /// <summary>
        /// Constructs a new ColorBgra instance from the values in the given Color instance.
        /// </summary>
        public static  ColorBgra sFromColor(Color c)
        {
            return sFromBgra(c.B, c.G, c.R, c.A);
        }

        /// <summary>
        /// Converts this ColorBgra instance to a Color instance.
        /// </summary>
        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }

        /// <summary>
        /// Smoothly blends between two colors.
        /// </summary>
        public ColorPixelBase Blend(ColorPixelBase ca, ColorPixelBase cb, byte cbAlpha)
        {
            uint caA = (uint)Utility.FastScaleByteByByte((byte)(255 - cbAlpha), ca.alpha );
            uint cbA = (uint)Utility.FastScaleByteByByte(cbAlpha, cb.alpha );
            uint cbAT = caA + cbA;

            uint r;
            uint g;
            uint b;

            if (cbAT == 0)
            {
                r = 0;
                g = 0;
                b = 0;
            }
            else
            {
                r = (uint)(((ca[2] * caA) + (cb[2] * cbA)) / cbAT);
                g = (uint)(((ca[1] * caA) + (cb[1] * cbA)) / cbAT);
                b = (uint)(((ca[0] * caA) + (cb[0] * cbA)) / cbAT);
            }

            return ColorBgra.sFromBgra((byte)b, (byte)g, (byte)r, (byte)cbAT);
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
            ColorBgra ret = new ColorBgra();

            ret.B = (byte)Utility.ClampToByte(Utility.Lerp(from[0] , to[0] , frac));
            ret.G = (byte)Utility.ClampToByte(Utility.Lerp(from[1] , to[1] , frac));
            ret.R = (byte)Utility.ClampToByte(Utility.Lerp(from[2] , to[2] , frac));
            ret.A = (byte)Utility.ClampToByte(Utility.Lerp(from.alpha , to.alpha , frac));

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
            ColorBgra ret = new ColorBgra();

            ret.B = (byte)Utility.ClampToByte(Utility.Lerp(from[0] , to[0] , frac));
            ret.G = (byte)Utility.ClampToByte(Utility.Lerp(from[1] , to[1] , frac));
            ret.R = (byte)Utility.ClampToByte(Utility.Lerp(from[2] , to[2] , frac));
            ret.A = (byte)Utility.ClampToByte(Utility.Lerp(from.alpha , to.alpha , frac));

            return ret;
        }


        public ColorPixelBase BlendColors4W16IP(Int32 c1, uint w1, Int32 c2, uint w2, Int32 c3, uint w3, Int32 c4, uint w4)
        {
           return  BlendColors4W16IP(ColorBgra.FromUInt32((UInt32)c1), w1, ColorBgra.FromUInt32((UInt32)c2), w2, ColorBgra.FromUInt32((UInt32)c3), w3, ColorBgra.FromUInt32((UInt32)c4), w4);
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
        //public static ColorBgra BlendColors4W16IP(ColorBgra c1, uint w1, ColorBgra c2, uint w2, ColorBgra c3, uint w3, ColorBgra c4, uint w4)
        {
#if DEBUG
            /*
            if ((w1 + w2 + w3 + w4) != 65536)
            {
                throw new ArgumentOutOfRangeException("w1 + w2 + w3 + w4 must equal 65536!");
            }
             * */
#endif
            ColorBgra c1 = (ColorBgra)Cc1;
            ColorBgra c2 = (ColorBgra)Cc2;
            ColorBgra c3 = (ColorBgra)Cc3;
            ColorBgra c4 = (ColorBgra)Cc4;

            const uint ww = 32768;
            uint af = (c1.A  * w1) + (c2.A  * w2) + (c3.A  * w3) + (c4.A  * w4);
            uint a = (af + ww) >> 16;

            uint b;
            uint g;
            uint r;

            if (a == 0)
            {
                b = 0;
                g = 0;
                r = 0;
            }
            else
            {
                b = (uint)((((long)c1.A * c1.B * w1) + ((long)c2.A * c2.B * w2) + ((long)c3.A * c3.B * w3) + ((long)c4.A * c4.B * w4)) / af);
                g = (uint)((((long)c1.A * c1.G * w1) + ((long)c2.A * c2.G * w2) + ((long)c3.A * c3.G * w3) + ((long)c4.A * c4.G * w4)) / af);
                r = (uint)((((long)c1.A * c1.R * w1) + ((long)c2.A * c2.R * w2) + ((long)c3.A * c3.R * w3) + ((long)c4.A * c4.R * w4)) / af);
            }

            return FromBgra((byte)b, (byte)g, (byte)r, (byte)a);
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
        public static ColorBgra BlendColorsWAIP(ColorBgra[] c, uint[] w)
        {
            if (c.Length != w.Length)
            {
                throw new ArgumentException("c.Length != w.Length");
            }

            if (c.Length == 0)
            {
                return ColorBgra.FromUInt32(0);
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
                    b += (long)c[i].A * c[i].B * w[i];
                    g += (long)c[i].A * c[i].G * w[i];
                    r += (long)c[i].A * c[i].R * w[i];
                }

                b /= asum;
                g /= asum;
                r /= asum;
            }

            return ColorBgra.FromUInt32((uint)b + ((uint)g << 8) + ((uint)r << 16) + ((uint)a << 24));
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
        public static ColorBgra BlendColorsWFP(ColorBgra[] c, double[] w)
        {
            if (c.Length != w.Length)
            {
                throw new ArgumentException("c.Length != w.Length");
            }

            if (c.Length == 0)
            {
                return ColorBgra.Transparent;
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
                    b += (double)c[i].A * c[i].B * w[i];
                    g += (double)c[i].A * c[i].G * w[i];
                    r += (double)c[i].A * c[i].R * w[i];
                }

                b /= aMultWsum;
                g /= aMultWsum;
                r /= aMultWsum;
            }

            return ColorBgra.sFromBgra((byte)b, (byte)g, (byte)r, (byte)a);
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
                return ColorBgra.Transparent;
            }

            ulong aSum = 0;

            for (int i = 0; i < count; ++i)
            {
                aSum += (ulong)colors[i].alpha ;
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
                    bSum += (ulong)(colors[i].alpha  * colors[i][0] );
                    gSum += (ulong)(colors[i].alpha  * colors[i][1] );
                    rSum += (ulong)(colors[i].alpha  * colors[i][2] );
                }

                b = (byte)(bSum / aSum);
                g = (byte)(gSum / aSum);
                r = (byte)(rSum / aSum);
            }

            return ColorBgra.sFromBgra(b, g, r, a);
        }

        public override string ToString()
        {
            return "B: " + B + ", G: " + G + ", R: " + R + ", A: " + A;
        }

        /// <summary>
        /// Casts a ColorIntensity to a ColorBGRA.
        /// </summary>
        public static explicit operator ColorBgra(ColorIntensity color)
        {
            byte R, G, B;
            Color LUT = Color.White;
            double C = color.GetIntensity();
            R = (byte)(C * LUT.R);
            G = (byte)(C * LUT.G);
            B = (byte)(C * LUT.B);
            return ColorBgra.sFromBgra(B, G, R, color.A);
        }

        public ColorBgra ToColorBgra()
        {
            return this;
        }
        public ColorIntensity ToColorIntensity()
        {
            return ColorIntensity.sFromBgra(B , G, R, A );
        }

        /// <summary>
        /// Casts a ColorBgra to a UInt32.
        /// </summary>
        public static explicit operator UInt32(ColorBgra color)
        {
            return color.Bgra;
        }

        /// <summary>
        /// Casts a UInt32 to a ColorBgra.
        /// </summary>
        public static explicit operator ColorBgra(UInt32 uint32)
        {
            return ColorBgra.FromUInt32(uint32);
        }

        // Colors: copied from System.Drawing.Color's list (don't worry I didn't type it in 
        // manually, I used a code generator w/ reflection ...)
        public ColorPixelBase BlackColor() { return Black; }
        public ColorPixelBase WhiteColor() { return White; }
        public ColorPixelBase TransparentColor() { return Transparent; }
        public ColorPixelBase ClearColor() { return White; }

        public static ColorBgra Transparent
        {
            get
            {
                return ColorBgra.sFromBgra(255, 255, 255, 0);
            }
        }

        public static ColorBgra AliceBlue
        {
            get
            {
                return ColorBgra.sFromBgra(255, 248, 240, 255);
            }
        }

        public static ColorBgra AntiqueWhite
        {
            get
            {
                return ColorBgra.sFromBgra(215, 235, 250, 255);
            }
        }

        public static ColorBgra Aqua
        {
            get
            {
                return ColorBgra.sFromBgra(255, 255, 0, 255);
            }
        }

        public static ColorBgra Aquamarine
        {
            get
            {
                return ColorBgra.sFromBgra(212, 255, 127, 255);
            }
        }

        public static ColorBgra Azure
        {
            get
            {
                return ColorBgra.sFromBgra(255, 255, 240, 255);
            }
        }

        public static ColorBgra Beige
        {
            get
            {
                return ColorBgra.sFromBgra(220, 245, 245, 255);
            }
        }

        public static ColorBgra Bisque
        {
            get
            {
                return ColorBgra.sFromBgra(196, 228, 255, 255);
            }
        }

        public static ColorBgra Black
        {
            get
            {
                return ColorBgra.sFromBgra(0, 0, 0, 255);
            }
        }

        public static ColorBgra BlanchedAlmond
        {
            get
            {
                return ColorBgra.sFromBgra(205, 235, 255, 255);
            }
        }

        public static ColorBgra Blue
        {
            get
            {
                return ColorBgra.sFromBgra(255, 0, 0, 255);
            }
        }

        public static ColorBgra BlueViolet
        {
            get
            {
                return ColorBgra.sFromBgra(226, 43, 138, 255);
            }
        }

        public static ColorBgra Brown
        {
            get
            {
                return ColorBgra.sFromBgra(42, 42, 165, 255);
            }
        }

        public static ColorBgra BurlyWood
        {
            get
            {
                return ColorBgra.sFromBgra(135, 184, 222, 255);
            }
        }

        public static ColorBgra CadetBlue
        {
            get
            {
                return ColorBgra.sFromBgra(160, 158, 95, 255);
            }
        }

        public static ColorBgra Chartreuse
        {
            get
            {
                return ColorBgra.sFromBgra(0, 255, 127, 255);
            }
        }

        public static ColorBgra Chocolate
        {
            get
            {
                return ColorBgra.sFromBgra(30, 105, 210, 255);
            }
        }

        public static ColorBgra Coral
        {
            get
            {
                return ColorBgra.sFromBgra(80, 127, 255, 255);
            }
        }

        public static ColorBgra CornflowerBlue
        {
            get
            {
                return ColorBgra.sFromBgra(237, 149, 100, 255);
            }
        }

        public static ColorBgra Cornsilk
        {
            get
            {
                return ColorBgra.sFromBgra(220, 248, 255, 255);
            }
        }

        public static ColorBgra Crimson
        {
            get
            {
                return ColorBgra.sFromBgra(60, 20, 220, 255);
            }
        }

        public static ColorBgra Cyan
        {
            get
            {
                return ColorBgra.sFromBgra(255, 255, 0, 255);
            }
        }

        public static ColorBgra DarkBlue
        {
            get
            {
                return ColorBgra.sFromBgra(139, 0, 0, 255);
            }
        }

        public static ColorBgra DarkCyan
        {
            get
            {
                return ColorBgra.sFromBgra(139, 139, 0, 255);
            }
        }

        public static ColorBgra DarkGoldenrod
        {
            get
            {
                return ColorBgra.sFromBgra(11, 134, 184, 255);
            }
        }

        public static ColorBgra DarkGray
        {
            get
            {
                return ColorBgra.sFromBgra(169, 169, 169, 255);
            }
        }

        public static ColorBgra DarkGreen
        {
            get
            {
                return ColorBgra.sFromBgra(0, 100, 0, 255);
            }
        }

        public static ColorBgra DarkKhaki
        {
            get
            {
                return ColorBgra.sFromBgra(107, 183, 189, 255);
            }
        }

        public static ColorBgra DarkMagenta
        {
            get
            {
                return ColorBgra.sFromBgra(139, 0, 139, 255);
            }
        }

        public static ColorBgra DarkOliveGreen
        {
            get
            {
                return ColorBgra.sFromBgra(47, 107, 85, 255);
            }
        }

        public static ColorBgra DarkOrange
        {
            get
            {
                return ColorBgra.sFromBgra(0, 140, 255, 255);
            }
        }

        public static ColorBgra DarkOrchid
        {
            get
            {
                return ColorBgra.sFromBgra(204, 50, 153, 255);
            }
        }

        public static ColorBgra DarkRed
        {
            get
            {
                return ColorBgra.sFromBgra(0, 0, 139, 255);
            }
        }

        public static ColorBgra DarkSalmon
        {
            get
            {
                return ColorBgra.sFromBgra(122, 150, 233, 255);
            }
        }

        public static ColorBgra DarkSeaGreen
        {
            get
            {
                return ColorBgra.sFromBgra(139, 188, 143, 255);
            }
        }

        public static ColorBgra DarkSlateBlue
        {
            get
            {
                return ColorBgra.sFromBgra(139, 61, 72, 255);
            }
        }

        public static ColorBgra DarkSlateGray
        {
            get
            {
                return ColorBgra.sFromBgra(79, 79, 47, 255);
            }
        }

        public static ColorBgra DarkTurquoise
        {
            get
            {
                return ColorBgra.sFromBgra(209, 206, 0, 255);
            }
        }

        public static ColorBgra DarkViolet
        {
            get
            {
                return ColorBgra.sFromBgra(211, 0, 148, 255);
            }
        }

        public static ColorBgra DeepPink
        {
            get
            {
                return ColorBgra.sFromBgra(147, 20, 255, 255);
            }
        }

        public static ColorBgra DeepSkyBlue
        {
            get
            {
                return ColorBgra.sFromBgra(255, 191, 0, 255);
            }
        }

        public static ColorBgra DimGray
        {
            get
            {
                return ColorBgra.sFromBgra(105, 105, 105, 255);
            }
        }

        public static ColorBgra DodgerBlue
        {
            get
            {
                return ColorBgra.sFromBgra(255, 144, 30, 255);
            }
        }

        public static ColorBgra Firebrick
        {
            get
            {
                return ColorBgra.sFromBgra(34, 34, 178, 255);
            }
        }

        public static ColorBgra FloralWhite
        {
            get
            {
                return ColorBgra.sFromBgra(240, 250, 255, 255);
            }
        }

        public static ColorBgra ForestGreen
        {
            get
            {
                return ColorBgra.sFromBgra(34, 139, 34, 255);
            }
        }

        public static ColorBgra Fuchsia
        {
            get
            {
                return ColorBgra.sFromBgra(255, 0, 255, 255);
            }
        }

        public static ColorBgra Gainsboro
        {
            get
            {
                return ColorBgra.sFromBgra(220, 220, 220, 255);
            }
        }

        public static ColorBgra GhostWhite
        {
            get
            {
                return ColorBgra.sFromBgra(255, 248, 248, 255);
            }
        }

        public static ColorBgra Gold
        {
            get
            {
                return ColorBgra.sFromBgra(0, 215, 255, 255);
            }
        }

        public static ColorBgra Goldenrod
        {
            get
            {
                return ColorBgra.sFromBgra(32, 165, 218, 255);
            }
        }

        public static ColorBgra Gray
        {
            get
            {
                return ColorBgra.sFromBgra(128, 128, 128, 255);
            }
        }

        public static ColorBgra Green
        {
            get
            {
                return ColorBgra.sFromBgra(0, 128, 0, 255);
            }
        }

        public static ColorBgra GreenYellow
        {
            get
            {
                return ColorBgra.sFromBgra(47, 255, 173, 255);
            }
        }

        public static ColorBgra Honeydew
        {
            get
            {
                return ColorBgra.sFromBgra(240, 255, 240, 255);
            }
        }

        public static ColorBgra HotPink
        {
            get
            {
                return ColorBgra.sFromBgra(180, 105, 255, 255);
            }
        }

        public static ColorBgra IndianRed
        {
            get
            {
                return ColorBgra.sFromBgra(92, 92, 205, 255);
            }
        }

        public static ColorBgra Indigo
        {
            get
            {
                return ColorBgra.sFromBgra(130, 0, 75, 255);
            }
        }

        public static ColorBgra Ivory
        {
            get
            {
                return ColorBgra.sFromBgra(240, 255, 255, 255);
            }
        }

        public static ColorBgra Khaki
        {
            get
            {
                return ColorBgra.sFromBgra(140, 230, 240, 255);
            }
        }

        public static ColorBgra Lavender
        {
            get
            {
                return ColorBgra.sFromBgra(250, 230, 230, 255);
            }
        }

        public static ColorBgra LavenderBlush
        {
            get
            {
                return ColorBgra.sFromBgra(245, 240, 255, 255);
            }
        }

        public static ColorBgra LawnGreen
        {
            get
            {
                return ColorBgra.sFromBgra(0, 252, 124, 255);
            }
        }

        public static ColorBgra LemonChiffon
        {
            get
            {
                return ColorBgra.sFromBgra(205, 250, 255, 255);
            }
        }

        public static ColorBgra LightBlue
        {
            get
            {
                return ColorBgra.sFromBgra(230, 216, 173, 255);
            }
        }

        public static ColorBgra LightCoral
        {
            get
            {
                return ColorBgra.sFromBgra(128, 128, 240, 255);
            }
        }

        public static ColorBgra LightCyan
        {
            get
            {
                return ColorBgra.sFromBgra(255, 255, 224, 255);
            }
        }

        public static ColorBgra LightGoldenrodYellow
        {
            get
            {
                return ColorBgra.sFromBgra(210, 250, 250, 255);
            }
        }

        public static ColorBgra LightGreen
        {
            get
            {
                return ColorBgra.sFromBgra(144, 238, 144, 255);
            }
        }

        public static ColorBgra LightGray
        {
            get
            {
                return ColorBgra.sFromBgra(211, 211, 211, 255);
            }
        }

        public static ColorBgra LightPink
        {
            get
            {
                return ColorBgra.sFromBgra(193, 182, 255, 255);
            }
        }

        public static ColorBgra LightSalmon
        {
            get
            {
                return ColorBgra.sFromBgra(122, 160, 255, 255);
            }
        }

        public static ColorBgra LightSeaGreen
        {
            get
            {
                return ColorBgra.sFromBgra(170, 178, 32, 255);
            }
        }

        public static ColorBgra LightSkyBlue
        {
            get
            {
                return ColorBgra.sFromBgra(250, 206, 135, 255);
            }
        }

        public static ColorBgra LightSlateGray
        {
            get
            {
                return ColorBgra.sFromBgra(153, 136, 119, 255);
            }
        }

        public static ColorBgra LightSteelBlue
        {
            get
            {
                return ColorBgra.sFromBgra(222, 196, 176, 255);
            }
        }

        public static ColorBgra LightYellow
        {
            get
            {
                return ColorBgra.sFromBgra(224, 255, 255, 255);
            }
        }

        public static ColorBgra Lime
        {
            get
            {
                return ColorBgra.sFromBgra(0, 255, 0, 255);
            }
        }

        public static ColorBgra LimeGreen
        {
            get
            {
                return ColorBgra.sFromBgra(50, 205, 50, 255);
            }
        }

        public static ColorBgra Linen
        {
            get
            {
                return ColorBgra.sFromBgra(230, 240, 250, 255);
            }
        }

        public static ColorBgra Magenta
        {
            get
            {
                return ColorBgra.sFromBgra(255, 0, 255, 255);
            }
        }

        public static ColorBgra Maroon
        {
            get
            {
                return ColorBgra.sFromBgra(0, 0, 128, 255);
            }
        }

        public static ColorBgra MediumAquamarine
        {
            get
            {
                return ColorBgra.sFromBgra(170, 205, 102, 255);
            }
        }

        public static ColorBgra MediumBlue
        {
            get
            {
                return ColorBgra.sFromBgra(205, 0, 0, 255);
            }
        }

        public static ColorBgra MediumOrchid
        {
            get
            {
                return ColorBgra.sFromBgra(211, 85, 186, 255);
            }
        }

        public static ColorBgra MediumPurple
        {
            get
            {
                return ColorBgra.sFromBgra(219, 112, 147, 255);
            }
        }

        public static ColorBgra MediumSeaGreen
        {
            get
            {
                return ColorBgra.sFromBgra(113, 179, 60, 255);
            }
        }

        public static ColorBgra MediumSlateBlue
        {
            get
            {
                return ColorBgra.sFromBgra(238, 104, 123, 255);
            }
        }

        public static ColorBgra MediumSpringGreen
        {
            get
            {
                return ColorBgra.sFromBgra(154, 250, 0, 255);
            }
        }

        public static ColorBgra MediumTurquoise
        {
            get
            {
                return ColorBgra.sFromBgra(204, 209, 72, 255);
            }
        }

        public static ColorBgra MediumVioletRed
        {
            get
            {
                return ColorBgra.sFromBgra(133, 21, 199, 255);
            }
        }

        public static ColorBgra MidnightBlue
        {
            get
            {
                return ColorBgra.sFromBgra(112, 25, 25, 255);
            }
        }

        public static ColorBgra MintCream
        {
            get
            {
                return ColorBgra.sFromBgra(250, 255, 245, 255);
            }
        }

        public static ColorBgra MistyRose
        {
            get
            {
                return ColorBgra.sFromBgra(225, 228, 255, 255);
            }
        }

        public static ColorBgra Moccasin
        {
            get
            {
                return ColorBgra.sFromBgra(181, 228, 255, 255);
            }
        }

        public static ColorBgra NavajoWhite
        {
            get
            {
                return ColorBgra.sFromBgra(173, 222, 255, 255);
            }
        }

        public static ColorBgra Navy
        {
            get
            {
                return ColorBgra.sFromBgra(128, 0, 0, 255);
            }
        }

        public static ColorBgra OldLace
        {
            get
            {
                return ColorBgra.sFromBgra(230, 245, 253, 255);
            }
        }

        public static ColorBgra Olive
        {
            get
            {
                return ColorBgra.sFromBgra(0, 128, 128, 255);
            }
        }

        public static ColorBgra OliveDrab
        {
            get
            {
                return ColorBgra.sFromBgra(35, 142, 107, 255);
            }
        }

        public static ColorBgra Orange
        {
            get
            {
                return ColorBgra.sFromBgra(0, 165, 255, 255);
            }
        }

        public static ColorBgra OrangeRed
        {
            get
            {
                return ColorBgra.sFromBgra(0, 69, 255, 255);
            }
        }

        public static ColorBgra Orchid
        {
            get
            {
                return ColorBgra.sFromBgra(214, 112, 218, 255);
            }
        }

        public static ColorBgra PaleGoldenrod
        {
            get
            {
                return ColorBgra.sFromBgra(170, 232, 238, 255);
            }
        }

        public static ColorBgra PaleGreen
        {
            get
            {
                return ColorBgra.sFromBgra(152, 251, 152, 255);
            }
        }

        public static ColorBgra PaleTurquoise
        {
            get
            {
                return ColorBgra.sFromBgra(238, 238, 175, 255);
            }
        }

        public static ColorBgra PaleVioletRed
        {
            get
            {
                return ColorBgra.sFromBgra(147, 112, 219, 255);
            }
        }

        public static ColorBgra PapayaWhip
        {
            get
            {
                return ColorBgra.sFromBgra(213, 239, 255, 255);
            }
        }

        public static ColorBgra PeachPuff
        {
            get
            {
                return ColorBgra.sFromBgra(185, 218, 255, 255);
            }
        }

        public static ColorBgra Peru
        {
            get
            {
                return ColorBgra.sFromBgra(63, 133, 205, 255);
            }
        }

        public static ColorBgra Pink
        {
            get
            {
                return ColorBgra.sFromBgra(203, 192, 255, 255);
            }
        }

        public static ColorBgra Plum
        {
            get
            {
                return ColorBgra.sFromBgra(221, 160, 221, 255);
            }
        }

        public static ColorBgra PowderBlue
        {
            get
            {
                return ColorBgra.sFromBgra(230, 224, 176, 255);
            }
        }

        public static ColorBgra Purple
        {
            get
            {
                return ColorBgra.sFromBgra(128, 0, 128, 255);
            }
        }

        public static ColorBgra Red
        {
            get
            {
                return ColorBgra.sFromBgra(0, 0, 255, 255);
            }
        }

        public static ColorBgra RosyBrown
        {
            get
            {
                return ColorBgra.sFromBgra(143, 143, 188, 255);
            }
        }

        public static ColorBgra RoyalBlue
        {
            get
            {
                return ColorBgra.sFromBgra(225, 105, 65, 255);
            }
        }

        public static ColorBgra SaddleBrown
        {
            get
            {
                return ColorBgra.sFromBgra(19, 69, 139, 255);
            }
        }

        public static ColorBgra Salmon
        {
            get
            {
                return ColorBgra.sFromBgra(114, 128, 250, 255);
            }
        }

        public static ColorBgra SandyBrown
        {
            get
            {
                return ColorBgra.sFromBgra(96, 164, 244, 255);
            }
        }

        public static ColorBgra SeaGreen
        {
            get
            {
                return ColorBgra.sFromBgra(87, 139, 46, 255);
            }
        }

        public static ColorBgra SeaShell
        {
            get
            {
                return ColorBgra.sFromBgra(238, 245, 255, 255);
            }
        }

        public static ColorBgra Sienna
        {
            get
            {
                return ColorBgra.sFromBgra(45, 82, 160, 255);
            }
        }

        public static ColorBgra Silver
        {
            get
            {
                return ColorBgra.sFromBgra(192, 192, 192, 255);
            }
        }

        public static ColorBgra SkyBlue
        {
            get
            {
                return ColorBgra.sFromBgra(235, 206, 135, 255);
            }
        }

        public static ColorBgra SlateBlue
        {
            get
            {
                return ColorBgra.sFromBgra(205, 90, 106, 255);
            }
        }

        public static ColorBgra SlateGray
        {
            get
            {
                return ColorBgra.sFromBgra(144, 128, 112, 255);
            }
        }

        public static ColorBgra Snow
        {
            get
            {
                return ColorBgra.sFromBgra(250, 250, 255, 255);
            }
        }

        public static ColorBgra SpringGreen
        {
            get
            {
                return ColorBgra.sFromBgra(127, 255, 0, 255);
            }
        }

        public static ColorBgra SteelBlue
        {
            get
            {
                return ColorBgra.sFromBgra(180, 130, 70, 255);
            }
        }

        public static ColorBgra Tan
        {
            get
            {
                return ColorBgra.sFromBgra(140, 180, 210, 255);
            }
        }

        public static ColorBgra Teal
        {
            get
            {
                return ColorBgra.sFromBgra(128, 128, 0, 255);
            }
        }

        public static ColorBgra Thistle
        {
            get
            {
                return ColorBgra.sFromBgra(216, 191, 216, 255);
            }
        }

        public static ColorBgra Tomato
        {
            get
            {
                return ColorBgra.sFromBgra(71, 99, 255, 255);
            }
        }

        public static ColorBgra Turquoise
        {
            get
            {
                return ColorBgra.sFromBgra(208, 224, 64, 255);
            }
        }

        public static ColorBgra Violet
        {
            get
            {
                return ColorBgra.sFromBgra(238, 130, 238, 255);
            }
        }

        public static ColorBgra Wheat
        {
            get
            {
                return ColorBgra.sFromBgra(179, 222, 245, 255);
            }
        }

        public static ColorBgra White
        {
            get
            {
                return ColorBgra.sFromBgra(255, 255, 255, 255);
            }
        }

        public static ColorBgra WhiteSmoke
        {
            get
            {
                return ColorBgra.sFromBgra(245, 245, 245, 255);
            }
        }

        public static ColorBgra Yellow
        {
            get
            {
                return ColorBgra.sFromBgra(0, 255, 255, 255);
            }
        }

        public static ColorBgra YellowGreen
        {
            get
            {
                return ColorBgra.sFromBgra(50, 205, 154, 255);
            }
        }

        public static ColorBgra Zero
        {
            get
            {
                return (ColorBgra)0;
            }
        }

        private static Dictionary<string, ColorBgra> predefinedColors;

        /// <summary>
        /// Gets a hashtable that contains a list of all the predefined colors.
        /// These are the same color values that are defined as public static properties
        /// in System.Drawing.Color. The hashtable uses strings for the keys, and
        /// ColorBgras for the values.
        /// </summary>
        public static Dictionary<string, ColorBgra> PredefinedColors
        {
            get
            {
                if (predefinedColors != null)
                {
                    Type colorBgraType = typeof(ColorBgra);
                    PropertyInfo[] propInfos = colorBgraType.GetProperties(BindingFlags.Static | BindingFlags.Public);
                    Hashtable colors = new Hashtable();

                    foreach (PropertyInfo pi in propInfos)
                    {
                        if (pi.PropertyType == colorBgraType)
                        {
                            colors.Add(pi.Name, (ColorBgra)pi.GetValue(null, null));
                        }
                    }
                }

                return new Dictionary<string, ColorBgra>(predefinedColors);
            }
        }
    }
}
