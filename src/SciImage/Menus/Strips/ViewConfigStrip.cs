/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.Core;
using SciImage.SystemLayer;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SciImage.PaintForms.UserControls.Rulers;
using SciImage.SciResources;
using SciImage.SystemLayer.Forms;
using SciImage.SystemLayer.System;

namespace SciImage.Menus.Strips
{
    public sealed class ViewConfigStrip
        : ToolStripEx
    {
        private string windowText;
        private string percentageFormat;
        private ToolStripSeparator separator0;
        private ScaleFactor scaleFactor;
        private ToolStripButton zoomOutButton;
        private ToolStripButton zoomInButton;
        private ToolStripComboBox zoomComboBox;
        private ToolStripSeparator separator1;
        private ToolStripButton gridButton;
        private ToolStripButton rulersButton;
        private ToolStripLabel unitsLabel;
        private UnitsComboBoxStrip unitsComboBox;

        private int scaleFactorRecursionDepth = 0;
        private int suspendEvents = 0;
        private int ignoreZoomChanges = 0;

        public void SuspendEvents()
        {
            ++this.suspendEvents;
        }

        public void ResumeEvents()
        {
            --this.suspendEvents;
        }

        public void BeginZoomChanges()
        {
            ++this.ignoreZoomChanges;
        }

        public void EndZoomChanges()
        {
            --this.ignoreZoomChanges;
        }

        private ZoomBasis zoomBasis;
        public ZoomBasis ZoomBasis
        {
            get
            {
                return this.zoomBasis;
            }

            set
            {
                if (this.zoomBasis != value)
                {
                    this.zoomBasis = value;
                    SetZoomText();
                    OnZoomBasisChanged();
                }
            }
        }

        public bool DrawGrid
        {
            get
            {
                return gridButton.Checked;
            }

            set
            {
                if (gridButton.Checked != value)
                {
                    gridButton.Checked = value;
                    this.OnDrawGridChanged();
                }
            }
        }

        public bool RulersEnabled
        {
            get
            {
                return rulersButton.Checked;
            }

            set
            {
                if (rulersButton.Checked != value)
                {
                    rulersButton.Checked = value;
                    this.OnRulersEnabledChanged();
                }
            }
        }

        public MeasurementUnit Units
        {
            get
            {
                return this.unitsComboBox.Units;
            }

            set
            {
                this.unitsComboBox.Units = value;
            }
        }

        public ScaleFactor ScaleFactor
        {
            get
            {
                return this.scaleFactor;
            }

            set
            {
                if (this.scaleFactor.Ratio != value.Ratio)
                {
                    this.scaleFactor = value;
                    ++this.scaleFactorRecursionDepth;

                    // Prevent infinite recursion that was reported by one person.
                    // This may cause the scale factor to settle on a less than
                    // desirable value, but this is obviously more desirable than
                    // a StackOverflow crash.
                    if (this.scaleFactorRecursionDepth < 100)
                    {
                        SetZoomText();
                        OnZoomScaleChanged();
                    }

                    --this.scaleFactorRecursionDepth;
                }
            }
        }

        public ViewConfigStrip()
        {
            this.SuspendLayout();
            InitializeComponent();

            this.windowText = EnumLocalizer.EnumValueToLocalizedName(typeof(ZoomBasis), ZoomBasis.FitToWindow);
            this.percentageFormat = "{0}%";

            double[] zoomValues = ScaleFactor.PresetValues;

            this.zoomComboBox.ComboBox.SuspendLayout();

            string percent100 = null; // ScaleFactor.PresetValues guarantees that 1.0, or "100%" is in the list, but the compiler can't be shown this so we must assign a value here
            for (int i = zoomValues.Length - 1; i >= 0; --i)
            {
                string zoomValueString = (zoomValues[i] * 100.0).ToString();
                string zoomItemString = string.Format(this.percentageFormat, zoomValueString);

                if (zoomValues[i] == 1.0)
                {
                    percent100 = zoomItemString;
                }

                this.zoomComboBox.Items.Add(zoomItemString);
            }

            this.zoomComboBox.Items.Add(this.windowText);
            this.zoomComboBox.ComboBox.ResumeLayout(false);
            this.zoomComboBox.Size = new Size(UI.ScaleWidth(this.zoomComboBox.Width), zoomComboBox.Height);

            this.unitsLabel.Text = "Image Units:";

            this.zoomComboBox.Text = percent100;
            this.ScaleFactor = ScaleFactor.OneToOne;

            this.zoomOutButton.Image = SciResources.SciResources.GetImageResource("Icons.MenuViewZoomOutIcon.png").Reference;
            this.zoomInButton.Image = SciResources.SciResources.GetImageResource("Icons.MenuViewZoomInIcon.png").Reference;
            this.gridButton.Image = SciResources.SciResources.GetImageResource("Icons.MenuViewGridIcon.png").Reference;
            this.rulersButton.Image = SciResources.SciResources.GetImageResource("Icons.MenuViewRulersIcon.png").Reference;

            this.zoomOutButton.ToolTipText = "Zoom out";
            this.zoomInButton.ToolTipText = "Zoom in";
            this.gridButton.ToolTipText = "Display Grid";
            this.rulersButton.ToolTipText = "Rulers";

            this.unitsComboBox.Size = new Size(UI.ScaleWidth(this.unitsComboBox.Width), unitsComboBox.Height);

            this.zoomBasis = ZoomBasis.ScaleFactor;
            ScaleFactor = ScaleFactor.OneToOne;


            //get messages from the other componenets
            AppEnvironment.Environment.DrawGridChanged += ViewConfigStrip_DrawGridChanged;
            AppEnvironment.Environment.UnitsChanged += ViewConfigStrip_UnitsChanged;
            AppEnvironment.Environment.RulersEnabledChanged += ViewConfigStrip_RulersEnabledChanged;

            DocumentManager.Manager.OnZoomBasis += DocumentManager_OnZoomBasis;
            DocumentManager.Manager.OnZoomToScale += DocumentManager_OnZoomToScale;
            this.ResumeLayout(false);
        }

        private void DocumentManager_OnZoomToScale(object sender, EventArgs e)
        {
            var activeDoc = ((DocumentManager)sender).ActiveDocumentWorkspace;
            if (activeDoc != null)
            {
                var scale = activeDoc.ScaleFactor;
                if (ScaleFactor != scale)
                    ScaleFactor = scale;
            }
        }

        private void DocumentManager_OnZoomBasis(object sender, EventArgs e)
        {
            var activeDoc = ((DocumentManager)sender).ActiveDocumentWorkspace;
            if (activeDoc != null)
            {
                var zoomBias = activeDoc.ZoomBasis;
                if (ZoomBasis != zoomBias)
                    ZoomBasis = zoomBias;
            }
        }

        private void ViewConfigStrip_RulersEnabledChanged(object sender, EventArgs e)
        {
            RulersEnabled = ((AppEnvironment)sender).RulersEnabled;
        }

        private void ViewConfigStrip_UnitsChanged(object sender, EventArgs e)
        {
            Units = ((AppEnvironment)sender).Units;
        }

        private void ViewConfigStrip_DrawGridChanged(object sender, EventArgs e)
        {
            DrawGrid = ((AppEnvironment)sender).DrawGrid;
        }

        private void InitializeComponent()
        {
            this.separator0 = new ToolStripSeparator();
            this.zoomOutButton = new ToolStripButton();
            this.zoomComboBox = new ToolStripComboBox();
            this.zoomInButton = new ToolStripButton();
            this.separator1 = new ToolStripSeparator();
            this.gridButton = new ToolStripButton();
            this.rulersButton = new ToolStripButton();
            this.unitsLabel = new ToolStripLabel();
            this.unitsComboBox = new UnitsComboBoxStrip();
            this.SuspendLayout();
            //
            // separator0
            //
            this.separator0.Name = "separator0";
            //
            // zoomComboBox
            //
            this.zoomComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ZoomComboBox_KeyPress);
            this.zoomComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.ZoomComboBox_Validating);
            this.zoomComboBox.SelectedIndexChanged += new System.EventHandler(this.ZoomComboBox_SelectedIndexChanged);
            this.zoomComboBox.Size = new Size(75, this.zoomComboBox.Height);
            this.zoomComboBox.MaxDropDownItems = 99;
            //
            // unitsComboBox
            //
            this.unitsComboBox.UnitsChanged += new EventHandler(UnitsComboBox_UnitsChanged);
            this.unitsComboBox.LowercaseStrings = false;
            this.unitsComboBox.UnitsDisplayType = UnitsDisplayType.Plural;
            this.unitsComboBox.Units = MeasurementUnit.Pixel;
            this.unitsComboBox.Size = new Size(90, this.unitsComboBox.Height);
            //
            // ViewConfigStrip
            //
            this.Items.Add(this.separator0);
            this.Items.Add(this.zoomOutButton);
            this.Items.Add(this.zoomComboBox);
            this.Items.Add(this.zoomInButton);
            this.Items.Add(this.separator1);
            this.Items.Add(this.gridButton);
            this.Items.Add(this.rulersButton);
            this.Items.Add(this.unitsLabel);
            this.Items.Add(this.unitsComboBox);
            this.ResumeLayout(false);
        }

        private void UnitsComboBox_UnitsChanged(object sender, EventArgs e)
        {
            this.OnUnitsChanged();
        }

        private void SetZoomText()
        {
            if (this.ignoreZoomChanges == 0)
            {
                this.zoomComboBox.BackColor = SystemColors.Window;
                string newText = zoomComboBox.Text;

                switch (zoomBasis)
                {
                    case ZoomBasis.FitToWindow:
                        newText = this.windowText;
                        break;

                    case ZoomBasis.ScaleFactor:
                        newText = scaleFactor.ToString();
                        break;
                }

                if (zoomComboBox.Text != newText)
                {
                    zoomComboBox.Text = newText;
                    zoomComboBox.ComboBox.Update();
                }
            }
        }

        private void OnDrawGridChanged()
        {
            if (AppEnvironment.Environment.DrawGrid != this.DrawGrid)
                AppEnvironment.Environment.DrawGrid = this.DrawGrid;
        }

        private void OnRulersEnabledChanged()
        {
            if (AppEnvironment.Environment.RulersEnabled != this.RulersEnabled)
                AppEnvironment.Environment.RulersEnabled = this.RulersEnabled;
        }

        private void OnUnitsChanged()
        {
            if (AppEnvironment.Environment.Units != this.Units)
                AppEnvironment.Environment.Units = this.Units;
        }

        private void OnZoomScaleChanged()
        {
            if (zoomBasis == ZoomBasis.ScaleFactor)
            {
                DocumentManager.Manager.ZoomToScale(ScaleFactor);
            }
        }

        private void OnZoomIn()
        {
            DocumentManager.Manager.ZoomIn();
        }

        private void OnZoomOut()
        {
            DocumentManager.Manager.ZoomOut();
        }

        private void OnZoomBasisChanged()
        {
            DocumentManager.Manager.ZoomBasis(this.ZoomBasis);
        }

        private void ZoomComboBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                int val = 1;
                e.Cancel = false;

                if (zoomComboBox.Text == this.windowText)
                {
                    ZoomBasis = ZoomBasis.FitToWindow;
                }
                else
                {
                    try
                    {
                        string text = zoomComboBox.Text;

                        if (text.Length == 0)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (text[text.Length - 1] == '%')
                            {
                                text = text.Substring(0, text.Length - 1);
                            }
                            else if (text[0] == '%')
                            {
                                text = text.Substring(1);
                            }

                            val = (int)Math.Round(double.Parse(text));
                            ZoomBasis = ZoomBasis.ScaleFactor;
                        }
                    }

                    catch (FormatException)
                    {
                        e.Cancel = true;
                    }

                    catch (OverflowException)
                    {
                        e.Cancel = true;
                    }

                    if (e.Cancel)
                    {
                        this.zoomComboBox.BackColor = Color.Red;
                        this.zoomComboBox.ToolTipText = SciResources.SciResources.GetString("ZoomConfigWidget.Error.InvalidNumber");
                    }
                    else
                    {
                        if (val < 1)
                        {
                            e.Cancel = true;
                            this.zoomComboBox.BackColor = Color.Red;
                            this.zoomComboBox.ToolTipText = SciResources.SciResources.GetString("ZoomConfigWidget.Error.TooSmall");
                        }
                        else if (val > 3200)
                        {
                            e.Cancel = true;
                            this.zoomComboBox.BackColor = Color.Red;
                            this.zoomComboBox.ToolTipText = SciResources.SciResources.GetString("ZoomConfigWidget.Error.TooLarge");
                        }
                        else
                        {
                            // Clear the error
                            e.Cancel = false;
                            this.zoomComboBox.ToolTipText = string.Empty;
                            this.zoomComboBox.BackColor = SystemColors.Window;
                            ScaleFactor = new ScaleFactor(val, 100);
                            SuspendEvents();
                            ZoomBasis = ZoomBasis.ScaleFactor;
                            ResumeEvents();
                        }
                    }
                }
            }

            catch (FormatException)
            {
            }
        }

        private void ZoomComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.suspendEvents == 0)
            {
                ZoomComboBox_Validating(sender, new CancelEventArgs(false));
            }
        }

        private void ZoomComboBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == '\n' || e.KeyChar == '\r')
            {
                ZoomComboBox_Validating(sender, new CancelEventArgs(false));
                zoomComboBox.Select(0, zoomComboBox.Text.Length);
            }
        }

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {

            if (e.ClickedItem == this.zoomInButton)
            {
                Tracing.LogFeature("ViewConfigStrip(ZoomIn)");
                OnZoomIn();
            }
            else if (e.ClickedItem == this.zoomOutButton)
            {
                Tracing.LogFeature("ViewConfigStrip(ZoomOut)");
                OnZoomOut();
            }
            else if (e.ClickedItem == this.rulersButton)
            {
                Tracing.LogFeature("ViewConfigStrip(Rulers)");
                this.rulersButton.Checked = !this.rulersButton.Checked;
                OnRulersEnabledChanged();
            }
            else if (e.ClickedItem == this.gridButton)
            {
                Tracing.LogFeature("ViewConfigStrip(Grid)");
                this.gridButton.Checked = !this.gridButton.Checked;
                this.OnDrawGridChanged();
            }

            base.OnItemClicked(e);
        }
    }
}
