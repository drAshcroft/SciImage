/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Drawing;
using SciImage.Core.Surfaces;
using SciImage.Core.Surfaces.ColorsAndPixelOps;

namespace SciImage.Core.Renderer
{
    public sealed class SurfaceBoxBaseRenderer
        : SurfaceBoxRenderer
    {
        private Surface source;
        private RenderDelegate renderDelegate;

        public Surface Source
        {
            get
            {
                return this.source;
            }

            set
            {
                this.source = value;
                Flush();
            }
        }

        private void Flush()
        {
            this.renderDelegate = null;
        }

        protected override void OnVisibleChanged()
        {
            Invalidate();
        }

        private void ChooseRenderDelegate()
        {
            if (SourceSize.Width > DestinationSize.Width)
            {
                // zoom out
                this.renderDelegate = new RenderDelegate(RenderZoomOutRotatedGridMultisampling);
            }
            else if (SourceSize == DestinationSize)
            {
                // zoom 100%
                this.renderDelegate = new RenderDelegate(RenderOneToOne);
            }
            else if (SourceSize.Width < DestinationSize.Width)
            {
                // zoom in
                this.renderDelegate = new RenderDelegate(RenderZoomInNearestNeighbor);
            }
        }

        public override void OnDestinationSizeChanged()
        {
            ChooseRenderDelegate();
            this.OwnerList.InvalidateLookups();
            base.OnDestinationSizeChanged();
        }

        public override void OnSourceSizeChanged()
        {
            ChooseRenderDelegate();
            this.OwnerList.InvalidateLookups();
            base.OnSourceSizeChanged();
        }

        public static void RenderOneToOne(Surface dst, Surface source, Point offset)
        {
            unsafe
            {
                Rectangle srcRect = new Rectangle(offset, dst.Size);
                srcRect.Intersect(source.Bounds);

                for (int dstRow = 0; dstRow < dst.Height; ++dstRow)
                {
                    //ColorPixelBase* dstRowPtr = dst.GetRowAddressUnchecked(dstRow);
                    //ColorPixelBase* srcRowPtr = source.GetPointAddressUnchecked(offset.X, dstRow + offset.Y);

                    int dstCol = 0;
                    int dstColEnd = dst.Width;
                    int checkerY = dstRow + offset.Y;
                    int xx=0;
                    for (int x = dstCol; x < dstColEnd;x++ )
                    {
                        int[] Channels = new int[source.ColorPixelBase.NumChannels];
                        int c= source[offset.X +x,dstRow+offset.Y];
                       /* int a = c.alpha ;
                        int v = (((dstCol ^ checkerY) & 8) << 3) + 191;
                        a = a + (a >> 7);
                        int vmia = v * (256 - a);

                        for (int i = 0; i < Channels.Length; i++)
                        {
                            Channels[i] = (int)c.GetChannel(i);
                            c.SetChannel(i, ((Channels[i] * a) + vmia) >> 8);
                        }*/

                        dst[x, dstRow]=c;
                        xx++;
                    }
                }
            }
        }

        private void RenderOneToOne(Surface dst, Point offset)
        {
            RenderOneToOne(dst, this.source, offset);
        }

        private void RenderZoomInNearestNeighbor(Surface dst, Point offset)
        {
            unsafe
            {
                int[] d2SLookupY = OwnerList.Dst2SrcLookupY;
                int[] d2SLookupX = OwnerList.Dst2SrcLookupX;

                for (int dstRow = 0; dstRow < dst.Height; ++dstRow)
                {
                    int nnY = dstRow + offset.Y;
                    int srcY = d2SLookupY[nnY];
                    //ColorPixelBase *dstPtr = dst.GetRowAddressUnchecked(dstRow);
                    //ColorPixelBase *srcRow = this.source.GetRowAddressUnchecked(srcY);

                    for (int dstCol = 0; dstCol < dst.Width; ++dstCol)
                    {
                        int nnX = dstCol + offset.X;
                        int srcX = d2SLookupX[nnX];

                       // ColorPixelBase src = *(srcRow + srcX);

                        ColorPixelBase c = source.GetPoint(srcX  , srcY ).Clone();
                        int a = c.alpha;
                        int v = (((dstCol + offset.X) ^ (dstRow + offset.Y)) & 8) * 8 + 191;
                        a = a + (a >> 7);
                        int vmia = v * (256 - a);
                        int[] Channels = new int[c.NumChannels];
                        for (int i = 0; i < Channels.Length; i++)
                        {
                            Channels[i] = (int)c.GetChannel(i);
                            c.SetChannel(i, ((Channels[i] * a) + vmia) >> 8);
                        }

                        dst.SetPoint(dstCol , dstRow, c);
                        

                       
                    }
                }
            }
        }
        public static void RenderZoomOutRotatedGridMultisampling(Surface dst, Surface source, Point offset, Size destinationSize)
        {
            unsafe
            {
                Size sourceSize = source.Size;
                double scaleX = (double)(source.Width-1.5) / (double)destinationSize.Width;
                double scaleY = (double)(source.Height-1.5) / (double)destinationSize.Height;

                int dx =(int)( scaleX /2.0d);
                int dy =(int)( scaleY /2.0d);
                ColorBgra c=new ColorBgra();
                for (int dstRow =0  ; dstRow < dst.Height  ;  ++dstRow )
                {
                    for (int dstCol = 0 ; dstCol<dst.Width  ;dstCol++)
                    {
                        int Red=0,Green=0,Blue=0,a=0;
                        int cc = 0;
                        int SrcX =(int)( (dstCol  + offset.X) * scaleX  );
                        int SrcY =(int)( (dstRow  + offset.Y) * scaleY  );

                        //c = (ColorBgra)source.GetPoint(SrcX , SrcY );
                        int x1 = SrcX - dx;
                        int x2 = SrcX + dx;
                        int y1 = SrcY - dy;
                        int y2 = SrcY + dy;
                        if (x1 < 0) x1 = 0;
                        if (y1 < 0) y1 = 0;
                        if (x2 > source.Width) x2 = source.Width;
                        if (y2 > source.Height) y2 = source.Height;
                        for (int i=x1;i<=x2;i++)
                        {
                            for (int j=y1;j<=y2;j++)
                            {
                                c=(ColorBgra) source.GetPoint(i,j);
                                
                                Red+=c.R ;
                                Green +=c.G ;
                                Blue+=c.B;
                                a += c.A;
                                cc++;
                            }
                        }
                        //if (cc > 0)
                        //{
                            c.R = (byte)(Red / (double)cc);
                            c.G = (byte)(Green / (double)cc);
                            c.B = (byte)(Blue / (double)cc);
                            c.A = (byte)(a / (double)cc);
                        //}
                    
                       /* int checkerY = dstRow + offset.Y;
                        int checkerX = offset.X;
                        int maxCheckerX = checkerX + dst.Width;

                        // Blend it over the checkerboard background
                        int v = ((checkerX ^ checkerY) & 8) * 8 + 191;
                        int vmia = v * (256 - a);

                        int[] Channels = new int[c.NumChannels];
                        for (int i = 0; i < Channels.Length; i++)
                        {
                            c.SetChannel(i, ((Channels[i] * a) + vmia) >> 8);
                        }
                       */
                        dst.SetPoint(dstCol, dstRow, c.Clone());


                    }
                }
            }
        }
       /* public static void RenderZoomOutRotatedGridMultisampling(Surface dst, Surface source, Point offset, Size destinationSize)
        {
            unsafe
            {
                const int fpShift = 12;
                const int fpFactor = (1 << fpShift);

                Size sourceSize = source.Size;
                long fDstLeftLong = ((long)offset.X * fpFactor * (long)sourceSize.Width) / (long)destinationSize.Width;
                long fDstTopLong = ((long)offset.Y * fpFactor * (long)sourceSize.Height) / (long)destinationSize.Height;
                long fDstRightLong = ((long)(offset.X + dst.Width) * fpFactor * (long)sourceSize.Width) / (long)destinationSize.Width;
                long fDstBottomLong = ((long)(offset.Y + dst.Height) * fpFactor * (long)sourceSize.Height) / (long)destinationSize.Height;
                int fDstLeft = (int)fDstLeftLong;
                int fDstTop = (int)fDstTopLong;
                int fDstRight = (int)fDstRightLong;
                int fDstBottom = (int)fDstBottomLong;
                int dx = (fDstRight - fDstLeft) / dst.Width;
                int dy = (fDstBottom - fDstTop) / dst.Height;

                for (int dstRow = 0, fDstY = fDstTop;
                    dstRow < dst.Height && fDstY < fDstBottom;
                    ++dstRow, fDstY += dy)
                {
                    int srcY1 = fDstY >> fpShift;                            // y
                    int srcY2 = (fDstY + (dy >> 2)) >> fpShift;              // y + 0.25
                    int srcY3 = (fDstY + (dy >> 1)) >> fpShift;              // y + 0.50
                    int srcY4 = (fDstY + (dy >> 1) + (dy >> 2)) >> fpShift;  // y + 0.75

#if DEBUG
                    Debug.Assert(source.IsRowVisible(srcY1));
                    Debug.Assert(source.IsRowVisible(srcY2));
                    Debug.Assert(source.IsRowVisible(srcY3));
                    Debug.Assert(source.IsRowVisible(srcY4));
                    Debug.Assert(dst.IsRowVisible(dstRow));
#endif

                   // ColorPixelBase* src1 = source.GetRowAddressUnchecked(srcY1);
                   // ColorPixelBase* src2 = source.GetRowAddressUnchecked(srcY2);
                   // ColorPixelBase* src3 = source.GetRowAddressUnchecked(srcY3);
                   // ColorPixelBase* src4 = source.GetRowAddressUnchecked(srcY4);
                    //ColorPixelBase* dstPtr = dst.GetRowAddressUnchecked(dstRow);
                    int dstCol;
                    dstCol = 0;
                    int checkerY = dstRow + offset.Y;
                    int checkerX = offset.X;
                    int maxCheckerX = checkerX + dst.Width;

                    for (int fDstX = fDstLeft;
                         checkerX < maxCheckerX && fDstX < fDstRight;
                         ++checkerX, fDstX += dx)
                    {
                        int srcX1 = (fDstX + (dx >> 2)) >> fpShift;             // x + 0.25
                        int srcX2 = (fDstX + (dx >> 1) + (dx >> 2)) >> fpShift; // x + 0.75
                        int srcX3 = fDstX >> fpShift;                           // x
                        int srcX4 = (fDstX + (dx >> 1)) >> fpShift;             // x + 0.50

#if DEBUG
                        Debug.Assert(source.IsColumnVisible(srcX1));
                        Debug.Assert(source.IsColumnVisible(srcX2));
                        Debug.Assert(source.IsColumnVisible(srcX3));
                        Debug.Assert(source.IsColumnVisible(srcX4));
#endif

                        ColorPixelBase p1 =source.GetPoint(srcX1, srcY1 );
                        ColorPixelBase p2 = source.GetPoint(srcX2, srcY2);
                        ColorPixelBase p3 = source.GetPoint(srcX3, srcY3);
                        ColorPixelBase p4 = source.GetPoint(srcX4, srcY4);
                        ColorPixelBase c = p1.Clone();
                        int a = (2 + p1.alpha  + p2.alpha  + p3.alpha  + p4.alpha ) >> 2;

                        // Blend it over the checkerboard background
                        int v = ((checkerX ^ checkerY) & 8) * 8 + 191;
                        a = a + (a >> 7);
                        int vmia = v * (256 - a);


                        int[] Channels = new int[c.NumChannels];
                        for (int i = 0; i < Channels.Length; i++)
                        {
                            Channels[i] = (int)(2 + p1.GetChannel(i)+ p2.GetChannel(i)+ p3.GetChannel(i)+ p4.GetChannel(i))>>2 ;
                            c.SetChannel(i, ((Channels[i] * a) + vmia) >> 8);
                        }

                        dst.SetPoint(dstCol , dstRow, c);

                       
                    }
                }
            }
        }*/

        private void RenderZoomOutRotatedGridMultisampling(Surface dst, Point offset)
        {
            RenderZoomOutRotatedGridMultisampling(dst, this.source, offset, this.DestinationSize);
        }

        public override void Render(Surface dst, Point offset)
        {
            if (this.renderDelegate == null)
            {
                ChooseRenderDelegate();
            }

            this.renderDelegate(dst, offset);
        }

        public SurfaceBoxBaseRenderer(SurfaceBoxRendererList ownerList, Surface source)
            : base(ownerList)
        {
            this.source = source;
            ChooseRenderDelegate();
        }
    }
}
