/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.History_Actions
{
    public sealed class HistoryUndoAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "Undo";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return "Edit"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }
        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 0; }
        }
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null)
            {
                return ActionDisplayOptions.Visible;
            }
            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;

        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            if (documentWorkspace.History.UndoStack.Count > 0)
            {
                if (!(documentWorkspace.History.UndoStack[documentWorkspace.History.UndoStack.Count - 1] is NullHistoryMemento))
                {
                    using (new WaitCursorChanger(documentWorkspace.FindForm()))
                    {
                        documentWorkspace.History.StepBackward();
                        documentWorkspace.Update();
                    }
                }

                Utility.GCFullCollect();
            }

            return true ;
        }

        public HistoryUndoAction()
            : base(ActionFlags.KeepToolActive)
        {
            // We use ActionFlags.KeepToolActive because the process of undo/redo has its own
            // set of protocols for determine whether to keep the tool active, or to refresh it
        }
    }
}
