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
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.Layers;
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.Layer_Actions
{
    public class FlipLayerVerticalAction
        : PluginAction 
    {
        public override string Name
        {
            get
            {
                return StaticName ;
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
                return (System.Windows.Forms.Keys.F9);
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
        public static string StaticName
        {
            get
            {
                return "Flip Layer Vertical";
            }
        }

        private void Flip(Surface surface)
        {
            for (int x = 0; x < surface.Width; ++x)
            {
                for (int y = 0; y < surface.Height / 2; ++y)
                {
                    ColorPixelBase temp = surface.GetPoint(x, y);
                    surface[x, y] = surface[x, surface.Height - y - 1];
                    surface.SetPoint(x, surface.Height - y - 1, temp);
                }
            }
        }
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null)
            {
                return ActionDisplayOptions.Visible;
            }
            if (documentWorkspace.ActiveLayer.Surface!=null)
            {
                return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
            }
            return ActionDisplayOptions.Hidden;
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;

            Layer layer = (Layer)(ActiveDocumentWorkspace.Document.Layers[index]);

            HistoryMemento bha = new BitmapHistoryMemento
                ("Flip Layer Vertical",
                SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersFlipVerticalIcon.png"),
                ActiveDocumentWorkspace,
                index,
                ActiveDocumentWorkspace.Selection.CreateRegion(),
                layer.Surface);
            if (OptionalHistoryRecord == null)
                ActiveDocumentWorkspace.History.PushNewMemento(bha);
            else
                OptionalHistoryRecord.Add(bha);
            Flip(layer.Surface);
            layer.Invalidate();
            //appWorkspace.ActiveDocumentWorkspace.Update();
            return true;
        }

        public FlipLayerVerticalAction()
/*            : base(StaticName,
                   PdnResources.GetImageResource("Icons.MenuLayersFlipVerticalIcon.png"), 
                   FlipType.Vertical,
                   layerIndex)*/
        {
        }
    }
}
