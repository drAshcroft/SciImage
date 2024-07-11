/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

// Original C++ implementation by Jason Waltman as part of "Filter Explorer," http://www.jasonwaltman.com/thesis/index.html

using System.Collections.Generic;
using System.Drawing;
using SciImage.Core;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;
using SciImage.SystemLayer.System;

namespace SciImage_Effects.Artistics
{
    public sealed class OilPaintingEffect
        : Effect
    {
        public static string StaticName
        {
            get
            {
                return "Oil Painting";
            }
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new OilPaintingForm();
            t.EffectControl = this;
            return t;
        }
        public OilPaintingEffect()
            : base(StaticName,
                   SciImage.SciResources.SciResources.GetImageResource("Icons.OilPaintingEffect.png").Reference,
                   "Artistic",
                   EffectFlags.Configurable, "Effects")
        {
        }

        private int brushSize;
        private byte coarseness;

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("BrushSize", 3, 1, 8));
            props.Add(new Int32Property("Coarseness", 50, 3, 255));

            return new PropertyCollection(props);
        }

   
        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.brushSize = newToken.GetProperty<Int32Property>("BrushSize").Value;
            this.coarseness = (byte)newToken.GetProperty<Int32Property>("Coarseness").Value;
            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public unsafe override void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            Surface src = SrcArgs.Surface;
            Surface dst = DstArgs.Surface;
            int width = src.Width;
            int height = src.Height;

            int arrayLens = 1 + this.coarseness;

            int localStoreSize = arrayLens * 5 * sizeof(int);

            byte* localStore = stackalloc byte[localStoreSize];
            byte* p = localStore;

            int* intensityCount = (int*)p;
            p += arrayLens * sizeof(int);

            uint* avgRed = (uint*)p;
            p += arrayLens * sizeof(uint);

            uint* avgGreen = (uint*)p;
            p += arrayLens * sizeof(uint);

            uint* avgBlue = (uint*)p;
            p += arrayLens * sizeof(uint);

            uint* avgAlpha = (uint*)p;
            p += arrayLens * sizeof(uint);

            byte maxIntensity = this.coarseness;

            for (int r = startIndex; r < startIndex + length; ++r)
            {
                Rectangle rect = rois[r];

                int rectTop = rect.Top;
                int rectBottom = rect.Bottom;
                int rectLeft = rect.Left;
                int rectRight = rect.Right;

                for (int y = rectTop; y < rectBottom; ++y)
                {
                    //ColorPixelBase *dstPtr = dst.GetPointAddressUnchecked(rect.Left, y);

                    int top = y - brushSize;
                    int bottom = y + brushSize + 1;

                    if (top < 0)
                    {
                        top = 0;
                    }

                    if (bottom > height)
                    {
                        bottom = height;
                    }

                    for (int x = rectLeft; x < rectRight; ++x)
                    {
                        
                        Memory.SetToZero(localStore, (ulong)localStoreSize);

                        int left = x - brushSize;
                        int right = x + brushSize + 1;

                        if (left < 0)
                        {
                            left = 0;
                        }

                        if (right > width)
                        {
                            right = width;
                        }

                        int numInt = 0;

                        for (int j = top; j < bottom; ++j)
                        {
                            

                            for (int i = left; i < right; ++i)
                            {
                                ColorPixelBase srcPtr = src.GetPoint(i, j);
                                byte intensity = Utility.FastScaleByteByByte(srcPtr.GetIntensityByte(), maxIntensity);

                                ++intensityCount[intensity];
                                ++numInt;

                                avgRed[intensity] += (uint) srcPtr[2] ;
                                avgGreen[intensity] += (uint)srcPtr[1];
                                avgBlue[intensity] += (uint)srcPtr[0];
                                avgAlpha[intensity] += srcPtr.alpha ;

                            }
                        }

                        byte chosenIntensity = 0;
                        int maxInstance = 0;

                        for (int i = 0; i <= maxIntensity; ++i)
                        {
                            if (intensityCount[i] > maxInstance)
                            {
                                chosenIntensity = (byte)i;
                                maxInstance = intensityCount[i];
                            }
                        }

                        // TODO: correct handling of alpha values?

                        byte R = (byte)(avgRed[chosenIntensity] / maxInstance);
                        byte G = (byte)(avgGreen[chosenIntensity] / maxInstance);
                        byte B = (byte)(avgBlue[chosenIntensity] / maxInstance);
                        byte A = (byte)(avgAlpha[chosenIntensity] / maxInstance);

                        dst.SetPoint(x,y,src.ColorPixelBase.FromBgra(B, G, R, A)); 
                        //++dstPtr;
                    }
                }
            }
        }

    }
}
