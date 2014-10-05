using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Rainbow.ImgLib.Encoding;

namespace Rainbow.ImgLib.Formats
{
    public class TX48Texture : CommonTextureFormat
    {
        internal const string NAME = "TX48 (Super Robot Wars MX P)";

        internal TX48Texture(IList<byte[]> imageData, IList<byte[]> paletteData, int[] widths, int[] heights, int[] bpps):
             base(imageData, paletteData, widths, heights, bpps)
        {

        }

        internal TX48Texture(IList<Image> images,int[] bpps):
             base(images,bpps)
        {

        }

        public override string Name
        {
            get { return NAME; }
        }

        protected override ColorDecoder PaletteDecoder()
        {
            return ColorDecoder.DECODER_32BIT_RGBA;
        }
        protected override ColorEncoder PaletteEncoder()
        {
            return ColorEncoder.ENCODER_32BIT_RGBA;
        }

        protected override IndexRetriever IndexRetriever()
        {
            if (bpps[SelectedFrame] == 4)
                return new IndexRetriever4Bpp();
            else if (bpps[SelectedFrame] == 8)
                return new IndexRetriever8Bpp();
            else
                throw new TextureFormatException("Illegal bpp value!");
        }

        protected override IndexPacker IndexPacker()
        {
            if (bpps[SelectedFrame] == 4)
                return new IndexPacker4Bpp();
            else if (bpps[SelectedFrame] == 8)
                return new IndexPacker8Bpp();
            else
                throw new TextureFormatException("Illegal bpp value!");
        }
    }
}
