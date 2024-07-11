using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SciImage.Core;
using SciImage.Core.Surfaces;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.FileHandling.FileTypeBase;
using SciImage.Plugins.FileHandling.PropertyBasedFileTypes;
using SciImage.SystemLayer.Base;

namespace SciImage.Plugins.FileHandling.InternalTypes
{
    public sealed class JpgFileType
       : FileType
    {
        public JpgFileType()            : base("Jpeg File",

                       FileTypeFlags.SupportsCustomHeaders |
                       FileTypeFlags.SupportsLoading |
                       FileTypeFlags.SupportsSaving,
                   new string[] { ".jpg",".jpeg" })
        {
        }

        protected override Document OnLoad(Stream input)
        {
            return GdiPlusFileType.OnLoadImage(input);
        }

        protected override void OnSave(Document input, Stream output, SaveConfigToken token, Surface scratchSurface, ProgressEventHandler callback)
        {
            

            ImageCodecInfo icf = GdiPlusFileType.GetImageCodecInfo(ImageFormat.Jpeg );
            EncoderParameters parms = new EncoderParameters(1);
            EncoderParameter parm = new EncoderParameter(Encoder.ColorDepth, 32);
            parms.Param[0] = parm;

            using (Bitmap bitmap = CreateAliased32BppBitmap(scratchSurface))
            {
                GdiPlusFileType.LoadProperties(bitmap, input);
                bitmap.Save(output, icf, parms);
            }

        }

        public override bool IsReflexive(SaveConfigToken token)
        {
            return true;
        }

        private sealed class UpdateProgressTranslator
        {
            private long maxBytes;
            private long totalBytes;
            private ProgressEventHandler callback;

            public void IOEventHandler(object sender, IOEventArgs e)
            {
                double percent;

                lock (this)
                {
                    totalBytes += (long)e.Count;
                    percent = Math.Max(0.0, Math.Min(100.0, ((double)totalBytes * 100.0) / (double)maxBytes));
                }

                callback(sender, new ProgressEventArgs(percent));
            }

            public UpdateProgressTranslator(long maxBytes, ProgressEventHandler callback)
            {
                this.maxBytes = maxBytes;
                this.callback = callback;
                this.totalBytes = 0;
            }
        }
    }
}
