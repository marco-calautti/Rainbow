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

using Rainbow.ImgLib.Formats.Serialization;
using Rainbow.ImgLib.Formats.Serialization.Metadata;
using Rainbow.ImgLib.Common;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public class TIM2TextureSerializer : TextureFormatSerializer
    {

        public string Name { get { return TIM2Texture.NAME;  } }

        public string PreferredFormatExtension { get { return ".tm2";  } }

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
                metadata.Rewind();
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
            {
                throw new TextureFormatException("Not a valid TIM2Texture!");
            }

            BinaryWriter writer = new BinaryWriter(outFormatData);
            writer.Write("TIM2".ToCharArray());
            writer.Write((ushort)tim2.Version);
            writer.Write((ushort)tim2.TIM2SegmentsList.Count);

            for (int i = 0; i < 8; i++)
            {
                writer.Write((byte)0);
            }

            TIM2SegmentSerializer serializer=new TIM2SegmentSerializer(tim2.Swizzled);
            foreach (TIM2Segment segment in tim2.TIM2SegmentsList)
            {
                serializer.Save(segment, outFormatData);
            }
        }

        public void Export(TextureFormat texture, MetadataWriter metadata, string directory, string basename)
        {

            TIM2Texture tim2 = texture as TIM2Texture;
            if (tim2 == null)
            {
                throw new TextureFormatException("Not a valid TIM2Texture!");
            }

            metadata.BeginSection("TIM2");
            metadata.PutAttribute("Version", tim2.Version);
            metadata.PutAttribute("Basename", basename);
            metadata.PutAttribute("Swizzled", tim2.Swizzled);

            metadata.PutAttribute("Textures", tim2.TIM2SegmentsList.Count);
            int layer = 0;
            foreach (TIM2Segment segment in tim2.TIM2SegmentsList)
            {
                TextureFormatSerializer serializer = new TIM2SegmentSerializer();
                serializer.Export(segment,metadata, directory, basename + "_layer" + layer++);
            }

            metadata.EndSection();
        }

        public TextureFormat Import(MetadataReader metadata, string directory)
        {
            TIM2Texture tim2=null;
            try
            {
                metadata.EnterSection("TIM2");

                int version = metadata.GetAttribute<int>("Version");
                string basename = metadata.GetAttribute<string>("Basename");
                bool swizzled = metadata.GetAttribute<bool>("Swizzled");
                int textureCount = metadata.GetAttribute<int>("Textures");
                
                List<TIM2Segment> imagesList = new List<TIM2Segment>();

                for (int i = 0; i < textureCount;i++)
                {
                    TIM2Segment segment = (TIM2Segment)new TIM2SegmentSerializer(swizzled).Import(metadata, directory);
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
            {
                throw new TextureFormatException("Invalid TIM2 image!");
            }

            version = reader.ReadUInt16();
            textureCount = reader.ReadUInt16();
            reader.BaseStream.Position += 8;
        }
    }

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

        public bool IsValidFormat(Stream input)
        {
            throw new NotImplementedException();
        }


        public bool IsValidMetadataFormat(MetadataReader metadata)
        {
            throw new NotImplementedException();
        }

        public TextureFormat Open(Stream formatData)
        {

            uint dataSize, paletteSize, colorEntries;
            TIM2Segment.TIM2SegmentParameters parameters;

            AcquireInfoFromHeader(formatData, out parameters, out dataSize, out paletteSize, out colorEntries);

            byte[] imageData = new byte[dataSize];
            formatData.Read(imageData, 0, (int)dataSize);

            byte[] paletteData = new byte[paletteSize];
            formatData.Read(paletteData, 0, (int)paletteSize);

            return new TIM2Segment(imageData, paletteData, colorEntries, parameters);
        }

        public void Save(TextureFormat texture, Stream outFormatData)
        {
            TIM2Segment segment = texture as TIM2Segment;
            if (segment == null)
            {
                throw new TextureFormatException("Not A valid TIM2Segment!");
            }

            byte[] imageData = segment.GetImageData();
            byte[] paletteData = segment.GetPaletteData();
            TIM2Segment.TIM2SegmentParameters parameters = segment.GetParameters();

            //write header
            WriteHeader(parameters, outFormatData, imageData, paletteData);
            outFormatData.Write(imageData, 0, imageData.Length);
            outFormatData.Write(paletteData, 0, paletteData.Length);
        }

        public void Export(TextureFormat texture, MetadataWriter metadata, string directory, string basename)
        {
            TIM2Segment segment = texture as TIM2Segment;
            if (segment == null)
            {
                throw new TextureFormatException("Not A valid TIM2Segment!");
            }

            Writemetadata(segment, metadata, basename);
            int i = 0;
            Image referenceImage = null;
            List<Image> images = ConstructImages(segment, out referenceImage);

            foreach (Image img in images)
            {
                img.Save(Path.Combine(directory, basename + "_" + i++ + ".png"));
            }

            if (referenceImage != null)
            {
                referenceImage.Save(Path.Combine(directory, basename + "_reference.png"));
            }
        }

        public TextureFormat Import(MetadataReader metadata, string directory)
        {
            TIM2Segment segment = null;

            int palCount;
            string basename;

            TIM2Segment.TIM2SegmentParameters parameters;
            Readmetadata(metadata, out parameters, out basename, out palCount);
            Image referenceImage = null;
            List<Image> images = ReadImageData(directory, basename, palCount, out referenceImage);

            if (referenceImage != null)
            {
                segment = new TIM2Segment(referenceImage, images.Select(img => img.GetColorArray()).ToList(), parameters);
            }
            else
            {
                segment = new TIM2Segment(images.First(), null, parameters);
            }

            return segment;
        }

        private List<Image> ConstructImages(TIM2Segment segment, out Image referenceImage)
        {

            referenceImage = segment.GetReferenceImage();

            if(referenceImage == null)
            {
                return new List<Image>() { segment.GetImage() };
            }

            var list = new List<Image>();
            int oldSelected = segment.SelectedPalette;
            for (int i = 0; i < segment.PalettesCount; i++)
            {
                segment.SelectedPalette = i;
                Bitmap img = new Bitmap(segment.Palette.Length, 1);
                for (int j = 0; j < segment.Palette.Length; j++)
                {
                    img.SetPixel(j, 0, segment.Palette[j]);
                }
                list.Add(img);
            }
            segment.SelectedPalette = oldSelected;

            return list;
        }

        private List<Image> ReadImageData(string directory, string basename, int palCount, out Image referenceImage)
        {
            if (palCount > 1)
            {
                referenceImage = Image.FromFile(Path.Combine(directory, basename + "_reference.png"));
            }
            else
            {
                referenceImage = null;
            }

            List<Image> images = new List<Image>();

            for (int i = 0; i < (palCount == 0 ? 1 : palCount); i++)
            {
                string file = Path.Combine(directory, basename + "_" + i + ".png");
                images.Add(Image.FromFile(file));
            }

            return images;
        }

        private void Readmetadata(MetadataReader metadata, out TIM2Segment.TIM2SegmentParameters parameters, out string basename, out int palCount)
        {

            metadata.EnterSection("TIM2Texture");

            basename = metadata.GetAttribute<string>("Basename");
            palCount = metadata.GetAttribute<int>("Cluts");

            parameters = new TIM2Segment.TIM2SegmentParameters();

            parameters.swizzled = swizzled;

            parameters.linearPalette = metadata.GetAttribute<bool>("LinearClut");

            parameters.width = metadata.Get<int>("Width");
            parameters.height = metadata.Get<int>("Height");
            parameters.bpp = metadata.Get<byte>("Bpp");
            parameters.colorSize = metadata.Get<int>("ColorSize");
            parameters.mipmapCount = metadata.Get<byte>("MipmapCount");

            parameters.format = metadata.Get<byte>("Format");

            parameters.GsTEX0 = metadata.Get<byte[]>("GsTEX0");
            parameters.GsTEX1 = metadata.Get<byte[]>("GsTEX1");

            parameters.GsRegs = metadata.Get<uint>("GsRegs");
            parameters.GsTexClut = metadata.Get<uint>("GsTexClut");

            parameters.userdata = metadata.Get<byte[]>("UserData");

            metadata.ExitSection();

        }

        private void Writemetadata(TIM2Segment segment, MetadataWriter metadata, string basename)
        {

            metadata.BeginSection("TIM2Texture");
            metadata.PutAttribute("Basename", basename);
            metadata.PutAttribute("Cluts", segment.PalettesCount);
            metadata.PutAttribute("LinearClut", segment.GetParameters().linearPalette);

            metadata.Put("Width", segment.GetParameters().width);
            metadata.Put("Height", segment.GetParameters().height);
            metadata.Put("Bpp", segment.GetParameters().bpp);
            metadata.Put("ColorSize", segment.GetParameters().colorSize);
            metadata.Put("MipmapCount", segment.GetParameters().mipmapCount);

            metadata.Put("Format", segment.GetParameters().format);

            metadata.Put("GsTEX0", segment.GetParameters().GsTEX0);
            metadata.Put("GsTEX1", segment.GetParameters().GsTEX1);

            metadata.Put("GsRegs", segment.GetParameters().GsRegs);
            metadata.Put("GsTexClut", segment.GetParameters().GsTexClut);
            metadata.Put("UserData", segment.GetParameters().userdata);

            metadata.EndSection();
        }

        private void WriteHeader(TIM2Segment.TIM2SegmentParameters parameters, Stream outFormatData, byte[] imageData, byte[] paletteData)
        {
            BinaryWriter writer = new BinaryWriter(outFormatData);
            uint totalSize = (uint)(0x30 + parameters.userdata.Length + imageData.Length + paletteData.Length);
            writer.Write(totalSize);
            writer.Write((uint)paletteData.Length);
            writer.Write((uint)imageData.Length);
            writer.Write((ushort)(0x30 + parameters.userdata.Length));


            ushort colorEntries = (ushort)(parameters.bpp > 8 ? 0 : 1 << parameters.bpp);
            writer.Write(colorEntries);

            writer.Write(parameters.format);
            writer.Write(parameters.mipmapCount);

            byte clutFormat = (byte)(parameters.bpp > 8 ? 0 : parameters.colorSize - 1);

            clutFormat |= parameters.linearPalette ? (byte)0x80 : (byte)0;

            writer.Write(clutFormat);
            byte depth;
            switch (parameters.bpp)
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
                    throw new ArgumentException("Should never happen");
            }
            writer.Write(depth);
            writer.Write((ushort)parameters.width);
            writer.Write((ushort)parameters.height);
            writer.Write(parameters.GsTEX0);
            writer.Write(parameters.GsTEX1);
            writer.Write(parameters.GsRegs);
            writer.Write(parameters.GsTexClut);
            writer.Write(parameters.userdata);
        }

        private void AcquireInfoFromHeader(Stream formatData, out TIM2Segment.TIM2SegmentParameters parameters, out uint dataSize, out uint paletteSize, out uint colorEntries)
        {
            byte[] fullHeader = new byte[0x30];
            formatData.Read(fullHeader, 0, fullHeader.Length);

            BinaryReader reader = new BinaryReader(new MemoryStream(fullHeader));

#pragma warning disable 219
            uint totalSize = reader.ReadUInt32();
#pragma warning restore 219

            paletteSize = reader.ReadUInt32();
            dataSize = reader.ReadUInt32();
            ushort headerSize = reader.ReadUInt16();

            int userDataSize = headerSize - 0x30;

            colorEntries = reader.ReadUInt16();

            parameters = new TIM2Segment.TIM2SegmentParameters();
            parameters.swizzled = swizzled;

            parameters.format = reader.ReadByte();

            parameters.mipmapCount = reader.ReadByte();

            if (parameters.mipmapCount > 1)
            {
                throw new TextureFormatException("Mipmapped images not supported yet!");
            }

            byte clutFormat = reader.ReadByte();

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

            parameters.linearPalette = (clutFormat & 0x80) != 0;
            clutFormat &= 0x7F;

            parameters.colorSize = parameters.bpp > 8 ? parameters.bpp / 8 : (clutFormat & 0x07) + 1;

            if (userDataSize > 0)
            {
                byte[] data = new byte[userDataSize];
                formatData.Read(data, 0, userDataSize);
                parameters.userdata = data;
            }

        }

    }
}
