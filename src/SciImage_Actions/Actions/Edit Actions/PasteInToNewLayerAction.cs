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
using SciImage.Core.History.HistoryMementos;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;
using SciImage_Actions.Actions.Layer_Actions;

namespace SciImage_Actions.Actions.Edit_Actions
{
    public sealed class PasteInToNewLayerAction  : PluginAction 
    {
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null)
            {
                return ActionDisplayOptions.Visible;
            }
            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;

        }
        public override string Name
        {
            get
            {
                return "Paste in New Layer";
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V | System.Windows.Forms.Keys.Shift );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 2; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 5; }
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            bool hfr = new AddNewBlankLayerAction().PerformAction( OptionalHistoryRecord, TargetLayerIndex);
            
            if (hfr == true )
            {
                PasteAction pa = new PasteAction();
                bool result = pa.PerformAction(OptionalHistoryRecord,TargetLayerIndex  );

                if (!result)
                {
                    using (new WaitCursorChanger(documentWorkspace))
                    {
                        if (OptionalHistoryRecord ==null)
                            documentWorkspace.History.StepBackward();
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public PasteInToNewLayerAction()
        {
        }
    }
}
