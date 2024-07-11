/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;

namespace SciImage.Core.Renderer
{
    public abstract class GradientRenderer
    {
        private BinaryPixelOp normalBlendOp;
        private ColorPixelBase startColor;
        private ColorPixelBase endColor;
        private PointF startPoint;
        private PointF endPoint;
        private bool alphaBlending;
        private bool alphaOnly;

        private bool lerpCacheIsValid = false;
        private byte[] lerpAlphas;
        private ColorPixelBase[] lerpColors;
        
        public ColorPixelBase StartColor
        {
            get
            {
                return this.startColor;
            }

            set
            {
                if (this.startColor != value)
                {
                    this.startColor = value;
                    this.lerpCacheIsValid = false;
                }
            }
        }

        public ColorPixelBase EndColor
        {
            get
            {
                return this.endColor;
            }

            set
            {
                if (this.endColor != value)
                {
                    this.endColor = value;
                    this.lerpCacheIsValid = false;
                }
            }
        }

        public PointF StartPoint
        {
            get
            {
                return this.startPoint;
            }

            set
            {
                this.startPoint = value;
            }
        }

        public PointF EndPoint
        {
            get
            {
                return this.endPoint;
            }

            set
            {
                this.endPoint = value;
            }
        }

        public bool AlphaBlending
        {
            get
            {
                return this.alphaBlending;
            }

            set
            {
                this.alphaBlending = value;
            }
        }

        public bool AlphaOnly
        {
            get
            {
                return this.alphaOnly;
            }

            set
            {
                this.alphaOnly = value;
            }
        }

        public virtual void BeforeRender()
        {
            if (!this.lerpCacheIsValid)
            {
                byte startAlpha;
                byte endAlpha;

                if (this.alphaOnly)
                {
                    ComputeAlphaOnlyValuesFromColors(this.startColor, this.endColor, out startAlpha, out endAlpha);
                }
                else
                {
                    startAlpha = this.startColor.alpha ;
                    endAlpha = this.endColor.alpha ;
                } 
                
                this.lerpAlphas = new byte[256];
                this.lerpColors = new ColorPixelBase[256];

                for (int i = 0; i < 256; ++i)
                {
                    byte a = (byte)i;
                    this.lerpColors[a] = this.startColor.Blend(this.startColor, this.endColor, a);
                    this.lerpAlphas[a] = (byte)(startAlpha + ((endAlpha - startAlpha) * a) / 255);
                }

                this.lerpCacheIsValid = true;
            }
        }

        public abstract float ComputeUnboundedLerp(int x, int y);
        public abstract float BoundLerp(float t);

        public virtual void AfterRender()
        {
        }

        private static void ComputeAlphaOnlyValuesFromColors(ColorPixelBase startColor, ColorPixelBase endColor, out byte startAlpha, out byte endAlpha)
        {
            startAlpha = startColor.alpha ;
            endAlpha = (byte)(255 - endColor.alpha );
        }

        public unsafe void Render(Surface surface, Rectangle[] rois, int startIndex, int length)
        {
            byte startAlpha;
            byte endAlpha;

            if (this.alphaOnly)
            {
                ComputeAlphaOnlyValuesFromColors(this.startColor, this.endColor, out startAlpha, out endAlpha);
            }
            else
            {
                startAlpha = this.startColor.alpha ;
                endAlpha = this.endColor.alpha ;
            }

            for (int ri = startIndex; ri < startIndex + length; ++ri)
            {
                Rectangle rect = rois[ri];

                if (this.startPoint == this.endPoint)
                {
                    // Start and End point are the same ... fill with solid color.
                    for (int y = rect.Top; y < rect.Bottom; ++y)
                    {
                        //ColorPixelBase* pixelPtr = surface.GetPointAddress(rect.Left, y);

                        for (int x = rect.Left; x < rect.Right; ++x)
                        {
                            ColorPixelBase pixelPtr = surface.GetPoint(x, y);
                            ColorPixelBase result;

                            if (this.alphaOnly && this.alphaBlending)
                            {
                                byte resultAlpha = (byte)Utility.FastDivideShortByByte((ushort)(pixelPtr.alpha  * endAlpha), 255);
                                result = pixelPtr;
                                result.alpha  = resultAlpha;
                            }
                            else if (this.alphaOnly && !this.alphaBlending)
                            {
                                result = pixelPtr;
                                result.alpha  = endAlpha;
                            }
                            else if (!this.alphaOnly && this.alphaBlending)
                            {
                                result = this.normalBlendOp.Apply(pixelPtr, this.endColor);
                            }
                            else //if (!this.alphaOnly && !this.alphaBlending)
                            {
                                result = this.endColor;
                            }
                            surface.SetPoint(x, y, pixelPtr);
                        }
                    }
                }
                else
                {
                    for (int y = rect.Top; y < rect.Bottom; ++y)
                    {
                        

                        if (this.alphaOnly && this.alphaBlending)
                        {
                            
                            for (int x = rect.Left; x < rect.Right; ++x)
                            {
                                ColorPixelBase pixelPtr = surface.GetPoint(x, y);
                                float lerpUnbounded = ComputeUnboundedLerp(x, y);
                                float lerpBounded = BoundLerp(lerpUnbounded);
                                byte lerpByte = (byte)(lerpBounded * 255.0f);
                                byte lerpAlpha = this.lerpAlphas[lerpByte];
                                byte resultAlpha = Utility.FastScaleByteByByte(pixelPtr.alpha , lerpAlpha);
                                pixelPtr.alpha  = resultAlpha;
                               
                            }
                        }
                        else if (this.alphaOnly && !this.alphaBlending)
                        {
                            for (int x = rect.Left; x < rect.Right; ++x)
                            {
                                ColorPixelBase pixelPtr = surface.GetPoint(x, y);
                                float lerpUnbounded = ComputeUnboundedLerp(x, y);
                                float lerpBounded = BoundLerp(lerpUnbounded);
                                byte lerpByte = (byte)(lerpBounded * 255.0f);
                                byte lerpAlpha = this.lerpAlphas[lerpByte];
                                pixelPtr.alpha  = lerpAlpha;
                            }
                        }
                        else if (!this.alphaOnly && (this.alphaBlending && (startAlpha != 255 || endAlpha != 255)))
                        {
                            // If we're doing all color channels, and we're doing alpha blending, and if alpha blending is necessary
                            for (int x = rect.Left; x < rect.Right; ++x)
                            {
                                ColorPixelBase pixelPtr = surface.GetPoint(x, y);
                                float lerpUnbounded = ComputeUnboundedLerp(x, y);
                                float lerpBounded = BoundLerp(lerpUnbounded);
                                byte lerpByte = (byte)(lerpBounded * 255.0f);
                                ColorPixelBase lerpColor = this.lerpColors[lerpByte];
                                ColorPixelBase result = this.normalBlendOp.Apply(pixelPtr, lerpColor);
                                surface.SetPoint(x,y, result);
                            }
                        }
                        else //if (!this.alphaOnly && !this.alphaBlending) // or sC.A == 255 && eC.A == 255
                        {
                            for (int x = rect.Left; x < rect.Right; ++x)
                            {
                                ColorPixelBase pixelPtr = surface.GetPoint(x, y);
                                float lerpUnbounded = ComputeUnboundedLerp(x, y);
                                float lerpBounded = BoundLerp(lerpUnbounded);
                                byte lerpByte = (byte)(lerpBounded * 255.0f);
                                ColorPixelBase lerpColor = this.lerpColors[lerpByte];
                                surface.SetPoint(x,y, lerpColor);
                            }
                        }
                    }
                }
            }

            AfterRender();
        }

        protected GradientRenderer(bool alphaOnly, BinaryPixelOp normalBlendOp)
        {
            this.normalBlendOp = normalBlendOp;
            this.alphaOnly = alphaOnly;
        }
    }
}
