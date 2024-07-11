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

using System.Collections.Generic;
using SciImage.Core;
using SciImage.Core.Renderer;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base;
using SciImage.SystemLayer.Base.PropertySystem;
using SciImage_Effects.Abstracts;

namespace SciImage_Effects.Distort
{
    public sealed class PolarInversionEffect 
        : WarpEffectBase
    {
        public PolarInversionEffect()
            : base("Polar Inversion",
                   SciImage.SciResources.SciResources.GetImageResource("Icons.PolarInversionEffect.png").Reference,
                   "Distort", 
                   EffectFlags.Configurable, "Effects")
        {
        }
        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new PolarInversionForm4();
            t.EffectControl = this;
            return t;
        }
       

        private double amount;

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> properties = new List<Property>();

            properties.Add(new DoubleProperty("Amount", 1, -4, 4));
            properties.Add(new DoubleVectorProperty("Offset", Pair.Create<double, double>(0, 0), Pair.Create<double, double>(-2, -2), Pair.Create<double, double>(2, 2)));
            properties.Add(new StaticListChoiceProperty("EdgeBehavior", new object[] { WarpEdgeBehavior.Clamp, WarpEdgeBehavior.Reflect, WarpEdgeBehavior.Wrap }, 2));
            properties.Add(new Int32Property("Quality", 2, 1, 5));

            return new PropertyCollection(properties);
        }

    

        protected override void OnSetRenderInfo2(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.amount = newToken.GetProperty<DoubleProperty>("Amount").Value;
            base.Offset = newToken.GetProperty<DoubleVectorProperty>("Offset").Value;
            base.EdgeBehavior = (WarpEdgeBehavior)newToken.GetProperty<StaticListChoiceProperty>("EdgeBehavior").Value;
            base.Quality = newToken.GetProperty<Int32Property>("Quality").Value;
        }

        protected override void InverseTransform(ref TransformData data)
        {
            double x = data.X;
            double y = data.Y;

            // NOTE: when x and y are zero, this will divide by zero and return NaN
            double invertDistance = Utility.Lerp(1d, DefaultRadius2 / ((x * x) + (y * y)), amount);

            data.X = x * invertDistance;
            data.Y = y * invertDistance;
        }
    }
}
