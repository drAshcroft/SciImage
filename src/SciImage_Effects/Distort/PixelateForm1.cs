using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Distort
{
    public partial class PixelateForm1 : Form, IEffectConfigDialog
    {
        public PixelateForm1()
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
                gtP1.Label = "CellSize";


                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("CellSize").MaxValue;

                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("CellSize").MinValue;

                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("CellSize").Value;
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

