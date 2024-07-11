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
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Render
{
    public sealed class JuliaFractalEffect
        : Effect 
    {
    
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new JuliaFractalForm();
            t.EffectControl = this;
            return t;
        }
        public static string StaticName
        {
            get
            {
                return "Julia Fractal";
            }
        }

        public static Image StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.JuliaFractalEffectIcon.png").Reference;
            }
        }

        private double factor;
        private double zoom;
        private double angle;
        private double angleTheta;
        private int quality;

        private static readonly double log2_10000 = Math.Log(10000);

        public JuliaFractalEffect()
            : base(StaticName, 
                   StaticImage, 
                   "Render", 
                   EffectFlags.Configurable, "Effects")
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new DoubleProperty("Factor", 4.0, 1.0, 10.0));
            props.Add(new DoubleProperty("Zoom", 1, 0.1, 50));
            props.Add(new DoubleProperty("Angle", 0.0, -180.0, +180.0));
            props.Add(new Int32Property("Quality", 2, 1, 5));

            return new PropertyCollection(props, new PropertyCollectionRule[0]);
        }

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.zoom = newToken.GetProperty<DoubleProperty>("Zoom").Value;
            this.quality = newToken.GetProperty<Int32Property>("Quality").Value;
            this.angle = newToken.GetProperty<DoubleProperty>("Angle").Value;
            this.angleTheta = (this.angle * Math.PI * 2) / 360.0;
            this.factor = newToken.GetProperty<DoubleProperty>("Factor").Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

       

        private static double Julia(double x, double y, double r, double i)
        {
            double c = 0;

            while (c < 256 && x * x + y * y < 10000)
            {
                double t = x;
                x = x * x - y * y + r;
                y = 2 * t * y + i;
                ++c;
            }

            c -= 2 - 2 * log2_10000 / Math.Log(x * x + y * y);

            return c;
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            const double jr = 0.3125;
            const double ji = 0.03;

            int w = dstArgs.Width;
            int h = dstArgs.Height;
            double invH = 1.0 / h;
            double invZoom = 1.0 / this.zoom;
            double invQuality = 1.0 / this.quality;
            double aspect = (double)h / (double)w;
            int count = this.quality * this.quality + 1;
            double invCount = 1.0 / (double)count;

            for (int ri = startIndex; ri < startIndex + length; ++ri)
            {
                Rectangle rect = rois[ri];

                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                   // ColorPixelBase* dstPtr = dstArgs.Surface.GetPointAddressUnchecked(rect.Left, y);

                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        int r = 0;
                        int g = 0;
                        int b = 0;
                        int a = 0;

                        for (double i = 0; i < count; i++)
                        {
                            double u = (2.0 * x - w + (i * invCount)) * invH;
                            double v = (2.0 * y - h + ((i * invQuality) % 1)) * invH;

                            double radius = Math.Sqrt((u * u) + (v * v));
                            double radiusP = radius;
                            double theta = Math.Atan2(v, u);
                            double thetaP = theta + this.angleTheta;

                            double uP = radiusP * Math.Cos(thetaP);
                            double vP = radiusP * Math.Sin(thetaP);

                            double jX = (uP - vP * aspect) * invZoom;
                            double jY = (vP + uP * aspect) * invZoom;

                            double j = Julia(jX, jY, jr, ji);

                            double c = this.factor * j;

                            b += Utility.ClampToByte(c - 768);
                            g += Utility.ClampToByte(c - 512);
                            r += Utility.ClampToByte(c - 256);
                            a += Utility.ClampToByte(c - 0);
                        }

                        dstArgs.Surface.SetPoint(x,y, dstArgs.Surface.ColorPixelBase.FromBgra(
                            Utility.ClampToByte(b / count),
                            Utility.ClampToByte(g / count),
                            Utility.ClampToByte(r / count),
                            Utility.ClampToByte(a / count)));

                      
                    }
                }
            }
        }
    }
}
