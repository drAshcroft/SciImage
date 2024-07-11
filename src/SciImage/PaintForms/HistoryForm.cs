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
    public partial class HistoryForm : Form 
    {
        public HistoryForm()
        {
            InitializeComponent();
        }
        public  HistoryFormControl GetHistoryControl()
        {
            return HistoryControl;
        }

        private void HistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        
    }
}
