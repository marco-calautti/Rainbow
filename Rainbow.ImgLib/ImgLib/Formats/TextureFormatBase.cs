using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rainbow.ImgLib.Formats
{
    public abstract class TextureFormatBase : TextureFormat
    {
        private int activeFrame, activePalette;

        public abstract int FramesCount { get; }

        public abstract int PalettesCount { get;  }

        public TextureFormat SelectActiveFrame(int index)
        {
            if (index < 0 || index >= FramesCount)
                throw new IndexOutOfRangeException();
            activeFrame = index;
            return this;
        }

        public int GetActiveFrame()
        {
            return activeFrame;
        }

        public TextureFormat SelectActivePalette(int index)
        {
            if (PalettesCount == 0)
                return this;
            if (index < 0 || index >= PalettesCount)
                throw new IndexOutOfRangeException();

            activePalette = index;
            return this;
        }

        public int GetActivePalette()
        {
            return activePalette;
        }

        public Image GetImage()
        {
            return GetImage(activeFrame, activePalette);
        }

        protected abstract Image GetImage(int activeFrame, int activePalette);
    }
}
