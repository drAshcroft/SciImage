using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{

    /// <summary>
    /// Always returns a constant color.
    /// </summary>
    [Serializable]
    public class Constant
        : UnaryPixelOp
    {
        private ColorPixelBase setColor;

        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            return setColor;
        }



        public Constant(ColorPixelBase setColor)
        {
            this.setColor = setColor;
        }
    }
}
