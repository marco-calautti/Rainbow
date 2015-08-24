using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodec32BitABGR : ColorCodec
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            for (int i = 0; i < size / 4; i++)
                pal.Add(Color.FromArgb(palette[start + i * 4], palette[start + i * 4 + 3], palette[start + i * 4 + 2], palette[start + i * 4]));

            return pal.ToArray();
        }

        public override int BitDepth { get { return 32; } }

        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            throw new NotImplementedException();
        }
    }
}
