using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace PaintDotNet
{
    public partial class LayerForm : Form
    {
        public  PaintDotNet.LayerControl LayerControl
        {
            get { return layerFormControl1.LayerControl  ; }
        }
        public LayerForm()
        {
            InitializeComponent();
        }

        private void LayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
