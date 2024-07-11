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
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Adjustments
{
   
    public sealed class SepiaEffect
        : Effect 
    {
        private UnaryPixelOp levels;
        private UnaryPixelOp desaturate;

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            return PropertyCollection.CreateEmpty();
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            return null;
        }
        public override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            this.desaturate.Apply(dstArgs.Surface, srcArgs.Surface, rois, startIndex, length);
            this.levels.Apply(dstArgs.Surface, dstArgs.Surface, rois, startIndex, length);
        }

        public SepiaEffect()
            : base("Sepia",
                   SciImage.SciResources.SciResources.GetImageResource("Icons.SepiaEffect.png").Reference,
                   null,
                   EffectFlags.None, "Adjustments")
        {
            this.desaturate = new Desaturate();

            this.levels = new Level(
                ColorBgra.Black,
                ColorBgra.White,
                new float[] { 1.2f, 1.0f, 0.8f },
                ColorBgra.Black,
                ColorBgra.White);
        }
    }
}
