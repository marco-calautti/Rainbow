using System.Collections.Generic;
using System.Linq;

namespace Rainbow.ImgLib.Formats
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
                foreach (TIM2Segment tim2 in imagesList)
                    tim2.Swizzled = value;
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

        internal List<TIM2Segment> TIM2SegmentsList { get { return imagesList; } }
    }
}
