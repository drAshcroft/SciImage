using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    [Serializable]
    public class Desaturate
            : UnaryPixelOp
    {
        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            byte i = color.GetIntensityByte();
            return color.FromBgra(i, i, i, color.alpha);
        }
    }
}
