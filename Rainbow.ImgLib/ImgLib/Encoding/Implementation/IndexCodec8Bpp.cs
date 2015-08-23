using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class IndexCodec8Bpp : IndexCodec
    {
        public override int GetPixelIndex(byte[] pixelData, int width, int height, int x, int y)
        {
            return pixelData[x + y * width];
        }

        public override byte[] PackIndexes(int[] indexes, int start, int length)
        {
            byte[] packed = new byte[length];
            for (int i = start; i < length; i++)
            {
                if (indexes[i] > 255)
                    throw new Exception("Too big index!");
                packed[i - start] = (byte)indexes[i];
            }
            return packed;
        }

        public override int BitDepth
        {
            get { return 8; }
        }
    }
}
