using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public partial class NUTTexture : TextureContainer
    {
        internal static readonly string NAME = "Gamecube NUT Archive";

        internal class NUTSegmentParameters
        {
            public enum PaletteFormat { IA8, RGB565, RGB5A3, None};

            internal int width, height;
            internal byte bpp;
            internal byte mipmapCount;
            internal PaletteFormat paletteFormat;

            //raw header data we don't mind to process (I hope so).
            internal byte format;
            internal byte[] data = new byte[24];
            internal byte[] userdata = new byte[0];
        }

        public override string Name
        {
            get { return NAME; }
        }
    }
}
