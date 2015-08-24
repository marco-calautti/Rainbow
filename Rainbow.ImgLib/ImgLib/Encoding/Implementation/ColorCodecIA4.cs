using Rainbow.ImgLib.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecIA4: ColorCodecEndiannessDependent
    {
        public ColorCodecIA4(ByteOrder order) :
            base(order) { }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));
            Color[] decoded = new Color[length];

            for(int i=0;i<decoded.Length;i++)
            {
                byte data = reader.ReadByte();

                int alpha = ByteOrder == ByteOrder.LittleEndian ? data & 0xF : (data >> 4) & 0xF;
                int intensity = ByteOrder == ByteOrder.LittleEndian ? (data>>4) & 0xF : data & 0xF;

                alpha = ImageUtils.Conv4To8(alpha);
                intensity = ImageUtils.Conv4To8(intensity);
                decoded[i] = Color.FromArgb(alpha, intensity, intensity, intensity);
            }

            return decoded;
        }

        public override byte[] EncodeColors(System.Drawing.Color[] colors, int start, int length)
        {
            throw new NotImplementedException();
        }

        public override int BitDepth
        {
            get { return 8; }
        }
    }
}
