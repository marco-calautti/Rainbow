using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rainbow.ImgLib.Common;

using Rainbow.ImgLib.Formats.Serialization;
using Rainbow.ImgLib.Formats.Serialization.Metadata;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public class NUTTextureSerializer : TextureFormatSerializer
    {

        public string Name
        {
            get { return NUTTexture.NAME; }
        }

        public string PreferredFormatExtension
        {
            get { return ".nut"; }
        }

        public bool IsValidFormat(System.IO.Stream inputFormat)
        {
            long oldPos = inputFormat.Position;
            try
            {
                char[] magic = new BinaryReader(inputFormat).ReadChars(4);
                return new string(magic) == "NUTC";
            }
            finally
            {
                inputFormat.Position = oldPos;
            }
        }

        public bool IsValidMetadataFormat(MetadataReader metadataStream)
        {
            throw new NotImplementedException();
        }

        public TextureFormat Open(System.IO.Stream formatData)
        {
            BinaryReader reader = new BinaryReader(formatData);
            char[] magic=new char[4];
            reader.Read(magic, 0, 4);

            if (new string(magic) != "NUTC")
            {
                throw new TextureFormatException("Not a valid NUT texture!");
            }

            ushort version = reader.ReadUInt16BE();
            if(version != 0x8002)
            {
                throw new TextureFormatException("Unsupported NUT version!");
            }

            ushort texturesCount = reader.ReadUInt16BE();
            reader.BaseStream.Position += 0x18;

            NUTTexture texture = new NUTTexture();
            NUTSegmentSerializer serializer=new NUTSegmentSerializer();

            for(int i=0;i<texturesCount;i++)
            {
                texture.TextureFormats.Add(serializer.Open(formatData));
            }

            return texture;
        }

        public void Save(TextureFormat texture, System.IO.Stream outFormatData)
        {
            throw new NotImplementedException();
        }

        public void Export(TextureFormat texture, MetadataWriter metadata, string directory, string basename)
        {
            throw new NotImplementedException();
        }

        public TextureFormat Import(MetadataReader metadata, string directory, string basename)
        {
            throw new NotImplementedException();
        }
    }

    internal class NUTSegmentSerializer : TextureFormatSerializer
    {
        public string Name
        {
            get { return "NUT Segment"; }
        }

        public string PreferredFormatExtension
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsValidFormat(System.IO.Stream inputFormat)
        {
            throw new NotImplementedException();
        }

        public bool IsValidMetadataFormat(MetadataReader metadataStream)
        {
            throw new NotImplementedException();
        }

        public TextureFormat Open(System.IO.Stream formatData)
        {
            BinaryReader reader = new BinaryReader(formatData);

            NUTTexture.NUTSegmentParameters parameters = new NUTTexture.NUTSegmentParameters();

            uint totalSize = reader.ReadUInt32BE();
            uint paletteSize = reader.ReadUInt32BE();
            uint imageSize = reader.ReadUInt32BE();
            uint headerSize = reader.ReadUInt16BE();
            uint colorsCount = reader.ReadUInt16BE();
            byte format = reader.ReadByte();
            byte mipmapsCount = reader.ReadByte();

            if (mipmapsCount != 1)
            {
                throw new TextureFormatException("Mipmaps not supported!");
            }

            byte clutFormat = reader.ReadByte();
            byte depth = reader.ReadByte();

            int width = reader.ReadUInt16BE();
            int height = reader.ReadUInt16BE();

            reader.Read(parameters.data, 0, parameters.data.Length);

            parameters.width = width;
            parameters.height = height;
            parameters.format = format;
            parameters.mipmapCount = mipmapsCount;

            switch (depth)
            {
                case 6:
                    parameters.bpp = 8;
                    break;
                default:
                    throw new TextureFormatException("Unsupported depth " + depth);
            }

            switch (clutFormat)
            {
                case 3:
                    parameters.paletteFormat = NUTTexture.NUTSegmentParameters.PaletteFormat.RGB565;
                    break;
                default:
                    throw new TextureFormatException("Unsupported clut format!");
            }

            uint userDataSize = headerSize - 0x30;

            if (userDataSize > 0)
                parameters.userdata = reader.ReadBytes((int)userDataSize);

            byte[] imgData = reader.ReadBytes((int)imageSize);
            byte[] palData = reader.ReadBytes((int)paletteSize);

            int numberOfPalettes = palData.Length / ((int)colorsCount * 2);
            int singlePaletteSize = palData.Length / numberOfPalettes;

            IList<byte[]> palettes = new List<byte[]>(numberOfPalettes);
            for (int i = 0; i < numberOfPalettes; i++)
            {
                palettes.Add(palData.Skip(i * singlePaletteSize).Take(singlePaletteSize).ToArray());
            }

            NUTTexture.PalettedSegment segment = new NUTTexture.PalettedSegment(imgData, palettes, parameters);
            return segment;
        }

        public void Save(TextureFormat texture, System.IO.Stream outFormatData)
        {
            throw new NotImplementedException();
        }

        public void Export(TextureFormat texture, MetadataWriter metadata, string directory, string basename)
        {
            throw new NotImplementedException();
        }

        public TextureFormat Import(MetadataReader metadata, string directory, string basename)
        {
            throw new NotImplementedException();
        }
    }
}
