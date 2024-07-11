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
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.Layer_Actions
{
    public sealed class AddNewBlankLayerAction
        : PluginAction 
    {
        public override string Name
        {
            get
            {
                return "Add New Blank Layer";
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N | System.Windows.Forms.Keys.Shift );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
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
            DocumentWorkspace historyWorkspace = ActiveDocumentWorkspace;
            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;
            BitmapLayer newLayer = null;
            ColorBgra clr = ColorBgra.White;
            clr.alpha = 0;
            newLayer = new BitmapLayer(historyWorkspace.Document.Width, historyWorkspace.Document.Height, clr  );
            string newLayerNameFormat = "Blank Layer {0}";
            newLayer.Name = string.Format(newLayerNameFormat, (1 + historyWorkspace.Document.Layers.Count).ToString());

            int newLayerIndex = index  + 1;

            NewLayerHistoryMemento ha = new NewLayerHistoryMemento(
                "New Layer",
                SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersAddNewLayerIcon.png"),
                historyWorkspace,
                newLayerIndex);


            historyWorkspace.Document.Layers.Insert(newLayerIndex, newLayer);
            if (OptionalHistoryRecord == null)
                historyWorkspace.History.PushNewMemento(ha);
            else
                OptionalHistoryRecord.Add(ha);
            return true;
        }

        public AddNewBlankLayerAction()
            : base(ActionFlags.None)
        {
        }
    }
}
