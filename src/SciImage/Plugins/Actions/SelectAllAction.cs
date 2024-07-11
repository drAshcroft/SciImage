/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Drawing.Drawing2D;
using SciImage.Core.History.HistoryMementos;

namespace SciImage.Plugins.Actions
{
    public sealed class SelectAllAction
        : Actions.PluginAction 
    {
        public static string StaticName
        {
            get
            {
                return "Select All";
            }
        }

        public override string Name
        {
            get
            {
                return StaticName ;
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 3; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 4; }
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
            DocumentWorkspace historyWorkspace = ActiveDocumentWorkspace;
            
            SelectionHistoryMemento sha = new SelectionHistoryMemento(
                StaticName, 
                SciResources.SciResources.GetImageResource("Icons.MenuEditSelectAllIcon.png"),
                historyWorkspace);

           // EnterCriticalRegion();
            historyWorkspace.Selection.PerformChanging();
            historyWorkspace.Selection.Reset();
            historyWorkspace.Selection.SetContinuation(historyWorkspace.Document.Bounds, CombineMode.Replace);
            historyWorkspace.Selection.CommitContinuation();
            historyWorkspace.Selection.PerformChanged();
            if (OptionalHistoryRecord == null)
                historyWorkspace.History.PushNewMemento(sha);
            else
                OptionalHistoryRecord.Add(sha);
            return true ;
        }

        public SelectAllAction()
            : base(ActionFlags.None)
        {
        }
    }
}
