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
using SciImage.Core;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SciResources;
using SciImage.SystemLayer.Base;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Blurs
{
    public sealed class ZoomBlurEffect
        : Effect 
    {
        public static string StaticName
        {
            get
            {
                return "Zoom Blur";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new ZoomBlurForm2();
            t.EffectControl = this;
            return t;
        }
        public static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.ZoomBlurEffect.png");
            }
        }

        public ZoomBlurEffect()
            : base(StaticName,
                   StaticImage.Reference,
                   "Blurs",
                   EffectFlags.Configurable, "Effects")
        {
        }


        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Amount", 10, 0, 100));

            props.Add(new DoubleVectorProperty(
                "Offset",
                Pair.Create(0.0, 0.0),
                Pair.Create(-2.0, -2.0),
                Pair.Create(+2.0, +2.0)));

            return new PropertyCollection(props);
        }

        
        private int amount;
        private Pair<double, double> offset;

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.amount = newToken.GetProperty<Int32Property>("Amount").Value;
            this.offset = newToken.GetProperty<DoubleVectorProperty>("Offset").Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public unsafe override void Render(EffectConfigToken  parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            Surface dst = dstArgs.Surface;
            Surface src = srcArgs.Surface;
            long w = dst.Width;
            long h = dst.Height;
            long fox = (long)(dst.Width * this.offset.First * 32768.0);
            long foy = (long)(dst.Height * this.offset.Second * 32768.0);
            long fcx = fox + (w << 15);
            long fcy = foy + (h << 15);
            long fz = this.amount;

            const int n = 64;
            
            for (int r = startIndex; r < startIndex + length; ++r)
            {
                Rectangle rect = rois[r];

                for (int y = rect.Top; y < rect.Bottom; ++y)
                {
                    for (int x = rect.Left; x < rect.Right; ++x)
                    {
                        //ColorPixelBase* dstPtr = dst.GetPointAddressUnchecked(rect.Left, y);
                        //ColorPixelBase* srcPtr = src.GetPointAddressUnchecked(rect.Left, y);
                        ColorPixelBase srcP = src.GetPoint(x, y);

                        long fx = (x << 16) - fcx;
                        long fy = (y << 16) - fcy;

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

                        for (int i = 0; i < n; ++i)
                        {
                            fx -= ((fx >> 4) * fz) >> 10;
                            fy -= ((fy >> 4) * fz) >> 10;

                            int u = (int)(fx + fcx + 32768 >> 16);
                            int v = (int)(fy + fcy + 32768 >> 16);

                            if (src.IsVisible(u, v))
                            {
                                ColorPixelBase srcPtr2 = src.GetPoint(u, v);

                                sr += srcPtr2[2]  * srcPtr2.alpha ;
                                sg += srcPtr2[1]  * srcPtr2.alpha ;
                                sb += srcPtr2[0]  * srcPtr2.alpha ;
                                sa += srcPtr2.alpha ;
                                ++sc;
                            }
                        }
                 
                        if (sa != 0)
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
