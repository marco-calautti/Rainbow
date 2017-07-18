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

using Rainbow.ImgLib.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Rainbow.ImgLib.Formats
{
    public class TextureFormatException : Exception
    {
        public TextureFormatException(string msg)
            : base(msg) { }

        public TextureFormatException(string message, Exception e)
            : base(message, e) { }
    }

    /// <summary>
    /// A TextureFormat representes a set of textures (i.e., images) called frames.
    /// Each frame can have zero or more palettes.
    /// A TextureFormat is the result of calling the method Open of a TextureFormatSerializer.
    /// </summary>
    public interface TextureFormat
    {
        /// <summary>
        /// This event is fired whenever the Texture changes. For example, another frame/palette is selected, or
        /// some other properties, specific to this texture, changes.
        /// </summary>
        event EventHandler TextureChanged;
        /// <summary>
        /// A human readable name for the TextureFormat implemented by this serializer.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The width of the currently selected frame.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the currently selected frame.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The number of frames of this texture. An example of multi-frame texture format is the GIF format and the TIM2 format. Every TextureFormat
        /// has at least one frame, which is the image itself.
        /// </summary>
        int FramesCount { get; }

        /// <summary>
        /// The number of color palettes associated to the currently selected frame of this TextureFormat.
        /// A frame of a TextureFormat may have zero or more palettes.
        /// </summary>
        int PalettesCount { get; }

        /// <summary>
        /// The number of color mipmaps used by the currently selected frame.
        /// A frame of a TextureFormat may have one mipmap (being the image itself) or more mipmaps.
        /// <returns></returns>
        int MipmapsCount { get; }

        /// <summary>
        /// Selects the active frame.
        /// </summary>
        /// <exception cref="System.IndexOutOfRangeException">If the given index is out of range.</exception>
        /// <param name="index"></param>
        /// <returns>A reference to this object.</returns>
        int SelectedFrame { get; set; }


        /// <summary>
        /// Selects the active palette. If this TextureFormat has no palette, setting this property has no effect.
        /// </summary>
        /// <exception cref="System.IndexOutOfRangeException">If the texture has some palette and the given index is out of range.</exception>
        /// <param name="index"></param>
        /// <returns></returns>
        int SelectedPalette { get; set; }

        /// <summary>
        /// Returns the active palette. If this TextureFormat has no palette, null is returned.
        /// </summary>
        Color[] Palette { get; }

        /// <summary>
        /// Gets the Image representation of the currently selected active frame and active palette (if any).
        /// </summary>
        /// <returns></returns>
        Image GetImage();


        /// <summary>
        /// If the current frame contains more than one palette, this methods returns the gray scale Image representation of the currently selected active frame. 
        /// Otherwise, null is returned.
        /// </summary>
        /// <returns></returns>
        Image GetReferenceImage();

        /// <summary>
        /// Returns the i-th mipmaps image of the currently selected frame.
        /// The mipmap 0 coincides with the image itself.
        /// </summary>
        /// <returns></returns>
        Image GetMipmap(int i);

        GenericDictionary FormatSpecificData { get; set; }

    }
}