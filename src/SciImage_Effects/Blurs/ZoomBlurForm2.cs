using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Blurs
{
    public partial class ZoomBlurForm2 : Form, IEffectConfigDialog
    {
        public ZoomBlurForm2()
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
                gtP2.Label = "Offset";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Amount").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Offset").MaxValue;
            
                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Amount").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Offset").MinValue;
             
                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Amount").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Offset").Value;
             
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
            EffectControl.EffectToken.SetPropertyValue("Offset", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

      

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

