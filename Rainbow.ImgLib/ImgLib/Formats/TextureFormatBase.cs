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

        public abstract string Name { get;  }
        
        public abstract int Width{ get; }

        public abstract int Height { get; }

        public abstract int FramesCount { get; }

        public abstract int PalettesCount { get;  }

        public int SelectedFrame
        {
            get { return activeFrame; }
            set
            {
                if (value < 0 || value >= FramesCount)
                    throw new IndexOutOfRangeException();
                activeFrame = value;
            }
        }

        public int SelectedPalette
        {
            get{ return activePalette;}
            set
            {
                if (PalettesCount == 0)
                    return;
                if (value < 0 || value >= PalettesCount)
                    throw new IndexOutOfRangeException();

                activePalette = value;
            }
        }

        public Image GetImage()
        {
            return GetImage(activeFrame, activePalette);
        }

        protected abstract Image GetImage(int activeFrame, int activePalette);
    }
}
