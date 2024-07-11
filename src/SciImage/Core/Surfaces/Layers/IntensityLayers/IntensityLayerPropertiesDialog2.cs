using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SciImage.Layers
{
    public partial class IntensityPropertiesDialog2 : PdnBaseForm 
    {
        public IntensityPropertiesDialog2()
        {
            InitializeComponent();

            cBBlendModes.Items.Clear();
            // populate the blendOpComboBox with all the blend modes they're allowed to use
            foreach (Type type in UserBlendOps.GetBlendOps())
            {
                cBBlendModes.Items.Add(UserBlendOps.CreateBlendOp(type));
            }
        }

        private IntensityLayer layer;
        private object originalProperties = null;

        [Browsable(false)]
        public IntensityLayer Layer
        {
            get
            {
                return layer;
            }

            set
            {
                this.layer = value;
                this.originalProperties = this.layer.SaveProperties();
                InitDialogFromLayer();
            }
        }

        private void ChangeLayerOpacity()
        {
            if (((IntensityLayer)Layer).Opacity != (byte)OpacityUpDown.Value)
            {
                Layer.PushSuppressPropertyChanged();
                ((IntensityLayer)Layer).Opacity = (byte)opacityTrackBar.Value;
                Layer.PopSuppressPropertyChanged();
            }
        }

        private void SelectOp(UserBlendOp setOp)
        {

            foreach (object op in cBBlendModes.Items)
            {
                if (op.ToString() == setOp.ToString())
                {
                    cBBlendModes.SelectedItem = op;
                    break;
                }
            }
        }

        private void InitLayerFromDialog()
        {
            ((IntensityLayer)Layer).Opacity = (byte)OpacityUpDown.Value;
             
            if (cBBlendModes.SelectedItem != null)
            {
                ((IntensityLayer)Layer).SetBlendOp((UserBlendOp)cBBlendModes.SelectedItem);
            }
            long nValue = (long)(((double)tBMax.Value / (double)tBMax.Maximum) * Int16.MaxValue);
            this.layer.MaxContrast = nValue;
            nValue = (long)(((double)tbMin.Value / (double)tbMin.Maximum) * Int16.MaxValue);
            this.layer.MinContrast = nValue;
            this.layer.Name = tBLayerName.Text;
            this.layer.Visible = cBLayerVisible.Checked;

            if (this.Owner != null)
            {
                this.Owner.Update();
            }
        }

        private void InitDialogFromLayer()
        {
            OpacityUpDown.Value = Layer.Opacity;
            SelectOp(((IntensityLayer)Layer).BlendOp);
            tBLayerName.Text = this.layer.Name;
            cBLayerVisible.Checked = this.layer.Visible;
            tBMax.Value =(int)( 1000 *(double) this.layer.MaxContrast / (double)Int16.MaxValue);
            tbMin.Value =(int)( 1000 *(double) this.layer.MinContrast / (double)Int16.MaxValue);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            using (new WaitCursorChanger(this))
            {
                this.layer.PushSuppressPropertyChanged();
                InitLayerFromDialog();
                object currentProperties = this.layer.SaveProperties();
                this.layer.LoadProperties(this.originalProperties);
                this.layer.PopSuppressPropertyChanged();

                this.layer.LoadProperties(currentProperties);
                this.originalProperties = layer.SaveProperties();
                //blayer.Invalidate(); // no need to call Invalidate() -- it will be called by OnClosed()
            }

            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            using (new WaitCursorChanger(this))
            {
                this.layer.PushSuppressPropertyChanged();
                this.layer.LoadProperties(this.originalProperties);
                this.layer.PopSuppressPropertyChanged();
                this.layer.Invalidate();
            }

            base.OnClosed(e);
            this.Hide();
        }

        private void cBBlendModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (new WaitCursorChanger(this))
            {
                Layer.PushSuppressPropertyChanged();

                if (cBBlendModes.SelectedItem != null)
                {
                    ((IntensityLayer)Layer).SetBlendOp((UserBlendOp)cBBlendModes.SelectedItem);
                }

                Layer.PopSuppressPropertyChanged();
            }
        }

        private void OpacityUpDown_Leave(object sender, EventArgs e)
        {
            OpacityUpDown_ValueChanged(sender, e);
        }

        private void OpacityUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (opacityTrackBar.Value != (int)OpacityUpDown.Value)
            {
                using (new WaitCursorChanger(this))
                {
                    opacityTrackBar.Value = (int)OpacityUpDown.Value;
                    ChangeLayerOpacity();
                }
            }
        }

        private void opacityTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (OpacityUpDown.Value != (decimal)opacityTrackBar.Value)
            {
                using (new WaitCursorChanger(this))
                {
                    OpacityUpDown.Value = (decimal)opacityTrackBar.Value;
                    ChangeLayerOpacity();
                }
            }
        }

        private void OpacityUpDown_Enter(object sender, EventArgs e)
        {
            OpacityUpDown.Select(0, OpacityUpDown.Text.Length);
        }

        private void cBLayerVisible_CheckedChanged(object sender, EventArgs e)
        {
            Layer.PushSuppressPropertyChanged();
            Layer.Visible = cBLayerVisible.Checked;
            Layer.PopSuppressPropertyChanged();
        }

        private void tbMin_ValueChanged(object sender, EventArgs e)
        {
            long nValue =(long)( ((double)tbMin.Value / (double)tbMin.Maximum) * Int16.MaxValue);
            if (((IntensityLayer)Layer).MinContrast  != nValue )
            {
                Layer.PushSuppressPropertyChanged();
                ((IntensityLayer)Layer).MinContrast  = nValue ;
                Layer.PopSuppressPropertyChanged();
            }
        }

        private void tBMax_ValueChanged(object sender, EventArgs e)
        {
            long nValue = (long)(((double)tBMax.Value / (double)tBMax.Maximum) * Int16.MaxValue);
           
            if (((IntensityLayer)Layer).MaxContrast != nValue )
            {
                Layer.PushSuppressPropertyChanged();
                ((IntensityLayer)Layer).MaxContrast = nValue ;
                Layer.PopSuppressPropertyChanged();
            }
        }

        

    }
}
