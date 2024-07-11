using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Stylize
{
    public partial class ReliefForm : Form, IEffectConfigDialog
    {
        public ReliefForm()
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
              

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Angle").MaxValue;
               
                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Angle").MinValue;
              
                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Angle").Value;
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

   
      

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

