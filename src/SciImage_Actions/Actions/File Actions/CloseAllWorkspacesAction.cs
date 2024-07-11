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
using System.ComponentModel;
using System.Windows.Forms;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Forms;
using SciImage.Plugins.Actions;
using SciImage.SystemLayer.Forms;

namespace SciImage_Actions.Actions.File_Actions
{
    public sealed class CloseAllWorkspacesAction
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
                return "Close All Pictures";
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
            get { return 4; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 1; }
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace originalDW = ActiveDocumentWorkspace;

            int oldLatency = 10;

            try
            {
                oldLatency = FormsManager.Manager.DocumentStrip.ThumbnailUpdateLatency;
                FormsManager.Manager.DocumentStrip.ThumbnailUpdateLatency = 0;
            }

            catch (NullReferenceException)
            {
                // See bug #2544
            }

            List<DocumentWorkspace> unsavedDocs = new List<DocumentWorkspace>();
            foreach (DocumentWorkspace dw in DocumentManager.Manager.DocumentWorkspaces)
            {
                if (dw.Document != null && dw.Document.Dirty)
                {
                    unsavedDocs.Add(dw);
                }
            }

            if (unsavedDocs.Count == 1)
            {
                CloseWorkspaceAction cwa = new CloseWorkspaceAction();
                cwa.PerformAction( OptionalHistoryRecord, TargetLayerIndex);

            }
            else if (unsavedDocs.Count > 1)
            {
                using (UnsavedChangesDialog dialog = new UnsavedChangesDialog())
                {
                    dialog.DocumentClicked += (s, e2) => {  ActiveDocumentWorkspace = e2.Data; };

                    dialog.Documents = unsavedDocs.ToArray();

                    if (ActiveDocumentWorkspace.Document.Dirty)
                    {
                        dialog.SelectedDocument = ActiveDocumentWorkspace;
                    }

                    Form mainForm = FormsManager.BaseForm.FindForm();
                    if (mainForm != null)
                    {
                        SciBaseForm asPDF = mainForm as SciBaseForm;

                        if (asPDF != null)
                        {
                            asPDF.RestoreWindow();
                        }
                    }


                    DialogResult dr = Utility.ShowDialog(dialog, FormsManager.BaseForm);

                    switch (dr)
                    {
                        case DialogResult.Yes:
                            {
                                foreach (DocumentWorkspace dw in unsavedDocs)
                                {
                                    ActiveDocumentWorkspace = dw;
                                    bool result = dw.DoSave();

                                    if (result)
                                    {
                                        DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                                    }
                                    else
                                    {

                                        break;
                                    }
                                }
                            }
                            break;

                        case DialogResult.No:

                            break;

                        case DialogResult.Cancel:

                            break;

                        default:
                            throw new InvalidEnumArgumentException();
                    }
                }
            }

            try
            {
                FormsManager.Manager.DocumentStrip.ThumbnailUpdateLatency = oldLatency;
            }

            catch (NullReferenceException)
            {
                // See bug #2544
            }


            UI.SuspendControlPainting(FormsManager.BaseForm);

            foreach (DocumentWorkspace dw in DocumentManager.Manager.DocumentWorkspaces)
            {
                DocumentManager.Manager.RemoveDocumentWorkspace(dw);
            }

            UI.ResumeControlPainting(FormsManager.BaseForm);
            FormsManager.BaseForm.Invalidate(true);

            return true;
        }

        public CloseAllWorkspacesAction()
        {

        }
    }
}
