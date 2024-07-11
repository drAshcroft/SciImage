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
using SciImage.Core.Selection;
using SciImage.Core.Surfaces.Layers;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.Edit_Actions
{
    public sealed class CopyToClipboardAction:PluginAction 
    {
       
        public override string Name
        {
            get
            {
                return "Copy";
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 2; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 2; }
        }
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null)
            {
                return ActionDisplayOptions.Visible;
            }
            if (documentWorkspace.ActiveLayer == null)
            {
                return ActionDisplayOptions.Visible ;
            }
            if (documentWorkspace.ActiveLayer.Surface  != null)
            {
                return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
            }
            return ActionDisplayOptions.Hidden;
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;

            
            bool success = true;

            if (documentWorkspace.Selection.IsEmpty ||
                (   ( (Layer) documentWorkspace.Document.Layers[index]).Surface ==null ))
            {
                return false ;
            }

            try
            {
                using (new WaitCursorChanger(documentWorkspace))
                {
                    Utility.GCFullCollect();
                    SciRegion selectionRegion = documentWorkspace.Selection.CreateRegion();
                    PdnGraphicsPath selectionOutline = documentWorkspace.Selection.CreatePath();
                    success= documentWorkspace.ActiveLayer.CopyAction(selectionRegion, selectionOutline);
                    selectionRegion.Dispose();
                    selectionOutline.Dispose();
                    
                }
            }

            catch (OutOfMemoryException)
            {
                success = false;
                Utility.ErrorBox(documentWorkspace, SciImage.SciResources.SciResources.GetString("CopyAction.Error.OutOfMemory"));
            }

            catch (Exception)
            {
                success = false;
                Utility.ErrorBox(documentWorkspace, SciImage.SciResources.SciResources.GetString("CopyAction.Error.Generic"));
            }

            Utility.GCFullCollect();
            return success  ;
        }

        public CopyToClipboardAction()
        {
        }
    }
}
