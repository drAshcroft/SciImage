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
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Distort
{
    public sealed class TwistEffect
        : Effect 
    {
        public static Image StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.TwistEffect.png").Reference;
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new TwistForm();
            t.EffectControl = this;
            return t;
        }
        public static string StaticName
        {
            get
            {
                return "Twist";
            }
        }

        public static string StaticSubMenuName
        {
            get
            {
                return "Distort";
            }
        }

      

        public TwistEffect()
            : base(StaticName, StaticImage, StaticSubMenuName, EffectFlags.Configurable, "Effects")
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new DoubleProperty("Amount", 30.0, -200.0, 200.0));
            props.Add(new DoubleProperty("Size", 1.0, 0.01, 2.0));

            props.Add(new DoubleVectorProperty(
                "Offset",
                Pair.Create(0.0, 0.0),
                Pair.Create(-2.0, -2.0),
                Pair.Create(+2.0, +2.0)));

            props.Add(new Int32Property("Quality", 2, 1, 5));

            return new PropertyCollection(props);
        }


        private double inv100 = 1.0 / 100.0;

        private double amount;
        private double size;
        private int quality;
        private Pair<double, double> offset;

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.amount = -newToken.GetProperty<DoubleProperty>("Amount").Value;
            this.size = 1.0 / newToken.GetProperty<DoubleProperty>("Size").Value;
            this.quality = newToken.GetProperty<Int32Property>("Quality").Value;
            this.offset = newToken.GetProperty<DoubleVectorProperty>("Offset").Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public unsafe override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            double twist = this.amount * this.amount * Math.Sign(this.amount);

            Surface dst = dstArgs.Surface;
            Surface src = srcArgs.Surface;

            float hw = dst.Width / 2.0f;
            hw += (float)(hw * this.offset.First);
            float hh = dst.Height / 2.0f;
            hh += (float)(hh * this.offset.Second);

            //*double maxrad = Math.Min(dst.Width / 2.0, dst.Height / 2.0);
            double invmaxrad = 1.0 / Math.Min(dst.Width / 2.0, dst.Height / 2.0);

            int aaLevel = this.quality;
            int aaSamples = aaLevel * aaLevel;
            PointF* aaPoints = stackalloc PointF[aaSamples];
            Utility.GetRgssOffsets(aaPoints, aaSamples, aaLevel);

            ColorPixelBase[] samples = new ColorPixelBase[aaSamples];

            for (int n = startIndex; n < startIndex + length; ++n)
            {
                Rectangle rect = rois[n];

                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    float j = y - hh;
                    //ColorPixelBase* dstPtr = dst.GetPointAddressUnchecked(rect.Left, y);
                    //ColorPixelBase* srcPtr = src.GetPointAddressUnchecked(rect.Left, y);

                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        //ColorPixelBase* dstPtr = dst.GetPointAddressUnchecked(rect.Left, y);
                        ColorPixelBase srcPtr = src.GetPoint(x, y);

                        float i = x - hw;

                        int sampleCount = 0;

                        for (int p = 0; p < aaSamples; ++p)
                        {
                            float u = i + aaPoints[p].X;
                            float v = j + aaPoints[p].Y;

                            double rad = Math.Sqrt(u * u + v * v);
                            double theta = Math.Atan2(v, u);

                            double t = 1 - ((rad * this.size) * invmaxrad);

                            t = (t < 0) ? 0 : (t * t * t);

                            theta += (t * twist) * inv100;

                            float sampleX = (hw + (float)(rad * Math.Cos(theta)));
                            float sampleY = (hh + (float)(rad * Math.Sin(theta)));

                            samples[sampleCount] = src.GetBilinearSampleClamped(sampleX, sampleY,src.ColorPixelBase );
                            ++sampleCount;
                        }

                         dst.SetPoint(x,y, src.ColorPixelBase.Blend(samples));


                    }
                }
            }
        }
    }
}
