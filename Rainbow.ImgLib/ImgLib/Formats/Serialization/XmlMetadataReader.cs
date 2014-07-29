using Rainbow.ImgLib.Formats.Serialization.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Rainbow.ImgLib.Formats.Serialization
{
    public class XmlMetadataReader : MetadataReaderBase
    {
        XDocument doc;
        XElement currentElement;
        Stream inputStream;
        private XmlMetadataReader(Stream stream)
        {
            inputStream = stream;
            doc = XDocument.Load(new StreamReader(inputStream));
            currentElement = doc.Root;
        }

        public override void EnterSection(string name)
        {
            try
            {
                currentElement = currentElement.Element(name);
            }catch(Exception e)
            {
                throw new MetadataException("Cannot enter the given section!", e);
            }
        }

        public override void ExitSection()
        {
            if (currentElement.Parent == null)
                throw new MetadataException("Cannot exit from root section!");
            currentElement = currentElement.Parent;
        }

        public override string GetString(string key)
        {
            try
            {
                return currentElement.Element(key).Value;
            }catch(Exception e)
            {
                throw new MetadataException("Error while retrieving element value!", e);
            }
        }

        public override string GetAttributeString(string key)
        {
            try
            {
                return currentElement.Attribute(key).Value;
            }catch(Exception e)
            {
                throw new MetadataException("Error while retrieving element value!", e);
            }
        }

        public override void Dispose()
        {
            inputStream.Dispose();
        }

        public static XmlMetadataReader Create(Stream stream)
        {
            return new XmlMetadataReader(stream);
        }

        public static XmlMetadataReader Create(string filename)
        {
            return Create(File.Open(filename, FileMode.Open));
        }
    }
}
