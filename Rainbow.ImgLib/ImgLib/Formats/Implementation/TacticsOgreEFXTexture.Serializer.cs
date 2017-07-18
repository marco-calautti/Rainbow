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
using System.IO;

using Rainbow.ImgLib.Common;
using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Encoding.ColorComparers;
using Rainbow.ImgLib.Filters;
using Rainbow.ImgLib.Formats.Serialization;
using System.Drawing;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public class TacticsOgreEFXTextureSerializer : SimpleTextureFormatSerializer<TacticsOgreEFXTexture>
    {
        public static readonly string RAW_DATA_KEY = "RawData";
        public static readonly string SCALE_KEY = "Scale";

        public static readonly string UNK1_KEY = "Unk1";
        public static readonly string UNK2_KEY = "Unk2";
        public static readonly string ID_KEY = "Id";
        public static readonly string ENTRY_TYPE_KEY = "EntryType";
        public static readonly string UNK3_KEY = "Unk3";
        public static readonly string BPP_KEY = "Bpp";
        public static readonly string UNK4_KEY = "Unk4";
        public static readonly string UNK5_KEY = "Unk5";
        public static readonly string UNK6_KEY = "Unk6";
        public static readonly string ENTRY_NO_HEADER_KEY = "NoHeaderKey";

        private static readonly byte[] magic = ASCIIEncoding.ASCII.GetBytes("EFX0011");

        public override string Name
        {
            get { return TacticsOgreEFXTexture.NAME; }
        }

        public override string PreferredFormatExtension
        {
            get { return ".efx"; }
        }

        public override byte[] MagicID
        {
            get { return magic; }
        }

        public override string MetadataID
        {
            get { return "TOEFX0011"; }
        }

        public override TextureFormat Open(System.IO.Stream formatData)
        {
            if (!IsValidFormat(formatData))
            {
                throw new TextureFormatException("Not a valid EFX image archive!");
            }

            TacticsOgreEFXTexture texture = new TacticsOgreEFXTexture();

            BinaryReader reader = new BinaryReader(formatData);

            //read magic and discard
            reader.ReadBytes(8);

            float scale = reader.ReadSingle();

            uint fileSize = reader.ReadUInt32();

            texture.FormatSpecificData.Put<float>(SCALE_KEY, scale);

            while (reader.BaseStream.Position < fileSize)
            {
                TextureFormat segment = ReadEntry(reader);
                texture.TextureFormats.Add(segment);
            }

            return texture;
        }

        public override void Save(TextureFormat texture, System.IO.Stream outFormatData)
        {
            TacticsOgreEFXTexture efxTexture = texture as TacticsOgreEFXTexture;
            if (efxTexture == null)
            {
                throw new TextureFormatException("Not a valid EFX Testure!");
            }

            BinaryWriter writer = new BinaryWriter(new MemoryStream());

            writer.Write(magic);
            writer.Write((byte)0);

            writer.Write(efxTexture.FormatSpecificData.Get<float>(SCALE_KEY));

            writer.Write((uint)0); //temporarily write dummy file size

            int fileSize = 0x10;

            foreach (TextureFormat segment in efxTexture.TextureFormats)
            {
                byte[] entryData = null;
                int fullEntrySize = 0;
                int sizeOfEntryNoHeader1 = 0;

                byte[] imgData = null, palData = null;
                PalettedTextureFormat palSegment = segment as PalettedTextureFormat;
                DummyTexture dummySegment = segment as DummyTexture;

                if (palSegment != null)
                {
                    imgData = palSegment.GetImageData();
                    IList<byte[]> palettes = palSegment.GetPaletteData();
                    if (palettes.Count > 1)
                        throw new TextureFormatException("EFX should not support multi palette images!");

                    palData = palettes.First();

                    fullEntrySize = 0x30 + imgData.Length + palData.Length;

                    entryData = imgData.Concat(palData).ToArray();
                }
                else if (dummySegment != null)
                {
                    entryData = dummySegment.FormatSpecificData.Get<byte[]>(RAW_DATA_KEY);

                    fullEntrySize = 0x10 + entryData.Length;
                }
                else
                {
                    throw new TextureFormatException("EFX segments should be Paletted or dummy!");
                }

                writer.Write(fullEntrySize);

                writer.Write(segment.FormatSpecificData.Get<ushort>(UNK1_KEY));
                writer.Write(segment.FormatSpecificData.Get<ushort>(UNK2_KEY));

                sizeOfEntryNoHeader1 = fullEntrySize - 0x10;

                if (palSegment != null)
                {
                    writer.Write((uint)sizeOfEntryNoHeader1);
                }
                else
                {
                    writer.Write(segment.FormatSpecificData.Get<uint>(ENTRY_NO_HEADER_KEY));
                }

                writer.Write(segment.FormatSpecificData.Get<ushort>(ID_KEY));
                writer.Write(segment.FormatSpecificData.Get<byte>(ENTRY_TYPE_KEY));
                writer.Write(segment.FormatSpecificData.Get<byte>(UNK3_KEY));

                if (palSegment != null)
                {
                    byte bpp = segment.FormatSpecificData.Get<byte>(BPP_KEY);
                    writer.Write(bpp);
                    writer.Write(segment.FormatSpecificData.Get<byte>(UNK4_KEY));
                    writer.Write(segment.FormatSpecificData.Get<ushort>(UNK5_KEY));
                    writer.Write((ushort)(1 << bpp));
                    writer.Write((ushort)segment.Width);
                    writer.Write((ushort)segment.Height);
                    writer.Write((ushort)segment.Width);
                    writer.Write((ushort)segment.Height);
                    writer.Write((ushort)0);
                    writer.Write(segment.FormatSpecificData.Get<uint>(UNK6_KEY));

                    int header2AndImgSize = 0x20 + imgData.Length;

                    writer.Write((uint)header2AndImgSize);
                    writer.Write((uint)sizeOfEntryNoHeader1);
                    writer.Write((uint)0);
                }

                writer.Write(entryData);

                fileSize += fullEntrySize;

                if (fullEntrySize % 4 != 0)
                {
                    fileSize = fileSize + 4 - fullEntrySize % 4;
                    writer.Write(Enumerable.Repeat((byte)0, 4 - fullEntrySize % 4).ToArray());
                }
            }
            writer.BaseStream.Position = 0x0C;
            writer.Write((uint)fileSize);

            byte[] finalData = (writer.BaseStream as MemoryStream).ToArray();
            outFormatData.Write(finalData, 0, finalData.Length);

            writer.Close();
        }


        protected override TextureFormat GetTextureFrame(TacticsOgreEFXTexture texture, int frame)
        {
            return texture.TextureFormats[frame];
        }

        protected override TacticsOgreEFXTexture CreateGeneralTextureFromFormatSpecificData(GenericDictionary formatSpecificData)
        {
            TacticsOgreEFXTexture texture = new TacticsOgreEFXTexture();
            return texture;
        }

        protected override TextureFormat CreateFrameForGeneralTexture(TacticsOgreEFXTexture texture, int frame, GenericDictionary formatSpecificData, IList<System.Drawing.Image> images, System.Drawing.Image referenceImage, int mipmapsCount)
        {
            if (referenceImage != null || images.Count > 1)
            {
                throw new TextureFormatException("EFX texture should not contain multiple palettes!");
            }

            TextureFormat segment = null;

            var image = images.First();
            byte entryType = formatSpecificData.Get<byte>(ENTRY_TYPE_KEY);

            if (entryType != 0x52)
            {
                segment = new DummyTexture(string.Format("Data entry, type=0x{0:X}", entryType));
            }
            else
            {
                byte bpp = formatSpecificData.Get<byte>(BPP_KEY);

                segment = new PalettedTextureFormat.Builder()
                              .SetIndexCodec(IndexCodec.FromBitPerPixel(bpp))
                              .SetImageFilter(new SwizzleFilter(image.Width, image.Height, bpp))
                              .SetPaletteCodec(ColorCodec.CODEC_32BIT_RGBA)
                              .SetColorComparer(new ARGBColorComparer())
                              .Build(image); 
            }

            texture.TextureFormats.Add(segment);

            return segment;
        }

        private TextureFormat ReadEntry(BinaryReader reader)
        {
            TextureFormat segment = null;

            uint fullEntrySize = reader.ReadUInt32();
            ushort unk1 = reader.ReadUInt16();
            ushort unk2 = reader.ReadUInt16();

            uint sizeEntryNotHeader = reader.ReadUInt32();

            ushort id = reader.ReadUInt16();
            byte entryType = reader.ReadByte();
            byte unk3 = reader.ReadByte();


            if (entryType != 0x52) // non-image entry
            {
                //let's copy the raw data and put it in a dummy texture
                byte[] buf = reader.ReadBytes((int)fullEntrySize - 0x10);

                segment = new DummyTexture(string.Format("Data entry, type=0x{0:X}", entryType));

                segment.FormatSpecificData.Put<byte[]>(RAW_DATA_KEY, buf);
                segment.FormatSpecificData.Put<uint>(ENTRY_NO_HEADER_KEY, sizeEntryNotHeader);
            }
            else //image data, let's read header 2 data
            {
                if (fullEntrySize - sizeEntryNotHeader != 0x10)
                {
                    throw new TextureFormatException("Not a valid EFX file, full size and size without header 1 do not match!");
                }

                byte bpp = reader.ReadByte();
                if (bpp != 4 && bpp != 8)
                {
                    throw new TextureFormatException("Not a valid EFX file, unsupported bpp=" + bpp);
                }

                byte unk4 = reader.ReadByte();
                if (unk4 != 0x20)
                {
                    throw new TextureFormatException("Not a valid EFX file, unk3 not equal to 0x20!");
                }

                ushort unk5 = reader.ReadUInt16();
                if (unk5 != 0x01)
                {
                    throw new TextureFormatException("Not a valid EFX file, unk4 not equal to 0x01!");
                }

                ushort paletteColors = reader.ReadUInt16();
                if (paletteColors != 1 << bpp)
                {
                    throw new TextureFormatException("This EFX file contains more colors then requested by bpp. Is this a multi-palette texture?");
                }

                ushort width = reader.ReadUInt16();
                ushort height = reader.ReadUInt16();

                if (reader.ReadUInt16() != width)
                {
                    throw new TextureFormatException("Not a valid EFX file, widths not corresponding!");
                }
                if (reader.ReadUInt16() != height)
                {
                    throw new TextureFormatException("Not a valid EFX file, widths not corresponding!");
                }

                reader.ReadUInt16(); //pad

                uint unk6 = reader.ReadUInt32();
                if (unk6 != 0x20)
                {
                    throw new TextureFormatException("Not a valid EFX file, unk5 not equal to 0x20!");
                }

                uint header2AndImgDataSize = reader.ReadUInt32();
                uint imgSize = header2AndImgDataSize - 0x20;

                int paletteSize = paletteColors * 4;
                if (sizeEntryNotHeader - header2AndImgDataSize != paletteSize)
                {
                    throw new TextureFormatException("Not a valid EFX file, size of image data not corresponding!");
                }

                if (reader.ReadUInt32() != sizeEntryNotHeader)
                {
                    throw new TextureFormatException("Not a valid EFX file, size of entry without header 1 not corresponding in header 2!");
                }

                reader.ReadUInt32(); //pad

                //retrieve image and palette data and construct texture

                byte[] imgData = reader.ReadBytes((int)imgSize);
                byte[] palData = reader.ReadBytes(paletteSize);

                segment = new PalettedTextureFormat.Builder()
                    .SetIndexCodec(IndexCodec.FromBitPerPixel(bpp))
                    .SetImageFilter(new SwizzleFilter(width, height, bpp))
                    .SetPaletteCodec(ColorCodec.CODEC_32BIT_RGBA)
                    .SetColorComparer(new ARGBColorComparer())
                    .Build(imgData, palData, width, height);

                segment.FormatSpecificData.Put<byte>(BPP_KEY, bpp);
                segment.FormatSpecificData.Put<byte>(UNK4_KEY, unk4);
                segment.FormatSpecificData.Put<ushort>(UNK5_KEY, unk5);
                segment.FormatSpecificData.Put<uint>(UNK6_KEY, unk6);
            }

            segment.FormatSpecificData.Put<ushort>(UNK1_KEY, unk1);
            segment.FormatSpecificData.Put<ushort>(UNK2_KEY, unk2);
            segment.FormatSpecificData.Put<ushort>(ID_KEY, id);
            segment.FormatSpecificData.Put<byte>(ENTRY_TYPE_KEY, entryType);
            segment.FormatSpecificData.Put<byte>(UNK3_KEY, unk3);
            return segment;
        }
    }
}
