using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using SciImage;
namespace Test
{
    public partial class ToolBar : DockContent,IControlHoldingForm 
    {
        public ToolBar()
        {
            InitializeComponent();
        }
        private Control _UserControl;
        public Control MainUserControl
        {
            get { return _UserControl; }

        }
        public Form HostForm
        {
            get { return this; }
        }
        public void AddUserControl(UserControl uc)
        {
            _UserControl=uc;
            ExtraInformation = uc.GetType().ToString();
            this.SuspendLayout();
            // 
            // button1
            // 
            uc.Location = new System.Drawing.Point(0, 0);
            uc.TabIndex = 0;
            // 
            // ToolBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(uc.Width , uc.Height );
            Size s =SizeFromClientSize(new System.Drawing.Size(uc.Width , uc.Height ));
            this.Size=s;
            this.Width = s.Width;
            this.Height = s.Height;
            
            this.MinimumSize = new Size(s.Width, s.Height);
            this.ControlBox = false;
            this.Controls.Add(uc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = uc.Name ;
            this.ShowInTaskbar = false;
            this.Text = uc.Name ;
            this.ResumeLayout(false);

        }
    }
}
