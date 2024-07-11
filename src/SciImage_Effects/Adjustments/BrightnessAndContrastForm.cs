using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Adjustments
{
    public partial class BrightnessAndContrastForm : Form, IEffectConfigDialog
    {
        public BrightnessAndContrastForm()
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
                gtBrightness.Value = _EffectControl.EffectToken .GetProperty<Int32Property>("Brightness").Value;
                gTContrast.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Contrast").Value;
                this.Text = _EffectControl.Name;
                this.Icon = _EffectControl.GetConfigDialogIcon();
            }
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

      

        private void gtBrightness_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Brightness", gtBrightness.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gTContrast_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Contrast", gTContrast.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
