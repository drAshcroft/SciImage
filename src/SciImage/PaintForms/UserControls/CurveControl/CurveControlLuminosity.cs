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
    /// Curve control specialized for luminosity
    /// </summary>
    public sealed class CurveControlLuminosity
        : CurveControl
    {
        public CurveControlLuminosity()
            : base(1, 256)
        {
            this.mask = new bool[1]{true};
            visualColors = new ColorPixelBase[]{     
                                              ColorBgra.Black
                                          };
            channelNames = new string[]{
                        SciResources.SciResources.GetString("CurveControlLuminosity.Luminosity")
            };
            ResetControlPoints();
        }

        public override ColorTransferMode ColorTransferMode
        {
            get
            {
                return ColorTransferMode.Luminosity;
            }
        }
    }
}
