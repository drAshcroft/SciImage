using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Blurs
{
    public partial class UnfocusForm : Form, IEffectConfigDialog
    {
        public UnfocusForm()
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
              

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Radius").MaxValue;
               
                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Radius").MinValue;
              
                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Radius").Value;
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

   
      

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

