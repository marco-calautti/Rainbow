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

using System.Drawing;

using Rainbow.ImgLib.Encoding.Implementation;
using Rainbow.ImgLib.Common;
using Rainbow.ImgLib.Filters;

// TODO: Fix interface of ColorCodec, supporting matrices/width x height inputs.
namespace Rainbow.ImgLib.Encoding
{

    /// <summary>
    /// Base class for implementing color decoders. A color decoder is an object that converts a byte array of raw color data into a sequence of Colors.
    /// One example of implementation is ColorDecoder32BitRGBA, that converts quadruples of bytes representing the four components RGBA.
    /// </summary>
    public abstract class ColorCodec
    {
        public static readonly ColorCodec CODEC_24BIT_RGB = new ColorCodec24BitRGB();
        public static readonly ColorCodec CODEC_32BIT_RGBA = new ColorCodec32BitRGBA();
        public static readonly ColorCodec CODEC_32BIT_BGRA = new ColorCodec32BitBGRA();
        public static readonly ColorCodec CODEC_32BIT_ARGB = new ColorCodec32BitARGB();
        public static readonly ColorCodec CODEC_16BITLE_ABGR = new ColorCodec16BitLEABGR();
        public static readonly ColorCodec CODEC_16BITLE_RGB5A3 = new ColorCodecRGB5A3(ByteOrder.LittleEndian);
        public static readonly ColorCodec CODEC_16BITBE_RGB5A3 = new ColorCodecRGB5A3(ByteOrder.BigEndian);
        public static readonly ColorCodec CODEC_16BITLE_RGB565 = new ColorCodecRGB565(ByteOrder.LittleEndian);
        public static readonly ColorCodec CODEC_16BITBE_RGB565 = new ColorCodecRGB565(ByteOrder.BigEndian);
        public static readonly ColorCodec CODEC_16BITLE_IA8 = new ColorCodecIA8(ByteOrder.LittleEndian);
        public static readonly ColorCodec CODEC_16BITBE_IA8 = new ColorCodecIA8(ByteOrder.BigEndian);
        public static readonly ColorCodec CODEC_8BITLE_IA4 = new ColorCodecIA4(ByteOrder.LittleEndian);
        public static readonly ColorCodec CODEC_8BITBE_IA4 = new ColorCodecIA4(ByteOrder.BigEndian);
        public static readonly ColorCodec CODEC_8BIT_I8 = new ColorCodecI8();
        public static readonly ColorCodec CODEC_4BITLE_I4 = new ColorCodecI4(ByteOrder.LittleEndian);
        public static readonly ColorCodec CODEC_4BITBE_I4 = new ColorCodecI4(ByteOrder.BigEndian);


        /// <summary>
        /// Decodes an array of bytes, representing a sequence of color data in some format,
        /// into an array of Colors objects.
        /// </summary>
        /// <param name="colors">The encoded byte array representing the sequence of colors to decode.</param>
        /// <param name="start">the position in the byte array from which to start.</param>
        /// <param name="length">How many bytes have to be considered. If the length is such that the last pixel cannot be completely decoded, then this pixel is discarded.</param>
        /// <returns></returns>
        public abstract Color[] DecodeColors(byte[] colors, int start, int length);

        /// <summary>
        /// See Color[] DecodeColors(byte[] colors, int start, int length) documentation.
        /// </summary>
        public virtual Color[] DecodeColors(byte[] colors)
        {
            return DecodeColors(colors, 0, colors.Length);
        }

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
        /// Returns the size in bit of one color encoded in the format implemented by this ColorDecoder.
        /// </summary>
        public abstract int BitDepth { get; }

        /// <summary>
        /// Returns the number of bytes needed to encode a sequence of pixels organized in a matrix
        /// defined by the "width" and "height".
        /// Subclasses are encouraged to override this method whenever the number of bytes needed
        /// might not be equal to just the number of total pixels weighted w.r.t. the bit depth.
        /// </summary>
        public virtual int GetBytesNeededForEncode(int width, int height, ImageFilter referenceFilter = null)
        {
            int encWidth = width, encHeight = height;
            if(referenceFilter != null)
            {
                encWidth = referenceFilter.GetWidthForEncoding(width);
                encHeight = referenceFilter.GetHeightForEncoding(height);
            }

            int totalPixel = encWidth * encHeight;
            int bytes = totalPixel * BitDepth / 8;
            int remainder = (totalPixel * BitDepth) % 8;
            return remainder == 0 ? bytes : bytes+1;
        }
    }

}