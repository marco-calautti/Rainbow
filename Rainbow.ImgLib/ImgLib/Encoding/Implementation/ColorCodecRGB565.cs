using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Common;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecRGB565 : ColorCodec, EndiannessDependent
    {

        public ColorCodecRGB565(ByteOrder order)
        {
            ByteOrder = order;
        }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));

            Color[] encoded = new Color[length / 2];

            for (int i = 0; i < encoded.Length; i++)
            {
                ushort color = 0;
                color = reader.ReadUInt16(ByteOrder);


                int red, green, blue;
                red = ((color >> 11) & 0x1f) * 8;
                green = ((color >> 5) & 0x3f) * 4;
                blue = ((color) & 0x1f) * 8;

                encoded[i] = Color.FromArgb(255, red, green, blue);
            }

            return encoded;
        }

        public override int BitDepth
        {
            get { return 16; }
        }

        public ByteOrder ByteOrder {get; set;}

        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            throw new NotImplementedException();
        }
    }
}
