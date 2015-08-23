using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Common;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecRGB5A3 : ColorCodecEndiannessDependent
    {
        public ColorCodecRGB5A3(ByteOrder order):
            base(order) { }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));

            Color[] encoded = new Color[length / 2];

            for (int i = 0; i < encoded.Length; i++)
            {
                ushort color = 0;
                color = reader.ReadUInt16(ByteOrder);


                int red, green, blue, alpha;
                if ((color & 0x8000) != 0) //no alpha
                {
                    red = ((color >> 10) & 0x1F) * 8;
                    green = ((color >> 5) & 0x1F) * 8;
                    blue = ((color) & 0x1F) * 8;
                    alpha = 255;
                }
                else // with alpha
                {
                    alpha = ((color >> 12) & 0x7) * 32;
                    red = ((color >> 8) & 0xf) * 16;
                    green = ((color >> 4) & 0xf) * 16;
                    blue = ((color) & 0xf) * 16;
                }

                encoded[i] = Color.FromArgb(alpha, red, green, blue);
            }

            return encoded;
        }

        public override int BitDepth
        {
            get { return 16; }
        }

        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            throw new NotImplementedException();
        }
    }
}
