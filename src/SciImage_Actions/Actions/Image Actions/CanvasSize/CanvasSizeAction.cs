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
using System.Windows.Forms;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps;
using SciImage.Core.Surfaces.Layers;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Plugins.Actions;
using SciImage.Plugins.Tools;
using SciImage.Plugins.Tools.Enums;
using SciImage.SciResources;
using SciImage.SystemLayer.System;

namespace SciImage_Actions.Actions.Image_Actions.CanvasSize
{
    /// <summary>
    /// There are two ways to use this action:
    /// 1. Through the normal "PerformAction" interface provided through DoucmentAction
    /// 2. Through the ResizeCanvas static method
    /// </summary>
    // TODO: split in to Action and Function
    public sealed class CanvasSizeAction
        : PluginAction
    {
        public static string StaticName
        {
            get
            {
                return "Canvas Size";
            }
        }

        public static ImageResource StaticImage
        {
            get
            {
                return SciImage.SciResources.SciResources.GetImageResource("Icons.MenuImageCanvasSizeIcon.png");
            }
        }

        public override string Name
        {
            get
            {
                return StaticName;
            }
        }
        public override System.Drawing.Image Image
        {
            get { return StaticImage.Reference ; }
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
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 2; }
        }
        public static BitmapLayer ResizeLayer(BitmapLayer layer, Size newSize, AnchorEdge anchor, ColorPixelBase background)
        {
            BitmapLayer newLayer = new BitmapLayer(newSize.Width, newSize.Height,layer.Surface.ColorPixelBase );

            // Background
            new Constant(background).Apply(newLayer.Surface, newLayer.Surface.Bounds);

            // non-background = clear the alpha channel (see-through)
            if (!layer.IsBackground)
            {
                new SetAlphaChannel(0).Apply(newLayer.Surface, newLayer.Surface.Bounds);
            }

            int topY = 0;
            int leftX = 0;
            int rightX = newSize.Width - layer.Width;
            int bottomY = newSize.Height - layer.Height;
            int middleX = (newSize.Width - layer.Width) / 2;
            int middleY = (newSize.Height - layer.Height) / 2;

            int x = 0;
            int y = 0;

            #region choose x,y from AnchorEdge
            switch (anchor)
            {
                case AnchorEdge.TopLeft:
                    x = leftX;
                    y = topY;
                    break;

                case AnchorEdge.Top:
                    x = middleX;
                    y = topY;
                    break;

                case AnchorEdge.TopRight:
                    x = rightX;
                    y = topY;
                    break;

                case AnchorEdge.Left:
                    x = leftX;
                    y = middleY;
                    break;

                case AnchorEdge.Middle:
                    x = middleX;
                    y = middleY;
                    break;

                case AnchorEdge.Right:
                    x = rightX;
                    y = middleY;
                    break;

                case AnchorEdge.BottomLeft:
                    x = leftX;
                    y = bottomY;
                    break;

                case AnchorEdge.Bottom:
                    x = middleX;
                    y = bottomY;
                    break;

                case AnchorEdge.BottomRight:
                    x = rightX;
                    y = bottomY;
                    break;
            }
            #endregion

            newLayer.Surface.CopySurface(layer.Surface, new Point(x, y));
            newLayer.LoadProperties(layer.SaveProperties());
            return newLayer;
        }

        public static Document ResizeDocument(Document document, Size newSize, AnchorEdge edge, ColorPixelBase background)
        {
            Document newDoc = new Document(newSize.Width, newSize.Height);
            newDoc.ReplaceMetaDataFrom(document);

            for (int i = 0; i < document.Layers.Count; ++i)
            {
                Layer layer = (Layer)document.Layers[i];

                if (layer is BitmapLayer)
                {
                    Layer newLayer;

                    try
                    {
                        newLayer = ResizeLayer((BitmapLayer)layer, newSize, edge, background);
                    }

                    catch (OutOfMemoryException)
                    {
                        newDoc.Dispose();
                        throw;
                    }

                    newDoc.Layers.Add(newLayer);
                }
                else
                {
                    throw new InvalidOperationException("Canvas Size does not support Layers that are not BitmapLayers");
                }
            }
                    
            return newDoc;
        }

        // returns null to indicate user cancelled, or if initialNewSize = newSize that the user requested, 
        // or if there was an error (out of memory)
        public static Document ResizeDocument(IWin32Window parent, 
                                              Document document, 
                                              Size initialNewSize, 
                                              AnchorEdge initialAnchor, 
                                              ColorPixelBase background,
                                              bool loadAndSaveMaintainAspect,
                                              bool saveAnchor)
        {
            using (CanvasSizeDialog csd = new CanvasSizeDialog())
            {
                bool maintainAspect;
                
                if (loadAndSaveMaintainAspect)
                {
                    maintainAspect = Settings.CurrentUser.GetBoolean(SettingNames.LastMaintainAspectRatioCS, false);
                }
                else
                {
                    maintainAspect = false;
                }

                csd.OriginalSize = document.Size;
                csd.OriginalDpuUnit = document.DpuUnit;
                csd.OriginalDpu = document.DpuX;
                csd.ImageWidth = initialNewSize.Width;
                csd.ImageHeight = initialNewSize.Height;
                csd.LayerCount = document.Layers.Count;
                csd.AnchorEdge = initialAnchor;
                csd.Units = csd.OriginalDpuUnit;
                csd.Resolution = document.DpuX;
                csd.Units = SettingNames.GetLastNonPixelUnits();
                csd.ConstrainToAspect = maintainAspect;

                DialogResult result = csd.ShowDialog(parent);
                Size newSize = new Size(csd.ImageWidth, csd.ImageHeight);
                MeasurementUnit newDpuUnit = csd.Units;
                double newDpu = csd.Resolution;

                // If they cancelled, get out
                if (result == DialogResult.Cancel)
                {
                    return null;
                }

                // If they clicked OK, then we save the aspect checkbox, and maybe the anchor
                if (loadAndSaveMaintainAspect)
                {
                    Settings.CurrentUser.SetBoolean(SettingNames.LastMaintainAspectRatioCS, csd.ConstrainToAspect);
                }

                if (saveAnchor)
                {
                    Settings.CurrentUser.SetString(SettingNames.LastCanvasSizeAnchorEdge, csd.AnchorEdge.ToString());
                }

                if (newSize == document.Size && newDpuUnit == document.DpuUnit && newDpu == document.DpuX)
                {
                    return null;
                }

                try
                {
                    Utility.GCFullCollect();
                    Document newDoc = ResizeDocument(document, newSize, csd.AnchorEdge, background);
                    newDoc.DpuUnit = newDpuUnit;
                    newDoc.DpuX = newDpu;
                    newDoc.DpuY = newDpu;
                    return newDoc;
                }

                catch (OutOfMemoryException)
                {
                    Utility.ErrorBox(parent, SciImage.SciResources.SciResources.GetString("CanvasSizeAction.ResizeDocument.OutOfMemory"));
                    return null;
                }

                catch
                {
                    return null;
                }
            }
        }
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
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            DocumentWorkspace documentWorkspace = ActiveDocumentWorkspace;
            AnchorEdge initialEdge = SettingNames.GetLastCanvasSizeAnchorEdge();

            Document newDoc = ResizeDocument(
                documentWorkspace.FindForm(),
                documentWorkspace.Document, 
                documentWorkspace.Document.Size, 
                initialEdge,
                ToolEnvironment.Environment.SecondaryColor, 
                true, 
                true);

            if (newDoc != null)
            {
                using (new PushNullToolMode(documentWorkspace))
                {
                    if (newDoc.DpuUnit != MeasurementUnit.Pixel)
                    {
                        Settings.CurrentUser.SetString(SettingNames.LastNonPixelUnits, newDoc.DpuUnit.ToString());

                        if (AppEnvironment.Environment.Units != MeasurementUnit.Pixel)
                        {
                            AppEnvironment.Environment.Units = newDoc.DpuUnit;
                        }
                    }

                    ReplaceDocumentHistoryMemento rdha = new ReplaceDocumentHistoryMemento(StaticName, StaticImage, documentWorkspace);
                    if (OptionalHistoryRecord == null)
                        documentWorkspace.History.PushNewMemento(rdha);
                    else
                        OptionalHistoryRecord.Add(rdha);

                    documentWorkspace.Document = newDoc;
                    return true;
                }
            }
            else
            {
                return false ;
            }
        }

        public CanvasSizeAction()
            : base(ActionFlags.KeepToolActive)
        {
            // We use ActionFlags.KeepToolActive because opening this dialog does not necessitate
            // refreshing the tool. This is handled by PerformAction() as appropriate.
        }
    }
}
