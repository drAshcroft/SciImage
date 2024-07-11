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

namespace SciImage.Core.Surfaces.ColorsAndPixelOps
{
    [Serializable]
    public unsafe abstract class PixelOp
        : IPixelOp
    {
        /// <summary>
        /// Computes alpha for r OVER l operation.
        /// </summary>
        public static byte ComputeAlpha(byte la, byte ra)
        {
            return (byte)(((la * (256 - (ra + (ra >> 7)))) >> 8) + ra);
        }

        public void Apply(Surface dst, Surface src, Rectangle[] rois, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                ApplyBase(dst, rois[i].Location, src, rois[i].Location, rois[i].Size);
            }
        }

        public void Apply(Surface dst, Point dstOffset, Surface src, Point srcOffset, Size roiSize)
        {
            ApplyBase(dst, dstOffset, src, srcOffset, roiSize);
        }

        /// <summary>
        /// Provides a default implementation for performing dst = F(dst, src) or F(src) over some rectangle 
        /// of interest. May be slightly faster than calling the other multi-parameter Apply method, as less 
        /// variables are used in the implementation, thus inducing less register pressure.
        /// </summary>
        /// <param name="dst">The Surface to write pixels to, and from which pixels are read and used as the lhs parameter for calling the method <b>ColorPixelBase Apply(ColorPixelBase, ColorPixelBase)</b>.</param>
        /// <param name="dstOffset">The pixel offset that defines the upper-left of the rectangle-of-interest for the dst Surface.</param>
        /// <param name="src">The Surface to read pixels from for the rhs parameter given to the method <b>ColorPixelBase Apply(ColorPixelBase, ColorPixelBase)</b>b>.</param></param>
        /// <param name="srcOffset">The pixel offset that defines the upper-left of the rectangle-of-interest for the src Surface.</param>
        /// <param name="roiSize">The size of the rectangles-of-interest for all Surfaces.</param>
        public void ApplyBase(Surface dst, Point dstOffset, Surface src, Point srcOffset, Size roiSize)
        {
            // Create bounding rectangles for each Surface
            Rectangle dstRect = new Rectangle(dstOffset, roiSize);

            if (dstRect.Width == 0 || dstRect.Height == 0)
            {
                return;
            }

            Rectangle srcRect = new Rectangle(srcOffset, roiSize);

            if (srcRect.Width == 0 || srcRect.Height == 0)
            {
                return;
            }

            // Clip those rectangles to those Surface's bounding rectangles
            Rectangle dstClip = Rectangle.Intersect(dstRect, dst.Bounds);
            Rectangle srcClip = Rectangle.Intersect(srcRect, src.Bounds);

            // If any of those Rectangles actually got clipped, then throw an exception
            if (dstRect != dstClip)
            {
                throw new ArgumentOutOfRangeException
                (
                    "roiSize",
                    "Destination roi out of bounds" +
                    ", dst.Size=" + dst.Size.ToString() +
                    ", dst.Bounds=" + dst.Bounds.ToString() +
                    ", dstOffset=" + dstOffset.ToString() +
                    ", src.Size=" + src.Size.ToString() +
                    ", srcOffset=" + srcOffset.ToString() +
                    ", roiSize=" + roiSize.ToString() +
                    ", dstRect=" + dstRect.ToString() +
                    ", dstClip=" + dstClip.ToString() +
                    ", srcRect=" + srcRect.ToString() +
                    ", srcClip=" + srcClip.ToString()
                );
            }

            if (srcRect != srcClip)
            {
                throw new ArgumentOutOfRangeException("roiSize", "Source roi out of bounds");
            }

            // Cache the width and height properties
            int width = roiSize.Width;
            int height = roiSize.Height;

           
               
                    for (int row = 0; row < roiSize.Height; ++row)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            ColorPixelBase dstPtr = dst.GetPoint(dstOffset.X+col, dstOffset.Y + row);
                            ColorPixelBase srcPtr = src.GetPoint(srcOffset.X+col, srcOffset.Y + row,dstPtr );
                            dst.SetPoint(dstOffset.X + col, dstOffset.Y + row,  Apply(dstPtr, srcPtr) );
                        }
                    }
               
            
        }

        public virtual void Apply(Surface dst, Point dstOffset, Surface src, Point srcOffset, int scanLength)
        {
            for (int col = 0; col < scanLength; col++)
            {
                ColorPixelBase dstPtr = dst.GetPoint(dstOffset.X + col, dstOffset.Y );
                ColorPixelBase srcPtr = src.GetPoint(srcOffset.X  + col, srcOffset.Y , dstPtr);
                Apply(dstPtr, srcPtr);
            }
            //Apply(dst.GetPointAddress(dstOffset), src.GetPointAddress(srcOffset), scanLength);
        }

        public virtual  ColorPixelBase  Apply(ColorPixelBase dst, ColorPixelBase src)
        {
            throw new System.NotImplementedException("Derived class must implement Apply(ColorPixelBase*,ColorPixelBase*,int)");
        }

        public PixelOp()
        {
        }
    }
}
