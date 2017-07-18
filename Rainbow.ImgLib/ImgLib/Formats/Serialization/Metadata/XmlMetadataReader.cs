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
    public class XmlMetadataReader : MetadataReaderBase
    {
        XDocument doc;
        IEnumerator<XElement> subSections = null;
        XElement currentElement = null;
        Stream inputStream;
        Stack<IEnumerator<XElement>> savedPointers = new Stack<IEnumerator<XElement>>();
        Stack<XElement> savedElements = new Stack<XElement>();

        private XmlMetadataReader(Stream stream)
        {
            inputStream = stream;
            doc = XDocument.Load(new StreamReader(inputStream));
            if (doc.Root.Name != "textureFormatMetadata")
            {
                throw new MetadataException("Illegal metadata!");
            }

            IEnumerable<XElement> en = doc.Root.Elements("section");
            if (en == null)
            {
                throw new MetadataException("At least one section is required!");
            }

            subSections = en.GetEnumerator();
        }

        public override void EnterSection(string name)
        {
            try
            {
                if (subSections == null || !subSections.MoveNext())
                {
                    throw new MetadataException("No more sections available on this level!");
                }

                if (subSections.Current.Attribute("name").Value != name)
                {
                    throw new MetadataException("Expected section named " + name + " but found " + subSections.Current.Attribute("name").Value);
                }

                savedElements.Push(currentElement);
                savedPointers.Push(subSections);
                currentElement = subSections.Current;
                if (currentElement.Elements("section") != null)
                {
                    subSections = currentElement.Elements("section").GetEnumerator();
                }
                else
                {
                    subSections = null;
                }
            }
            catch (Exception e)
            {
                if (e is MetadataException)
                {
                    throw;
                }
                throw new MetadataException("Cannot enter the given section!", e);
            }
        }

        public override void ExitSection()
        {
            if (savedPointers.Count == 0)
            {
                throw new MetadataException("Cannot exit from root section!");
            }

            subSections = savedPointers.Pop();
            currentElement = subSections.Current;
        }

        public override ICollection<string> Keys
        {
            get
            {
                try
                {
                    if (currentElement == null)
                    {
                        throw new MetadataException("No sections entered");
                    }
                    return currentElement.Elements("data")
                                            .Select(el => el.Attribute("name").Value)
                                            .ToList();
                }
                catch (Exception e)
                {
                    if (e is MetadataException)
                    {
                        throw;
                    }
                    throw new MetadataException("Error while retrieving element value!", e);
                }
            }
        }

        public override ICollection<string> AttributesKeys
        {
            get
            {
                try
                {
                    if (currentElement == null)
                    {
                        throw new MetadataException("No sections entered");
                    }

                    return currentElement.Elements("attribute")
                                            .Select(el => el.Attribute("name").Value)
                                            .ToList();
                }
                catch (Exception e)
                {
                    if (e is MetadataException)
                    {
                        throw;
                    }
                    throw new MetadataException("Error while retrieving element value!", e);
                }
            }
        }

        protected override string GetDataStringRepresentation(string key)
        {
            try
            {
                if (currentElement == null)
                {
                    throw new MetadataException("No sections entered");
                }

                IEnumerable<XElement> en = currentElement.Elements("data").Where(el => el.Attribute("name").Value == key);

                if (en.Count() != 1)
                {
                    throw new MetadataException("Data " + key + " not found or many occurrences found in section " + currentElement.Attribute("name").Value);
                }

                return en.First().Value;
            }
            catch (Exception e)
            {
                if (e is MetadataException)
                {
                    throw;
                }
                throw new MetadataException("Error while retrieving element value!", e);
            }
        }

        protected override string GetAttributeStringRepresentation(string key)
        {
            try
            {
                if (currentElement == null)
                {
                    throw new MetadataException("No sections entered");
                }

                IEnumerable<XElement> en = currentElement.Elements("attribute").Where(el => el.Attribute("name").Value == key);

                if (en.Count() != 1)
                {
                    throw new MetadataException("Attribute " + key + " not found or many occurrences found in section " + currentElement.Attribute("name").Value);
                }

                return en.First().Value;
            }
            catch (Exception e)
            {
                if (e is MetadataException)
                {
                    throw;
                }

                throw new MetadataException("Error while retrieving element value!", e);
            }
        }

        public override Type GetValueType(string key)
        {
            if (currentElement == null)
            {
                throw new MetadataException("No sections entered");
            }

            try
            {
                IEnumerable<string> en = currentElement.Elements("data")
                                                            .Where(el => el.Attribute("name").Value == key)
                                                            .Select(el => el.Attribute("type").Value);

                return Type.GetType(en.First());

            }
            catch (Exception e)
            {
                if (e is MetadataException)
                {
                    throw;
                }
                throw new MetadataException("Error while retrieving element value!", e);
            }
        }

        public override Type GetAttributeValueType(string key)
        {
            if (currentElement == null)
            {
                throw new MetadataException("No sections entered");
            }
            try
            {
                IEnumerable<string> en = currentElement.Elements("attribute")
                                                            .Where(el => el.Attribute("name").Value == key)
                                                            .Select(el => el.Attribute("type").Value);

                return Type.GetType(en.First());

            }
            catch (Exception e)
            {
                if (e is MetadataException)
                {
                    throw;
                }
                throw new MetadataException("Error while retrieving element value!", e);
            }
        }

        public override void Dispose()
        {
            inputStream.Dispose();
        }

        public override void Rewind()
        {
            subSections = doc.Root.Elements("section").GetEnumerator();
            currentElement = null;
            savedPointers.Clear();
            savedElements.Clear();
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
