/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using SciImage.Core;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Blurs
{
    public sealed class RadialBlurEffect
        : Effect 
    {
        public static string StaticName
        {
            get
            {
                return "Radial Blur";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new RadialBlurForm();
            t.EffectControl = this;
            return t;
        }
        public RadialBlurEffect()
            : base(StaticName,
                   SciImage.SciResources.SciResources.GetImageResource("Icons.RadialBlurEffect.png").Reference,
                   "Blurs",
                   EffectFlags.Configurable, "Effects")
        {
        }

      

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new DoubleProperty("Angle", 2, 0, 360));

            props.Add(new DoubleVectorProperty(
                "Offset",
                Pair.Create(0.0, 0.0),
                Pair.Create(-2.0, -2.0),
                Pair.Create(+2.0, +2.0)));

            props.Add(new Int32Property("Quality", 2, 1, 5));

            return new PropertyCollection(props);
        }

        //protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        //{
        //    ControlInfo configUI = CreateDefaultConfigUI(props);

        //    configUI.SetPropertyControlValue("Angle, ControlInfo"DisplayName, SciImage.SciResource.SciResources.GetString("RadialBlurEffect.ConfigDialog.RadialLabel"));
        //    configUI.FindControlForPropertyName("Angle).ControlType.Value = PropertyControlType.AngleChooser;

        //    configUI.SetPropertyControlValue("Offset, ControlInfo"DisplayName, SciImage.SciResource.SciResources.GetString("RadialBlurEffect.ConfigDialog.OffsetLabel"));
        //    configUI.SetPropertyControlValue("Offset, ControlInfo"SliderSmallChangeX, 0.05);
        //    configUI.SetPropertyControlValue("Offset, ControlInfo"SliderLargeChangeX, 0.25);
        //    configUI.SetPropertyControlValue("Offset, ControlInfo"UpDownIncrementX, 0.01);
        //    configUI.SetPropertyControlValue("Offset, ControlInfo"SliderSmallChangeY, 0.05);
        //    configUI.SetPropertyControlValue("Offset, ControlInfo"SliderLargeChangeY, 0.25);
        //    configUI.SetPropertyControlValue("Offset, ControlInfo"UpDownIncrementY, 0.01);

        //    Surface sourceSurface = this.EnvironmentParameters.SourceSurface;
        //    Bitmap bitmap = sourceSurface.CreateAliasedBitmap();
        //    ImageResource imageResource = ImageResource.FromImage(bitmap);
        //    configUI.SetPropertyControlValue("Offset, ControlInfo"StaticImageUnderlay, imageResource);

        //    configUI.SetPropertyControlValue("Quality, ControlInfo"DisplayName, SciImage.SciResource.SciResources.GetString("RadialBlurEffect.ConfigDialog.QualityLabel"));
        //    configUI.SetPropertyControlValue("Quality, ControlInfo"Description, SciImage.SciResource.SciResources.GetString("RadialBlurEffect.ConfigDialog.QualityDescription"));

        //    return configUI;
        //}

        private static void Rotate(ref int fx, ref int fy, int fr)
        {
            int cx = fx;
            int cy = fy;

            //sin(x) ~~ x
            //cos(x)~~ 1 - x^2/2
            fx = cx - ((cy >> 8) * fr >> 8) - ((cx >> 14) * (fr * fr >> 11) >> 8);
            fy = cy + ((cx >> 8) * fr >> 8) - ((cy >> 14) * (fr * fr >> 11) >> 8);
        }

        private double angle;
        private double offsetX;
        private double offsetY;
        private int quality;

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.angle = newToken.GetProperty<DoubleProperty>("Angle").Value;
            this.offsetX = newToken.GetProperty<DoubleVectorProperty>("Offset").ValueX;
            this.offsetY = newToken.GetProperty<DoubleVectorProperty>("Offset").ValueY;

            this.quality = newToken.GetProperty<Int32Property>("Quality").Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public unsafe override void Render(EffectConfigToken  parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            Surface src = srcArgs.Surface;
            Surface dst = dstArgs.Surface;
            int w = dst.Width;
            int h = dst.Height;
            int fcx = (w << 15) + (int)(this.offsetX * (w << 15));
            int fcy = (h << 15) + (int)(this.offsetY * (h << 15));

            int n = (this.quality * this.quality) * (30 + this.quality * this.quality);

            int fr = (int)(this.angle * Math.PI * 65536.0 / 181.0);

            for (int r = startIndex; r < startIndex + length; ++r)
            {
                Rectangle rect = rois[r];

                for (int y = rect.Top; y < rect.Bottom; ++y)
                {
                   // ColorPixelBase* dstPtr = dst.GetPointAddressUnchecked(rect.Left, y);
                   // ColorPixelBase* srcPtr = src.GetPointAddressUnchecked(rect.Left, y);

                    for (int x = rect.Left; x < rect.Right; ++x)
                    {
                        ColorPixelBase srcP = src.GetPoint(x, y);
                        int fx = (x << 16) - fcx;
                        int fy = (y << 16) - fcy;

                        int fsr = fr / n;

                        int sr = 0;
                        int sg = 0;
                        int sb = 0;
                        int sa = 0;
                        int sc = 0;

                        sr += srcP[2]  * srcP.alpha ;
                        sg += srcP[1]  * srcP.alpha ;
                        sb += srcP[0]  * srcP.alpha ;
                        sa += srcP.alpha ;
                        ++sc;

                        int ox1 = fx;
                        int ox2 = fx;
                        int oy1 = fy;
                        int oy2 = fy;

                        for (int i = 0; i < n; ++i)
                        {
                            Rotate(ref ox1, ref oy1, fsr);
                            Rotate(ref ox2, ref oy2, -fsr);

                            int u1 = ox1 + fcx + 32768 >> 16;
                            int v1 = oy1 + fcy + 32768 >> 16;

                            if (u1 > 0 && v1 > 0 && u1 < w && v1 < h)
                            {
                                ColorPixelBase sample = src.GetPoint(u1, v1);

                                sr += srcP[2] * srcP.alpha;
                                sg += srcP[1] * srcP.alpha;
                                sb += srcP[0] * srcP.alpha;
                                sa += srcP.alpha;
                                ++sc;
                            }

                            int u2 = ox2 + fcx + 32768 >> 16;
                            int v2 = oy2 + fcy + 32768 >> 16;

                            if (u2 > 0 && v2 > 0 && u2 < w && v2 < h)
                            {
                                ColorPixelBase sample = src.GetPoint(u2, v2);

                                sr += srcP[2] * srcP.alpha;
                                sg += srcP[1] * srcP.alpha;
                                sb += srcP[0] * srcP.alpha;
                                sa += srcP.alpha;
                                ++sc;
                            }
                        }

                        if (sa > 0)
                        {
                            dst.SetPoint(x,y, src.ColorPixelBase.FromBgra(
                                Utility.ClampToByte(sb / sa),
                                Utility.ClampToByte(sg / sa),
                                Utility.ClampToByte(sr / sa),
                                Utility.ClampToByte(sa / sc)));
                        }
                        else
                        {
                            dst[x,y]=0;
                        }

                       
                    }
                }
            }
        }
    }
}
