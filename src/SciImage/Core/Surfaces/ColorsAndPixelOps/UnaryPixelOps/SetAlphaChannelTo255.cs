using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    /// <summary>
    /// Specialization of SetAlphaChannel that always sets alpha to 255.
    /// </summary>
    [Serializable]
    public class SetAlphaChannelTo255
        : UnaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            ColorPixelBase c = color.AnotherPixel();
            c.alpha = 255;
            return c;
        }


    }
}
