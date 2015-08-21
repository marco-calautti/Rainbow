using Rainbow.ImgLib.Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public partial class TX48Texture : TextureContainer
    {
        internal class Segment : PalettedTextureFormat
        {
            internal Segment(byte[] imgData, byte[] palData, int width, int height, int bpp) :
                base(imgData, palData, width, height, bpp) { }

            internal Segment(byte[] imgData, IList<byte[]> palData, int width, int height, int bpp) :
                base(imgData, palData, width, height, bpp) { }

            internal Segment(Image image, int bpp) :
                base(image, bpp) { }

            internal Segment(IList<Image> images, int bpp) :
                base(images, bpp) { }

            public override string Name
            {
                get { return "TX48 Segment"; }
            }

            protected override ColorDecoder PaletteDecoder()
            {
                return ColorDecoder.DECODER_32BIT_RGBA;
            }
            protected override ColorEncoder PaletteEncoder()
            {
                return ColorEncoder.ENCODER_32BIT_RGBA;
            }

            protected override IndexCodec GetIndexCodec()
            {
                return IndexCodec.FromBitPerPixel(bpp);
            }
        }
    }
}
