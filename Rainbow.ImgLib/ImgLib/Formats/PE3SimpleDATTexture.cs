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
    public class PE3SimpleDATTexture : TextureFormatBase
    {
        internal static readonly string NAME = "The 3rd Birthday Simple DAT";

        private byte[] rawHeader, imageData;
        private int width, height;

        internal PE3SimpleDATTexture(byte[] rawHeader,byte[] imageData)
        {
            this.rawHeader = rawHeader;
            width = 512;
            height = imageData.Length * 2 / width;

            this.imageData = new SwizzleFilter(width, height, 4).Defilter(imageData);
        }

        internal PE3SimpleDATTexture(byte[] rawHeader, Image img)
        {
            this.rawHeader = rawHeader;
            if(img.Width!=512)
                throw new TextureFormatException("Only a width of 512 pixel is allowed!");

            width = img.Width;
            height = img.Height;

            IndexedImageEncoder encoder = new IndexedImageEncoder(new List<Image> { img }, 16);
            imageData = encoder.Encode();
        }
        public override string Name
        {
            get { return NAME; }
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
            get { return 0; }
        }

        internal byte[] GetRawHeader()
        {
            return rawHeader;
        }

        internal byte[] GetImageData()
        {
            return new SwizzleFilter(Width,Height,4).ApplyFilter(imageData);
        }

        protected override System.Drawing.Image GetImage(int activeFrame, int activePalette)
        {

            IndexedImageDecoder decoder = new IndexedImageDecoder(imageData, Width, Height, new IndexRetriever4Bpp());
            return decoder.DecodeImage();
        }
    }
}
