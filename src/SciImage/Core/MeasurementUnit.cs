/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace SciImage.Core
{
    /// <summary>
    /// Specifies the unit of measure for the given data.
    /// </summary>
    /// <remarks>
    /// These enumeration values correspond to the values used in the EXIF ResolutionUnit tag.
    /// </remarks>
  
       public enum MeasurementUnit   : int
       {
        Pixel = 1,
        Inch = 2,
        Centimeter = 3
       }

    public class MeasurementUnitDetails
    {
       public static string Abbreviation(MeasurementUnit Unit)
       {
               if (Unit == MeasurementUnit.Pixel) return "Pix";
               if (Unit == MeasurementUnit.Centimeter ) return "cm";
               return "in";
       }
       public static  double  Conversion(MeasurementUnit fromUnit, MeasurementUnit toUnit)
       {
           return 1;
       }
       public static string Plural(MeasurementUnit Unit)
       {
           if (Unit == MeasurementUnit.Pixel) return "Pixels";
           if (Unit == MeasurementUnit.Centimeter) return "Centimeters";
           return "Inches";
       }

    }
}
