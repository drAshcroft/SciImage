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
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Adjustments
{
   
    public sealed class HueAndSaturationAdjustment
        : Effect
    {
        public static string StaticName
        {
            get
            {
                return "Hue / Saturation";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new HueAndSaturationForm();
            t.EffectControl = this;
            return t;
          
        }
        public static Image StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.HueAndSaturationAdjustment.png").Reference;
            }
        }

        public HueAndSaturationAdjustment()
            : base(StaticName,
                   StaticImage,
                   null,
                   EffectFlags.Configurable, "Adjustments")
        {
        }

      

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Hue", 0, -180, +180));
            props.Add(new Int32Property("Saturation", 100, 0, 200));
            props.Add(new Int32Property("Lightness", 0, -100, +100));

            return new PropertyCollection(props);
        }

        private int hue;
        private int saturation;
        private int lightness;
        private UnaryPixelOp pixelOp;

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.hue = newToken.GetProperty<Int32Property>("Hue").Value;
            this.saturation = newToken.GetProperty<Int32Property>("Saturation").Value;
            this.lightness = newToken.GetProperty<Int32Property>("Lightness").Value;

            // map the range [0,100] -> [0,100] and the range [101,200] -> [103,400]
            if (this.saturation > 100)
            {
                this.saturation = ((this.saturation - 100) * 3) + 100;
            }

            if (this.hue == 0 && this.saturation == 100 && this.lightness == 0)
            {
                this.pixelOp = new Identity();
            }
            else
            {
                this.pixelOp = new HueSaturationLightness(this.hue, this.saturation, this.lightness,dstArgs.Surface.ColorPixelBase );
            }

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

      
        public override void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            Surface dst = DstArgs.Surface;
            Surface src = SrcArgs.Surface;

            this.pixelOp.Apply(dst, src, rois, startIndex, length);
        }
    }

   
}
