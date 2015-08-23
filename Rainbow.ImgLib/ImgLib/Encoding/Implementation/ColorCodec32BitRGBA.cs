using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    /// <summary>
    /// This  ColorDecoder decodes sequences of pixels in 32 bit RGBA format.
    /// </summary>
    public class ColorCodec32BitRGBA : ColorCodec
    {
        public override Color[] DecodeColors(byte[] palette, int start, int size)
        {
            List<Color> pal = new List<Color>();

            for (int i = 0; i < size / 4; i++)
                pal.Add(Color.FromArgb(palette[start + i * 4 + 3], palette[start + i * 4], palette[start + i * 4 + 1], palette[start + i * 4 + 2]));

            return pal.ToArray();
        }

        public override byte[] EncodeColors(System.Drawing.Color[] colors, int start, int length)
        {

            byte[] palette = new byte[colors.Length * 4];
            for (int i = start; i < colors.Length; i++)
            {
                palette[(i - start) * 4] = colors[i].R;
                palette[(i - start) * 4 + 1] = colors[i].G;
                palette[(i - start) * 4 + 2] = colors[i].B;
                palette[(i - start) * 4 + 3] = colors[i].A;
            }
            return palette;
        }

        public override int BitDepth { get { return 32; } }
    }
}
