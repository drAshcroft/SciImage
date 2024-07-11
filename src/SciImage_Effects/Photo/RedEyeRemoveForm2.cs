using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Photo
{
    public partial class RedEyeRemoveForm2 : Form, IEffectConfigDialog
    {
        public RedEyeRemoveForm2()
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
                gtP1.Label = "Tolerance";
                gtP2.Label = "Saturation";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Tolerance").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Saturation").MaxValue;
            
                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Tolerance").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Saturation").MinValue;
             
                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Tolerance").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Saturation").Value;
             
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
            EffectControl.EffectToken.SetPropertyValue("Tolerance", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Saturation", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

      

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

