using Rainbow.ImgLib.Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats
{
    public abstract class CommonTextureFormat : TextureFormatBase
    {
        protected IList<byte[]> imagesData;
        protected IList<byte[]> palettesData;
        protected int[] widths;
        protected int[] heights;
        protected int[] bpps;

        internal CommonTextureFormat(IList<byte[]> imgData, IList<byte[]> palData, int[] widths, int[] heights, int[] bpps)
        {
            imagesData = imgData;
            palettesData = palData;
            this.widths = widths;
            this.heights = heights;
            this.bpps = bpps;
        }

        public override abstract string Name
        {
            get;
        }

        public override int Width
        {
            get { return widths[SelectedFrame]; }
        }

        public override int Height
        {
            get { return heights[SelectedFrame]; }
        }

        public virtual int Bpp
        {
            get { return bpps[SelectedFrame]; }
        }

        public override int FramesCount
        {
            get { return bpps.Length; }
        }

        public override int PalettesCount
        {
            get { return 1; }
        }

        protected override System.Drawing.Image GetImage(int activeFrame, int activePalette)
        {
            return new IndexedImageDecoder(imagesData[activeFrame],
                                           widths[activeFrame],
                                           heights[activeFrame],
                                           IndexRetriever(),
                                           PaletteDecoder().DecodeColors(palettesData[activeFrame])).DecodeImage();
        }

        protected abstract ColorDecoder PaletteDecoder();
        protected abstract ColorEncoder PaletteEncoder();

        protected abstract IndexRetriever IndexRetriever();
        protected abstract IndexPacker IndexPacker();
    }
}
