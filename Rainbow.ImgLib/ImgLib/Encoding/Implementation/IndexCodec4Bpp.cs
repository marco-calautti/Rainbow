using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Common;

namespace Rainbow.ImgLib.Encoding.Implementation
{
    public class IndexCodec4Bpp : IndexCodec, EndiannessDependent
    {
        public override int GetPixelIndex(byte[] pixelData, int width, int height, int x, int y)
        {
            int pos = x + y * width;
            byte b = pixelData[pos / 2];

            if (ByteOrder == ByteOrder.LittleEndian)
                return pos % 2 == 0 ? b & 0xF : (b >> 4) & 0xF;
            else
                return pos % 2 != 0 ? b & 0xF : (b >> 4) & 0xF;
        }

        public override byte[] PackIndexes(int[] indexes, int start, int length)
        {
            if (length % 2 != 0)
                throw new Exception("Number of indexes must be odd!");

            byte[] packed = new byte[length / 2];
            int k = 0;
            for (int i = start; i < length; i += 2)
            {
                if (indexes[i] > 15 || indexes[i + 1] > 15)
                    throw new Exception("Too big index!");
                if (ByteOrder == ByteOrder.LittleEndian)
                    packed[k++] = (byte)((indexes[i + 1] << 4) | indexes[i]);
                else
                    packed[k++] = (byte)((indexes[i] << 4) | indexes[i + 1]);
            }
            return packed;
        }

        public override int BitDepth
        {
            get { return 4; }
        }

        public ByteOrder ByteOrder { get; set; }
    }
}
