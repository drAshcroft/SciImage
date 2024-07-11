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
    public sealed class ZoomToWindowAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "Zoom to Window";
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 3; }
        }
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null)
            {
                return ActionDisplayOptions.Visible;
            }
            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;

        }

        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord,int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            if (documentWorkspace.ZoomBasis == ZoomBasis.FitToWindow)
            {
                documentWorkspace.ZoomBasis = ZoomBasis.ScaleFactor;
            }
            else
            {
                documentWorkspace.ZoomBasis = ZoomBasis.FitToWindow;
            } 
            
            return true ;
        }

        public ZoomToWindowAction()
            : base(ActionFlags.KeepToolActive)
        {
        }
    }
}
