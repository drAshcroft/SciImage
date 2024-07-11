namespace SciImage.Core.Surfaces.ColorsAndPixelOps.BinaryPixelOps
{
    /// <summary>
    /// F(lhs, rhs) = lhs.R,g,b + rhs.A
    /// </summary>
    public class SetColorChannels
        : BinaryPixelOp
    {

        public override ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs)
        {
            rhs.alpha = lhs.alpha;
            return rhs;
        }
    }
}
