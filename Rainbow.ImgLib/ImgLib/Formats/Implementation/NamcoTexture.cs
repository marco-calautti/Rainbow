//Copyright (C) 2014+ Marco (Phoenix) Calautti.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 2.0.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License 2.0 for more details.

//A copy of the GPL 2.0 should have been included with the program.
//If not, see http://www.gnu.org/licenses/

//Official repository and contact information can be found at
//http://github.com/marco-calautti/Rainbow

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
