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
    public abstract class NamcoTextureSerializer : SimpleTextureFormatSerializer<NamcoTexture>
    {

        public static readonly string VERSION_KEY       = "Version",
                                      CLUTFORMAT_KEY    = "ClutFormat",
                                      FORMAT_KEY        = "Format",
                                      DEPTH_KEY         = "Depth",
                                      DATA_KEY          = "Data",
                                      USERDATA_KEY      = "UserData";

        protected abstract ByteOrder GetByteOrder();

        protected abstract bool IsSupportedVersion(ushort version);

        protected abstract bool IsSupportedFormat(byte format);

        protected abstract int GetMainHeaderSize();


        protected abstract bool IsPaletted(byte clutFormat);

        protected abstract int GetPaletteColorSize(byte clutFormat);

        protected abstract void GetPalettedTools(       ushort version, 
                                                        byte clutFormat, 
                                                        byte depth, 
                                                        int colorsCount, 
                                                        int width, 
                                                        int height, 
                                                        byte[] data,
                                                        byte[] userData,
                                                        out ColorCodec paletteCodec, 
                                                        out IndexCodec indexCodec,
                                                        out ImageFilter imgFilter,
                                                        out PaletteFilter palFilter);

        protected abstract void GetUnpalettedTools( ushort version,
                                                    byte clutFormat,
                                                    byte depth,
                                                    int colorsCount,
                                                    int width,
                                                    int height,
                                                    byte[] data,
                                                    byte[] userData,
                                                    out ColorCodec imageCodec,
                                                    out ImageFilter imgFilter);

        public override TextureFormat Open(System.IO.Stream formatData)
        {
            if(!IsValidFormat(formatData))
            {
                throw new TextureFormatException("Not a valid Texture!");
            }

            ByteOrder order = GetByteOrder();
            BinaryReader reader = new BinaryReader(formatData);
            reader.BaseStream.Seek(4, SeekOrigin.Begin);

            ushort version = reader.ReadUInt16(order);
            
            if(!IsSupportedVersion(version))
            {
                throw new TextureFormatException("Unsupported Texture version!");
            }

            ushort texturesCount = reader.ReadUInt16(order);

            int headerSize = GetMainHeaderSize();
            int padding = headerSize - 8;

            reader.BaseStream.Position += padding;

            NamcoTexture texture = new NamcoTexture();

            texture.SetName(this.Name);

            texture.FormatSpecificData.Put<int>(VERSION_KEY, version);

            for(int i=0;i<texturesCount;i++)
            {
                texture.TextureFormats.Add(ConstructSegment(reader,version));
            }

            return texture;
        }

        public override void Save(TextureFormat texture, System.IO.Stream outFormatData)
        {
            throw new NotImplementedException();
        }

        protected override TextureFormat GetTextureFrame(NamcoTexture texture, int frame)
        {
            return texture.TextureFormats[frame];
        }

        protected override NamcoTexture OnImportGeneralTextureMetadata(MetadataReader metadata)
        {
            throw new NotImplementedException();
        }

        protected override void OnImportFrameMetadata(NamcoTexture texture, int frame, MetadataReader metadata, IList<System.Drawing.Image> images, System.Drawing.Image referenceImage)
        {
            throw new NotImplementedException();
        }

        private TextureFormat ConstructSegment(BinaryReader reader, ushort version)
        {
            ByteOrder order = GetByteOrder();
            uint totalSize = reader.ReadUInt32(order);
            uint paletteSize = reader.ReadUInt32(order);
            uint imageSize = reader.ReadUInt32(order);
            uint headerSize = reader.ReadUInt16(order);
            int colorsCount = reader.ReadUInt16(order);
            byte format = reader.ReadByte();
            byte mipmapsCount = reader.ReadByte();

            if(!IsSupportedFormat(format))
            {
                throw new TextureFormatException("Unsupported image format!");
            }

            byte clutFormat = reader.ReadByte();
            byte depth = reader.ReadByte();

            int width = reader.ReadUInt16(order);
            int height = reader.ReadUInt16(order);

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

            TextureFormat segment = null;

            if(IsPaletted(clutFormat))
            {
                int colorSize = GetPaletteColorSize(clutFormat);
                int numberOfPalettes = palData.Length / ((int)colorsCount * colorSize);
                int singlePaletteSize = palData.Length / numberOfPalettes;

                IList<byte[]> palettes = new List<byte[]>(numberOfPalettes);
                for (int i = 0; i < numberOfPalettes; i++)
                {
                    palettes.Add(palData.Skip(i * singlePaletteSize).Take(singlePaletteSize).ToArray());
                }

                PalettedTextureFormat.Builder builder = new PalettedTextureFormat.Builder();

                ColorCodec paletteCodec;
                IndexCodec indexCodec;
                ImageFilter imgFilter;
                PaletteFilter palFilter;

                GetPalettedTools(version, clutFormat, depth, colorsCount, width, height, data, userdata, 
                                 out paletteCodec, out indexCodec, out imgFilter, out palFilter);

                builder.SetPaletteCodec(paletteCodec)
                       .SetMipmapsCount(mipmapsCount)
                       .SetIndexCodec(indexCodec) 
                       .SetImageFilter(imgFilter) 
                       .SetPaletteFilter(palFilter);

                segment = builder.Build(imgData, palettes, width, height);
            }else
            {
                ColorCodec imageCodec;
                ImageFilter imgFilter;
                GenericTextureFormat.Builder builder = new GenericTextureFormat.Builder();

                GetUnpalettedTools(version, clutFormat, depth, colorsCount, width, height, data, userdata,
                                    out imageCodec, out imgFilter);

                segment = builder.SetColorCodec(imageCodec)
                                 .SetMipmapsCount(mipmapsCount)
                                 .SetImageFilter(imgFilter)
                                 .Build(imgData, width, height);

            }
            
            segment.FormatSpecificData.Put<byte>(CLUTFORMAT_KEY, clutFormat)
                                      .Put<byte>(FORMAT_KEY, format)
                                      .Put<byte>(DEPTH_KEY,depth)
                                      .Put<byte[]>(DATA_KEY, data)
                                      .Put<byte[]>(USERDATA_KEY, userdata);

            return segment;
        }
    }
}
