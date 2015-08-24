using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Common;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecRGB565 : ColorCodecEndiannessDependent
    {

        public ColorCodecRGB565(ByteOrder order):
            base(order) { }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));

            Color[] encoded = new Color[length / 2];

            for (int i = 0; i < encoded.Length; i++)
            {
                ushort color = 0;
                color = reader.ReadUInt16(ByteOrder);


                int red, green, blue;
                red = ImageUtils.Conv5To8((color >> 11) & 0x1f);
                green = ImageUtils.Conv6To8((color >> 5) & 0x3f);
                blue = ImageUtils.Conv5To8((color) & 0x1f);

                encoded[i] = Color.FromArgb(255, red, green, blue);
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
