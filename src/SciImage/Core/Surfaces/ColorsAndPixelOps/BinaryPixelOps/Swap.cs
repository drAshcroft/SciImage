using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.BinaryPixelOps
{
    [Serializable]
    public class Swap
         : BinaryPixelOp
    {
        BinaryPixelOp swapMyArgs;

        public override ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs)
        {
            return swapMyArgs.Apply(rhs, lhs);
        }

        public Swap(BinaryPixelOp swapMyArgs)
        {
            this.swapMyArgs = swapMyArgs;
        }
    }
}
