namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    public class PosterizePixelOp
           : UnaryPixelOp
    {
        private byte[] redLevels;
        private byte[] greenLevels;
        private byte[] blueLevels;

        public PosterizePixelOp(int red, int green, int blue)
        {
            this.redLevels = CalcLevels(red);
            this.greenLevels = CalcLevels(green);
            this.blueLevels = CalcLevels(blue);
        }

        private static byte[] CalcLevels(int levelCount)
        {
            byte[] t1 = new byte[levelCount];

            for (int i = 1; i < levelCount; i++)
            {
                t1[i] = (byte)((255 * i) / (levelCount - 1));
            }

            byte[] levels = new byte[256];

            int j = 0;
            int k = 0;

            for (int i = 0; i < 256; i++)
            {
                levels[i] = t1[j];

                k += levelCount;

                if (k > 255)
                {
                    k -= 255;
                    j++;
                }
            }

            return levels;
        }

        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            return color.FromBgra(blueLevels[color[0]], greenLevels[color[1]], redLevels[color[2]], color.alpha);
        }




    }
}
