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
using SciImage_Effects.Histograms;

namespace SciImage_Effects.Adjustments
{
   
    public sealed class AutoLevelEffect
        : Effect 
    {
        private Level levels = null;

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            return PropertyCollection.CreateEmpty();
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            return null;
        }
        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            HistogramRgb histogram = new HistogramRgb();
            histogram.UpdateHistogram(srcArgs.Surface, this.EnvironmentParameters.GetSelection(dstArgs.Bounds));
            this.levels = histogram.MakeLevelsAuto();

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            if (this.levels.isValid)
            {
                this.levels.Apply(dstArgs.Surface, srcArgs.Surface, rois, startIndex, length);
            }
        }

        public AutoLevelEffect()
            : base("Auto-Level",
                   SciImage.SciResources.SciResources.GetImageResource("Icons.AutoLevel.png").Reference,
                   null,
                   EffectFlags.None,"Adjustments")
        {
        }
    }


   
}
