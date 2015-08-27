//Copyright (C) 2014+ Marco (Phoenix) Calautti.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 2.0.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License 2.0 for more details.

//A copy of the GPL 2.0 should have been included with the program.
//If not, see http://www.gnu.org/licenses/

//Official repository and contact information can be found at
//http://github.com/marco-calautti/Rainbow

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
