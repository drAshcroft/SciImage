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

namespace SciImage.Core.Surfaces.ColorsAndPixelOps
{
    /// <summary>
    /// Defines a way to operate on a pixel, or a region of pixels, in a unary fashion.
    /// That is, it is a simple function F that takes one parameter and returns a
    /// result of the form: d = F(c)
    /// </summary>
    [Serializable]
    public unsafe abstract class UnaryPixelOp
        : PixelOp
    {
        public abstract ColorPixelBase Apply(ColorPixelBase color);

        private unsafe void ApplyRectangle(Surface surface, Rectangle rect)
        {
            for (int y = rect.Top; y < rect.Bottom; ++y)
            {
                for (int x = rect.Left; x < rect.Right; ++x)
                {
                    ColorPixelBase ptr = surface.GetPoint( x, y);
                    surface[x, y] = Apply(ptr).ToInt32();
                }
            }
        }

        public void Apply(Surface surface, Rectangle[] roi, int startIndex, int length)
        {
            Rectangle regionBounds = Utility.GetRegionBounds(roi, startIndex, length);

            if (regionBounds != Rectangle.Intersect(surface.Bounds, regionBounds))
            {
                throw new ArgumentOutOfRangeException("roi", "Region is out of bounds");
            }

            unsafe
            {
                for (int x = startIndex; x < startIndex + length; ++x)
                {
                    ApplyRectangle(surface, roi[x]);
                }
            }
        }

        public void Apply(Surface surface, Rectangle[] roi)
        {
            Apply(surface, roi, 0, roi.Length);
        }

        public void Apply(Surface surface, RectangleF[] roiF, int startIndex, int length)
        {
            Rectangle regionBounds = Rectangle.Truncate(Utility.GetRegionBounds(roiF, startIndex, length));

            if (regionBounds != Rectangle.Intersect(surface.Bounds, regionBounds))
            {
                throw new ArgumentOutOfRangeException("roiF", "Region is out of bounds");
            }

            unsafe
            {
                for (int x = startIndex; x < startIndex + length; ++x)
                {
                    ApplyRectangle(surface, Rectangle.Truncate(roiF[x]));
                }
            }
        }

        public void Apply(Surface surface, RectangleF[] roiF)
        {
            Apply(surface, roiF, 0, roiF.Length);
        }

        public unsafe void Apply(Surface surface, Rectangle roi)
        {
            ApplyRectangle(surface, roi);
        }

        public void Apply(Surface surface, Scanline scan)
        {
            int MaxX=scan.X + scan.Length ;
            for (int x = scan.X ; x < MaxX ; x++)
            {

                surface[x,scan.Y ]=  Apply(surface.GetPoint(x , scan.Y)).ToInt32();
            }
        }

        public void Apply(Surface surface, Scanline[] scans)
        {
            foreach (Scanline scan in scans)
            {
                Apply(surface, scan);
            }
        }

       /* public override void Apply(Surface dst, Point dstOffset, Surface src, Point srcOffset, int scanLength)
        {
            Apply(dst.GetPointAddress(dstOffset), src.GetPointAddress(srcOffset), scanLength);
        }*/

        public void Apply(Surface dst, Surface src, Rectangle roi)
        {
            for (int y = roi.Top; y < roi.Bottom; ++y)
            {
                for (int x = roi.Left; x < roi.Right; ++x)
                {
                    //ColorPixelBase* dstPtr = dst.GetPoint(x, y);
                    ColorPixelBase srcPtr = src.GetPoint(x, y,dst.ColorPixelBase );
                    dst[x,y]= Apply( srcPtr).ToInt32();
                }
            }
        }

        public void Apply(Surface surface, SciRegion roi)
        {
            Apply(surface, roi.GetRegionScansReadOnlyInt());
        }

        public override ColorPixelBase Apply(ColorPixelBase dst, ColorPixelBase src)
        {
            return Apply(src);
        }

        public UnaryPixelOp()
        {
        }
    }
}
