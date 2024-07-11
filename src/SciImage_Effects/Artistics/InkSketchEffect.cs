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
InkSketchEffect.cs 
Copyright (c) 2007 David Issel 
Contact info: BoltBait@hotmail.com http://www.BoltBait.com 

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
using SciImage_Effects.Photo;

namespace SciImage_Effects.Artistics
{
    public sealed class InkSketchEffect
        : Effect
    {
       
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new InkSketchForm();
            t.EffectControl = this;
            return t;
        }
        public static string StaticName
        {
            get
            {
                return "Ink Sketch";
            }
        }

        public static Image StaticIcon
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.InkSketchEffectIcon.png").Reference;
            }
        }

        public InkSketchEffect()
            : base(StaticName, StaticIcon, "Artistic", EffectFlags.Configurable, "Effects")
        {
            this.glowEffect = new GlowEffect();
            this.glowProps = this.glowEffect.CreatePropertyCollection();
        }

        static InkSketchEffect()
        {
            conv = new int[5][];

            for (int i = 0; i < conv.Length; ++i)
            {
                conv[i] = new int[5];
            }

            conv[0] = new int[] { -1, -1, -1, -1, -1 };
            conv[1] = new int[] { -1, -1, -1, -1, -1 };
            conv[2] = new int[] { -1, -1, 30, -1, -1 };
            conv[3] = new int[] { -1, -1, -1, -1, -1 };
            conv[4] = new int[] { -1, -1, -5, -1, -1 };
        }

        private static readonly int[][] conv;
        private const int size = 5;
        private const int radius = (size - 1) / 2;

        private Desaturate desaturateOp = new Desaturate();
        private Photo.GlowEffect glowEffect;
        private PropertyCollection glowProps;
        private DarkenBlendOp darkenOp = new DarkenBlendOp();

        private int inkOutline;
        private int coloring;

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("InkOutline", 50, 0, 99));
            props.Add(new Int32Property("Coloring", 50, 0, 100));

            return new PropertyCollection(props);
        }

        //protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        //{
        //    ControlInfo configUI = CreateDefaultConfigUI(props);

        //    configUI.SetPropertyControlValue("InkOutline, ControlInfo"DisplayName, SciImage.SciResource.SciResources.GetString("InkSketchEffect.ConfigDialog.InkOutlineLabel"));
        //    configUI.SetPropertyControlValue("Coloring, ControlInfo"DisplayName, SciImage.SciResource.SciResources.GetString("InkSketchEffect.ConfigDialog.ColoringLabel"));

        //    return configUI;
        //}

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.inkOutline = newToken.GetProperty<Int32Property>("InkOutline").Value;
            this.coloring = newToken.GetProperty<Int32Property>("Coloring").Value;

            EffectConfigToken glowToken = new EffectConfigToken(this.glowProps);
            glowToken.SetPropertyValue("Radius", 6);
            glowToken.SetPropertyValue("Brightness", -(this.coloring - 50) * 2);
            glowToken.SetPropertyValue("Contrast", -(this.coloring - 50) * 2);
            this.glowEffect.SetRenderInfo(glowToken, dstArgs, srcArgs);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public override unsafe void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            // Glow backgound 
            this.glowEffect.Render(parameters, DstArgs, SrcArgs, rois, startIndex, length);

            // Create black outlines by finding the edges of objects 

            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Rectangle roi = rois[i];

                for (int y = roi.Top; y < roi.Bottom; ++y)
                {
                    int top = y - radius;
                    int bottom = y + radius + 1;

                    if (top < 0)
                    {
                        top = 0;
                    }

                    if (bottom > DstArgs.Height)
                    {
                        bottom = DstArgs.Height;
                    }



                    for (int x = roi.Left; x < roi.Right; ++x)
                    {
                        //ColorPixelBase* srcPtr = SrcArgs.Surface.GetPointAddress(roi.X, y);
                        //ColorPixelBase* dstPtr = DstArgs.Surface.GetPointAddress(roi.X, y);

                        int left = x - radius;
                        int right = x + radius + 1;

                        if (left < 0)
                        {
                            left = 0;
                        }

                        if (right > DstArgs.Width)
                        {
                            right = DstArgs.Width;
                        }

                        int r = 0;
                        int g = 0;
                        int b = 0;

                        for (int v = top; v < bottom; v++)
                        {
                            //ColorPixelBase* pRow = SrcArgs.Surface.GetRowAddress(v);
                            int j = v - y + radius;

                            for (int u = left; u < right; u++)
                            {
                                int i1 = u - x + radius;
                                int w = conv[j][i1];

                                ColorPixelBase pRef = SrcArgs.Surface.GetPoint(u, v);

                                r += pRef[2] * w;
                                g += pRef[1] * w;
                                b += pRef[0] * w;
                            }
                        }

                        ColorPixelBase topLayer = SrcArgs.Surface.ColorPixelBase.FromBgra(
                            Utility.ClampToByte(b),
                            Utility.ClampToByte(g),
                            Utility.ClampToByte(r), 255);

                        // Desaturate 
                        topLayer = this.desaturateOp.Apply(topLayer);

                        // Adjust Brightness and Contrast 
                        if (topLayer[2] > (this.inkOutline * 255 / 100))
                        {
                            topLayer = topLayer.FromBgra(255, 255, 255, topLayer.alpha);
                        }
                        else
                        {
                            topLayer = topLayer.FromBgra(0, 0, 0, topLayer.alpha);
                        }

                        // Change Blend Mode to Darken 
                        ColorPixelBase myPixel = this.darkenOp.Apply(topLayer, DstArgs.Surface.GetPoint(x, y, topLayer));
                        DstArgs.Surface.SetPoint(x, y, myPixel);
                    }
                }
            }
        }
    }
}