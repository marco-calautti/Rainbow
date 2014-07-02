using System.Drawing;

namespace ImgLib.Encoding
{

    public class ColorEncoder32BitRGBA : ColorEncoder
    {
        public override byte[] EncodeColors(System.Drawing.Color[] colors, int start, int length)
        {

            byte[] palette = new byte[colors.Length * 4];
            for (int i = start; i < colors.Length; i++)
            {
                palette[(i - start) * 4] = colors[i].R;
                palette[(i - start) * 4 + 1] = colors[i].G;
                palette[(i - start) * 4 + 2] = colors[i].B;
                palette[(i - start) * 4 + 3] = colors[i].A;
            }
            return palette;
        }

        public override int BitDepth
        {
            get { return 24; }
        }
    }
    public class ColorEncoder24BitRGB : ColorEncoder
    {
        public override byte[] EncodeColors(System.Drawing.Color[] colors, int start, int length)
        {
            byte[] palette = new byte[colors.Length * 3];
            for (int i = start; i < colors.Length; i++)
            {
                palette[(i - start) * 3] = colors[i].R;
                palette[(i - start) * 3 + 1] = colors[i].G;
                palette[(i - start) * 3 + 2] = colors[i].B;
            }
            return palette;
        }

        public override int BitDepth
        {
            get { return 24; }
        }
    }

    public class ColorEncoder16BitLEABGR : ColorEncoder
    {
        public override byte[] EncodeColors(Color[] colors, int start, int length)
        {
            byte[] palette = new byte[colors.Length * 2];

            for (int i = start; i < colors.Length; i++)
            {
                ushort data = (ushort)(colors[i].A > 0 ? 0x8000 : 0);

                data = (byte)(((colors[i].B >> 3) << 10) | ((colors[i].G >> 3) << 5) | ((colors[i].R >> 3) & 0x1F));
                palette[(i - start) * 2] = (byte)(data & 0xFF);
                palette[(i - start) * 2 + 1] = (byte)(data >> 8);
            }
            return palette;
        }

        public override int BitDepth
        {
            get { return 16; }
        }
    }
}
