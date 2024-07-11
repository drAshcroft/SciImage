using System;
using System.Drawing;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    [Serializable]
    public class ChannelCurve
             : UnaryPixelOp
    {
        public byte[] CurveB = new byte[256];
        public byte[] CurveG = new byte[256];
        public byte[] CurveR = new byte[256];

        public ChannelCurve()
        {
            for (int i = 0; i < 256; ++i)
            {
                CurveB[i] = (byte)i;
                CurveG[i] = (byte)i;
                CurveR[i] = (byte)i;
            }
        }
        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            return color.FromBgra(CurveB[color[0]], CurveG[color[1]], CurveR[color[2]], color.alpha);
        }

        public override void Apply(Surface dst, Point dstOffset, Surface src, Point srcOffset, int scanLength)
        {
            base.Apply(dst, dstOffset, src, srcOffset, scanLength);
        }
    }
}
