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
using Rainbow.ImgLib.Filters;
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
        protected IList<byte[]> encodedPalettes;

        protected int[] widths;
        protected int[] heights;
        protected int[] bpps;

        internal CommonTextureFormat(byte[] imgData,byte[] palData,int width, int height,int bpp):
            this(new List<byte[]> { imgData }, new List<byte[]> { palData }, new int[] { width }, new int[] { height }, new int[] {bpp})
        {

        }
        internal CommonTextureFormat(IList<byte[]> imgData, IList<byte[]> palData, int[] widths, int[] heights, int[] bpps)
        {
            imagesData = imgData;
            encodedPalettes = palData;
            palettes=new List<Color[]>(palData.Count);

            for(int frame=0; frame<bpps.Length; frame++)
            {
                PaletteFilter filter=GetPaletteFilter(frame);
                Color[] decoded=PaletteDecoder(frame).DecodeColors(palData[frame]);
                palettes.Add(filter==null? decoded : filter.Defilter(decoded));
            }

            this.widths = widths;
            this.heights = heights;
            this.bpps = bpps;
        }

        internal CommonTextureFormat(Image image,int bpp):
            this(new List<Image> { image }, new int[] {bpp})
        {

        }

        internal CommonTextureFormat(IList<Image> images,int[] bpps)
        {
            imagesData = new List<byte[]>(bpps.Length);
            palettes = new List<Color[]>(bpps.Length);
            encodedPalettes = new List<byte[]>(bpps.Length);

            this.bpps = bpps;
            widths = images.Select(img => img.Width).ToArray();
            heights = images.Select(img => img.Height).ToArray();

            for (int i = 0; i < images.Count; i++)
            {
                Image img = images[i];
                IndexedImageEncoder encoder = new IndexedImageEncoder(new List<Image> { img }, 
                                                                      IndexCodec.FromBitPerPixel(bpps[i]),
                                                                      PixelComparer(i),
                                                                      PaletteEncoder(i),
                                                                      GetImageFilter(i),
                                                                      GetPaletteFilter(i));
                imagesData.Add(encoder.Encode());
                palettes.Add(encoder.Palettes[0]);
                encodedPalettes.Add(encoder.EncodedPalettes[0]);
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
                                           GetIndexCodec(activeFrame),
                                           palettes[activeFrame],
                                           GetImageFilter(activeFrame),
                                           GetPaletteFilter(activeFrame)).DecodeImage();
        }

        internal virtual IList<byte[]> GetImagesData()
        {
            return imagesData;
        }

        internal virtual IList<byte[]> GetPaletteData()
        {
            return encodedPalettes;
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

        protected abstract IndexCodec GetIndexCodec(int activeFrame);

        protected virtual ImageFilter GetImageFilter(int activeFrame){ return null; }
        protected virtual PaletteFilter GetPaletteFilter(int activeFrame) { return null; }

        protected virtual IComparer<Color> PixelComparer(int activeFrame) { return null; }

    }
}
