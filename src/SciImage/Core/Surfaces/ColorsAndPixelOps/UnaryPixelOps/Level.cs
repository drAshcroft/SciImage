using System;
using System.Drawing;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    [Serializable]
    public class Level
          : UnaryPixelOps.ChannelCurve,
            ICloneable
    {
        private ColorPixelBase colorInLow;
        public ColorPixelBase ColorInLow
        {
            get
            {
                return colorInLow;
            }

            set
            {
                if (value[2] == 255)
                {
                    value[2] = 254;
                }

                if (value[1] == 255)
                {
                    value[1] = 254;
                }

                if (value[0] == 255)
                {
                    value[0] = 254;
                }

                if (colorInHigh[2] < value[2] + 1)
                {
                    colorInHigh[2] = (byte)(value[2] + 1);
                }

                if (colorInHigh[1] < value[1] + 1)
                {
                    colorInHigh[1] = (byte)(value[2] + 1);
                }

                if (colorInHigh[0] < value[0] + 1)
                {
                    colorInHigh[0] = (byte)(value[2] + 1);
                }

                colorInLow = value;
                UpdateLookupTable();
            }
        }

        private ColorPixelBase colorInHigh;
        public ColorPixelBase ColorInHigh
        {
            get
            {
                return colorInHigh;
            }

            set
            {
                if (value[2] == 0)
                {
                    value[2] = 1;
                }

                if (value[1] == 0)
                {
                    value[1] = 1;
                }

                if (value[0] == 0)
                {
                    value[0] = 1;
                }

                if (colorInLow[2] > value[2] - 1)
                {
                    colorInLow[2] = (byte)(value[2] - 1);
                }

                if (colorInLow[1] > value[1] - 1)
                {
                    colorInLow[1] = (byte)(value[2] - 1);
                }

                if (colorInLow[0] > value[0] - 1)
                {
                    colorInLow[0] = (byte)(value[2] - 1);
                }

                colorInHigh = value;
                UpdateLookupTable();
            }
        }

        private ColorPixelBase colorOutLow;
        public ColorPixelBase ColorOutLow
        {
            get
            {
                return colorOutLow;
            }

            set
            {
                if (value[2] == 255)
                {
                    value[2] = 254;
                }

                if (value[1] == 255)
                {
                    value[1] = 254;
                }

                if (value[0] == 255)
                {
                    value[0] = 254;
                }

                if (colorOutHigh[2] < value[2] + 1)
                {
                    colorOutHigh[2] = (byte)(value[2] + 1);
                }

                if (colorOutHigh[1] < value[1] + 1)
                {
                    colorOutHigh[1] = (byte)(value[1] + 1);
                }

                if (colorOutHigh[0] < value[0] + 1)
                {
                    colorOutHigh[0] = (byte)(value[0] + 1);
                }

                colorOutLow = value;
                UpdateLookupTable();
            }
        }

        private ColorPixelBase colorOutHigh;
        public ColorPixelBase ColorOutHigh
        {
            get
            {
                return colorOutHigh;
            }

            set
            {
                if (value[2] == 0)
                {
                    value[2] = 1;
                }

                if (value[1] == 0)
                {
                    value[1] = 1;
                }

                if (value[0] == 0)
                {
                    value[0] = 1;
                }

                if (colorOutLow[2] > value[2] - 1)
                {
                    colorOutLow[2] = (byte)(value[2] - 1);
                }

                if (colorOutLow[1] > value[1] - 1)
                {
                    colorOutLow[1] = (byte)(value[1] - 1);
                }

                if (colorOutLow[0] > value[0] - 1)
                {
                    colorOutLow[0] = (byte)(value[0] - 1);
                }

                colorOutHigh = value;
                UpdateLookupTable();
            }
        }

        private float[] gamma = new float[3];
        public float GetGamma(int index)
        {
            if (index < 0 || index >= 3)
            {
                throw new ArgumentOutOfRangeException("index", index, "Index must be between 0 and 2");
            }

            return gamma[index];
        }

        public void SetGamma(int index, float val)
        {
            if (index < 0 || index >= 3)
            {
                throw new ArgumentOutOfRangeException("index", index, "Index must be between 0 and 2");
            }

            gamma[index] = Utility.Clamp(val, 0.1f, 10.0f);
            UpdateLookupTable();
        }

        public bool isValid = true;

        public static Level AutoFromLoMdHi(ColorPixelBase lo, ColorPixelBase md, ColorPixelBase hi)
        {
            float[] gamma = new float[3];

            for (int i = 0; i < 3; i++)
            {
                if (lo[i] < md[i] && md[i] < hi[i])
                {
                    gamma[i] = (float)Utility.Clamp(Math.Log(0.5, (float)(md[i] - lo[i]) / (float)(hi[i] - lo[i])), 0.1, 10.0);
                }
                else
                {
                    gamma[i] = 1.0f;
                }
            }

            return new Level(lo, hi, gamma, lo.FromColor(Color.Black), lo.FromColor(Color.White));
        }

        private void UpdateLookupTable()
        {
            for (int i = 0; i < 3; i++)
            {
                if (colorOutHigh[i] < colorOutLow[i] ||
                    colorInHigh[i] <= colorInLow[i] ||
                    gamma[i] < 0)
                {
                    isValid = false;
                    return;
                }

                for (int j = 0; j < 256; j++)
                {
                    ColorPixelBase col = Apply(j, j, j, colorOutHigh);
                    CurveB[j] = (byte)col[0];
                    CurveG[j] = (byte)col[1];
                    CurveR[j] = (byte)col[2];
                }
            }
        }

        /* public Level() 
             : this(ColorPixelBase.FromColor(Color.Black),
                    ColorPixelBase.FromColor(Color.White),
                    new float[] { 1, 1, 1 },
                    ColorPixelBase.FromColor(Color.Black),
                    ColorPixelBase.FromColor(Color.White))
         {
         }*/

        public Level(ColorPixelBase in_lo, ColorPixelBase in_hi, float[] gamma, ColorPixelBase out_lo, ColorPixelBase out_hi)
        {
            colorInLow = in_lo;
            colorInHigh = in_hi;
            colorOutLow = out_lo;
            colorOutHigh = out_hi;

            if (gamma.Length != 3)
            {
                throw new ArgumentException("gamma", "gamma must be a float[3]");
            }

            this.gamma = gamma;
            UpdateLookupTable();
        }

        public ColorPixelBase Apply(float r, float g, float b, ColorPixelBase RequiredFormat)
        {
            ColorPixelBase ret = RequiredFormat.AnotherPixel();
            float[] input = new float[] { b, g, r };

            for (int i = 0; i < 3; i++)
            {
                float v = (input[i] - colorInLow[i]);

                if (v < 0)
                {
                    ret[i] = colorOutLow[i];
                }
                else if (v + colorInLow[i] >= colorInHigh[i])
                {
                    ret[i] = colorOutHigh[i];
                }
                else
                {
                    ret[i] = (byte)Utility.Clamp(
                        colorOutLow[i] + (colorOutHigh[i] - colorOutLow[i]) * Math.Pow(v / (colorInHigh[i] - colorInLow[i]), gamma[i]),
                        0.0f,
                        255.0f);
                }
            }

            return ret;
        }

        public void UnApply(ColorPixelBase after, float[] beforeOut, float[] slopesOut)
        {
            if (beforeOut.Length != 3)
            {
                throw new ArgumentException("before must be a float[3]", "before");
            }

            if (slopesOut.Length != 3)
            {
                throw new ArgumentException("slopes must be a float[3]", "slopes");
            }

            for (int i = 0; i < 3; i++)
            {
                beforeOut[i] = colorInLow[i] + (colorInHigh[i] - colorInLow[i]) *
                    (float)Math.Pow((float)(after[i] - colorOutLow[i]) / (colorOutHigh[i] - colorOutLow[i]), 1 / gamma[i]);

                slopesOut[i] = (float)(colorInHigh[i] - colorInLow[i]) / ((colorOutHigh[i] - colorOutLow[i]) * gamma[i]) *
                    (float)Math.Pow((float)(after[i] - colorOutLow[i]) / (colorOutHigh[i] - colorOutLow[i]), 1 / gamma[i] - 1);

                if (float.IsInfinity(slopesOut[i]) || float.IsNaN(slopesOut[i]))
                {
                    slopesOut[i] = 0;
                }
            }
        }

        public object Clone()
        {
            Level copy = new Level(colorInLow, colorInHigh, (float[])gamma.Clone(), colorOutLow, colorOutHigh);

            copy.CurveB = (byte[])this.CurveB.Clone();
            copy.CurveG = (byte[])this.CurveG.Clone();
            copy.CurveR = (byte[])this.CurveR.Clone();

            return copy;
        }
    }
}
