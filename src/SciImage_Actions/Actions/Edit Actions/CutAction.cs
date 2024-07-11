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
using SciImage.Plugins.Actions;
using SciImage.Plugins.Tools;
using SciImage.SciResources;
using SciImage.SystemLayer.System;

namespace SciImage_Actions.Actions.Edit_Actions
{
    public sealed class CutAction:PluginAction 
    {
        private static string StaticName
        {
            get
            {
                return "Cut";
            }
        }

        private static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuEditCutIcon.png");
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
            get { return StaticImage.Reference ; }
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 2; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 1; }
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
            HistoryMemento finalHM;

            if (documentWorkspace.Selection.IsEmpty)
            {
                finalHM = null;
            }
            else
            {
                CopyToClipboardAction ctca = new CopyToClipboardAction();
                ctca.PerformAction(OptionalHistoryRecord,TargetLayerIndex   );

                
                    using (new PushNullToolMode(documentWorkspace))
                    {
                        EraseSelectionAction esa = new EraseSelectionAction();
                        esa.PerformAction( OptionalHistoryRecord, TargetLayerIndex);
                    }
                
            }

           
            return true ;
        }

        public CutAction()
        {
            Tracing.LogFeature("CutAction");
        }
    }
}
