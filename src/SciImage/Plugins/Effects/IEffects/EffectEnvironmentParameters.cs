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
using SciImage.Core.Selection;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;

namespace SciImage.Plugins.Effects.IEffects
{
    public class EffectEnvironmentParameters
        : IDisposable
    {
        public static EffectEnvironmentParameters DefaultParameters
        {
            get
            {
                return new EffectEnvironmentParameters(ColorBgra.sFromBgra(255, 255, 255, 255),
                                                       ColorBgra.sFromBgra(0, 0, 0, 255),
                                                       2.0f,
                                                       new SciRegion(),
                                                       null);
            }
        }

        private ColorPixelBase primaryColor = ColorBgra.sFromBgra(0, 0, 0, 0);
        private ColorPixelBase secondaryColor = ColorBgra.sFromBgra(0, 0, 0, 0);
        private float brushWidth = 0.0f;
        private SciRegion selection;
        private bool haveIntersectedSelection = false;
        private Surface sourceSurface;

        public ColorPixelBase PrimaryColor
        {
            get
            {
                return this.primaryColor;
            }
        }

        public ColorPixelBase SecondaryColor
        {
            get
            {
                return this.secondaryColor;
            }
        }

        public float BrushWidth 
        {
            get 
            {
                return this.brushWidth;
            }
        }

        public Surface SourceSurface
        {
            get
            {
                return this.sourceSurface;
            }
        }

        /// <summary>
        /// Gets the user's currently selected area.
        /// </summary>
        /// <param name="boundingRect">
        /// The bounding rectangle of the RGB32_Surface you will be rendering to. 
        /// The region returned will be clipped to this bounding rectangle.
        /// </param>
        /// <remarks>
        /// Note that calls to Render() will already be clipped to this selection area. 
        /// This data is only useful when an effect wants to change its rendering based
        /// on what the user has selected. For instance, This is used by Auto-Levels to
        /// only calculate new levels based on what the user has selected
        /// </remarks>
        public SciRegion GetSelection(Rectangle boundingRect)
        {
            if (!this.haveIntersectedSelection)
            {
                this.selection.Intersect(boundingRect);
                this.haveIntersectedSelection = true;
            }

            return this.selection;
        }

        public EffectEnvironmentParameters(ColorPixelBase primaryColor, ColorPixelBase secondaryColor, float brushWidth, SciRegion selection, Surface sourceSurface)
        {
            this.primaryColor = primaryColor;
            this.secondaryColor = secondaryColor;
            this.brushWidth = brushWidth;
            this.selection = (SciRegion)selection.Clone();
            this.sourceSurface = sourceSurface;
        }

        ~EffectEnvironmentParameters()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.selection != null)
                {
                    this.selection.Dispose();
                    this.selection = null;
                }
            }
        }
    }
}
