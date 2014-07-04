using System.Drawing;

namespace Rainbow.ImgLib.Encoding
{

    /// <summary>
    /// Base class for implementing color encoders. A color encoder is an object that converts a sequence of Color objects
    /// into a raw encoding.
    /// One example of implementation is ColorEncoder32BitRGBA, that converts a Color into four bytes representing the four components RGBA.
    /// </summary>
    public abstract class ColorEncoder
    {
        public static readonly ColorEncoder ENCODER_24BIT_RGB = new ColorEncoder24BitRGB();
        public static readonly ColorEncoder ENCODER_32BIT_RGBA = new ColorEncoder32BitRGBA();
        public static readonly ColorEncoder ENCODER_16BITLE_ABGR = new ColorEncoder16BitLEABGR();

        /// <summary>
        /// Encodes an array of colors into an array of bytes, following the encoding of this object.
        /// </summary>
        /// <param name="colors">The array of colors to be encoded.</param>
        /// <param name="start">The position of the first color to be encoded.</param>
        /// <param name="length">How many colors need to be encoded.</param>
        public abstract byte[] EncodeColors(Color[] colors, int start, int length);

        /// <summary>
        /// See byte[] EncodeColors(Color[] colors, int start, int length) documentation.
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        public virtual byte[] EncodeColors(Color[] colors)
        {
            return EncodeColors(colors, 0, colors.Length);
        }

        /// <summary>
        /// Returns the size in bit of one encoded color in this format.
        /// </summary>
        public abstract int BitDepth { get; }
    }

}