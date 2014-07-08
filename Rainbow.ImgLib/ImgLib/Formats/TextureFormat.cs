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
    /// A TextureFormat is used to retrieve image data from a particular format, save this data into this same format and import/export
    /// this data into a suitable format for editing. A TextureFormat can contain more than one images, in general.
    /// </summary>
    public interface TextureFormat
    {
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
        /// Gets the Image representation of the currently selected active frame and active palette (if any).
        /// </summary>
        /// <returns></returns>
        Image GetImage();

    }
}