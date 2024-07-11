using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UserBlendOps
{
    [Serializable]
    public sealed class ColorBurnBlendOp : UserBlendOp
    {
        public static string StaticName
        {
            get
            {
                return SciResources.SciResources.GetString("UserBlendOps." + "ColorBurn" + "BlendOp.Name");
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
                    if (((rhs)[0]) == 0)
                    {
                        fB = 0;
                    }
                    else
                    {

                        {
                            int i = (((rhs)[0])) * 3; uint M = masTable[i]; uint A = masTable[i + 1]; uint S = masTable[i + 2]; fB = (int)(((((255 - ((lhs)[0])) * 255) * M) + A) >> (int)S);
                        }
                        ; fB = 255 - fB; fB = Math.Max(0, fB);
                    }

                }
                ;
                {
                    if (((rhs)[1]) == 0)
                    {
                        fG = 0;
                    }
                    else
                    {

                        {
                            int i = (((rhs)[1])) * 3; uint M = masTable[i]; uint A = masTable[i + 1]; uint S = masTable[i + 2]; fG = (int)(((((255 - ((lhs)[1])) * 255) * M) + A) >> (int)S);
                        }
                        ; fG = 255 - fG; fG = Math.Max(0, fG);
                    }

                }
                ;
                {
                    if (((rhs)[2]) == 0)
                    {
                        fR = 0;
                    }
                    else
                    {

                        {
                            int i = (((rhs)[2])) * 3; uint M = masTable[i]; uint A = masTable[i + 1]; uint S = masTable[i + 2]; fR = (int)(((((255 - ((lhs)[2])) * 255) * M) + A) >> (int)S);
                        }
                        ; fR = 255 - fR; fR = Math.Max(0, fR);
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
                    if (((rhs)[0]) == 0)
                    {
                        fB = 0;
                    }
                    else
                    {

                        {
                            int i = (((rhs)[0])) * 3; uint M = masTable[i]; uint A = masTable[i + 1]; uint S = masTable[i + 2]; fB = (int)(((((255 - ((lhs)[0])) * 255) * M) + A) >> (int)S);
                        }
                        ; fB = 255 - fB; fB = Math.Max(0, fB);
                    }

                }
                ;
                {
                    if (((rhs)[1]) == 0)
                    {
                        fG = 0;
                    }
                    else
                    {

                        {
                            int i = (((rhs)[1])) * 3; uint M = masTable[i]; uint A = masTable[i + 1]; uint S = masTable[i + 2]; fG = (int)(((((255 - ((lhs)[1])) * 255) * M) + A) >> (int)S);
                        }
                        ; fG = 255 - fG; fG = Math.Max(0, fG);
                    }

                }
                ;
                {
                    if (((rhs)[2]) == 0)
                    {
                        fR = 0;
                    }
                    else
                    {

                        {
                            int i = (((rhs)[2])) * 3; uint M = masTable[i]; uint A = masTable[i + 1]; uint S = masTable[i + 2]; fR = (int)(((((255 - ((lhs)[2])) * 255) * M) + A) >> (int)S);
                        }
                        ; fR = 255 - fR; fR = Math.Max(0, fR);
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
            return new ColorBurnBlendOpWithOpacity(opacity);
        }
        private sealed class ColorBurnBlendOpWithOpacity : UserBlendOp
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
                    return SciResources.SciResources.GetString("UserBlendOps." + "ColorBurn" + "BlendOp.Name");
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
                        if (((rhs)[0]) == 0)
                        {
                            fB = 0;
                        }
                        else
                        {

                            {
                                int i = (((rhs)[0])) * 3; uint M = masTable[i]; uint A = masTable[i + 1]; uint S = masTable[i + 2]; fB = (int)(((((255 - ((lhs)[0])) * 255) * M) + A) >> (int)S);
                            }
                            ; fB = 255 - fB; fB = Math.Max(0, fB);
                        }

                    }
                    ;
                    {
                        if (((rhs)[1]) == 0)
                        {
                            fG = 0;
                        }
                        else
                        {

                            {
                                int i = (((rhs)[1])) * 3; uint M = masTable[i]; uint A = masTable[i + 1]; uint S = masTable[i + 2]; fG = (int)(((((255 - ((lhs)[1])) * 255) * M) + A) >> (int)S);
                            }
                            ; fG = 255 - fG; fG = Math.Max(0, fG);
                        }

                    }
                    ;
                    {
                        if (((rhs)[2]) == 0)
                        {
                            fR = 0;
                        }
                        else
                        {

                            {
                                int i = (((rhs)[2])) * 3; uint M = masTable[i]; uint A = masTable[i + 1]; uint S = masTable[i + 2]; fR = (int)(((((255 - ((lhs)[2])) * 255) * M) + A) >> (int)S);
                            }
                            ; fR = 255 - fR; fR = Math.Max(0, fR);
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

            public ColorBurnBlendOpWithOpacity(int opacity)
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
