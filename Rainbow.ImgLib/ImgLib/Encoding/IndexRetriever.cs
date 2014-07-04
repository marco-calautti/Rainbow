using System;

namespace Rainbow.ImgLib.Encoding
{
    public abstract class IndexRetriever
    {
        public abstract int GetPixelIndex(byte[] pixelData, int width, int height, int x, int y);
        public abstract int BitDepth { get; }

        public static IndexRetriever FromBitPerPixel(int bpp)
        {
            return FromNumberOfColors(1 << bpp);
        }

        public static IndexRetriever FromNumberOfColors(int colors)
        {
            if (colors <= 16)
                return new IndexRetriever4Bpp();
            else if (colors <= 256)
                return new IndexRetriever8Bpp();
            else
                throw new ArgumentException("Unsupported number of colors");
        }
    }
}
