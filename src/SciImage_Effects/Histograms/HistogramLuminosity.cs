/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Drawing;
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
    public  class HistogramLuminosity 
        : Histogram
    {
        public HistogramLuminosity()
            : base(1, 256)
        {
            this.visualColors = new ColorPixelBase[] { ColorBgra.Black };
        }

        public override ColorPixelBase GetMeanColor() 
        {
            float[] mean = GetMean();
            return SurfacePixel.FromBgra((byte)(mean[0] + 0.5f), (byte)(mean[0] + 0.5f), (byte)(mean[0] + 0.5f),255);
        }

        public override ColorPixelBase GetPercentileColor(float fraction) 
        {
            int[] perc = GetPercentile(fraction);
            return SurfacePixel.FromBgra((byte)(perc[0]), (byte)(perc[0]), (byte)(perc[0]),255);
        }

        protected ColorPixelBase SurfacePixel=ColorBgra.Black  ;
        protected override unsafe void AddSurfaceRectangleToHistogram(Surface surface, Rectangle rect)
        {
            SurfacePixel = surface.ColorPixelBase;
            long[] histogramLuminosity = histogram[0];

            for (int y = rect.Top; y < rect.Bottom; ++y)
            {

               // ColorPixelBase* ptr = surface.GetPointAddressUnchecked(rect.Left, y);
                for (int x = rect.Left; x < rect.Right; ++x)
                {

                    ++histogramLuminosity[surface.GetPoint(x, y).GetIntensityByte()]; //ptr->GetIntensityByte()];
                    //++ptr;
                }
            }
        }

        

        public Level MakeLevelsAuto() 
        {
            ColorPixelBase lo, md, hi;

            lo = GetPercentileColor(0.005f);
            md = GetMeanColor();
            hi = GetPercentileColor(0.995f);

            return Level.AutoFromLoMdHi(lo, md, hi);
        }
    }
}
