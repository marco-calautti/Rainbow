using Rainbow.ImgLib.Formats;
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
        [DescriptionAttribute("Denotes if swizzling has to be applied or not to all the frames of this TIM2.")]
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
