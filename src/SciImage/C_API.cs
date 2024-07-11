using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.PaintForms.ColorPickers;
using SciImage.PaintForms.HistoryForm;
using SciImage.PaintForms.LayerForm;
using SciImage.PaintForms.ToolsForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.Layers;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Core.Surfaces.Layers.IntensityLayers;
using SciImage.Menus;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;
using SciImage.Plugins.Tools;
using SciImage.PaintForms;

namespace SciImage
{

    /// <summary>
    /// it is a pain in the butt to deal with finding methods on labview or matlab.  This class holds all the methods needed to control the user control from outside the interface
    /// </summary>
    public class C_API
    {
        public C_API(AppWorkspace appWorkspace)
        {
            _AppWorkspace = appWorkspace;
        }

        private AppWorkspace _AppWorkspace = null;
        public AppWorkspace AppWorkspace
        {
            get
            {
                return _AppWorkspace;
            }
        }

        public bool DrawGrid
        {
            get
            {
                return AppEnvironment.Environment.DrawGrid;
            }

            set
            {
                AppEnvironment.Environment.DrawGrid = value;
            }
        }

        public DocumentWorkspace ActiveDocumentWorkspace
        {
            get
            {
                return DocumentManager.Manager.ActiveDocumentWorkspace;
            }

            set
            {
                DocumentManager.Manager.ActiveDocumentWorkspace = value;
            }
        }

        public bool RulersEnabled
        {
            get
            {
                return AppEnvironment.Environment.RulersEnabled;
            }

            set
            {
                AppEnvironment.Environment.RulersEnabled = value;
            }
        }

        public event RequestMovieStopEvent RequestMovieStop;

        void dw1_RequestMovieStop(object sender)
        {
            if (RequestMovieStop != null) RequestMovieStop(sender);
        }

        public event PauseMovieUpdatesEvent PauseMovieUpdates;

        public bool BlockMovieUpdates = false;

        private Layer CreateContextsensitiveLayer(Image NewImage)
        {
            Layer layer = null;
            if (NewImage.PixelFormat == PixelFormat.Format8bppIndexed
                            || NewImage.PixelFormat == PixelFormat.Format16bppGrayScale)
            {
                layer = (IntensityLayer)IntensityLayer.FromImage(NewImage);
                layer.IsBackground = true;

            }
            else
            {
                layer = Layer.CreateBackgroundLayer(NewImage.Width, NewImage.Height, new ColorBgra());
                //blayer.BlendOp = UserBlendOps.OverlayBlendOp;
            }
            return layer;
        }

        private void QuietReplace(int LayerIndex, Image NewImage)
        {
            if (LayerIndex > DocumentManager.Manager.ActiveDocumentWorkspace.Document.Layers.Count)
            {
                for (int i = DocumentManager.Manager.ActiveDocumentWorkspace.Document.Layers.Count; i < LayerIndex; i++)
                {
                    Layer layer = CreateContextsensitiveLayer(NewImage);
                    DocumentManager.Manager.ActiveDocumentWorkspace.Document.Layers.Add(layer);
                }
            }
            if (NewImage.PixelFormat == PixelFormat.Format16bppGrayScale ||
                NewImage.PixelFormat == PixelFormat.Format8bppIndexed)
            {

                ((IntensityLayer)ActiveDocumentWorkspace.Document.Layers[LayerIndex]).Surface.ReplaceImage(NewImage);
                // we invalidate each blayer so that the blayer previews refresh themselves
                ((Layer)ActiveDocumentWorkspace.Document.Layers[LayerIndex]).Invalidate();
            }
            else
            {
                ((BitmapLayer)DocumentManager.Manager.ActiveDocumentWorkspace.Document.Layers[LayerIndex]).Surface.ReplaceImage(NewImage);
                // we invalidate each blayer so that the blayer previews refresh themselves
                ((Layer)DocumentManager.Manager.ActiveDocumentWorkspace.Document.Layers[LayerIndex]).Invalidate();
            }
        }

        public void UpdatesPaused()
        {
            Document doc = ActiveDocumentWorkspace.Document;
            //todo: need to make sure that the image is only sent to an apporiate document that is the right size and width.
            DocumentManager.Manager.SuspendThumbnailUpdates();

            for (int i = 0; i < doc.Layers.Count; i++)
            {
                doc.Layers.SetAt(i, (Layer)doc.Layers[i]);

            }


            // we invalidate each blayer so that the blayer previews refresh themselves
            foreach (Layer layer in doc.Layers)
            {
                layer.Invalidate();
            }

            bool oldDirty = doc.Dirty;
            doc.Invalidate();
            doc.Dirty = oldDirty;
            ActiveDocumentWorkspace.Update(true);

        }

        public void SetLayerWithImage(ImageWithContrast[] NewImages)
        {
            if (ActiveDocumentWorkspace == null || BlockMovieUpdates == true)
            {
                SetBackgroundImage(NewImages[0].TheImage);
            }

            ActiveDocumentWorkspace.MovieRunningOnBackGroundLayer = true;
            Document doc = ActiveDocumentWorkspace.Document;
            //todo: need to make sure that the image is only sent to an apporiate document that is the right size and width.
            DocumentManager.Manager.SuspendThumbnailUpdates();

            for (int i = 0; i < NewImages.Length; i++)
            {
                QuietReplace(i, NewImages[i].TheImage);
                if (ActiveDocumentWorkspace.Document.Layers[i].GetType() == typeof(IntensityLayer))
                {
                    ((IntensityLayer)ActiveDocumentWorkspace.Document.Layers[i]).MaxIntensity = NewImages[i].MaxIntensity;
                    ((IntensityLayer)ActiveDocumentWorkspace.Document.Layers[i]).MinIntensity = NewImages[i].MinIntensity;
                }
            }

            bool oldDirty = doc.Dirty;
            doc.Invalidate();
            doc.Dirty = oldDirty;
            ActiveDocumentWorkspace.Update(true);
        }

        public void SetLayerWithImage(int LayerIndex, ImageWithContrast NewImage)
        {
            if (ActiveDocumentWorkspace == null || BlockMovieUpdates == true)
            {
                SetBackgroundImage(NewImage.TheImage);
            }
            ActiveDocumentWorkspace.MovieRunningOnBackGroundLayer = true;
            Document doc = ActiveDocumentWorkspace.Document;
            DocumentManager.Manager.SuspendThumbnailUpdates();

            QuietReplace(LayerIndex, NewImage.TheImage);

            if (ActiveDocumentWorkspace.Document.Layers[LayerIndex].GetType() == typeof(IntensityLayer))
            {
                ((IntensityLayer)ActiveDocumentWorkspace.Document.Layers[LayerIndex]).MaxIntensity = NewImage.MaxIntensity;
                ((IntensityLayer)ActiveDocumentWorkspace.Document.Layers[LayerIndex]).MinIntensity = NewImage.MinIntensity;
            }

            bool oldDirty = doc.Dirty;
            doc.Invalidate();
            //doc.Dirty = oldDirty;
            ActiveDocumentWorkspace.Update(true);
        }

        public void SetLayerWithImage(Image[] NewImages)
        {
            if (ActiveDocumentWorkspace == null || BlockMovieUpdates == true)
            {
                SetBackgroundImage(NewImages[0]);
            }

            ActiveDocumentWorkspace.MovieRunningOnBackGroundLayer = true;
            Document doc = ActiveDocumentWorkspace.Document;
            //todo: need to make sure that the image is only sent to an apporiate document that is the right size and width.
            DocumentManager.Manager.SuspendThumbnailUpdates();

            for (int i = 0; i < NewImages.Length; i++)
            {
                QuietReplace(i, NewImages[i]);
            }

            bool oldDirty = doc.Dirty;
            doc.Invalidate();
            doc.Dirty = oldDirty;
            ActiveDocumentWorkspace.Update(true);
        }

        public void SetLayerWithImage(int LayerIndex, Image NewImage)
        {
            if (ActiveDocumentWorkspace == null || BlockMovieUpdates == true)
            {
                SetBackgroundImage(NewImage);
            }
            ActiveDocumentWorkspace.MovieRunningOnBackGroundLayer = true;
            Document doc = ActiveDocumentWorkspace.Document;
            DocumentManager.Manager.SuspendThumbnailUpdates();

            QuietReplace(LayerIndex, NewImage);

            bool oldDirty = doc.Dirty;
            doc.Invalidate();
            doc.Dirty = oldDirty;
            ActiveDocumentWorkspace.Update(true);
        }

        private void SetBackgroundImage(Image NewImage)
        {
            BlockMovieUpdates = false;
            CreateImageDocumentInNewWorkspace(NewImage, true);
            //ActiveDocumentWorkspace.RequestMovieStop += new RequestMovieStopEvent(ActiveDocumentWorkspace_RequestMovieStop);
        }

        public bool CreateImageDocumentInNewWorkspace(Image NewImage, bool isInitial)
        {
            DocumentWorkspace dw1 = DocumentManager.Manager.ActiveDocumentWorkspace;
            if (dw1 != null)
            {
                dw1.RequestMovieStop += new RequestMovieStopEvent(dw1_RequestMovieStop);
                dw1.SuspendRefresh();
            }

            try
            {
                Document untitled = new Document(NewImage.Width, NewImage.Height);
                Layer layer = null;
                try
                {
                    using (new WaitCursorChanger(AppWorkspace))
                    {

                        layer = CreateContextsensitiveLayer(NewImage);
                    }
                }
                catch (OutOfMemoryException)
                {
                    Utility.ErrorBox(AppWorkspace, SciResources.SciResources.GetString("NewImageAction.Error.OutOfMemory"));
                    return false;
                }

                untitled.Layers.Add(layer);

                using (new WaitCursorChanger(AppWorkspace))
                {
                    bool focused = false;

                    if (DocumentManager.Manager.ActiveDocumentWorkspace != null && DocumentManager.Manager.ActiveDocumentWorkspace.Focused)
                    {
                        focused = true;
                    }



                    DocumentWorkspace dw = DocumentManager.Manager.AddNewDocumentWorkspace();
                    FormsManager.Manager.DocumentStrip.LockDocumentWorkspaceDirtyValue(dw, false);
                    dw.SuspendRefresh();

                    try
                    {
                        dw.Document = untitled;
                    }

                    catch (OutOfMemoryException)
                    {
                        Utility.ErrorBox(AppWorkspace, SciResources.SciResources.GetString("NewImageAction.Error.OutOfMemory"));
                        DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                        untitled.Dispose();
                        return false;
                    }

                    dw.ActiveLayer = (Layer)dw.Document.Layers[0];

                    DocumentManager.Manager.ActiveDocumentWorkspace = dw;

                    dw.SetDocumentSaveOptions(null, null, null);
                    dw.History.ClearAll();
                    dw.History.PushNewMemento(
                        new NullHistoryMemento("New Image",
                        SciResources.SciResources.GetImageResource("Icons.MenuFileNewIcon.png")));

                    dw.Document.Dirty = false;
                    dw.ResumeRefresh();


                    if (isInitial)
                    {
                        DocumentManager.Manager.InitialWorkspace = dw;
                    }

                    PerformAction("AddNewBlankLayerAction");

                    if (focused)
                    {
                        ActiveDocumentWorkspace.Focus();
                    }

                    FormsManager.Manager.DocumentStrip.UnlockDocumentWorkspaceDirtyValue(dw);
                }
            }

            finally
            {
                if (dw1 != null)
                {
                    dw1.ResumeRefresh();
                }
            }

            return true;
        }

        public void AutoSave(string filename)
        {

            throw (new Exception("Not implemented"));
            // activeDocumentWorkspace.DoSaveAsJPG(filename);
        }

        void ActiveDocumentWorkspace_RequestMovieStop(object sender)
        {
            BlockMovieUpdates = true;
            if (PauseMovieUpdates != null) PauseMovieUpdates(this);

        }

        public bool OpenFilesInNewWorkspace(string[] fileNames)
        {
            return DocumentManager.Manager.OpenFilesInNewWorkspace(fileNames);
        }

        public bool OpenFileInNewWorkspace(string fileName)
        {
            return DocumentManager.Manager.OpenFileInNewWorkspace(fileName, true);
        }

        public bool CreateBlankDocumentInNewWorkspace(Size size, MeasurementUnit dpuUnit, double dpu, bool isInitial)
        {
            return DocumentManager.Manager.CreateBlankDocumentInNewWorkspace(size, dpuUnit, dpu, isInitial);
        }

        public Type DefaultToolType
        {
            get
            {
                return ToolFactory.Factory.DefaultToolType;
            }

            set
            {
                ToolFactory.Factory.DefaultToolType = value;
            }
        }

        public Type ToolChoice
        {
            get { return ToolEnvironment.Environment.CurrentToolChoice; }
            set { ToolEnvironment.Environment.CurrentToolChoice = value; }
        }

        public bool PerformAction(string ActionType)
        {
            return ActionFactory.PerformAction(ActionType);
        }

        public IToolPicker ToolForm
        {
            get
            {
                return FormsManager.Manager.ToolsForm;
            }
        }

        public ILayerForm LayerForm
        {
            get
            {
                return FormsManager.Manager.LayerForm;
            }
        }

        public PaintForms.IColorPicker ColorsForm
        {
            get
            {
                return FormsManager.Manager.ColorsForm;
            }
        }

        public IHistoryForm HistoryForm
        {
            get
            {
                return FormsManager.Manager.HistoryForm;
            }
        }

        public List<Menus.SciMenuItem> Menus()
        {
            return MenuManager.MainMenu.Menus;

        }

        //public SciToolBar ToolBar()
        //{
        //    toolBar = new SciToolBar();
        //}

        public void InitializeImageC()
        {
            AppEnvironment.Environment.Initialize();
            // statusBar = new SciStatusBar();
            MenuManager.MainMenu.CreateMenus();
            //ToolConfigStrip = new ToolConfigStrip();
            //ToolConfigStrip.ToolBarConfigItems = ToolBarConfigItems.None;
            //toolConfigStrip.LoadFromToolEnvironment(ToolEnvironment.Environment);

            //ToolBar.ToolChooserStrip.SetTools(ToolFactory.Factory.ToolInfos.ToArray());

            //AppEnvironment.Environment.ToolBar.ToolChooserStrip.ToolClicked += new ToolClickedEventHandler(this.MainToolBar_ToolClicked);

            ToolEnvironment.Environment.PerformAllChanged();
        }
    }
}
