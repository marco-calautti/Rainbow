//Copyright (C) 2014+ Marco (Phoenix) Calautti.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 2.0.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License 2.0 for more details.

//A copy of the GPL 2.0 should have been included with the program.
//If not, see http://www.gnu.org/licenses/

//Official repository and contact information can be found at
//http://github.com/marco-calautti/Rainbow

using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats
{
    /// <summary>
    /// This TextureFormat represents image data with palettes. To construct an instance,
    /// use the inner Builder class. It allows to properly set the image index codec,
    /// palette color codec, an (optional) image filter, an (optional) palette filter, 
    /// an (optional) pixel comparer for ordering the palettes colors when rebuilding the texture
    /// to raw form, and the (optional) mipmap count.
    /// </summary>
    internal class PalettedTextureFormat : TextureFormatBase
    {
        protected byte[] imageData;
        protected IList<Color[]> palettes;
        protected IList<byte[]> encodedPalettes;

        protected int width;
        protected int height;

        private PalettedTextureFormat(int mipmapsCount) :
            base(mipmapsCount) { }

        private void Init(byte[] imgData, byte[] palData, int width, int height)
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
                Color[] decoded=PaletteCodec.DecodeColors(palData[pal]);
                palettes.Add(filter==null? decoded : filter.Defilter(decoded));
            }

        }


        private void Init(Image image)
        {
            width = image.Width;
            height = image.Height;

            ImageEncoderIndexed encoder = new ImageEncoderIndexed(image, 
                                                                  IndexCodec,
                                                                  PixelComparer,
                                                                  PaletteCodec,
                                                                  ImageFilter,
                                                                  PaletteFilter);
             imageData = encoder.Encode();
             palettes = encoder.Palettes;

             encodedPalettes = encoder.EncodedPalettes;
        }

        private void Init(Image referenceImage, IList<Color[]> palettes)
        {
            width = referenceImage.Width;
            height = referenceImage.Height;

            ImageEncoderIndexed encoder = new ImageEncoderIndexed(palettes, 
                                                                  referenceImage,
                                                                  IndexCodec,
                                                                  PaletteCodec,
                                                                  ImageFilter,
                                                                  PaletteFilter);
            imageData = encoder.Encode();
            this.palettes = encoder.Palettes;

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
            return new ImageDecoderIndexed(imageData,
                                           width,
                                           height,
                                           IndexCodec,
                                           palettes[activePalette],
                                           ImageFilter,
                                           PaletteFilter).DecodeImage();
        }

        protected override Color[] GetPalette(int activePalette)
        {
            return palettes[activePalette];
        }

        public byte[] GetImageData()
        {
            return imageData;
        }

        public IList<byte[]> GetPaletteData()
        {
            return encodedPalettes;
        }

        public override Image GetReferenceImage()
        {
            if (PalettesCount <= 1)
            {
                return null;
            }

            return new ImageDecoderIndexed(imageData,
                                           width,
                                           height,
                                           IndexCodec,
                                           palettes[0],
                                           ImageFilter,
                                           PaletteFilter).ReferenceImage;
        }

        public ColorCodec PaletteCodec { get; private set; }

        public IndexCodec IndexCodec { get; private set; }

        public ImageFilter ImageFilter { get; private set; }
        public PaletteFilter PaletteFilter { get; private set; }

        public IComparer<Color> PixelComparer { get; private set; }



        public sealed class Builder
        {
            private PalettedTextureFormat texture;
            private ColorCodec decoder;
            private IndexCodec codec;
            private ImageFilter imgFilter;
            private PaletteFilter palFilter;
            private IComparer<Color> comparer;
            private int mipmaps=1;

            public Builder SetPaletteCodec(ColorCodec decoder)
            {
                this.decoder = decoder;
                return this;
            }

            public Builder SetIndexCodec(IndexCodec codec)
            {
                this.codec = codec;
                return this;
            }

            public Builder SetImageFilter(ImageFilter filter)
            {
                imgFilter = filter;
                return this;
            }

            public Builder SetPaletteFilter(PaletteFilter filter)
            {
                palFilter = filter;
                return this;
            }

            public Builder SetColorComparer(IComparer<Color> comparer)
            {
                this.comparer = comparer;
                return this;
            }

            public Builder SetMipmapsCount(int mipmaps)
            {
                this.mipmaps = mipmaps;
                return this;
            }

            public PalettedTextureFormat Build(byte[] imgData, byte[] palData, int width, int height)
            {
                CreateTexture();
                texture.Init(imgData, palData, width, height);
                return texture;
            }

            public PalettedTextureFormat Build(byte[] imgData, IList<byte[]> palData, int width, int height)
            {
                CreateTexture();
                texture.Init(imgData, palData, width, height);
                return texture;
            }

            public PalettedTextureFormat Build(Image image)
            {
                CreateTexture();
                texture.Init(image);
                return texture;
            }

            public PalettedTextureFormat Build(Image referenceImage, IList<Color[]> palettes)
            {
                CreateTexture();
                texture.Init(referenceImage,palettes);
                return texture;
            }

            private void CreateTexture()
            {
                texture = new PalettedTextureFormat(mipmaps);
                texture.PaletteCodec = decoder;
                texture.IndexCodec = codec;
                texture.ImageFilter = imgFilter;
                texture.PaletteFilter = palFilter;
                texture.PixelComparer = comparer;
            }
        }
    }
}
