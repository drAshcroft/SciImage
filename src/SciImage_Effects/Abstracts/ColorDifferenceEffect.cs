/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Drawing;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;

namespace SciImage_Effects.Abstracts
{
    /// <summary>
    /// ColorDifferenctEffect is a base class for my difference effects
    /// that have floating point (double) convolution filters.
    /// its architecture is just like ConvolutionFilterEffect, adding a
    /// function (RenderColorDifferenceEffect) called from Render in each
    /// derived class.
    /// It is also limited to 3x3 kernels.
    /// (Chris Crosetto)
    /// </summary>
    public abstract class ColorDifferenceEffect
        : Effect
    {            
        public unsafe void RenderColorDifferenceEffect(
            double[][] weights, 
            RenderArgs dstArgs, 
            RenderArgs srcArgs, 
            Rectangle[] rois, 
            int startIndex, 
            int length)
        {
            Surface dst = dstArgs.Surface;
            Surface src = srcArgs.Surface;
            ColorPixelBase cout=src.ColorPixelBase.AnotherPixel ();
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Rectangle rect = rois[i];

                // loop through each line of target rectangle
                for (int y = rect.Top; y < rect.Bottom; ++y)
                {
                    int fyStart = 0;
                    int fyEnd = 3;

                    if (y == src.Bounds.Top) fyStart = 1;
                    if (y == src.Bounds.Bottom - 1) fyEnd = 2;

                    

                    for (int x = rect.Left; x < rect.Right; ++x)
                    {
                       
                        int fxStart = 0;
                        int fxEnd = 3;

                        if (x == src.Bounds.Left)
                        {
                            fxStart = 1;
                        }

                        if (x == src.Bounds.Right - 1) 
                        {
                            fxEnd = 2;
                        }

                        // loop through each weight
                        double[] ChannelSums = new double[src.ColorPixelBase.NumChannels];
                        

                        for (int fy = fyStart; fy < fyEnd; ++fy)
                        {
                            for (int fx = fxStart; fx < fxEnd; ++fx)
                            {
                                double weight = weights[fy][fx];
                                ColorPixelBase c = src.GetPoint(x - 1 + fx, y - 1 + fy);
                                for (int ii = 0; ii < src.ColorPixelBase.NumChannels; ii++)
                                {
                                    ChannelSums[ii] += weight * (double)c.GetChannel(i);
                                 //   rSum += weight * (double)c.R;
                                 //   gSum += weight * (double)c.G;
                                 //   bSum += weight * (double)c.B;
                                }
                            }
                        }
                        
                        for (int ii = 0; ii < src.ColorPixelBase.NumChannels; ii++)
                        {
                            cout.SetChannel(ii, (int)ChannelSums[ii] );
                            
                        }
                        cout.alpha =255;
                        dst.SetPoint(x,y,cout);
                    }
                }
            }
        }

        public ColorDifferenceEffect(string name, Image image, string subMenuName, EffectFlags flags)
            : base(name, image, subMenuName, flags, "Effects")
        {
        }
    }    
}   