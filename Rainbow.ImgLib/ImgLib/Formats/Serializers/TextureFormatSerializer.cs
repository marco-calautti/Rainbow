using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Serializers
{
    /// <summary>
    /// A class implementing this interface must be able to retrieve a TextureFormat object starting from a stream encoding an texture in the format implemented by this
    /// serializer, or from a meta format used to export ad import textures in the format implemented by this serializer.
    /// A TextureFormatSerializer may not store any internal state and it should be possible to open/save/import/export multiple TextureFormat objects with the same
    /// serializer.
    /// </summary>
    public interface TextureFormatSerializer
    {

        /// <summary>
        /// A human readable name for the TextureFormat implemented by this serializer. This value is the same returned by the property Name
        /// of TextureFormat.
        /// </summary>
        string Name { get;  }

        /// <summary>
        /// The preferred extension for files encoded in the texture format this serializer implements.
        /// </summary>
        string PreferredFormatExtension { get; }

        /// <summary>
        /// The preferred extension for the exported meta format of the texture format this serializer implements.
        /// </summary>
        string PreferredMetadataExtension { get; }

        /// <summary>
        /// Checks whether the given stream represents a valid texture encoded in the format implemented by this TextureFormatSerializer.
        /// This method does not close the given stream and it restores the original stream position, so that calls to other TextureFormatSeriazliers' IsValidXX can be performed.
        /// </summary>
        /// <param name="inputFormat"></param>
        /// <returns></returns>
        bool IsValidFormat(Stream inputFormat);

        /// <summary>
        /// Checks whether the given stream representes valid metadata for a texture encoded in the format implemented by this TextureFormatSerializer.
        /// This method does not close the given stream and it restores the original stream position, so that calls to other TextureFormatSeriazliers' IsValidXX can be performed.
        /// </summary>
        /// <param name="metadataStream"></param>
        /// <returns></returns>
        bool IsValidMetadataFormat(Stream metadataStream);

        /// <summary>
        /// Opens data encoded in the format implemented by this TextureFormatSerializer from the given stream.
        /// The method does not close the stream, but it does not restore the stream position.
        /// </summary>
        /// <param name="formatData">A TextureFormat instance of the format implemented by this Serializer</param>
        TextureFormat Open(Stream formatData);

        /// <summary>
        /// Saves this texture to the given stream into the format represented by this TextureFormatSerializer.
        /// The method does not close the stream, but it does not restore the stream position.
        /// </summary>
        /// <param name="outFormatData"></param>
        void Save(TextureFormat texture, Stream outFormatData);


        /// <summary>
        /// Exports a texture of the format implemented by this TextureFormatSerializer to the given metadata stream.
        /// Additional data may be saved to the given directory and base file name. What is stored in the given directory is specific to this serializer.
        /// The method does not close the stream, but it does not restore the stream position.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="directory"></param>
        /// <param name="basename"></param>
        void Export(TextureFormat texture, Stream metadata, string directory, string basename);

        /// <summary>
        /// Initializes a texture of the format implemented by this TextureFormatSerializer from a given metadata stream and a directory containing additional data
        /// w.r.t. the given basename. What is stored in the given directory is specific to this serializer.
        /// The method does not close the stream, but it does not restore the stream position.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="directory"></param>
        TextureFormat Import(Stream metadata, string directory, string basename);
    }
}
