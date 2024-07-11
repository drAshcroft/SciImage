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
using SciImage.Core.Surfaces.Layers;
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.Layer_Actions
{
    public sealed class OpenActiveLayerPropertiesAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "Layer Properties...";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return "Layers"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }
        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.F4 );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 3; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 1; }
        }
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null || documentWorkspace.ActiveLayer == null)
            {
                return ActionDisplayOptions.Visible;
            }
            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;

        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            bool oldDirtyValue = documentWorkspace.Document.Dirty;
            int index = TargetLayerIndex;
            if (index == -1) index = documentWorkspace.ActiveLayerIndex;
            
            using (Form lpd =((Layer) documentWorkspace.Document.Layers[index]).CreateConfigDialog())
            {
                DialogResult result = Utility.ShowDialog(lpd, documentWorkspace.FindForm());

                if (result == DialogResult.Cancel)
                {
                    documentWorkspace.Document.Dirty = oldDirtyValue;
                }
            }

            return false ;
        }

        public OpenActiveLayerPropertiesAction()
            : base(ActionFlags.KeepToolActive)
        {
            // This action does not require that the current tool be deactivated.
        }
    }
}
