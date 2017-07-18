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
using System.Drawing;
using Rainbow.ImgLib.Common;

namespace Rainbow.ImgLib.Formats
{
    public abstract class TextureFormatBase : TextureFormat
    {
        private GenericDictionary specificData = new GenericDictionary();

        private int activeFrame, activePalette;

        private int mipmapsCount;

        public abstract string Name { get;  }
        
        public abstract int Width{ get; }

        public abstract int Height { get; }

        public abstract int FramesCount { get; }

        public abstract int PalettesCount { get;  }

        public virtual int MipmapsCount { get { return mipmapsCount; }}

        public TextureFormatBase(int mipmapsCount=1)
        {
            this.mipmapsCount = mipmapsCount;
        }

        public abstract Image GetReferenceImage();

        public Image GetMipmap(int i)
        {
            if (i < 0 || i >= MipmapsCount)
            {
                throw new ArgumentException("Mipmap index out of range!");
            }

            Image img = GetImage();


            return img.GetMipmap(i);
        }

        public event EventHandler TextureChanged;

        public int SelectedFrame
        {
            get { return activeFrame; }
            set
            {
                if (value < 0 || value >= FramesCount)
                {
                    throw new IndexOutOfRangeException();
                }

                bool changed = activeFrame != value;

                activeFrame = value;

                if (changed)
                {
                    OnTextureChanged();
                }
            }
        }

        public int SelectedPalette
        {
            get{ return activePalette;}
            set
            {
                if (PalettesCount == 0)
                {
                    return;
                }

                if (value < 0 || value >= PalettesCount)
                {
                    throw new IndexOutOfRangeException();
                }
                bool changed = activePalette != value;
                activePalette = value;

                if (changed)
                {
                    OnTextureChanged();
                }
            }
        }

        public Image GetImage()
        {
            return GetImage(activeFrame, activePalette);
        }

        public Color[] Palette
        {
            get
            {
                return PalettesCount == 0? null : GetPalette(activePalette);
            }
        }

        public GenericDictionary FormatSpecificData 
        {
            get { return specificData; }
            set { specificData = value; }
        }

        protected abstract Image GetImage(int activeFrame, int activePalette);

        protected abstract Color[] GetPalette(int paletteIndex);

        protected void OnTextureChanged()
        {
            if (TextureChanged != null)
            {
                TextureChanged(this, new EventArgs());
            }
        }
    }
}
