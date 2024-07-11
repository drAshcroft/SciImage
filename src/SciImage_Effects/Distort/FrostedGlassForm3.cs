using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Distort
{
    public partial class FrostedGlassForm3 : Form, IEffectConfigDialog
    {
        public FrostedGlassForm3()
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
                gtP1.Label = "Max Scatter Radius";
                gtP2.Label = "Min Scatter Radius";
                gtP2.Label = "NumSamples";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("MaxScatterRadius").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("MinScatterRadius").MaxValue;
                gtP3.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("NumSamples").MaxValue;

                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("MaxScatterRadius").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("MinScatterRadius").MinValue;
                gtP3.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("NumSamples").MinValue;

                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("MaxScatterRadius").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("MinScatterRadius").Value;
                gtP3.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("NumSamples").Value;

                this.Text = _EffectControl.Name;
                this.Icon = _EffectControl.GetConfigDialogIcon();
            }
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }



        private void gtP1_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("MaxScatterRadius", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("MinScatterRadius", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP3_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("NumSamples", gtP3.Value);
            EffectControl.PropertiesChanged(this, e);
        }



        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

