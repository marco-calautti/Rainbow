using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats
{
    public abstract class CommonIndexedTextureFormat : CommonTextureFormat
    {
        protected IList<Color[]> palettes;
        protected IList<byte[]> encodedPalettes;

        protected int[] bpps;

        internal CommonIndexedTextureFormat(byte[] imgData,byte[] palData,int width, int height,int bpp):
            this(new List<byte[]> { imgData }, new List<byte[]> { palData }, new int[] { width }, new int[] { height }, new int[] {bpp})
        {

        }

        internal CommonIndexedTextureFormat(IList<byte[]> imgData, IList<byte[]> palData, int[] widths, int[] heights, int[] bpps):
            base(imgData, widths, heights)
        {
            this.bpps = bpps;
            encodedPalettes = palData;
            palettes=new List<Color[]>(palData.Count);

            for(int frame=0; frame<bpps.Length; frame++)
            {
                PaletteFilter filter=GetPaletteFilter(frame);
                Color[] decoded=PaletteDecoder(frame).DecodeColors(palData[frame]);
                palettes.Add(filter==null? decoded : filter.Defilter(decoded));
            }
        }

        internal CommonIndexedTextureFormat(Image image,int bpp):
            this(new List<Image> { image }, new int[] {bpp})
        {

        }

        internal CommonIndexedTextureFormat(IList<Image> images, int[] bpps):
            base(images)
        {
            this.bpps = bpps;
            palettes = new List<Color[]>(bpps.Length);
            encodedPalettes = new List<byte[]>(bpps.Length);

            for (int i = 0; i < images.Count; i++)
            {
                Image img = images[i];
                IndexedImageEncoder encoder = (IndexedImageEncoder)GetImageEncoder(i, img);

                imagesData.Add(encoder.Encode());
                palettes.Add(encoder.Palettes[0]);
                encodedPalettes.Add(encoder.EncodedPalettes[0]);
            }
        }

        public virtual int Bpp
        {
            get { return bpps[SelectedFrame]; }
        }

        public override int PalettesCount
        {
            get { return 1; }
        }

        internal virtual IList<byte[]> GetPaletteData()
        {
            return encodedPalettes;
        }

        public virtual int[] GetBpps()
        {
            return bpps;
        }

        protected override ImageDecoder GetImageDecoder(int activeFrame)
        {
            return new IndexedImageDecoder(imagesData[activeFrame],
                                           widths[activeFrame],
                                           heights[activeFrame],
                                           GetIndexCodec(activeFrame),
                                           palettes[activeFrame],
                                           GetImageFilter(activeFrame),
                                           GetPaletteFilter(activeFrame));
        }

        protected override ImageEncoder GetImageEncoder(int activeFrame, Image img)
        {
            return new IndexedImageEncoder( new List<Image> { img },
                                            GetIndexCodec(activeFrame),
                                            PixelComparer(activeFrame),
                                            PaletteEncoder(activeFrame),
                                            GetImageFilter(activeFrame),
                                            GetPaletteFilter(activeFrame));
        }

        protected abstract ColorDecoder PaletteDecoder(int activeFrame);
        protected abstract ColorEncoder PaletteEncoder(int activeFrame);

        protected abstract IndexCodec GetIndexCodec(int activeFrame);

        protected virtual IComparer<Color> PixelComparer(int activeFrame) { return null; }
    }
}
