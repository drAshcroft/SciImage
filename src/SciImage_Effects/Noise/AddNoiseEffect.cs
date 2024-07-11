/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using SciImage.Core;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Noise
{
    public sealed class AddNoiseEffect
        : Effect
    {
        public static string StaticName
        {
            get
            {
                return "Add Noise";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new AddNoiseForm();
            t.EffectControl = this;
            return t;
        }
        public static Image StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.AddNoiseEffect.png").Reference;
            }
        }

        static AddNoiseEffect()
        {
            InitLookup();
        }

        public AddNoiseEffect()
            : base(StaticName, StaticImage, "Noise", EffectFlags.Configurable, "Effects")
        {
        }

     

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Intensity", 64, 0, 100));
            props.Add(new Int32Property("Saturation", 100, 0, 400));
            props.Add(new DoubleProperty("Coverage", 100, 0, 100));

            return new PropertyCollection(props);
        }

        

        private int intensity;
        private int saturation;
        private double coverage;

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.intensity = newToken.GetProperty<Int32Property>("Intensity").Value;
            this.saturation = newToken.GetProperty<Int32Property>("Saturation").Value;
            this.coverage = 0.01 * newToken.GetProperty<DoubleProperty>("Coverage").Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        private const int tableSize = 16384;
        private static int[] lookup;

        private static double NormalCurve(double x, double scale)
        {
            return scale * Math.Exp(-x * x / 2);
        }

        private static void InitLookup()
        {
            int[] curve = new int[tableSize];
            int[] integral = new int[tableSize];

            double l = 5;
            double r = 10;
            double scale = 50;
            double sum = 0;

            while (r - l > 0.0000001)
            {
                sum = 0;
                scale = (l + r) * 0.5;

                for (int i = 0; i < tableSize; ++i)
                {
                    sum += NormalCurve(16.0 * ((double)i - tableSize / 2) / tableSize, scale);

                    if (sum > 1000000)
                    {
                        break;
                    }
                }

                if (sum > tableSize)
                {
                    r = scale;
                }
                else if (sum < tableSize)
                {
                    l = scale;
                }
                else
                {
                    break;
                }
            }

            lookup = new int[tableSize];
            sum = 0;
            int roundedSum = 0, lastRoundedSum;

            for (int i = 0; i < tableSize; ++i)
            {
                sum += NormalCurve(16.0 * ((double)i - tableSize / 2) / tableSize, scale);
                lastRoundedSum = roundedSum;
                roundedSum = (int)sum;

                for (int j = lastRoundedSum; j < roundedSum; ++j)
                {
                    lookup[j] = (i - tableSize / 2) * 65536 / tableSize;
                }
            }
        }

        [ThreadStatic]
        private static Random threadRand = new Random();

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            int dev = this.intensity * this.intensity / 4;
            int sat = this.saturation * 4096 / 100;

            if (threadRand == null)
            {
                threadRand = new Random(unchecked(System.Threading.Thread.CurrentThread.GetHashCode() ^ 
                    unchecked((int)DateTime.Now.Ticks)));
            }

            Random localRand = threadRand;
            int[] localLookup = lookup;

            for (int ri = startIndex; ri < startIndex + length; ++ri)
            {
                Rectangle rect = rois[ri];

                for (int y = rect.Top; y < rect.Bottom; ++y)
                {
                    //ColorPixelBase *srcPtr = SrcArgs.Surface.GetPointAddressUnchecked(rect.Left, y);
                    //ColorPixelBase *dstPtr = DstArgs.Surface.GetPointAddressUnchecked(rect.Left, y);

                    for (int x = 0; x < rect.Width; ++x)
                    {
                        ColorPixelBase srcPtr = SrcArgs.Surface.GetPoint(rect.Left+x, y);
                        if (localRand.NextDouble() > this.coverage)
                        {
                            DstArgs.Surface.SetPoint(x+rect.Left ,y, srcPtr);
                        }
                        else
                        {
                            int r;
                            int g;
                            int b;
                            int i;

                            r = localLookup[localRand.Next(tableSize)];
                            g = localLookup[localRand.Next(tableSize)];
                            b = localLookup[localRand.Next(tableSize)];

                            i = (4899 * r + 9618 * g + 1867 * b) >> 14; 

                            r = i + (((r - i) * sat) >> 12);
                            g = i + (((g - i) * sat) >> 12);
                            b = i + (((b - i) * sat) >> 12);


                            srcPtr[2] = Utility.ClampToByte(srcPtr[2] + ((r * dev + 32768) >> 16));
                            srcPtr[1] = Utility.ClampToByte(srcPtr[1] + ((g * dev + 32768) >> 16));
                            srcPtr[0] = Utility.ClampToByte(srcPtr[0] + ((b * dev + 32768) >> 16));
                            srcPtr.alpha = srcPtr.alpha;
                            DstArgs.Surface.SetPoint(x + rect.Left, y, srcPtr);
                        }

                    }
                }
            }
        }
    }
}