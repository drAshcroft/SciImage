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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.Edit_Actions
{
    public sealed class PasteInToNewImageAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "Paste in New Image";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V | System.Windows.Forms.Keys.Alt );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 2; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 4; }
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
            try
            {
                IDataObject pasted;
                Image image;

                using (new WaitCursorChanger())
                {
                    Utility.GCFullCollect();
                    pasted = Clipboard.GetDataObject();
                    image = (Image)pasted.GetData(DataFormats.Bitmap);
                }

                if (image == null)
                {
                    Utility.ErrorBox(FormsManager.BaseForm, SciImage.SciResources.SciResources.GetString("PasteInToNewImageAction.Error.NoClipboardImage"));
                }
                else
                {
                    Size newSize = image.Size;
                    image.Dispose();
                    image = null;
                    pasted = null;

                    Document document = null;

                    using (new WaitCursorChanger())
                    {
                        document = new Document(newSize);
                        DocumentWorkspace dw = DocumentManager.Manager.AddNewDocumentWorkspace();
                        dw.Document = document;

                        dw.History.PushNewMemento(new NullHistoryMemento(string.Empty, null));

                        PasteInToNewLayerAction pitnla = new PasteInToNewLayerAction();
                        bool result = pitnla.PerformAction(OptionalHistoryRecord,TargetLayerIndex  );

                        if (result)
                        {
                            dw.Selection.Reset();
                            dw.SetDocumentSaveOptions(null, null, null);
                            dw.History.ClearAll();

                            dw.History.PushNewMemento(
                                new NullHistoryMemento(
                                    "New Image",
                                    SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersAddNewLayerIcon.png")));

                            DocumentManager.Manager.ActiveDocumentWorkspace = dw;
                        }
                        else
                        {
                            DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                            document.Dispose();
                        }
                    }
                }
            }

            catch (ExternalException)
            {
                Utility.ErrorBox(FormsManager.BaseForm, SciImage.SciResources.SciResources.GetString("AcquireImageAction.Error.Clipboard.TransferError"));
                return false ;
            }

            catch (OutOfMemoryException)
            {
                Utility.ErrorBox(FormsManager.BaseForm, SciImage.SciResources.SciResources.GetString("AcquireImageAction.Error.Clipboard.OutOfMemory"));
                return false ;
            }

            catch (ThreadStateException)
            {
                // The ApartmentState property of the application is not set to ApartmentState.STA
                // I don't think this one will ever happen, seeing as how Main is tagged with the
                // STA attribute.
                return false ;
            }
            return true ;
        }
    }
}
