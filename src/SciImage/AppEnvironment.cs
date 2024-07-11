
using SciImage.Core;
using SciImage.Menus;
using SciImage.Menus.Strips;
using SciImage.PaintForms.ColorPickers;
using SciImage.PaintForms.HistoryForm;
using SciImage.PaintForms.LayerForm;
using SciImage.PaintForms.ToolsForm;
using SciImage.SystemLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciImage.PaintForms.UserControls.ColorPickers;
using SciImage.Plugins.Tools;
using SciImage.SciResources;
using SciImage.SystemLayer.System;

namespace SciImage
{
    /// <summary>
    /// this class has all the global settings as well as acting as a message accelerator to link the user controls with the app core
    /// </summary>
    public class AppEnvironment
    {

        public void Initialize()
        {
            LoadSettings();

            Units = MeasurementUnit.Pixel;
            RulersEnabled = true;
        }

        static AppEnvironment()
        {
            _Environment = new AppEnvironment();
        }

        //prevent non-static calls
        private AppEnvironment()
        {

        }

        private static AppEnvironment _Environment = null;

        public static AppEnvironment Environment
        {
            get
            {
                return _Environment;
            }
        }

        public void SaveSettings()
        {
            Settings.CurrentUser.SetBoolean(SettingNames.Rulers, this.RulersEnabled);
            Settings.CurrentUser.SetBoolean(SettingNames.DrawGrid, this.DrawGrid);

            ToolEnvironment.Environment.SaveSettings();
            MenuManager.MainMenu.SaveSettings();
        }

        public void LoadSettings()
        {
            try
            {

                RulersEnabled = Settings.CurrentUser.GetBoolean(SettingNames.Rulers, false);
                this.DrawGrid = Settings.CurrentUser.GetBoolean(SettingNames.DrawGrid, false);

                AppEnvironment.Environment.Units = (MeasurementUnit)Enum.Parse(typeof(MeasurementUnit), Settings.CurrentUser.GetString(SettingNames.Units, MeasurementUnit.Pixel.ToString()), true);
            }
            catch (Exception)
            {
                try
                {
                    Settings.CurrentUser.Delete(
                        new string[]
                        {
                            SettingNames.Rulers,
                            SettingNames.DrawGrid,
                            SettingNames.Units,
                            SettingNames.DefaultAppEnvironment,
                            SettingNames.DefaultToolTypeName,
                        });
                }
                catch (Exception)
                {
                }
            }
            ToolEnvironment.Environment.LoadSettings();

        }

        #region Documents

        private bool _DrawGrid = true;
        public event EventHandler DrawGridChanged;
        public bool DrawGrid
        {
            get
            {
                return _DrawGrid;
            }

            set
            {
                _DrawGrid = value;
                if (DrawGridChanged != null)
                    DrawGridChanged(this, EventArgs.Empty);

                //if (DocumentManager.ActiveDocumentWorkspace != null && DocumentManager.ActiveDocumentWorkspace.DrawGrid != value)
                //{
                //    DocumentManager.ActiveDocumentWorkspace.DrawGrid = value;
                //}

                Settings.CurrentUser.SetBoolean(SettingNames.DrawGrid, this.DrawGrid);
            }
        }

        public MeasurementUnit _Units = MeasurementUnit.Pixel;
        public event EventHandler UnitsChanged;
        public MeasurementUnit Units
        {
            get
            {
                return _Units;
            }

            set
            {
                if (value != MeasurementUnit.Pixel)
                {
                    Settings.CurrentUser.SetString(SettingNames.LastNonPixelUnits, _Units.ToString());
                }

                _Units = value;
                // ViewConfigStrip.Units = value;
                if (UnitsChanged != null)
                {
                    Settings.CurrentUser.SetString(SettingNames.Units, _Units.ToString());
                    UnitsChanged(this, EventArgs.Empty);
                }
            }
        }

        private bool _RulersEnabled = false;
        public event EventHandler RulersEnabledChanged;
        public bool RulersEnabled
        {
            get
            {
                return _RulersEnabled;
            }
            set
            {

                if (_RulersEnabled != value)
                {
                    _RulersEnabled = value;

                    if (DocumentManager.Manager.ActiveDocumentWorkspace != null)
                    {

                        DocumentManager.Manager.ActiveDocumentWorkspace.UpdateRulerSelectionTinting();
                        Settings.CurrentUser.SetBoolean(SettingNames.Rulers, DocumentManager.Manager.ActiveDocumentWorkspace.RulersEnabled);
                    }
                }

                _RulersEnabled = value;
                if (RulersEnabledChanged != null)
                {
                    RulersEnabledChanged(this, EventArgs.Empty);
                }


            }
        }




        #endregion
      public void  ResetProgressStatusBar()
        {
            if (statusBarProgress !=null)
            {
                StatusBarProgress.ResetProgressStatusBar();
            }
        }

        public void SetProgressStatusBar(double percent)
        {
            if (statusBarProgress != null)
            {
                StatusBarProgress.SetProgressStatusBar(percent);
            }
        }

        public void EraseProgressStatusBar()
        {
            if (statusBarProgress != null)
            {
                StatusBarProgress.EraseProgressStatusBar();
            }
        }

        public void ResetProgressStatusBarAsync()
        {
            if (statusBarProgress != null)
            {
                StatusBarProgress.ResetProgressStatusBarAsync();
            }
        }

        public void EraseProgressStatusBarAsync()
        {
            if (statusBarProgress != null)
            {
                StatusBarProgress.EraseProgressStatusBarAsync();
            }
        }

        private IStatusBarProgress statusBarProgress;
        public IStatusBarProgress StatusBarProgress
        {
        private     get
            {
                return this.statusBarProgress;
            }
            set
            {
                this.statusBarProgress = value;
            }
        }


        public event EventHandler StatusChanged;
        public void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, EventArgs.Empty);
            }
        }

        public event UpdateStatusEvent StatusUpdated;
        public void UpdateStatusBarContextStatus(object sender)
        {
            if (StatusUpdated != null)
            {
                if (DocumentManager.Manager.ActiveDocumentWorkspace != null)
                {
                    StatusUpdated(sender, DocumentManager.Manager.ActiveDocumentWorkspace.StatusText,
                        DocumentManager.Manager.ActiveDocumentWorkspace.StatusIcon);
                }
                else
                {
                    StatusUpdated(sender, string.Empty, null);
                }
            }
        }

        private string _ImageInfoStatusText = "";
        public string ImageInfoStatusText
        {
            get
            {
                return _ImageInfoStatusText;
            }
            set
            {

            }
        }

        private string _CursorInfoText = "";
        public string CursorInfoText
        {
            get
            {
                return _CursorInfoText;
            }
            set
            {

            }
        }
    }

    public delegate void UpdateStatusEvent(object sender, string statusText, ImageResource icon);
}
