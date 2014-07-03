using System.IO;

namespace ImgLib.Encoding
{

    /// <summary>
    /// This interface represents an object that can convert its internal image data into the given stream, following the encoding implemented by this object.
    /// </summary>
    public interface ImageEncoder
    {
        /// <summary>
        /// Encodes the image associated to this ImageEncoder.
        /// </summary>
        /// <param name="s"></param>
        byte[] Encode();
    }
}