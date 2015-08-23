using Rainbow.ImgLib.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecIA8 : ColorCodecEndiannessDependent
    {
        public ColorCodecIA8(ByteOrder order):
            base(order) { }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));
            Color[] decoded = new Color[length / 2];

            for(int i=0;i<decoded.Length;i++)
            {
                ushort data=reader.ReadUInt16(ByteOrder);
                int alpha=(data>>8)&0xFF;
                int intensity=data&0xFF;
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
            get { return 16; }
        }
    }
}
