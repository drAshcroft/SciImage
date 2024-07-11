/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Threading;

namespace SciImage
{
    [Serializable]
    public class IntensityLayer
        : Layer,
          IDeserializationCallback
    {

        public override Surface RenderThumbnail(int maxEdgeLength)
        {
            Size thumbSize = Utility.ComputeThumbnailSize(this.Size, maxEdgeLength);
            Surface thumb = new Surface(thumbSize);

            thumb.SuperSamplingFitSurface(this.Intensity_Surface );

            Surface thumb2 = new Surface(thumbSize);
            thumb2.ClearWithCheckboardPattern();
            UserBlendOps.NormalBlendOp nbop = new UserBlendOps.NormalBlendOp();
            nbop.Apply(thumb2, thumb);

            thumb.Dispose();
            thumb = null;

            return thumb2;
        }

        private bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                try
                {
                    if (disposing)
                    {
                        
                        if (Intensity_Surface != null)
                        {
                            Intensity_Surface.Dispose();
                            Intensity_Surface = null;
                        }
                    }
                }

                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// Creates a layer from an Image
        /// </summary>
        /// <param name="image">The Image to make a copy of that will be the first layer ("Background") in the document.</param>
        public static IntensityLayer FromImage(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("IntensityLayer FromImage");
            }

            IntensityLayer layer = new IntensityLayer(image.Width, image.Height, ColorBgra.White);


            Bitmap asBitmap = image as Bitmap;

            throw new Exception("need to create intensity suface here");
            

            return layer;
        }


        [NonSerialized]
        private BinaryPixelOp compiledBlendOp = null;

        private void CompileBlendOp()
        {
            bool isDefaultOp = (properties.blendOp.GetType() == UserBlendOps.GetDefaultBlendOp());

            if (this.Opacity == 255)
            {
                this.compiledBlendOp = properties.blendOp;
            }
            else
            {
                this.compiledBlendOp = properties.blendOp.CreateWithOpacity(this.Opacity);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            compiledBlendOp = null;
            base.OnPropertyChanged(propertyName);
        }

        [Serializable]
        public sealed class IntensityLayerProperties
            : ICloneable,
              ISerializable
        {
            public UserBlendOp blendOp;
            public int opacity; // this is ONLY used when loading older version PDN files! should normally equal -1

            private const string blendOpTag = "blendOp";
            private const string opacityTag = "opacity";

            public static string BlendOpName
            {
                get
                {
                    return PdnResources.GetString("BitmapLayer.Properties.BlendOp.Name");
                }
            }

            public IntensityLayerProperties(UserBlendOp blendOp)
            {
                this.blendOp = blendOp;
                this.opacity = -1;
            }

            public IntensityLayerProperties(IntensityLayerProperties cloneMe)
            {
                this.blendOp = cloneMe.blendOp;
                this.opacity = -1;
            }

            public object Clone()
            {
                return new IntensityLayerProperties(this);
            }

            public IntensityLayerProperties(SerializationInfo info, StreamingContext context)
            {
                this.blendOp = (UserBlendOp)info.GetValue(blendOpTag, typeof(UserBlendOp));

                // search for 'opacity' and load it if it exists
                this.opacity = -1;

                foreach (SerializationEntry entry in info)
                {
                    if (entry.Name == opacityTag)
                    {
                        this.opacity = (int)((byte)entry.Value);
                        break;
                    }
                }
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(blendOpTag, this.blendOp);
            }
        }

        private IntensityLayerProperties properties;
        private IntensitySurface2 Intensity_Surface;
        

        public override object SaveProperties()
        {
            
            if (disposed)
            {
                throw new ObjectDisposedException("IntensityLayer");
            }

            object baseProperties = base.SaveProperties();
            return new pdnList(properties.Clone(), new pdnList(baseProperties, null));
        }

        public override void LoadProperties(object oldState, bool suppressEvents)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("IntensityLayer");
            }

            pdnList list = (pdnList)oldState;

            // Get the base class' state, and our state
            LayerProperties baseState = (LayerProperties)list.Tail.Head;
            IntensityLayerProperties blp = (IntensityLayerProperties)(((pdnList)oldState).Head);

            // Opacity is only couriered for compatibility with PDN v2.0 and v1.1
            // files. It should not be present in v2.1+ files (well, it'll be
            // part of the base class' serialization)
            if (blp.opacity != -1)
            {
                baseState.opacity = (byte)blp.opacity;
                blp.opacity = -1;
            }

            // Have the base class load its properties
            base.LoadProperties(baseState, suppressEvents);

            // Now load our properties, and announce them to the world
            bool raiseBlendOp = false;

            if (blp.blendOp.GetType() != properties.blendOp.GetType())
            {
                if (!suppressEvents)
                {
                    raiseBlendOp = true;
                    OnPropertyChanging(IntensityLayerProperties.BlendOpName);
                }
            }

            this.properties = (IntensityLayerProperties)blp.Clone();
            this.compiledBlendOp = null;

            Invalidate();

            if (raiseBlendOp)
            {
                OnPropertyChanged(IntensityLayerProperties.BlendOpName);
            }
        }

        public void SetBlendOp(UserBlendOp blendOp)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("IntensityLayer");
            }

            if (blendOp.GetType() != properties.blendOp.GetType())
            {
                OnPropertyChanging(IntensityLayerProperties.BlendOpName);
                properties.blendOp = blendOp;
                compiledBlendOp = null;
                Invalidate();
                OnPropertyChanged(IntensityLayerProperties.BlendOpName);
            }
        }

        public override object Clone()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("IntensityLayer");
            }

            return (object)new IntensityLayer(this);
        }

        public override  Surface Surface
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IntensityLayer");
                }

                return Intensity_Surface;
            }
        }

        public UserBlendOp BlendOp
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("IntensityLayer");
                }

                return properties.blendOp;
            }
        }

        public IntensityLayer(int width, int height)
            : this(width, height, ColorBgra.FromBgra(255, 255, 255, 0))
        {
        }

        public IntensityLayer(int width, int height, ColorBgra fillColor)
            : base(width, height)
        {
            this.Intensity_Surface  = new IntensitySurface2(width, height);
            // clear to see-through white, 0x00ffffff
            this.Surface.Clear(fillColor);
            this.properties = new IntensityLayerProperties(UserBlendOps.CreateDefaultBlendOp());
        }

        /// <summary>
        /// Creates a new BitmapLayer of the same size as the given Surface, and copies the 
        /// pixels from the given Surface.
        /// </summary>
        /// <param name="RGB32_Surface">The Surface to copy pixels from.</param>
        public IntensityLayer(Surface surface)
            : this(surface, false)
        {
        }


        private IntensitySurface2 ConvertSurfaceToIntensity(Surface s)
        {
            if (s.GetType() == typeof(IntensitySurface2))
                return (IntensitySurface2 ) s;
            else 
                return new IntensitySurface2(new Size(s.Width, s.Height));
        }
        /// <summary>
        /// Creates a new BitmapLayer of the same size as the given Surface, and either
        /// copies the pixels of the given Surface or takes ownership of it.
        /// </summary>
        /// <param name="RGB32_Surface">The Surface.</param>
        /// <param name="takeOwnership">
        /// true to take ownership of the RGB32_Surface (make sure to Dispose() it yourself), or
        /// false to copy its pixels
        /// </param>
        public IntensityLayer(Surface surface, bool takeOwnership)
            : base(surface.Width, surface.Height)
        {
            //todo: need to convert a normal surface to an intensity surface at this
            //point
            if (takeOwnership)
            {
               this.Intensity_Surface = ConvertSurfaceToIntensity( surface );
            }
            else
            {
               this.Intensity_Surface = ConvertSurfaceToIntensity( surface.Clone() );
            }

            this.properties = new IntensityLayerProperties(UserBlendOps.CreateDefaultBlendOp());
        }
        public void ReplaceSurface(Surface surface)
        {
            this.Intensity_Surface  = ConvertSurfaceToIntensity(  surface );

        }
        protected IntensityLayer(IntensityLayer copyMe)
            : base(copyMe)
        {
            this.Intensity_Surface = ConvertSurfaceToIntensity( copyMe.Surface.Clone() );
            this.properties = (IntensityLayerProperties)copyMe.properties.Clone();
        }

        protected unsafe override void RenderImpl(RenderArgs args, Rectangle roi)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("IntesityLayer");
            }

            if (Opacity == 0)
            {
                return;
            }

            if (compiledBlendOp == null)
            {
                CompileBlendOp();
            }

            for (int y = roi.Top; y < roi.Bottom; ++y)
            {
                ColorBgra* dstPtr = args.Surface.GetPointAddressUnchecked(roi.Left, y);
                ColorBgra* srcPtr = this.Intensity_Surface.GetPointAddressUnchecked(roi.Left, y);

                this.compiledBlendOp.Apply(dstPtr, srcPtr, roi.Width);
            }
        }

        protected unsafe override void RenderImpl(RenderArgs args, Rectangle[] rois, int startIndex, int length)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("IntensityLayer");
            }

            if (Opacity == 0)
            {
                return;
            }

            if (compiledBlendOp == null)
            {
                CompileBlendOp();
            }

            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Rectangle roi = rois[i];

                for (int y = roi.Top; y < roi.Bottom; ++y)
                {
                    ColorBgra* dstPtr = args.Surface.GetPointAddressUnchecked(roi.Left, y);
                    ColorBgra* srcPtr = this.Intensity_Surface.GetPointAddressUnchecked(roi.Left, y);

                    this.compiledBlendOp.Apply(dstPtr, srcPtr, roi.Width);
                }
            }
        }

        public override PdnBaseForm CreateConfigDialog()
        {
            IntensityLayerPropertiesDialog blpd = new IntensityLayerPropertiesDialog();
            blpd.Layer = this;
            return blpd;
        }

        public void OnDeserialization(object sender)
        {
            if (this.properties.opacity != -1)
            {
                this.PushSuppressPropertyChanged();
                base.Opacity = (byte)this.properties.opacity;
                this.properties.opacity = -1;
                this.PopSuppressPropertyChanged();
            }

            this.compiledBlendOp = null;
        }
    }
}
