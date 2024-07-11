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

    public partial class NiViewer :UserControl  ,Micromanager_net.IPictureView,Micromanager_net.ISubMDI  // PaintDotNet.FloatingToolForm   
    {
        Micromanager_net. CoreDevices.EasyCore eCore;
        private PaintDotNet.AppWorkspace PS;
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

            this.PS = new PaintDotNet.AppWorkspace();
            PS.MakeNewForm += new MakeNewMDISubFormEvent(PS_MakeNewForm);
            this.SuspendLayout();
            // 
            // PS
            // 
            this.PS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PS.Location = new System.Drawing.Point(0, 0);
            this.PS.Name = "PS";
            this.PS.RulersEnabled = false;
            this.PS.Size = new System.Drawing.Size(1046, 651);
            this.PS.TabIndex = 1;
            //this.PS.Units = PaintDotNet.MeasurementUnit.Pixel;

            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1046, 651);
            this.Controls.Add(this.PS);
            this.MinimumSize = new System.Drawing.Size(820, 685);
            this.Name = "View";
            this.Text = "View";
            this.Load += new System.EventHandler(this.View_Load);
            this.ResumeLayout(false);



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
              
            }
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
        public NiViewer()
        {
            InitializeComponent();
            
        }

        void PS_MakeNewForm(object sender, Form NewForm)
        {
            if (OnNewForm != null)
            {
                OnNewForm(this, new Micromanager_net.NewFormEvents(NewForm));

            }
            else
                NewForm.Show();
        }
        public void SetCore(Micromanager_net.CoreDevices.EasyCore Ecore)
        {
            eCore = Ecore;
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
       
        private void View_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                eCore.MMCamera.StopFocusMovie();
            }
            catch
            { }
        }

        private void View_Load(object sender, EventArgs e)
        {

            PS.PauseMovieUpdates += new PaintDotNet.PauseMovieUpdatesEvent(PS_PauseMovieUpdates);
            PS.MoveUp += new PaintDotNet.MoveUpEvent(PS_MoveUp);
            PS.MoveDown += new PaintDotNet.MoveDownEvent(PS_MoveDown);
            PS.MoveRight += new PaintDotNet.MoveRightEvent(PS_MoveRight);
            PS.MoveLeft += new PaintDotNet.MoveLeftEvent(PS_MoveLeft);
            PS.Left = 0;
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

