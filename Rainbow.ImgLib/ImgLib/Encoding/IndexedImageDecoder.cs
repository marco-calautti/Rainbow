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

using System.Drawing;

namespace Rainbow.ImgLib.Encoding
{
    public class IndexedImageDecoder : ImageDecoder
    {

        protected byte[] pixelData;

        protected int width, height;

        protected IndexRetriever retriever;

        public IndexedImageDecoder(byte[] pixelData, int width, int height, IndexRetriever retriever, Color[] palette = null)
        {
            this.pixelData = pixelData;
            this.width = width;
            this.height = height;
            this.retriever = retriever;
            if (palette == null)
            {
                palette = new Color[1 << retriever.BitDepth];
                for (int i = 0; i < palette.Length; i++)
                    palette[i] = Color.FromArgb(255, i * (256/palette.Length), i * (256/palette.Length), i * (256/palette.Length));
            }
            
            Palette = palette;
        }

        public Color[] Palette { get; set; }

        public int BitDepth { get { return retriever.BitDepth; } }

        public Image DecodeImage()
        {
            Bitmap bmp = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    bmp.SetPixel(x, y, Palette[retriever.GetPixelIndex(pixelData, width, height, x, y)]);

            return bmp;
        }

    }
}