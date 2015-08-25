using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rainbow.ImgLib.Common;

using Rainbow.ImgLib.Formats.Serialization;
using Rainbow.ImgLib.Formats.Serialization.Metadata;
using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Encoding.Implementation;
using Rainbow.ImgLib.Filters;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public class NUTTextureSerializer : SimpleTextureFormatSerializer<NUTTexture>
    {

        public override string Name
        {
            get { return NUTTexture.NAME; }
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

        public override TextureFormat Open(System.IO.Stream formatData)
        {
            if(!IsValidFormat(formatData))
            {
                throw new TextureFormatException("Not a valid NUT Texture!");
            }

            BinaryReader reader = new BinaryReader(formatData);
            reader.BaseStream.Seek(4, SeekOrigin.Begin);

            ushort version = reader.ReadUInt16(ByteOrder.BigEndian);
            if(version != 0x8002)
            {
                throw new TextureFormatException("Unsupported NUT version!");
            }

            ushort texturesCount = reader.ReadUInt16(ByteOrder.BigEndian);
            reader.BaseStream.Position += 0x18;

            NUTTexture texture = new NUTTexture();
            texture.Version = version;
            texture.FormatSpecificData.Put<int>("Version", version);

            for(int i=0;i<texturesCount;i++)
            {
                texture.TextureFormats.Add(ConstructSegment(reader));
            }

            return texture;
        }

        public override void Save(TextureFormat texture, System.IO.Stream outFormatData)
        {
            throw new NotImplementedException();
        }

        protected override TextureFormat GetTextureFrame(NUTTexture texture, int frame)
        {
            return texture.TextureFormats[frame];
        }

        protected override NUTTexture OnImportGeneralTextureMetadata(MetadataReader metadata)
        {
            NUTTexture texture = new NUTTexture();

            return texture;
        }

        protected override void OnImportFrameMetadata(NUTTexture texture, int frame, MetadataReader metadata, IList<System.Drawing.Image> images, System.Drawing.Image referenceImage)
        {
        }

        private TextureFormat ConstructSegment(BinaryReader reader)
        {
            uint totalSize = reader.ReadUInt32(ByteOrder.BigEndian);
            uint paletteSize = reader.ReadUInt32(ByteOrder.BigEndian);
            uint imageSize = reader.ReadUInt32(ByteOrder.BigEndian);
            uint headerSize = reader.ReadUInt16(ByteOrder.BigEndian);
            uint colorsCount = reader.ReadUInt16(ByteOrder.BigEndian);
            byte format = reader.ReadByte();
            byte mipmapsCount = reader.ReadByte();

            byte clutFormat = reader.ReadByte();
            byte depth = reader.ReadByte();

            int width = reader.ReadUInt16(ByteOrder.BigEndian);
            int height = reader.ReadUInt16(ByteOrder.BigEndian);

            byte[] data=reader.ReadBytes(24);

            uint userDataSize = headerSize - 0x30;

            byte[] userdata = null;
            if (userDataSize > 0)
                userdata = reader.ReadBytes((int)userDataSize);

            int totalPixels = 0;
            for(int i=0;i<mipmapsCount;i++)
            {
                totalPixels += ImageUtils.GetMipmapWidth(width, i) * ImageUtils.GetMipmapHeight(height, i);
            }

            int firstImageSize = mipmapsCount!=0? (int)((width * height * imageSize) / totalPixels) : 0;

            byte[] imgData = reader.ReadBytes(firstImageSize);
            //we don't need to load the mipmaps, since they can be generated as needed.
            //just skip the mipmaps
            reader.BaseStream.Position += (imageSize - firstImageSize);

            byte[] palData = reader.ReadBytes((int)paletteSize);

            ColorCodec decoder = null;
            TextureFormat segment = null;

            if(clutFormat!=0)
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
                        decoder = ColorCodec.CODEC_16BITBE_RGB565;
                        break;
                    case 2:
                        decoder = ColorCodec.CODEC_16BITBE_RGB5A3;
                        break;
                    case 3:
                        decoder = ColorCodec.CODEC_16BITBE_IA8;
                        break;
                    case 0xB: //not sure about this (FIX)
                        decoder = ColorCodec.CODEC_16BITBE_IA8;
                        break;
                    default:
                        throw new TextureFormatException("Unsupported clut format " + clutFormat);
                }

                int numberOfPalettes = palData.Length / ((int)colorsCount * 2);
                int singlePaletteSize = palData.Length / numberOfPalettes;

                IList<byte[]> palettes = new List<byte[]>(numberOfPalettes);
                for (int i = 0; i < numberOfPalettes; i++)
                {
                    palettes.Add(palData.Skip(i * singlePaletteSize).Take(singlePaletteSize).ToArray());
                }

                PalettedTextureFormat.Builder builder = new PalettedTextureFormat.Builder();

                builder.SetPaletteCodec(decoder)
                       .SetMipmapsCount(mipmapsCount)
                       .SetIndexCodec(IndexCodec.FromBitPerPixel(bpp, ByteOrder.BigEndian))
                       .SetImageFilter(new TileFilter(bpp, 8, 32 / bpp, width, height));

                segment = builder.Build(imgData, palettes, width, height);

            }else
            {
                GenericTextureFormat.Builder builder = new GenericTextureFormat.Builder();
                switch (depth)
                {
                    case 3: //RGBA 32bit?
                        decoder = ColorCodec.CODEC_32BIT_ARGB;
                        ImageFilterComposer composer = new ImageFilterComposer 
                        { new GamecubePlanarFilter(), new TileFilter(32, 4, 4, width, height) };

                        builder.SetImageFilter(composer);
                        break;
                    case 4: //DXT1
                        decoder = new ColorCodecDXT1Gamecube(width, height);
                        break;
                    case 0xA: // I8
                        decoder = ColorCodec.CODEC_8BIT_I8;
                        builder.SetImageFilter(new TileFilter(8, 8, 4, width, height));
                        break;
                    case 0xB: // IA8
                        decoder=ColorCodec.CODEC_16BITBE_IA8;
                        builder.SetImageFilter(new TileFilter(16, 4, 4, width, height));
                        break;
                    default:
                        throw new TextureFormatException("Usupported unpalletted image format " + depth);
                }

                
                segment = builder.SetColorCodec(decoder)
                                 .SetMipmapsCount(mipmapsCount)
                                 .Build(imgData, width, height);
            }

            
            segment.FormatSpecificData.Put<int>("Mipmap", mipmapsCount)
                                      .Put<byte>("ClutFormat", clutFormat)
                                      .Put<byte>("Format", format)
                                      .Put<byte>("Depth",depth)
                                      .Put<byte[]>("Data", data)
                                      .Put<byte[]>("UserData", userdata);

            return segment;
        }
    }
}
