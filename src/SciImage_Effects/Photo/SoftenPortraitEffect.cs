/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

// This effect was graciously provided by David Issel, aka BoltBait. His original
// copyright and license (MIT License) are reproduced below.

/*
PortraitEffect.cs 
Copyright (c) 2007 David Issel 
Contact Info: BoltBait@hotmail.com http://www.BoltBait.com 

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions: 

The above copyright notice and this permission notice shall be included in 
all copies or substantial portions of the Software. 

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
THE SOFTWARE. 
*/


using System.Collections.Generic;
using System.Drawing;
using SciImage.Core;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UserBlendOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;
using SciImage_Effects.Adjustments;
using SciImage_Effects.Blurs;

namespace SciImage_Effects.Photo
{
    public sealed class SoftenPortraitEffect
        : Effect
    {
    
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new SoftenPortraitForm();
            t.EffectControl = this;
            return t;
        }
        public static string StaticName
        {
            get
            {
                return "Soften Portrait";
            }
        }

        public static Image StaticIcon
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.SoftenPortraitEffectIcon.png").Reference;
            }
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
 	        List<Property> props = new List<Property>();

            props.Add(new Int32Property("Softness", 5, 0, 10));
            props.Add(new Int32Property("Lighting", 0, -20, +20));
            props.Add(new Int32Property("Warmth", 10, 0, 20));

            return new PropertyCollection(props);
        }


        private GaussianBlurEffect blurEffect;
        private PropertyCollection blurProps;
        private Desaturate desaturateOp;
        private BrightnessAndContrastAdjustment bacAdjustment;
        private PropertyCollection bacProps;
        private OverlayBlendOp overlayOp;

        private int softness;
        private int lighting;
        private int warmth;

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.softness = newToken.GetProperty<Int32Property>("Softness").Value;
            this.lighting = newToken.GetProperty<Int32Property>("Lighting").Value;
            this.warmth = newToken.GetProperty<Int32Property>("Warmth").Value;

            EffectConfigToken blurToken = new EffectConfigToken(this.blurProps);
            blurToken.SetPropertyValue("Radius", this.softness * 3);
            this.blurEffect.SetRenderInfo(blurToken, dstArgs, srcArgs);

            EffectConfigToken bacToken = new EffectConfigToken(this.bacProps);
            bacToken.SetPropertyValue("Brightness", this.lighting);
            bacToken.SetPropertyValue("Contrast", -this.lighting / 2);
            this.bacAdjustment.SetRenderInfo(bacToken, dstArgs, dstArgs);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            float redAdjust = 1.0f + (this.warmth / 100.0f);
            float blueAdjust = 1.0f - (this.warmth / 100.0f);

            this.blurEffect.Render(parameters,DstArgs,SrcArgs, rois, startIndex, length);
            this.bacAdjustment.Render(parameters, DstArgs, SrcArgs, rois, startIndex, length);

            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Rectangle roi = rois[i];

                for (int y = roi.Top; y < roi.Bottom; ++y)
                {
                   

                    for (int x = roi.Left; x < roi.Right; ++x)
                    {
                        //ColorPixelBase* srcPtr = SrcArgs.Surface.GetPointAddress(roi.X, y);
                        ColorPixelBase dstPtr = DstArgs.Surface.GetPoint(x, y);
                        ColorPixelBase srcPtr = SrcArgs.Surface.GetPoint(x, y,dstPtr);
                        ColorPixelBase srcGrey = this.desaturateOp.Apply(srcPtr);

                        srcGrey[2]  = Utility.ClampToByte((int)((float)srcGrey[2]  * redAdjust));
                        srcGrey[0]  = Utility.ClampToByte((int)((float)srcGrey[0]  * blueAdjust));

                        ColorPixelBase mypixel = this.overlayOp.Apply(srcGrey, dstPtr);
                        DstArgs.Surface.SetPoint(x, y, mypixel);
                    }
                }
            }
        }

        public SoftenPortraitEffect()
            : base(StaticName, StaticIcon, "Photo", EffectFlags.Configurable, "Effects")
        {
            this.blurEffect = new GaussianBlurEffect();
            this.blurProps = this.blurEffect.CreatePropertyCollection();

            this.desaturateOp = new Desaturate();

            this.bacAdjustment = new BrightnessAndContrastAdjustment();
            this.bacProps = this.bacAdjustment.CreatePropertyCollection();

            this.overlayOp = new OverlayBlendOp();
        }
    }
}