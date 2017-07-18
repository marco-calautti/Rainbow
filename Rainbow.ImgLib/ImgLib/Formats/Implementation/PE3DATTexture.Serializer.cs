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
//
// Parts of this code are inspired by...

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Rainbow.ImgLib.Formats.Serialization;
using Rainbow.ImgLib.Formats.Serialization.Metadata;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public class PE3DATSerializer : TextureFormatSerializer
    {

        public string Name
        {
            get { return PE3DATTexture.NAME; }
        }

        public string PreferredFormatExtension
        {
            get { return ".dat"; }
        }

        public bool IsValidFormat(System.IO.Stream inputFormat)
        {
            long oldPos=inputFormat.Position;

            BinaryReader reader = new BinaryReader(inputFormat);

            uint count = 0;
            if ((count = reader.ReadUInt32()) == 0 || reader.ReadUInt64() != 0 || reader.ReadUInt32() != 0)
            {
                inputFormat.Position = oldPos;
                return false;
            }

            
            for (int i = 0; i < count;i++)
            {
                reader.BaseStream.Seek(0x0C, SeekOrigin.Current);
                uint format=reader.ReadUInt32();
                if (format != 2 && format != 1)
                {
                    inputFormat.Position = oldPos;
                    return false;
                }
            }

            inputFormat.Position = oldPos;
            return true;
        }

        public bool IsValidMetadataFormat(MetadataReader metadata)
        {
            try
            {
                metadata.EnterSection("PE3DAT");
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                metadata.Rewind();
            }
            return true;
        }

        public TextureFormat Open(System.IO.Stream formatData)
        {
            BinaryReader reader = new BinaryReader(formatData);

            uint texturesCount = reader.ReadUInt32();
            reader.BaseStream.Seek(0x0C, SeekOrigin.Current);

            uint[] positions1 = new uint[texturesCount];
            ushort[] widths = new ushort[texturesCount];
            ushort[] heights = new ushort[texturesCount];
            uint[] positions2 = new uint[texturesCount];
            uint[] formats = new uint[texturesCount];


            for (int i = 0; i < texturesCount; i++)
            {
                positions1[i] = reader.ReadUInt32();
                widths[i] = reader.ReadUInt16();
                heights[i] = reader.ReadUInt16();
                positions2[i] = reader.ReadUInt32();
                formats[i] = reader.ReadUInt32();
            }

            int[] bpps = new int[formats.Length];
            for (int i = 0; i < bpps.Length;i++ )
            {
                if (formats[i] != 2 && formats[i] != 1)
                    throw new TextureFormatException("Not valid format code: " + formats[i]);
                bpps[i] = formats[i] == 2 ? 8 : 4;
            }
            
            return new PE3DATTexture(reader, positions1, positions2, widths, heights, bpps);
            
        }

        public void Save(TextureFormat texture, System.IO.Stream outFormatData)
        {
            PE3DATTexture dat = texture as PE3DATTexture;
            if (dat == null)
            {
                throw new TextureFormatException("Not a valid PE3 DAT texture!");
            }
            BinaryWriter writer=new BinaryWriter(outFormatData);

            writer.Write((uint)dat.FramesCount);
            for (int i = 0; i < 3; i++)
            {
                writer.Write((uint)0);
            }

            int oldSelected = dat.SelectedFrame;
            for(int i=0;i<dat.FramesCount;i++)
            {
                dat.SelectedFrame = i;
                writer.Write(dat.Position1);
                writer.Write((ushort)dat.Width);
                writer.Write((ushort)dat.Height);
                writer.Write(dat.Position2);
                writer.Write(dat.Bpp == 8 ? (uint)2 : (uint)1);
            }
            dat.SelectedFrame = oldSelected;

            IList<byte[]> imagesData=dat.GetImagesData();
            IList<byte[]> palettesData=dat.GetPalettesData();

            for (int i = 0; i < dat.FramesCount;i++ )
            {
                writer.Write(palettesData[i]);
                writer.Write(imagesData[i]);
            }
        }

        public void Export(TextureFormat texture, MetadataWriter metadata, string directory, string basename)
        {
            PE3DATTexture dat = texture as PE3DATTexture;
            if (dat == null)
            {
                throw new TextureFormatException("Not a valid PE3 DAT texture!");
            }

            metadata.BeginSection("PE3DAT");
            metadata.PutAttribute("Textures", dat.FramesCount);
            metadata.PutAttribute("Basename", basename);

            int oldSelected=dat.SelectedFrame;
            for(int i=0;i<dat.FramesCount;i++)
            {
                dat.SelectedFrame=i;

                metadata.BeginSection("PE3DATSegment");
                metadata.Put("Position1", dat.Position1);
                metadata.Put("Position2", dat.Position2);
                metadata.Put("Bpp", dat.Bpp);
                metadata.EndSection();

                dat.GetImage().Save(Path.Combine(directory, basename + "_" + i + ".png"));
            }
            dat.SelectedFrame = oldSelected;
            metadata.EndSection();
        }

        public TextureFormat Import(MetadataReader metadata, string directory)
        {
            metadata.EnterSection("PE3DAT");

            int count = metadata.GetAttribute<int>("Textures");
            string basename = metadata.GetAttribute<string>("Basename");

            uint[] positions1 = new uint[count];
            ushort[] widths = new ushort[count];
            ushort[] heights = new ushort[count];
            uint[] positions2 = new uint[count];
            int[] bpps = new int[count];

            IList<Image> images = new List<Image>(count);

            for(int i=0;i<count;i++)
            {
                metadata.EnterSection("PE3DATSegment");

                positions1[i] = metadata.Get<uint>("Position1");
                positions2[i] = metadata.Get<uint>("Position2");
                bpps[i] = metadata.Get<int>("Bpp");

                Image img=Image.FromFile(Path.Combine(directory, basename + "_" + i + ".png"));
                widths[i] = (ushort)img.Width;
                heights[i] = (ushort)img.Height;
                images.Add(img);

                metadata.ExitSection();
            }

            metadata.ExitSection();
            return new PE3DATTexture(images, positions1, positions2, widths, heights, bpps);
        }
    }
}
