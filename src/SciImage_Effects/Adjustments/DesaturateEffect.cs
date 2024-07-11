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
using SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Adjustments
{
  
    public sealed class DesaturateEffect
        : Effect 
    {
        private Desaturate desaturateOp;
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            return null;
        }
        protected override PropertyCollection OnCreatePropertyCollection()
        {
            return PropertyCollection.CreateEmpty();
        }

        public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            this.desaturateOp.Apply(dstArgs.Surface, srcArgs.Surface, rois, startIndex, length);
        }

        public DesaturateEffect()
            : base("Black and White",
                   SciImage.SciResources.SciResources.GetImageResource("Icons.DesaturateEffect.png").Reference,
                   null,
                   EffectFlags.None,"Adjustments")
        {
            this.desaturateOp = new Desaturate();
        }
    }
}
