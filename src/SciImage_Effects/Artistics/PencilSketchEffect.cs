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
using SciImage.Core.Surfaces.ColorsAndPixelOps.UserBlendOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SciResources;
using SciImage.SystemLayer.Base.PropertySystem;
using SciImage_Effects.Adjustments;
using SciImage_Effects.Blurs;

namespace SciImage_Effects.Artistics
{
    public sealed class PencilSketchEffect
        : Effect
    {
      
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new PosterizeAdjustmentForm();
            t.EffectControl = this;
            return t;
        }
        private static string StaticName
        {
            get
            {
                return "Pencil Sketch";
            }
        }

        private static ImageResource StaticIcon
        {
            get
            {
                return SciResources.GetImageResource("Icons.PencilSketchEffectIcon.png");
            }
        }

        private GaussianBlurEffect blurEffect;
        private PropertyCollection blurProps;

        private Desaturate desaturateOp = new Desaturate();

        private DesaturateEffect desaturateEffect = new DesaturateEffect();
        private InvertColorsEffect invertEffect = new InvertColorsEffect();

        private BrightnessAndContrastAdjustment bacAdjustment = new BrightnessAndContrastAdjustment();
        private PropertyCollection bacProps;

        private ColorDodgeBlendOp colorDodgeOp = new ColorDodgeBlendOp();

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("PencilTipSize", 2, 1, 20));
            props.Add(new Int32Property("ColorRange", 0, -20, +20));

            return new PropertyCollection(props);
        }

       

        private int pencilTipSize;
        private int colorRange;

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.pencilTipSize = newToken.GetProperty<Int32Property>("PencilTipSize").Value;
            this.colorRange = newToken.GetProperty<Int32Property>("ColorRange").Value;

            EffectConfigToken blurToken = new EffectConfigToken(this.blurProps);
            blurToken.SetPropertyValue("Radius", this.pencilTipSize);
            this.blurEffect.SetRenderInfo(blurToken, dstArgs, srcArgs);

            EffectConfigToken bacToken = new EffectConfigToken(this.bacProps);
            bacToken.SetPropertyValue("Brightness", this.colorRange);
            bacToken.SetPropertyValue("Contrast", -this.colorRange);
            this.bacAdjustment.SetRenderInfo(bacToken, dstArgs, dstArgs);

            this.desaturateEffect.SetRenderInfo(null, dstArgs, dstArgs);

            this.invertEffect.SetRenderInfo(null, dstArgs, dstArgs);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            this.blurEffect.Render(parameters, dstArgs, srcArgs, rois, startIndex, length);
            this.bacAdjustment.Render(parameters, dstArgs, srcArgs, rois, startIndex, length);
            this.invertEffect.Render(parameters, dstArgs, srcArgs, rois, startIndex, length);
            this.desaturateEffect.Render(parameters, dstArgs, srcArgs, rois, startIndex, length);

            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Rectangle roi = rois[i];

                for (int y = roi.Top; y < roi.Bottom; ++y)
                {

                    for (int x = roi.Left; x < roi.Right; ++x)
                    {
                        //ColorPixelBase* srcPtr = srcArgs.Surface.GetPointAddressUnchecked(roi.X, y);
                        //ColorPixelBase* dstPtr = dstArgs.Surface.GetPointAddressUnchecked(roi.X, y);
                        ColorPixelBase srcP = srcArgs.Surface.GetPoint(x, y, dstArgs.Surface.ColorPixelBase);

                        ColorPixelBase srcGrey = this.desaturateOp.Apply(srcP);
                        ColorPixelBase sketched = this.colorDodgeOp.Apply(srcGrey, dstArgs.Surface.GetPoint(x, y));
                        dstArgs.Surface.SetPoint(x, y, sketched);

                    }
                }
            }
        }

        public PencilSketchEffect()            : base(StaticName, StaticIcon.Reference, "Artistic", EffectFlags.Configurable, "Effects")
        {
            this.blurEffect = new GaussianBlurEffect();
            this.blurProps = this.blurEffect.CreatePropertyCollection();

            this.bacAdjustment = new BrightnessAndContrastAdjustment();
            this.bacProps = this.bacAdjustment.CreatePropertyCollection();
        }
    }
}
