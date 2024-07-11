using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    [Serializable]
    public class LuminosityCurve
            : UnaryPixelOp
    {
        public byte[] Curve = new byte[256];

        public LuminosityCurve()
        {
            for (int i = 0; i < 256; ++i)
            {
                Curve[i] = (byte)i;
            }
        }

        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            byte lumi = color.GetIntensityByte();
            int diff = Curve[lumi] - lumi;

            return color.FromBgraClamped(
                color[0] + diff,
                color[1] + diff,
                color[2] + diff,
                color.alpha);
        }
    }
}
