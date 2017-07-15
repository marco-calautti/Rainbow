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
            byte[] encoded = new byte[(length +1)/ 2];

            for(int i=0;i<length;i+=2)
            {
                Color first = ImageUtils.ToGrayScale(colors[start + i]);
                Color second = i == length - 1 ? Color.Black : ImageUtils.ToGrayScale(colors[start + i + 1]);

                int firstNibble = ImageUtils.Conv8To4(first.R);
                int secondNibble = ImageUtils.Conv8To4(second.R);

                byte value = 0;

                if(ByteOrder == ByteOrder.LittleEndian)
                {
                    value = (byte)((firstNibble & 0xF) | ((secondNibble & 0xF) << 4));
                }else
                {
                    value = (byte)((secondNibble & 0xF) | ((firstNibble & 0xF) << 4));
                }

                encoded[i / 2] = value;
            }

            return encoded;
        }

        public override int BitDepth
        {
            get { return 4; }
        }
    }
}
