using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Photo
{
    public partial class SharpenForm1 : Form, IEffectConfigDialog
    {
        public SharpenForm1()
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
              

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Amount").MaxValue;
               
                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Amount").MinValue;
              
                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Amount").Value;
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

   
      

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

