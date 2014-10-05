using System;
using System.Collections.Generic;
using System.Drawing;
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
                metadata.Rewind();
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
                    bpps.Add((bpp+1)*4);

                    long skip = (0x800 - (reader.BaseStream.Position - oldPos  ) % 0x800) % 0x800;

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

        public void Save(TextureFormat txt, System.IO.Stream outFormatData)
        {
            TX48Texture texture = txt as TX48Texture;
            if (texture == null)
                throw new TextureFormatException("Not a valid TX48Texture!");

            BinaryWriter writer = new BinaryWriter(outFormatData);

            IList<byte[]> imagesData = texture.GetImagesData();
            IList<byte[]> palettesData = texture.GetPaletteData();
            int[] widths = texture.GetWidths();
            int[] heights = texture.GetHeights();
            int[] bpps = texture.GetBpps();

            for (int i = 0; i < bpps.Length;i++)
            {
                int total = 0;
                byte[] pal = palettesData[i];
                byte[] img = imagesData[i];

                writer.Write(MAGIC.ToCharArray());
                writer.Write(bpps[i] / 4 - 1);

                writer.Write(widths[i]);
                writer.Write(heights[i]);

                writer.Write(0x40);
                writer.Write(pal.Length);

                writer.Write(0x40 + pal.Length);
                writer.Write(img.Length);

                for (int j = 0; j < 8; j++)
                    writer.Write(0);

                total += 0x40;

                writer.Write(pal);
                writer.Write(img);

                total += pal.Length + img.Length;
                int remainingBytes = (0x800 - total % 0x800) % 0x800;
                for (int j = 0; j < remainingBytes; j++)
                    writer.Write((byte)0);
            }
                
            
        }

        public void Export(TextureFormat txt, Metadata.MetadataWriter metadata, string directory, string basename)
        {
            TX48Texture texture = txt as TX48Texture;
            if (texture == null)
                throw new TextureFormatException("Not a valid TX48Texture!");
            try
            {
                metadata.BeginSection("TX48Texture");
                metadata.PutAttribute("Textures", texture.FramesCount);
                metadata.PutAttribute("Basename", basename);

                int oldSelected = texture.SelectedFrame;
                for (int i = 0; i < texture.FramesCount; i++)
                {
                    texture.SelectedFrame = i;

                    metadata.BeginSection("TX48Segment");
                    metadata.Put("Bpp", texture.Bpp);
                    metadata.EndSection();

                    texture.GetImage().Save(Path.Combine(directory, basename + "_" + i + ".png"));
                }

                texture.SelectedFrame = oldSelected;
                metadata.EndSection();

            }catch(Exception e)
            {
                throw new TextureFormatException(e.Message, e);
            }
        }

        public TextureFormat Import(Metadata.MetadataReader metadata, string directory, string bname)
        {
            metadata.EnterSection("TX48Texture");

            int count = metadata.GetAttributeInt("Textures");
            string basename = metadata.GetAttributeString("Basename");

            int[] bpps = new int[count];

            IList<Image> images = new List<Image>(count);

            for (int i = 0; i < count; i++)
            {
                metadata.EnterSection("TX48Segment");

                bpps[i] = metadata.GetInt("Bpp");

                Image img = Image.FromFile(Path.Combine(directory, basename + "_" + i + ".png"));
                images.Add(img);
                metadata.ExitSection();
            }

            metadata.ExitSection();
            return new TX48Texture(images, bpps);
        }
    }
}
