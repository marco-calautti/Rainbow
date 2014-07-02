using ImgLib.Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ImgLib.Formats.Serializers
{
    internal class TIM2SegmentSerializer : TextureFormatSerializer
    {

        public TextureFormat Open(Stream formatData)
        {

            byte[] fullHeader = new byte[0x30];
            formatData.Read(fullHeader, 0, fullHeader.Length);

            uint dataSize, paletteSize, colorEntries;
            TIM2Segment.TIM2SegmentParameters parameters;

            AcquireInfoFromHeader(fullHeader, out parameters, out dataSize, out paletteSize, out colorEntries);

            byte[] imageData = new byte[dataSize];
            formatData.Read(imageData, 0, (int)dataSize);

            byte[] paletteData = new byte[paletteSize];
            formatData.Read(paletteData, 0, (int)paletteSize);

            return new TIM2Segment(imageData,paletteData,colorEntries,parameters);
        }

        public void Save(TextureFormat texture, Stream outFormatData)
        {

        }

        public void Export(TextureFormat texture, Stream metadata, string directory, string basename)
        {
            TIM2Segment segment = texture as TIM2Segment;
            if (segment == null)
                throw new TextureFormatException("Not A valid TIM2Segment!");

            WriteMetadata(segment,metadata, basename);
            int i = 0;
            foreach (Image img in ConstructImages(segment))
            {
                img.Save(Path.Combine(directory, basename + "_" + i++ + ".png"));
            }
        }

        public TextureFormat Import(Stream metadata, string directory)
        {
            TIM2Segment segment = null;
            try
            {
                int palCount;
                string basename;

                TIM2Segment.TIM2SegmentParameters parameters;
                ReadMetadata(metadata, out parameters,out basename, out palCount);
                ICollection<Image> images=ReadImageData(directory, basename, palCount);

                segment = new TIM2Segment(images,parameters);
            }
            catch (XmlException e)
            {
                throw new TextureFormatException("Invalid metadata file!", e);
            }
            catch (FormatException e)
            {
                throw new TextureFormatException("Invalid metadata file!", e);
            }
            return segment;
        }

        private ICollection<Image> ConstructImages(TIM2Segment segment)
        {

            var list = new List<Image>();
            int oldSelected = segment.GetActivePalette();
            for (int i = 0; i < (segment.PalettesCount == 0 ? 1 : segment.PalettesCount); i++)
            {
                segment.SelectActivePalette(i);
                list.Add(segment.GetImage());
            }
            segment.SelectActivePalette(oldSelected);
            return list;
        }

        private ICollection<Image> ReadImageData(string directory, string basename, int palCount)
        {

                ICollection<Image> images=new List<Image>();
                for (int i = 0; i < (palCount==0? 1 : palCount); i++)
                {
                    string file = Path.Combine(directory, basename + "_" + i + ".png");
                    images.Add(Image.FromFile(file));
                }

            return images;
        }

        private void ReadMetadata(Stream metadata,out TIM2Segment.TIM2SegmentParameters parameters, out string basename, out int palCount)
        {
            XmlReader reader = XmlReader.Create(metadata);
            reader.ReadStartElement("TIM2Texture");
            basename = reader.GetAttribute("basename");
            palCount = int.Parse(reader.GetAttribute("cluts"));

            parameters = new TIM2Segment.TIM2SegmentParameters();

            parameters.width = reader.ReadElementContentAsInt("Width", "");
            parameters.height = reader.ReadElementContentAsInt("Height", "");
            parameters.bpp = (byte)reader.ReadElementContentAsInt("Bpp", "");
            parameters.pixelSize = reader.ReadElementContentAsInt("PixelSize", "");
            parameters.mipmapCount = (byte)reader.ReadElementContentAsInt("MipmapCount", "");

            parameters.format = (byte)reader.ReadElementContentAsInt("Format", "");
            parameters.clutFormat = (byte)reader.ReadElementContentAsInt("ClutFormat", "");
            reader.ReadStartElement("GsTEX0");
            reader.ReadElementContentAsBinHex(parameters.GsTEX0, 0, parameters.GsTEX0.Length);
            reader.ReadEndElement();
            reader.ReadStartElement("GsTEX1");
            reader.ReadElementContentAsBinHex(parameters.GsTEX1, 0, parameters.GsTEX1.Length);
            reader.ReadEndElement();

            parameters.GsRegs = (uint)reader.ReadElementContentAsLong("GsRegs", "");
            parameters.GsTexClut = (uint)reader.ReadElementContentAsLong("GsTexClut", "");
            reader.ReadEndElement();
        }

        private void WriteMetadata(TIM2Segment segment, Stream metadata, string basename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.OmitXmlDeclaration = true;

            XmlWriter xml = XmlWriter.Create(metadata, settings);
            xml.WriteStartElement("TIM2Texture");
            xml.WriteAttributeString("basename", basename);
            xml.WriteAttributeString("cluts", segment.PalettesCount.ToString());

            xml.WriteElementString("Width", segment.GetParameters().width.ToString());
            xml.WriteElementString("Height", segment.GetParameters().height.ToString());
            xml.WriteElementString("Bpp", segment.GetParameters().bpp.ToString());
            xml.WriteElementString("PixelSize", segment.GetParameters().pixelSize.ToString());
            xml.WriteElementString("MipmapCount", segment.GetParameters().mipmapCount.ToString());

            xml.WriteComment("Raw data from TIM2 header");
            xml.WriteElementString("Format", segment.GetParameters().format.ToString());
            xml.WriteElementString("ClutFormat", segment.GetParameters().clutFormat.ToString());
            xml.WriteStartElement("GsTEX0"); xml.WriteBinHex(segment.GetParameters().GsTEX0, 0, segment.GetParameters().GsTEX0.Length); xml.WriteEndElement();
            xml.WriteStartElement("GsTEX1"); xml.WriteBinHex(segment.GetParameters().GsTEX1, 0, segment.GetParameters().GsTEX1.Length); xml.WriteEndElement();

            xml.WriteElementString("GsRegs", segment.GetParameters().GsRegs.ToString());
            xml.WriteElementString("GsTexClut", segment.GetParameters().GsTexClut.ToString());
            xml.WriteEndElement();
            xml.Close();
        }

        private void AcquireInfoFromHeader(byte[] fullHeader, out TIM2Segment.TIM2SegmentParameters parameters, out uint dataSize, out uint paletteSize, out uint colorEntries)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(fullHeader));

            uint totalSize = reader.ReadUInt32();
            paletteSize = reader.ReadUInt32();
            dataSize = reader.ReadUInt32();
            ushort headerSize = reader.ReadUInt16();

            if (headerSize != 0x30)
                throw new TextureFormatException("Bad subheader size, unsupported TIM2 format!");

            colorEntries = reader.ReadUInt16();

            parameters = new TIM2Segment.TIM2SegmentParameters();
            parameters.format = reader.ReadByte();

            parameters.mipmapCount = reader.ReadByte();

            if (parameters.mipmapCount > 1)
                throw new TextureFormatException("Mipmapped images not supported yet!");

            parameters.clutFormat = reader.ReadByte();

            byte depth = reader.ReadByte();

            switch (depth)
            {
                case 01:
                    parameters.bpp = 16;
                    break;
                case 02:
                    parameters.bpp = 24;
                    break;
                case 03:
                    parameters.bpp = 32;
                    break;
                case 04:
                    parameters.bpp = 4;
                    break;
                case 05:
                    parameters.bpp = 8;
                    break;
                default:
                    throw new TextureFormatException("Illegal bit depth!");
            }
            parameters.width = reader.ReadUInt16();
            parameters.height = reader.ReadUInt16();

            parameters.GsTEX0 = reader.ReadBytes(8);
            parameters.GsTEX1 = reader.ReadBytes(8);

            parameters.GsRegs = reader.ReadUInt32();
            parameters.GsTexClut = reader.ReadUInt32();

            reader.Close();

            //TODO: here we should understand how to use clutFormat to identify the pixel size.
            if (colorEntries > 0)
                parameters.pixelSize = (int)(paletteSize / (uint)colorEntries);
            else
                parameters.pixelSize = (int)dataSize / (parameters.width * parameters.height);
        }
    }
}
