/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

// Copyright (c) 2006-2008 Ed Harvey 
//
// MIT License: http://www.opensource.org/licenses/mit-license.php
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software. 
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE. 
//


using System;
using System.Drawing;
using SciImage.Core;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base;

namespace SciImage_Effects.Abstracts
{
    public abstract class WarpEffectBase
        : Effect
    {
        public WarpEffectBase(string name, Image image, string submenuName, EffectFlags options, string MenuName)
            : base(name, image, submenuName, options, MenuName)
        {
        }

        /// <summary>
        /// The amount to offset the 'center' of the effect
        /// </summary>
        /// <comments>
        /// <para>The offset is scaled relative to the image size. Each part of the <see cref="Pair`2"/> covers the range �1.</para>
        /// <para>The coordinates supplied to <see cref="InverseTransform"/> are relative to the calcuated logical center. </para>
        /// </comments>
        protected Pair<double, double> Offset
        {
            get 
            { 
                return this.offset; 
            }

            set 
            { 
                this.offset = value; 
            }
        }

        protected WarpEdgeBehavior EdgeBehavior
        {
            get 
            { 
                return edgeBehavior; 
            }

            set 
            { 
                edgeBehavior = value; 
            }
        }

        protected int Quality
        {
            get 
            { 
                return quality; 
            }

            set 
            { 
                quality = value; 
            }
        }

        /// <summary>
        /// The radius (in pixels) of the largest circle that can completely fit within the effect selection bounds
        /// </summary>
        protected double DefaultRadius
        {
            get 
            { 
                return this.defaultRadius; 
            }
        }

        /// <summary>
        /// The square of the DefaultRadius
        /// </summary>
        protected double DefaultRadius2
        {
            get 
            {
                return this.defaultRadius2; 
            }
        }

        /// <summary>
        /// The reciprical of the DefaultRadius
        /// </summary>
        protected double DefaultRadiusR
        {
            get 
            {
                return this.defaultRadiusR; 
            }
        }

        private Pair<double, double> offset = new Pair<double, double>(0, 0);
        private WarpEdgeBehavior edgeBehavior = WarpEdgeBehavior.Wrap;
        private int quality = 2;

        private double defaultRadius;
        private double defaultRadius2;
        private double defaultRadiusR;

        private double width;
        private double height;
        private double xCenterOffset;
        private double yCenterOffset;

        protected virtual void OnSetRenderInfo2(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
        }

        protected override sealed void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Rectangle selection = this.EnvironmentParameters.GetSelection(srcArgs.Bounds).GetBoundsInt();

            this.defaultRadius = Math.Min(selection.Width, selection.Height) * 0.5;
            this.defaultRadius2 = this.defaultRadius * this.defaultRadius;
            this.defaultRadiusR = 1.0 / this.defaultRadius;
            this.width = selection.Width;
            this.height = selection.Height;

            OnSetRenderInfo2(newToken, dstArgs, srcArgs);

            this.xCenterOffset = selection.Left + (this.width * (1.0 + this.offset.First) * 0.5);
            this.yCenterOffset = selection.Top + (this.height * (1.0 + this.offset.Second) * 0.5);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public unsafe override void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            Surface dst = DstArgs.Surface;
            Surface src = SrcArgs.Surface;

            ColorPixelBase colPrimary = EnvironmentParameters.PrimaryColor;
            ColorPixelBase colSecondary = EnvironmentParameters.SecondaryColor;
            ColorPixelBase colTransparent = colPrimary.TransparentColor();

            int aaSampleCount = quality * quality;
            PointF* aaPoints = stackalloc PointF[aaSampleCount];
            Utility.GetRgssOffsets(aaPoints, aaSampleCount, quality);
            ColorPixelBase[] samples =new ColorPixelBase[aaSampleCount];

            TransformData td;

            for (int n = startIndex; n < startIndex + length; ++n)
            {
                Rectangle rect = rois[n];

                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    //ColorPixelBase* dstPtr = dst.GetPointAddressUnchecked(rect.Left, y);

                    double relativeY = y - this.yCenterOffset;

                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        double relativeX = x - this.xCenterOffset;

                        int sampleCount = 0;

                        for (int p = 0; p < aaSampleCount; ++p)
                        {
                            td.X = relativeX + aaPoints[p].X;
                            td.Y = relativeY - aaPoints[p].Y;

                            InverseTransform(ref td);

                            float sampleX = (float)(td.X + this.xCenterOffset);
                            float sampleY = (float)(td.Y + this.yCenterOffset);

                            ColorPixelBase sample = colPrimary;

                            if (IsOnSurface(src, sampleX, sampleY))
                            {
                                sample = src.GetBilinearSample(sampleX, sampleY,src.ColorPixelBase );
                            }
                            else
                            {
                                switch (this.edgeBehavior)
                                {
                                    case WarpEdgeBehavior.Clamp:
                                        sample = src.GetBilinearSampleClamped(sampleX, sampleY, src.ColorPixelBase);
                                        break;

                                    case WarpEdgeBehavior.Wrap:
                                        sample = src.GetBilinearSampleWrapped(sampleX, sampleY, src.ColorPixelBase);
                                        break;

                                    case WarpEdgeBehavior.Reflect:
                                        sample = src.GetBilinearSampleClamped(
                                            ReflectCoord(sampleX, src.Width),
                                            ReflectCoord(sampleY, src.Height), src.ColorPixelBase);

                                        break;

                                    case WarpEdgeBehavior.Primary:
                                        sample = colPrimary;
                                        break;

                                    case WarpEdgeBehavior.Secondary:
                                        sample = colSecondary;
                                        break;

                                    case WarpEdgeBehavior.Transparent:
                                        sample = colTransparent;
                                        break;

                                    case WarpEdgeBehavior.Original:
                                        sample = src.GetPoint(x, y);
                                        break;

                                    default:
                                        break;
                                }
                            }

                            samples[sampleCount] = sample;
                            ++sampleCount;
                        }
                        DstArgs.Surface.SetPoint(x,y, src.ColorPixelBase.Blend(samples) );
                       
                    }
                }
            }
        }

        protected abstract void InverseTransform(ref TransformData data);

        protected struct TransformData
        {
            public double X;
            public double Y;
        }

        private static bool IsOnSurface(Surface src, float u, float v)
        {
            return (u >= 0 && u <= (src.Width - 1) && v >= 0 && v <= (src.Height - 1));
        }

        private static float ReflectCoord(float value, int max)
        {
            bool reflection = false;

            while (value < 0)
            {
                value += max;
                reflection = !reflection;
            }

            while (value > max)
            {
                value -= max;
                reflection = !reflection;
            }

            if (reflection)
            {
                value = max - value;
            }

            return value;
        }
    }

}
