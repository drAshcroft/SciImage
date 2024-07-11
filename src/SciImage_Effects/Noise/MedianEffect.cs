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

namespace SciImage_Effects.Noise
{
    public sealed class MedianEffect
        : LocalHistogramEffect
    {
        public static string StaticName
        {
            get
            {
                return "Median";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new MedianForm();
            t.EffectControl = this;
            return t;
        }
        public static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MedianEffectIcon.png");
            }
        }

        private int radius;
	    private int percentile;

        public MedianEffect() 
            : base(StaticName, 
                   StaticImage.Reference, 
                   "Noise",
                   EffectFlags.Configurable)
        {
        }


        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Radius", 10, 1, 200));
            props.Add(new Int32Property("Percentile", 50, 0, 100));

            return new PropertyCollection(props);
        }

      

        public unsafe override ColorPixelBase Apply(ColorPixelBase src, int area, int* hb, int* hg, int* hr, int* ha)
        {
	        ColorPixelBase c = GetPercentile(this.percentile, area, hb, hg, hr, ha,src);
            return c;
        }

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.radius = newToken.GetProperty<Int32Property>("Radius").Value;
            this.percentile = newToken.GetProperty<Int32Property>("Percentile").Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
	        foreach (Rectangle rect in rois)
	        {
		        RenderRect(this.radius, SrcArgs.Surface, DstArgs.Surface, rect);
	        }
        }
    }
}
