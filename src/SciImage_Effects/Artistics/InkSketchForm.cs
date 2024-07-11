using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Artistics
{
    public partial class InkSketchForm : Form, IEffectConfigDialog
    {
        public InkSketchForm()
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

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("InkOutline").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Coloring").MaxValue;
            
                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("InkOutline").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Coloring").MinValue;
             
                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("InkOutline").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Coloring").Value;
             
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
            EffectControl.EffectToken.SetPropertyValue("InkOutline", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Coloring", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

      

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

