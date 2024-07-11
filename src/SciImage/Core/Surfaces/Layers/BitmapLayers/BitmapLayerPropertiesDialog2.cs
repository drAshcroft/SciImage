using System;
using System.ComponentModel;
using System.Windows.Forms;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Forms;
using SciImage.PaintForms.UserControls.ProgressBars;

namespace SciImage.Core.Surfaces.Layers.BitmapLayers
{
    public partial class BitmapLayerPropertiesDialog2 : SciBaseForm 
    {
        public BitmapLayerPropertiesDialog2()
        {
            InitializeComponent();

            cBBlendModes.Items.Clear();
            // populate the blendOpComboBox with all the blend modes they're allowed to use
            foreach (Type type in UserBlendOpsMaster.GetBlendOps())
            {
                cBBlendModes.Items.Add(UserBlendOpsMaster.CreateBlendOp(type));
            }
        }

        private Layer layer;
        private object originalProperties = null;

        [Browsable(false)]
        public Layer Layer
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
            if (((BitmapLayer)Layer).Opacity != (byte)OpacityUpDown.Value)
            {
                Layer.PushSuppressPropertyChanged();
                ((BitmapLayer)Layer).Opacity = (byte)opacityTrackBar.Value;
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
            ((BitmapLayer)Layer).Opacity = (byte)OpacityUpDown.Value;
             
            if (cBBlendModes.SelectedItem != null)
            {
                ((BitmapLayer)Layer).SetBlendOp((UserBlendOp)cBBlendModes.SelectedItem);
            }

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
            SelectOp(((BitmapLayer)Layer).BlendOp);
            tBLayerName.Text = this.layer.Name;
            cBLayerVisible.Checked = this.layer.Visible;
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
        }

        private void cBBlendModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (new WaitCursorChanger(this))
            {
                Layer.PushSuppressPropertyChanged();

                if (cBBlendModes.SelectedItem != null)
                {
                    ((BitmapLayer)Layer).SetBlendOp((UserBlendOp)cBBlendModes.SelectedItem);
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

        

    }
}
