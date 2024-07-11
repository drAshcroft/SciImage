﻿using System;
using System.Windows.Forms;
using SciImage.Plugins.Effects;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage_Effects.Distort
{
    public partial class TileForm : Form, IEffectConfigDialog
    {
        public TileForm()
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
                gtP1.Label = "Rotation";
                gtP2.Label = "SquareSize";
                gtP3.Label = "Curvature";
                gtP4.Label = "Quality";

                gtP1.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Rotation").MaxValue;
                gtP2.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("SquareSize").MaxValue;
                gtP3.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Curvature").MaxValue;
                gtP4.MaxValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Quality").MaxValue;

                gtP1.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Rotation").MinValue;
                gtP2.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("SquareSize").MinValue;
                gtP3.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Curvature").MinValue;
                gtP4.MinValue = _EffectControl.EffectToken.GetProperty<Int32Property>("Quality").MinValue;

                gtP1.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Rotation").Value;
                gtP2.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("SquareSize").Value;
                gtP3.Value = _EffectControl.EffectToken.GetProperty<Int32Property>("Curvature").Value;
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
            EffectControl.EffectToken.SetPropertyValue("Rotation", gtP1.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP2_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("SquareSize", gtP2.Value);
            EffectControl.PropertiesChanged(this, e);
        }

        private void gtP3_ValueChanged(object sender, EventArgs e)
        {
            EffectControl.EffectToken.SetPropertyValue("Curvature", gtP3.Value);
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

