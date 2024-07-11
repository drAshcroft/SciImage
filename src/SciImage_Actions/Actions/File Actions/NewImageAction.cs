/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Plugins.Actions;
using SciImage.SciResources;
using SciImage.SystemLayer.System;

namespace SciImage_Actions.Actions.File_Actions
{
    public sealed class NewImageAction
        : PluginAction
    {
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {

            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
        }
        public override string Name
        {
            get
            {
                return "New Image";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return "File"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }
        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 1; }
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            using (NewFileDialog nfd = new NewFileDialog())
            {
                Size newDocSize = DocumentManager.Manager.GetNewDocumentSize();

                if (Utility.IsClipboardImageAvailable())
                {
                    try
                    {
                        Utility.GCFullCollect();
                        IDataObject clipData = System.Windows.Forms.Clipboard.GetDataObject();

                        using (Image clipImage = (Image)clipData.GetData(DataFormats.Bitmap))
                        {
                            int width2 = clipImage.Width;
                            int height2 = clipImage.Height;
                            newDocSize = new Size(width2, height2);
                        }
                    }

                    catch (Exception ex)
                    {
                        if (ex is OutOfMemoryException ||
                            ex is ExternalException ||
                            ex is NullReferenceException)
                        {
                            // ignore
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                nfd.OriginalSize = new Size(newDocSize.Width, newDocSize.Height);
                nfd.OriginalDpuUnit = SettingNames.GetLastNonPixelUnits();
                nfd.OriginalDpu = Document.GetDefaultDpu(nfd.OriginalDpuUnit);
                nfd.Units = nfd.OriginalDpuUnit;
                nfd.Resolution = nfd.OriginalDpu;
                nfd.ConstrainToAspect = Settings.CurrentUser.GetBoolean(SettingNames.LastMaintainAspectRatioNF, false);

                DialogResult dr = nfd.ShowDialog(FormsManager.BaseForm);

                if (dr == DialogResult.OK)
                {
                    bool success = DocumentManager.Manager .CreateBlankDocumentInNewWorkspace(new Size(nfd.ImageWidth, nfd.ImageHeight), nfd.Units, nfd.Resolution, false);

                    if (success)
                    {
                        ActiveDocumentWorkspace.ZoomBasis = ZoomBasis.FitToWindow;
                        Settings.CurrentUser.SetBoolean(SettingNames.LastMaintainAspectRatioNF, nfd.ConstrainToAspect);

                        if (nfd.Units != MeasurementUnit.Pixel)
                        {
                            Settings.CurrentUser.SetString(SettingNames.LastNonPixelUnits, nfd.Units.ToString());
                        }

                        if (AppEnvironment.Environment.Units != MeasurementUnit.Pixel)
                        {
                            AppEnvironment.Environment.Units = nfd.Units;
                        }
                    }
                }
            }
            return true;
        }

        public NewImageAction()
        {
        }
    }
}
