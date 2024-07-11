/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Forms;
using SciImage.PaintForms;
using SciImage.PaintForms.UserControls;
using SciImage.PaintForms.UserControls.Buttons;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;
using SciImage.Plugins.Effects.IEffects;
using SciImage.Plugins.Tools;
using SciImage.SciResources;
using SciImage.SystemLayer.Base.PropertySystem;
using SciImage.SystemLayer.System;
using ThreadPool = SciImage.SystemLayer.Threading.ThreadPool;

namespace SciImage.Plugins.Effects
{


    public abstract class Effect : IPluginMenuItem
    {
        #region run params

        public DialogResult dialogResult { get; private set; }
        private System.Windows.Forms.Timer invalidateTimer;
        private const int tilesPerCpu = 75;
        private int renderingThreadCount = Math.Max(2, Processor.LogicalCpuCount);
        private int TrueNumberofTiles = 75;
        private const int effectRefreshInterval = 15;
        private SciRegion[] progressRegions;
        private int progressRegionsStartIndex;

        protected DocumentWorkspace ActiveDocumentWorkspace
        {
            get
            {
                return DocumentManager.Manager.ActiveDocumentWorkspace;
            }
        }

        private void RunUnConfigurableEffect(DocumentWorkspace activeDW, BitmapLayer layer, SciRegion selectedRegion, Exception exception)
        {
            //Surface copy = activeDW.BorrowScratchSurface(this.GetType() + ".RunEffect() using scratch RGB32_Surface for non-configurable rendering");
            Surface copy = null;//=(Surface)  blayer.Clone();
            try
            {
                using (new WaitCursorChanger())
                {
                    copy = (Surface)layer.Surface.Clone();
                    //copy.CopySurface(blayer.Surface);
                }

                EffectEnvironmentParameters eep = new EffectEnvironmentParameters(
                    ToolEnvironment.Environment.PrimaryColor,
                    ToolEnvironment.Environment.SecondaryColor,
                    ToolEnvironment.Environment.PenInfo.Width,
                    selectedRegion,
                    copy);

                EnvironmentParameters = eep;

                DoEffect(null, selectedRegion, selectedRegion, copy, out exception);
            }

            finally
            {
                // activeDW.ReturnScratchSurface(copy);
            }
        }

        private void RunConfigurableEffect(DocumentWorkspace activeDW, BitmapLayer layer, SciRegion selectedRegion, out Exception exception, ref EffectConfigToken newLastToken, ref bool resetDirtyValue)
        {
            SciRegion previewRegion = (SciRegion)selectedRegion.Clone();
            previewRegion.Intersect(RectangleF.Inflate(activeDW.VisibleDocumentRectangleF, 1, 1));

            Surface originalSurface = activeDW.BorrowScratchSurface(this.GetType() + ".RunEffect() using scratch RGB32_Surface for rendering during configuration");

            try
            {
                using (new WaitCursorChanger())
                {
                    originalSurface.CopySurface(layer.Surface);
                }

                EffectEnvironmentParameters eep = new EffectEnvironmentParameters(
                    ToolEnvironment.Environment.PrimaryColor,
                    ToolEnvironment.Environment.SecondaryColor,
                    ToolEnvironment.Environment.PenInfo.Width,
                    selectedRegion,
                    originalSurface);

                EnvironmentParameters = eep;
                CreatePropertyCollection();
                //
                IDisposable resumeTUFn = DocumentManager.Manager.SuspendThumbnailUpdates();
                //
                Exception t = null;
                using (IEffectConfigDialog configDialog = CreateConfigDialog())
                {
                    SourceSurface = originalSurface;
                    Selection = selectedRegion;
                    BackgroundEffectRenderer ber = null;

                    EventHandler eh =
                        delegate (object sender, EventArgs e)
                        {
                            IEffectConfigDialog ecf = (IEffectConfigDialog)sender;

                            if (ber != null)
                            {
                                AppEnvironment.Environment.ResetProgressStatusBarAsync();
                                try
                                {
                                    ber.Start();
                                }

                                catch (Exception ex)
                                {
                                    t = ex;
                                    ecf.Close();
                                }
                                //exception = t;
                            }
                        };

                    exception = t;
                    EffectTokenChanged += eh;


                    int NumTiles = tilesPerCpu * renderingThreadCount;

                    if (CheckForEffectFlags(EffectFlags.RenderInOnePiece) == true)
                    {
                        NumTiles = 1;
                        if (renderingThreadCount > NumTiles) renderingThreadCount = NumTiles;

                    }
                    TrueNumberofTiles = NumTiles;
                    ber = new BackgroundEffectRenderer(
                        this,
                        this.EffectToken,
                        new RenderArgs(layer.Surface),
                        new RenderArgs(originalSurface),
                        previewRegion,
                        NumTiles,
                        renderingThreadCount);

                    ber.RenderedTile += new RenderedTileEventHandler(RenderedTileHandler);
                    ber.StartingRendering += new EventHandler(StartingRenderingHandler);
                    ber.FinishedRendering += new EventHandler(FinishedRenderingHandler);

                    invalidateTimer.Enabled = true;



                    try
                    {
                        this.dialogResult = Utility.ShowDialog(configDialog, FormsManager.BaseForm);
                    }
                    catch (Exception ex)
                    {
                        this.dialogResult = DialogResult.None;
                        exception = ex;
                    }

                    invalidateTimer.Enabled = false;

                    this.InvalidateTimer_Tick(invalidateTimer, EventArgs.Empty);

                    using (new WaitCursorChanger())
                    {
                        try
                        {
                            ber.Abort();
                            ber.Join();
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }

                        ber.Dispose();
                        ber = null;

                        if (this.dialogResult != DialogResult.OK)
                        {
                            ((BitmapLayer)activeDW.ActiveLayer).Surface.CopySurface(originalSurface);
                            activeDW.ActiveLayer.Invalidate();
                        }

                        EffectTokenChanged -= eh;
                        configDialog.Hide();
                        FormsManager.BaseForm.Update();
                        previewRegion.Dispose();
                    }

                    //
                    if (resumeTUFn != null)//????
                        resumeTUFn.Dispose();
                    resumeTUFn = null;
                    //

                    if (this.dialogResult == DialogResult.OK)
                    {
                        SciRegion remainingToRender = selectedRegion.Clone();
                        SciRegion alreadyRendered = SciRegion.CreateEmpty();

                        for (int i = 0; i < this.progressRegions.Length; ++i)
                        {
                            if (this.progressRegions[i] == null)
                            {
                                break;
                            }
                            else
                            {
                                remainingToRender.Exclude(this.progressRegions[i]);
                                alreadyRendered.Union(this.progressRegions[i]);
                            }
                        }

                        activeDW.ActiveLayer.Invalidate(alreadyRendered);
                        newLastToken = (EffectConfigToken)EffectToken.Clone();

                        AppEnvironment.Environment.ResetProgressStatusBar();
                        DoEffect(newLastToken, selectedRegion, remainingToRender, originalSurface, out exception);
                    }
                    else // if (dr == DialogResult.Cancel)
                    {
                        using (new WaitCursorChanger())
                        {
                            activeDW.ActiveLayer.Invalidate();
                            Utility.GCFullCollect();
                        }

                        resetDirtyValue = true;
                        return;
                    }
                }
            }

            catch (Exception ex)
            {
                exception = ex;
            }

            finally
            {
                activeDW.ReturnScratchSurface(originalSurface);
            }
        }

        private void RenderedTileHandler(object sender, RenderedTileEventArgs e)
        {
            if (this.progressRegions[e.TileNumber] == null)
            {
                this.progressRegions[e.TileNumber] = e.RenderedRegion;
            }
        }

        private void FinishedRenderingHandler(object sender, EventArgs e)
        {
            if (FormsManager.BaseForm.InvokeRequired)
            {
                FormsManager.BaseForm.BeginInvoke(new EventHandler(FinishedRenderingHandler), new object[] { sender, e });
            }
            else
            {
                ActiveDocumentWorkspace.EnableOutlineAnimation = true;
            }
        }

        private void StartingRenderingHandler(object sender, EventArgs e)
        {

            AppEnvironment.Environment.ResetProgressStatusBarAsync();
            ActiveDocumentWorkspace.EnableOutlineAnimation = false;

            if (this.progressRegions == null)
            {
                this.progressRegions = new SciRegion[TrueNumberofTiles];
            }

            lock (this.progressRegions)
            {
                for (int i = 0; i < progressRegions.Length; ++i)
                {
                    progressRegions[i] = null;
                }

                this.progressRegionsStartIndex = 0;
            }
        }

        /// <summary>
        /// Run the effect, assuming the token describes everything
        /// </summary>
        /// <param name="token"></param>
        /// <param name="selectedRegion"></param>
        /// <param name="regionToRender"></param>
        /// <param name="originalSurface"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool DoEffect(EffectConfigToken token, SciRegion selectedRegion,
            SciRegion regionToRender, Surface originalSurface, out Exception exception)
        {
            this.dialogResult = DialogResult.None;
            exception = null;
            bool oldDirtyValue = ActiveDocumentWorkspace.Document.Dirty;
            bool resetDirtyValue = false;

            bool returnVal = false;
            ActiveDocumentWorkspace.EnableOutlineAnimation = false;

            try
            {
                using (ProgressDialog aed = new ProgressDialog())
                {
                    if (Image != null)
                    {
                        aed.Icon = Utility.ImageToIcon(Image, Utility.TransparentKey);
                    }

                    aed.Opacity = 0.9;
                    aed.Value = 0;
                    aed.Text = Name;
                    aed.Description = string.Format(SciResources.SciResources.GetString("Effects.ApplyingDialog.Description"), Name);

                    invalidateTimer.Enabled = true;

                    using (new WaitCursorChanger())
                    {
                        HistoryMemento ha = null;
                        DialogResult result = DialogResult.None;


                        AppEnvironment.Environment.ResetProgressStatusBar();

                         
                            FormsManager.Manager.SuspendLayerPreviewUpdates();

                        try
                        {
                            ManualResetEvent saveEvent = new ManualResetEvent(false);
                            BitmapHistoryMemento bha = null;

                            // perf bug #1445: save this data in a background thread
                            SciRegion selectedRegionCopy = selectedRegion.Clone();
                            ThreadPool.Global.QueueUserWorkItem(
                                delegate (object context)
                                {
                                    try
                                    {
                                        ImageResource image;

                                        if (Image == null)
                                        {
                                            image = null;
                                        }
                                        else
                                        {
                                            image = ImageResource.FromImage(Image);
                                        }

                                        bha = new BitmapHistoryMemento(Name, image, ActiveDocumentWorkspace,
                                            ActiveDocumentWorkspace.ActiveLayerIndex, selectedRegionCopy, originalSurface);
                                    }

                                    finally
                                    {
                                        saveEvent.Set();
                                        selectedRegionCopy.Dispose();
                                        selectedRegionCopy = null;
                                    }
                                });

                            int NumTiles = tilesPerCpu * renderingThreadCount;

                            if (this.CheckForEffectFlags(EffectFlags.RenderInOnePiece) == true)
                            {
                                NumTiles = 1;
                                if (renderingThreadCount > NumTiles) renderingThreadCount = NumTiles;

                            }
                            TrueNumberofTiles = NumTiles;
                            BackgroundEffectRenderer ber = new BackgroundEffectRenderer(
                                this,
                                token,
                                new RenderArgs(((BitmapLayer)ActiveDocumentWorkspace.ActiveLayer).Surface),
                                new RenderArgs(originalSurface),
                                regionToRender,
                                NumTiles,
                                renderingThreadCount);

                            ber.RenderedTile += new RenderedTileEventHandler(aed.RenderedTileHandler);
                            ber.RenderedTile += new RenderedTileEventHandler(RenderedTileHandler);
                            ber.StartingRendering += new EventHandler(StartingRenderingHandler);
                            ber.FinishedRendering += new EventHandler(aed.FinishedRenderingHandler);
                            ber.FinishedRendering += new EventHandler(FinishedRenderingHandler);
                            ber.Start();

                            result = Utility.ShowDialog(aed, FormsManager.BaseForm);

                            if (result == DialogResult.Cancel)
                            {
                                resetDirtyValue = true;

                                using (new WaitCursorChanger())
                                {
                                    try
                                    {
                                        ber.Abort();
                                        ber.Join();
                                    }

                                    catch (Exception ex)
                                    {
                                        exception = ex;
                                    }

                                    ((BitmapLayer)ActiveDocumentWorkspace.ActiveLayer).Surface.CopySurface(originalSurface);
                                }
                            }

                            invalidateTimer.Enabled = false;

                            try
                            {
                                ber.Join();
                            }

                            catch (Exception ex)
                            {
                                exception = ex;
                            }

                            ber.Dispose();

                            saveEvent.WaitOne();
                            saveEvent.Close();
                            saveEvent = null;

                            ha = bha;
                        }

                        catch (Exception)
                        {
                            using (new WaitCursorChanger())
                            {
                                ((BitmapLayer)ActiveDocumentWorkspace.ActiveLayer).Surface.CopySurface(originalSurface);
                                ha = null;
                            }
                        }

                        finally
                        {
                             
                                FormsManager.Manager.ResumeLayerPreviewUpdates();
                        }

                        using (SciRegion simplifiedRenderRegion = Utility.SimplifyAndInflateRegion(selectedRegion))
                        {
                            using (new WaitCursorChanger())
                            {
                                ActiveDocumentWorkspace.ActiveLayer.Invalidate(simplifiedRenderRegion);
                            }
                        }

                        using (new WaitCursorChanger())
                        {
                            if (result == DialogResult.OK)
                            {
                                if (ha != null)
                                {
                                    ActiveDocumentWorkspace.History.PushNewMemento(ha);
                                }

                                FormsManager.BaseForm.Update();
                                returnVal = true;
                            }
                            else
                            {
                                Utility.GCFullCollect();
                            }
                        }
                    } // using
                } // using
            }

            finally
            {
                ActiveDocumentWorkspace.EnableOutlineAnimation = true;

                if (resetDirtyValue)
                {
                    ActiveDocumentWorkspace.Document.Dirty = oldDirtyValue;
                }
            }

            AppEnvironment.Environment.EraseProgressStatusBarAsync();
            return returnVal;
        }

        private void InvalidateTimer_Tick(object sender, System.EventArgs e)
        {
            if (FormsManager.BaseForm.FindForm().WindowState == FormWindowState.Minimized)
            {
                return;
            }

            if (this.progressRegions == null)
            {
                return;
            }

            lock (this.progressRegions)
            {
                int min = this.progressRegionsStartIndex;
                int max;

                for (max = min; max < progressRegions.Length; ++max)
                {
                    if (this.progressRegions[max] == null)
                    {
                        break;
                    }
                }

                if (min != max)
                {
                    using (SciRegion updateRegion = SciRegion.CreateEmpty())
                    {
                        for (int i = min; i < max; ++i)
                        {
                            updateRegion.Union(this.progressRegions[i]);
                        }

                        using (SciRegion simplified = Utility.SimplifyAndInflateRegion(updateRegion))
                        {
                            ActiveDocumentWorkspace.ActiveLayer.Invalidate(simplified);
                        }

                        this.progressRegionsStartIndex = max;
                    }
                }

                double progress = 100.0 * (double)max / (double)progressRegions.Length;

                AppEnvironment.Environment.SetProgressStatusBar(progress);
            }
        }

        /// <summary>
        /// Run with dialog to allow parameter changes
        /// </summary>
        /// <param name="appWorkspace"></param>
        /// <returns></returns>
        public EffectConfigToken RunEffect()
        {
            this.dialogResult = DialogResult.None;

            this.invalidateTimer = new System.Windows.Forms.Timer();
            this.invalidateTimer.Enabled = false;
            this.invalidateTimer.Tick += InvalidateTimer_Tick;
            this.invalidateTimer.Interval = effectRefreshInterval;

            bool oldDirtyValue = ActiveDocumentWorkspace.Document.Dirty;
            bool resetDirtyValue = false;

            FormsManager.BaseForm.Update(); // make sure the window is done 'closing'

            AppEnvironment.Environment.ResetProgressStatusBar();


            SciRegion selectedRegion;

            if (ActiveDocumentWorkspace.Selection.IsEmpty)
            {
                selectedRegion = new SciRegion(ActiveDocumentWorkspace.Document.Bounds);
            }
            else
            {
                selectedRegion = ActiveDocumentWorkspace.Selection.CreateRegion();
            }

            Exception exception = null;
            BitmapLayer layer = (BitmapLayer)ActiveDocumentWorkspace.ActiveLayer;
            EffectConfigToken newLastToken = null;
            using (new PushNullToolMode(ActiveDocumentWorkspace))
            {
                try
                {
                    if (!(CheckForEffectFlags(EffectFlags.Configurable)))
                    {
                        RunUnConfigurableEffect(ActiveDocumentWorkspace, layer, selectedRegion, exception);
                    }
                    else
                    {
                        RunConfigurableEffect(ActiveDocumentWorkspace, layer, selectedRegion, out exception, ref newLastToken, ref resetDirtyValue);
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    selectedRegion.Dispose();

                    {
                        AppEnvironment.Environment.ResetProgressStatusBar();
                        AppEnvironment.Environment.EraseProgressStatusBar();
                    }
                    ActiveDocumentWorkspace.EnableOutlineAnimation = true;

                    if (this.progressRegions != null)
                    {
                        for (int i = 0; i < this.progressRegions.Length; ++i)
                        {
                            if (this.progressRegions[i] != null)
                            {
                                this.progressRegions[i].Dispose();
                                this.progressRegions[i] = null;
                            }
                        }
                    }

                    if (resetDirtyValue)
                    {
                        ActiveDocumentWorkspace.Document.Dirty = oldDirtyValue;
                    }

                    if (exception != null)
                    {
                        HandleEffectException(exception);
                    }
                }
                return newLastToken;
            }
        }

        public void HandleEffectException(Exception ex)
        {
            try
            {

                {
                    AppEnvironment.Environment.ResetProgressStatusBar();
                    AppEnvironment.Environment.EraseProgressStatusBar();
                }
            }

            catch (Exception)
            {
            }

            // Figure out if it's a built-in effect, or a plug-in
            bool builtIn = IsBuiltInEffect();

            if (builtIn)
            {
                // For built-in effects, tear down Paint.NET which will result in a crash log
                throw new ApplicationException("Effect threw an exception", ex);
            }
            else
            {
                Icon formIcon = Utility.ImageToIcon(SciResources.SciResources.GetImageResource("Icons.BugWarning.png").Reference);

                string formTitle = SciResources.SciResources.GetString("Effect.PluginErrorDialog.Title");

                Image taskImage = null;

                string introText = SciResources.SciResources.GetString("Effect.PluginErrorDialog.IntroText");

                TaskButton restartTB = new TaskButton(
                    SciResources.SciResources.GetImageResource("Icons.RightArrowBlue.png").Reference,
                    SciResources.SciResources.GetString("Effect.PluginErrorDialog.RestartTB.ActionText"),
                    SciResources.SciResources.GetString("Effect.PluginErrorDialog.RestartTB.ExplanationText"));

                TaskButton doNotRestartTB = new TaskButton(
                    SciResources.SciResources.GetImageResource("Icons.WarningIcon.png").Reference,
                    SciResources.SciResources.GetString("Effect.PluginErrorDialog.DoNotRestartTB.ActionText"),
                    SciResources.SciResources.GetString("Effect.PluginErrorDialog.DoNotRestartTB.ExplanationText"));

                string auxButtonText = SciResources.SciResources.GetString("Effect.PluginErrorDialog.AuxButton1.Text");

                EventHandler auxButtonClickHandler =
                    delegate (object sender, EventArgs e)
                    {
                        using (SciBaseForm textBoxForm = new SciBaseForm())
                        {
                            textBoxForm.Name = "EffectCrash";

                            TextBox exceptionBox = new TextBox();

                            textBoxForm.Icon = Utility.ImageToIcon(SciResources.SciResources.GetImageResource("Icons.WarningIcon.png").Reference);
                            textBoxForm.Text = SciResources.SciResources.GetString("Effect.PluginErrorDialog.Title");

                            exceptionBox.Dock = DockStyle.Fill;
                            exceptionBox.ReadOnly = true;
                            exceptionBox.Multiline = true;

                            string exceptionText = EffectFactory.GetLocalizedEffectErrorMessage(this.GetType().Assembly, this.GetType(), ex);

                            exceptionBox.Font = new Font(FontFamily.GenericMonospace, exceptionBox.Font.Size);
                            exceptionBox.Text = exceptionText;
                            exceptionBox.ScrollBars = ScrollBars.Vertical;

                            textBoxForm.StartPosition = FormStartPosition.CenterParent;
                            textBoxForm.ShowInTaskbar = false;
                            textBoxForm.MinimizeBox = false;
                            textBoxForm.Controls.Add(exceptionBox);
                            textBoxForm.Width = 700;

                            textBoxForm.ShowDialog();
                        }
                    };

                TaskButton clickedTB = TaskDialog.Show(
                    FormsManager.BaseForm,
                    formIcon,
                    formTitle,
                    taskImage,
                    true,
                    introText,
                    new TaskButton[] { restartTB, doNotRestartTB },
                    restartTB,
                    doNotRestartTB,
                    TaskDialog.DefaultPixelWidth96Dpi * 2,
                    auxButtonText,
                    auxButtonClickHandler);

                if (clickedTB == restartTB)
                {
                    // Next, apply restart logic
                    ActionFactory.PerformAction("CloseAllWorkspacesAction");
                    Shell.RestartApplication();
                }
            }
        }
        #endregion


        public bool IsBuiltInEffect()
        {
            if (this == null)
            {
                return true;
            }

            Type effectType = this.GetType();
            Type effectBaseType = typeof(Effect);

            // Built-in effects only live in SciImage.Effects.dll

            if (effectType.Assembly == effectBaseType.Assembly)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Surface SourceSurface { get; set; }
        public SciRegion Selection { get; set; }
        public event System.EventHandler EffectTokenChanged;

        protected EffectEnvironmentParameters envParams;
        protected EffectFlags effectFlags;
        private bool setRenderInfoCalled = false;
        protected string _Name;
        protected Image _Image;
        protected string _SubMenuName;
        protected string _MainMenuName;
        protected double _SuggestedMenuOrder = 0;

        public void PropertiesChanged(object sender, EventArgs e)
        {
            if (EffectTokenChanged != null)
                EffectTokenChanged(sender, e);
        }

        protected bool SetRenderInfoCalled
        {
            get
            {
                return this.setRenderInfoCalled;
            }
        }

        public EffectEnvironmentParameters EnvironmentParameters
        {
            get
            {
                return this.envParams;
            }

            set
            {
                this.envParams = value;
            }
        }

        public EffectFlags EffectFlags
        {
            get
            {
                return this.effectFlags;
            }
        }

        public bool CheckForEffectFlags(EffectFlags flags)
        {
            return (EffectFlags & flags) == flags;
        }

        #region Menu
        public string SubMenuName
        {
            get
            {
                return this._SubMenuName;
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
        }

        public Image Image
        {
            get
            {
                return this._Image;
            }
        }

        public string MainMenuName
        {
            get
            {
                return _MainMenuName;
            }
        }

        public double MenuOrder
        {
            get
            {
                return _SuggestedMenuOrder;
            }
        }



        public virtual System.Windows.Forms.Keys ShortCutKeys
        {
            get { return Keys.F9; }
        }
        #endregion

        public void SetRenderInfo(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.setRenderInfoCalled = true;
            OnSetRenderInfo(parameters, dstArgs, srcArgs);
        }

        protected virtual void OnSetRenderInfo(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs)
        {
        }

        #region Render

        /// <summary>
        /// Performs the effect's rendering. The source is to be treated as read-only,
        /// and only the destination pixels within the given rectangle-of-interest are
        /// to be written to. However, in order to compute the destination pixels,
        /// any pixels from the source may be utilized.
        /// </summary>
        /// <param name="parameters">The parameters to the effect. If IsConfigurable is true, then this must not be null.</param>
        /// <param name="dstArgs">Describes the destination RGB32_Surface.</param>
        /// <param name="srcArgs">Describes the source RGB32_Surface.</param>
        /// <param name="rois">The list of rectangles that describes the region of interest.</param>
        /// <param name="startIndex">The index within roi to start enumerating from.</param>
        /// <param name="length">The number of rectangles to enumerate from roi.</param>
        public abstract void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length);

        public void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois)
        {
            Render(parameters, dstArgs, srcArgs, rois, 0, rois.Length);
        }

        public void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, SciRegion roi)
        {
            Rectangle[] scans = roi.GetRegionScansReadOnlyInt();
            Render(parameters, dstArgs, srcArgs, scans, 0, scans.Length);
        }

        /// <summary>
        /// This is a helper function. It allows you to render an effect "in place."
        /// That is, you don't need both a destination and a source Surface.
        /// </summary>
        public void RenderInPlace(RenderArgs srcAndDstArgs, SciRegion roi)
        {
            using (Surface renderSurface = new Surface(srcAndDstArgs.Surface.Size, srcAndDstArgs.Surface.ColorPixelBase))
            {
                using (RenderArgs renderArgs = new RenderArgs(renderSurface))
                {
                    Rectangle[] scans = roi.GetRegionScansReadOnlyInt();
                    Render(null, renderArgs, srcAndDstArgs, scans);
                    srcAndDstArgs.Surface.CopySurface(renderSurface, roi);
                }
            }
        }

        public void RenderInPlace(RenderArgs srcAndDstArgs, Rectangle roi)
        {
            using (SciRegion region = new SciRegion(roi))
            {
                RenderInPlace(srcAndDstArgs, region);
            }
        }
        #endregion

        #region Constructor
        public Effect(string name, Image image, string MenuName)
            : this(name, image, EffectFlags.None, MenuName)
        {
        }

        public Effect(string name, Image image, EffectFlags flags, string MenuName)
            : this(name, image, null, flags, MenuName)
        {
        }



        public Effect(string name, Image image, string subMenuName, string MenuName)
            : this(name, image, subMenuName, EffectFlags.None, MenuName)
        {
        }



        public Effect(string name, Image image, string subMenuName, EffectFlags effectFlags, string MenuName)
        {
            this._Name = name;
            this._Image = image;
            this._SubMenuName = subMenuName;
            this._MainMenuName = MenuName;
            this.effectFlags = effectFlags;
            this.envParams = EffectEnvironmentParameters.DefaultParameters;
        }
        #endregion

        protected abstract PropertyCollection OnCreatePropertyCollection();

        public PropertyCollection CreatePropertyCollection()
        {
            var t = OnCreatePropertyCollection();
            CreateInitialToken(t);
            return t;
        }

        protected virtual UserControl OnCreateConfigUI()
        {
            return new UserControl();
        }

        public Icon GetConfigDialogIcon()
        {
            Image image = this.Image;

            Icon icon = null;

            if (image != null)
            {
                icon = Utility.ImageToIcon(image);
            }

            return icon;
        }

        public EffectConfigToken EffectToken
        {
            get; set;
        }

        protected void CreateInitialToken(PropertyCollection props1)
        {
            EffectToken = new EffectConfigToken(props1);
        }

        protected abstract IEffectConfigDialog OnCreateConfigDialog();


        public IEffectConfigDialog CreateConfigDialog()
        {
            return OnCreateConfigDialog();
        }

        public void ClearMemory()
        {
            SourceSurface = null;
            Selection = null;
        }
    }
}
