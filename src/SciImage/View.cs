using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
//using VS2005Extender;

namespace PaintDotNet
{

    public partial class View : DockContent,Micromanager_net.IPictureView,Micromanager_net.ISubMDI  // PaintDotNet.FloatingToolForm   
    {
        Micromanager_net. CoreDevices.EasyCore eCore;

        public event EventHandler<Micromanager_net.NewDockableFormEvents> OnNewDockableForm;
        public event EventHandler<Micromanager_net.NewFormEvents> OnNewForm;
        public DockContent GetForm(string FormPersistenceString, string ExtraInformation)
        {
             
            if (FormPersistenceString.Contains("LayerForm"))
            {
                LayerForm lc ;
               
                PS.InitializeLayerForm(out lc );
                return lc;
            }
            if (FormPersistenceString.Contains("HistoryForm"))
            {
                HistoryForm hc;
               
                PS.InitializeHistoryForm(out hc );
                return hc;
            }
            if (FormPersistenceString.Contains("ColorsForm"))
            {
                ColorPickers. ColorsForm cc ;

                PS.InitializeColorsForm(out cc);
                return cc;
            }
            if (FormPersistenceString.Contains("ToolsForm"))
            {
                ToolsForm tc ;
                PS.InitializeToolForm(out tc );
                return tc;
            }
            return null;
        }
        public void CreateBasicSetup()
        {
            LayerForm lc;
            HistoryForm hc;
            ColorPickers.ColorsForm cc;
            ToolsForm tc;

            PS.InitializeLayerForm(out lc);
            PS.InitializeHistoryForm(out  hc);
            PS.InitializeColorsForm(out cc);
            PS.InitializeToolForm(out tc);

            if (OnNewDockableForm != null)
            {
                OnNewDockableForm(this, new Micromanager_net.NewDockableFormEvents(lc));
                OnNewDockableForm(this, new Micromanager_net.NewDockableFormEvents(hc));
                OnNewDockableForm(this, new Micromanager_net.NewDockableFormEvents(cc));
                OnNewDockableForm(this, new Micromanager_net.NewDockableFormEvents(tc));
            }
            if (OnNewForm != null)
            {
                //OnNewForm(this, new Micromanager_net.NewFormEvents(new ToolsFormX()));

            }
            //lc.Show(PS.dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
            

        }

        public PaintDotNet.AppWorkspace AppWorkSpace()
        {
            return PS;
        }
        public void ForceSave(string filename)
        {
            PS.AutoSave(filename);
        }
        public void ShowToolsbars()
        {
            PS.ShowToolsbars();
           // PS.GetToolForm();
        }
        public View()
        {
            InitializeComponent();
            
        }

        void PS_MakeNewForm(object sender, Form NewForm)
        {
            if (OnNewForm != null)
            {
                OnNewForm(this, new Micromanager_net.NewFormEvents(NewForm ));

            }
        }
        public void SetCore(Micromanager_net.CoreDevices.EasyCore Ecore)
        {
            
            eCore = Ecore;
            
          // eCore.ImageProduced += new Micromanager_net.CoreDevices.ImageProducedEvent(eCore_ImageProduced);
          // UpdateImage +=new UpdateImageEvent(eCore_ImageProduced);
        }

        private delegate void SendImageRawEvent(int Width, int Height, int Stride, int BPP, int ByteSize, IntPtr Data, long MaxPixelValue, long MinPixelValue);
        private delegate void SendImageBitmapEvent(Image NewImage);
        private delegate void SendImageLayerAndBitmap(Image[] NewImages);
        public void SendImage(Micromanager_net. CoreDevices.CoreImage cImage)
        {
            SendImage(cImage.ImageRGB);
        }
        public void SendImage(Micromanager_net. CoreDevices.CoreImage[] cImages)
        {
            Bitmap[] Images = new Bitmap[cImages.Length];
            for (int i = 0; i < cImages.Length; i++)
                Images[i] = cImages[i].ImageRGB;
            SendImage(Images);
        }
        public void SendImage( Image[] newImages)
        {
            if (PS.InvokeRequired)
            {
                object[] pars = { newImages };
                try
                {
                    PS.Invoke(new SendImageLayerAndBitmap(SendImage), pars);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

            }
            else
            {
                PS.SetLayerWithImage( newImages);
            }

        }
        delegate void UpdatesPausedEvent();
        public  void UpdatesPaused()
        {
            if (PS.InvokeRequired)
            {
                
                try
                {
                    PS.Invoke(new UpdatesPausedEvent (UpdatesPaused ));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

            }
            else
            {
                PS.UpdatesPaused() ;
            }
        }
        public void SendImage(Image newImage)
        {
            if (PS.InvokeRequired)
            {
                object[] pars = {newImage  };
                try
                {
                    PS.Invoke(new SendImageBitmapEvent (SendImage ), pars);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

            }
            else
            {
                PS.SetLayerWithImage(newImage );
            }

        }
        public void SendImage(int Width, int Height, int Stride, int BPP, int ByteSize, IntPtr Data, long MaxPixelValue, long MinPixelValue)
        {
            if (PS.InvokeRequired)
            {
                object[] pars = { Width, Height, Stride, BPP, ByteSize, Data, MaxPixelValue, MinPixelValue };
                try
                {
                    PS.Invoke(new SendImageRawEvent( SendImage), pars);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }

            }
            else
            {
                PS.SetLayerWithImage( Width, Height, Stride, BPP, ByteSize, Data, MaxPixelValue, MinPixelValue);
            }
        }
        //private event UpdateImageEvent UpdateImage;
        //delegate void UpdateImageEvent(Micromanager_net.CoreDevices.CoreImage cImage);
        
        /*void eCore_ImageProduced(Micromanager_net.CoreDevices.CoreImage cImage)
        {
            if (PS.InvokeRequired)
            {
                object[] pars = { cImage };
                //try
                {
                    PS.Invoke(UpdateImage , pars);
                }
                //catch { }

            }
            else
            {
                PS.SetLayerWithImage(0, cImage);
            }
        }
        */
        private void View_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                eCore.MMCamera.StopFocusMovie();
            }
            catch
            { }
            //PS.OnClosed(new EventArgs()); 
        }

        private void View_Load(object sender, EventArgs e)
        {

            PS.PauseMovieUpdates += new PaintDotNet.PauseMovieUpdatesEvent(PS_PauseMovieUpdates);
            PS.MoveUp += new PaintDotNet.MoveUpEvent(PS_MoveUp);
            PS.MoveDown += new PaintDotNet.MoveDownEvent(PS_MoveDown);
            PS.MoveRight += new PaintDotNet.MoveRightEvent(PS_MoveRight);
            PS.MoveLeft += new PaintDotNet.MoveLeftEvent(PS_MoveLeft);
            PS.Left = 0;//toolsFormControl1.Width;
           
            this.DockPanel.Height = PS.Height;
            this.DockPanel.Size = new Size(PS.Width, PS.Height);
            //this.DockPanel.DockLeftPortion =.01;
            //this.DockPanel.DockRightPortion = PS.Width;
            //this.Bounds = new Rectangle(this.Left, this.Top, PS.Width, PS.Height);
            //this.Width = PS.Width + historyFormControl1.Width + 10;
            //this.Height = PS.Height + 10;
           // PS.GetToolForm();
        }
        private void View_Resize(object sender, EventArgs e)
        {
            this.Width = PS.Width +  10;
            this.Height = PS.Height + 10;
        }

        void PS_PauseMovieUpdates(object sender)
        {
            try
            {
                eCore.MMCamera.StopFocusMovie();
            }
            catch { }
        }

        void PS_MoveLeft(object sender)
        {
            eCore.MMXYStage.MoveStageRelative(-1*eCore.ScreenSize *.75, 0);
        }

        void PS_MoveRight(object sender)
        {
            eCore.MMXYStage.MoveStageRelative(eCore.ScreenSize * .75, 0);
        }

        void PS_MoveDown(object sender)
        {
            eCore.MMXYStage.MoveStageRelative(0, -1 * eCore.ScreenSize * .75);
        }

        void PS_MoveUp(object sender)
        {
            eCore.MMXYStage.MoveStageRelative(0, eCore.ScreenSize * .75);
        }
        


        private void View_FormClosing(object sender, FormClosingEventArgs e)
        {
            PS.Dispose();
            //PS.OnClosing(new  CancelEventArgs());  
        }
    }
}
