/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

// Copyright (c) 2007,2008 Ed Harvey 
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
using System.Drawing;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Adjustments
{
    
    public sealed class PosterizeAdjustmentEffect
        : Effect
    {
        private UnaryPixelOp op;

        public PosterizeAdjustmentEffect()
            : base("Posterize",
                   SciImage.SciResources.SciResources.GetImageResource("Icons.PosterizeEffectIcon.png").Reference,
                   null,
                   EffectFlags.Configurable, "Adjustments")
        {
        }

        protected override IEffectConfigDialog OnCreateConfigDialog()
        {
            var t = new PosterizeAdjustmentForm();
            t.EffectControl = this;
            return t;
        }
       

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property("RedLevels", 16, 2, 64));
            props.Add(new Int32Property("GreenLevels", 16, 2, 64));
            props.Add(new Int32Property("BlueLevels", 16, 2, 64));
            props.Add(new BooleanProperty("LinkLevels", true));

            List<PropertyCollectionRule> rules = new List<PropertyCollectionRule>();

            rules.Add(new LinkValuesBasedOnBooleanRule<int, Int32Property>(
                new object[] { "RedLevels", "GreenLevels", "BlueLevels" }, 
                "LinkLevels", 
                false));

            return new PropertyCollection(props, rules);
        }
               

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            int red = newToken.GetProperty<Int32Property>("RedLevels").Value;
            int green = newToken.GetProperty<Int32Property>("GreenLevels").Value;
            int blue = newToken.GetProperty<Int32Property>("BlueLevels").Value;

            this.op = new PosterizePixelOp(red, green, blue);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        public unsafe override void Render(EffectConfigToken parameters, RenderArgs DstArgs, RenderArgs SrcArgs, Rectangle[] rois, int startIndex, int length)
        {
            this.op.Apply(DstArgs.Surface, SrcArgs.Surface, rois , startIndex, length);
        }
    }
}
