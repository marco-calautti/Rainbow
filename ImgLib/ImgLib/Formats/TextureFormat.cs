using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ImgLib.Formats
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
        /// The preferred extension for files of this texture format.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// A human readable name for this format.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The number of frames of this texture. An example of multi-frame texture format is the GIF format and the TIM2 format. Every TextureFormat
        /// has at least one frame, which is the image itself.
        /// </summary>
        int FramesCount { get; }

        /// <summary>
        /// The number of color palettes associated to the currently selected active frame of this TextureFormat.
        /// A frame of a TextureFormat may have zero or more palettes.
        /// </summary>
        int PalettesCount { get; }

        /// <summary>
        /// Selects the active frame.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A reference to this object.</returns>
        TextureFormat SelectActiveFrame(int index);

        int GetActiveFrame();

        /// <summary>
        /// Selects the active palette. If this TextureFormat has no palette, this method does nothing.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        TextureFormat SelectActivePalette(int index);

        int GetActivePalette();

        /// <summary>
        /// Gets the Image representation of the currently selected active frame and active palette (if any).
        /// </summary>
        /// <returns></returns>
        Image GetImage();

    }
}