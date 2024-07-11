using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PaintDotNet
{
    public partial class PaintCanvas : UserControl
    {
        public PaintCanvas()
        {
            InitializeComponent();
        }
        public AppWorkspace Workspace
        {
            get { return appWorkspace1; }

        }
    }
}
