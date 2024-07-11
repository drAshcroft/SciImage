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
using SciImage.Core.Surfaces.Layers;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.Layer_Actions
{
    public sealed class DuplicateLayerAction
        : PluginAction 
    {
        public static string StaticName
        {
            get
            {
                return "Duplicate Layer";
            }
        }

        public static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersDuplicateLayerIcon.png");
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D | System.Windows.Forms.Keys.Shift );
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
            if (documentWorkspace == null || documentWorkspace.ActiveLayer == null)
            {
                return ActionDisplayOptions.Visible;
            }
            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;

        }
        public override bool PerformAction( System.Collections.Generic.List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace historyWorkspace = ActiveDocumentWorkspace;
            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;
            int layerIndex = index;
            if (layerIndex < 0 || layerIndex >= historyWorkspace.Document.Layers.Count)
            {
                throw new ArgumentOutOfRangeException("layerIndex = " + layerIndex + ", expected [0, " + historyWorkspace.Document.Layers.Count + ")");
            }

            Layer newLayer = null;

            newLayer = (Layer)historyWorkspace.ActiveLayer.Clone();
            newLayer.IsBackground = false;
            int newIndex = 1 + layerIndex;

            HistoryMemento ha = new NewLayerHistoryMemento(
                StaticName,
                StaticImage,
                historyWorkspace,
                newIndex);

            //EnterCriticalRegion();
            historyWorkspace.Document.Layers.Insert(newIndex, newLayer);
            newLayer.Invalidate();
            if (OptionalHistoryRecord == null)
                historyWorkspace.History.PushNewMemento(ha);
            else
                OptionalHistoryRecord.Add(ha);

            return true ;
        }

        public DuplicateLayerAction()
            : base(ActionFlags.None)
        {
           
        }
    }
}
