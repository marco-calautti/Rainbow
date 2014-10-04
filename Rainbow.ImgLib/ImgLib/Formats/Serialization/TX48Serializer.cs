using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Serialization
{
    public class TX48Serializer : TextureFormatSerializer
    {
        private const string MAGIC = "TX48";
        public string Name
        {
            get { return TX48Texture.NAME; }
        }

        public string PreferredFormatExtension
        {
            get { return ".tx48"; }
        }

        public bool IsValidFormat(System.IO.Stream inputFormat)
        {
            long oldPos = inputFormat.Position;
            try
            {
                char[] magic = new BinaryReader(inputFormat).ReadChars(MAGIC.Length);
                return new string(magic) == MAGIC;
            }
            finally
            {
                inputFormat.Position = oldPos;
            }
        }

        public bool IsValidMetadataFormat(Metadata.MetadataReader metadata)
        {
            try
            {
                metadata.EnterSection("TX48Texture");
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                metadata.ExitSection();
            }
            return true;
        }

        public TextureFormat Open(System.IO.Stream formatData)
        {
            long oldPos = formatData.Position;
            formatData.Seek(0, SeekOrigin.End);
            long inputEnd = formatData.Position;

            formatData.Position = oldPos;

            BinaryReader reader = new BinaryReader(formatData);
            
            try
            {
                IList<byte[]> imagesData = new List<byte[]>();
                IList<byte[]> palettesData = new List<byte[]>();
                IList<int> widths = new List<int>(), heights = new List<int>();
                IList<int> bpps = new List<int>();

                while (formatData.Position < inputEnd)
                {
                    char[] magic = reader.ReadChars(MAGIC.Length);
                    if (new string(magic) != MAGIC)
                        throw new TextureFormatException("Not a valid TX48 Texture!");

                    int bpp = reader.ReadInt32();
                    if (bpp != 0 && bpp != 1)
                        throw new TextureFormatException("Illegal Bit per pixel value!");

                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    int paletteOffset = reader.ReadInt32();
                    if (paletteOffset != 0x40)
                        throw new TextureFormatException("TX48 Header is wrong!");

                    int paletteSize = reader.ReadInt32();
                    int imageOffset = reader.ReadInt32();
                    int imageSize = reader.ReadInt32();
                    reader.BaseStream.Position += 0x20;

                    byte[] paletteData = reader.ReadBytes(paletteSize);
                    byte[] imageData = reader.ReadBytes(imageSize);

                    imagesData.Add(imageData);
                    palettesData.Add(paletteData);
                    widths.Add(width);
                    heights.Add(height);
                    bpps.Add(bpp==0? 4 : 8);

                    long skip = 0x800 - (reader.BaseStream.Position - oldPos  ) % 0x800;
                    if (skip == 0x800)
                        skip = 0;

                    reader.BaseStream.Position += skip;
                }

                return new TX48Texture(imagesData, palettesData, widths.ToArray(), heights.ToArray(),bpps.ToArray());

            }catch(Exception e)
            {
                if (e is TextureFormatException)
                    throw e;
                throw new TextureFormatException(e.Message, e);
            }
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
