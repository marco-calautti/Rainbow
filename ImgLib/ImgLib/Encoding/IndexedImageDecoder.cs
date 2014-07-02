using System.Drawing;

namespace ImgLib.Encoding
{
    public class IndexedImageDecoder : ImageDecoder
    {
        public static readonly Color[] DEFAULT_PALETTE;

        protected byte[] pixelData;

        protected int width, height;

        protected IndexRetriever retriever;

        static IndexedImageDecoder()
        {
            DEFAULT_PALETTE = new Color[256];
            for (int i = 0; i < DEFAULT_PALETTE.Length; i++)
                DEFAULT_PALETTE[i] = Color.FromArgb(255, i, i, i);
        }

        public IndexedImageDecoder(byte[] pixelData, int width, int height, IndexRetriever retriever, Color[] palette = null)
        {
            this.pixelData = pixelData;
            this.width = width;
            this.height = height;
            this.retriever = retriever;
            if (palette == null)
                Palette = DEFAULT_PALETTE;
            else
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