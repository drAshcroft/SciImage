using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Test
{
    public partial class Form1 : Form
    {
        private void CreateBasicLayout()
        {
            appWorkspace1.UserInterface.InitializeImageC();
            ToolBar tb = new ToolBar();
            tb.AddUserControl((UserControl)appWorkspace1.UserInterface.ToolForm);
            ToolBar cb = new ToolBar();
            cb.AddUserControl((UserControl) appWorkspace1.UserInterface.ColorsForm);
            ToolBar hb = new ToolBar();
            hb.AddUserControl((UserControl)appWorkspace1.UserInterface.HistoryForm);
            ToolBar lb = new ToolBar();
            lb.AddUserControl((UserControl)appWorkspace1.UserInterface.LayerForm);

            tb.Show(this);
            cb.Show(this);
            hb.Show(this);
            lb.Show(this);

            var menus = appWorkspace1.UserInterface.Menus();
            foreach (var menu in menus)
            {
                menuStrip1.Items.Add(menu);
            }
        }

        public Form1()
        {
            InitializeComponent();
            CreateBasicLayout();
            appWorkspace1.UserInterface.RulersEnabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            appWorkspace1.UserInterface.SetLayerWithImage(0, new Bitmap("TestImage.bmp"));

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            appWorkspace1.Dispose();
        }

        private Form appWorkspace1_MakeFormFromUsercontrol(UserControl Control)
        {
            ToolBar tb = new ToolBar();
            tb.AddUserControl(Control);


            tb.Show(this);

            //tb.SetBounds(0, 0, s.Width, s.Height);
            return tb;
        }

        private void appWorkspace1_MakeNewForm(object sender, Form NewForm)
        {
            System.Diagnostics.Debug.Print(NewForm.GetType().ToString());
        }



    }
}
