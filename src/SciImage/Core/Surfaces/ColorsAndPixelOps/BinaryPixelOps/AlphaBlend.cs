using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.BinaryPixelOps
{
    // This is provided solely for data file format compatibility
    [Obsolete("User UserBlendOps.NormalBlendOp instead", true)]
    [Serializable]
    public class AlphaBlend
        : BinaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs)
        {
            return lhs;
        }
    }
}
