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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats
{
    /// <summary>
    /// This class acts as a container for multiple TextureFormats.
    /// This is usually used as a collector for each frame of a specific file format.
    /// A frame TextureFormat can be added to the TextureContainer
    /// by adding the frame to the TextureFormats property.
    /// Users of this class should create a subclass for it, by overriding the Name/get property.
    /// </summary>
    public abstract class TextureContainer : TextureFormatBase
    {
        private IList<TextureFormat> textureFormats = new List<TextureFormat>();
        
        public IList<TextureFormat> TextureFormats
        {
            get { return textureFormats; }
            set { textureFormats = value; }
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

        public override int MipmapsCount
        {
            get { return textureFormats[SelectedFrame].MipmapsCount; }
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

        protected override Color[] GetPalette(int activePalette)
        {
            TextureFormat format = textureFormats[SelectedFrame];
            int oldPal = format.SelectedPalette;
            format.SelectedPalette = activePalette;
            Color[] palette = format.Palette;
            format.SelectedPalette = oldPal;
            return palette;
        }

        public override Image GetReferenceImage()
        {
            return textureFormats[SelectedFrame].GetReferenceImage();
        }

        protected string GetCurrentFrameSpecificData(string key)
        {
            return TextureFormats[SelectedFrame].FormatSpecificData.Get(key);
        }

        protected string GetTextureSpecificData(string key)
        {
            return FormatSpecificData.Get(key);
        }
    }
}
