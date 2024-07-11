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
using System.Drawing.Imaging;
using System.IO;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Renderer;
using SciImage.Core.Surfaces;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;
using SciImage.Plugins.Tools;
using SciImage.SystemLayer.Dialogs.FileDialogs;
using SciImage.SystemLayer.ScanningAndCapture;
using SciImage.SystemLayer.System;

namespace SciImage_Actions.Actions.File_Actions
{
    public sealed class PrintAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "Print";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return "File"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }

        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P);
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
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            if (!ScanningAndPrinting.CanPrint)
            {
                Utility.ShowWiaError(documentWorkspace);
                return false ;
            }

            using (new PushNullToolMode(documentWorkspace))
            {
                // render image to a bitmap, save it to disk
                Surface scratch = documentWorkspace.BorrowScratchSurface(this.GetType().Name + ".PerformAction()");

                try
                {
                    scratch.Clear();
                    RenderArgs ra = new RenderArgs(scratch);

                    documentWorkspace.Update();

                    using (new WaitCursorChanger(documentWorkspace))
                    {
                        ra.Surface.Clear(scratch.ColorPixelBase.WhiteColor());
                        documentWorkspace.Document.Render(ra, false);
                    }

                    string tempName = Path.GetTempFileName() + ".bmp";
                    ra.Bitmap.Save(tempName, ImageFormat.Bmp);

                    try
                    {
                        ScanningAndPrinting.Print(documentWorkspace, tempName);
                    }

                    catch (Exception ex)
                    {
                        Utility.ShowWiaError(documentWorkspace);
                        Tracing.Ping(ex.ToString());
                        // TODO: do a "better" error dialog here
                    }

                    // Try to delete the temp file but don't worry if we can't
                    bool result = FileSystem.TryDeleteFile(tempName);
                }

                finally
                {
                    documentWorkspace.ReturnScratchSurface(scratch);
                }
            }

            return true ;
        }

        public PrintAction()
            : base(ActionFlags.KeepToolActive)
        {
        }
    }
}
