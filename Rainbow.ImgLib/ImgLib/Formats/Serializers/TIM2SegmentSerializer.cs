using Rainbow.ImgLib.Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Rainbow.ImgLib.Formats.Serializers
{
    internal class TIM2SegmentSerializer : TextureFormatSerializer
    {
        private bool swizzled;

        public TIM2SegmentSerializer()
        {
        }

        public TIM2SegmentSerializer(bool swizzled)
        {
            this.swizzled = swizzled;
        }

        public string Name { get { return TIM2Segment.NAME; } }

        public string PreferredFormatExtension { get { return ""; } }

        public string PreferredMetadataExtension{ get {return ""; } }

        public bool IsValidFormat(Stream input)
        {
            throw new NotImplementedException();
        }

        public bool IsValidMetadataFormat(Stream metadata)
        {
            throw new NotImplementedException();
        }

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
            TIM2Segment segment = texture as TIM2Segment;
            if (segment == null)
                throw new TextureFormatException("Not A valid TIM2Segment!");

            byte[] imageData = segment.GetImageData();
            byte[] paletteData = segment.GetPaletteData();
            TIM2Segment.TIM2SegmentParameters parameters = segment.GetParameters();

            //write header
            WriteHeader(parameters, outFormatData,imageData, paletteData);
            outFormatData.Write(imageData, 0, imageData.Length);
            outFormatData.Write(paletteData, 0, paletteData.Length);
        }

        public void Export(TextureFormat texture, Stream metadata, string directory, string basename)
        {
            TIM2Segment segment = texture as TIM2Segment;
            if (segment == null)
                throw new TextureFormatException("Not A valid TIM2Segment!");

            Writemetadata(segment,metadata, basename);
            int i = 0;
            foreach (Image img in ConstructImages(segment))
            {
                img.Save(Path.Combine(directory, basename + "_" + i++ + ".png"));
            }
        }

        public TextureFormat Import(Stream metadata, string directory,string bname)
        {
            TIM2Segment segment = null;
            try
            {
                int palCount;
                string basename;

                TIM2Segment.TIM2SegmentParameters parameters;
                Readmetadata(metadata, out parameters,out basename, out palCount);
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
            int oldSelected = segment.SelectedPalette;
            for (int i = 0; i < (segment.PalettesCount == 0 ? 1 : segment.PalettesCount); i++)
            {
                segment.SelectedPalette=i;
                list.Add(segment.GetImage());
            }
            segment.SelectedPalette=oldSelected;
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

        private void Readmetadata(Stream metadata,out TIM2Segment.TIM2SegmentParameters parameters, out string basename, out int palCount)
        {
            try
            {
                XDocument doc = XDocument.Load(metadata);

                if (doc.Root.Name != "TIM2Texture")
                    throw new XmlException();

                XElement node = doc.Root;

                basename = node.Attribute("basename").Value;
                palCount = int.Parse(node.Attribute("cluts").Value);

                parameters = new TIM2Segment.TIM2SegmentParameters();

                parameters.swizzled = swizzled;
                parameters.width = int.Parse(node.Element("Width").Value);
                parameters.height = int.Parse(node.Element("Height").Value);
                parameters.bpp = (byte)int.Parse(node.Element("Bpp").Value);
                parameters.pixelSize = int.Parse(node.Element("PixelSize").Value);
                parameters.mipmapCount = (byte)int.Parse(node.Element("MipmapCount").Value);

                parameters.format = (byte)int.Parse(node.Element("Format").Value);
                parameters.clutFormat = (byte)int.Parse(node.Element("ClutFormat").Value);

                parameters.GsTEX0 = Convert.FromBase64String(node.Element("GsTEX0").Value);
                parameters.GsTEX1 = Convert.FromBase64String(node.Element("GsTEX1").Value);

                parameters.GsRegs = (uint)int.Parse(node.Element("GsRegs").Value);
                parameters.GsTexClut = (uint)int.Parse(node.Element("GsTexClut").Value);
            }catch(Exception e)
            {
                throw new TextureFormatException("Non valid metadata!",e);
            }
        }

        private void Writemetadata(TIM2Segment segment, Stream metadata, string basename)
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
            xml.WriteStartElement("GsTEX0"); xml.WriteBase64(segment.GetParameters().GsTEX0, 0, segment.GetParameters().GsTEX0.Length); xml.WriteEndElement();
            xml.WriteStartElement("GsTEX1"); xml.WriteBase64(segment.GetParameters().GsTEX1, 0, segment.GetParameters().GsTEX1.Length); xml.WriteEndElement();

            xml.WriteElementString("GsRegs", segment.GetParameters().GsRegs.ToString());
            xml.WriteElementString("GsTexClut", segment.GetParameters().GsTexClut.ToString());
            xml.WriteEndElement();
            xml.Close();
        }

        private void WriteHeader(TIM2Segment.TIM2SegmentParameters parameters, Stream outFormatData, byte[] imageData, byte[] paletteData)
        {
            BinaryWriter writer = new BinaryWriter(outFormatData);
            uint totalSize = (uint)(0x30 + imageData.Length + paletteData.Length);
            writer.Write(totalSize);
            writer.Write((uint)paletteData.Length);
            writer.Write((uint)imageData.Length);
            writer.Write((ushort)0x30);
            
            ushort colorEntries = (ushort)(paletteData.Length / parameters.pixelSize);
            writer.Write(colorEntries);

            writer.Write(parameters.format);
            writer.Write(parameters.mipmapCount);
            writer.Write(parameters.clutFormat);
            byte depth;
            switch(parameters.bpp)
            {
                case 4:
                    depth = 4;
                    break;
                case 8:
                    depth = 5;
                    break;
                case 16:
                    depth = 1;
                    break;
                case 24:
                    depth = 2;
                    break;
                case 32:
                    depth = 3;
                    break;
                default:
                    throw new Exception("Should never happen");
            }
            writer.Write(depth);
            writer.Write((ushort)parameters.width);
            writer.Write((ushort)parameters.height);
            writer.Write(parameters.GsTEX0);
            writer.Write(parameters.GsTEX1);
            writer.Write(parameters.GsRegs);
            writer.Write(parameters.GsTexClut);
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
            parameters.swizzled = swizzled;

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
