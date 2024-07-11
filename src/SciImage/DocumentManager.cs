using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.Layers;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Forms;
using SciImage.Menus;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.FileHandling;
using SciImage.Plugins.FileHandling.FileTypeBase;
using SciImage.Plugins.Tools;
using SciImage.SciResources;
using SciImage.SystemLayer.Forms;
using SciImage.Core.History;

namespace SciImage
{
    public class DocumentManager
    {
        private static DocumentManager _DocumentManager = null;
        static DocumentManager()
        {
            _DocumentManager = new DocumentManager();
        }

        public static DocumentManager Manager
        {
            get { return _DocumentManager; }
        }

        private DocumentWorkspace activeDocumentWorkspace;

        private int suspendThumbnailUpdates = 0;

        // if a new workspace is added, and this workspace is not dirty, then it will be removed. 
        // This keeps track of the last workspace added via CreateBlankDocumentInNewWorkspace (if 
        // true was passed for its 2nd parameter)
        private DocumentWorkspace initialWorkspace;
        public DocumentWorkspace InitialWorkspace
        {
            get { return initialWorkspace; }
            set { initialWorkspace = value; }
        }

        private List<DocumentWorkspace> documentWorkspaces = new List<DocumentWorkspace>();
        public List<DocumentWorkspace> DocumentWorkspaces
        {
            get { return documentWorkspaces; }
            set { documentWorkspaces = value; }
        }


        public delegate void ActiveDocumentWorkSpaceChangedEvent(DocumentWorkspace newActive);
        public event ActiveDocumentWorkSpaceChangedEvent ActiveDocumentWorkSpaceChanged;

        public DocumentWorkspace ActiveDocumentWorkspace
        {
            get
            {
                return activeDocumentWorkspace;
            }

            set
            {
                if (value != activeDocumentWorkspace)
                {
                    if (value != null && this.documentWorkspaces.IndexOf(value) == -1)
                    {
                        return;
                        throw new ArgumentException("DocumentWorkspace was not created with AddNewDocumentWorkspace");
                    }

                    bool focused = false;
                    if (activeDocumentWorkspace != null)
                    {
                        focused = activeDocumentWorkspace.Focused;
                    }

                    UI.SuspendControlPainting(FormsManager.BaseForm);
                    OnActiveDocumentWorkspaceChanging();
                    activeDocumentWorkspace = value;
                    OnActiveDocumentWorkspaceChanged(activeDocumentWorkspace);
                    UI.ResumeControlPainting(FormsManager.BaseForm);

                    FormsManager.BaseForm.Refresh();

                    if (value != null)
                    {
                        value.Focus();
                    }
                }
            }
        }

        public void OnActiveDocumentWorkspaceChanged(DocumentWorkspace activeDocumentWorkspace)
        {
            if (activeDocumentWorkspace == null)
            {
                MenuManager.MainMenu.SetEnableSubMenu("PrintAction", false);
                MenuManager.MainMenu.SetEnableSubMenu("SaveFileAction", false);
            }
            else
            {
                activeDocumentWorkspace.SuspendLayout();

                MenuManager.MainMenu.SetEnableSubMenu("PrintAction", true);
                MenuManager.MainMenu.SetEnableSubMenu("SaveFileAction", true);

                activeDocumentWorkspace.BackColor = System.Drawing.SystemColors.ControlDark;
                activeDocumentWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
                activeDocumentWorkspace.DrawGrid = AppEnvironment.Environment.DrawGrid;
                activeDocumentWorkspace.PanelAutoScroll = true;
                activeDocumentWorkspace.RulersEnabled = AppEnvironment.Environment.RulersEnabled;
                activeDocumentWorkspace.TabIndex = 0;
                activeDocumentWorkspace.TabStop = false;
                //ActiveDocumentWorkspace.RulersEnabledChanged += this.DocumentWorkspace_RulersEnabledChanged;
                //ActiveDocumentWorkspace.DocumentMouseEnter += this.DocumentMouseEnterHandler;
                //ActiveDocumentWorkspace.DocumentMouseLeave += this.DocumentMouseLeaveHandler;
                //ActiveDocumentWorkspace.DocumentMouseMove += this.DocumentMouseMoveHandler;
                //ActiveDocumentWorkspace.DocumentMouseDown += this.DocumentMouseDownHandler;
                //ActiveDocumentWorkspace.Scroll += this.DocumentWorkspace_Scroll;
                //ActiveDocumentWorkspace.DrawGridChanged += this.DocumentWorkspace_DrawGridChanged;
                //ActiveDocumentWorkspace.DocumentClick += this.DocumentClick;
                //ActiveDocumentWorkspace.DocumentMouseUp += this.DocumentMouseUpHandler;
                //ActiveDocumentWorkspace.DocumentKeyPress += this.DocumentKeyPress;
                //ActiveDocumentWorkspace.DocumentKeyUp += this.DocumentKeyUp;
                //ActiveDocumentWorkspace.DocumentKeyDown += this.DocumentKeyDown;

                //if (_MDIWorkspace == true)
                //{
                //    Form hostform = FindHostMDIForm(ActiveDocumentWorkspace);
                //    if (hostform != null)
                //    {
                //        ActiveDocumentWorkspace.Visible = true;
                //        hostform.Activate();
                //    }
                //    else
                //    {
                //        ActiveDocumentWorkspace.Dock = DockStyle.Fill;
                //        //this.workspacePanel.Controls.Add(ActiveDocumentWorkspace);

                //        Form df = RequestFormCreation(ActiveDocumentWorkspace);

                //        /*
                //        PaintDNetWindow.PaintForms.MDIChildForm  df = new PaintDNetWindow.PaintForms.MDIChildForm ();


                //        df.MainUserControl = ActiveDocumentWorkspace;
                //        df.Show(this);
                //         */
                //        DocumentWorkspaceForms.Add((IControlHoldingForm)df);
                //        df.MdiChildActivate += new EventHandler(MDI_DocumentWorkspace_MdiChildActivate);
                //        df.GotFocus += new EventHandler(MDI_DocumentWorkspace_GotFocus);
                //        df.Activated += new EventHandler(MDI_DocumentWorkspace_Activated);
                //        df.FormClosed += new FormClosedEventHandler(MDI_DocumentWorkspace_FormClosed);
                //    }
                //}
                //else
                {
                    if (FormsManager.BaseForm.Controls.Contains(activeDocumentWorkspace))
                    {
                        activeDocumentWorkspace.Visible = true;
                    }
                    else
                    {
                        activeDocumentWorkspace.Dock = DockStyle.Fill;
                        FormsManager.BaseForm.Controls.Add(activeDocumentWorkspace);
                    }
                }


                //this.toolBar.ViewConfigStrip.ScaleFactor = ActiveDocumentWorkspace.ScaleFactor;
                //this.toolBar.ViewConfigStrip.ZoomBasis = ActiveDocumentWorkspace.ZoomBasis;

                // ActiveDocumentWorkspace.AppWorkspace = FormsManager.BaseForm;
                //ActiveDocumentWorkspace.History.Changed += HistoryChangedHandler;
                //ActiveDocumentWorkspace.StatusChanged += OnDocumentWorkspaceStatusChanged;
                //ActiveDocumentWorkspace.DocumentChanging += DocumentWorkspace_DocumentChanging;
                //ActiveDocumentWorkspace.DocumentChanged += DocumentWorkspace_DocumentChanged;
                //ActiveDocumentWorkspace.Selection.Changing += SelectedPathChangingHandler;
                //ActiveDocumentWorkspace.Selection.Changed += SelectedPathChangedHandler;
                //ActiveDocumentWorkspace.ScaleFactorChanged += ZoomChangedHandler;
                //ActiveDocumentWorkspace.ZoomBasisChanged += DocumentWorkspace_ZoomBasisChanged;

                activeDocumentWorkspace.Units = AppEnvironment.Environment.Units;

             //FormsManager.Manager.HistoryForm.HistoryStack = activeDocumentWorkspace.History;

                //ActiveDocumentWorkspace.ToolChanging += this.ToolChangingHandler;
                //ActiveDocumentWorkspace.ToolChanged += this.ToolChangedHandler;

                //this.toolBar.ViewConfigStrip.RulersEnabled = ActiveDocumentWorkspace.RulersEnabled;
                //this.toolBar.DocumentStrip.SelectDocumentWorkspace(ActiveDocumentWorkspace);

                activeDocumentWorkspace.SetToolFromType(ToolEnvironment.Environment.CurrentToolChoice);

                //UpdateSelectionToolbarButtons();
                //UpdateHistoryButtons();
                //UpdateDocInfoInStatusBar();

                activeDocumentWorkspace.ResumeLayout();
                activeDocumentWorkspace.PerformLayout();

                //ActiveDocumentWorkspace.FirstInputAfterGotFocus +=
                //    ActiveDocumentWorkspace_FirstInputAfterGotFocus;
            }

            if (ActiveDocumentWorkSpaceChanged != null)
            {
                ActiveDocumentWorkSpaceChanged(activeDocumentWorkspace);
            }

            //  UpdateStatusBarContextStatus();

        }

        public void OnActiveDocumentWorkspaceChanging()
        {
            if (ActiveDocumentWorkspaceChanging != null)
            {
                ActiveDocumentWorkspaceChanging(this, EventArgs.Empty);
            }

            if (ActiveDocumentWorkspace != null)
            {
                //ActiveDocumentWorkspace.FirstInputAfterGotFocus +=
                //    ActiveDocumentWorkspace_FirstInputAfterGotFocus;

                //ActiveDocumentWorkspace.RulersEnabledChanged -= this.DocumentWorkspace_RulersEnabledChanged;
                //ActiveDocumentWorkspace.DocumentMouseEnter -= this.DocumentMouseEnterHandler;
                //ActiveDocumentWorkspace.DocumentMouseLeave -= this.DocumentMouseLeaveHandler;
                //ActiveDocumentWorkspace.DocumentMouseMove -= this.DocumentMouseMoveHandler;
                //ActiveDocumentWorkspace.DocumentMouseDown -= this.DocumentMouseDownHandler;
                //ActiveDocumentWorkspace.Scroll -= this.DocumentWorkspace_Scroll;

                //ActiveDocumentWorkspace.DrawGridChanged -= this.DocumentWorkspace_DrawGridChanged;
                //ActiveDocumentWorkspace.DocumentClick -= this.DocumentClick;
                //ActiveDocumentWorkspace.DocumentMouseUp -= this.DocumentMouseUpHandler;
                //ActiveDocumentWorkspace.DocumentKeyPress -= this.DocumentKeyPress;
                //ActiveDocumentWorkspace.DocumentKeyUp -= this.DocumentKeyUp;
                //ActiveDocumentWorkspace.DocumentKeyDown -= this.DocumentKeyDown;

                //ActiveDocumentWorkspace.History.Changed -= HistoryChangedHandler;
                //ActiveDocumentWorkspace.StatusChanged -= OnDocumentWorkspaceStatusChanged;
                //ActiveDocumentWorkspace.DocumentChanging -= DocumentWorkspace_DocumentChanging;
                //ActiveDocumentWorkspace.DocumentChanged -= DocumentWorkspace_DocumentChanged;
                //ActiveDocumentWorkspace.Selection.Changing -= SelectedPathChangingHandler;
                //ActiveDocumentWorkspace.Selection.Changed -= SelectedPathChangedHandler;
                //ActiveDocumentWorkspace.ScaleFactorChanged -= ZoomChangedHandler;
                //ActiveDocumentWorkspace.ZoomBasisChanged -= DocumentWorkspace_ZoomBasisChanged;
                //if (_MDIWorkspace == false)
                //    ActiveDocumentWorkspace.Visible = false;
                //this.historyForm.HistoryControl.HistoryStack = null;

                //ActiveDocumentWorkspace.ToolChanging -= this.ToolChangingHandler;
                //ActiveDocumentWorkspace.ToolChanged -= this.ToolChangedHandler;

                if (ActiveDocumentWorkspace.Tool != null)
                {
                    while (ActiveDocumentWorkspace.Tool.IsMouseEntered)
                    {
                        ActiveDocumentWorkspace.Tool.PerformMouseLeave();
                    }
                }

                Type toolType = ActiveDocumentWorkspace.GetToolType();

                if (toolType != null)
                {
                    ToolEnvironment.Environment.CurrentToolChoice = ActiveDocumentWorkspace.GetToolType();
                }
            }
        }

        public DocumentWorkspace AddNewDocumentWorkspace()
        {
            if (this.initialWorkspace != null)
            {
                if (this.initialWorkspace.Document == null || !this.initialWorkspace.Document.Dirty)
                {
                    ToolEnvironment.Environment.CurrentToolChoice = this.initialWorkspace.GetToolType();
                    RemoveDocumentWorkspace(this.initialWorkspace);
                    this.initialWorkspace = null;
                }
            }

            DocumentWorkspace dw = new DocumentWorkspace();

            this.documentWorkspaces.Add(dw);

            dw.CreateControl();

            var ds = FormsManager.Manager.DocumentStrip;
            if (ds != null)
                ds.AddDocumentWorkspace(dw);

            ToolFactory.Factory.ToolClicked += dw.ToolEnvironment_ToolClicked;

            return dw;

        }

        public Image GetDocumentWorkspaceThumbnail(DocumentWorkspace dw)
        {
            //this.toolBar.DocumentStrip.SyncThumbnails();
            //Image[] images = this.toolBar.DocumentStrip.DocumentThumbnails;
            //DocumentWorkspace[] documents = this.toolBar.DocumentStrip.DocumentList;

            //for (int i = 0; i < documents.Length; ++i)
            //{
            //    if (documents[i] == dw)
            //    {
            //        return images[i];
            //    }
            //}

            //throw new ArgumentException("The requested DocumentWorkspace doesn't exist in this AppWorkspace");
            return null;
        }

        public void RemoveDocumentWorkspace(DocumentWorkspace documentWorkspace)
        {
            int dwIndex = this.documentWorkspaces.IndexOf(documentWorkspace);

            if (dwIndex == -1)
            {
                throw new ArgumentException("DocumentWorkspace was not created with AddNewDocumentWorkspace");
            }

            bool removingCurrentDW;
            if (DocumentManager.Manager.ActiveDocumentWorkspace == documentWorkspace)
            {
                removingCurrentDW = true;
                ToolEnvironment.Environment.CurrentToolChoice = documentWorkspace.GetToolType();

            }
            else
            {
                removingCurrentDW = false;
            }

            documentWorkspace.SetTool(null);

            // Choose new active DW if removing the current DW
            if (removingCurrentDW)
            {
                if (this.documentWorkspaces.Count == 1)
                {
                    DocumentManager.Manager.ActiveDocumentWorkspace = null;
                }
                else if (dwIndex == 0)
                {
                    DocumentManager.Manager.ActiveDocumentWorkspace = this.documentWorkspaces[1];
                }
                else
                {
                    DocumentManager.Manager.ActiveDocumentWorkspace = this.documentWorkspaces[dwIndex - 1];
                }
            }

            this.documentWorkspaces.Remove(documentWorkspace);

            if (FormsManager.Manager.DocumentStrip != null)
                FormsManager.Manager.DocumentStrip.RemoveDocumentWorkspace(documentWorkspace);

            if (this.initialWorkspace == documentWorkspace)
            {
                this.initialWorkspace = null;
            }

            // Clean up the DocumentWorkspace
            Document document = documentWorkspace.Document;

            documentWorkspace.Document = null;
            if (document != null) document.Dispose();

            documentWorkspace.Dispose();
            documentWorkspace = null;
        }

        public IDisposable SuspendThumbnailUpdates()
        {
            //CallbackOnDispose resumeFn = new CallbackOnDispose(ResumeThumbnailUpdates);

            //++this.suspendThumbnailUpdates;

            //if (this.suspendThumbnailUpdates == 1)
            //{
            //    Widgets.DocumentStrip.SuspendThumbnailUpdates();
            //    Widgets.LayerControl.SuspendLayerPreviewUpdates();
            //}

            //return resumeFn;
            return null;
        }

        private void ResumeThumbnailUpdates()
        {
            --this.suspendThumbnailUpdates;

            if (this.suspendThumbnailUpdates == 0)
            {
                //  Widgets.DocumentStrip.ResumeThumbnailUpdates();
                //   Widgets.LayerControl.ResumeLayerPreviewUpdates();
            }
        }

        public event EventHandler OnZoomToScale;
        public void ZoomToScale(Core.ScaleFactor scaleFactor)
        {
            if (activeDocumentWorkspace != null)
            {
                if (activeDocumentWorkspace.ZoomBasis == Core.ZoomBasis.ScaleFactor)
                {
                    activeDocumentWorkspace.ScaleFactor = scaleFactor;
                    if (OnZoomToScale != null)
                        OnZoomToScale(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler OnZoomIn;
        public void ZoomIn()
        {
            if (activeDocumentWorkspace != null)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.ZoomIn();
                if (OnZoomIn != null)
                    OnZoomIn(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnZoomOut;
        public void ZoomOut()
        {
            if (activeDocumentWorkspace != null)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.ZoomOut();
                if (OnZoomOut != null)
                    OnZoomOut(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnZoomBasis;
        public void ZoomBasis(ZoomBasis zoomBasis)
        {
            if (activeDocumentWorkspace != null)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.ZoomBasis = zoomBasis;
                if (OnZoomBasis != null)
                    OnZoomBasis(this, EventArgs.Empty);
            }
        }

        public void EnsureActiveDocFullyVisible()
        {
            FormsManager.Manager.DocumentStrip.EnsureItemFullyVisible(FormsManager.Manager.DocumentStrip.SelectedDocumentIndex);
        }

        public event EventHandler ActiveDocumentWorkspaceChanging;

        /// <summary>
        /// Creates a blank document of the given size in a new workspace, and activates that workspace.
        /// </summary>
        /// <remarks>
        /// If isInitial=true, then last workspace added by this method is kept track of, and if it is not modified by
        /// the time the next workspace is added, then it will be removed.
        /// </remarks>
        /// <returns>true if everything was successful, false if there wasn't enough memory</returns>
        public bool CreateBlankDocumentInNewWorkspace(Size size, MeasurementUnit dpuUnit, double dpu, bool isInitial)
        {
            DocumentWorkspace dw1 = ActiveDocumentWorkspace;
            if (dw1 != null)
            {
                dw1.SuspendRefresh();
            }

            try
            {
                Document untitled = new Document(size.Width, size.Height);
                untitled.DpuUnit = dpuUnit;
                untitled.DpuX = dpu;
                untitled.DpuY = dpu;

                BitmapLayer bitmapLayer;

                try
                {
                    using (new WaitCursorChanger())
                    {
                        bitmapLayer = (BitmapLayer)Layer.CreateBackgroundLayer(size.Width, size.Height, ColorBgra.White);
                    }
                }
                catch (OutOfMemoryException)
                {
                    Utility.ErrorBox(FormsManager.BaseForm, SciResources.SciResources.GetString("NewImageAction.Error.OutOfMemory"));
                    return false;
                }

                using (new WaitCursorChanger())
                {
                    bool focused = false;

                    if (ActiveDocumentWorkspace != null && ActiveDocumentWorkspace.Focused)
                    {
                        focused = true;
                    }

                    untitled.Layers.Add(bitmapLayer);

                    DocumentWorkspace dw = DocumentManager.Manager.AddNewDocumentWorkspace();
                    if (FormsManager.Manager.DocumentStrip != null)
                        FormsManager.Manager.DocumentStrip.LockDocumentWorkspaceDirtyValue(dw, false);
                    dw.SuspendRefresh();

                    try
                    {
                        dw.Document = untitled;
                    }
                    catch (OutOfMemoryException)
                    {
                        Utility.ErrorBox(FormsManager.BaseForm, SciResources.SciResources.GetString("NewImageAction.Error.OutOfMemory"));
                        DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                        untitled.Dispose();
                        return false;
                    }

                    dw.ActiveLayer = (Layer)dw.Document.Layers[0];

                    ActiveDocumentWorkspace = dw;

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

                    ActiveDocumentWorkspace.Focus();


                    if (FormsManager.Manager.DocumentStrip != null)
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

        public bool OpenFileInNewWorkspace(string fileName, bool addToMruList)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentOutOfRangeException("fileName.Length == 0");
            }

            SciBaseForm.UpdateAllForms();

            FileType fileType;
            Document document;

            AppEnvironment.Environment.ResetProgressStatusBar();

            ProgressEventHandler progressCallback =
                delegate (object sender, ProgressEventArgs e)
                {

                    AppEnvironment.Environment.SetProgressStatusBar(e.Percent);
                };

            document = DocumentWorkspace.LoadDocument(FormsManager.BaseForm, fileName, out fileType, progressCallback);

            AppEnvironment.Environment.EraseProgressStatusBar();

            if (document == null)
            {
                // FormsManager.BaseForm .Cursor = FormsManager.BaseForm.Cursors.Default;
            }
            else
            {
                using (new WaitCursorChanger())
                {
                    DocumentWorkspace dw = DocumentManager.Manager.AddNewDocumentWorkspace();
               //     FormsManager.Manager.DocumentStrip.LockDocumentWorkspaceDirtyValue(dw, false);

                    try
                    {
                        dw.Document = document;
                    }
                    catch (OutOfMemoryException)
                    {
                        Utility.ErrorBox(FormsManager.BaseForm, SciResources.SciResources.GetString("LoadImage.Error.OutOfMemoryException"));
                        DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                        document.Dispose();
                        return false;
                    }

                    dw.ActiveLayer = (Layer)document.Layers[0];

                    dw.SetDocumentSaveOptions(fileName, fileType, null);

                    ActiveDocumentWorkspace = dw;

                    dw.History.ClearAll();

                    dw.History.PushNewMemento(
                        new NullHistoryMemento(
                            SciResources.SciResources.GetString("OpenImageAction.Name"),
                            SciResources.SciResources.GetImageResource("Icons.ImageFromDiskIcon.png")));

                    document.Dirty = false;
              //      FormsManager.Manager.DocumentStrip.UnlockDocumentWorkspaceDirtyValue(dw);
                }

                if (document != null)
                {
                    ActiveDocumentWorkspace.ZoomBasis = Core.ZoomBasis.FitToWindow;
                }

                // add to MRU list
                if (addToMruList)
                {
                    ActiveDocumentWorkspace.AddToMruList();
                }

               // FormsManager.Manager.DocumentStrip.SyncThumbnails();

                WarnAboutSavedWithVersion(document.SavedWithVersion);
            }

            if (ActiveDocumentWorkspace != null)
            {
                ActiveDocumentWorkspace.Focus();
            }

            return document != null;
        }

        protected void WarnAboutSavedWithVersion(Version savedWith)
        {
            // warn about version?
            // 2.1 Build 1897 signifies when the file format changed and broke backwards compatibility (for saving)
            // 2.1 Build 1921 signifies when MemoryBlock was upgraded to support 64-bits, which broke it again
            // 2.1 Build 1924 upgraded to "unimportant ordering" for MemoryBlock serialization so we can to faster multiproc saves
            //                (in v2.5 we always save in order, although that doesn't change the file format's laxness)
            // 2.5 Build 2105 changed the way PropertyItems are serialized
            // 2.6 Build      upgrade to .NET 2.0, does not appear to be compatible with 2.5 and earlier files as a result
            if (savedWith < new Version(2, 6, 0))
            {
                Version ourVersion = SciInfo.GetVersion();
                Version ourVersion2 = new Version(ourVersion.Major, ourVersion.Minor);
                Version ourVersion3 = new Version(ourVersion.Major, ourVersion.Minor, ourVersion.Build);

                int fields;

                if (savedWith < ourVersion2)
                {
                    fields = 2;
                }
                else
                {
                    fields = 3;
                }

                string format = SciResources.SciResources.GetString("SavedWithOlderVersion.Format");
                string text = string.Format(format, savedWith.ToString(fields), ourVersion.ToString(fields));

                // TODO: should we even bother to inform them? It is probably more annoying than not,
                //       especially since older versions will say "Hey this file is corrupt OR saved with a newer version"
                //Utility.InfoBox(this, text);
            }
        }

        /// <summary>
        /// Computes what the size of a new document should be. If the screen is in a normal,
        /// wider-than-tall (landscape) mode then it returns 800x600. If the screen is in a
        /// taller-than-wide (portrait) mode then it retusn 600x800. If the screen is square
        /// then it returns 800x600.
        /// </summary>
        public Size GetNewDocumentSize()
        {
            SciBaseForm findForm = FormsManager.BaseForm.FindForm() as SciBaseForm;

            if (findForm != null && findForm.ScreenAspect < 1.0)
            {
                return new Size(600, 800);
            }
            else
            {
                return new Size(800, 600);
            }
        }

        public bool OpenPictureInNewWorkspace(Image image, string Suggestedfilename)
        {
            string fileName = Suggestedfilename;

            if (image == null)
            {
                throw new ArgumentNullException("Image");
            }

            SciBaseForm.UpdateAllForms();

            Document document = null;


            AppEnvironment.Environment.ResetProgressStatusBar();

            ProgressEventHandler progressCallback =
                delegate (object sender, ProgressEventArgs e)
                {
                    AppEnvironment.Environment.SetProgressStatusBar(e.Percent);
                };
            document = Document.FromImage(image);


            AppEnvironment.Environment.EraseProgressStatusBar();

            if (document == null)
            {
                // this.Cursor = Cursors.Default;
            }
            else
            {
                using (new WaitCursorChanger())
                {
                    DocumentWorkspace dw = DocumentManager.Manager.AddNewDocumentWorkspace();
                    FormsManager.Manager.DocumentStrip.LockDocumentWorkspaceDirtyValue(dw, false);

                    try
                    {
                        dw.Document = document;
                    }

                    catch (OutOfMemoryException)
                    {
                        Utility.ErrorBox(FormsManager.BaseForm, SciResources.SciResources.GetString("LoadImage.Error.OutOfMemoryException"));
                        DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                        document.Dispose();
                        return false;
                    }

                    dw.ActiveLayer = (Layer)document.Layers[0];

                    FileTypeCollection fileTypes;
                    int ftIndex;
                    FileType fileType;
                    try
                    {
                        fileTypes = FileTypes.GetFileTypes();
                        ftIndex = fileTypes.IndexOfExtension("tif");

                        if (ftIndex == -1)
                        {
                            Utility.ErrorBox(FormsManager.BaseForm, SciResources.SciResources.GetString("LoadImage.Error.ImageTypeNotRecognized"));
                            return (false);
                        }

                        fileType = fileTypes[ftIndex];
                    }

                    catch (ArgumentException)
                    {
                        string format = SciResources.SciResources.GetString("LoadImage.Error.InvalidFileName.Format");
                        string error = string.Format(format, fileName);
                        Utility.ErrorBox(FormsManager.BaseForm, error);
                        return (false);
                    }


                    dw.SetDocumentSaveOptions(fileName, fileType, null);

                    ActiveDocumentWorkspace = dw;

                    dw.History.ClearAll();

                    dw.History.PushNewMemento(
                        new NullHistoryMemento(
                            SciResources.SciResources.GetString("OpenImageAction.Name"),
                            SciResources.SciResources.GetImageResource("Icons.ImageFromDiskIcon.png")));

                    document.Dirty = false;
                    FormsManager.Manager.DocumentStrip.UnlockDocumentWorkspaceDirtyValue(dw);
                }

                if (document != null)
                {
                    ActiveDocumentWorkspace.ZoomBasis = Core.ZoomBasis.FitToWindow;
                }


                FormsManager.Manager.DocumentStrip.SyncThumbnails();

                WarnAboutSavedWithVersion(document.SavedWithVersion);
            }

            if (ActiveDocumentWorkspace != null)
            {
                ActiveDocumentWorkspace.Focus();
            }

            return document != null;
        }

        public bool OpenFilesInNewWorkspace(string[] fileNames)
        {
            if (FormsManager.BaseForm.IsDisposed)
            {
                return false;
            }

            bool result = true;

            foreach (string fileName in fileNames)
            {
                result &= OpenFileInNewWorkspace(fileName);

                if (!result)
                {
                    break;
                }
            }

            return result;
        }

        public bool OpenFileInNewWorkspace(string fileName)
        {
            return OpenFileInNewWorkspace(fileName, true);
        }


        //private void DocumentWorkspace_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        //{
        //    OnScroll(e);
        //}

        //private void DocumentWorkspace_DocumentChanging(object sender, EventArgs<Document> e)
        //{
        //    UI.SuspendControlPainting(this);
        //}

        //private void DocumentWorkspace_DocumentChanged(object sender, EventArgs e)
        //{
        //    UI.ResumeControlPainting(this);
        //    Invalidate(true);
        //}

        //private void ActiveDocumentWorkspace_FirstInputAfterGotFocus(object sender, EventArgs e)
        //{
        //    DocumentManager.Manager.EnsureActiveDocFullyVisible();
        //}
    }
}

