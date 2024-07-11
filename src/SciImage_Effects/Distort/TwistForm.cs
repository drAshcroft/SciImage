using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Distort
{
    public partial class TwistForm : Form, IEffectConfigDialog
    {
        public TwistForm()
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
                gtP1.Label = "Amount";
                gtP2.Label = "Size";
                gtP3.Label = "Offset";
                gtP4.Label = "Quality";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Amount").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Size").MaxValue;
                gtP3.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Offset").MaxValue;
                gtP4.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Quality").MaxValue;

                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Amount").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Size").MinValue;
                gtP3.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Offset").MinValue;
                gtP4.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Quality").MinValue;

                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Amount").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Size").Value;
                gtP3.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Offset").Value;
                gtP4.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Quality").Value;

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
            EffectControl.EffectToken.SetPropertyValue("Amount", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Size", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP3_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Offset", gtP3.Value);
            EffectControl.PropertiesChanged(this, e);
        }
        private void gtP4_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Quality", gtP3.Value);
            EffectControl.PropertiesChanged(this, e);
        }


        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

       
    }
}

