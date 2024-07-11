/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.Core.Surfaces.ColorsAndPixelOps;

namespace SciImage.PaintForms.UserControls.CurveControl
{
    /// <summary>
    /// Curve control specialization for RGB curves
    /// </summary>
    public sealed class CurveControlRgb
        : CurveControl
    {
        public CurveControlRgb()
            : base(3, 256)
        {
            this.mask = new bool[3] { true, true, true };
            visualColors = new ColorPixelBase[] {     
                                               ColorBgra.Red,
                                               ColorBgra.Green,
                                               ColorBgra.Blue 
                                           };
            channelNames = new string[]{
                SciResources.SciResources.GetString("CurveControlRgb.Red"),
                SciResources.SciResources.GetString("CurveControlRgb.Green"),
                SciResources.SciResources.GetString("CurveControlRgb[0]")
            };
            ResetControlPoints();
        }

        public override ColorTransferMode ColorTransferMode
        {
            get
            {
                return ColorTransferMode.Rgb;
            }
        }
    }
}
