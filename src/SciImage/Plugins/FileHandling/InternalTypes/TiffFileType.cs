using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FreeImageAPI;
using SciImage.Core;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;
using SciImage.Core.Surfaces.Layers;
using SciImage.Core.Surfaces.Layers.BitmapLayers;
using SciImage.Core.Surfaces.Layers.IntensityLayers;
using SciImage.PaintForms.UserControls.ProgressBars;
using SciImage.Plugins.FileHandling.FileTypeBase;
using SciImage.Plugins.FileHandling.PropertyBasedFileTypes;
using SciImage.SystemLayer.Base;

namespace SciImage.Plugins.FileHandling.InternalTypes
{
    public sealed class TiffFileType
       : FileType
    {
        public TiffFileType()
            : base("Tiff File",
                       FileTypeFlags.SupportsLoading |
                       FileTypeFlags.SupportsLayers |
                       FileTypeFlags.SupportsSaving,
                   new string[] { ".tif",".tiff" })
        {
        }

        /// <summary>
        /// Returns a freeimage bitmap for manipulation (Mostly for internal use)
        /// </summary>
        /// <returns></returns>
        public FreeImageBitmap MakeRawFIBITMAP(Surface  data)
        {
            FIBITMAP dib;
            int _BitsPerPixel;
            if (data.ColorPixelBase is ColorIntensity)
                _BitsPerPixel = 16;
            else
                _BitsPerPixel = 32;
            
            int _Width =data.Width ;
            int _Height = data.Height;

            if (_BitsPerPixel == 16)
            {
                dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_UINT16, _Width, _Height, 16, 0, 0, 0);
                for (int y = 0; y < _Height; y++)
                {
                    System.IntPtr scanline = FreeImage.GetScanLine(dib, y);
                    unsafe
                    {
                        ushort* bits = (ushort*)(void*)scanline;
                        
                        for (int x = 0; x < _Width; x++)
                        {
                            ushort gray = ((ColorIntensity)data[x, y, data.ColorPixelBase]).Intensity;
                            bits[x] = gray;
                        }
                    }
                }
            }
            else if (_BitsPerPixel == 8)
            {
                dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_BITMAP , _Width, _Height, 8, 0, 0, 0);
                
                    Palette p = FreeImage.GetPaletteEx(dib);
                    for (int i = 0; i < 256; i++)
                    {
                        p[i] = new RGBQUAD(Color.FromArgb(i, i, i));
                    }
            }
            else 
            {
               
                dib = FreeImage.AllocateT(FREE_IMAGE_TYPE.FIT_BITMAP , _Width, _Height, 32,FreeImage.FI_RGBA_RED_MASK, FreeImage.FI_RGBA_GREEN_MASK , FreeImage.FI_RGBA_BLUE_MASK  );
                for (int y = 0; y < _Height; y++)
                {
                    System.IntPtr scanline = FreeImage.GetScanLine(dib, y);
                    unsafe
                    {
                        Int32 * bits = (Int32 *)(void*)scanline;

                        for (int x = 0; x < _Width; x++)
                        {
                            Int32  gray = ((ColorBgra)data[x, y, data.ColorPixelBase]).ToInt32();
                            bits[x] = gray;
                        }
                    }
                }
            }
            return  new FreeImageBitmap(dib, "");
           

        }

        public bool saveToStream( Stream output,  Document  pImageBuf, int width, int height)

        {
            string szFilename=Path.GetDirectoryName( Application.ExecutablePath) + "\\" + Path.GetFileNameWithoutExtension(  Path.GetTempFileName() ) + ".tif" ;
           
                FreeImageBitmap test = MakeRawFIBITMAP( ((Layer) pImageBuf.Layers[0]).Surface );
                
                    test.Save(output, FREE_IMAGE_FORMAT.FIF_TIFF);
               
            return true;
        }

        protected override Document OnLoad(Stream input)
        {
            FreeImageBitmap  InternalFreeImage = new FreeImageBitmap(input);

            int _Width =  InternalFreeImage.Width;
            int _Height =  InternalFreeImage.Height;
            int  _BitsPerPixel = InternalFreeImage.ColorDepth;


            Document doc = new Document(_Width, _Height);
            doc.DpuUnit = MeasurementUnit.Inch;
            doc.DpuX = 100;// image.HorizontalResolution;
            doc.DpuY = 100;// image.VerticalResolution;
            if (_BitsPerPixel == 16)
            {
                for (int i = 0; i < InternalFreeImage.FrameCount; i++)
                {
                    InternalFreeImage.SelectActiveFrame(i);
                    IntPtr Data = InternalFreeImage.Bits;
                    int _lSize = InternalFreeImage.DataSize;
                    int _Stride = InternalFreeImage.Pitch;

                    Surface surf = new Surface(new Size(_Width, _Height), new ColorIntensity());
                    ColorIntensity ci=new ColorIntensity();
                    unsafe
                    {
                        for (int y = 0; y < _Height; y++)
                        {
                            ushort* inData = (ushort*)((byte*)Data + (y * _Stride));
                            for (int x = 0; x < _Width; x++)
                            {
                               ushort gray = inData[x];
                               ci.Intensity = gray;
                               ci.alpha = 255;
                                try
                                {
                                    surf[x, y] =  ci.ToInt32();
                                }
                                catch 
                                {
                                    System.Diagnostics.Debug.Print("");
                                }
                            }
                        }

                        doc.Layers.Add(IntensityLayer.FromImage(surf));
                    }
                }
            }
            else
            {
                for (int i = 0; i < InternalFreeImage.FrameCount; i++)
                {
                    InternalFreeImage.SelectActiveFrame(i);
                    IntPtr Data = InternalFreeImage.Bits;
                    int _lSize = InternalFreeImage.DataSize;
                    int _Stride = InternalFreeImage.Pitch;

                    Surface surf = new Surface(new Size(_Width, _Height), new ColorBgra());
                    
                    unsafe
                    {
                        for (int y = 0; y < _Height; y++)
                        {
                            Int32 * inData = (Int32 *)((byte*)Data + (y * _Stride));
                            for (int x = 0; x < _Width; x++)
                            {
                                Int32  gray = inData[x];
                                
                                surf[x, y] = gray ;
                            }
                        }

                        doc.Layers.Add(BitmapLayer.FromImage(surf));
                    }
                }
            }
            return doc;
        }

        protected override void OnSave(Document input, Stream output, SaveConfigToken token, Surface scratchSurface, ProgressEventHandler callback)
        {
            
                saveToStream(output, input , input.Width, input.Height);
          

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
