using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public partial class NUTTexture : TextureContainer
    {
        internal static readonly string NAME = "Gamecube NUT Archive";

        public override string Name
        {
            get { return NAME; }
        }

        public int Version
        {
            get;

            internal set;
        }
    }
}
