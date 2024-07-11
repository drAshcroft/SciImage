﻿using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UserBlendOps
{
    [Serializable]
    public sealed class OverlayBlendOp : UserBlendOp
    {
        public static string StaticName
        {
            get
            {
                return SciResources.SciResources.GetString("UserBlendOps." + "Overlay" + "BlendOp.Name");
            }

        }
        public override ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs)
        {
            int lhsA;
            {
                lhsA = ((lhs).alpha);
            }
            ; int rhsA;
            {
                rhsA = ((rhs).alpha);
            }
            ; int y;
            {
                y = ((lhsA) * (255 - rhsA) + 0x80); y = ((((y) >> 8) + (y)) >> 8);
            }
            ; int totalA = y + rhsA; uint ret; if (totalA == 0)
            {
                ret = 0;
            }
            else
            {
                int fB; int fG; int fR;
                {
                    if (((lhs)[0]) < 128)
                    {

                        {
                            fB = ((2 * ((lhs)[0])) * (((rhs)[0])) + 0x80); fB = ((((fB) >> 8) + (fB)) >> 8);
                        }
                        ;
                    }
                    else
                    {

                        {
                            fB = ((2 * (255 - ((lhs)[0]))) * (255 - ((rhs)[0])) + 0x80); fB = ((((fB) >> 8) + (fB)) >> 8);
                        }
                        ; fB = 255 - fB;
                    }

                }
                ;
                {
                    if (((lhs)[1]) < 128)
                    {

                        {
                            fG = ((2 * ((lhs)[1])) * (((rhs)[1])) + 0x80); fG = ((((fG) >> 8) + (fG)) >> 8);
                        }
                        ;
                    }
                    else
                    {

                        {
                            fG = ((2 * (255 - ((lhs)[1]))) * (255 - ((rhs)[1])) + 0x80); fG = ((((fG) >> 8) + (fG)) >> 8);
                        }
                        ; fG = 255 - fG;
                    }

                }
                ;
                {
                    if (((lhs)[2]) < 128)
                    {

                        {
                            fR = ((2 * ((lhs)[2])) * (((rhs)[2])) + 0x80); fR = ((((fR) >> 8) + (fR)) >> 8);
                        }
                        ;
                    }
                    else
                    {

                        {
                            fR = ((2 * (255 - ((lhs)[2]))) * (255 - ((rhs)[2])) + 0x80); fR = ((((fR) >> 8) + (fR)) >> 8);
                        }
                        ; fR = 255 - fR;
                    }

                }
                ; int x;
                {
                    x = ((lhsA) * (rhsA) + 0x80); x = ((((x) >> 8) + (x)) >> 8);
                }
                ; int z = rhsA - x; int masIndex = totalA * 3; uint taM = masTable[masIndex]; uint taA = masTable[masIndex + 1]; uint taS = masTable[masIndex + 2]; uint b = (uint)(((((long)((((lhs)[0] * y) + ((rhs)[0] * z) + (fB * x)))) * taM) + taA) >> (int)taS); uint g = (uint)(((((long)((((lhs)[1] * y) + ((rhs)[1] * z) + (fG * x)))) * taM) + taA) >> (int)taS); uint r = (uint)(((((long)((((lhs)[2] * y) + ((rhs)[2] * z) + (fR * x)))) * taM) + taA) >> (int)taS); int a;
                {

                    {
                        a = ((lhsA) * (255 - (rhsA)) + 0x80); a = ((((a) >> 8) + (a)) >> 8);
                    }
                    ; a += (rhsA);
                }
                ; ret = b + (g << 8) + (r << 16) + ((uint)a << 24);
            }
            ; return rhs.TranslateColor(ret);
        }

        public static ColorPixelBase ApplyStatic(ColorPixelBase lhs, ColorPixelBase rhs)
        {
            int lhsA;
            {
                lhsA = ((lhs).alpha);
            }
            ; int rhsA;
            {
                rhsA = ((rhs).alpha);
            }
            ; int y;
            {
                y = ((lhsA) * (255 - rhsA) + 0x80); y = ((((y) >> 8) + (y)) >> 8);
            }
            ; int totalA = y + rhsA; uint ret; if (totalA == 0)
            {
                ret = 0;
            }
            else
            {
                int fB; int fG; int fR;
                {
                    if (((lhs)[0]) < 128)
                    {

                        {
                            fB = ((2 * ((lhs)[0])) * (((rhs)[0])) + 0x80); fB = ((((fB) >> 8) + (fB)) >> 8);
                        }
                        ;
                    }
                    else
                    {

                        {
                            fB = ((2 * (255 - ((lhs)[0]))) * (255 - ((rhs)[0])) + 0x80); fB = ((((fB) >> 8) + (fB)) >> 8);
                        }
                        ; fB = 255 - fB;
                    }

                }
                ;
                {
                    if (((lhs)[1]) < 128)
                    {

                        {
                            fG = ((2 * ((lhs)[1])) * (((rhs)[1])) + 0x80); fG = ((((fG) >> 8) + (fG)) >> 8);
                        }
                        ;
                    }
                    else
                    {

                        {
                            fG = ((2 * (255 - ((lhs)[1]))) * (255 - ((rhs)[1])) + 0x80); fG = ((((fG) >> 8) + (fG)) >> 8);
                        }
                        ; fG = 255 - fG;
                    }

                }
                ;
                {
                    if (((lhs)[2]) < 128)
                    {

                        {
                            fR = ((2 * ((lhs)[2])) * (((rhs)[2])) + 0x80); fR = ((((fR) >> 8) + (fR)) >> 8);
                        }
                        ;
                    }
                    else
                    {

                        {
                            fR = ((2 * (255 - ((lhs)[2]))) * (255 - ((rhs)[2])) + 0x80); fR = ((((fR) >> 8) + (fR)) >> 8);
                        }
                        ; fR = 255 - fR;
                    }

                }
                ; int x;
                {
                    x = ((lhsA) * (rhsA) + 0x80); x = ((((x) >> 8) + (x)) >> 8);
                }
                ; int z = rhsA - x; int masIndex = totalA * 3; uint taM = masTable[masIndex]; uint taA = masTable[masIndex + 1]; uint taS = masTable[masIndex + 2]; uint b = (uint)(((((long)((((lhs)[0] * y) + ((rhs)[0] * z) + (fB * x)))) * taM) + taA) >> (int)taS); uint g = (uint)(((((long)((((lhs)[1] * y) + ((rhs)[1] * z) + (fG * x)))) * taM) + taA) >> (int)taS); uint r = (uint)(((((long)((((lhs)[2] * y) + ((rhs)[2] * z) + (fR * x)))) * taM) + taA) >> (int)taS); int a;
                {

                    {
                        a = ((lhsA) * (255 - (rhsA)) + 0x80); a = ((((a) >> 8) + (a)) >> 8);
                    }
                    ; a += (rhsA);
                }
                ; ret = b + (g << 8) + (r << 16) + ((uint)a << 24);
            }
            ; return rhs.TranslateColor(ret);
        }
        public override UserBlendOp CreateWithOpacity(int opacity)
        {
            return new OverlayBlendOpWithOpacity(opacity);
        }
        private sealed class OverlayBlendOpWithOpacity : UserBlendOp
        {
            private int opacity;
            private byte ApplyOpacity(byte a)
            {
                int r;
                {
                    r = (a);
                }
                ;
                {
                    r = ((r) * (this.opacity) + 0x80); r = ((((r) >> 8) + (r)) >> 8);
                }
                ; return (byte)r;
            }
            public static string StaticName
            {
                get
                {
                    return SciResources.SciResources.GetString("UserBlendOps." + "Overlay" + "BlendOp.Name");
                }

            }
            public override ColorPixelBase Apply(ColorPixelBase lhs, ColorPixelBase rhs)
            {
                int lhsA;
                {
                    lhsA = ((lhs).alpha);
                }
                ; int rhsA;
                {
                    rhsA = ApplyOpacity((rhs).alpha);
                }
                ; int y;
                {
                    y = ((lhsA) * (255 - rhsA) + 0x80); y = ((((y) >> 8) + (y)) >> 8);
                }
                ; int totalA = y + rhsA; uint ret; if (totalA == 0)
                {
                    ret = 0;
                }
                else
                {
                    int fB; int fG; int fR;
                    {
                        if (((lhs)[0]) < 128)
                        {

                            {
                                fB = ((2 * ((lhs)[0])) * (((rhs)[0])) + 0x80); fB = ((((fB) >> 8) + (fB)) >> 8);
                            }
                            ;
                        }
                        else
                        {

                            {
                                fB = ((2 * (255 - ((lhs)[0]))) * (255 - ((rhs)[0])) + 0x80); fB = ((((fB) >> 8) + (fB)) >> 8);
                            }
                            ; fB = 255 - fB;
                        }

                    }
                    ;
                    {
                        if (((lhs)[1]) < 128)
                        {

                            {
                                fG = ((2 * ((lhs)[1])) * (((rhs)[1])) + 0x80); fG = ((((fG) >> 8) + (fG)) >> 8);
                            }
                            ;
                        }
                        else
                        {

                            {
                                fG = ((2 * (255 - ((lhs)[1]))) * (255 - ((rhs)[1])) + 0x80); fG = ((((fG) >> 8) + (fG)) >> 8);
                            }
                            ; fG = 255 - fG;
                        }

                    }
                    ;
                    {
                        if (((lhs)[2]) < 128)
                        {

                            {
                                fR = ((2 * ((lhs)[2])) * (((rhs)[2])) + 0x80); fR = ((((fR) >> 8) + (fR)) >> 8);
                            }
                            ;
                        }
                        else
                        {

                            {
                                fR = ((2 * (255 - ((lhs)[2]))) * (255 - ((rhs)[2])) + 0x80); fR = ((((fR) >> 8) + (fR)) >> 8);
                            }
                            ; fR = 255 - fR;
                        }

                    }
                    ; int x;
                    {
                        x = ((lhsA) * (rhsA) + 0x80); x = ((((x) >> 8) + (x)) >> 8);
                    }
                    ; int z = rhsA - x; int masIndex = totalA * 3; uint taM = masTable[masIndex]; uint taA = masTable[masIndex + 1]; uint taS = masTable[masIndex + 2]; uint b = (uint)(((((long)((((lhs)[0] * y) + ((rhs)[0] * z) + (fB * x)))) * taM) + taA) >> (int)taS); uint g = (uint)(((((long)((((lhs)[1] * y) + ((rhs)[1] * z) + (fG * x)))) * taM) + taA) >> (int)taS); uint r = (uint)(((((long)((((lhs)[2] * y) + ((rhs)[2] * z) + (fR * x)))) * taM) + taA) >> (int)taS); int a;
                    {

                        {
                            a = ((lhsA) * (255 - (rhsA)) + 0x80); a = ((((a) >> 8) + (a)) >> 8);
                        }
                        ; a += (rhsA);
                    }
                    ; ret = b + (g << 8) + (r << 16) + ((uint)a << 24);
                }
                ; return rhs.TranslateColor(ret);
            }

            public OverlayBlendOpWithOpacity(int opacity)
            {
                if (this.opacity < 0 || this.opacity > 255)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.opacity = opacity;
            }

        }

    }
}
