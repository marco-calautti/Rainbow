//Copyright (C) 2014 Marco (Phoenix) Calautti.

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


namespace Rainbow.ImgLib.Encoding
{
    public class IndexedImageEncoder : ImageEncoder
    {
        private IList<Image> images;
        private int colors;
        private int width, height;

        public IndexedImageEncoder(IList<Image> images, int numberOfColors)
        {
            this.images = images;
            colors = numberOfColors;

            if (images.Count == 0)
                throw new ArgumentException("The image list cannot be empty!");

            width = images.First().Width;
            height = images.First().Height;

            foreach (Image img in images)
                if (img.Width != width || img.Height != height)
                    throw new ArgumentException("The images are not of the same size!");

        }

        public IList<Color[]> Palettes { get; private set; }

        public byte[] Encode()
        {
            List<Bitmap> bitmaps = null;
            if(images.Count==1) // We can quantize a single palette image
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
                bitmaps = new List<Image>(images).ConvertAll(x => new Bitmap(x)); 

            var indexes = new int[width * height];

            var reversePal=new Dictionary<Color,int>();
            Palettes = new List<Color[]>();

            for (int i = 0; i < bitmaps.Count; i++)
                Palettes.Add(new Color[colors]);

            int index=0;
            int k = 0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmaps[0].GetPixel(x, y);
                    if (!reversePal.ContainsKey(pixel))
                    {
                        if (index >= colors)
                            throw new Exception("Too many colors! The maximum for this TIM2 is "+colors+"!");

                        reversePal[pixel] = index;
                        for (int i = 0; i < Palettes.Count; i++)
                            Palettes[i][index] = bitmaps[i].GetPixel(x, y);
                        index++;
                    }
                    indexes[k++] = reversePal[pixel];
                        
                }

            return IndexPacker.FromNumberOfColors(colors).PackIndexes(indexes);

            /*
          //var palettes = new Color[bitmaps.Count,colors];
          var reversePalettes = new IDictionary<Color, HashSet<int>>[bitmaps.Count];
          for (int i = 0; i < reversePalettes.Length; i++)
          {
              reversePalettes[i] = new Dictionary<Color, HashSet<int>>();
          }

          int index = 0;
          int k = 0;
          for (int x = 0; x < width; x++)
              for (int y = 0; y < height; y++)
              {
                  HashSet<int> possibleIndexes;
                  reversePalettes[0].TryGetValue(bitmaps[0].GetPixel(x, y), out possibleIndexes);
                  if (possibleIndexes == null)
                      possibleIndexes = new HashSet<int>();

                  for (int i = 0; i < bitmaps.Count; i++)
                  {
                      Color pixel = bitmaps[i].GetPixel(x, y);

                      HashSet<int> theOtherSet;
                      reversePalettes[i].TryGetValue(pixel, out theOtherSet);
                      if (theOtherSet == null)
                      {
                          index++;
                          break;
                      }
                      possibleIndexes.IntersectWith(reversePalettes[i][pixel]);
                      if (possibleIndexes.Count == 0) //no compatible index found, we need to create a new one
                      {
                          index++;
                          break;
                      }
                  }
                  if (possibleIndexes.Count == 0)
                  {
                      indexes[k++] = index;
                      for (int i = 0; i < bitmaps.Count; i++)
                      {
                          Color pixel = bitmaps[i].GetPixel(x, y);
                          HashSet<int> set; reversePalettes[i].TryGetValue(pixel, out set);
                          if (set == null)
                              set = new HashSet<int>();
                          set.Add(index);
                          reversePalettes[i][pixel] = set;
                      }
                  }
                  else
                      indexes[k++] = possibleIndexes.First();
              }
           * */
        }

    }
}
