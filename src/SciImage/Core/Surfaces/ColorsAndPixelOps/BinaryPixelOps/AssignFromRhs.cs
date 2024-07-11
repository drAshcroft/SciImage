using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.BinaryPixelOps
{
    /// <summary>
    /// result(lhs,rhs) = rhs
    /// </summary>
    [Serializable]
    public class AssignFromRhs
        : BinaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs)
        {
            return rhs;
        }

        /*public unsafe override void Apply(ColorPixelBase* dst, ColorPixelBase* lhs, ColorPixelBase* rhs, int length)
        {
            Memory.Copy(dst, rhs, (ulong)length * (ulong)ColorPixelBase.SizeOf);
        }

        public unsafe override void Apply(ColorPixelBase* dst, ColorPixelBase* src, int length)
        {
            Memory.Copy(dst, src, (ulong)length * (ulong)ColorPixelBase.SizeOf);
        }*/

        public AssignFromRhs()
        {
        }
    }
}
