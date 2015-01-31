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

        internal TX48Texture(byte[] imgData,byte[] palData,int width, int height,int bpp):
            base(imgData,palData,width,height,bpp)
        {

        }
        internal TX48Texture(IList<byte[]> imageData, IList<byte[]> paletteData, int[] widths, int[] heights, int[] bpps):
             base(imageData, paletteData, widths, heights, bpps)
        {

        }

        internal TX48Texture(Image image,int bpp):
            base(image,bpp)
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

        protected override ColorDecoder PaletteDecoder(int activeFrame)
        {
            return ColorDecoder.DECODER_32BIT_RGBA;
        }
        protected override ColorEncoder PaletteEncoder(int activeFrame)
        {
            return ColorEncoder.ENCODER_32BIT_RGBA;
        }

        protected override IndexCodec GetIndexCodec(int activeFrame)
        {
            return IndexCodec.FromBitPerPixel(bpps[activeFrame]);
        }
    }
}
