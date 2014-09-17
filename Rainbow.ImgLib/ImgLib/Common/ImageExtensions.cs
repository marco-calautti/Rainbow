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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Common
{
    public static class ImageExtensions
    {
        public static Color[] GetColorArray(this Image img)
        {
            Bitmap bmp = new Bitmap(img);
            var list = new List<Color>();
            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    list.Add(bmp.GetPixel(x, y));
            return list.ToArray();
        }

        public static int ColorsCount(this Image img)
        {
            Bitmap bmp = new Bitmap(img);
            HashSet<Color> colors = new HashSet<Color>();
            int count = 0;
            for (int y = 0; y < img.Height; y++)
                for (int x = 0; x < img.Width; x++)
                    if (!colors.Contains(bmp.GetPixel(x, y)))
                    {
                        colors.Add(bmp.GetPixel(x, y));
                        count++;
                    }
            return count;
        }
    }
}
