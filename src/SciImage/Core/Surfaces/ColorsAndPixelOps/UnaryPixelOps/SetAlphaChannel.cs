using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{


    /// <summary>
    /// Specialization of SetChannel that sets the alpha channel.
    /// </summary>
    /// <remarks>This class depends on the system being litte-endian with the alpha channel 
    /// occupying the 8 most-significant-bits of a ColorPixelBase instance.
    /// By the way, we use addition instead of bitwise-OR because an addition can be
    /// perform very fast (0.5 cycles) on a Pentium 4.</remarks>
    [Serializable]
    public class SetAlphaChannel
        : UnaryPixelOp
    {
        private byte addValue;

        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            ColorPixelBase c = color.AnotherPixel();
            c.alpha = (byte)(color.alpha + addValue);
            return c;
        }


        public SetAlphaChannel(byte alphaValue)
        {
            addValue = alphaValue;
        }
    }
}
