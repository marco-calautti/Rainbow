using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Encoding.ColorComparers
{
    public class ARGBColorComparer : IComparer<Color>
    {
        public int Compare(Color x, Color y)
        {
            long c1 = (uint)(x.A << 24 | x.B << 16 | x.G << 8 | x.R);
            long c2 = (uint)(y.A << 24 | y.B << 16 | y.G << 8 | y.R);
            long result = c1 - c2;
            return result < 0 ? -1 : result > 0 ? 1 : 0;
        }
    }
}
