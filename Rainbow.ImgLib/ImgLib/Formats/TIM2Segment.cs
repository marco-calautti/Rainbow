﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Common;

namespace Rainbow.ImgLib.Formats
{
    public class TIM2Segment : TextureFormatBase
    {
        internal class TIM2SegmentParameters
        {
            internal int width, height;
            internal bool swizzled = false;
            internal byte format;
            internal byte bpp;
            internal int pixelSize;
            internal byte mipmapCount;
            internal byte clutFormat; //should correspond to pixelSize
            internal byte[] GsTEX0=new byte[8], GsTEX1=new byte[8];
            internal uint GsRegs, GsTexClut;
            internal byte[] userdata = new byte[0];
        }

        #region Members

        private TIM2SegmentParameters parameters=new TIM2SegmentParameters();

        private byte[] imageData;
        private Color[][] palettes = new Color[0][];
        private ImageDecoder decoder;
        
        #endregion

        internal static readonly string NAME = "TIM2Segment";

        internal TIM2Segment(byte[] imageData,byte[] paletteData,uint colorEntries,TIM2SegmentParameters parameters)
        {
            this.imageData = imageData;
            this.parameters = parameters;
            if (parameters.swizzled)
                this.imageData = ImgUtils.unSwizzle(imageData, parameters.width, parameters.height, parameters.bpp);

            ConstructPalettes(paletteData, colorEntries);
            CreateImageDecoder(imageData);
        }

        internal TIM2Segment(ICollection<Image> images,TIM2SegmentParameters parameters)
        {
            this.parameters = parameters;

            if(parameters.bpp>8) //true color image
            {
                if (images.Count > 1) //something wrong, we can have at most one true color segment
                    throw new TextureFormatException("Too many images for this true color segment!");

                IEnumerator<Image> en = images.GetEnumerator();
                en.MoveNext();
                imageData=GetColorEncoder(parameters.pixelSize).
                                         EncodeColors(
                                            en.Current.GetColorArray() //I love extension methods. Hurray!
                                         );
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
                imageData = parameters.swizzled? ImgUtils.unSwizzle(imageData, parameters.width, parameters.height, parameters.bpp) : ImgUtils.Swizzle(imageData,parameters.width,parameters.height,parameters.bpp);
                CreateImageDecoder(imageData);
            }
        }

        public int PixelSize { get { return parameters.pixelSize;} }

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
                                              GetColorDecoder(parameters.pixelSize));

            }
        }

        private void ConstructPalettes(byte[] paletteData, uint colorEntries)
        {

            if (parameters.bpp > 8)
                return;

            int colors = 1 << parameters.bpp;
            int numberOfPalettes = (int)colorEntries / colors;
            int singlePaletteSize = paletteData.Length / numberOfPalettes;

            palettes = new Color[numberOfPalettes][];

            int start = 0;
            for (int i = 0; i < numberOfPalettes; i++)
            {

                palettes[i] = GetColorDecoder(parameters.pixelSize).DecodeColors(paletteData, start, singlePaletteSize);
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
            return parameters.swizzled ?
                ImgUtils.Swizzle(imageData, parameters.width, parameters.height, parameters.bpp) : imageData;

        }

        internal byte[] GetPaletteData()
        {
            ColorEncoder encoder = GetColorEncoder(parameters.pixelSize);
            int maximumColors = 1 << parameters.bpp;

            MemoryStream stream = new MemoryStream();
            foreach(Color[] palette in palettes)
            {
                List<Color> newPalette=new List<Color>(palette);
                for(int i=0;i<maximumColors-palette.Length;i++)
                {
                    newPalette.Add(Color.Black);
                }
                byte[] buf = encoder.EncodeColors(newPalette.ToArray());
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