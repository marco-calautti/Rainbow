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

using Rainbow.ImgLib.Formats.Serialization;
using Rainbow.ImgLib.Formats.Serialization.Metadata;
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

        //public string PreferredMetadataExtension { get { return ".xml"; } }

        public bool IsValidFormat(Stream format)
        {
            long oldPos = format.Position;

            BinaryReader reader = new BinaryReader(format);

            char[] magic = reader.ReadChars(4);
            format.Position = oldPos;

            return new string(magic) == "TIM2";
        }

        
        public bool IsValidMetadataFormat(MetadataReader metadata)
        {
            try
            {
                metadata.EnterSection("TIM2");
            }
            catch (Exception)
            {
                return false;
            }
            metadata.ExitSection();
            metadata.Rewind();
            return true;
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

        public void Export(TextureFormat texture, MetadataWriter metadata, string directory, string basename)
        {

            TIM2Texture tim2 = texture as TIM2Texture;
            if (tim2 == null)
                throw new TextureFormatException("Not a valid TIM2Texture!");


            metadata.BeginSection("TIM2");
            metadata.PutAttribute("version", tim2.Version);
            metadata.PutAttribute("basename", basename);
            metadata.PutAttribute("swizzled", tim2.Swizzled);

            metadata.PutAttribute("textures", tim2.TIM2SegmentsList.Count);
            int layer = 0;
            foreach (TIM2Segment segment in tim2.TIM2SegmentsList)
            {
                TextureFormatSerializer serializer = new TIM2SegmentSerializer();
                serializer.Export(segment,metadata, directory, basename + "_layer" + layer++);
            }

            metadata.EndSection();
        }

        public TextureFormat Import(MetadataReader metadata, string directory,string bname)
        {
            TIM2Texture tim2=null;
            try
            {
                metadata.EnterSection("TIM2");

                int version = metadata.GetAttributeInt("version");
                string basename = metadata.GetAttributeString("basename");
                bool swizzled = metadata.GetAttributeBool("swizzled");
                int textureCount = metadata.GetAttributeInt("textures");
                
                List<TIM2Segment> imagesList = new List<TIM2Segment>();

                for (int i = 0; i < textureCount;i++)
                {
                    TIM2Segment segment = (TIM2Segment)new TIM2SegmentSerializer(swizzled).Import(metadata, directory, basename);
                    imagesList.Add(segment);
                }

                metadata.ExitSection();
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
