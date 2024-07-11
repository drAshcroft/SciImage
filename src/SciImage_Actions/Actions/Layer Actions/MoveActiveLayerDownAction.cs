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
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Surfaces.Layers;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.Layer_Actions
{
    public sealed class MoveActiveLayerDownAction
        : PluginAction
    {

        public static string StaticName
        {
            get
            {
                return SciImage.SciResources.SciResources.GetString("MoveLayerDown.HistoryMementoName");
            }
        }

        public static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersMoveLayerDownIcon.png");
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
            get { return StaticImage.Reference; }
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

        private void SwapLayers(DocumentWorkspace docWorkspace, Layer layer1,Layer layer2,int layerIndex1,int layerIndex2)
        {
            
            int firstIndex = Math.Min(layerIndex1, layerIndex2);
            int secondIndex = Math.Max(layerIndex1, layerIndex2);

            if (secondIndex - firstIndex == 1)
            {
                docWorkspace.Document.Layers.RemoveAt(layerIndex1);
                docWorkspace.Document.Layers.Insert(layerIndex2, layer1);
            }
            else
            {
                // general version
                docWorkspace.Document.Layers[layerIndex1] = layer2;
                docWorkspace.Document.Layers[layerIndex2] = layer1;
            }

            ((Layer)docWorkspace.Document.Layers[layerIndex1]).Invalidate();
            ((Layer)docWorkspace.Document.Layers[layerIndex2]).Invalidate();



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

            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;
            

            if (index != 0)
            {
                BitmapLayer L1 = ((BitmapLayer)documentWorkspace.ActiveLayer);
                BitmapLayer L2 = ((BitmapLayer)documentWorkspace.Document.Layers[index-1]);

                 HistoryMemento bh1 = new BitmapHistoryMemento
                ("",
                StaticImage ,
                documentWorkspace,
                index,
                documentWorkspace.Selection.CreateRegion(),
                L1.Surface );


                 HistoryMemento bh2 = new BitmapHistoryMemento
                ("",
                StaticImage ,
                documentWorkspace,
                index-1,
                documentWorkspace.Selection.CreateRegion(),
                L2.Surface);

                SwapLayers(documentWorkspace, L1 ,
                    L2, index, index - 1);
                if (OptionalHistoryRecord == null)
                {
                    CompoundHistoryMemento hm =
                        new CompoundHistoryMemento("Move Layer Down", StaticImage, new HistoryMemento[] { bh1, bh2 });
                    documentWorkspace.History.PushNewMemento(hm);
                }
                else
                {
                    OptionalHistoryRecord.Add(bh1);
                    OptionalHistoryRecord.Add(bh2);
                }
            }

            return true ;
        }

        public MoveActiveLayerDownAction()
            : base(ActionFlags.None)
        {
        }
    }
}
