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
using System.Drawing;
using SciImage;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.Layer_Actions
{
    public sealed class MergeLayerDownAction
        : PluginAction 
    {
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null || documentWorkspace.ActiveLayer == null)
            {
                return ActionDisplayOptions.Visible;
            }
            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;

        }
        private static string StaticName
        {
            get
            {
                return "Merge Layer Down";
            }
        }

        private static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersMergeLayerDownIcon.png");
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 4; }
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;
            DocumentWorkspace docWorkspace = ActiveDocumentWorkspace;
            int layerIndex = index;

            if (layerIndex < 1 || layerIndex >= docWorkspace.Document.Layers.Count)
            {
                throw new ArgumentException("layerIndex must be greater than or equal to 1, and a valid blayer index. layerIndex=" + 
                    layerIndex + ", allowableRange=[0," + docWorkspace.Document.Layers.Count + ")");
            }

            int bottomLayerIndex = layerIndex - 1;
            Rectangle bounds = docWorkspace.Document.Bounds;
            SciRegion region = new SciRegion(bounds);

            BitmapHistoryMemento bhm = new BitmapHistoryMemento(
                null,
                null,
                docWorkspace,
                bottomLayerIndex,
                region);

            BitmapLayer topLayer = (BitmapLayer)docWorkspace.Document.Layers[layerIndex];
            BitmapLayer bottomLayer = (BitmapLayer)docWorkspace.Document.Layers[bottomLayerIndex];
            RenderArgs bottomRA = new RenderArgs(bottomLayer.Surface);

            topLayer.Render(bottomRA, region);
            bottomLayer.Invalidate();

            bottomRA.Dispose();
            bottomRA = null;

            region.Dispose();
            region = null;

            DeleteLayerAction dlf = new DeleteLayerAction();
            List<HistoryMemento > hlm = new List<HistoryMemento>();
            dlf.PerformAction(hlm,layerIndex );

            HistoryMemento dlhm = hlm[0];

            if (OptionalHistoryRecord == null)
            {
                CompoundHistoryMemento chm = new CompoundHistoryMemento(StaticName, StaticImage, new HistoryMemento[] { bhm, dlhm });
                docWorkspace.History.PushNewMemento(chm);
            }
            else
            {
                OptionalHistoryRecord.Add(bhm);
                OptionalHistoryRecord.Add(dlhm);
            }
            
            return true ;
        }

        public MergeLayerDownAction()
            : base(ActionFlags.None)
        {
            
        }
    }
}
