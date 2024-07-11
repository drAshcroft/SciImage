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
using SciImage.Core.Surfaces.Layers;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.Edit_Actions
{
    public sealed class EraseSelectionAction
        : PluginAction 
    {
        private static string StaticName
        {
            get
            {
                return "Erase Selection";
            }
        }

        private static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuEditEraseSelectionIcon.png");
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
                return (System.Windows.Forms.Keys.Delete );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 3; }
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
            DocumentWorkspace docWorkspace = ActiveDocumentWorkspace;
            if (docWorkspace.Selection.IsEmpty)
            {
                return false;
            }

            SelectionHistoryMemento shm = new SelectionHistoryMemento(string.Empty, null, docWorkspace);

            SciRegion region = docWorkspace.Selection.CreateRegion();

            int index = TargetLayerIndex;
            if (index == -1) index = ActiveDocumentWorkspace.ActiveLayerIndex;

            Layer layer = (Layer)(ActiveDocumentWorkspace.Document.Layers[index]);

            SciRegion simplifiedRegion = Utility.SimplifyAndInflateRegion(region);

            HistoryMemento hm = new BitmapHistoryMemento(
                null, 
                null,
                docWorkspace,
                docWorkspace.ActiveLayerIndex, 
                simplifiedRegion);

            if (OptionalHistoryRecord == null)
            {
                HistoryMemento chm = new CompoundHistoryMemento(
                    StaticName,
                    StaticImage,
                    new HistoryMemento[] { shm, hm });

                //EnterCriticalRegion();
                docWorkspace.History.PushNewMemento(chm);
            }
            else
            {
                OptionalHistoryRecord.Add(shm);
                OptionalHistoryRecord.Add(hm);
            }
            layer.Surface.Clear(region, layer.Surface.ColorPixelBase.FromBgra(255, 255, 255, 0));

            layer.Invalidate(simplifiedRegion);
            docWorkspace.Document.Invalidate(simplifiedRegion);
            simplifiedRegion.Dispose();
            region.Dispose();
            docWorkspace.Selection.Reset();

            return true ;
        }

        public EraseSelectionAction()
            : base(ActionFlags.None)
        {
        }
    }
}
