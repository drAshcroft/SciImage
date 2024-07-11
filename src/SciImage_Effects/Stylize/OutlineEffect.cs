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
using SciImage.SciResources;
using SciImage.SystemLayer.Base.PropertySystem;
using SciImage_Effects.Abstracts;

namespace SciImage_Effects.Stylize
{
    public sealed class OutlineEffect
        : LocalHistogramEffect
    {
        public static string StaticName
        {
            get
            {
                return "Outline";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new OutlineForm();
            t.EffectControl = this;
            return t;
        }
        public static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.OutlineEffectIcon.png");
            }
        }


        private int thickness;
        private int intensity;

        public OutlineEffect()
            : base(StaticName, StaticImage.Reference, "Stylize", EffectFlags.Configurable)
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Thickness", 3, 1, 200));
            props.Add(new Int32Property("Intensity", 50, 0, 100));

            return new PropertyCollection(props);
        }

       

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.thickness = newToken.GetProperty<Int32Property>("Thickness").Value;
            this.intensity = newToken.GetProperty<Int32Property>("Intensity").Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public unsafe override ColorPixelBase Apply(ColorPixelBase src, int area, int* hb, int* hg, int* hr, int* ha)
        {
            int minCount1 = area * (100 - this.intensity) / 200;
            int minCount2 = area * (100 + this.intensity) / 200;

            int bCount = 0;
            int b1 = 0;
            while (b1 < 255 && hb[b1] == 0)
            {
                ++b1;
            }

            while (b1 < 255 && bCount < minCount1)
            {
                bCount += hb[b1];
                ++b1;
            }

            int b2 = b1;
            while (b2 < 255 && bCount < minCount2)
            {
                bCount += hb[b2];
                ++b2;
            }

            int gCount = 0;
            int g1 = 0;
            while (g1 < 255 && hg[g1] == 0)
            {
                ++g1;
            }

            while (g1 < 255 && gCount < minCount1)
            {
                gCount += hg[g1];
                ++g1;
            }

            int g2 = g1;
            while (g2 < 255 && gCount < minCount2)
            {
                gCount += hg[g2];
                ++g2;
            }

            int rCount = 0;
            int r1 = 0;
            while (r1 < 255 && hr[r1] == 0)
            {
                ++r1;
            }

            while (r1 < 255 && rCount < minCount1)
            {
                rCount += hr[r1];
                ++r1;
            }

            int r2 = r1;
            while (r2 < 255 && rCount < minCount2)
            {
                rCount += hr[r2];
                ++r2;
            }

            int aCount = 0;
            int a1 = 0;
            while (a1 < 255 && hb[a1] == 0)
            {
                ++a1;
            }

            while (a1 < 255 && aCount < minCount1)
            {
                aCount += ha[a1];
                ++a1;
            }

            int a2 = a1;
            while (a2 < 255 && aCount < minCount2)
            {
                aCount += ha[a2];
                ++a2;
            }

            return src.FromBgra(
                (byte)(255 - (b2 - b1)),
                (byte)(255 - (g2 - g1)),
                (byte)(255 - (r2 - r1)),
                (byte)(a2));
        }

        public unsafe override void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            foreach (Rectangle rect in rois)
            {
                RenderRect(this.thickness, SrcArgs.Surface, DstArgs.Surface, rect);
            }
        }
    }
}
