using Rainbow.ImgLib.Formats.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Rainbow.App.GUI.Model
{
    public class NamcoTexturePropertyGridObject : TexturePropertyGridObject
    {
        public NamcoTexturePropertyGridObject(NamcoTexture texture ):
            base(texture) { }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The version number of this texture.")]
        [DisplayName("Version")]
        public string Version
        {
            get
            {
                return ((NamcoTexture)texture).Version;
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The depth parameter of this texture.")]
        [DisplayName("Depth")]
        public string Depth
        {
            get
            {
                return ((NamcoTexture)texture).Depth;
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The format parameter of this texture.")]
        [DisplayName("Format")]
        public string Format
        {
            get
            {
                return ((NamcoTexture)texture).Format;
            }
        }

        [CategoryAttribute(CATEGORY_SPECIFIC)]
        [DescriptionAttribute("The clut format parameter of this texture.")]
        [DisplayName("Clut Format")]
        public string ClutFormat
        {
            get
            {
                return ((NamcoTexture)texture).ClutFormat;
            }
        }
    }
}
