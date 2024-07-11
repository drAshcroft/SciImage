using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SciImage.Core.Surfaces.ColorsAndPixelOps;

namespace SciImage.Core.Surfaces
{
    public class SurfaceFactory
    {
        public static Surface ConvertSurface(Surface Source, ColorPixelBase DestinationFormat)
        {
            return Source;
        }
    }
}
