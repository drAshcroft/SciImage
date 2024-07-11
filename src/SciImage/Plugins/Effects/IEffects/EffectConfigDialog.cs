/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage;
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using SciImage.PropertySystem;
using SciImage.IndirectUI;
using System.Collections.Generic;
using SciImage.SystemLayer;
using System.Threading;

namespace SciImage.Effects
{
    public class EffectConfigDialog
        : PdnBaseForm
    {
        private Surface effectSourceSurface;
        private PdnRegion effectSelection = null;

        /// <summary>
        /// This is the surface that will be used as the source for rendering.
        /// Its contents will not change for the lifetime of this dialog box
        /// ("lifetime" being defined as "until Close() is called")
        /// Treat this object as read-only. In your OnLoad method, feel free
        /// to do any analysis of this surface to populate the dialog box.
        /// </summary>
        [Browsable(false)]
        public Surface EffectSourceSurface
        {
            get
            {
                return effectSourceSurface;
            }

            set
            {
                effectSourceSurface = value;
            }
        }

        [Browsable(false)]
        public PdnRegion Selection
        {
            get
            {
                if (this.DesignMode)
                {
                    return null;
                }
                else
                {
                    if (effectSelection == null || effectSelection.IsEmpty()) 
                    {
                        effectSelection = new PdnRegion();
                        effectSelection.MakeInfinite();
                    }

                    return effectSelection;
                }
            }

            set
            {
                if (effectSelection != null)
                {
                    effectSelection.Dispose();
                    effectSelection = null;
                }

                effectSelection = value;
            }
        }


        public EffectConfigDialog(object context)
        {
            OnBeforeConstructor(context);
            Constructor();
        }

        public EffectConfigDialog()
        {
            Constructor();
        }

        private void Constructor()
        {
            InitializeComponent();
            InitialInitToken();
            effectSelection = new PdnRegion();
            effectSelection.MakeInfinite();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // 
            // EffectConfigDialog
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EffectConfigDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

       

        [Browsable(false)]
        public event EventHandler EffectTokenChanged;
        protected virtual void OnEffectTokenChanged()
        {
            if (EffectTokenChanged != null)
            {
                EffectTokenChanged(this, EventArgs.Empty);
            }
        }

        [Obsolete("Use FinishTokenUpdate() instead", true)]
        public void UpdateToken()
        {
            FinishTokenUpdate();
        }

        public void FinishTokenUpdate()
        {
            InitTokenFromDialog();
            OnEffectTokenChanged();
        }

        /// <summary>
        /// This method must be overriden in the derived classes.
        /// In this you initialize the default values for the token, and
        /// thus the default values for the dialog box.
        /// The job of this function is to initialize this.theEffectToken with
        /// a non-null reference.
        /// </summary>
        protected virtual void InitialInitToken()
        {
            //throw new InvalidOperationException("InitialInitToken was not implemented, or the derived method called the base method");
        }

        

        protected void InitDialogFromToken()
        {   
            // If we don't check for null, we get awful errors in the designer.
            // Good idea to check for that anyway, yeah?
            if (theEffectToken != null)
            {
                InitDialogFromToken((EffectConfigToken)theEffectToken.Clone());
            }
        }

        /// <summary>
        /// This method must be overridden in derived classes.
        /// In this method you must take the values from the dialog box
        /// and use them to properly initialize theEffectToken.
        /// </summary>
        protected virtual void InitTokenFromDialog()
        {
            //throw new InvalidOperationException("InitTokenFromDialog was not implemented, or the derived method called the base method");
        }

        private Effect effect;

        [Browsable(false)]
        public Effect Effect
        {
            get
            {
                return effect;
            }

            set
            {
                effect = value;

                if (effect.Image != null)
                {
                    this.Icon = Utility.ImageToIcon(effect.Image, Utility.TransparentKey);
                }
                else
                {
                    this.Icon = null;
                }
            }
        }

        protected EffectConfigToken theEffectToken;

        [Browsable(false)]
        public EffectConfigToken EffectToken
        {
            get
            {
                return theEffectToken;
            }

            set
            {
                theEffectToken = value;
                OnEffectTokenChanged();
                InitDialogFromToken();
            }
        }

         private const int defaultClientWidth96Dpi = 345;
        private const int defaultClientHeight96Dpi = 125;

        private Button okButton;
        private Button cancelButton;
        private EtchedLine etchedLine;
        private ControlInfo configUI;
        private Panel configUIPanel;
        private Control configUIControl;
        private PropertyCollection properties;
        private PropertyCollection windowProperties;

        public static PropertyCollection CreateWindowProperties()
        {
            List<Property> props = new List<Property>();

            // Title
            props.Add(new StringProperty(
                ControlInfoPropertyNames.WindowTitle, 
                string.Empty, 
                Math.Min(1024, StringProperty.MaxMaxLength)));

            // Sizability -- defaults to false
            props.Add(new BooleanProperty(ControlInfoPropertyNames.WindowIsSizable, false));
            
            // Window width, as a scale of the default width (1.0 is obviously the default)
            props.Add(new DoubleProperty(ControlInfoPropertyNames.WindowWidthScale, 1.0, 1.0, 2.0));

            return new PropertyCollection(props);
        }

        protected EffectConfigToken CreateInitialToken()
        {
            EffectConfigToken token = new EffectConfigToken(this.properties);
            return token;
        }

        protected void InitDialogFromToken(EffectConfigToken effectTokenCopy)
        {
            // We run this twice so that rules don't execute on stale values
            // See bug #2719
            InitDialogFromTokenImpl(effectTokenCopy);
            InitDialogFromTokenImpl(effectTokenCopy);
        }

        private void InitDialogFromTokenImpl(EffectConfigToken effectTokenCopy)
        {
            foreach (string propertyName in effectTokenCopy.PropertyNames)
            {
                Property srcProperty = effectTokenCopy.GetProperty<Property>(propertyName);
                PropertyControlInfo dstPropertyControlInfo = this.configUI.FindControlForPropertyName(propertyName);

                if (dstPropertyControlInfo != null)
                {
                    Property dstProperty = dstPropertyControlInfo.Property;

                    if (dstProperty.ReadOnly)
                    {
                        dstProperty.ReadOnly = false;
                        dstProperty.Value = srcProperty.Value;
                        dstProperty.ReadOnly = true;
                    }
                    else
                    {
                        dstProperty.Value = srcProperty.Value;
                    }
                }
            }
        }

        protected void LoadIntoTokenFromDialog(EffectConfigToken writeValuesHere)
        {
            foreach (string propertyName in this.EffectToken.PropertyNames)
            {
                PropertyControlInfo srcPropertyControlInfo = this.configUI.FindControlForPropertyName(propertyName);

                if (srcPropertyControlInfo != null)
                {
                    Property srcProperty = srcPropertyControlInfo.Property;
                    Property dstProperty = writeValuesHere.GetProperty(srcProperty.Name);

                    if (dstProperty.ReadOnly)
                    {
                        dstProperty.ReadOnly = false;
                        dstProperty.Value = srcProperty.Value;
                        dstProperty.ReadOnly = true;
                    }
                    else
                    {
                        dstProperty.Value = srcProperty.Value;
                    }
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (this.configUIControl is IFirstSelection)
            {
                ((IFirstSelection)this.configUIControl).FirstSelect();
            }

            base.OnShown(e);
        }

        protected void OnLoad(EventArgs e)
        {
            Size idealClientSize = OnLayoutImpl();
            Size idealWindowSize = ClientSizeToWindowSize(idealClientSize);

            Rectangle workingArea = Screen.FromControl(this).WorkingArea;

            Size targetWindowSize = new Size(
                Math.Min(workingArea.Width, idealWindowSize.Width),
                Math.Min(workingArea.Height, idealWindowSize.Height));

            Size targetClientSize = WindowSizeToClientSize(targetWindowSize);

            if (ClientSize != targetClientSize)
            {
                ClientSize = targetClientSize;
                OnLayoutImpl();
            }

            Size newMinimumSize = new Size(idealWindowSize.Width, Math.Min(workingArea.Height, Math.Min(idealWindowSize.Height, UI.ScaleHeight(500))));

            Size minimumClientSize = WindowSizeToClientSize(newMinimumSize);

            MinimumSize = newMinimumSize;

            ClientSize = new Size(
                (int)(ClientSize.Width * (double)this.windowProperties[ControlInfoPropertyNames.WindowWidthScale].Value),
                Math.Min(ClientSize.Height, (workingArea.Height * 9) / 10)); // max height should ever be 90% of working screen area height

          
            base.OnLoad(e);

            try
            {
                InitDialogFromToken();
                FinishTokenUpdate();
            }

            catch (Exception ex)
            {
                SystemLayer.Tracing.Ping(ex.ToString());
                throw;
            }
        
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            Size idealClientSize = OnLayoutImpl();
            base.OnLayout(levent);
        }

        private Size OnLayoutImpl()
        {
            int leftMargin = UI.ScaleWidth(8);
            int rightMargin = UI.ScaleHeight(8);
            int insetWidth = ClientSize.Width - leftMargin - rightMargin;

            int topMargin = UI.ScaleHeight(8);
            int bottomMargin = UI.ScaleHeight(8);
            int insetHeight = ClientSize.Height - topMargin - bottomMargin;

            int hMargin = UI.ScaleWidth(6);
            int vMargin = UI.ScaleHeight(6);

            int minButtonWidth = UI.ScaleWidth(80);

            this.cancelButton.Width = minButtonWidth;
            this.cancelButton.PerformLayout();
            this.cancelButton.Location = new Point(
                ClientSize.Width - rightMargin - this.cancelButton.Width,
                ClientSize.Height - bottomMargin - this.cancelButton.Height);

            this.okButton.Width = minButtonWidth;
            this.okButton.PerformLayout();
            this.okButton.Location = new Point(
                this.cancelButton.Left - hMargin - this.okButton.Width,
                ClientSize.Height - bottomMargin - this.okButton.Height);

            this.etchedLine.Size = this.etchedLine.GetPreferredSize(new Size(insetWidth, 0));
            this.etchedLine.Location = new Point(
                leftMargin,
                Math.Min(this.okButton.Top, this.cancelButton.Top) - vMargin - this.etchedLine.Height);

            // Commenting out this line of code, along with the others marked //2 and //3, fixes the
            // problem whereby the trackbar was always drawing a focus rectangle. However, if we enable
            // a resizable dialog, commenting out these lines results in some fidgety looking drawing.
            bool paintingSuspended = false;
            if (IsHandleCreated)
            {
                UI.SuspendControlPainting(this.configUIPanel); //1
                paintingSuspended = true;
            }

            this.configUIPanel.SuspendLayout();

            int configUIWidth = insetWidth;
            this.configUIPanel.Bounds = new Rectangle(
                leftMargin,
                topMargin,
                insetWidth,
                ClientSize.Height - topMargin - (ClientSize.Height - this.etchedLine.Top) - vMargin);

            Point autoScrollPos = this.configUIPanel.AutoScrollPosition;
            this.configUIPanel.AutoScroll = false;

            int propertyWidth1 = configUIWidth;
            int propertyWidth2 = configUIWidth - SystemInformation.VerticalScrollBarWidth - hMargin;

            int propertyWidth;

            if (this.configUIControl.Width == propertyWidth1)
            {
                propertyWidth = propertyWidth1;
            }
            else if (this.configUIControl.Height <= this.configUIPanel.ClientRectangle.Height)
            {
                propertyWidth = propertyWidth1;
            }
            else
            {
                propertyWidth = propertyWidth2;
            }

            int tries = 3;

            while (tries > 0)
            {
                this.configUIControl.SuspendLayout();
                this.configUIControl.Location = this.configUIPanel.AutoScrollPosition;
                this.configUIControl.Width = propertyWidth;
                this.configUIControl.ResumeLayout(false);
                this.configUIControl.PerformLayout();

                if (this.configUIControl.Height > this.configUIPanel.ClientRectangle.Height)
                {
                    propertyWidth = propertyWidth2;
                    this.configUIPanel.AutoScroll = true;
                    --tries;
                }
                else if (this.configUIControl.Height <= this.configUIPanel.ClientRectangle.Height)
                {
                    propertyWidth = propertyWidth1;
                    this.configUIPanel.AutoScroll = false;
                    --tries;
                }
                else
                {
                    break;
                }
            }

            this.configUIPanel.ResumeLayout(false);
            this.configUIPanel.PerformLayout();

            SendOrPostCallback finishPainting = new SendOrPostCallback((s) =>
                {
                    try
                    {
                        if (IsHandleCreated && !IsDisposed)
                        {
                            this.configUIControl.Location = new Point(0, 0);
                            this.configUIPanel.AutoScrollPosition = new Point(-autoScrollPos.X, -autoScrollPos.Y);

                            if (paintingSuspended)
                            {
                                UI.ResumeControlPainting(this.configUIPanel); //2
                            }

                            Refresh();
                        }
                    }

                    catch (Exception)
                    {
                    }
                });

            SynchronizationContext.Current.Post(finishPainting, null);

            Size idealClientSize = new Size(
                ClientSize.Width, 
                ClientSize.Height + this.configUIControl.Height - this.configUIPanel.Height);

            return idealClientSize;
        }

        public void OnBeforeConstructor(object context)
        {
            this.properties = ((PropertyCollection)context).Clone();
        }

        public EffectConfigDialog(PropertyCollection propertyCollection, ControlInfo configUI, PropertyCollection windowProperties)
            : this(propertyCollection)
        {
            this.windowProperties = windowProperties.Clone();
            this.configUI = (ControlInfo)configUI.Clone();

            // Make sure that the properties in props and configUI are not the same objects
            foreach (Property property in propertyCollection)
            {
                PropertyControlInfo pci = this.configUI.FindControlForPropertyName(property.Name);

                if (pci != null && object.ReferenceEquals(property, pci.Property))
                {
                    throw new ArgumentException("Property references in propertyCollection must not be the same as those in configUI");
                }
            }

            SuspendLayout();

            this.okButton = new Button();
            this.cancelButton = new Button();
            this.cancelButton.Name = "cancelButton";
            this.configUIPanel = new Panel();
            this.configUIControl = (Control)this.configUI.CreateConcreteControl(this);
            this.configUIControl.Location = new Point(0, 0);

            this.configUIPanel.SuspendLayout();
            this.configUIControl.SuspendLayout();

            this.okButton.Name = "okButton";
            this.okButton.AutoSize = true;
            this.okButton.Click += OkButton_Click;
            this.okButton.Text = "OK";
            this.okButton.FlatStyle = FlatStyle.System;

            this.cancelButton.AutoSize = true;
            this.cancelButton.Click += CancelButton_Click;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.FlatStyle = FlatStyle.System;

            this.configUIPanel.Name = "configUIPanel";
            this.configUIPanel.TabStop = false;
            this.configUIPanel.Controls.Add(this.configUIControl);

            this.configUIControl.Name = "configUIControl";

            this.etchedLine = new EtchedLine();
            this.etchedLine.Name = "etchedLine";

            Controls.AddRange(
                new Control[]
                {
                    this.okButton,
                    this.cancelButton,
                    this.etchedLine,
                    this.configUIPanel
                });

            int tabIndex = 0;

            this.configUIControl.TabIndex = tabIndex;
            ++tabIndex;

            // Set up data binding
            foreach (Property property in this.properties)
            {
                PropertyControlInfo pci = this.configUI.FindControlForPropertyName(property.Name);

                if (pci == null)
                {
                    throw new InvalidOperationException("Every property must have a control associated with it");
                }
                else
                {
                    Property controlsProperty = pci.Property;

                    // ASSUMPTION: We assume that the concrete WinForms Control holds a reference to
                    //             the same Property instance as the ControlInfo it was created from.

                    controlsProperty.ValueChanged += ControlsProperty_ValueChanged;
                }
            }

            this.okButton.TabIndex = tabIndex;
            ++tabIndex;

            this.cancelButton.TabIndex = tabIndex;
            ++tabIndex;

            AcceptButton = this.okButton;
            CancelButton = this.cancelButton;

            bool isSizable =  (bool)this.windowProperties[ControlInfoPropertyNames.WindowIsSizable].Value;
            FormBorderStyle = isSizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;

            Text = (string)this.windowProperties[ControlInfoPropertyNames.WindowTitle].Value;

            ClientSize = new Size(UI.ScaleWidth(defaultClientWidth96Dpi), UI.ScaleHeight(defaultClientHeight96Dpi));

            this.configUIControl.ResumeLayout(false);
            this.configUIPanel.ResumeLayout(false);

            ResumeLayout(false);
            PerformLayout();
        }

        private void ControlsProperty_ValueChanged(object sender, EventArgs e)
        {
            Property controlsProperty = (Property)sender;
            Property property = this.properties[controlsProperty.Name];

            if (!property.Value.Equals(controlsProperty.Value))
            {
                if (property.ReadOnly)
                {
                    property.ReadOnly = false;
                    property.Value = controlsProperty.Value;
                    property.ReadOnly = true;
                }
                else
                {
                    property.Value = controlsProperty.Value;
                }
            }

            FinishTokenUpdate();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
