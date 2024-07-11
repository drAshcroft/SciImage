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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SciImage.Core;
using SciImage.Core.Surfaces;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.FileHandling.FileTypeBase;
using SciImage.Plugins.FileHandling.PropertyBasedFileTypes;
using SciImage.SystemLayer.Base;
using SciImage.SystemLayer.Base.PropertySystem;

namespace SciImage.Plugins.FileHandling.InternalTypes
{
    public sealed class GifFileType
        : InternalFileType
    {
        public enum PropertyNames
        {
            Threshold = 0,
            DitherLevel = 1
        }

        public GifFileType()
            : base("GIF", FileTypeFlags.SupportsLoading | FileTypeFlags.SupportsSaving | FileTypeFlags.SavesWithProgress, new string[] { ".gif" })
        {
        }

        public override PropertyCollection OnCreateSavePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property(PropertyNames.DitherLevel, 7, 0, 8));
            props.Add(new Int32Property(PropertyNames.Threshold, 128, 0, 255));

            return new PropertyCollection(props);
        }

        public override int GetThresholdFromToken(PropertyBasedSaveConfigToken token)
        {
            int threshold = token.GetProperty<Int32Property>(PropertyNames.Threshold).Value;
            return threshold;
        }

        public override int GetDitherLevelFromToken(PropertyBasedSaveConfigToken token)
        {
            int ditherLevel = token.GetProperty<Int32Property>(PropertyNames.DitherLevel).Value;
            return ditherLevel;
        }

        public override Set<SavableBitDepths> CreateAllowedBitDepthListFromToken(PropertyBasedSaveConfigToken token)
        {
            var bitDepths = new Set<SavableBitDepths>();

            bitDepths.Add(SavableBitDepths.Rgb8);
            bitDepths.Add(SavableBitDepths.Rgba8);

            return bitDepths;
        }

        protected override PropertyBasedSaveConfigWidget OnCreateSaveConfigWidgetT()
        {
            throw new NotImplementedException();
        }
        //public override ControlInfo OnCreateSaveConfigUI(PropertyCollection props)
        //{
        //    ControlInfo configUI = CreateDefaultSaveConfigUI(props);

        //    configUI.SetPropertyControlValue(
        //        PropertyNames.DitherLevel,
        //        ControlInfoPropertyNames.DisplayName,"DitherLevel");

        //    configUI.SetPropertyControlValue(
        //        PropertyNames.Threshold,
        //        ControlInfoPropertyNames.DisplayName,"Threshold");

        //    configUI.SetPropertyControlValue(
        //        PropertyNames.Threshold,
        //        ControlInfoPropertyNames.Description,"Threshold");

        //    return configUI;
        //}

        protected override Document OnLoad(Stream input)
        {
            using (Image image = SciResources.SciResources.LoadImage(input))
            {
                Document document = Document.FromImage(image);
                return document;
            }
        }

        public override void FinalSave(
            Document input, 
            Stream output, 
            Surface scratchSurface, 
            int ditherLevel, 
            SavableBitDepths bitDepth,
            PropertyBasedSaveConfigToken token,
            ProgressEventHandler progressCallback)
        {
            bool enableAlpha;

            switch (bitDepth)
            {
                case SavableBitDepths.Rgb8:
                    enableAlpha = false;
                    break;

                case SavableBitDepths.Rgba8:
                    enableAlpha = true;
                    break;

                default:
                    throw new InvalidEnumArgumentException("bitDepth", (int)bitDepth, typeof(SavableBitDepths));
            }

            using (Bitmap quantized = Quantize(scratchSurface, ditherLevel, 256, enableAlpha, progressCallback))
            {
                quantized.Save(output, ImageFormat.Gif);
            }
        }
    }
}
