using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats
{
    internal abstract class PalettedTextureFormat : TextureFormatBase
    {
        protected byte[] imageData;
        protected IList<Color[]> palettes;
        protected IList<byte[]> encodedPalettes;

        protected int width;
        protected int height;
        protected int bpp;

        private PalettedTextureFormat() { }

        internal PalettedTextureFormat(byte[] imgData,byte[] palData,int width, int height,int bpp):
            this(imgData, new List<byte[]> { palData },  width , height ,bpp)
        {

        }

        internal PalettedTextureFormat(byte[] imgData, IList<byte[]> palData, int widths, int heights, int bpps)
        {
            imageData = imgData;
            this.width = widths;
            this.height = heights;
            this.bpp = bpps;

            encodedPalettes = palData;
            palettes=new List<Color[]>(palData.Count);

            for(int pal=0; pal<palData.Count; pal++)
            {
                PaletteFilter filter=GetPaletteFilter();
                Color[] decoded=PaletteDecoder().DecodeColors(palData[pal]);
                palettes.Add(filter==null? decoded : filter.Defilter(decoded));
            }

        }


        internal PalettedTextureFormat(Image image,int bpp):
            this(new List<Image>{image}, bpp)
        {

        }

        internal PalettedTextureFormat(IList<Image> images, int bpps)
        {

            encodedPalettes = new List<byte[]>(images.Count);

            this.bpp = bpps;

            width = images.First().Width;
            height = images.First().Height;


            IndexedImageEncoder encoder = new IndexedImageEncoder(images, 
                                                                      GetIndexCodec(),
                                                                      PixelComparer(),
                                                                      PaletteEncoder(),
                                                                      GetImageFilter(),
                                                                      GetPaletteFilter());
                imageData = encoder.Encode();
                palettes = encoder.Palettes;

                encodedPalettes = encoder.EncodedPalettes;
        }

        public override abstract string Name
        {
            get;
        }

        public override int Width
        {
            get { return width; }
        }

        public override int Height
        {
            get { return height; }
        }

        public virtual int Bpp
        {
            get { return bpp; }
        }

        public override int FramesCount
        {
            get { return 1; }
        }

        public override int PalettesCount
        {
            get { return palettes.Count; }
        }

        protected override System.Drawing.Image GetImage(int activeFrame, int activePalette)
        {
            return new IndexedImageDecoder(imageData,
                                           width,
                                           height,
                                           GetIndexCodec(),
                                           palettes[activePalette],
                                           GetImageFilter(),
                                           GetPaletteFilter()).DecodeImage();
        }

        internal byte[] GetImagesData()
        {
            return imageData;
        }

        internal IList<byte[]> GetPaletteData()
        {
            return encodedPalettes;
        }

        public override Image GetReferenceImage()
        {
            if (PalettesCount <= 1)
                return null;

            return new IndexedImageDecoder(imageData,
                                           width,
                                           height,
                                           GetIndexCodec(),
                                           palettes[0],
                                           GetImageFilter(),
                                           GetPaletteFilter()).ReferenceImage;
        }

        protected abstract ColorDecoder PaletteDecoder();
        protected abstract ColorEncoder PaletteEncoder();

        protected abstract IndexCodec GetIndexCodec();

        protected virtual ImageFilter GetImageFilter(){ return null; }
        protected virtual PaletteFilter GetPaletteFilter() { return null; }

        protected virtual IComparer<Color> PixelComparer() { return null; }
    }
}
