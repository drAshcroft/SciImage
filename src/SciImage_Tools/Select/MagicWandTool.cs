/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SciImage;
using SciImage.Core.History.HistoryMementos;
using SciImage.Plugins.Tools;
using SciImage.Plugins.Tools.FloodTools;
using SciImage.Core.Selection;

namespace SciImage_Tools.Select
{
    public class MagicWandTool
        : SelectionTool
    {

        private FloodCheck FloodTool;

        delegate void FillCompletedEvent(Point[][] FillRegion);
        private class FloodCheck : FloodToolBase
        {
            public FloodCheck(DocumentWorkspace workspace) :
                base(
                    workspace,
                    null,
                    "flood",
                    "",
                    ' ',
                    true,
                    0,
                    ToolBarConfigItems.Brush | ToolBarConfigItems.Antialiasing | ToolBarConfigItems.AlphaBlending)
            {

            }

            public event FillCompletedEvent FillCompleted;

            protected override void OnFillRegionComputed(Point[][] polygonSet)
            {
                if (FillCompleted != null)
                    FillCompleted(polygonSet);
            }

            public void ForceMouse(MouseEventArgs e)
            {
                OnMouseDown(e);
            }
        }

        protected override void OnActivate()
        {
            FloodTool = new FloodCheck(documentWorkspace);
            FloodTool.FillCompleted += FloodTool_FillCompleted;

            SetCursors(
              "Cursors.RectangleSelectToolCursor.cur",
              "Cursors.RectangleSelectToolCursorMinus.cur",
              "Cursors.RectangleSelectToolCursorPlus.cur",
              "Cursors.RectangleSelectToolCursorMouseDown.cur");

            //DocumentWorkspace.EnableSelectionTinting = true;
            //try
            //{
            //    this.cursorMouseUp = new Cursor(SciImage.SciResources.SciResources.GetResourceStream("Cursors.MagicWandToolCursor.cur"));
            //    this.cursorMouseUpMinus = new Cursor(SciImage.SciResources.SciResources.GetResourceStream("Cursors.MagicWandToolCursorMinus.cur"));
            //    this.cursorMouseUpPlus = new Cursor(SciImage.SciResources.SciResources.GetResourceStream("Cursors.MagicWandToolCursorPlus.cur"));
            //}
            //catch { }
            //this.Cursor = GetCursor();
            base.OnActivate();
        }

        private void FloodTool_FillCompleted(Point[][] FillRegion)
        {
            SelectionHistoryMemento undoAction = new SelectionHistoryMemento(this.Name, this.Image, this.DocumentWorkspace);

            Selection.PerformChanging();
            Selection.SetContinuation(FillRegion, this.combineMode);
            Selection.CommitContinuation();
            Selection.PerformChanged();

            HistoryStack.PushNewMemento(undoAction);
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            //this.lastButton = e.Button;
            Selection.Reset();
            this.newSelection.Reset();
            PdnGraphicsPath basePath = Selection.CreatePath();
            this.newSelection.SetContinuation(basePath, CombineMode.Replace, true);
            this.newSelection.CommitContinuation();

            bool newSelectionRendererVisible = true;

            ///            this.newSelectionRenderer.Visible = newSelectionRendererVisible;


            FloodTool.SetScratch(scratchSurface);
            FloodTool.ForceMouse(e);


        }

        public MagicWandTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciImage.SciResources.SciResources.GetImageResource("Icons.glyphicons-10-magic.png"),
                   "Magic Selection tool",
                   "Click a color to select the region around that color",
                   's',
                    ToolBarConfigItems.SelectionDrawMode)
        {
            //ClipToSelection = false;
        }
    }
}