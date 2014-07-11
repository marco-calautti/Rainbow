using Rainbow.ImgLib.Formats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
