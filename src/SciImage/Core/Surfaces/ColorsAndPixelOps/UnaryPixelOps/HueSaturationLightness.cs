using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    [Serializable]
    public class HueSaturationLightness
            : UnaryPixelOp
    {
        private int hueDelta;
        private int satFactor;
        private UnaryPixelOp blendOp;

        public HueSaturationLightness(int hueDelta, int satDelta, int lightness, ColorPixelBase RequestedColor)
        {
            this.hueDelta = hueDelta;
            this.satFactor = (satDelta * 1024) / 100;

            if (lightness == 0)
            {
                blendOp = new UnaryPixelOps.Identity();
            }
            else if (lightness > 0)
            {
                blendOp = new UnaryPixelOps.BlendConstant(RequestedColor.FromBgra(255, 255, 255, (byte)((lightness * 255) / 100)));
            }
            else // if (lightness < 0)
            {
                blendOp = new UnaryPixelOps.BlendConstant(RequestedColor.FromBgra(0, 0, 0, (byte)((-lightness * 255) / 100)));
            }
        }

        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            //adjust saturation
            byte intensity = color.GetIntensityByte();
            color[2] = Utility.ClampToByte((intensity * 1024 + (color[2] - intensity) * satFactor) >> 10);
            color[1] = Utility.ClampToByte((intensity * 1024 + (color[1] - intensity) * satFactor) >> 10);
            color[0] = Utility.ClampToByte((intensity * 1024 + (color[0] - intensity) * satFactor) >> 10);

            HsvColor hsvColor = HsvColor.FromColor(color.ToColor());
            int hue = hsvColor.Hue;

            hue += hueDelta;

            while (hue < 0)
            {
                hue += 360;
            }

            while (hue > 360)
            {
                hue -= 360;
            }

            hsvColor.Hue = hue;

            ColorPixelBase newColor = color.FromColor(hsvColor.ToColor());
            newColor = blendOp.Apply(newColor);
            newColor.alpha = color.alpha;

            return newColor;
        }
    }
}
