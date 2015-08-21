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

using Rainbow.ImgLib.Formats.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Rainbow.App.GUI.Model
{
    class PE3DATPropertyGridObject: TexturePropertyGridObject
    {
        public PE3DATPropertyGridObject(PE3DATTexture texture)
            : base(texture)
        {

        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("Position1 value")]
        [DisplayName("Position1")]
        public uint Position1
        {
            get
            {
                return ((PE3DATTexture)texture).Position1;
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("Position2 value")]
        [DisplayName("Position2")]
        public uint Position2
        {
            get
            {
                return ((PE3DATTexture)texture).Position2;
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The number of bits used to encode one pixel.")]
        [DisplayName("Bit Depth")]
        public int Bpp
        {
            get
            {
                return ((PE3DATTexture)texture).Bpp;
            }
        }
    }
}
