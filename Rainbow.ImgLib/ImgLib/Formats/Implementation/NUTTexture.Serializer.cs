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
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Common;
using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Filters;
using Rainbow.ImgLib.Encoding.Implementation;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public class NUTTextureSerializer : NamcoTextureSerializer
    {

        public override string Name
        {
            get { return "Gamecube NUT Archive"; }
        }

        public override string PreferredFormatExtension
        {
            get { return ".nut"; }
        }

        public override byte[] MagicID
        {
            get { return ASCIIEncoding.ASCII.GetBytes("NUTC"); }
        }

        public override string MetadataID
        {
            get { return "NUTTexture"; }
        }

        protected override ByteOrder GetByteOrder()
        {
            return ByteOrder.BigEndian;
        }

        protected override bool IsSupportedVersion(ushort version)
        {
            return version == 0x8002;
        }

        protected override bool IsSupportedFormat(byte format)
        {
            return format == 0;
        }

        protected override int GetMainHeaderSize()
        {
            return 0x20;
        }

        protected override bool IsPaletted(byte clutFormat)
        {
            return clutFormat != 0;
        }

        protected override int GetPaletteColorSize(byte clutFormat)
        {
            return 2;
        }

        protected override void GetPalettedTools(ushort version, byte clutFormat, byte depth, int colorsCount, int width, int height, byte[] data, byte[] userData, 
                                                 out Encoding.ColorCodec paletteCodec, 
                                                 out Encoding.IndexCodec indexCodec, 
                                                 out Filters.ImageFilter imgFilter, 
                                                 out Filters.PaletteFilter palFilter)
        {
            int bpp;
            switch (depth)
            {
                case 5: //C4
                    bpp = 4;
                    break;
                case 6: //C8
                    bpp = 8;
                    break;
                default:
                    throw new TextureFormatException("Unsupported depth " + depth);
            }
            switch (clutFormat)
            {
                case 1:
                    paletteCodec = ColorCodec.CODEC_16BITBE_RGB565;
                    break;
                case 2:
                    paletteCodec = ColorCodec.CODEC_16BITBE_RGB5A3;
                    break;
                case 3:
                    paletteCodec = ColorCodec.CODEC_16BITBE_IA8;
                    break;
                case 0xB: //not sure about this (FIX)
                    paletteCodec = ColorCodec.CODEC_16BITBE_IA8;
                    break;
                default:
                    throw new TextureFormatException("Unsupported clut format " + clutFormat);
            }

            indexCodec = IndexCodec.FromBitPerPixel(bpp, ByteOrder.BigEndian);
            imgFilter = new TileFilter(bpp, 8, 32 / bpp, width, height);
            palFilter = null;
        }

        protected override void GetUnpalettedTools(ushort version, byte clutFormat, byte depth, int colorsCount, int width, int height, byte[] data, byte[] userData, 
                                                    out Encoding.ColorCodec imageCodec, 
                                                    out Filters.ImageFilter imgFilter)
        {
            switch (depth)
            {
                case 3: //RGBA32bit 2 planes
                    imageCodec = ColorCodec.CODEC_32BIT_ARGB;
                    imgFilter = new ImageFilterComposer { new GamecubePlanarFilter(), new TileFilter(32, 4, 4, width, height) };
                    break;
                case 4: //DXT1
                    imageCodec = new ColorCodecDXT1Gamecube(width, height);
                    imgFilter = null;
                    break;
                case 0xA: // I8
                    imageCodec = ColorCodec.CODEC_8BIT_I8;
                    imgFilter=new TileFilter(8, 8, 4, width, height);
                    break;
                case 0xB: // IA8
                    imageCodec = ColorCodec.CODEC_16BITBE_IA8;
                    imgFilter=new TileFilter(16, 4, 4, width, height);
                    break;
                default:
                    throw new TextureFormatException("Usupported unpalletted image format " + depth);
            }
        }
    }
}
