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

namespace SciImage_Effects.Photo
{
    public sealed class SharpenEffect
        : LocalHistogramEffect
    {
        public static string StaticName
        {
            get
            {
                return "Sharpen";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new SharpenForm1();
            t.EffectControl = this;
            return t;
        }
        public static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.SharpenEffect.png");
            }
        }


        public SharpenEffect()
            : base(StaticName, StaticImage.Reference, "Photo", EffectFlags.Configurable)
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Amount", 2, 1, 20));

            return new PropertyCollection(props);
        }

        //protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        //{
        //    ControlInfo configUI = CreateDefaultConfigUI(props);

        //    configUI.SetPropertyControlValue("Amount, ControlInfo"DisplayName, SciImage.SciResource.SciResources.GetString("SharpenEffect.ConfigDialog.SliderLabel"));

        //    return configUI;
        //}

        private int amount;

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.amount = newToken.GetProperty<Int32Property>("Amount").Value;
            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public unsafe override ColorPixelBase Apply(ColorPixelBase src, int area, int* hb, int* hg, int* hr, int* ha)
        {
            ColorPixelBase median = GetPercentile(50, area, hb, hg, hr, ha,src);
            return src.Lerp(src, median, -0.5f);
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            foreach (Rectangle rect in rois)
            {
                RenderRect(this.amount, SrcArgs.Surface, DstArgs.Surface, rect);
            }
        }
    }
}
