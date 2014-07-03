using System.Collections.Generic;
using System.Drawing;
namespace ImgLib.Common
{
    public static class ImgUtils
    {
        public static Color[] GetColorArray(this Image img)
        {
            Bitmap bmp = new Bitmap(img);
            var list = new List<Color>();
            for (int x = 0; x < img.Width; x++)
                for (int y = 0; y < img.Height; y++)
                    list.Add(bmp.GetPixel(x, y));
            return list.ToArray();
        }

        public static byte[] unSwizzle(byte[] Swizzled, int w, int height, int bitDepth)
        {
            byte[] Buf = new byte[Swizzled.Length];
            int val = 16;
            int width = (w * bitDepth) / 8;
            int rowblocks = width / val;

            int totalBlocksx = width / val;
            int totalBlocksy = height / 8;

            for (int blocky = 0; blocky< totalBlocksy; blocky++)
                for (int blockx = 0; blockx < totalBlocksx; blockx++)
                {
                    int block_index = blockx + blocky * rowblocks;
                    int block_address = block_index * val * 8;

                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < val; x++)
                        {
                            int absolutex = x + blockx * val;
                            int absolutey = y + blocky * 8;

                            Buf[absolutex + absolutey * width] =
                                Swizzled[block_address + x + y * val];
                        }
                }

            int start = totalBlocksy * rowblocks * val * 8;
            for (int i = start; i < Swizzled.Length; i++)
                Buf[i] = Swizzled[i];

            return Buf;
        }

        public static byte[] Swizzle(byte[] Swizzled, int w, int height, int bitDepth)
        {
            byte[] Buf = new byte[Swizzled.Length];
            int val = 16;
            int width = (w * bitDepth) / 8;
            int rowblocks = width / val;

            int totalBlocksx = width / val;
            int totalBlocksy = height / 8;

            for (int blocky = 0; blocky < totalBlocksy; blocky++)
                for (int blockx = 0; blockx < totalBlocksx; blockx++)
                {
                    int block_index = blockx + blocky * rowblocks;
                    int block_address = block_index * val * 8;

                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < val; x++)
                        {
                            int absolutex = x + blockx * val;
                            int absolutey = y + blocky * 8;

                            Buf[block_address + x + y * val] =
                                Swizzled[absolutex + absolutey * width];
                        }
                }

            int start = totalBlocksy * rowblocks * val * 8;
            for (int i = start; i < Swizzled.Length; i++)
                Buf[i] = Swizzled[i];

            return Buf;
        }
    }
}