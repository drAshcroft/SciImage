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
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Render
{
    public sealed class MandelbrotFractalEffect
        : Effect 
    {
        public static string StaticName
        {
            get
            {
                return "Mandelbrot Fractal";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new MandelbrotFractalForm();
            t.EffectControl = this;
            return t;
        }
        public static Image StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MandelbrotFractalEffectIcon.png").Reference;
            }
        }


        public MandelbrotFractalEffect()
            : base(StaticName,
                   StaticImage, 
                   "Render", 
                   EffectFlags.Configurable, "Effects")
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("Factor", 1, 1, 10));
            props.Add(new DoubleProperty("Zoom", 10, 0, 100));
            props.Add(new DoubleProperty("Angle", 0.0, -180.0, +180.0));
            props.Add(new Int32Property("Quality", 2, 1, 5));
            props.Add(new BooleanProperty("InvertColors"));

            return new PropertyCollection(props);
        }

      

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.zoom = 1 + zoomFactor * newToken.GetProperty<DoubleProperty>("Zoom").Value;
            this.factor = newToken.GetProperty<Int32Property>("Factor").Value;
            this.quality = newToken.GetProperty<Int32Property>("Quality").Value;
            this.angle = newToken.GetProperty<DoubleProperty>("Angle").Value;
            this.invertColors = newToken.GetProperty<BooleanProperty>("InvertColors").Value;
            this.angleTheta = (this.angle * 2 * Math.PI) / 360;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        private static double zoomFactor = 20.0;
        private double zoom;

        private int factor;
        private int quality = 1;
        private double angle = 0;

        private double angleTheta;

        private const double xOffsetBasis = -0.7;
        private double xOffset = xOffsetBasis;

        private const double yOffsetBasis = -0.29;
        private double yOffset = yOffsetBasis;

        private bool invertColors;

        private const double max = 100000;
        private static readonly double invLogMax = 1.0 / Math.Log(max);

        private static double Mandelbrot(double r, double i, int factor)
        {
            int c = 0;
            double x = 0;
            double y = 0;

            while ((c * factor) < 1024 && 
                   ((x * x) + (y * y)) < max)
            {
                double t = x;

                x = x * x - y * y + r;
                y = 2 * t * y + i;

                ++c;
            }

            return c - Math.Log(y * y + x * x) * invLogMax;
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            int w = dstArgs.Width;
            int h = dstArgs.Height;

            double wDiv2 = (double)w / 2;
            double hDiv2 = (double)h / 2;

            double invH = 1.0 / h;
            double invZoom = 1.0 / this.zoom;

            double invQuality = 1.0 / (double)this.quality;

            int count = this.quality * this.quality + 1;
            double invCount = 1.0 / (double)count;

            for (int ri = startIndex; ri < startIndex + length; ++ri)
            {
                Rectangle rect = rois[ri];

                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    //ColorPixelBase* dstPtr = dstArgs.Surface.GetPointAddressUnchecked(rect.Left, y);

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

                            double m = Mandelbrot(
                                (uP * invZoom) + this.xOffset, 
                                (vP * invZoom) + this.yOffset, 
                                this.factor);

                            double c = 64 + this.factor * m;

                            r += Utility.ClampToByte(c - 768);
                            g += Utility.ClampToByte(c - 512);
                            b += Utility.ClampToByte(c - 256);
                            a += Utility.ClampToByte(c - 0);
                        }

                        dstArgs.Surface.SetPoint(x,y,dstArgs.Surface.ColorPixelBase.FromBgra(
                            Utility.ClampToByte(b / count),
                            Utility.ClampToByte(g / count),
                            Utility.ClampToByte(r / count),
                            Utility.ClampToByte(a / count)));
                    }
                }

                if (this.invertColors)
                {
                    for (int y = rect.Top; y < rect.Bottom; y++)
                    {
                        //ColorPixelBase* dstPtr = dstArgs.Surface.GetPointAddressUnchecked(rect.Left, y);

                        for (int x = rect.Left; x < rect.Right; ++x)
                        {
                            ColorPixelBase c = dstArgs.Surface.GetPoint(x,y);
                            dstArgs.Surface.SetPoint(x,y, c.InvertColor(c));
                        }
                    }
                }
            }            
        }
    }
}
