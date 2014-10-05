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

using Rainbow.ImgLib.Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats
{
    public abstract class CommonTextureFormat : TextureFormatBase
    {
        protected IList<byte[]> imagesData;
        protected IList<Color[]> palettes;
        protected int[] widths;
        protected int[] heights;
        protected int[] bpps;

        internal CommonTextureFormat(IList<byte[]> imgData, IList<byte[]> palData, int[] widths, int[] heights, int[] bpps)
        {
            imagesData = imgData;
            int frame = 0;
            palettes = palData.Select(data => PaletteDecoder(frame++).DecodeColors(data)).ToList();
            this.widths = widths;
            this.heights = heights;
            this.bpps = bpps;
        }

        internal CommonTextureFormat(IList<Image> images,int[] bpps)
        {
            imagesData = new List<byte[]>(bpps.Length);
            palettes = new List<Color[]>(bpps.Length);

            this.bpps = bpps;
            widths = images.Select(img => img.Width).ToArray();
            heights = images.Select(img => img.Height).ToArray();

            for (int i = 0; i < images.Count; i++)
            {
                Image img = images[i];
                IndexedImageEncoder encoder = new IndexedImageEncoder(new List<Image> { img }, 1<<bpps[i]);
                imagesData.Add(encoder.Encode());
                palettes.Add(encoder.Palettes[0]);
            }
        }
        public override abstract string Name
        {
            get;
        }

        public override int Width
        {
            get { return widths[SelectedFrame]; }
        }

        public override int Height
        {
            get { return heights[SelectedFrame]; }
        }

        public virtual int Bpp
        {
            get { return bpps[SelectedFrame]; }
        }

        public override int FramesCount
        {
            get { return bpps.Length; }
        }

        public override int PalettesCount
        {
            get { return 1; }
        }

        protected override System.Drawing.Image GetImage(int activeFrame, int activePalette)
        {
            return new IndexedImageDecoder(imagesData[activeFrame],
                                           widths[activeFrame],
                                           heights[activeFrame],
                                           IndexRetriever(activeFrame),
                                           palettes[activeFrame]).DecodeImage();
        }

        public virtual IList<byte[]> GetImagesData()
        {
            return imagesData;
        }

        public virtual IList<byte[]> GetPaletteData()
        {
            int frame = 0;
            return palettes.Select(pal => PaletteEncoder(frame++).EncodeColors(pal)).ToList();
        }

        public virtual int[] GetWidths()
        {
            return widths;
        }

        public virtual int[] GetHeights()
        {
            return heights;
        }

        public virtual int[] GetBpps()
        {
            return bpps;
        }

        protected abstract ColorDecoder PaletteDecoder(int activeFrame);
        protected abstract ColorEncoder PaletteEncoder(int activeFrame);

        protected abstract IndexRetriever IndexRetriever(int activeFrame);
        protected abstract IndexPacker IndexPacker(int activeFrame);
    }
}
