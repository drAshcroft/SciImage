using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    /// <summary>
    /// Averages the input color's red, green, and blue channels. The alpha component
    /// is unaffected.
    /// </summary>
    [Serializable]
    public class AverageChannels
        : UnaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            return color.AverageChannels(color);
        }
    }
}
