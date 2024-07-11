using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    /// <summary>
    /// Blends pixels with the specified constant color.
    /// </summary>
    [Serializable]
    public class BlendConstant
        : UnaryPixelOp
    {
        private ColorPixelBase blendColor;

        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            int a = blendColor.alpha;
            int invA = 255 - a;

            long[] ColorChannels = new long[color.NumChannels];
            for (int i = 0; i < ColorChannels.Length; i++)
                ColorChannels[i] = ((color.GetChannel(i) * invA) + (color.GetChannel(i) * a)) / 256;

            byte a2 = ComputeAlpha(color.alpha, blendColor.alpha);

            return color.FromArray(ColorChannels, a2);
        }

        public BlendConstant(ColorPixelBase blendColor)
        {
            this.blendColor = blendColor;
        }
    }
}
