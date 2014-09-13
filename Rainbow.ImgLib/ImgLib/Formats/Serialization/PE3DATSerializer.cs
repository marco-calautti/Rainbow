using Rainbow.ImgLib.Formats.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Serialization
{
    public class PE3DATSerializer : TextureFormatSerializer
    {

        public string Name
        {
            get { return PE3DATTexture.NAME; }
        }

        public string PreferredFormatExtension
        {
            get { return ".dat"; }
        }

        public bool IsValidFormat(System.IO.Stream inputFormat)
        {
            long oldPos=inputFormat.Position;

            BinaryReader reader = new BinaryReader(inputFormat);

            uint count = 0;
            if ((count = reader.ReadUInt32()) == 0 || reader.ReadUInt64() != 0 || reader.ReadUInt32() != 0)
            {
                inputFormat.Position = oldPos;
                return false;
            }

            
            for (int i = 0; i < count;i++)
            {
                reader.BaseStream.Seek(0x0C, SeekOrigin.Current);
                uint format=reader.ReadUInt32();
                if (format != 2 && format != 1)
                {
                    inputFormat.Position = oldPos;
                    return false;
                }
            }

            inputFormat.Position = oldPos;
            return true;
        }

        public bool IsValidMetadataFormat(Metadata.MetadataReader metadataStream)
        {
            throw new NotImplementedException();
        }

        public TextureFormat Open(System.IO.Stream formatData)
        {
            BinaryReader reader = new BinaryReader(formatData);

            uint texturesCount = reader.ReadUInt32();
            reader.BaseStream.Seek(0x0C, SeekOrigin.Current);

            uint[] positions1 = new uint[texturesCount];
            ushort[] widths = new ushort[texturesCount];
            ushort[] heights = new ushort[texturesCount];
            uint[] positions2 = new uint[texturesCount];
            uint[] formats = new uint[texturesCount];


            for (int i = 0; i < texturesCount; i++)
            {
                positions1[i] = reader.ReadUInt32();
                widths[i] = reader.ReadUInt16();
                heights[i] = reader.ReadUInt16();
                positions2[i] = reader.ReadUInt32();
                formats[i] = reader.ReadUInt32();
            }

            return new PE3DATTexture(reader, positions1, positions2, widths, heights, formats);
            
        }

        public void Save(TextureFormat texture, System.IO.Stream outFormatData)
        {
            throw new NotImplementedException();
        }

        public void Export(TextureFormat texture, Metadata.MetadataWriter metadata, string directory, string basename)
        {
            throw new NotImplementedException();
        }

        public TextureFormat Import(Metadata.MetadataReader metadata, string directory, string basename)
        {
            throw new NotImplementedException();
        }
    }
}
