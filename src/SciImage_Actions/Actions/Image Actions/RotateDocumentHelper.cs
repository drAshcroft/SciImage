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
using System.ComponentModel;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.Layers;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.Image_Actions
{
    public class RotateDocumentHelper
    {
       
        public enum RotateType
        {
            Clockwise90,
            CounterClockwise90,
            Rotate180,
        }

        public HistoryMemento OnExecute(DocumentWorkspace historyWorkspace,RotateType rotation,int TargetLayerIndex)
        {
            int newWidth;
            int newHeight;

            // Get new width and Height
            switch (rotation)
            {
                case RotateType.Clockwise90:
                case RotateType.CounterClockwise90:
                    newWidth = historyWorkspace.Document.Height;
                    newHeight = historyWorkspace.Document.Width;
                    break;

                case RotateType.Rotate180:
                    newWidth = historyWorkspace.Document.Width;
                    newHeight = historyWorkspace.Document.Height;
                    break;

                default:
                    throw new InvalidEnumArgumentException("invalid RotateType");
            }

            // Figure out which icon and text to use
            string iconResName;
            string suffix;

            switch (rotation)
            {
                case RotateType.Rotate180:
                    iconResName = "Icons.MenuImageRotate180Icon.png";
                    suffix = SciImage.SciResources.SciResources.GetString("RotateAction.180");
                    break;

                case RotateType.Clockwise90:
                    iconResName = "Icons.MenuImageRotate90CWIcon.png";
                    suffix = SciImage.SciResources.SciResources.GetString("RotateAction.90CW");
                    break;

                case RotateType.CounterClockwise90:
                    iconResName = "Icons.MenuImageRotate90CCWIcon.png";
                    suffix = SciImage.SciResources.SciResources.GetString("RotateAction.90CCW");
                    break;

                default:
                    throw new InvalidEnumArgumentException("invalid RotateType");
            }

            // Initialize the new Doc
            string haNameFormat = SciImage.SciResources.SciResources.GetString("RotateAction.HistoryMementoName.Format");
            string haName = string.Format(haNameFormat, "Rotations", suffix);
            ImageResource haImage = SciImage.SciResources.SciResources.GetImageResource(iconResName);

            List<HistoryMemento> actions = new List<HistoryMemento>();

            // do the memory allocation now: if this fails, we can still bail out cleanly
            Document newDoc = new Document(newWidth, newHeight);

            if (!historyWorkspace.Selection.IsEmpty)
            {
                DeselectAction da = new DeselectAction();
                da.PerformAction( actions, TargetLayerIndex);
                
            }

            ReplaceDocumentHistoryMemento rdha = new ReplaceDocumentHistoryMemento(null, null, historyWorkspace);
            actions.Add(rdha);

            newDoc.ReplaceMetaDataFrom(historyWorkspace.Document);

            // TODO: serialize oldDoc to disk, and let the GC purge it if needed
            //OnProgress(0.0);

            for (int i = 0; i < historyWorkspace.Document.Layers.Count; ++i)
            {
                Layer layer = historyWorkspace.Document.Layers.GetAt(i);

                double progressStart = 100.0 * ((double)i / (double)historyWorkspace.Document.Layers.Count);
                double progressEnd = 100.0 * ((double)(i + 1) / (double)historyWorkspace.Document.Layers.Count);

                if (layer is BitmapLayer)
                {
                    Layer nl = RotateLayer((BitmapLayer)layer, rotation, newWidth, newHeight, progressStart, progressEnd);
                    newDoc.Layers.Add(nl);
                }
                else
                {
                    throw new InvalidOperationException("Cannot Rotate non-BitmapLayers");
                }

                
            }

            CompoundHistoryMemento chm = new CompoundHistoryMemento(
                haName,
                haImage,
                actions);

            historyWorkspace.Document = newDoc;
            return chm;
        }

        private BitmapLayer RotateLayer(Layer layer, RotateType rotationType, int width, int height, double startProgress, double endProgress)
        {
            Surface surface = new Surface(width, height,layer.Surface.ColorPixelBase );

            if (rotationType == RotateType.Rotate180)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        surface[x, y] = layer.Surface[width - x - 1, height - y - 1];
                    }

                    //OnProgress(((double)y / (double)height) * (endProgress - startProgress) + startProgress);
                }
            }
            else if (rotationType == RotateType.CounterClockwise90)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        surface[x, y] = layer.Surface[height - y - 1, x];
                    }

                    //OnProgress(((double)y / (double)height) * (endProgress - startProgress) + startProgress);
                }
            }
            else if (rotationType == RotateType.Clockwise90)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        surface[x, y] = layer.Surface[y, width - 1 - x];
                    }

                    //OnProgress(((double)y / (double)height) * (endProgress - startProgress) + startProgress);
                }
            }
           
            BitmapLayer returnMe = new BitmapLayer(surface, true);
            returnMe.LoadProperties(layer.SaveProperties());
            return returnMe;
        }

       public RotateDocumentHelper()
            
        {
            
        }
    }
}
