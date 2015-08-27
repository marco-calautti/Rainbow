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
