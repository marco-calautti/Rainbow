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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Formats.Implementation;

namespace Rainbow.ImgLib.Formats.Serialization
{
    /// <summary>
    /// This class helps creating the right TextureFormatSerializer for a given source texture. The texture can be given in stream form or file form.
    /// Given a file extension, it is also possible to request a serializer of a texture format whose preferred file extension if the given one.
    /// User made serializers can be made available at runtime by means of the static method RegisterSerializer.
    /// </summary>
    public static class TextureFormatSerializerProvider
    {
        private static List<TextureFormatSerializer> serializers = new List<TextureFormatSerializer>();

        static TextureFormatSerializerProvider()
        {
            RegisterSerializer(new TIM2TextureSerializer());
            RegisterSerializer(new PE3DATSerializer());
            RegisterSerializer(new PE3SimpleDATSerializer());
            RegisterSerializer(new TX48Serializer());
            RegisterSerializer(new NUTTextureSerializer());
            RegisterSerializer(new TPLTextureSerializer());
            RegisterSerializer(new TacticsOgreEFXTextureSerializer());
        }

        /// <summary>
        /// The list of all available TextureFormatSerializers
        /// </summary>
        public static IEnumerable<TextureFormatSerializer> RegisteredSerializers
        {
            get
            {
                return serializers;
            }
        }

        /// <summary>
        /// Adds the given serializer to the list of available TextureFormatSerializers.
        /// </summary>
        /// <param name="serializer"></param>
        public static void RegisterSerializer(TextureFormatSerializer serializer)
        {
            serializers.Add(serializer);
        }

        /// <summary>
        /// Retrieves a list possible serializers for textures of the given file format extension.
        /// </summary>
        /// <param name="formatExtension"></param>
        /// <returns>A list of possible serializers found for this format extension.</returns>
        public static IEnumerable<TextureFormatSerializer> FromFileFormatExtension(string formatExtension)
        {
            return serializers.Where( s => s.PreferredFormatExtension == formatExtension);
        }

        /// <summary>
        /// Retrieves a serializer for textures encoded in the same format of the given stream.
        /// </summary>
        /// <exception cref="IOException">if any I/O exception occurs during reading the given stream.</exception>
        /// <param name="stream"></param>
        /// <returns>the requested serializer if found, null otherwise.</returns>
        public static TextureFormatSerializer FromStream(Stream stream)
        {
            foreach (var serializer in serializers)
            {
                if (serializer.IsValidFormat(stream))
                {
                    return serializer;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves a serializer for textures encoded in the same format represented by the given metadata.
        /// </summary>
        /// <exception cref="IOException">if any I/O exception occurs during reading the given stream.</exception>
        /// <param name="stream"></param>
        /// <returns>the requested serializer if found, null otherwise.</returns>
        public static TextureFormatSerializer FromMetadata(MetadataReader  reader)
        {
            foreach (var serializer in serializers)
            {
                if (serializer.IsValidMetadataFormat(reader))
                {
                    return serializer;
                }
            }

            return null;
        }
        /// <summary>
        /// Retrieves a serializer for textures encoded in the same format of the given file.
        /// </summary>
        /// <exception cref="IOException">if any I/O exception occurs during reading the given file.</exception>
        /// <param name="stream"></param>
        /// <returns>the requested serializer if found, null otherwise.</returns>
        public static TextureFormatSerializer FromFile(string filePath)
        {
            TextureFormatSerializer serializer = null;
            using (Stream s = File.Open(filePath, FileMode.Open))
            {
                serializer = FromStream(s);
            }

            return serializer;
        }
    }
}
