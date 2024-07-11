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
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.Image_Actions
{
    public sealed class FlattenAction
        : PluginAction 
    {
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
            get { return "Image"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }
        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F | System.Windows.Forms.Keys.Shift );
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
        public static string StaticName
        {
            get
            {
                return "Flatten";
            }
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
            object savedSelection = null;
            List<HistoryMemento> actions = new List<HistoryMemento>();

            if (!historyWorkspace.Selection.IsEmpty)
            {
                savedSelection = historyWorkspace.Selection.Save();
                DeselectAction da = new DeselectAction();
                List<HistoryMemento> lhm = new List<HistoryMemento>();
                da.PerformAction(lhm,TargetLayerIndex  );
                actions.AddRange(lhm);
            }

            ReplaceDocumentHistoryMemento rdha = new ReplaceDocumentHistoryMemento(null, null, historyWorkspace);
            actions.Add(rdha);
            CompoundHistoryMemento chm = null;
            if (OptionalHistoryRecord == null)
            {
                chm = new CompoundHistoryMemento(
                    StaticName,
                    SciImage.SciResources.SciResources.GetImageResource("Icons.MenuImageFlattenIcon.png"),
                    actions);
            }
            else
                OptionalHistoryRecord.AddRange(actions);
            // TODO: we can save memory here by serializing, then flattening on to an existing blayer
            Document flat = historyWorkspace.Document.Flatten();

            //EnterCriticalRegion();
            historyWorkspace.Document = flat;

            if (savedSelection != null)
            {
                SelectionHistoryMemento shm = new SelectionHistoryMemento(null, null, historyWorkspace);
                historyWorkspace.Selection.Restore(savedSelection);
                if (OptionalHistoryRecord == null)
                    chm.PushNewAction(shm);
                else
                    OptionalHistoryRecord.Add(shm);
            }
            if (OptionalHistoryRecord == null)
                historyWorkspace.History.PushNewMemento(chm);
            
            return true ;
        }

        public FlattenAction()
            : base(ActionFlags.None)
        {
        }
    }
}
