using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SciImage.Core;
using SciImage.Menus;
using SciImage.Menus.Strips;
using SciImage.PaintForms.ColorPickers;
using SciImage.PaintForms.HistoryForm;
using SciImage.PaintForms.LayerForm;
using SciImage.PaintForms.ToolsForm;
using SciImage.PaintForms.UserControls.ColorPickers;
using SciImage.Plugins.Tools;
using SciImage.PaintForms;

namespace SciImage
{
    public class FormsManager
    {
        public static AppWorkspace BaseForm = null;

        public static FormsManager Manager;
        //prevent non static construction
        private FormsManager() { }

        static FormsManager()
        {
            Manager = new FormsManager();
        }

        public event MakeNewMDISubFormEvent MakeNewForm;
        public event MakeFormFromUserControl MakeFormFromUsercontrol;

        public void Initialize()
        {

        }


        private void InitializeToolForm(ref IToolPicker TFC)
        {

            this.toolsForm = TFC;

            TFC.SetTools(ToolFactory.Factory.ToolInfos.ToArray());
            ToolEnvironment.Environment.ToolChosen += TFC.Environment_ToolChosen;
            //this.mainToolBarForm.ToolsControl.ToolClicked += new ToolClickedEventHandler(this.MainToolBar_ToolClicked);

        }

        private void InitializeLayerForm(ref ILayerForm LFC)
        {
            layerForm = LFC;
            DocumentManager.Manager.ActiveDocumentWorkSpaceChanged += LFC.SetupNewDocument;
            //layerForm.RelinquishFocus += RelinquishFocusHandler;
            //layerForm.ProcessCmdKeyEvent += OnToolFormProcessCmdKeyEvent;
        }
        private void InitializeColorsForm(ref IColorPicker CFC)
        {
            colorsForm = CFC;

            colorsForm.PaletteCollection = new PaletteCollection();
            colorsForm.WhichUserColor = WhichUserColor.Primary;

            ToolEnvironment.Environment.PrimaryColorChanged += CFC._PrimaryColorChanged;
            ToolEnvironment.Environment.SecondaryColorChanged += CFC._SecondaryColorChanged;

            //colorsForm.RelinquishFocus += RelinquishFocusHandler;
            //colorsForm.ProcessCmdKeyEvent += OnToolFormProcessCmdKeyEvent;
        }
        private void InitializeHistoryForm(ref IHistoryForm HFC)
        {
            DocumentManager.Manager.ActiveDocumentWorkSpaceChanged += HFC.NewActiveDocument;
        }
        private void InitializeStatusBar(ref IStatusBarProgress statusBar)
        {
            //AppEnvironment.Environment.StatusUpdated+=
        }

        private SciToolBar toolBar = null;
        public SciToolBar ToolBar
        {
            get
            {
                return toolBar;
            }
        }

        private IStatusBarProgress statusBar = null;
        public IStatusBarProgress StatusBar
        {
            get
            {
                if (statusBar == null)
                {
                    statusBar = new SciStatusBar();
                    InitializeStatusBar(ref statusBar);
                }

                return statusBar;
            }

            set
            {
                statusBar = value;
                InitializeStatusBar(ref statusBar);
            }
        }

        private DocumentStrip documentStrip = null;
        public DocumentStrip DocumentStrip
        {
            get
            {
                return this.documentStrip;
            }

            set
            {
                this.documentStrip = value;
            }
        }

        private ViewConfigStrip viewConfigStrip = null;
        public ViewConfigStrip ViewConfigStrip
        {
            get
            {
                return this.viewConfigStrip;
            }

            set
            {
                this.viewConfigStrip = value;
            }
        }

        private ToolConfigStrip toolConfigStrip = null;
        public  ToolConfigStrip ToolConfigStrip
        {
            get
            {
                return this.toolConfigStrip;
            }

            set
            {
                this.toolConfigStrip = value;
            }
        }

        //private CommonActionsStrip commonActionsStrip;
        //public CommonActionsStrip CommonActionsStrip
        //{
        //    get
        //    {
        //        return this.commonActionsStrip;
        //    }

        //    set
        //    {
        //        this.commonActionsStrip = value;
        //    }
        //}

        #region Toolsform
        private IToolPicker toolsForm = null;
        public IToolPicker ToolsForm
        {
            get
            {
                if (toolsForm == null)
                {
                    toolsForm = new ToolsFormControl();
                    InitializeToolForm(ref toolsForm);
                }
                return this.toolsForm;
            }

            set
            {
                this.toolsForm = value;
                InitializeToolForm(ref toolsForm);
            }
        }

        #endregion

        #region LayerControl
        private ILayerForm layerForm = null;
        public ILayerForm LayerForm
        {
            get
            {
                if (layerForm == null)
                {
                    layerForm = new LayerFormControl();
                    InitializeLayerForm(ref layerForm);
                }
                return layerForm;
            }

            set
            {
                layerForm = value;
                InitializeLayerForm(ref layerForm);
            }
        }
        public void SuspendLayerPreviewUpdates()
        {
            if (layerForm != null)
                layerForm.SuspendLayerPreviewUpdates();
        }
        public void ResumeLayerPreviewUpdates()
        {
            if (layerForm != null)
                layerForm.ResumeLayerPreviewUpdates();
        }
        #endregion

        #region History Control

        private IHistoryForm historyForm = null;
        public IHistoryForm HistoryForm
        {
            get
            {
                if (historyForm == null)
                {
                    historyForm = new HistoryFormControl();
                    InitializeHistoryForm(ref historyForm);
                }
                return this.historyForm;
            }

            set
            {
                this.historyForm = value;
                InitializeHistoryForm(ref historyForm);
            }
        }
        #endregion

        #region ColorForm
        private IColorPicker colorsForm = null;
        public IColorPicker ColorsForm
        {
            get
            {
                if (colorsForm == null)
                {
                    colorsForm = new ColorsFormControl();
                    InitializeColorsForm(ref colorsForm);
                }
                return this.colorsForm;
            }

            set
            {
                this.colorsForm = value;
                InitializeColorsForm(ref colorsForm);
            }
        }
        
        #endregion

        #region MDI forms
        private Dictionary<string, Form> ChildForms = new Dictionary<string, Form>();

        public Form GetChildForm(string FormTitle)
        {
            if (ChildForms.ContainsKey(FormTitle))
                return ChildForms[FormTitle];
            else
                return null;

        }

        public void ResetFloatingForms()
        {
            //ResetFloatingForm(AppEnvironment.Environment.ToolsForm);
            //ResetFloatingForm(AppEnvironment.Environment.HistoryForm);
            //ResetFloatingForm(AppEnvironment.Environment.LayerForm);
            //ResetFloatingForm(AppEnvironment.Environment.ColorsForm);
        }

        public void ShowToolsbars()
        {
            // RequestFormCreation(mainToolBarForm);
            // RequestFormCreation(colorsForm);
        }

        public void RequestFormShow(Form ShowForm)
        {
            if (MakeNewForm != null) MakeNewForm(this, ShowForm);
        }

        public Form RequestFormCreation(UserControl newControl)
        {
            Form NewForm;
            if (MakeFormFromUsercontrol != null)
                NewForm = MakeFormFromUsercontrol(newControl);
            else
            {
                throw new Exception("You must have a form server.");
            }
            return NewForm;

        }

        public Form ShowForm(string FormTitle, Form NewForm)
        {
            Form showForm = NewForm;
            if (ChildForms.ContainsKey(FormTitle) == true)
            {
                showForm = ChildForms[FormTitle];

                if (showForm.Visible != true)
                    if (MakeNewForm != null) MakeNewForm(this, showForm);
                return showForm;
            }
            else
            {
                ChildForms.Add(FormTitle, NewForm);
                NewForm.FormClosed += new FormClosedEventHandler(NewForm_FormClosed);
                if (MakeNewForm != null) MakeNewForm(this, showForm);
                return NewForm;
            }
        }

        public Form ShowForm(string FormTitle, UserControl NewControl)
        {

            Form newForm = RequestFormCreation(NewControl);
            ShowForm(FormTitle, newForm);
            return newForm;
        }

        void NewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form sForm = (Form)sender;
            if (ChildForms.ContainsValue(sForm) == true)
            {
                foreach (KeyValuePair<string, Form> kvp in ChildForms)
                {
                    if (kvp.Value == sForm)
                    {
                        ChildForms.Remove(kvp.Key);
                        return;
                    }
                }

            }
        }

        private bool _MDIWorkspace = false;

        public bool MDIworkspace
        {
            get { return _MDIWorkspace; }
            set { _MDIWorkspace = value; }

        }

        private List<IControlHoldingForm> DocumentWorkspaceForms = new List<IControlHoldingForm>();

        public Form FindHostMDIForm(DocumentWorkspace dw)
        {

            foreach (IControlHoldingForm f in DocumentWorkspaceForms)
            {

                if (f.MainUserControl == dw)
                    return f.HostForm;

            }
            return null;
        }

        void MDI_DocumentWorkspace_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                DocumentManager.Manager.RemoveDocumentWorkspace((DocumentWorkspace)((IControlHoldingForm)sender).MainUserControl);
            }
            catch { }
            try
            {
                DocumentWorkspaceForms.Remove((IControlHoldingForm)sender);
            }
            catch
            { }
        }

        public void FormActivated(object sender)
        {
            MDI_DocumentWorkspace_Activated(sender, EventArgs.Empty);
        }

        void MDI_DocumentWorkspace_Activated(object sender, EventArgs e)
        {
            IControlHoldingForm mcf = (IControlHoldingForm)sender;
            if ((DocumentWorkspace)mcf.MainUserControl != DocumentManager.Manager.ActiveDocumentWorkspace)
            {
                DocumentWorkspace dw = (DocumentWorkspace)mcf.MainUserControl;
                if (dw != null)
                    DocumentManager.Manager.ActiveDocumentWorkspace = (DocumentWorkspace)mcf.MainUserControl;
            }
        }

        void MDI_DocumentWorkspace_GotFocus(object sender, EventArgs e)
        {
            MDI_DocumentWorkspace_Activated(sender, e);
        }

        void MDI_DocumentWorkspace_MdiChildActivate(object sender, EventArgs e)
        {
            MDI_DocumentWorkspace_Activated(sender, e);
        }

        #endregion
    }
}
