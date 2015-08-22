using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats
{
    internal class PalettedTextureFormat : TextureFormatBase
    {
        protected byte[] imageData;
        protected IList<Color[]> palettes;
        protected IList<byte[]> encodedPalettes;

        protected int width;
        protected int height;
        protected int bpp;

        private PalettedTextureFormat() { }

        private void Init(byte[] imgData,byte[] palData, int width, int height)
        {
            Init(imgData, new List<byte[]> { palData }, width, height);
        }

        private void Init(byte[] imgData, IList<byte[]> palData, int widths, int heights)
        {
            imageData = imgData;
            this.width = widths;
            this.height = heights;

            encodedPalettes = palData;
            palettes=new List<Color[]>(palData.Count);

            for(int pal=0; pal<palData.Count; pal++)
            {
                PaletteFilter filter=PaletteFilter;
                Color[] decoded=PaletteDecoder.DecodeColors(palData[pal]);
                palettes.Add(filter==null? decoded : filter.Defilter(decoded));
            }

        }


        private void Init(Image image)
        {
            Init(new List<Image> { image });

        }

        private void Init(IList<Image> images)
        {

            encodedPalettes = new List<byte[]>(images.Count);

            width = images.First().Width;
            height = images.First().Height;


            IndexedImageEncoder encoder = new IndexedImageEncoder(images, 
                                                                  IndexCodec,
                                                                  PixelComparer,
                                                                  PaletteEncoder,
                                                                  ImageFilter,
                                                                  PaletteFilter);
                imageData = encoder.Encode();
                palettes = encoder.Palettes;

                encodedPalettes = encoder.EncodedPalettes;
        }

        public override string Name
        {
            get { return "Paletted Texture Format"; }
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
            get { return IndexCodec.BitDepth; }
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
                                           IndexCodec,
                                           palettes[activePalette],
                                           ImageFilter,
                                           PaletteFilter).DecodeImage();
        }

        internal byte[] GetImageData()
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
                                           IndexCodec,
                                           palettes[0],
                                           ImageFilter,
                                           PaletteFilter).ReferenceImage;
        }

        public ColorDecoder PaletteDecoder { get; private set; }
        public ColorEncoder PaletteEncoder { get; private set; }

        public IndexCodec IndexCodec { get; private set; }

        public ImageFilter ImageFilter { get; private set; }
        public PaletteFilter PaletteFilter { get; private set; }

        public IComparer<Color> PixelComparer { get; private set; }



        public sealed class Builder
        {
            private PalettedTextureFormat texture=new PalettedTextureFormat();
            public Builder SetPaletteDecoder(ColorDecoder decoder)
            {
                texture.PaletteDecoder = decoder;
                return this;
            }

            public Builder SetPaletteEncoder(ColorEncoder encoder)
            {
                texture.PaletteEncoder = encoder;
                return this;
            }

            public Builder SetIndexCodec(IndexCodec codec)
            {
                texture.IndexCodec = codec;
                return this;
            }

            public Builder SetImageFilter(ImageFilter filter)
            {
                texture.ImageFilter = filter;
                return this;
            }

            public Builder SetPaletteFilter(PaletteFilter filter)
            {
                texture.PaletteFilter = filter;
                return this;
            }

            public Builder SetPixelComparer(IComparer<Color> comparer)
            {
                texture.PixelComparer = comparer;
                return this;
            }

            public PalettedTextureFormat Build(byte[] imgData, byte[] palData, int width, int height)
            {
                texture.Init(imgData, palData, width, height);
                return texture;
            }

            public PalettedTextureFormat Build(byte[] imgData, IList<byte[]> palData, int width, int height)
            {
                texture.Init(imgData, palData, width, height);
                return texture;
            }

            public PalettedTextureFormat Build(Image image)
            {
                texture.Init(image);
                return texture;
            }

            public PalettedTextureFormat Build(IList<Image> images)
            {
                texture.Init(images);
                return texture;
            }
        }
    }
}
