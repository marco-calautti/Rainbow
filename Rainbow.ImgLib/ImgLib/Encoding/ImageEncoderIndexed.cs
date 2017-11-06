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
using Rainbow.ImgLib.Common;
using Rainbow.ImgLib.Filters;


namespace Rainbow.ImgLib.Encoding
{
    public class ImageEncoderIndexed : ImageEncoder
    {
        private class DefaultColorSorter : IComparer<Color>
        {
            public int Compare(Color x, Color y)
            {
                long result = (long)(uint)x.ToArgb() - (long)(uint)y.ToArgb();
                return result < 0 ? -1 : result > 0 ? 1 : 0;
            }
        }

        private IList<Image> images;
        private Image referenceImage;

        private int colors;
        private int width, height;
        private IndexCodec codec;
        private IComparer<Color> pixelSorter=new DefaultColorSorter();
        private ColorCodec colorEncoder;
        private ImageFilter imageFilter;
        private PaletteFilter paletteFilter;

        private bool fromReference;

        public ImageEncoderIndexed(IList<Color[]> palettes, Image referenceImage, IndexCodec codec, ColorCodec encoder = null, ImageFilter imageFilter = null, PaletteFilter paletteFilter = null)
        {
            fromReference = true;
            Palettes = palettes;
            this.referenceImage = referenceImage;

            width = referenceImage.Width;
            height = referenceImage.Height;

            if (!IsGreyScale(referenceImage))
            {
                throw new ArgumentException("The reference image must be in grey scale!");
            }

            Init(codec, null, encoder, imageFilter, paletteFilter);


        }

        public ImageEncoderIndexed(Image image, IndexCodec codec, IComparer<Color> pixelComparer = null, ColorCodec encoder = null, ImageFilter imageFilter = null, PaletteFilter paletteFilter = null)
        : this(new List<Image>() { image}, codec,pixelComparer, encoder, imageFilter,paletteFilter)
        {

        }


        private ImageEncoderIndexed(IList<Image> images, IndexCodec codec, IComparer<Color> pixelComparer = null, ColorCodec encoder = null, ImageFilter imageFilter = null, PaletteFilter paletteFilter = null)
        {
            fromReference = false;

            if (images.Count == 0)
            {
                throw new ArgumentException("The image list cannot be empty!");
            }

            width = images.First().Width;
            height = images.First().Height;

            foreach (Image img in images)
            {
                if (img.Width != width || img.Height != height)
                {
                    throw new ArgumentException("The images are not of the same size!");
                }
            }

            this.images = images;

            Init(codec,pixelComparer,encoder,imageFilter,paletteFilter);
        }

        private void Init(IndexCodec codec, IComparer<Color> pixelComparer, ColorCodec encoder, ImageFilter imageFilter, PaletteFilter paletteFilter)
        {
            this.codec = codec;
            this.colorEncoder = encoder;

            this.imageFilter = imageFilter;
            this.paletteFilter = paletteFilter;
            colors = 1 << codec.BitDepth;

            if (pixelComparer != null)
            {
                pixelSorter = pixelComparer;
            }
        }

        private bool IsGreyScale(Image referenceImage)
        {
            return referenceImage.GetColorArray().All((c) => c.R == c.G && c.R == c.B);
        }

        public IList<Color[]> Palettes { get; private set; }
        public IList<byte[]> EncodedPalettes { get; private set; }

        public byte[] Encode()
        {
            if(fromReference)
            {
                return EncodeFromReference();
            }else
            {
                return EncodeFromImages();
            }
        }

        private byte[] EncodeFromReference()
        {
            int[] indexes = referenceImage.GetColorArray().Select((c) => (c.R >> (8 - codec.BitDepth))).ToArray();

            if (colorEncoder != null)
            {
                EncodedPalettes = new List<byte[]>(Palettes.Count);
                foreach (Color[] pal in Palettes)
                {
                    EncodedPalettes.Add(colorEncoder.EncodeColors(paletteFilter == null ? pal : paletteFilter.ApplyFilter(pal)));
                }
            }
            return imageFilter == null ? codec.PackIndexes(indexes) : imageFilter.ApplyFilter(codec.PackIndexes(indexes));
        }

        private byte[] EncodeFromImages()
        {
            List<Bitmap> bitmaps = null;
            if (images.Count == 1) // We can quantize a single palette image
            {
                Image img = images.First();
                if (img.ColorsCount() > colors)
                {
                    nQuant.WuQuantizerBase quantizer = new nQuant.WuQuantizer(colors);
                    img = quantizer.QuantizeImage(new Bitmap(img));
                }
                bitmaps = new List<Bitmap>(); bitmaps.Add(new Bitmap(img));

            }
            else //for multi palette images, quantization may break the pixel structure of the images. We must trust the work of the graphics editor.
            {
                bitmaps = new List<Image>(images).ConvertAll(x => new Bitmap(x));
            }

            var indexes = new int[width * height];


            Palettes = new List<Color[]>();

            for (int i = 0; i < bitmaps.Count; i++)
            {
                Palettes.Add(Enumerable.Repeat<Color>(Color.Black, colors).ToArray());
            }

            for (int i = 0; i < bitmaps.Count; i++)
            {
                int count = 0;
                List<Color> palette = new List<Color>();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixel = bitmaps[i].GetPixel(x, y);
                        if (!palette.Contains(pixel))
                        {
                            if (count >= colors)
                            {
                                throw new ArgumentException("Too many colors! The maximum for this image is " + colors + "!");
                            }
                            palette.Add(pixel);
                            count++;
                        }
                    }
                }

                for (int c = 0; c < colors - count; c++)
                {
                    palette.Add(Color.Black);
                }

                palette.Sort(pixelSorter);
                Palettes[i] = palette.ToArray();
            }


            int k = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmaps[0].GetPixel(x, y);
                    int idx = Array.BinarySearch(Palettes[0], pixel, pixelSorter);
                    indexes[k++] = idx;
                }
            }
            if (colorEncoder != null)
            {
                EncodedPalettes = new List<byte[]>(Palettes.Count);
                foreach (Color[] pal in Palettes)
                {
                    EncodedPalettes.Add(colorEncoder.EncodeColors(paletteFilter == null ? pal : paletteFilter.ApplyFilter(pal)));
                }
            }
            
            return imageFilter == null ? codec.PackIndexes(indexes) : imageFilter.ApplyFilter(codec.PackIndexes(indexes));
        }
    }
}
