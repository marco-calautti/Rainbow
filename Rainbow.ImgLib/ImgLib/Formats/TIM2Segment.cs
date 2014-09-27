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
using System.IO;
using System.Xml;
using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Common;
using Rainbow.ImgLib.Filters;

namespace Rainbow.ImgLib.Formats
{
    public class TIM2Segment : TextureFormatBase
    {
        internal class TIM2SegmentParameters
        {
            //segment parameters
            internal int width, height;
            internal bool swizzled = false;
            internal bool linearPalette;
            internal byte bpp;
            internal int colorSize;
            internal byte mipmapCount;

            //raw header data we don't mind to process (I hope so).
            internal byte format;
            internal byte[] GsTEX0=new byte[8], GsTEX1=new byte[8];
            internal uint GsRegs, GsTexClut;
            internal byte[] userdata = new byte[0];
        }

        #region Members

        private TIM2SegmentParameters parameters=new TIM2SegmentParameters();

        private byte[] imageData;
        private Color[][] palettes = new Color[0][];
        private ImageDecoder decoder;
        private TIM2PaletteFilter paletteFilter;
        private SwizzleFilter swizzleFilter;

        #endregion

        internal static readonly string NAME = "TIM2Segment";

        internal TIM2Segment(byte[] imageData,byte[] paletteData, uint colorEntries,TIM2SegmentParameters parameters)
        {
            this.imageData = imageData;
            this.parameters = parameters;
            swizzleFilter = new SwizzleFilter(parameters.width, parameters.height, parameters.bpp);
            paletteFilter = new TIM2PaletteFilter(parameters.bpp);

            if (parameters.swizzled)
            {
                this.imageData = swizzleFilter.Defilter(imageData);
            }

            ConstructPalettes(paletteData, colorEntries);
            CreateImageDecoder(imageData);

            if(!parameters.linearPalette)
            {
                for (int i = 0; i < palettes.Length; i++)
                {
                    palettes[i] = paletteFilter.Defilter(palettes[i]);
                }
            }

        }

        internal TIM2Segment(ICollection<Image> images,TIM2SegmentParameters parameters)
        {
            this.parameters = parameters;
            swizzleFilter = new SwizzleFilter(parameters.width, parameters.height, parameters.bpp);
            paletteFilter = new TIM2PaletteFilter(parameters.bpp);

            if(parameters.bpp>8) //true color image
            {
                if (images.Count > 1) //something wrong, we can have at most one true color segment
                    throw new TextureFormatException("Too many images for this true color segment!");

                IEnumerator<Image> en = images.GetEnumerator(); en.MoveNext();
                imageData = GetColorEncoder(parameters.colorSize).EncodeColors(en.Current.GetColorArray()); //I love extension methods. Hurray!
            }else
            {
                IndexedImageEncoder encoder=new IndexedImageEncoder(new List<Image>(images), 1 << parameters.bpp);
                imageData = encoder.Encode();
                palettes = new List<Color[]>(encoder.Palettes).ToArray();
            }
            CreateImageDecoder(imageData);
        }

        #region Properties

        public override string Name
        {
            get { return NAME; }
        }

        public override int Width
        {
            get { return parameters.width; }
        }

        public override int Height
        {
            get { return parameters.height; }
        }

        public override int PalettesCount { get { return palettes.Length; } }

        public override int FramesCount { get { return 1; } }

        public int Bpp { get { return parameters.bpp;} }

        internal bool Swizzled
        {
            get { return parameters.swizzled; }
            set
            {
                if (parameters.swizzled == value)
                    return;

                parameters.swizzled = value;

                imageData = parameters.swizzled ? swizzleFilter.Defilter(imageData) : swizzleFilter.ApplyFilter(imageData);
                CreateImageDecoder(imageData);
            }
        }

        public bool LinearPalette
        {
            get
            {
                return parameters.linearPalette;
            }
        }

        public int ColorSize { get { return parameters.colorSize;} }

        #endregion

        #region Non public methods

        protected override Image GetImage(int activeFrame,int activePalette)
        {
            IndexedImageDecoder iDecoder = decoder as IndexedImageDecoder;
            if (iDecoder != null)
                iDecoder.Palette = palettes[activePalette];

            return decoder.DecodeImage();

        }

        private void CreateImageDecoder(byte[] imageData)
        {
            if (Bpp <= 8) //here we have an Indexed TIM2
            {
                decoder = new IndexedImageDecoder(imageData,
                                              parameters.width, parameters.height,
                                              IndexRetriever.FromBitPerPixel(Bpp),
                                              palettes[SelectedPalette]);
            }
            else //otherwise, we have a true color TIM2
            {
                decoder = new TrueColorImageDecoder(imageData,
                                              parameters.width, parameters.height,
                                              GetColorDecoder(parameters.colorSize));

            }
        }

        private void ConstructPalettes(byte[] paletteData, uint colorEntries)
        {

            if (parameters.bpp > 8)
                return;

            //int colors = 1 << parameters.bpp;
            int numberOfPalettes = paletteData.Length / ((int)colorEntries * parameters.colorSize); //(int)colorEntries / colors;
            int singlePaletteSize = paletteData.Length / numberOfPalettes;

            palettes = new Color[numberOfPalettes][];

            int start = 0;
            for (int i = 0; i < numberOfPalettes; i++)
            {

                palettes[i] = GetColorDecoder(parameters.colorSize).DecodeColors(paletteData, start, singlePaletteSize);
                start += singlePaletteSize;
            }
        }

        private ColorDecoder GetColorDecoder(int pixelSize)
        {
            switch (pixelSize)
            {
                case 2:
                    return ColorDecoder.DECODER_16BITLE_ABGR;
                case 3:
                    return ColorDecoder.DECODER_24BIT_RGB;
                case 4:
                    return ColorDecoder.DECODER_32BIT_RGBA;
                default:
                    throw new TextureFormatException("Illegal Pixel size!");
            }
        }

        private ColorEncoder GetColorEncoder(int pixelSize)
        {
            switch (pixelSize)
            {
                case 2:
                    return ColorEncoder.ENCODER_16BITLE_ABGR;
                case 3:
                    return ColorEncoder.ENCODER_24BIT_RGB;
                case 4:
                    return ColorEncoder.ENCODER_32BIT_RGBA;
                default:
                    throw new TextureFormatException("Illegal Pixel size!");
            }
        }

        #endregion

        #region Internal methods for serializers

        internal byte[] GetImageData()
        {
            return parameters.swizzled ? swizzleFilter.ApplyFilter(imageData) : imageData;
        }

        internal byte[] GetPaletteData()
        {

            ColorEncoder encoder = GetColorEncoder(parameters.colorSize);

            MemoryStream stream = new MemoryStream();
            foreach(Color[] palette in palettes)
            {
                Color[] pal = !parameters.linearPalette ? paletteFilter.ApplyFilter(palette) : palette;

                byte[] buf = encoder.EncodeColors(pal);
                stream.Write(buf, 0, buf.Length);
            }
            stream.Close();
            return stream.ToArray();
        }

        internal TIM2SegmentParameters GetParameters()
        {
            return parameters;
        }

        #endregion
    }
}