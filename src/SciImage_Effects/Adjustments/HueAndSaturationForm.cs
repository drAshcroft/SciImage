using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Adjustments
{
    public partial class HueAndSaturationForm : Form, IEffectConfigDialog
    {
        public HueAndSaturationForm()
        {
            InitializeComponent();
        }
        Effect _EffectControl;
        public Effect EffectControl
        {
            get
            {
                return _EffectControl;
            }
            set
            {
                _EffectControl = value;
                gtHue.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Hue").Value;
                gtSaturation.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Saturation").Value;
                gtLightness.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Lightness").Value;
                this.Text = _EffectControl.Name;
                this.Icon = _EffectControl.GetConfigDialogIcon();
            }
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }



        private void gtHue_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Hue", gtHue.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtSaturation_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Saturation", gtSaturation.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtLightness_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Lightness", gtLightness.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

