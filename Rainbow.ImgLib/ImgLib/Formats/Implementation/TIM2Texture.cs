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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rainbow.ImgLib.Formats.Implementation
{
    /// <summary>
    /// This class represents a TIM2 texture. It supports multi frame TIM2 images. Each frame can also have multiple cluts.
    /// Every segment may have its cluts interleaved or not.
    /// It currently does not support mipmap TIM2 images.
    /// </summary>
    public class TIM2Texture : TextureFormatBase
    {
        #region Members

        private List<TIM2Segment> imagesList;

        #endregion

        internal static readonly string NAME = "TIM2";

        internal TIM2Texture(List<TIM2Segment> list) 
        {
            imagesList = list;
        }


        #region Properties

        /// <summary>
        /// Sets if ALL the frames of this TIM2 image are in swizzled form or not. The value returned by GetImage
        /// will change accordingly.
        /// </summary>
        public bool Swizzled
        {
            get { return imagesList.First().Swizzled; }
            set
            {
                bool changed = imagesList.First().Swizzled != value;
                foreach (TIM2Segment tim2 in imagesList)
                    tim2.Swizzled = value;

                if (changed)
                {
                    OnTextureChanged();
                }
            }
        }

        /// <summary>
        /// True if the current frame has a linear palette. If it is false then the palette is interleaved.
        /// </summary>
        public bool LinearPalette
        {
            get
            {
                return imagesList[SelectedFrame].LinearPalette;
            }
        }

        /// <summary>
        /// The bith depth of this TIM2 image.
        /// </summary>
        public int Bpp { get { return imagesList[SelectedFrame].Bpp;  } }

        /// <summary>
        /// The number of bytes used to encode the colors of this TIM2 image.
        /// </summary>
        public int ColorSize { get { return imagesList[SelectedFrame].ColorSize; } }

        /// <summary>
        /// The version number of this TIM2 image.
        /// </summary>
        public int Version { get; internal set; }

        /// <inheritdoc />
        public override string Name
        {
            get { return NAME; }
        }

        /// <inheritdoc />
        public override int Width
        {
            get { return imagesList[SelectedFrame].Width; }
        }

        /// <inheritdoc />
        public override int Height
        {
            get { return imagesList[SelectedFrame].Height; }
        }
        /// <inheritdoc />
        public override int FramesCount
        {
            get
            {
                return imagesList.Count;
            }
        }

        /// <inheritdoc />
        public override int PalettesCount
        {
            get
            {
                return imagesList[SelectedFrame].PalettesCount;
            }
        }

        #endregion

        /// <inheritdoc />
        protected override System.Drawing.Image GetImage(int activeFrame,int activePalette)
        {
            TIM2Segment tim2 = imagesList[activeFrame];
            tim2.SelectedPalette=activePalette;
            return tim2.GetImage();
        }

        protected override Color[] GetPalette(int activePalette)
        {
            TIM2Segment format = imagesList[SelectedFrame];
            int oldPal = format.SelectedPalette;
            format.SelectedPalette = activePalette;
            Color[] palette = format.Palette;
            format.SelectedPalette = oldPal;
            return palette;
        }

        internal List<TIM2Segment> TIM2SegmentsList { get { return imagesList; } }

        public override System.Drawing.Image GetReferenceImage()
        {
            return imagesList[SelectedFrame].GetReferenceImage();
        }
    }
}
