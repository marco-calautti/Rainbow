using Rainbow.ImgLib.Formats.Serialization.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Rainbow.ImgLib.Formats.Serialization
{
    public class XmlMetadataWriter : MetadataWriterBase
    {
        private XDocument doc;
        private XElement currentElement;
        private Stream outputStream;
        private XmlMetadataWriter(Stream stream)
        {
            outputStream = stream;
            doc = new XDocument();
            currentElement = doc.Root;
        }
        public override void BeginSection(string name)
        {
            try
            {
                var elem = new XElement(name);
                currentElement.Add(elem);
                currentElement = elem;
            }catch(Exception e)
            {
                throw new MetadataException("Cannot create section!", e);
            }
        }

        public override void EndSection()
        {
            if (currentElement.Parent == null)
                throw new MetadataException("Cannot close root section!");
            currentElement = currentElement.Parent;
        }

        public override void Put(string key, string value)
        {
            try
            {
                currentElement.Add(new XElement(key, value));
            }catch(Exception e)
            {
                throw new MetadataException("Error while inserting value " + key + "!", e);
            }
        }

        public override void PutAttribute(string key, string value)
        {
            try
            {
                currentElement.SetAttributeValue(key, value);
            }catch(Exception e)
            {
                throw new MetadataException("Error while inserting attribute " + key + "!", e);
            }
        }

        public override void Dispose()
        {
            doc.Save(new StreamWriter(outputStream));
            outputStream.Dispose();
        }

        public static XmlMetadataWriter Create(Stream outputStream)
        {
            return new XmlMetadataWriter(outputStream);
        }

        public static XmlMetadataWriter Create(string filename)
        {
            return Create(File.Open(filename, FileMode.Create));
        }

    }
}
