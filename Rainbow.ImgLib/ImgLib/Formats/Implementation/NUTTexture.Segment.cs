using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Filters;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public partial class NUTTexture : TextureContainer
    {
		internal class PalettedSegment : PalettedTextureFormat
        {
            private NUTSegmentParameters parameters;

			internal PalettedSegment(byte[] imgData, byte[] palData, NUTSegmentParameters parameters):
                this(imgData, new List<byte[]>() { palData }, parameters) { }

            internal PalettedSegment(byte[] imgData, IList<byte[]> palData, NUTSegmentParameters parameters) :
                base(imgData, palData, parameters.width, parameters.height, parameters.bpp) 
            {
                this.parameters = parameters;
            }

            internal PalettedSegment(Image img, NUTSegmentParameters parameters) :
                this(new List<Image>() { img }, parameters) { }

            internal PalettedSegment(IList<Image> img, NUTSegmentParameters parameters) :
                base(img, parameters.bpp) 
            {
                this.parameters = parameters;
            }

            public override string Name
            {
                get { return "NUT Paletted Segment"; }
            }

            protected override ColorDecoder PaletteDecoder()
            {
                return ColorDecoder.DECODER_16BITLE_ABGR;
            }

            protected override ColorEncoder PaletteEncoder()
            {
                return ColorEncoder.ENCODER_16BITLE_ABGR;
            }

            protected override IndexCodec GetIndexCodec()
            {
                return IndexCodec.FromBitPerPixel(parameters.bpp, IndexCodec.ByteOrder.BigEndian);
            }

            protected override ImageFilter GetImageFilter()
            {
                return new TileFilter(parameters.bpp, 8, 4, parameters.width, parameters.height);
            }
        }
    }
}
