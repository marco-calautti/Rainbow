using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;


namespace ImgLib.Formats
{
    public class TIM2Texture : TextureFormatBase
    {
        #region Members

        private List<TIM2Segment> imagesList;

        #endregion

        internal TIM2Texture(List<TIM2Segment> list) 
        {
            imagesList = list;
        }

        #region Properties

        public bool Swizzled
        {
            get { return imagesList.First().Swizzled; }
            set
            {
                foreach (TIM2Segment tim2 in imagesList)
                    tim2.Swizzled = value;
            }
        }

        public int Version { get; internal set; }


        /// <inheritdoc />
        public override string Name { get { return "TIM2"; } }

        /// <inheritdoc />
        public override string Extension { get { return ".tm2"; } }

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
                return imagesList.ElementAt(GetActiveFrame()).PalettesCount;
            }
        }

        #endregion

        /// <inheritdoc />
        protected override System.Drawing.Image GetImage(int activeFrame,int activePalette)
        {
            TIM2Segment tim2 = imagesList.ElementAt(activeFrame);
            return tim2.SelectActivePalette(activePalette).GetImage();
        }

        #region Internal methods for serializers

        internal List<TIM2Segment> ImagesList { get { return imagesList; } }

        #endregion

    }
}
