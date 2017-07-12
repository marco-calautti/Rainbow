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
using Rainbow.ImgLib.Filters;
using Rainbow.ImgLib.Formats.Serialization;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public class TacticsOgreEFXTextureSerializer : SimpleTextureFormatSerializer<TacticsOgreEFXTexture>
    {
		public static readonly string RAW_DATA_KEY="RawData";
        public static readonly string SCALE_KEY = "Scale";

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

			while(reader.BaseStream.Position<fileSize)
            {
                TextureFormat segment = ReadEntry(reader);
                texture.TextureFormats.Add(segment);
            }

            return texture;
        }

        public override void Save(TextureFormat texture, System.IO.Stream outFormatData)
        {
            throw new NotImplementedException();
        }


        protected override TextureFormat GetTextureFrame(TacticsOgreEFXTexture texture, int frame)
        {
            return texture.TextureFormats[frame];
        }

        protected override TacticsOgreEFXTexture OnImportGeneralTextureMetadata(Serialization.Metadata.MetadataReader metadata)
        {
            throw new NotImplementedException();
        }

        protected override void OnImportFrameMetadata(TacticsOgreEFXTexture texture, int frame, Serialization.Metadata.MetadataReader metadata, IList<System.Drawing.Image> images, System.Drawing.Image referenceImage)
        {
            throw new NotImplementedException();
        }

		private TextureFormat ReadEntry(BinaryReader reader)
        {
			//TODO: store all unknown and important data into segments FormatSpecificData
			TextureFormat segment = null;

			uint fullEntrySize = reader.ReadUInt32();
			uint unk1 = reader.ReadUInt32();
			uint sizeEntryNotHeader = reader.ReadUInt32();

			ushort id = reader.ReadUInt16();
			byte entryType = reader.ReadByte();
			byte unk2 = reader.ReadByte();

			if (entryType != 0x52) // non-image entry
			{
				//let's copy the raw data and put it in a dummy texture
                byte[] buf = reader.ReadBytes((int)fullEntrySize-0x10);
                
				segment = new DummyTexture(string.Format("Data entry, type=0x{0:X}", entryType));

				segment.FormatSpecificData.Put<byte[]>(RAW_DATA_KEY, buf);
			}
			else //image data, let's read header 2 data
			{
				if (fullEntrySize - sizeEntryNotHeader != 0x10)
					throw new TextureFormatException("Not a valid EFX file, full size and size without header 1 do not match!");
				
				byte bpp = reader.ReadByte();
				if (bpp != 4 && bpp != 8)
					throw new TextureFormatException("Not a valid EFX file, unsupported bpp=" + bpp);

				byte unk3 = reader.ReadByte();
				if (unk3 != 0x20)
					throw new TextureFormatException("Not a valid EFX file, unk3 not equal to 0x20!");
				
				ushort unk4 = reader.ReadUInt16();
				if (unk4 != 0x01)
					throw new TextureFormatException("Not a valid EFX file, unk4 not equal to 0x01!");
				
				ushort paletteColors = reader.ReadUInt16();
				ushort width = reader.ReadUInt16();
				ushort height = reader.ReadUInt16();

				if (reader.ReadUInt16() != width)
					throw new TextureFormatException("Not a valid EFX file, widths not corresponding!");
				if (reader.ReadUInt16() != height)
					throw new TextureFormatException("Not a valid EFX file, widths not corresponding!");
				
				reader.ReadUInt16(); //pad

				uint unk5 = reader.ReadUInt32();
				if(unk5!=0x20)
					throw new TextureFormatException("Not a valid EFX file, unk5 not equal to 0x20!");

				uint header2AndImgDataSize = reader.ReadUInt32();
				uint imgSize = header2AndImgDataSize - 0x20;

				int paletteSize = paletteColors * 4;
				if (sizeEntryNotHeader - header2AndImgDataSize != paletteSize)
					throw new TextureFormatException("Not a valid EFX file, size of image data not corresponding!");

				if (reader.ReadUInt32() != sizeEntryNotHeader)
					throw new TextureFormatException("Not a valid EFX file, size of entry without header 1 not corresponding in header 2!");

				reader.ReadUInt32(); //pad

				//retrieve image and palette data and construct texture

				byte[] imgData = reader.ReadBytes((int)imgSize);
				byte[] palData = reader.ReadBytes((int)paletteSize);

				segment = new PalettedTextureFormat.Builder()
					.SetIndexCodec(IndexCodec.FromBitPerPixel(bpp))
					.SetImageFilter(new SwizzleFilter(width, height, bpp))
					.SetPaletteCodec(ColorCodec.CODEC_32BIT_RGBA)
					.Build(imgData, palData, width, height);
			}

			return segment;
        }
    }
}
