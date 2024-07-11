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
using SciImage.Core.Surfaces.ColorsAndPixelOps.UserBlendOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;
using SciImage_Effects.Adjustments;
using SciImage_Effects.Blurs;

namespace SciImage_Effects.Photo
{
    public sealed class GlowEffect
        : Effect
    {
        public static string StaticName
        {
            get
            {
                return "Glow";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new GlowForm3();
            t.EffectControl = this;
            return t;
        }
        public static Image StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.GlowEffect.png").Reference;
            }
        }


        private GaussianBlurEffect blurEffect = new GaussianBlurEffect();
        private PropertyCollection blurProps;
        private BrightnessAndContrastAdjustment bcAdjustment = new BrightnessAndContrastAdjustment();
        private PropertyCollection bcProps;

        private ScreenBlendOp screenBlendOp = new ScreenBlendOp();

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Radius", 6, 1, 20));
            props.Add(new Int32Property("Brightness", 10, -100, +100));
            props.Add(new Int32Property("Contrast", 10, -100, +100));

            return new PropertyCollection(props);
        }

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            PropertyCollection blurValues = this.blurProps.Clone();
            blurValues["Radius"].Value = newToken.GetProperty<Int32Property>("Radius").Value;
            EffectConfigToken blurToken = new EffectConfigToken(blurValues);
            this.blurEffect.SetRenderInfo(blurToken, dstArgs, srcArgs);

            PropertyCollection bcValues = this.bcProps.Clone();

            bcValues["Brightness"].Value =
                newToken.GetProperty<Int32Property>("Brightness").Value;

            bcValues["Contrast"].Value =
                newToken.GetProperty<Int32Property>("Contrast").Value;

            EffectConfigToken bcToken = new EffectConfigToken(bcValues);
            this.bcAdjustment.SetRenderInfo(bcToken, dstArgs, dstArgs); // have to do adjustment in place, hence dstArgs for both 'args' parameters

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            // First we blur the source, and write the result to the destination RGB32_Surface
            // Then we apply Brightness/Contrast with the input as the dst, and the output as the dst
            // Third, we apply the Screen blend operation so that dst = dst OVER src

            this.blurEffect.Render(parameters,DstArgs,SrcArgs, rois, startIndex, length);
            this.bcAdjustment.Render(parameters, DstArgs, SrcArgs, rois, startIndex, length);

            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Rectangle roi = rois[i];

                for (int y = roi.Top; y < roi.Bottom; ++y)
                {
                    for (int x = roi.Left; x < roi.Right; ++x)
                    {
                        ColorPixelBase dstPtr = DstArgs.Surface.GetPoint(roi.Left, y);
                        ColorPixelBase srcPtr = SrcArgs.Surface.GetPoint(roi.Left, y,dstPtr );

                        DstArgs.Surface[x,y]= screenBlendOp.Apply(dstPtr, srcPtr).ToInt32();
                    }
                }
            }
        }

      

        public GlowEffect()
            : base(StaticName, StaticImage, "Photo", EffectFlags.Configurable, "Effects")
        {
            this.blurEffect = new GaussianBlurEffect();
            this.blurProps = this.blurEffect.CreatePropertyCollection();

            this.bcAdjustment = new BrightnessAndContrastAdjustment();
            this.bcProps = this.bcAdjustment.CreatePropertyCollection();

            this.screenBlendOp = new ScreenBlendOp();
        }
    }
}
