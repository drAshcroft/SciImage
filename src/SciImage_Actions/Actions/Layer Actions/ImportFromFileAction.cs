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
using System.IO;
using System.Windows.Forms;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Surfaces.Layers;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.Actions;
using SciImage.Plugins.FileHandling;
using SciImage.Plugins.Tools.DefaultTools;
using SciImage.Plugins.Tools.Enums;
using SciImage.SciResources;
using SciImage_Actions.Actions.Image_Actions.CanvasSize;

namespace SciImage_Actions.Actions.Layer_Actions
{
    
    public sealed class ImportFromFileAction
        : PluginAction
    {
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null)
            {
                return ActionDisplayOptions.Visible;
            }
            if (documentWorkspace.ActiveLayer.GetType() == typeof(BitmapLayer))
            {
                return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
            }
            return ActionDisplayOptions.Hidden;
        }
        private static string StaticName
        {
            get
            {
                return "Import From File into Layer";
            }
        }

        private static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuLayersImportFromFileIcon.png");
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
                return (System.Windows.Forms.Keys.F9 );
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 5; }
        }

        private void Rollback(List<HistoryMemento> historyMementos)
        {
            for (int i = historyMementos.Count - 1; i >= 0; i--)
            {
                HistoryMemento ha = historyMementos[i];
                ha.PerformUndo();
            }
        }

        private HistoryMemento DoCanvasResize(DocumentWorkspace documentWorkspace, Size newLayerSize)
        {
            HistoryMemento retHA;

            int layerIndex = documentWorkspace.ActiveLayerIndex;

            Size newSize = new Size(Math.Max(newLayerSize.Width, documentWorkspace.Document.Width),
                Math.Max(newLayerSize.Height, documentWorkspace.Document.Height));

            Document newDoc;
            
            try
            {
                using (new WaitCursorChanger(documentWorkspace))
                {
                    Utility.GCFullCollect();

                    newDoc = CanvasSizeAction.ResizeDocument(documentWorkspace.Document, newSize, 
                        AnchorEdge.TopLeft, ToolEnvironment.Environment.SecondaryColor);
                }
            }

            catch (OutOfMemoryException)
            {
                Utility.ErrorBox(documentWorkspace, SciImage.SciResources.SciResources.GetString("ImportFromFileAction.AskForCanvasResize.OutOfMemory"));
                newDoc = null;
            }

            if (newDoc == null)
            {
                retHA = null;
            }
            else
            {
                retHA = new ReplaceDocumentHistoryMemento(string.Empty, null, documentWorkspace);

                using (new WaitCursorChanger(documentWorkspace))
                {
                    documentWorkspace.Document = newDoc;
                }

                documentWorkspace.ActiveLayer = (Layer)documentWorkspace.Document.Layers[layerIndex];
            }

            return retHA;
        }

        private HistoryMemento ImportOneLayer(DocumentWorkspace documentWorkspace, BitmapLayer layer)
        {
            HistoryMemento retHA;
            List<HistoryMemento> historyMementos = new List<HistoryMemento>();
            bool success = true;
            
            if (success)
            {
                if (!documentWorkspace.Selection.IsEmpty)
                {
                    List<HistoryMemento> lhm = new List<HistoryMemento>();
                    new DeselectAction().PerformAction(lhm,-1);

                    historyMementos.AddRange(lhm);
                }
            }

            if (success)
            {
                if (layer.Width > documentWorkspace.Document.Width ||
                    layer.Height > documentWorkspace.Document.Height)
                {
                    HistoryMemento ha = DoCanvasResize(documentWorkspace, layer.Size);
                
                    if (ha == null)
                    {
                        success = false;
                    }
                    else
                    {
                        historyMementos.Add(ha);
                    }
                }
            }

            if (success)
            {
                if (layer.Size != documentWorkspace.Document.Size)
                {
                    BitmapLayer newLayer;
                    
                    try
                    {
                        using (new WaitCursorChanger(documentWorkspace))
                        {
                            Utility.GCFullCollect();

                            newLayer = CanvasSizeAction.ResizeLayer((BitmapLayer)layer, documentWorkspace.Document.Size, 
                                AnchorEdge.TopLeft, layer.Surface.ColorPixelBase.WhiteColor().NewAlpha(0));
                        }
                    }

                    catch (OutOfMemoryException)
                    {
                        Utility.ErrorBox(documentWorkspace, SciImage.SciResources.SciResources.GetString("ImportFromFileAction.ImportOneLayer.OutOfMemory"));
                        success = false;
                        newLayer = null;
                    }

                    if (newLayer != null)
                    {
                        layer.Dispose();
                        layer = newLayer;
                    }
                }
            }

            if (success)
            {
                NewLayerHistoryMemento nlha = new NewLayerHistoryMemento(string.Empty, null, documentWorkspace, documentWorkspace.Document.Layers.Count);
                documentWorkspace.Document.Layers.Add(layer);
                historyMementos.Add(nlha);
            }

            if (success)
            {
                HistoryMemento[] has = historyMementos.ToArray();
                retHA = new CompoundHistoryMemento(string.Empty, null, has);
            }
            else
            {
                Rollback(historyMementos);
                retHA = null;
            }

            return retHA;
        }

        /// <summary>
        /// Presents a user interface and performs the operations required for importing an entire document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function will take ownership of the Document given to it, and will Dispose() of it.
        /// </remarks>
        private HistoryMemento ImportDocument(DocumentWorkspace documentWorkspace, Document document, out Rectangle lastLayerBounds)
        {
            List<HistoryMemento> historyMementos = new List<HistoryMemento>();
            bool[] selected;

            selected = new bool[document.Layers.Count];
            for (int i = 0; i < selected.Length; ++i)
            {
                selected[i] = true;
            }

            lastLayerBounds = Rectangle.Empty;

            if (selected != null)
            {
                List<Layer> layers = new List<Layer>();

                for (int i = 0; i < selected.Length; ++i)
                {
                    if (selected[i])
                    {
                        layers.Add((Layer)document.Layers[i]);
                    }
                }

                foreach (Layer layer in layers)
                {
                    document.Layers.Remove(layer);
                }

                document.Dispose();
                document = null;

                foreach (Layer layer in layers)
                {
                    lastLayerBounds = layer.Bounds;
                    HistoryMemento ha = ImportOneLayer(documentWorkspace, (BitmapLayer)layer);

                    if (ha != null)
                    {
                        historyMementos.Add(ha);
                    }
                    else
                    {
                        Rollback(historyMementos);
                        historyMementos.Clear();
                        break;
                    }
                }
            }

            if (document != null)
            {
                document.Dispose();
                document = null;
            }

            if (historyMementos.Count > 0)
            {
                HistoryMemento[] has = historyMementos.ToArray();
                return new CompoundHistoryMemento(string.Empty, null, has);
            }
            else
            {
                lastLayerBounds = Rectangle.Empty;
                return null;
            }
        }

        private HistoryMemento ImportOneFile(DocumentWorkspace documentWorkspace, string fileName, out Rectangle lastLayerBounds)
        {
             
                AppEnvironment.Environment.ResetProgressStatusBar();

            ProgressEventHandler progressCallback = (s, e) =>
                {
                     
                        AppEnvironment.Environment.SetProgressStatusBar(e.Percent);
                };

            FileType fileType;
            Document document = DocumentWorkspace.LoadDocument(documentWorkspace, fileName, out fileType, progressCallback);

             
                AppEnvironment.Environment.EraseProgressStatusBar();

            if (document != null)
            {
                string name = Path.ChangeExtension(Path.GetFileName(fileName), null);
                string newLayerNameFormat = SciImage.SciResources.SciResources.GetString("ImportFromFileAction.ImportOneFile.NewLayer.Format");

                foreach (Layer layer in document.Layers)
                {
                    layer.Name = string.Format(newLayerNameFormat, name, layer.Name);
                    layer.IsBackground = false;
                }

                HistoryMemento ha = ImportDocument(documentWorkspace, document, out lastLayerBounds);
                return ha;
            }
            else
            {
                lastLayerBounds = Rectangle.Empty;
                return null;
            }
        }

        public HistoryMemento ImportMultipleFiles(DocumentWorkspace documentWorkspace, string[] fileNames)
        {
            HistoryMemento retHA = null;
            List<HistoryMemento> historyMementos = new List<HistoryMemento>();
            Rectangle lastLayerBounds = Rectangle.Empty;

            foreach (string fileName in fileNames)
            {
                HistoryMemento ha = ImportOneFile(documentWorkspace, fileName, out lastLayerBounds);

                if (ha != null)
                {
                    historyMementos.Add(ha);
                }
                else
                {
                    Rollback(historyMementos);
                    historyMementos.Clear();
                    break;
                }
            }

            if (lastLayerBounds.Width > 0 && lastLayerBounds.Height > 0)
            {
                SelectionHistoryMemento sha = new SelectionHistoryMemento(null, null, documentWorkspace);
                historyMementos.Add(sha);
                documentWorkspace.Selection.PerformChanging();
                documentWorkspace.Selection.Reset();
                documentWorkspace.Selection.SetContinuation(lastLayerBounds, System.Drawing.Drawing2D.CombineMode.Replace);
                documentWorkspace.Selection.CommitContinuation();
                documentWorkspace.Selection.PerformChanged();
            }

            if (historyMementos.Count > 0)
            {
                HistoryMemento[] haArray = historyMementos.ToArray();
                retHA = new CompoundHistoryMemento(StaticName, StaticImage, haArray);
            }

            return retHA;
        }

        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            string[] fileNames;
            string startingDir = Path.GetDirectoryName(documentWorkspace.FilePath);
            DialogResult result = DocumentWorkspace.ChooseFiles(documentWorkspace, out fileNames, true, startingDir);
            HistoryMemento retHA = null;

            if (result == DialogResult.OK)
            {
                Type oldToolType = documentWorkspace.GetToolType();
                documentWorkspace.SetTool(null);

                retHA = ImportMultipleFiles(documentWorkspace, fileNames);

                Type newToolType;
                if (retHA != null)
                {
                    if (OptionalHistoryRecord == null)
                    {
                        CompoundHistoryMemento cha = new CompoundHistoryMemento(StaticName, StaticImage, new HistoryMemento[] { retHA });
                        retHA = cha;
                    }
                    else
                        OptionalHistoryRecord.Add(retHA);
                    newToolType = typeof(MoveTool);
                }
                else
                {
                    newToolType = oldToolType;
                }

                documentWorkspace.SetToolFromType(newToolType);
            }
            if (OptionalHistoryRecord == null)
            {
                documentWorkspace.History.PushNewMemento(retHA);
            }
            return true ;
        }

        public ImportFromFileAction()
            : base(ActionFlags.KeepToolActive)
        {
            // We use ActionFlags.KeepToolActive because opening this dialog does not necessitate
            // refreshing the tool. This is handled by PerformAction() as appropriate.
            // The tool should only be changed if the action is performed, but not if the dialog
            // is cancelled out of.
        }
    }
}
