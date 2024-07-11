/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.SystemLayer;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SciImage.Core;
using System.Reflection;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.PaintForms.UserControls.ColorPickers;
using SciImage.Plugins.Tools.Enums;
using SciImage.Plugins.Tools.Infos;
using SciImage.SciResources;
using SciImage.SystemLayer.Fonts;
using SciImage.SystemLayer.System;
using SciImage.Plugins.Tools;

namespace SciImage
{
    public delegate void ToolChoiceEvent(Type tool);
    /// <summary>
    /// Manages document-independent workspace configuration details, and provides
    /// notification events for every item that can change.
    /// </summary>
    [Serializable]
    public sealed class ToolEnvironment
        : IDisposable,
          ICloneable,
          IDeserializationCallback
    {
        public void LoadSettings()
        {
            ToolFactory.Factory.FindTools();
            ToolFactory.Factory.LoadDefaultToolType();

            this.CurrentToolChoice = ToolFactory.Factory.DefaultToolType;

            //try
            //{
            //    this.toolBar.ToolConfigStrip.LoadFromAppEnvironment(_ToolEnvironment);
            //}
            //catch (Exception)
            //{
            //    _ToolEnvironment.SetToDefaults();
            //    this.toolBar.ToolConfigStrip.LoadFromAppEnvironment(_ToolEnvironment);
            //}
        }
        public void SaveSettings()
        {
            Settings.CurrentUser.SetString(SettingNames.DefaultToolTypeName, ToolFactory.Factory.DefaultToolType.ToString());
        }

        private static ToolEnvironment _ToolEnvironment = null;

        public static ToolEnvironment Environment
        {
            get
            {
                return _ToolEnvironment;
            }
        }

        static ToolEnvironment()
        {
            try
            {
                string defaultAppEnvBase64 = Settings.CurrentUser.GetString(SettingNames.DefaultAppEnvironment, null);

                if (defaultAppEnvBase64 == null)
                {
                    _ToolEnvironment = null;
                }
                else
                {
                    byte[] defaultAppEnvBytes = System.Convert.FromBase64String(defaultAppEnvBase64);
                    BinaryFormatter formatter = new BinaryFormatter();

                    using (MemoryStream stream = new MemoryStream(defaultAppEnvBytes, false))
                    {
                        object defaultAppEnvObject = formatter.Deserialize(stream);
                        _ToolEnvironment = (ToolEnvironment)defaultAppEnvObject;
                    }
                }
            }
            catch (Exception)
            {
                _ToolEnvironment = null;
            }

            if (_ToolEnvironment == null)
            {
                _ToolEnvironment = new ToolEnvironment();
                _ToolEnvironment.SetToDefaults();


            }


        }


        public event ToolChoiceEvent ToolChosen;
        private Type _CurrentToolTypeChoice = null;
        public Type CurrentToolChoice
        {
            get
            {
                return _CurrentToolTypeChoice;
            }
            set
            {
                if (_CurrentToolTypeChoice != value)
                {
                    _CurrentToolTypeChoice = value;
                    if (ToolChosen != null)
                    {
                        ToolChosen(value);
                    }
                }
            }
        }

        private TextAlignment textAlignment;
        private GradientInfo gradientInfo;
        private FontSmoothing fontSmoothing;
        private FontInfo fontInfo;
        private PenInfo penInfo;
        private BrushInfo brushInfo;
        private ColorPixelBase primaryColor;
        private ColorPixelBase secondaryColor;
        private bool alphaBlending;
        private ShapeDrawType shapeDrawType;
        private bool antiAliasing;
        private ColorPickerClickBehavior colorPickerClickBehavior;
        private ResamplingAlgorithm resamplingAlgorithm;
        private float tolerance;

        public static   int MaxFontSize = 2000;
        public static int MinFontSize = 1;
        public static int InitialFontSize = 12;
        public static float MinPenSize = 1.0f;
        public static float MaxPenSize = 500.0f;
        public static int[] BrushSizes =
            new int[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                11, 12, 13, 14, 15, 20, 25, 30,
                35, 40, 45, 50, 55, 60, 65, 70,
                75, 80, 85, 90, 95, 100, 125,
                150, 175, 200, 225, 250, 275, 300,
                325, 350, 375, 400, 425, 450, 475,
                500
            };
        public static int[] DefaultFontSizes =
           new int[]
           {
                8, 9, 10, 11, 12, 14, 16, 18, 20,
                22, 24, 26, 28, 36, 48, 72, 84, 96,
                108, 144, 192, 216, 288
           };

        [OptionalField]
        private CombineMode selectionCombineMode;

        [OptionalField]
        private FloodMode floodMode;

        [OptionalField]
        private SelectionDrawModeInfo selectionDrawModeInfo;


        public void SaveAsDefaultToolEnvironment()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            byte[] bytes = stream.GetBuffer();
            string base64 = Convert.ToBase64String(bytes);
            Settings.CurrentUser.SetString(SettingNames.DefaultAppEnvironment, base64);
        }

        public void LoadFrom(ToolEnvironment toolEnvironment)
        {
            this.textAlignment = toolEnvironment.textAlignment;
            this.gradientInfo = toolEnvironment.gradientInfo.Clone();
            this.fontSmoothing = toolEnvironment.fontSmoothing;
            this.fontInfo = toolEnvironment.fontInfo.Clone();
            this.penInfo = toolEnvironment.penInfo.Clone();
            this.brushInfo = toolEnvironment.brushInfo.Clone();
            this.primaryColor = toolEnvironment.primaryColor;
            this.secondaryColor = toolEnvironment.secondaryColor;
            this.alphaBlending = toolEnvironment.alphaBlending;
            this.shapeDrawType = toolEnvironment.shapeDrawType;
            this.antiAliasing = toolEnvironment.antiAliasing;
            this.colorPickerClickBehavior = toolEnvironment.colorPickerClickBehavior;
            this.resamplingAlgorithm = toolEnvironment.resamplingAlgorithm;
            this.tolerance = toolEnvironment.tolerance;
            this.selectionCombineMode = toolEnvironment.selectionCombineMode;
            this.floodMode = toolEnvironment.floodMode;
            this.selectionDrawModeInfo = toolEnvironment.selectionDrawModeInfo.Clone();
            PerformAllChanged();
        }

        #region Font stuff
        public TextAlignment TextAlignment
        {
            get
            {
                return this.textAlignment;
            }

            set
            {
                if (value != this.textAlignment)
                {
                    OnTextAlignmentChanging();
                    this.textAlignment = value;
                    OnTextAlignmentChanged();
                }
            }
        }

        [field: NonSerialized]
        public event EventHandler TextAlignmentChanging;

        private void OnTextAlignmentChanging()
        {
            if (TextAlignmentChanging != null)
            {
                TextAlignmentChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler TextAlignmentChanged;

        private void OnTextAlignmentChanged()
        {
            if (TextAlignmentChanged != null)
            {
                TextAlignmentChanged(this, EventArgs.Empty);
            }
        }

        public FontInfo FontInfo
        {
            get
            {
                return this.fontInfo;
            }

            set
            {
                if (this.fontInfo != value)
                {
                    OnFontInfoChanging();
                    this.fontInfo = value;
                    OnFontInfoChanged();
                }
            }
        }

        [field: NonSerialized]
        public event EventHandler FontInfoChanging;

        private void OnFontInfoChanging()
        {
            if (FontInfoChanging != null)
            {
                FontInfoChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler FontInfoChanged;

        private void OnFontInfoChanged()
        {
            if (FontInfoChanged != null)
            {
                FontInfoChanged(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler FontSmoothingChanging;

        private void OnFontSmoothingChanging()
        {
            if (FontSmoothingChanging != null)
            {
                FontSmoothingChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler FontSmoothingChanged;

        private void OnFontSmoothingChanged()
        {
            if (FontSmoothingChanged != null)
            {
                FontSmoothingChanged(this, EventArgs.Empty);
            }
        }

        public FontSmoothing FontSmoothing
        {
            get
            {
                return this.fontSmoothing;
            }

            set
            {
                if (this.fontSmoothing != value)
                {
                    OnFontSmoothingChanging();
                    this.fontSmoothing = value;
                    OnFontSmoothingChanged();
                }
            }
        }
        #endregion

        #region GradientInfo
        [field: NonSerialized]
        public event EventHandler GradientInfoChanging;

        private void OnGradientInfoChanging()
        {
            if (GradientInfoChanging != null)
            {
                GradientInfoChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler GradientInfoChanged;

        private void OnGradientInfoChanged()
        {
            if (GradientInfoChanged != null)
            {
                GradientInfoChanged(this, EventArgs.Empty);
            }
        }

        public GradientInfo GradientInfo
        {
            get
            {
                return this.gradientInfo;
            }

            set
            {
                OnGradientInfoChanging();
                this.gradientInfo = value;
                OnGradientInfoChanged();
            }
        }
        #endregion

        #region PenInfo
        public Pen CreatePen(bool swapColors)
        {
            if (!swapColors)
            {
                return PenInfo.CreatePen(BrushInfo, PrimaryColor.ToColor(), SecondaryColor.ToColor());
            }
            else
            {
                return PenInfo.CreatePen(BrushInfo, SecondaryColor.ToColor(), PrimaryColor.ToColor());
            }
        }

        public PenInfo PenInfo
        {
            get
            {
                return this.penInfo.Clone();
            }

            set
            {
                if (this.penInfo != value)
                {
                    OnPenInfoChanging();
                    this.penInfo = value.Clone();
                    OnPenInfoChanged();
                }
            }
        }

        public void AddToPenSize(float delta)
        {
            float newWidth = Utility.Clamp(PenInfo.Width + delta, MinPenSize, MaxPenSize);
            PenInfo newPenInfo = PenInfo.Clone();
            newPenInfo.Width += delta;
            newPenInfo.Width = (float)Utility.Clamp(newPenInfo.Width, MinPenSize, MaxPenSize);
            PenInfo = newPenInfo;
        }


        [field: NonSerialized]
        public event EventHandler PenInfoChanging;

        private void OnPenInfoChanging()
        {
            if (PenInfoChanging != null)
            {
                PenInfoChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler PenInfoChanged;

        private void OnPenInfoChanged()
        {
            if (PenInfoChanged != null)
            {
                PenInfoChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region BrushInfo
        public Brush CreateBrush(bool swapColors)
        {
            if (!swapColors)
            {
                return BrushInfo.CreateBrush(PrimaryColor.ToColor(), SecondaryColor.ToColor());
            }
            else
            {
                return BrushInfo.CreateBrush(SecondaryColor.ToColor(), PrimaryColor.ToColor());
            }
        }

        public BrushInfo BrushInfo
        {
            get
            {
                return this.brushInfo.Clone();
            }

            set
            {
                OnBrushInfoChanging();
                this.brushInfo = value.Clone();
                OnBrushInfoChanged();
            }
        }

        [field: NonSerialized]
        public event EventHandler BrushInfoChanging;

        private void OnBrushInfoChanging()
        {
            if (BrushInfoChanging != null)
            {
                BrushInfoChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler BrushInfoChanged;

        private void OnBrushInfoChanged()
        {
            if (BrushInfoChanged != null)
            {
                BrushInfoChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        public void SwapUserColors()
        {
            //   if (colorsForm != null)
            //      colorsForm.SwapUserColors();
        }
        public void ToggleWhichUserColor()
        {
            //  if (colorsForm != null)
            //     colorsForm.ToggleWhichUserColor();
        }

        #region PrimaryColor
        public ColorPixelBase PrimaryColor
        {
            get
            {
                return this.primaryColor;
            }

            set
            {
                if (this.primaryColor != value)
                {
                    OnPrimaryColorChanging();
                    this.primaryColor = value;
                    OnPrimaryColorChanged();
                }
            }
        }

        [field: NonSerialized]
        public event EventHandler PrimaryColorChanging;

        private void OnPrimaryColorChanging()
        {
            if (PrimaryColorChanging != null)
            {
                PrimaryColorChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler PrimaryColorChanged;

        private void OnPrimaryColorChanged()
        {
            if (PrimaryColorChanged != null)
            {
                PrimaryColorChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region SecondaryColor
        public ColorPixelBase SecondaryColor
        {
            get
            {
                return this.secondaryColor;
            }

            set
            {
                if (this.secondaryColor != value)
                {
                    OnBackColorChanging();
                    this.secondaryColor = value;
                    OnSecondaryColorChanged();
                }
            }
        }

        [field: NonSerialized]
        public event EventHandler SecondaryColorChanging;

        private void OnBackColorChanging()
        {
            if (SecondaryColorChanging != null)
            {
                SecondaryColorChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler SecondaryColorChanged;

        private void OnSecondaryColorChanged()
        {
            if (SecondaryColorChanged != null)
            {
                SecondaryColorChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region AlphaBlending
        public CompositingMode GetCompositingMode()
        {
            return this.alphaBlending ? CompositingMode.SourceOver : CompositingMode.SourceCopy;
        }

        public bool AlphaBlending
        {
            get
            {
                return this.alphaBlending;
            }

            set
            {
                if (value != this.alphaBlending)
                {
                    OnAlphaBlendingChanging();
                    this.alphaBlending = value;
                    OnAlphaBlendingChanged();
                }
            }
        }

        [field: NonSerialized]
        public event EventHandler AlphaBlendingChanging;

        private void OnAlphaBlendingChanging()
        {
            if (AlphaBlendingChanging != null)
            {
                AlphaBlendingChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler AlphaBlendingChanged;

        private void OnAlphaBlendingChanged()
        {
            if (AlphaBlendingChanged != null)
            {
                AlphaBlendingChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region ShapeDrawType
        public ShapeDrawType ShapeDrawType
        {
            get
            {
                return this.shapeDrawType;
            }

            set
            {
                if (this.shapeDrawType != value)
                {
                    OnShapeDrawTypeChanging();
                    this.shapeDrawType = value;
                    OnShapeDrawTypeChanged();
                }
            }
        }

        [field: NonSerialized]
        public event EventHandler ShapeDrawTypeChanging;

        private void OnShapeDrawTypeChanging()
        {
            if (ShapeDrawTypeChanging != null)
            {
                ShapeDrawTypeChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler ShapeDrawTypeChanged;

        private void OnShapeDrawTypeChanged()
        {
            if (ShapeDrawTypeChanged != null)
            {
                ShapeDrawTypeChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region AntiAliasing
        [field: NonSerialized]
        public event EventHandler AntiAliasingChanging;

        private void OnAntiAliasingChanging()
        {
            if (AntiAliasingChanging != null)
            {
                AntiAliasingChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler AntiAliasingChanged;

        private void OnAntiAliasingChanged()
        {
            if (AntiAliasingChanged != null)
            {
                AntiAliasingChanged(this, EventArgs.Empty);
            }
        }

        public bool AntiAliasing
        {
            get
            {
                return this.antiAliasing;
            }

            set
            {
                if (this.antiAliasing != value)
                {
                    OnAntiAliasingChanging();
                    this.antiAliasing = value;
                    OnAntiAliasingChanged();
                }
            }
        }
        #endregion

        #region Color Picker behavior
        [field: NonSerialized]
        public event EventHandler ColorPickerClickBehaviorChanging;

        private void OnColorPickerClickBehaviorChanging()
        {
            if (ColorPickerClickBehaviorChanging != null)
            {
                ColorPickerClickBehaviorChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler ColorPickerClickBehaviorChanged;

        private void OnColorPickerClickBehaviorChanged()
        {
            if (ColorPickerClickBehaviorChanged != null)
            {
                ColorPickerClickBehaviorChanged(this, EventArgs.Empty);
            }
        }

        public ColorPickerClickBehavior ColorPickerClickBehavior
        {
            get
            {
                return this.colorPickerClickBehavior;
            }

            set
            {
                if (this.colorPickerClickBehavior != value)
                {
                    OnColorPickerClickBehaviorChanging();
                    this.colorPickerClickBehavior = value;
                    OnColorPickerClickBehaviorChanged();
                }
            }
        }
        #endregion

        #region ResamplingAlgorithm
        [field: NonSerialized]
        public event EventHandler ResamplingAlgorithmChanging;

        private void OnResamplingAlgorithmChanging()
        {
            if (ResamplingAlgorithmChanging != null)
            {
                ResamplingAlgorithmChanging(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler ResamplingAlgorithmChanged;

        private void OnResamplingAlgorithmChanged()
        {
            if (ResamplingAlgorithmChanged != null)
            {
                ResamplingAlgorithmChanged(this, EventArgs.Empty);
            }
        }

        public ResamplingAlgorithm ResamplingAlgorithm
        {
            get
            {
                return this.resamplingAlgorithm;
            }

            set
            {
                if (value != this.resamplingAlgorithm)
                {
                    OnResamplingAlgorithmChanging();
                    this.resamplingAlgorithm = value;
                    OnResamplingAlgorithmChanged();
                }
            }
        }
        #endregion

        #region Tolerance
        [field: NonSerialized]
        public event EventHandler ToleranceChanged;

        private void OnToleranceChanged()
        {
            if (ToleranceChanged != null)
            {
                ToleranceChanged(this, EventArgs.Empty);
            }
        }

        [field: NonSerialized]
        public event EventHandler ToleranceChanging;

        private void OnToleranceChanging()
        {
            if (ToleranceChanging != null)
            {
                ToleranceChanging(this, EventArgs.Empty);
            }
        }

        public float Tolerance
        {
            get
            {
                return tolerance;
            }

            set
            {
                if (tolerance != value)
                {
                    tolerance = value;
                    OnToleranceChanged();
                }
            }
        }
        #endregion

        #region SelectionCombineMode
        [field: NonSerialized]
        public event EventHandler SelectionCombineModeChanged;

        private void OnSelectionCombineModeChanged()
        {
            if (SelectionCombineModeChanged != null)
            {
                SelectionCombineModeChanged(this, EventArgs.Empty);
            }
        }

        public CombineMode SelectionCombineMode
        {
            get
            {
                return this.selectionCombineMode;
            }

            set
            {
                if (this.selectionCombineMode != value)
                {
                    this.selectionCombineMode = value;
                    OnSelectionCombineModeChanged();
                }
            }
        }
        #endregion

        #region FloodMode
        [field: NonSerialized]
        public event EventHandler FloodModeChanged;

        private void OnFloodModeChanged()
        {
            if (FloodModeChanged != null)
            {
                FloodModeChanged(this, EventArgs.Empty);
            }
        }

        public FloodMode FloodMode
        {
            get
            {
                return this.floodMode;
            }

            set
            {
                if (this.floodMode != value)
                {
                    this.floodMode = value;
                    OnFloodModeChanged();
                }
            }
        }
        #endregion

        #region SelectionDrawModeInfo
        [field: NonSerialized]
        public event EventHandler SelectionDrawModeInfoChanged;

        private void OnSelectionDrawModeInfoChanged()
        {
            if (SelectionDrawModeInfoChanged != null)
            {
                SelectionDrawModeInfoChanged(this, EventArgs.Empty);
            }
        }

        public SelectionDrawModeInfo SelectionDrawModeInfo
        {
            get
            {
                return this.selectionDrawModeInfo.Clone();
            }

            set
            {
                if (!this.selectionDrawModeInfo.Equals(value))
                {
                    this.selectionDrawModeInfo = value.Clone();
                    OnSelectionDrawModeInfoChanged();
                }
            }
        }
        #endregion

        public void PerformAllChanged()
        {
            OnFontInfoChanged();
            OnFontSmoothingChanged();
            OnPenInfoChanged();
            OnBrushInfoChanged();
            OnGradientInfoChanged();
            OnSecondaryColorChanged();
            OnPrimaryColorChanged();
            OnAlphaBlendingChanged();
            OnShapeDrawTypeChanged();
            OnAntiAliasingChanged();
            OnTextAlignmentChanged();
            OnToleranceChanged();
            OnColorPickerClickBehaviorChanged();
            OnResamplingAlgorithmChanging();
            OnSelectionCombineModeChanged();
            OnFloodModeChanged();
            OnSelectionDrawModeInfoChanged();
        }

        public void SetToDefaults()
        {
            this.antiAliasing = true;
            this.fontSmoothing = FontSmoothing.Smooth;
            this.primaryColor = ColorBgra.sFromBgra(0, 0, 0, 255);
            this.secondaryColor = ColorBgra.sFromBgra(255, 255, 255, 255);
            this.gradientInfo = new GradientInfo(GradientType.LinearClamped, false);
            this.penInfo = new PenInfo(PenInfo.DefaultDashStyle, 2.0f, PenInfo.DefaultLineCap, PenInfo.DefaultLineCap, PenInfo.DefaultCapScale);
            this.brushInfo = new BrushInfo(BrushType.Solid, HatchStyle.BackwardDiagonal);

            try
            {
                this.fontInfo = new FontInfo(new FontFamily("Arial"), 12, FontStyle.Regular);
            }

            catch (Exception)
            {
                this.fontInfo = new FontInfo(new FontFamily(GenericFontFamilies.SansSerif), 12, FontStyle.Regular);
            }

            this.textAlignment = TextAlignment.Left;
            this.shapeDrawType = ShapeDrawType.Outline;
            this.alphaBlending = true;
            this.tolerance = 0.5f;

            this.colorPickerClickBehavior = ColorPickerClickBehavior.NoToolSwitch;
            this.resamplingAlgorithm = ResamplingAlgorithm.Bilinear;
            this.selectionCombineMode = CombineMode.Replace;
            this.floodMode = FloodMode.Local;
            this.selectionDrawModeInfo = SelectionDrawModeInfo.CreateDefault();
        }

        private ToolEnvironment()
        {
            SetToDefaults();

        }

        ~ToolEnvironment()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public ToolEnvironment Clone()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            stream.Seek(0, SeekOrigin.Begin);
            object cloned = formatter.Deserialize(stream);
            stream.Dispose();
            stream = null;
            return (ToolEnvironment)cloned;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            if (this.selectionDrawModeInfo == null)
            {
                this.selectionDrawModeInfo = SelectionDrawModeInfo.CreateDefault();
            }
        }
    }
}