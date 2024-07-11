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
using SciImage.Core.Surfaces;
using SciImage.Core.Renderer;
using SciImage.Core.Selection;

namespace SciImage.Core.BitVector
{
    /// <summary>
    /// Adapts a Surface class so it can be used as a two dimensional boolean array.
    /// Elements are stored compactly, such that each pixel stores 32 boolean values.
    /// However, the usable width is the same as that of the adapted RGB32_Surface.
    /// (in other words, a RGB32_Surface that is 100 pixels wide can still only store 100
    /// booleans per row)
    /// </summary>
    public sealed class BitVector2DSurfaceAdapter
        : IBitVector2D
    {
        private Surface surface;
        Int32  FalseColor;
        public BitVector2DSurfaceAdapter(Surface surface)
        {
            if (surface == null)
            {
                throw new ArgumentNullException("RGB32_Surface");
            }

            this.surface = surface;
            FalseColor = this.surface.ColorPixelBase.BlackColor().ToInt32();
        }

        public int Width
        {
            get
            {
                return surface.Width;
            }
        }

        public int Height
        {
            get
            {
                return surface.Height;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (Width == 0) || (Height == 0);
            }
        }

        public void Clear(bool newValue)
        {
            unsafe
            {
                Int32  val = newValue ? surface.ColorPixelBase  .WhiteColor().ToInt32()  : surface.ColorPixelBase .BlackColor().ToInt32();
                
                for (int y = 0; y < Height; ++y)
                {
                    //ColorPixelBase *row = surface.GetRowAddress(y);

                    for (int w= this.Width-1 ;w>0;w--)
                    {
                        surface[w, y] = val;
                    }
                }
            }
        }

        public bool Get(int x, int y)
        {
            if (x < 0 || x >= this.Width)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            if (y < 0 || y >= this.Height)
            {
                throw new ArgumentOutOfRangeException("y");
            }

            return GetUnchecked(x, y);
        }

        public unsafe bool GetUnchecked(int x, int y)
        {
            int i = surface[x, y];
            bool  mask =( FalseColor  != i );
            return mask;
        }

        public void Set(int x, int y, bool newValue)
        {
            if (x < 0 || x >= this.Width)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            if (y < 0 || y >= this.Height)
            {
                throw new ArgumentOutOfRangeException("y");
            }

            SetUnchecked(x, y, newValue);
        }

        public void Set(Point pt, bool newValue)
        {
            Set(pt.X, pt.Y, newValue);
        }

        public void Set(Rectangle rect, bool newValue)
        {
            for (int y = rect.Top; y < rect.Bottom; ++y)
            {
                for (int x = rect.Left; x < rect.Right; ++x)
                {
                    Set(x, y, newValue);
                }
            }
        }

        public void Set(Scanline scan, bool newValue)
        {
            int x = scan.X;

            while (x < scan.X + scan.Length)
            {
                Set(x, scan.Y, newValue);
                ++x;
            }
        }

        public void Set(SciRegion region, bool newValue)
        {
            foreach (Rectangle rect in region.GetRegionScansReadOnlyInt())
            {
                Set(rect, newValue);
            }
        }

        public unsafe void SetUnchecked(int x, int y, bool newValue)
        {
           
            int newMask;
            
            if (newValue)
            {
                newMask =(int) surface.ColorPixelBase.WhiteColor().ToInt32();
            }
            else
            {
                newMask = FalseColor ;
            }

            surface[x, y] = newMask;// SetPoint(x, y, ptr.TranslateColor((Int32)newMask));
        }

        public void Invert(int x, int y)
        {
            Set(x, y, !Get(x, y));
        }

        public void Invert(Point pt)
        {
            Invert(pt.X, pt.Y);
        }

        public void Invert(Rectangle rect)
        {
            for (int y = rect.Top; y < rect.Bottom; ++y)
            {
                for (int x = rect.Left; x < rect.Right; ++x)
                {
                    Invert(x, y);
                }
            }
        }

        public void Invert(Scanline scan)
        {
            int x = scan.X;

            while (x < scan.X + scan.Length)
            {
                Invert(x, scan.Y);
                ++x;
            }
        }

        public void Invert(SciRegion region)
        {
            foreach (Rectangle rect in region.GetRegionScansReadOnlyInt())
            {
                Invert(rect);
            }        
        }

        public bool this[System.Drawing.Point pt]
        {
            get
            {
                return this[pt.X, pt.Y];
            }

            set
            {
                this[pt.X, pt.Y] = value;
            }
        }

        public bool this[int x, int y]
        {
            get
            {
                return Get(x, y);
            }

            set
            {
                Set(x, y, value);
            }
        }

        public BitVector2DSurfaceAdapter Clone()
        {
            Surface clonedSurface = this.surface.Clone();
            return new BitVector2DSurfaceAdapter(clonedSurface);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
