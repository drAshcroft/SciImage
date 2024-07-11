using System;

namespace SciImage.Core.Surfaces.ColorsAndPixelOps.UnaryPixelOps
{
    /// <summary>
    /// Used to set a given channel of a pixel to a given, predefined color.
    /// Useful if you want to set only the alpha value of a given region.
    /// </summary>
    [Serializable]
    public class SetChannel
        : UnaryPixelOp
    {
        private int channel;
        private byte setValue;

        public override ColorPixelBase Apply(ColorPixelBase color)
        {
            color.SetChannel(channel, setValue);
            return color;
        }


        public SetChannel(int channel, byte setValue)
        {
            this.channel = channel;
            this.setValue = setValue;
        }
    }
}
