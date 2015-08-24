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

using Rainbow.ImgLib.Formats;
using System.ComponentModel;

namespace Rainbow.App.GUI.Model
{
    public class TexturePropertyGridObject
    {

        protected TextureFormat texture;

        protected const string  CATEGORY_GENERAL = "General",
                                CATEGORY_SPECIFIC="Format Specific";

        public TexturePropertyGridObject(TextureFormat texture)
        {
            this.texture = texture;
        }

        [CategoryAttribute(CATEGORY_GENERAL)]
        [DisplayName("Texture Format")]
        [DescriptionAttribute("The format type of this texture.")]
        public string Name
        {
            get { return texture.Name; }
        }

        [CategoryAttribute(CATEGORY_GENERAL)]
        [DescriptionAttribute("The width in pixel of this texture.")]
        [DisplayName("Width")]
        public int Width
        {
            get { return texture.Width; }
        }

        [CategoryAttribute(CATEGORY_GENERAL)]
        [DescriptionAttribute("The height in pixel of this texture.")]
        [DisplayName("Height")]
        public int Height
        {
            get { return texture.Height; }
        }

        [CategoryAttribute(CATEGORY_GENERAL)]
        [DescriptionAttribute("The total number of frames in this texture.")]
        [DisplayName("Total Frames")]
        public int FramesCount
        {
            get
            {
                return texture.FramesCount;
            }
        }

        [CategoryAttribute(CATEGORY_GENERAL)]
        [DescriptionAttribute("The total number of palettes associated to the currently selected frame.")]
        [DisplayName("Total Palettes")]
        public int PalettesCount
        {
            get
            {
                return texture.PalettesCount;
            }
        }

        [CategoryAttribute(CATEGORY_GENERAL)]
        [DescriptionAttribute("The total number of mipmaps associated to the currently selected frame.")]
        [DisplayName("Total Mipmaps")]
        public int MipmapsCount
        {
            get
            {
                return texture.MipmapsCount;
            }
        }

        [CategoryAttribute(CATEGORY_GENERAL)]
        [TypeConverter(typeof(RangedTypeConveterFrames))]
        [DescriptionAttribute("Selects the desired frame.")]
        [DisplayName("Select Frame")]
        public int SelectedFrame
        {
            get
            {
                return texture.SelectedFrame;
            }
            set
            {
                texture.SelectedFrame = value;
            }
        }

        [CategoryAttribute(CATEGORY_GENERAL)]
        [TypeConverter(typeof(RangedTypeConveterPalettes))]
        [DescriptionAttribute("Selects the desired palette for the currently selected frame.")]
        [DisplayName("Select Palette")]
        public int SelectedPalette
        {
            get
            {
                return texture.SelectedPalette;
            }
            set
            {

                texture.SelectedPalette = value;
            }
        }

        [Browsable(false)]
        public TextureFormat Texture
        {
            get { return texture; }
        }
    }
}
