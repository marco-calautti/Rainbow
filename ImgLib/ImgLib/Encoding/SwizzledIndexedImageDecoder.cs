using System.Drawing;
using ImgLib.Common;

namespace ImgLib.Encoding
{
    public class SwizzledIndexedImageDecoder : IndexedImageDecoder
    {

        public SwizzledIndexedImageDecoder(byte[] pixel, int width, int height, IndexRetriever retriever, Color[] palette = null)
            : base(ImgUtils.unSwizzle(pixel,width,height,retriever.BitDepth), width, height, retriever, palette)
        {
  
        }
    }
}