/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Drawing.Drawing2D;
using SciImage;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Selection;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.Edit_Actions
{
    public sealed class InvertSelectionAction
        : PluginAction 
    {
        public static string StaticName
        {
            get
            {
                return "Invert Selection";
            }
        }

        public static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuEditInvertSelectionIcon.png");
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 3; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 3; }
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
            if (historyWorkspace.Selection.IsEmpty)
            {
                return false ;
            }
            else
            {
                SelectionHistoryMemento sha = new SelectionHistoryMemento(
                    StaticName,
                    StaticImage,
                    historyWorkspace);

                //PdnGraphicsPath selectedPath = historyWorkspace.Selection.GetPathReadOnly();
                PdnGraphicsPath selectedPath = historyWorkspace.Selection.CreatePath();

                PdnGraphicsPath boundsOutline = new PdnGraphicsPath();
                boundsOutline.AddRectangle(historyWorkspace.Document.Bounds);

                PdnGraphicsPath clippedPath = PdnGraphicsPath.Combine(selectedPath, CombineMode.Intersect, boundsOutline);
                PdnGraphicsPath invertedPath = PdnGraphicsPath.Combine(clippedPath, CombineMode.Xor, boundsOutline);

                selectedPath.Dispose();
                selectedPath = null;

                clippedPath.Dispose();
                clippedPath = null;

                //EnterCriticalRegion();
                historyWorkspace.Selection.PerformChanging();
                historyWorkspace.Selection.Reset();
                historyWorkspace.Selection.SetContinuation(invertedPath, CombineMode.Replace, true);
                historyWorkspace.Selection.CommitContinuation();
                historyWorkspace.Selection.PerformChanged();

                boundsOutline.Dispose();
                boundsOutline = null;
                if (OptionalHistoryRecord == null)
                    historyWorkspace.History.PushNewMemento(sha);
                else
                    OptionalHistoryRecord.Add(sha);
                return true ;
            }
        }
 
        public InvertSelectionAction()
            : base(ActionFlags.None)
        {
        }
    }
}
