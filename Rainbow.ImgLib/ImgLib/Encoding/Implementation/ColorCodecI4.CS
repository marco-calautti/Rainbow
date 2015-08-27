using Rainbow.ImgLib.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecI4 : ColorCodecEndiannessDependent
    {
        public ColorCodecI4(ByteOrder order):
            base(order) { }

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            Color[] decoded = new Color[length * 2];

            for(int i=0;i<length;i++)
            {
                byte value=colors[start+i];
                int first   = ByteOrder == ByteOrder.LittleEndian ? value & 0xF : (value >> 4) & 0xF;
                int second = ByteOrder == ByteOrder.LittleEndian ? (value >> 4) & 0xF : value & 0xF;

                first = ImageUtils.Conv4To8(first);
                second = ImageUtils.Conv4To8(second);

                decoded[i * 2]  = Color.FromArgb(255, first, first, first);
                decoded[i * 2 + 1] = Color.FromArgb(255, second, second, second);
            }

            return decoded;
        }

        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            throw new NotImplementedException();
        }

        public override int BitDepth
        {
            get { return 4; }
        }
    }
}
