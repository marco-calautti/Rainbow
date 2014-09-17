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

namespace Rainbow.ImgLib.Encoding
{

    /// <summary>
    /// Base class for implementing color decoders. A color decoder is an object that converts a byte array of raw color data into a sequence of Colors.
    /// One example of implementation is ColorDecoder32BitRGBA, that converts quadruples of bytes representing the four components RGBA.
    /// </summary>
    public abstract class ColorDecoder
    {
        public static readonly ColorDecoder DECODER_24BIT_RGB = new ColorDecoder24BitRGB();
        public static readonly ColorDecoder DECODER_32BIT_RGBA = new ColorDecoder32BitRGBA();
        public static readonly ColorDecoder DECODER_32BIT_BGRA = new ColorDecoder32BitBGRA();
        public static readonly ColorDecoder DECODER_16BITLE_ABGR = new ColorDecoder16BitLEABGR();

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
        /// Returns the size in bit of one color encoded in the format implemented by this ColorDecoder.
        /// </summary>
        public abstract int BitDepth { get; }
    }

}