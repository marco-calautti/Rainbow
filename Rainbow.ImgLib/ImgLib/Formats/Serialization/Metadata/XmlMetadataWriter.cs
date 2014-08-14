//Copyright (C) 2014 Marco (Phoenix) Calautti.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 2.0.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License 2.0 for more details.

//A copy of the GPL 2.0 should have been included with the program.
//If not, see http://www.gnu.org/licenses/

//Official repository and contact information can be found at
//http://github.com/marco-calautti/Rainbow

using Rainbow.ImgLib.Formats.Serialization.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Rainbow.ImgLib.Formats.Serialization.Metadata
{
    public class XmlMetadataWriter : MetadataWriterBase
    {
        private XDocument doc;
        private XElement currentElement;
        private Stream outputStream;
        private XmlMetadataWriter(Stream stream)
        {
            outputStream = stream;
            doc = new XDocument(new XElement("TextureFormatMetadata"));
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
