using Rainbow.ImgLib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Filters
{
    public class GamecubePlanarFilter : ImageFilter
    {

        public GamecubePlanarFilter()
        {

        }

        public override byte[] ApplyFilter(byte[] originalData, int index, int length)
        {
            throw new NotImplementedException();
        }

        public override byte[] Defilter(byte[] originalData, int index, int length)
        {
            byte[] decoded = new byte[length];

            BinaryReader reader = new BinaryReader(new MemoryStream(originalData, index, length));

            int planeSize = 32;
            int totalPlanes=2;
            int tileSize = planeSize * totalPlanes;

            int totalTiles=length/tileSize;

            byte[] ar = new byte[planeSize];
            byte[] gb = new byte[planeSize];

            for (int pos = 0; pos < length ;pos+=tileSize)
            {
                reader.Read(ar, 0, ar.Length);
                reader.Read(gb, 0, gb.Length);

                for(int i=0;i<planeSize;i+=2)
                {
                    byte a = ar[i];
                    byte r = ar[i + 1];

                    byte g = gb[i];
                    byte b = gb[i + 1];

                    decoded[pos + i * 2]        = a;
                    decoded[pos + i * 2 + 1]    = r;
                    decoded[pos + i * 2 + 2]    = g;
                    decoded[pos + i * 2 + 3]    = b;
                }
            }

            reader.Close();
            return decoded;
        }

    }
}
