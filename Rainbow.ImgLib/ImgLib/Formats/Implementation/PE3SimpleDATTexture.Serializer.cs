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
    public class PE3SimpleDATSerializer : TextureFormatSerializer
    {
        public string Name
        {
            get { return PE3SimpleDATTexture.NAME; }
        }

        public string PreferredFormatExtension
        {
            get { return ".dat"; }
        }

        public bool IsValidFormat(System.IO.Stream inputFormat)
        {
            long oldPos = inputFormat.Position;
            BinaryReader reader = new BinaryReader(inputFormat);
            uint data = reader.ReadUInt32();
            inputFormat.Position = 0x0E;
            ushort data2 = reader.ReadUInt16();
            inputFormat.Position = 0x16;
            ushort data3 = reader.ReadUInt16();
            inputFormat.Position = oldPos;

            return data == 0x200 && data2 == 0x21 && data3==0x10;
        }

        public bool IsValidMetadataFormat(MetadataReader metadata)
        {
            try
            {
                metadata.EnterSection("PE3SimpleDAT");
                metadata.ExitSection();
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
            byte[] rawHeader = new byte[0x20];

            formatData.Read(rawHeader, 0, rawHeader.Length);

            long oldPos = formatData.Position;
            formatData.Seek(0, SeekOrigin.End);

            byte[] imageData = new byte[formatData.Position - oldPos];
            formatData.Position = oldPos;

            formatData.Read(imageData, 0, imageData.Length);

            return new PE3SimpleDATTexture(rawHeader, imageData);
        }

        public void Save(TextureFormat t, System.IO.Stream outFormatData)
        {
            PE3SimpleDATTexture texture = t as PE3SimpleDATTexture;
            if (texture == null)
                throw new TextureFormatException("Not a valid PE3 Simple DAT Texture!");

            byte[] header = texture.GetRawHeader();
            byte[] imageData = texture.GetImageData();

            outFormatData.Write(header, 0, header.Length);
            outFormatData.Write(imageData, 0, imageData.Length);
        }

        public void Export(TextureFormat t, MetadataWriter metadata, string directory, string basename)
        {
            PE3SimpleDATTexture texture = t as PE3SimpleDATTexture;
            if (texture == null)
                throw new TextureFormatException("Not a valid PE3 Simple DAT Texture!");

            metadata.BeginSection("PE3SimpleDAT");
            metadata.PutAttribute("Basename", basename);
            metadata.Put("RawHeader", texture.GetRawHeader());

            texture.GetImage().Save(Path.Combine(directory, basename + ".png"));

            metadata.EndSection();
        }

        public TextureFormat Import(MetadataReader metadata, string directory)
        {
            metadata.EnterSection("PE3SimpleDAT");
            string basename = metadata.GetAttribute<string>("Basename");
            byte[] rawHeader = metadata.Get<byte[]>("RawHeader");

            Image img=Image.FromFile(Path.Combine(directory,basename+".png"));

            metadata.ExitSection();

            return new PE3SimpleDATTexture(rawHeader, img);
        }
    }
}
