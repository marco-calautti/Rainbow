using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ImgLib.Formats.Serializers
{
    public class TIM2TextureSerializer : TextureFormatSerializer
    {

        public TextureFormat Open(Stream formatData)
        {
            bool swizzled = true;
            int version, textureCount;
            ReadHeader(formatData, out version, out textureCount);

            //construct images
            List<TIM2Segment> imagesList = new List<TIM2Segment>();

            for (int i = 0; i < textureCount; i++)
            {
                TextureFormatSerializer serializer = new TIM2SegmentSerializer();
                TIM2Segment segment=(TIM2Segment)serializer.Open(formatData);
                imagesList.Add(segment);
                swizzled = segment.Swizzled;
            }

            TIM2Texture tim = new TIM2Texture(imagesList);
            tim.Swizzled = swizzled;
            tim.Version = version;
            return tim;
        }

        public void Save(TextureFormat texture, Stream outFormatData)
        {
            throw new NotImplementedException();
        }

        public void Export(TextureFormat texture, Stream metadata, string directory, string basename)
        {

            TIM2Texture tim2 = texture as TIM2Texture;
            if (tim2 == null)
                throw new TextureFormatException("Not a valid TIM2Texture!");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            XmlWriter xml = XmlWriter.Create(metadata, settings);

            xml.WriteStartDocument();
            xml.WriteStartElement("TIM2");
            xml.WriteAttributeString("version", tim2.Version.ToString());
            xml.WriteAttributeString("basename", basename);
            xml.WriteAttributeString("swizzled", tim2.Swizzled.ToString());

            xml.WriteAttributeString("textures", tim2.ImagesList.Count.ToString());
            int layer = 0;
            foreach (TIM2Segment segment in tim2.ImagesList)
            {
                TextureFormatSerializer serializer = new TIM2SegmentSerializer();
                MemoryStream timmeta = null;
                using (timmeta = new MemoryStream())
                    serializer.Export(segment,timmeta, directory, basename + "_layer" + layer);
                xml.WriteRaw("\n");
                xml.WriteRaw(System.Text.Encoding.UTF8.GetString(timmeta.ToArray()));
                xml.WriteRaw("\n");
            }

            xml.WriteEndElement();
            xml.WriteEndDocument();
            xml.Close();
        }

        public TextureFormat Import(Stream metadata, string directory)
        {
            TIM2Texture tim2=null;
            XmlReader reader = XmlReader.Create(metadata);
            try
            {
                reader.ReadStartElement("TIM2");
                int version = int.Parse(reader.GetAttribute("version"));
                string basename = reader.GetAttribute("basename");
                bool swizzled = bool.Parse(reader.GetAttribute("swizzled"));
                int textureCount = int.Parse(reader.GetAttribute("textures"));

                List<TIM2Segment> imagesList = new List<TIM2Segment>();

                for (int i = 0; i < textureCount; i++)
                {
                    TIM2Segment segment = (TIM2Segment) new TIM2SegmentSerializer().Import(metadata, directory);
                    imagesList.Add(segment);
                }
                reader.ReadEndElement();

                tim2 = new TIM2Texture(imagesList);
                tim2.Swizzled = swizzled;
                tim2.Version = version;
            }
            catch (XmlException e)
            {
                throw new TextureFormatException("Not valid metadata!", e);
            }
            catch (FormatException e)
            {
                throw new TextureFormatException("Cannot parse value!", e);
            }
   
            return tim2;
        }

        private void ReadHeader(Stream stream, out int version, out int textureCount)
        {
            BinaryReader reader = new BinaryReader(stream);

            char[] magic = reader.ReadChars(4);
            if (new string(magic) != "TIM2")
                throw new TextureFormatException("Invalid TIM2 image!");

            version = reader.ReadUInt16();
            textureCount = reader.ReadUInt16();
            reader.BaseStream.Position += 8;
        }
    }
}
