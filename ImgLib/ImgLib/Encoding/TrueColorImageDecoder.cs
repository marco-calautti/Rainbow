using System.Drawing;

namespace ImgLib.Encoding
{
    public class TrueColorImageDecoder : ImageDecoder
    {
        protected byte[] pixelData;
        protected int width, height;

        protected ColorDecoder decoder;

        public TrueColorImageDecoder(byte[] pixelData, int width, int height, ColorDecoder decoder)
        {
            this.pixelData = pixelData;
            this.width = width;
            this.height = height;
            this.decoder = decoder;
        }
        public Image DecodeImage()
        {
            Color[] colors = decoder.DecodeColors(pixelData);
            Bitmap bmp = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    bmp.SetPixel(x, y, colors[y * width + x]);

            return bmp;
        }

    }
}