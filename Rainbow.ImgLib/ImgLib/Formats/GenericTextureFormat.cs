using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats
{
    public class GenericTextureFormat : TextureFormatBase
    {
        private ImageDecoder decoder;
        protected byte[] imageData;

        protected int width;
        protected int height;

        private GenericTextureFormat(int mipmaps=1):
            base(mipmaps){}

        private void Init(byte[] imgData, int width, int height)
        {
            this.imageData=imgData;
            this.width=width;
            this.height=height;

            decoder = new ImageDecoderDirectColor(imageData, width, height, ColorCodec, ImageFilter);
        }

        public byte[] GetImageData()
        {
            return imageData;
        }

        public ColorCodec ColorCodec
        {
            get;
            private set;
        }

        public ImageFilter ImageFilter
        {
            get;
            private set;
        }

        public override string Name
        {
            get { return "Generic Texture Format"; }
        }

        public override int Width
        {
            get { return width; }
        }

        public override int Height
        {
            get { return height; }
        }

        public override int FramesCount
        {
            get { return 1; }
        }

        public override int PalettesCount
        {
            get {return 0; }
        }

        public override System.Drawing.Image GetReferenceImage()
        {
            return null;
        }

        protected override System.Drawing.Image GetImage(int activeFrame, int activePalette)
        {
            return decoder.DecodeImage();
        }

        public class Builder
        {
            private GenericTextureFormat texture;
            private ColorCodec decoder;
            private ImageFilter filter;
            private int mipmaps = 1;

            public Builder SetColorCodec(ColorCodec decoder)
            {
                this.decoder = decoder;
                return this;
            }

            public Builder SetImageFilter(ImageFilter filter)
            {
                this.filter = filter;
                return this;
            }

            public Builder SetMipmapsCount(int mipmaps)
            {
                this.mipmaps = mipmaps;
                return this;
            }

            public GenericTextureFormat Build(byte[] imgData, int width, int height)
            {
                CreateTexture();
                texture.Init(imgData, width, height);
                return texture;
            }

            private void CreateTexture()
            {
                texture= new GenericTextureFormat(mipmaps);
                texture.ImageFilter = filter;
                texture.ColorCodec = decoder;
            }
        }
    }
}
