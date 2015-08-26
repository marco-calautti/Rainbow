using Rainbow.ImgLib.Formats.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Rainbow.App.GUI.Model
{
    public class TPLTexturePropertyGridObject : TexturePropertyGridObject
    {
        public TPLTexturePropertyGridObject(TPLTexture texture):
            base(texture) { }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The format parameter of this texture.")]
        [DisplayName("Format")]
        public string Format
        {
            get
            {
                return ((TPLTexture)texture).TextureFormats[texture.SelectedFrame].FormatSpecificData.Get(TPLTextureSerializer.FORMAT_KEY);
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The palette format parameter of this texture.")]
        [DisplayName("Palette Format")]
        public string PaletteFormat
        {
            get
            {
                return ((TPLTexture)texture).TextureFormats[texture.SelectedFrame].FormatSpecificData.Get(TPLTextureSerializer.PALETTEFORMAT_KEY);
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("This value is unknown.")]
        [DisplayName("Unknown parameter")]
        public string Unknown
        {
            get
            {
                return ((TPLTexture)texture).TextureFormats[texture.SelectedFrame].FormatSpecificData.Get(TPLTextureSerializer.UNKNOWN_KEY);
            }
        }
    }
}
