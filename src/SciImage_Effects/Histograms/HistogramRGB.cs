/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using SciImage.Core;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps;
using SciImage.PaintForms.UserControls.Widgets;

namespace SciImage_Effects.Histograms
{
    /// <summary>
    /// Histogram is used to calculate a histogram for a RGB32_Surface (in a selection,
    /// if desired). This can then be used to retrieve percentile, average, peak,
    /// and distribution information.
    /// </summary>
    public sealed class HistogramRgb 
        : Histogram
    {
        public HistogramRgb()
            : base(3, 256)
        {
            visualColors = new ColorPixelBase[]{     
                                              ColorBgra.Blue ,
                                              ColorBgra.Green,
                                              ColorBgra.Red
                                          };
        }

        public override ColorPixelBase GetMeanColor() 
        {
            float[] mean = GetMean();
            return SurfacePixel.FromBgra((byte)(mean[0] + 0.5f), (byte)(mean[1] + 0.5f), (byte)(mean[2] + 0.5f),255);
        }

        public override ColorPixelBase GetPercentileColor(float fraction) 
        {
            int[] perc = GetPercentile(fraction);

            return SurfacePixel.FromBgra((byte)(perc[0]), (byte)(perc[1]), (byte)(perc[2]),255);
        }

        protected ColorPixelBase SurfacePixel = ColorBgra.Black;
        protected override unsafe void AddSurfaceRectangleToHistogram(Surface surface, Rectangle rect)
        {
            SurfacePixel = surface.ColorPixelBase;
            long[] histogramB = histogram[0];
            long[] histogramG = histogram[1];
            long[] histogramR = histogram[2];

            for (int y = rect.Top; y < rect.Bottom; ++y)
            {
                
                for (int x = rect.Left; x < rect.Right; ++x)
                {
                    ColorPixelBase ptr = surface.GetPoint(x, y);
                    ++histogramB[ptr[0] ];
                    ++histogramG[ptr[1] ];
                    ++histogramR[ptr[2] ];
                    
                }
            }
        }

        public void SetFromLeveledHistogram(HistogramRgb inputHistogram, Level upo)
        {
            if (inputHistogram == null || upo == null) 
            {
                return;
            }

            Clear();

            float[] before = new float[3];
            float[] slopes = new float[3];

            for (int c = 0; c < 3; c++)
            {
                long[] channelHistogramOutput = histogram[c];
                long[] channelHistogramInput = inputHistogram.histogram[c];

                for (int v = 0; v <= 255; v++)
                {
                    ColorPixelBase after = SurfacePixel.FromBgra((byte)v, (byte)v, (byte)v,255);

                    upo.UnApply(after, before, slopes);

                    if (after[c] > upo.ColorOutHigh[c]
                        || after[c] < upo.ColorOutLow[c]
                        || (int)Math.Floor(before[c]) < 0
                        || (int)Math.Ceiling(before[c]) > 255
                        || float.IsNaN(before[c])) 
                    {
                        channelHistogramOutput[v] = 0;
                    }
                    else if (before[c] <= upo.ColorInLow[c]) 
                    {
                        channelHistogramOutput[v] = 0;

                        for (int i = 0; i <= upo.ColorInLow[c]; i++)
                        {
                            channelHistogramOutput[v] += channelHistogramInput[i];
                        }
                    } 
                    else if (before[c] >= upo.ColorInHigh[c])
                    {
                        channelHistogramOutput[v] = 0;

                        for (int i = (int)upo.ColorInHigh[c]; i < 256; i++)
                        {
                            channelHistogramOutput[v] += channelHistogramInput[i];
                        }
                    }
                    else
                    {
                        channelHistogramOutput[v] = (int)(slopes[c] * Utility.Lerp(
                            channelHistogramInput[(int)Math.Floor(before[c])],
                            channelHistogramInput[(int)Math.Ceiling(before[c])],
                            before[c] - Math.Floor(before[c])));
                    }
                }
            }

            OnHistogramUpdated();
        }

        public Level MakeLevelsAuto() 
        {
            ColorPixelBase lo = GetPercentileColor(0.005f);
            ColorPixelBase md = GetMeanColor();
            ColorPixelBase hi = GetPercentileColor(0.995f);

            return Level.AutoFromLoMdHi(lo, md, hi);
        }
    }
}
