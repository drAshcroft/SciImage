/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SciImage;
using SciImage.Core.History.HistoryMementos;
using SciImage.Plugins.Actions;
using SciImage.Plugins.FileHandling;
using SciImage.Plugins.FileHandling.PropertyBasedFileTypes;

namespace SciImage_Actions.Actions.File_Actions
{
    public sealed class OpenFileAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "Open...";
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 2; }
        }
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {

            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            string filePath;

            if (ActiveDocumentWorkspace == null)
            {
                filePath = null;
            }
            else
            {
                // Default to the directory the active document came from
                string fileName;
                FileType fileType;
                SaveConfigToken saveConfigToken;
                ActiveDocumentWorkspace.GetDocumentSaveOptions(out fileName, out fileType, out saveConfigToken);
                filePath = Path.GetDirectoryName(fileName);
            }

            string[] newFileNames;
            DialogResult result = DocumentWorkspace.ChooseFiles(FormsManager.BaseForm, out newFileNames, true, filePath);

            if (result == DialogResult.OK)
            {
                DocumentManager.Manager.OpenFilesInNewWorkspace(newFileNames);
            }
            return true ;
        }

        public OpenFileAction()
        {
        }
    }
}
