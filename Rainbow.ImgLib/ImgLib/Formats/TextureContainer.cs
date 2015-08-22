using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats
{
    public abstract class TextureContainer : TextureFormatBase
    {
        private IList<TextureFormat> textureFormats = new List<TextureFormat>();

        internal IList<TextureFormat> TextureFormats
        {
            get { return textureFormats; }
        }

        public override int Width
        {
            get { return textureFormats[SelectedFrame].Width; }
        }

        public override int Height
        {
            get { return textureFormats[SelectedFrame].Height; }
        }

        public override int FramesCount
        {
            get { return textureFormats.Count; }
        }

        public override int PalettesCount
        {
            get { return textureFormats[SelectedFrame].PalettesCount; }
        }

        protected override System.Drawing.Image GetImage(int activeFrame, int activePalette)
        {
            TextureFormat format = textureFormats[activeFrame];
            int oldPal = format.SelectedPalette;
            format.SelectedPalette = activePalette;
            Image img = format.GetImage();
            format.SelectedPalette = oldPal;

            return img;
        }

        public override Image GetReferenceImage()
        {
            return textureFormats[SelectedFrame].GetReferenceImage();
        }
    }
}
