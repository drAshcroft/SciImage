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
    /// <summary>
    /// Defines a way to operate on a pixel, or a region of pixels, in a binary fashion.
    /// That is, it is a simple function F that takes two parameters and returns a
    /// result of the form: c = F(a, b)
    /// </summary>
    [Serializable]
    public unsafe abstract class BinaryPixelOp
        : PixelOp
    {
        public override  abstract ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs);

        public unsafe virtual void Apply(ref ColorPixelBase dst, ColorPixelBase lhs, ColorPixelBase rhs)
        {
             dst = Apply(dst.TranslateColor( lhs) ,dst.TranslateColor( rhs ));
        }

        /// <summary>
        /// Provides a default implementation for performing dst = F(lhs, rhs) over some rectangle of interest.
        /// </summary>
        /// <param name="dst">The Surface to write pixels to.</param>
        /// <param name="dstOffset">The pixel offset that defines the upper-left of the rectangle-of-interest for the dst Surface.</param>
        /// <param name="lhs">The Surface to read pixels from for the lhs parameter given to the method <b>ColorPixelBase Apply(ColorPixelBase, ColorPixelBase)</b>b>.</param></param>
        /// <param name="lhsOffset">The pixel offset that defines the upper-left of the rectangle-of-interest for the lhs Surface.</param>
        /// <param name="rhs">The Surface to read pixels from for the rhs parameter given to the method <b>ColorPixelBase Apply(ColorPixelBase, ColorPixelBase)</b></param>
        /// <param name="rhsOffset">The pixel offset that defines the upper-left of the rectangle-of-interest for the rhs Surface.</param>
        /// <param name="roiSize">The size of the rectangles-of-interest for all Surfaces.</param>
        public void Apply(Surface dst, Point dstOffset, 
                          Surface lhs, Point lhsOffset, 
                          Surface rhs, Point rhsOffset, 
                          Size roiSize)
        {
            // Bounds checking only enabled in Debug builds.
#if DEBUG
            // Create bounding rectangles for each Surface
            Rectangle dstRect = new Rectangle(dstOffset, roiSize);
            Rectangle lhsRect = new Rectangle(lhsOffset, roiSize);
            Rectangle rhsRect = new Rectangle(rhsOffset, roiSize);

            // Clip those rectangles to those Surface's bounding rectangles
            Rectangle dstClip = Rectangle.Intersect(dstRect, dst.Bounds);
            Rectangle lhsClip = Rectangle.Intersect(lhsRect, lhs.Bounds);
            Rectangle rhsClip = Rectangle.Intersect(rhsRect, rhs.Bounds);

            // If any of those Rectangles actually got clipped, then throw an exception
            if (dstRect != dstClip)
            {
                throw new ArgumentOutOfRangeException("roiSize", "Destination roi out of bounds");
            }

            if (lhsRect != lhsClip)
            {
                throw new ArgumentOutOfRangeException("roiSize", "lhs roi out of bounds");
            }

            if (rhsRect != rhsClip)
            {
                throw new ArgumentOutOfRangeException("roiSize", "rhs roi out of bounds");
            }
#endif

            // Cache the width and height properties
            int width = roiSize.Width;
            int height = roiSize.Height;

           
                for (int row = 0; row < height; ++row)
                {
                    for (int col = 0; col < width; ++col)
                    {
                       // ColorPixelBase dstPtr = dst.GetPoint(dstOffset.X + col , dstOffset.Y + row);
                        ColorPixelBase lhsPtr = lhs.GetPoint(lhsOffset.X + col , lhsOffset.Y + row);
                        ColorPixelBase rhsPtr = rhs.GetPoint(rhsOffset.X + col , rhsOffset.Y + row, lhsPtr );

                        dst[dstOffset.X+col,dstOffset.Y+row ]= Apply( lhsPtr, rhsPtr).ToInt32();

                    }
                }
           
        }

       /* public unsafe override void Apply(ref ColorPixelBase dst, ColorPixelBase src)
        {
            dst = Apply(dst, src);
        }*/

        public override void Apply(Surface dst, Point dstOffset, Surface src, Point srcOffset, int roiLength)
        {
               
                for (int col = 0; col < roiLength; ++col)
                {
                    
                    ColorPixelBase lhsPtr = dst.GetPoint( dstOffset.X + col, dstOffset.Y);
                    ColorPixelBase rhsPtr = src.GetPoint(srcOffset.X + col, srcOffset.Y,lhsPtr );

                    dst[dstOffset.X + col, dstOffset.Y ] = Apply(lhsPtr, rhsPtr).ToInt32();

                }
            
            //Apply(dst.GetPointAddress(dstOffset), src.GetPointAddress(srcOffset), roiLength);
        }

        public void Apply(Surface dst, Surface src)
        {
            if (dst.Size != src.Size)
            {
                throw new ArgumentException("dst.Size != src.Size");
            }

           
                for (int y = 0; y < dst.Height; ++y)
                {
                    for (int x = 0; x < dst.Width; ++x) 
                    {
                        ColorPixelBase dstPtr = dst.GetPoint(x,y);
                        ColorPixelBase srcPtr = src.GetPoint(x, y, dstPtr);
                        dst[x,y] =  Apply(dstPtr, srcPtr).ToInt32();

                    }
                }
            
        }

        public void Apply(Surface dst, Surface lhs, Surface rhs)
        {
            if (dst.Size != lhs.Size)
            {
                throw new ArgumentException("dst.Size != lhs.Size");
            }

            if (lhs.Size != rhs.Size)
            {
                throw new ArgumentException("lhs.Size != rhs.Size");
            }


            for (int y = 0; y < dst.Height; ++y)
            {
                for (int x = 0; x < dst.Width; ++x)
                {
                    ColorPixelBase lhsPtr = lhs.GetPoint(x, y, dst.ColorPixelBase );
                    ColorPixelBase rhsPtr = rhs.GetPoint(x, y, dst.ColorPixelBase );
                    dst[x, y] = Apply(lhsPtr , rhsPtr ).ToInt32();

                }
            }
        }

        protected BinaryPixelOp()
        {
        }
    }
}
