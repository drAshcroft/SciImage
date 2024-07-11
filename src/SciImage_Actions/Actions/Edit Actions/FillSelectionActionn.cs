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
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Selection;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.Edit_Actions
{
    public sealed class FillSelectionAction
        : PluginAction 
    {
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null)
            {
                return ActionDisplayOptions.Visible;
            }
            if (documentWorkspace.ActiveLayer.Surface !=null)
            {
                return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
            }
            return ActionDisplayOptions.Hidden;
        }
        private static string StaticName
        {
            get
            {
                return "Fill Selection";
            }
        }

        private static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuEditFillSelectionIcon.png");
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
                return (System.Windows.Forms.Keys.F9 );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 3; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 2; }
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace docWorkspace = ActiveDocumentWorkspace;
            if (docWorkspace.Selection.IsEmpty)
            {
                return false ;
            }

            SciRegion region = docWorkspace.Selection.CreateRegion();

            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;

            BitmapLayer layer = (BitmapLayer)(ActiveDocumentWorkspace.Document.Layers[index]);

            SciRegion simplifiedRegion = Utility.SimplifyAndInflateRegion(region);

            HistoryMemento hm = new BitmapHistoryMemento(
                StaticName,
                StaticImage,
                docWorkspace,
                docWorkspace.ActiveLayerIndex,
                simplifiedRegion);
            if (OptionalHistoryRecord == null)
                docWorkspace.History.PushNewMemento(hm);
            else
                OptionalHistoryRecord.Add(hm);
            //EnterCriticalRegion();
            ColorPixelBase c = ToolEnvironment.Environment.PrimaryColor;
            layer.Surface.Clear(region, c);
            layer.Invalidate(simplifiedRegion);

            simplifiedRegion.Dispose();
            region.Dispose();

            return true;
        }

        public FillSelectionAction()
            : base(ActionFlags.None)
        {
            
        }
    }
}
