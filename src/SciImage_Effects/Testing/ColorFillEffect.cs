/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

namespace SciImage_Effects.Testing
{
    // This effect is for testing purposes only.
#if false
    public sealed class ColorFillEffect
        : Effect
    {
        public ColorFillEffect()
            : base("Color Fill", null, null, EffectFlags.Configurable)
        {
            if (PdnInfo.IsFinalBuild)
            {
                throw new InvalidOperationException("This effect should never make it in to a released build");
            }
        }

        public enum PropertyNames
        {
            AngleChooser,
            CheckBox,
            DoubleSlider,
            DoubleVectorPanAndSlider,
            DoubleVectorSlider,
            Int32ColorWheel,
            Int32IncrementButton,
            Int32Slider,
            StaticListDropDown,
            StaticListRadioButton,
            StringText,
        }

        private ColorPixelBase color;

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new DoubleProperty("AngleChooser, 0, -180, +180));
            props.Add(new BooleanProperty("CheckBox, true));
            props.Add(new DoubleProperty("DoubleSlider, 0, 0, 100));
            props.Add(new DoubleVectorProperty("DoubleVectorPanAndSlider, Pair.Create(0.0, 0.0), Pair.Create(-1.0, -1.0), Pair.Create(+1.0, +1.0)));
            props.Add(new DoubleVectorProperty("DoubleVectorSlider, Pair.Create(0.0, 0.0), Pair.Create(-1.0, -1.0), Pair.Create(+1.0, +1.0)));
            props.Add(new Int32Property("Int32ColorWheel, 0, 0, 0xffffff));
            props.Add(new Int32Property("Int32IncrementButton, 0, 0, 255));
            props.Add(new Int32Property("Int32Slider, 0, 0, 100));
            props.Add(StaticListChoiceProperty.CreateForEnum<System.Drawing.GraphicsUnit>("StaticListDropDown, GraphicsUnit.Millimeter, false));
            props.Add(StaticListChoiceProperty.CreateForEnum<System.Drawing.GraphicsUnit>("StaticListRadioButton, GraphicsUnit.Document, false));
            props.Add(new StringProperty("StringText, "hello", 100));

            return new PropertyCollection(props);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            ControlInfo configUI = CreateDefaultConfigUI(props);

            configUI.SetPropertyControlType("AngleChooser, PropertyControlType.AngleChooser);
            configUI.SetPropertyControlType("CheckBox, PropertyControlType.CheckBox);
            configUI.SetPropertyControlType("DoubleSlider, PropertyControlType.Slider);
            configUI.SetPropertyControlType("DoubleVectorPanAndSlider, PropertyControlType.PanAndSlider);
            configUI.SetPropertyControlType("DoubleVectorSlider, PropertyControlType.Slider);
            configUI.SetPropertyControlType("Int32ColorWheel, PropertyControlType.ColorWheel);
            configUI.SetPropertyControlType("Int32IncrementButton, PropertyControlType.IncrementButton);
            configUI.SetPropertyControlType("Int32Slider, PropertyControlType.Slider);
            configUI.SetPropertyControlType("StaticListDropDown, PropertyControlType.DropDown);
            configUI.SetPropertyControlType("StaticListRadioButton, PropertyControlType.RadioButton);
            configUI.SetPropertyControlType("StringText, PropertyControlType.TextBox);

            foreach (object propertyName in Enum.GetValues(typeof(PropertyNames)))
            {
                configUI.SetPropertyControlValue(propertyName, ControlInfo"DisplayName, string.Empty);
            }

            return configUI;
        }

        protected override void OnCustomizeConfigUIWindowProperties(PropertyCollection props)
        {
            base.OnCustomizeConfigUIWindowProperties(props);
            //props[ControlInfo"WindowIsSizable].Value = true;
        }

        protected override void OnSetRenderInfo(EffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            int colorValue = newToken.GetProperty<Int32Property>("Int32ColorWheel).Value;
            this.color = ColorPixelBase.FromOpaqueInt32(colorValue);
            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }
        
        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            foreach (Rectangle rect in renderRects)
            {
                for (int y = rect.Top; y < rect.Bottom; ++y)
                {
                    for (int x = rect.Left; x < rect.Right; ++x)
                    {
                        DstArgs.Surface[x, y] = this.color;
                    }
                }
            }
        }
    }
#endif
}