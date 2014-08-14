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
    public class XmlMetadataReader : MetadataReaderBase
    {
        XDocument doc;
        XElement currentElement;
        Stream inputStream;
        private XmlMetadataReader(Stream stream)
        {
            inputStream = stream;
            doc = XDocument.Load(new StreamReader(inputStream));
            if (doc.Root.Name != "TextureFormatMetadata")
                throw new MetadataException("Illegal metadata!");

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
