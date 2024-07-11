using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Adjustments
{
    public partial class PosterizeAdjustmentForm : Form, IEffectConfigDialog
    {
        public PosterizeAdjustmentForm()
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

                gtRed.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("RedLevels").MaxValue;
                gtGreen.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("GreenLevels").MaxValue;
                gtBlue.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("BlueLevels").MaxValue;

                gtRed.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("RedLevels").MinValue;
                gtGreen.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("GreenLevels").MinValue;
                gtBlue.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("BlueLevels").MinValue;

                gtRed.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("RedLevels").Value;
                gtGreen.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("GreenLevels").Value;
                gtBlue.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("BlueLevels").Value;

               

                this.Text = _EffectControl.Name;
                this.Icon = _EffectControl.GetConfigDialogIcon();
            }
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }



        private void gtRed_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("RedLevels", gtRed.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtGreen_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("GreenLevels", gtGreen.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtBlue_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("BlueLevels", gtBlue.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

