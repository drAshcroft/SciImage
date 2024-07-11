using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;



using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;


namespace PaintDotNet.ColorPickers
{


    public partial class ColorsForm : Form 
    {
        //PdnBaseForm 
        public ColorsForm()
        {
            InitializeComponent();
            this.Width = ColorsControl.Width;
            //this.ClientRectangle = ColorsControl.ClientRectangle;
        }
        public  ColorsFormControl ColorsControl
        {
            get
            {
                return ColorsControl1;
            }
        }
    }
}
