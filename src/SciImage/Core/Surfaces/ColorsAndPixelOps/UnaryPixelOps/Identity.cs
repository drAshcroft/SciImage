using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    /// <summary>
    /// Passes through the given color value.
    /// result(color) = color
    /// </summary>
    [Serializable]
    public class Identity
        : UnaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            return color;
        }
        public override ColorPixelBase Apply(ColorPixelBase dst, ColorPixelBase src)
        {
            return src;
        }

    }
}
