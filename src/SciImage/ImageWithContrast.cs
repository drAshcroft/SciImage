using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SciImage
{
    public class ImageWithContrast
    {
        public Image TheImage;
        public long MaxIntensity;
        public long MinIntensity;
        public ImageWithContrast(Image TheImage, long MaxIntensity, long MinIntensity)
        {
            this.TheImage = TheImage;
            this.MaxIntensity = MaxIntensity;
            this.MinIntensity = MinIntensity;
        }
    }
}
