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

using Rainbow.ImgLib.Common;
using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Encoding.Implementation;
using Rainbow.ImgLib.Filters;
using Rainbow.ImgLib.Formats.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public class TPLTextureSerializer : SimpleTextureFormatSerializer<TPLTexture>
    {
        private static readonly byte[] magic = new byte[] { 0x00, 0x20, 0xAF, 0x30 };

        public static readonly string FORMAT_KEY = "Format",
                                      PALETTEFORMAT_KEY = "PaletteFormat",
                                      UNKNOWN_KEY = "Unknown";
        public override string Name
        {
            get { return TPLTexture.NAME; }
        }

        public override string PreferredFormatExtension
        {
            get { return ".tpl"; }
        }

        public override TextureFormat Open(System.IO.Stream formatData)
        {
            if (!IsValidFormat(formatData))
            {
                throw new TextureFormatException("Not a valid TPL Texture!");
            }

            BinaryReader reader = new BinaryReader(formatData);
            reader.BaseStream.Position += 4;

            ByteOrder order = ByteOrder.BigEndian;

            int texturesCount = (int)reader.ReadUInt32(order);
            uint imageTableOffset = reader.ReadUInt32(order);

            reader.BaseStream.Position = imageTableOffset;

            TPLTexture texture = new TPLTexture();

            for (int i = 0; i < texturesCount; i++)
            {
                TextureFormat segment = null;

                //image table
                uint imageHeaderOffset = reader.ReadUInt32(order);
                uint offsetPaletteHeader = reader.ReadUInt32(order);

                long oldPos = reader.BaseStream.Position;

                reader.BaseStream.Position = imageHeaderOffset;

                int height = reader.ReadUInt16(order);
                int width = reader.ReadUInt16(order);
                uint format = reader.ReadUInt32(order);
                uint imgDataOffset = reader.ReadUInt32(order);

                ColorCodec colorCodec=null;
                IndexCodec idxCodec=null;
                ImageFilter imgFilter=null;

                ushort entryCount=0;
                ushort unknown=0;//this might be a set of flags denoting whether the palette is internal to the tpl image or external.
                uint paletteFormat=0;
                uint palDataOffset=0;
                bool isIndexed = false;

                int imgDataSize=0;
                int palDataSize=0;

                switch (format)
                {
                    case 0: //I4
                        colorCodec = ColorCodec.CODEC_4BITBE_I4;
                        imgFilter = new TileFilter(4, 8, 8, width, height);
                        imgDataSize = colorCodec.GetBytesNeededForEncode(width, height, imgFilter);
                        break;
                    case 1: //I8
                        colorCodec = ColorCodec.CODEC_8BIT_I8;
                        imgFilter = new TileFilter(8, 8, 4, width, height);
                        imgDataSize = colorCodec.GetBytesNeededForEncode(width, height, imgFilter);
                        break;
                    case 2: //IA4
                        colorCodec = ColorCodec.CODEC_8BITBE_IA4;
                        imgFilter = new TileFilter(8, 8, 4, width, height);
                        imgDataSize = colorCodec.GetBytesNeededForEncode(width, height, imgFilter);
                        break;
                    case 3: //IA8
                        colorCodec = ColorCodec.CODEC_16BITBE_IA8;
                        imgFilter = new TileFilter(16, 4, 4, width, height);
                        imgDataSize = colorCodec.GetBytesNeededForEncode(width, height, imgFilter);
                        break;
                    case 4: //RGB565
                        colorCodec = ColorCodec.CODEC_16BITBE_RGB565;
                        imgFilter = new TileFilter(16, 4, 4, width, height);
                        imgDataSize = colorCodec.GetBytesNeededForEncode(width, height, imgFilter);
                        break;
                    case 5: //RGB5A3
                        colorCodec = ColorCodec.CODEC_16BITBE_RGB5A3;
                        imgFilter = new TileFilter(16, 4, 4, width, height);
                        imgDataSize = colorCodec.GetBytesNeededForEncode(width, height,imgFilter);
                        break;
                    case 6: //RGBA32 2 planes
                        colorCodec = ColorCodec.CODEC_32BIT_ARGB;
                        imgFilter = new ImageFilterComposer{ new GamecubePlanarFilter(),
                                                           new TileFilter(32,4,4,width,height)};
                        imgDataSize = colorCodec.GetBytesNeededForEncode(width, height, imgFilter);
                        break;
                    case 8: //C4
                    case 9: //C8
                        isIndexed = true;
                        reader.BaseStream.Position = offsetPaletteHeader;

                        entryCount = reader.ReadUInt16(order);
                        unknown = reader.ReadUInt16(order);

                        paletteFormat = reader.ReadUInt32(order);
                        palDataOffset = reader.ReadUInt32(order);

                        switch (paletteFormat)
                        {
                            case 0:
                                colorCodec = ColorCodec.CODEC_16BITBE_IA8;
                                break;
                            case 1:
                                colorCodec = ColorCodec.CODEC_16BITBE_RGB565;
                                break;
                            case 2:
                                colorCodec = ColorCodec.CODEC_16BITBE_RGB5A3;
                                break;
                            default:
                                throw new TextureFormatException("Unsupported palette format " + paletteFormat);
                        }
                        palDataSize = colorCodec.GetBytesNeededForEncode(entryCount, 1);

                        if (format == 8)
                        {
                            idxCodec = IndexCodec.FromBitPerPixel(4, order);
                            imgFilter = new TileFilter(4, 8, 8, width, height);
                        }
                        else
                        {
                            idxCodec = IndexCodec.FromBitPerPixel(8, order);
                            imgFilter = new TileFilter(8, 8, 4, width, height);
                        }

                        imgDataSize = idxCodec.GetBytesNeededForEncode(width, height, imgFilter);
                        break;
                    case 0xA: //C14X2
                        throw new TextureFormatException("C14X2 not implemented yet!");
                        break;
                    case 0xE: //DXT1 (aka CMPR)
                        colorCodec = new ColorCodecDXT1Gamecube(width, height);
                        imgDataSize = colorCodec.GetBytesNeededForEncode(width, height);
                        break;
                    default:
                        throw new TextureFormatException("Unsupported TPL image format " + format);
                }

                reader.BaseStream.Position = imgDataOffset;
                byte[] imgData = reader.ReadBytes(imgDataSize);

                if (isIndexed)
                {
                    reader.BaseStream.Position = palDataOffset;
                    byte[] palData = reader.ReadBytes(palDataSize);

                    PalettedTextureFormat.Builder builder = new PalettedTextureFormat.Builder();
                    segment = builder.SetIndexCodec(idxCodec)
                                     .SetPaletteCodec(colorCodec)
                                     .SetImageFilter(imgFilter)
                                     .Build(imgData, palData, width, height);
                }
                else
                {
                    GenericTextureFormat.Builder builder = new GenericTextureFormat.Builder();
                    segment=  builder .SetColorCodec(colorCodec)
                                                    .SetImageFilter(imgFilter)
                                                    .Build(imgData, width, height);
                }

                segment.FormatSpecificData.Put<uint>(FORMAT_KEY, format)
                                          .Put<uint>(UNKNOWN_KEY,unknown)
                                          .Put<uint>(PALETTEFORMAT_KEY,paletteFormat);

                texture.TextureFormats.Add(segment);

                reader.BaseStream.Position = oldPos;

            }

            return texture;
        }

        public override void Save(TextureFormat texture, System.IO.Stream outFormatData)
        {
            throw new NotImplementedException();
        }

        public override byte[] MagicID
        {
            get { return magic; }
        }

        public override string MetadataID
        {
            get { return "TPLTexture"; }
        }

        protected override TextureFormat GetTextureFrame(TPLTexture texture, int frame)
        {
            return texture.TextureFormats[frame];
        }

        protected override TPLTexture CreateGeneralTextureFromFormatSpecificData(GenericDictionary formatSpecificData)
        {
            throw new NotImplementedException();
        }

        protected override TextureFormat CreateFrameForGeneralTexture(TPLTexture texture, int frame, GenericDictionary formatSpecificData, IList<System.Drawing.Image> images, System.Drawing.Image referenceImage, int mipmapsCount)
        {
            throw new NotImplementedException();
        }
    }
}
