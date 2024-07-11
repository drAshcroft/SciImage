/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Windows.Forms;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.History_Actions
{
    public sealed class ClearHistoryAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "Clear History";
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
            if (DialogResult.Yes == Utility.AskYesNo(documentWorkspace, 
                SciImage.SciResources.SciResources.GetString("ClearHistory.Confirmation")))
            {
                documentWorkspace.History.ClearAll();

                documentWorkspace.History.PushNewMemento(new NullHistoryMemento(
                    SciImage.SciResources.SciResources.GetString("ClearHistory.HistoryMementoName"),
                    SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersDeleteLayerIcon.png")));
            }

            return true ;
        }

        public ClearHistoryAction()
            : base(ActionFlags.None)
        {
        }
    }
}
