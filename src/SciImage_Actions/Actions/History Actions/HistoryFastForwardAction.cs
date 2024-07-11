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
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.History_Actions
{
    public sealed class HistoryFastForwardAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "History FastForward";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return ""; }
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
            get { return 0; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 0; }
        }
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {

            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            DateTime lastUpdate = DateTime.Now;

            documentWorkspace.History.BeginStepGroup();

            using (new WaitCursorChanger(documentWorkspace))
            {
                documentWorkspace.SuspendToolCursorChanges();

                while (documentWorkspace.History.RedoStack.Count > 0)
                {
                    documentWorkspace.History.StepForward();

                    if ((DateTime.Now - lastUpdate).TotalMilliseconds >= 500)
                    {
                        documentWorkspace.History.EndStepGroup();
                        documentWorkspace.Update();
                        lastUpdate = DateTime.Now;
                        documentWorkspace.History.BeginStepGroup();
                    }
                }

                documentWorkspace.ResumeToolCursorChanges();
            }

            documentWorkspace.History.EndStepGroup();

            Utility.GCFullCollect();
            documentWorkspace.Document.Invalidate();
            documentWorkspace.Update();

            return false ;
        }

        public HistoryFastForwardAction()
            : base(ActionFlags.KeepToolActive)
        {
            // We use ActionFlags.KeepToolActive because the process of undo/redo has its own
            // set of protocols for determining whether to keep the tool active, or to refresh it
        }
    }
}
