using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Common
{
    public static class ImageUtils
    {
        public static int GetMipmapWidth(int width, int mipmapIndex)
        {
            return width / (1 << mipmapIndex);
        }

        public static int GetMipmapHeight(int height, int mipmapIndex)
        {
            return height / (1 << mipmapIndex);
        }

        public static int Conv5To8(int value)
        {
            return (value & 0x1F) * 8;
        }

        public static int Conv6To8(int value)
        {
            return (value & 0x3F) * 4;
        }

        public static int Conv4To8(int value)
        {
            return (value & 0xF) * 0x11;
        }

        public static int Conv3To8(int value)
        {
            return (value & 0x7) * 32;
        }

        public static int Conv8To5(int value)
        {
            return (value & 0xFF) >> 3;
        }

        public static int Conv8To6(int value)
        {
            return (value & 0xFF) >> 2;
        }

        public static int Conv8To4(int value)
        {
            return (value & 0xFF) >> 4;
        }

        public static int Conv8To3(int value)
        {
            return (value & 0xFF) >> 5;
        }
    }
}
