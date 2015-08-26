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

        public int Version { get; internal set; }

        public string ClutFormat 
        { 
            get 
            { 
                string key=NamcoTextureSerializer.CLUTFORMAT_KEY;
                return TextureFormats[SelectedFrame].FormatSpecificData.Get(key);
            } 
        }

        public string Format
        {
            get
            {
                string key = NamcoTextureSerializer.FORMAT_KEY;
                return TextureFormats[SelectedFrame].FormatSpecificData.Get(key);
            }
        }

        public string Depth
        {
            get
            {
                string key = NamcoTextureSerializer.DEPTH_KEY;
                return TextureFormats[SelectedFrame].FormatSpecificData.Get(key);
            }
        }
    }
}
