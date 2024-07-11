using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Artistics
{
    public partial class PencilSketchForm : Form, IEffectConfigDialog
    {
        public PencilSketchForm()
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

                gtP1.Label = "Pencil Tip Size";
                gtP2.Label = "Color Range";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("PencilTipSize").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("ColorRange").MaxValue;
            
                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("PencilTipSize").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("ColorRange").MinValue;
             
                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("PencilTipSize").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("ColorRange").Value;
             
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
            EffectControl.EffectToken.SetPropertyValue("PencilTipSize", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("ColorRange", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

      

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

