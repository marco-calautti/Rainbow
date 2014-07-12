//Copyright (C) 2014 Marco (Phoenix) Calautti.

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