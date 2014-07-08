using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Rainbow.ImgLib.Formats.Serializers
{
    public class TIM2TextureSerializer : TextureFormatSerializer
    {

        public string Name { get { return TIM2Texture.NAME;  } }

        public string PreferredFormatExtension { get { return ".tm2";  } }

        public string PreferredMetadataExtension { get { return ".xml"; } }

        public bool IsValidFormat(Stream format)
        {
            long oldPos = format.Position;

            BinaryReader reader = new BinaryReader(format);

            char[] magic = reader.ReadChars(4);
            format.Position = oldPos;

            return new string(magic) == "TIM2";
        }

        public bool IsValidMetadataFormat(Stream metadata)
        {
            long oldPos = metadata.Position;
            XDocument doc = null;
            string name = "" ;
            try { 
                doc = XDocument.Load(metadata);
                name=doc.Root.Name.ToString();
            }catch(Exception)
            {

            }

            metadata.Position = oldPos;
            return name == "TIM2";
        }

        public TextureFormat Open(Stream formatData)
        {
            int version, textureCount;
            ReadHeader(formatData, out version, out textureCount);

            //construct images
            List<TIM2Segment> imagesList = new List<TIM2Segment>();

            for (int i = 0; i < textureCount; i++)
            {
                TextureFormatSerializer serializer = new TIM2SegmentSerializer();
                TIM2Segment segment=(TIM2Segment)serializer.Open(formatData);
                imagesList.Add(segment);
            }

            TIM2Texture tim = new TIM2Texture(imagesList);
            tim.Version = version;
            return tim;
        }

        public void Save(TextureFormat texture, Stream outFormatData)
        {
            TIM2Texture tim2 = texture as TIM2Texture;
            if (tim2 == null)
                throw new TextureFormatException("Not a valid TIM2Texture!");

            BinaryWriter writer = new BinaryWriter(outFormatData);
            writer.Write("TIM2".ToCharArray());
            writer.Write((ushort)tim2.Version);
            writer.Write((ushort)tim2.TIM2SegmentsList.Count);

            for (int i = 0; i < 8; i++) writer.Write((byte)0);

            TIM2SegmentSerializer serializer=new TIM2SegmentSerializer(tim2.Swizzled);
            foreach (TIM2Segment segment in tim2.TIM2SegmentsList)
                serializer.Save(segment, outFormatData);
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

            xml.WriteAttributeString("textures", tim2.TIM2SegmentsList.Count.ToString());
            int layer = 0;
            foreach (TIM2Segment segment in tim2.TIM2SegmentsList)
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

        public TextureFormat Import(Stream metadata, string directory,string bname)
        {
            TIM2Texture tim2=null;
            try
            {
                XDocument doc = XDocument.Load(metadata);
                if (doc.Root.Name != "TIM2")
                    throw new TextureFormatException("Illegal metadata!");

                XElement node = doc.Root;

                int version = int.Parse(node.Attribute("version").Value);
                string basename = node.Attribute("basename").Value;
                bool swizzled = bool.Parse(node.Attribute("swizzled").Value);
                int textureCount = int.Parse(node.Attribute("textures").Value);
                
                List<TIM2Segment> imagesList = new List<TIM2Segment>();

                var children = node.Elements();
                foreach(XNode child in children)
                {
                    string childmetadata = child.ToString();
                    MemoryStream s = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(childmetadata));
                    TIM2Segment segment = (TIM2Segment) new TIM2SegmentSerializer(swizzled).Import(s, directory,basename);
                    imagesList.Add(segment);
                }

                tim2 = new TIM2Texture(imagesList);
                tim2.Version = version;
            }
            catch (FormatException e)
            {
                throw new TextureFormatException("Cannot parse value!\n"+e.Message, e);
            }
            catch (XmlException e)
            {
                throw new TextureFormatException("Not valid metadata!\n"+e.Message, e);
            }
            catch(TextureFormatException e)
            {
                throw new TextureFormatException(e.Message, e);
            }
            catch(Exception e)
            {
                throw new TextureFormatException("Error:\n"+e.Message, e);
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
