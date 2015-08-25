using Rainbow.ImgLib.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class ColorCodecI8: ColorCodec
    {

        public override Color[] DecodeColors(byte[] colors, int start, int length)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(colors, start, length));
            Color[] decoded = new Color[length];

            for(int i=0;i<decoded.Length;i++)
            {
                int intensity = reader.ReadByte();
                decoded[i] = Color.FromArgb(255, intensity, intensity, intensity);
            }
            reader.Close();
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
