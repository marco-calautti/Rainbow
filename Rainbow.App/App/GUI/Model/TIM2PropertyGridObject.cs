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
using System.ComponentModel;

namespace Rainbow.App.GUI.Model
{
    public class TIM2PropertyGridObject : TexturePropertyGridObject
    {
        public TIM2PropertyGridObject(TIM2Texture texture) : base(texture)
        {

        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The version number of this TIM2 texture.")]
        [DisplayName("TIM2 Version")]
        public int Version
        {
            get
            {
                return ((TIM2Texture)texture).Version;
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The number of bits used to encode one pixel.")]
        [DisplayName("Bit Depth")]
        public int Bpp
        {
            get
            {
                return ((TIM2Texture)texture).Bpp;
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The number of bytes used to encode one color.")]
        [DisplayName("Bytes per Color")]
        public int ColorSize
        {
            get
            {
                return ((TIM2Texture)texture).ColorSize;
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("Denotes if the current frame has a palette in linear or interleaved form.")]
        [DisplayName("Is Linear Palette")]
        public bool LinearPalette
        {
            get
            {
                return ((TIM2Texture)texture).LinearPalette;
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("Denotes if swizzling has to be applied to all the frames of this TIM2 or not.")]
        [DisplayName("Swizzled")]
        public bool Swizzled
        {
            get
            {
                return ((TIM2Texture)texture).Swizzled;
            }

            set
            {
               ((TIM2Texture)texture).Swizzled=value;
            }
        }
    }
}
