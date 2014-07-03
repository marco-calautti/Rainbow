using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImgLib.Formats.Serializers
{
    public interface TextureFormatSerializer
    {
        /// <summary>
        /// Opens data encoded in the format implemented by this TextureFormatSerializer from the given stream.
        /// </summary>
        /// <param name="formatData">A TextureFormat instance of the format implemented by this Serializer</param>
        TextureFormat Open(Stream formatData);

        /// <summary>
        /// Saves this texture to the given stream into the format represented by this TextureFormat.
        /// </summary>
        /// <param name="outFormatData"></param>
        void Save(TextureFormat texture, Stream outFormatData);


        /// <summary>
        /// Exports this texture's metadata (if any) and image data into the given stream and directory. Image data files' are constructed
        /// starting from a base name.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="directory"></param>
        /// <param name="basename"></param>
        void Export(TextureFormat texture, Stream metadata, string directory, string basename);

        /// <summary>
        /// Initializes this texture from a given metadata stream and a directory containing image data.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="directory"></param>
        TextureFormat Import(Stream metadata, string directory);
    }
}
