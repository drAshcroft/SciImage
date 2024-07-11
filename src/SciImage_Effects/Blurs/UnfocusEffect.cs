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

namespace SciImage_Effects.Blurs
{
    public sealed class UnfocusEffect
        : LocalHistogramEffect
    {
        public static string StaticName
        {
            get
            {
                return "Unfocus";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new UnfocusForm();
            t.EffectControl = this;
            return t;
        }
        public static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.UnfocusEffectIcon.png");
            }
        }

        public UnfocusEffect() 
            : base(StaticName, 
                   StaticImage.Reference,
                   "Blurs",
                   EffectFlags.Configurable)
        { 
        }

     

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Radius", 4, 1, 200));

            return new PropertyCollection(props);
        }

     
        private int radius;

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.radius = newToken.GetProperty<Int32Property>("Radius").Value;
            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public unsafe override ColorPixelBase ApplyWithAlpha(ColorPixelBase src, int area, int sum, int* hb, int* hg, int* hr)
        {
            //each slot of the histgram can contain up to area * 255. This will overflow an int when area > 32k
            if (area < 32768)
            {
                int b = 0;
                int g = 0;
                int r = 0;

                for (int i = 1; i < 256; ++i)
                {
                    b += i * hb[i];
                    g += i * hg[i];
                    r += i * hr[i];
                }

                int alpha = sum / area;
                int div = area * 255;

                return src.FromBgraClamped(b / div, g / div, r / div, alpha);
            }
            else //use a long if an int will overflow.
            {
                long b = 0;
                long g = 0;
                long r = 0;

                for (long i = 1; i < 256; ++i)
                {
                    b += i * hb[i];
                    g += i * hg[i];
                    r += i * hr[i];
                }

                int alpha = sum / area;
                int div = area * 255;

                return src.FromBgraClamped(b / div, g / div, r / div, alpha);
            }
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            foreach (Rectangle rect in rois)
            {
                RenderRectWithAlpha(this.radius, SrcArgs.Surface, DstArgs.Surface, rect);
            }
        }
    }
}
