/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using SciImage;
using SciImage.Core.History.HistoryMementos;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.Layer_Actions
{
    public sealed class DeleteLayerAction
        : PluginAction 
    {

        private static string StaticName
        {
            get
            {
                return "Delete Layer";
            }
        }

        private static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersDeleteLayerIcon.png");
            }
        }
        public override string Name
        {
            get
            {
                return StaticName;
            }
        }
        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D | System.Windows.Forms.Keys.Shift );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 2; }
        }

        public override System.Drawing.Image Image
        {
            get { return StaticImage.Reference; }
        }
        public override string MainMenuName
        {
            get { return "Layers"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }

        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null || documentWorkspace.ActiveLayer==null)
            {
                return ActionDisplayOptions.Visible;
            }
            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;

        }
        public override bool  PerformAction( System.Collections.Generic.List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace historyWorkspace = ActiveDocumentWorkspace;
            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;
            int layerIndex=index ;
            if (layerIndex < 0 || layerIndex >= historyWorkspace.Document.Layers.Count)
            {
                throw new ArgumentOutOfRangeException("layerIndex = " + layerIndex + 
                    ", expected [0, " + historyWorkspace.Document.Layers.Count + ")");
            }

            HistoryMemento hm = new DeleteLayerHistoryMemento(StaticName, StaticImage, historyWorkspace, historyWorkspace.Document.Layers.GetAt(layerIndex));

            
            historyWorkspace.Document.Layers.RemoveAt(layerIndex);
            if (OptionalHistoryRecord == null)
                historyWorkspace.History.PushNewMemento(hm);
            else
                OptionalHistoryRecord.Add(hm);
            return true ;
        }

        public DeleteLayerAction()
            : base(ActionFlags.None)
        {
            
        }
    }
}
