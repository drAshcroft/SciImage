using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Photo
{
    public partial class SoftenPortraitForm : Form, IEffectConfigDialog
    {
        public SoftenPortraitForm()
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
                gtP1.Label = "Softness";
                gtP2.Label = "Lighting";
                gtP2.Label = "Warmth";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Softness").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Lighting").MaxValue;
                gtP3.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Warmth").MaxValue;

                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Softness").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Lighting").MinValue;
                gtP3.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Warmth").MinValue;

                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Softness").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Lighting").Value;
                gtP3.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Warmth").Value;

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
            EffectControl.EffectToken.SetPropertyValue("Softness", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Lighting", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP3_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Warmth", gtP3.Value);
            EffectControl.PropertiesChanged(this, e);
        }



        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

