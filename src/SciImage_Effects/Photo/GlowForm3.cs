using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Photo
{
    public partial class GlowForm3 : Form, IEffectConfigDialog
    {
        public GlowForm3()
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
                gtP1.Label = "Radius";
                gtP2.Label = "Brightness";
                gtP2.Label = "Contrast";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Radius").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Brightness").MaxValue;
                gtP3.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Contrast").MaxValue;

                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Radius").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Brightness").MinValue;
                gtP3.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Contrast").MinValue;

                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Radius").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Brightness").Value;
                gtP3.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Contrast").Value;

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
            EffectControl.EffectToken.SetPropertyValue("Radius", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Brightness", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP3_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Contrast", gtP3.Value);
            EffectControl.PropertiesChanged(this, e);
        }



        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

