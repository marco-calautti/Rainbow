using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImgLib.Encoding
{
    public class IndexPacker8Bpp : IndexPacker
    {
        public override int BitDepth
        {
            get { return 8; }
        }

        public override byte[] PackIndexes(int[] indexes, int start, int length)
        {
            byte[] packed = new byte[length];
            for(int i=start;i<length;i++)
            {
                if (indexes[i] > 255)
                    throw new Exception("Too big index!");
                packed[i-start] = (byte)indexes[i];
            }
            return packed;
        }
    }

    public class IndexPacker4Bpp : IndexPacker
    {
        public override int BitDepth
        {
            get { return 4; }
        }

        public override byte[] PackIndexes(int[] indexes, int start, int length)
        {
            if(length%2!=0)
                throw new Exception("number of indexes must be odd!");

            byte[] packed = new byte[length/2];
            int k = 0;
            for (int i = start; i < length; i+=2)
            {
                if (indexes[i] > 15 || indexes[i+1]>15)
                    throw new Exception("Too big index!");
                packed[k++] = (byte)((indexes[i+1]<<4) | indexes[i]);
            }
            return packed;
        }
    }
}
