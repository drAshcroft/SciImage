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
using SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Photo
{
    public sealed class RedEyeRemoveEffect
        : Effect 
    {
       
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new RedEyeRemoveForm2();
            t.EffectControl = this;
            return t;
        }
        private int tolerance;
        private int saturation;
        private PixelOp redEyeOp;

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Tolerance", 70, 0, 100));
            props.Add(new Int32Property("Saturation", 90, 0, 100));

            return new PropertyCollection(props);
        }

    

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.tolerance = newToken.GetProperty<Int32Property>("Tolerance").Value;
            this.saturation = newToken.GetProperty<Int32Property>("Saturation").Value;

            this.redEyeOp = new RedEyeRemove(this.tolerance, this.saturation);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            this.redEyeOp.Apply(DstArgs.Surface, SrcArgs.Surface, rois, startIndex, length);
        }

        public RedEyeRemoveEffect()
            : base("Red Eye Removal",
                   SciImage.SciResources.SciResources.GetImageResource("Icons.RedEyeRemoveEffect.png").Reference,
                   "Photo",
                   EffectFlags.Configurable, "Effects")
        {
        }
    }
}