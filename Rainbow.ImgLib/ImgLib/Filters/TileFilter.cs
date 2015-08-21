using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Filters
{
    public class TileFilter : ImageFilter
    {
        private int bpp;
        private int tileWidth;
        private int tileHeight;
        private int width, height;

        public TileFilter(int bpp, int tileWidth, int tileHeight, int width, int height)
        {
            this.bpp = bpp;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.width = width;
            this.height = height;
        }

        public override byte[] ApplyFilter(byte[] originalData, int index, int length)
        {
            byte[] newData = new byte[length];

            int lineSize = (tileWidth * bpp) / 8;
            int tileSize = lineSize * tileHeight;
            int pitch = (width * bpp) / 8;

            int tile = 0;

            for (int y = 0; y < height; y += tileHeight)
            {
                for (int x = 0; x < pitch; x += lineSize)
                {
                    for (int line = 0; line < tileHeight; line++)
                    {
                        Array.Copy(originalData, pitch * (y + line) + x, newData, index + tile * tileSize + line * lineSize, lineSize);
                    }

                    tile++;
                }
            }

            return newData;
        }

        public override byte[] Defilter(byte[] originalData, int index, int length)
        {
            byte[] newData = new byte[length];

            int lineSize = (tileWidth * bpp) / 8;
            int tileSize = lineSize * tileHeight;
            int pitch = (width * bpp) / 8;

            int tile = 0;

            for (int y = 0; y < height; y += tileHeight)
            {
                for (int x = 0; x < pitch; x += lineSize)
                {
                    for (int line = 0; line < tileHeight; line++)
                    {
                        Array.Copy(originalData, index + tile * tileSize + line * lineSize, newData, pitch * (y + line) + x, lineSize);
                    }

                    tile++;
                }
            }

            return newData;
        }
    }
}
