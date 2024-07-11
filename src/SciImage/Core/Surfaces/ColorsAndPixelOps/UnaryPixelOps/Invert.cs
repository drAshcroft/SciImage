using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    /// <summary>
    /// Inverts a pixel's color, and passes through the alpha component.
    /// </summary>
    [Serializable]
    public class Invert
        : UnaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            ColorPixelBase c = color.InvertColor(color);

            return c; //
        }
    }
}
