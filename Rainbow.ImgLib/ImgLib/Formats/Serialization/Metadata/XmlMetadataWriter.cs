//Copyright (C) 2014+ Marco (Phoenix) Calautti.

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
        XElement currentElement;
        Stream outputStream;
        Stack<XElement> savedElements = new Stack<XElement>();

        private XmlMetadataWriter(Stream stream)
        {
            outputStream = stream;
            doc = new XDocument(new XElement("textureFormatMetadata"));

            currentElement = doc.Root;
        }

        public override void BeginSection(string name)
        {
            try
            {
                XElement newElement = new XElement("section");
                newElement.SetAttributeValue("name", name);

                currentElement.Add(newElement);

                savedElements.Push(currentElement);
                currentElement = newElement;
            }catch(Exception e)
            {
                throw new MetadataException("Cannot create section!", e);
            }
        }

        public override void EndSection()
        {
            if (savedElements.Count == 0)
            {
                throw new MetadataException("Cannot close root section!");
            }
            currentElement = savedElements.Pop();
        }

        protected override void PutWithType(string key, string value, Type type)
        {
            try
            {
                if (currentElement == doc.Root)
                {
                    throw new MetadataException("Must first enter at least one section to insert a value!");
                }

                XElement element=new XElement("data");
                element.SetAttributeValue("name", key);
                element.SetAttributeValue("type", type.FullName);
                element.Value = value;
                currentElement.Add(element);

            }catch(Exception e)
            {
                if (e is MetadataException)
                {
                    throw;
                }
                throw new MetadataException("Error while inserting value " + key + "!", e);
            }
        }

        protected override void PutAttributeWithType(string key, string value, Type type)
        {
            try
            {
                if (currentElement == doc.Root)
                {
                    throw new MetadataException("Must first enter one section at least to insert an attribute!");
                }

                XElement element = new XElement("attribute");
                element.SetAttributeValue("name", key);
                element.SetAttributeValue("type", type.FullName);
                element.Value = value;
                currentElement.Add(element);

            }catch(Exception e)
            {
                if (e is MetadataException)
                {
                    throw;
                }
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
