using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Blurs
{
    public partial class MotionBlurForm : Form, IEffectConfigDialog
    {
        public MotionBlurForm()
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
                gtP1.Label = "Angle";
                gtP2.Label = "Distance";
                gtP2.Label = "Distance";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Angle").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Centered").MaxValue;
                gtP3.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Distance").MaxValue;

                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Angle").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Centered").MinValue;
                gtP3.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Distance").MinValue;

                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Angle").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Centered").Value;
                gtP3.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Distance").Value;

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
            EffectControl.EffectToken.SetPropertyValue("Angle", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Centered", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP3_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Distance", gtP3.Value);
            EffectControl.PropertiesChanged(this, e);
        }



        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

