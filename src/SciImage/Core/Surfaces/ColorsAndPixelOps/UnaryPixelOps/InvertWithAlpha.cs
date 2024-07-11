using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    /// <summary>
    /// Inverts a pixel's color and its alpha component.
    /// </summary>
    [Serializable]
    public class InvertWithAlpha
        : UnaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            return color.InvertColorAndAlpha(color);

        }
    }
}
