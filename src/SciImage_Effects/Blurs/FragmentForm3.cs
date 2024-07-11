using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Blurs
{
    public partial class FragmentForm : Form, IEffectConfigDialog
    {
        public FragmentForm()
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
                gtP1.Label = "Fragments";
                gtP2.Label = "Distance";
                gtP2.Label = "Rotation";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Fragments").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Distance").MaxValue;
                gtP3.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Rotation").MaxValue;

                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Fragments").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Distance").MinValue;
                gtP3.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Rotation").MinValue;

                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Fragments").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Distance").Value;
                gtP3.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Rotation").Value;

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
            EffectControl.EffectToken.SetPropertyValue("Fragments", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Distance", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP3_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Rotation", gtP3.Value);
            EffectControl.PropertiesChanged(this, e);
        }



        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

