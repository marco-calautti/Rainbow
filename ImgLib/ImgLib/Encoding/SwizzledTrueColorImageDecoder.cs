using ImgLib.Common;

namespace ImgLib.Encoding
{
    public class SwizzledTrueColorImageDecoder : TrueColorImageDecoder
    {
        public SwizzledTrueColorImageDecoder(byte[] pixelData, int width, int height, ColorDecoder decoder)
            : base(ImgUtils.unSwizzle(pixelData, width, height, decoder.BitDepth), width, height, decoder)
        {

        }
    }

}