using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps
{
    public interface  ColorPixelBase
    {
       // byte red { get; set; }
       // byte green { get; set; }
       // byte blue { get; set; }
        byte alpha { get; set; }
        int SizeOf { get; }

        bool CompareColor(ColorPixelBase testColor, int tolerance);
        int  this[int Channel] { get; set; }

        int NumChannels { get; }
        int GetChannel(int ChannelNumber);
        void SetChannel(int ChannelNumber, int  Value);
        /// <summary>
        /// Clears the pixel values to black
        /// </summary>
        void ZeroPixel();

        double GetIntensity();
        byte GetIntensityByte();
        PixelFormat pixelFormat { get; }

        #region Conversions
            Int64  ToInt64();
            Int32  ToInt32();
            Color ToColor();
            string ToHexString();
            ColorBgra ToColorBgra();
            ColorIntensity ToColorIntensity();
        #endregion

        #region CreatePixelOperations
            ColorPixelBase ClearColor();
            ColorPixelBase BlackColor();
            ColorPixelBase WhiteColor();
            ColorPixelBase TransparentColor();
            ColorPixelBase FromBgra(byte b, byte g , byte r, byte a);
            
            ColorPixelBase FromBgraClamped(int b, int g, int r, int a);
            ColorPixelBase FromBgraClamped(float b, float g, float r, float a);
            ColorPixelBase FromArray(int[] Channels,byte Alpha);
            ColorPixelBase FromArray(long[] Channels, byte Alpha);
            ColorPixelBase FromArray(byte[] Channels, byte Alpha);
            ColorPixelBase FromColor(Color color);
            ColorPixelBase TranslateColor(ColorPixelBase InputPixel);

            ColorPixelBase ParseHexString(string HexString);
        /// <summary>
        /// This assumes an Int32 that has been produced by the same type of pixel.  An int from another pixel type will result in strange behavior
        /// </summary>
        /// <param name="inputLong"></param>
        /// <returns></returns>
            ColorPixelBase TranslateColor(int inputLong);
            ColorPixelBase TranslateColor(uint inputColor);
            ColorPixelBase InvertColor(ColorPixelBase InputColor);
            ColorPixelBase InvertColorAndAlpha(ColorPixelBase InputColor);

        /// <summary>
        /// Takes the input parameter, averages all the channels and returns a new color with the average intensity on the channels;
        /// </summary>
        /// <param name="InputColor"></param>
        /// <returns></returns>
            ColorPixelBase  AverageChannels(ColorPixelBase InputColor);

            
        #endregion

        ColorPixelBase AnotherPixel();
        ColorPixelBase Clone();

        ColorPixelBase Blend(ColorPixelBase ca, ColorPixelBase cb, byte cbAlpha);
        
        ColorPixelBase Blend(ColorPixelBase[] colors);

        ColorPixelBase BlendColors4W16IP(ColorPixelBase c1, uint w1, ColorPixelBase c2, uint w2, ColorPixelBase c3, uint w3, ColorPixelBase c4, uint w4);
        ColorPixelBase BlendColors4W16IP(int c1, uint w1, int c2, uint w2, int c3, uint w3, int c4, uint w4);
        ColorPixelBase Lerp(ColorPixelBase from, ColorPixelBase to, double frac);
        ColorPixelBase Lerp(ColorPixelBase from, ColorPixelBase to, float frac);
        ColorPixelBase NewAlpha(byte Alpha);

    }
}
