using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Implementation
{
    public class NamcoTexture : TextureContainer
    {
        internal static readonly string NAME = "Namco Texture Container";

        private string specificName = NAME;
        public override string Name
        {
            get { return specificName; }
        }

        internal void SetName(string name)
        {
            specificName = name;
        }

        public string Version { get { return GetTextureSpecificData(NamcoTextureSerializer.VERSION_KEY); } }

        public string ClutFormat 
        { 
            get 
            { 
                return GetCurrentFrameSpecificData(NamcoTextureSerializer.CLUTFORMAT_KEY);
            } 
        }

        public string Format
        {
            get
            {
                return GetCurrentFrameSpecificData(NamcoTextureSerializer.FORMAT_KEY);
            }
        }

        public string Depth
        {
            get
            {
                return GetCurrentFrameSpecificData(NamcoTextureSerializer.DEPTH_KEY);
            }
        }
    }
}
