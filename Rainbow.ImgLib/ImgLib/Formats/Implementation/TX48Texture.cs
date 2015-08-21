using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public partial class TX48Texture : TextureContainer
    {
        internal const string NAME = "TX48 (Super Robot Wars MX P)";

        public override string Name
        {
            get { return NAME; }
        }

        public int Bpp
        {
            get { return ((TX48Texture.Segment)TextureFormats.First()).Bpp;  }
        }
    }
}
