/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Drawing;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;
using SciImage_Effects.Abstracts;

namespace SciImage_Effects.Noise
{
    public sealed class ReduceNoiseEffect
        : LocalHistogramEffect
    {
        private int radius;
        private double strength;

        public ReduceNoiseEffect()
            : base(StaticName, StaticImage, "Noise", EffectFlags.Configurable)
        {
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new ReduceNoiseForm();
            t.EffectControl = this;
            return t;
        }
        public static string StaticName
        {
            get
            {
                return "Reduce Noise";
            }
        }

        public static Image StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.ReduceNoiseEffectIcon.png").Reference;
            }
        }


        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Radius", 10, 0, 200));
            props.Add(new DoubleProperty("Strength", 0.4, 0, 1));

            return new PropertyCollection(props);
        }

      

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.radius = newToken.GetProperty<Int32Property>("Radius").Value;
            this.strength = -0.2 * newToken.GetProperty<DoubleProperty>("Strength").Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                RenderRect(radius, SrcArgs.Surface, DstArgs.Surface, rois [i]);
            }
        }

        public override unsafe ColorPixelBase Apply(ColorPixelBase color, int area, int* hb, int* hg, int* hr, int* ha)
        {
            ColorPixelBase normalized = GetPercentileOfColor(color, area, hb, hg, hr, ha);
            double lerp = strength * (1 - 0.75 * color.GetIntensity());

            return color.Lerp(color, normalized, lerp);
        }

        private static unsafe ColorPixelBase GetPercentileOfColor(ColorPixelBase color, int area, int* hb, int* hg, int* hr, int* ha)
        {
            int rc = 0;
            int gc = 0;
            int bc = 0;

            for (int i = 0; i < color[2]; ++i)
            {
                rc += hr[i];
            }

            for (int i = 0; i < color[1] ; ++i)
            {
                gc += hg[i];
            }

            for (int i = 0; i < color[0] ; ++i)
            {
                bc += hb[i];
            }

            rc = (rc * 255) / area;
            gc = (gc * 255) / area;
            bc = (bc * 255) / area;

            return color.FromBgra((byte)bc, (byte)gc, (byte)rc,255);
        }
    }
}
