using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using WeifenLuo.WinFormsUI.Docking;
using SciImage;
using SciImage.PaintForms.LayerForm;
using SciImage.PaintForms.HistoryForm;
using SciImage.PaintForms.ColorPickers;
using SciImage.PaintForms.ToolsForm;

namespace Test
{
    public partial class Form1 : Form
    {
        private void CreateBasicLayout()
         {

            // appWorkspace1.ShowToolsbars();
             
             /*ToolBar tb = new ToolBar();
             tb.AddUserControl(appWorkspace1.Widgets.ToolsForm);
             ToolBar cb = new ToolBar();
             cb.AddUserControl(appWorkspace1.Widgets.ColorsForm );
             tb.Show(DockPanel, DockState.Float  );
             cb.Show(DockPanel, DockState.Float  );*/
            
             

         }

        public DockContent GetForm(string FormPersistenceString, string ExtraInformation)
        {

            if (FormPersistenceString.Contains("LayerForm"))
            {
                LayerFormControl lc=null;
             //   appWorkspace1.UserInterface.LayerForm(out lc);
                return GetPaintForms(lc);
            }
            if (FormPersistenceString.Contains("HistoryForm"))
            {
                HistoryFormControl hc = null;

                //  appWorkspace1.UserInterface.HistoryForm(out hc);
                return GetPaintForms(hc);
            }
            if (FormPersistenceString.Contains("ColorsForm"))
            {
                ColorsFormControl cc = null;

                // appWorkspace1.UserInterface.ColorsForm(out cc);
                return GetPaintForms(cc);
            }
            if (FormPersistenceString.Contains("ToolsForm"))
            {
                ToolsFormControl tc = null;
                // appWorkspace1.UserInterface.ToolForm(out tc);
                return GetPaintForms(tc);
            }
            return null;
        }

        public IDockContent ReloadContent(string persistString, string extraInformation)
        {
            
                
           switch (persistString)
           {
               case "Test.ToolBar":
                  return  GetForm( extraInformation,"");

               default :
                   return (null);
           }
     
        }

        public Form1()
        {
            InitializeComponent();


            string configFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName( Application.ExecutablePath ), "DockPanel.config");
            // Apply a gray professional renderer as a default renderer
            //ToolStripManager.Renderer = oDefaultRenderer;
            //oDefaultRenderer.RoundedEdges = true   ;

          
            // Set DockPanel properties
            DockPanel.ActiveAutoHideContent = null;
           
            DockPanel.Parent = this  ;

           
            DockPanel.SuspendLayout(true);
            //todo: load the configfile dependant on the user so each person gets their own look and feel
            if (System.IO.File.Exists(configFile) )
            {
                DockPanel.LoadFromXml(configFile, ReloadContent);
            }
            else
            {
                // Load a basic layout
                CreateBasicLayout();
            }

           
            DockPanel.ResumeLayout(true, true);

            
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            appWorkspace1.UserInterface.SetLayerWithImage(0,new Bitmap("TestImage.bmp"));

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string configFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName( Application.ExecutablePath) , "DockPanel.config");
            DockPanel.SaveAsXml(configFile);
            appWorkspace1.Dispose();
            while (DockPanel.Contents.Count > 0)
            {
                DockContent dc = (DockContent)DockPanel.Contents[0];
                dc.Close();
            }
            
           
        }

        private DockContent GetPaintForms(UserControl Control)
        {
            ToolBar tb = new ToolBar();
            tb.AddUserControl(Control);
            Size s = SizeFromClientSize(new Size(Control.Width, Control.Height));
            DockPanel.DefaultFloatWindowSize = s;
            return tb;
        }
        private Form appWorkspace1_MakeFormFromUsercontrol(UserControl Control)
        {

            DockContent tb = GetPaintForms(Control);
            
            tb.Show(DockPanel, DockState.Float);
            
            //tb.SetBounds(0, 0, s.Width, s.Height);
            return tb;
        }

        private void appWorkspace1_MakeNewForm(object sender, Form NewForm)
        {
            System.Diagnostics.Debug.Print(NewForm.GetType().ToString());
        }

        private void DockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            DockPanel_ActiveContentChanged(sender, e);
        }

        private void DockPanel_ActiveContentChanged(object sender, EventArgs e)
        {
            DockContent dc = (DockContent)DockPanel.ActiveContent;
            if (dc != null)
            {
                if (dc.GetType() == typeof(Test.ToolBar))
                {
                    if (((Test.ToolBar)dc).MainUserControl.GetType() == typeof(SciImage.DocumentWorkspace))
                    {
                      //  appWorkspace1.FormActivated(dc);
                    }
                }
            }
            //DockContent dc2 = (DockContent)DockPanel.ActiveDocument;
        }
    }
}
