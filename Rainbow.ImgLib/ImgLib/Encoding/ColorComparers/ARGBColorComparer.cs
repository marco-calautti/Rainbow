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
