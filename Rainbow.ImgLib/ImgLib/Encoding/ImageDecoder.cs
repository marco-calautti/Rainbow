using System.Drawing;

namespace Rainbow.ImgLib.Encoding
{

    /// <summary>
    /// This interface represents an object that can convert its internal image data encoded with the encoding represented by this object, into an Image object.
    /// </summary>
    public interface ImageDecoder
    {
        /// <summary>
        /// Constructs an Image object associated to this ImageEncoder
        /// </summary>
        /// <returns></returns>
        Image DecodeImage();
    }
}