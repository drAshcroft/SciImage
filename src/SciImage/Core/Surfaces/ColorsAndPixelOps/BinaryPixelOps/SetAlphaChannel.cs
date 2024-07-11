namespace SciImage.Core.Surfaces.ColorsAndPixelOps.BinaryPixelOps
{
    /// <summary>
    /// F(lhs, rhs) = rhs.A + lhs.R,g,b
    /// </summary>
    public class SetAlphaChannel
        : BinaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs)
        {
            lhs.alpha = rhs.alpha;
            return lhs;
        }
    }
}
