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
using System.IO;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Plugins.Actions;
using SciImage.SystemLayer.Dialogs.FileDialogs;
using SciImage.SystemLayer.ScanningAndCapture;

namespace SciImage_Actions.Actions.File_Actions
{
    public sealed class AcquireFromScannerOrCameraAction
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
                return "Acquire From Scanner or Camera";
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
                return (System.Windows.Forms.Keys.F9);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 4; }
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            if (!ScanningAndPrinting.CanScan)
            {
                Utility.ShowWiaError(FormsManager.BaseForm);
                return false;
            }

            string tempName = Path.ChangeExtension(FileSystem.GetTempFileName(), ".bmp");
            ScanResult result;

            try
            {
                result = ScanningAndPrinting.Scan(FormsManager.BaseForm, tempName);
            }

            // If there was an exception, let's assume the user has already received an error dialog,
            // either from Windows or from the WIA UI, and let's /not/ present another error dialog.
            catch (Exception)
            {
                result = ScanResult.UserCancelled;
            }

            if (result == ScanResult.Success)
            {
                string errorText = null;

                try
                {
                    Image image;

                    try
                    {
                        image = SciImage.SciResources.SciResources.LoadImage(tempName);
                    }

                    catch (FileNotFoundException)
                    {
                        errorText = SciImage.SciResources.SciResources.GetString("LoadImage.Error.FileNotFoundException");
                        throw;
                    }

                    catch (OutOfMemoryException)
                    {
                        errorText = SciImage.SciResources.SciResources.GetString("LoadImage.Error.OutOfMemoryException");
                        throw;
                    }

                    Document document;

                    try
                    {
                        document = Document.FromImage(image);
                    }

                    catch (OutOfMemoryException)
                    {
                        errorText = SciImage.SciResources.SciResources.GetString("LoadImage.Error.OutOfMemoryException");
                        throw;
                    }

                    finally
                    {
                        image.Dispose();
                        image = null;
                    }

                    DocumentWorkspace dw = DocumentManager.Manager.AddNewDocumentWorkspace();

                    try
                    {
                        dw.Document = document;
                    }

                    catch (OutOfMemoryException)
                    {
                        errorText = SciImage.SciResources.SciResources.GetString("LoadImage.Error.OutOfMemoryException");
                        throw;
                    }

                    document = null;
                    dw.SetDocumentSaveOptions(null, null, null);
                    dw.History.ClearAll();

                    HistoryMemento newHA = new NullHistoryMemento(
                        SciImage.SciResources.SciResources.GetString("AcquireImageAction.Name"),
                        SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersAddNewLayerIcon.png"));
                    if (OptionalHistoryRecord == null)
                        dw.History.PushNewMemento(newHA);
                    else
                        OptionalHistoryRecord.Add(newHA);

                    DocumentManager.Manager.ActiveDocumentWorkspace = dw;

                    // Try to delete the temp file but don't worry if we can't
                    try
                    {
                        File.Delete(tempName);
                    }

                    catch
                    {
                    }
                }

                catch (Exception)
                {
                    if (errorText != null)
                    {
                        Utility.ErrorBox(FormsManager.BaseForm, errorText);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return true;
        }
    }
}
