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

namespace SciImage_Actions.Actions.View_Actions
{
    public sealed class ActualSizeAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "Actual Size";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return "View"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }
        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A | System.Windows.Forms.Keys.Shift  );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 5; }
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
            if (documentWorkspace != null)
            {
                documentWorkspace.ZoomBasis = ZoomBasis.ScaleFactor;
                documentWorkspace.ScaleFactor = ScaleFactor.OneToOne;
            }
            return true ;
        }

        public ActualSizeAction()
            : base(ActionFlags.KeepToolActive)
        {
        }
    }
}
