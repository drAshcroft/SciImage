/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using SciImage.Core.History.HistoryMementos;
using SciImage.SciResources;

namespace SciImage.Plugins.Actions
{
    public sealed class DeselectAction
        : Actions.PluginAction 
    {
        public static string StaticName
        {
            get
            {
                return "Deselect"; 
            }
        }

        public static ImageResource StaticImage
        {
            get
            {
                return SciResources.SciResources.GetImageResource("Icons.MenuEditDeselectIcon.png");
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 3; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 5; }
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
                return false ;
            }
            else
            {
                SelectionHistoryMemento sha = new SelectionHistoryMemento(StaticName, StaticImage, docWorkspace);
                if (OptionalHistoryRecord == null)
                {
                    docWorkspace.History.PushNewMemento(sha);
                }
                else
                    OptionalHistoryRecord.Add(sha);
                //EnterCriticalRegion();
                docWorkspace.Selection.Reset();

                return true ;
            }
        }

        public DeselectAction()
            : base(ActionFlags.None)
        {
        }
    }
}
