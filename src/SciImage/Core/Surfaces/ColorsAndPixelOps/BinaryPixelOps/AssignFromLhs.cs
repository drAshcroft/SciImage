using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.BinaryPixelOps
{
    /// <summary>
    /// result(lhs,rhs) = lhs
    /// </summary>
    [Serializable]
    public class AssignFromLhs
        : BinaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs)
        {
            return lhs;
        }

        public AssignFromLhs()
        {
        }
    }
}
