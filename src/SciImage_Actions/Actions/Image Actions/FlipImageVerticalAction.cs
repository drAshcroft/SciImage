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
using SciImage_Actions.Actions.Layer_Actions;

namespace SciImage_Actions.Actions.Image_Actions
{
    public class FlipImageVerticalAction
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
            get { return "Image"; }
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
                return "Flip Image Vertical";
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
            List<HistoryMemento> mementos = new List<HistoryMemento>();
            FlipLayerVerticalAction flh = new FlipLayerVerticalAction();
            for (int i = 0; i < ActiveDocumentWorkspace.Document.Layers.Count; i++)
            {
                flh.PerformAction( mementos, i);
            }

            if (OptionalHistoryRecord == null)
            {
                HistoryMemento hm = new CompoundHistoryMemento(this.Name, null, mementos);
                ActiveDocumentWorkspace.History.PushNewMemento(hm);
            }
            else
                OptionalHistoryRecord.AddRange(mementos);

            return true;
        }

        public FlipImageVerticalAction()
/*            : base(StaticName,
                   PdnResources.GetImageResource("Icons.MenuLayersFlipVerticalIcon.png"), 
                   FlipType.Vertical,
                   layerIndex)*/
        {
        }
    }
}
