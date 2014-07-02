
namespace ImgLib.Encoding
{

    public class IndexRetriever8Bpp : IndexRetriever
    {
        public override int GetPixelIndex(byte[] pixelData, int width, int height, int x, int y)
        {
            return pixelData[x + y * width];
        }

        public override int BitDepth
        {
            get { return 8; }
        }
    }

    public class IndexRetriever4Bpp : IndexRetriever
    {
        public override int GetPixelIndex(byte[] pixelData, int width, int height, int x, int y)
        {
            int pos = x + y * width;
            byte b = pixelData[pos / 2];

            return pos % 2 == 0 ? b & 0xF : (b >> 4) & 0xF;
        }

        public override int BitDepth
        {
            get { return 4; }
        }
    }
}