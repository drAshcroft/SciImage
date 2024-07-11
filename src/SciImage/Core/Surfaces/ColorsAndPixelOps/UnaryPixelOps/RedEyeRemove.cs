using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{

    /// <summary>
    /// If the color is within the red tolerance, remove it
    /// </summary>
    [Serializable]
    public class RedEyeRemove
        : UnaryPixelOp
    {
        private int tolerence;
        private double setSaturation;

        public RedEyeRemove(int tol, int sat)
        {
            tolerence = tol;
            setSaturation = (double)sat / 100;
        }

        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            // The higher the saturation, the more red it is
            int saturation = GetSaturation(color);

            // The higher the difference between the other colors, the more red it is
            int difference = color[2] - Math.Max(color[0], color[1]);

            // If it is within tolerence, and the saturation is high
            if ((difference > tolerence) && (saturation > 100))
            {
                double i = 255.0 * color.GetIntensity();
                byte ib = (byte)(i * setSaturation); // adjust the red color for user inputted saturation
                return color.FromBgra((byte)color[0], (byte)color[1], ib, color.alpha);
            }
            else
            {
                return color;
            }
        }

        //Saturation formula from RgbColor.cs, public HsvColor ToHsv()
        private int GetSaturation(ColorPixelBase color)
        {
            double min;
            double max;
            double delta;

            double r = (double)color[2] / 255;
            double g = (double)color[1] / 255;
            double b = (double)color[0] / 255;

            double s;

            min = Math.Min(Math.Min(r, g), b);
            max = Math.Max(Math.Max(r, g), b);
            delta = max - min;

            if (max == 0 || delta == 0)
            {
                // R, G, and B must be 0, or all the same.
                // In this case, S is 0, and H is undefined.
                // Using H = 0 is as good as any...
                s = 0;
            }
            else
            {
                s = delta / max;
            }

            return (int)(s * 255);
        }
    }
}
