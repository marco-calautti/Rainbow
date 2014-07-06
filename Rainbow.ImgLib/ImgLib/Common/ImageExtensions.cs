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
            for (int x = 0; x < img.Width; x++)
                for (int y = 0; y < img.Height; y++)
                    list.Add(bmp.GetPixel(x, y));
            return list.ToArray();
        }
    }
}
