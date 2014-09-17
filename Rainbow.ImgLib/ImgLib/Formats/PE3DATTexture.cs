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
using System.IO;
using System.Linq;
using System.Text;

using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Filters;

using System.Drawing;

namespace Rainbow.ImgLib.Formats
{
    public class PE3DATTexture : TextureFormatBase
    {
        internal static readonly string NAME = "The 3rd Brithday DAT";

        private uint[] positions1, positions2;
        private ushort[] widths, heights;
        private int[] bpps;
        private List<byte[]> imagesData;
        private List<Color[]> palettes;

        internal PE3DATTexture(BinaryReader reader,uint[] pos1,uint[] pos2,ushort[] w,ushort[] h,int[] bpps)
        {
            positions1=pos1;
            positions2=pos2;
            widths=w;
            heights=h;
            this.bpps=bpps;

            imagesData = new List<byte[]>(bpps.Length);
            palettes = new List<Color[]>(bpps.Length);

            
            for(int i=0;i<bpps.Length;i++)
            {
                int count = widths[i] * heights[i];
                if (bpps[i] == 4)
                    count /= 2;
                if(bpps[i]!=8 && bpps[i]!=4)
                    throw new TextureFormatException("Illegal bpp value: "+bpps[i]);

                byte[] paletteData = new byte[bpps[i] == 8? 4 * 256 : 4 * 16];
                byte[] imageData = new byte[count];

                reader.Read(paletteData, 0, paletteData.Length);
                palettes.Add(ColorDecoder.DECODER_32BIT_RGBA.DecodeColors(paletteData));

                reader.Read(imageData,0,count);

               
                byte[] deSwizzled=new SwizzleFilter((int)widths[i], (int)heights[i], bpps[i]).Defilter(imageData);

                imagesData.Add(deSwizzled);
            }
        }

        internal PE3DATTexture(IList<Image> images, uint[] pos1,uint[] pos2, ushort[] w, ushort[] h,int[] bpps)
        {
            imagesData = new List<byte[]>(images.Count);
            palettes = new List<Color[]>(images.Count);
            positions1=pos1;
            positions2=pos2;
            widths=w;
            heights=h;
            this.bpps=bpps;

            for(int i=0;i<images.Count;i++)
            {
                if(bpps[i]!=8 && bpps[i]!=4)
                    throw new TextureFormatException("Illegal bpp value: "+bpps[i]);

                Image img=images[i];
                IndexedImageEncoder encoder = new IndexedImageEncoder(new List<Image> { img }, bpps[i] == 8 ? 256 : 16);
                imagesData.Add(encoder.Encode());
                palettes.Add(encoder.Palettes[0]);
            }
        }

        public override string Name
        {
            get { return NAME; }
        }

        public override int Width
        {
            get { return widths[SelectedFrame]; }
        }

        public override int Height
        {
            get { return heights[SelectedFrame]; }
        }

        public override int FramesCount
        {
            get { return bpps.Length; }
        }

        public override int PalettesCount
        {
            get { return 1; }
        }

        public int Bpp
        {
            get
            {
                return bpps[SelectedFrame];
            }
        }

        public uint Position1
        {
            get { return positions1[SelectedFrame]; }
        }

        public uint Position2
        {
            get { return positions2[SelectedFrame]; }
        }

        protected override System.Drawing.Image GetImage(int activeFrame, int activePalette)
        {
            IndexRetriever retriever = null;
            if (bpps[activeFrame] == 8)
                retriever = new IndexRetriever8Bpp();
            else
                retriever = new IndexRetriever4Bpp();

            IndexedImageDecoder decoder = new IndexedImageDecoder(imagesData[activeFrame],
                                                                  widths[activeFrame],
                                                                  heights[activeFrame],
                                                                  retriever, palettes[activeFrame]);
            return decoder.DecodeImage();
        }

        internal IList<byte[]> GetImagesData()
        {
            IList<byte[]> result = new List<byte[]>(imagesData.Count);
            for (int i = 0; i < imagesData.Count; i++)
                result.Add(new SwizzleFilter(widths[i], heights[i], bpps[i]).ApplyFilter(imagesData[i]));

            return result;
        }

        internal IList<byte[]> GetPalettesData()
        {
            IList<byte[]> pal = new List<byte[]>(palettes.Count);

            
            foreach(Color[] palette in palettes)
            {
                pal.Add(ColorEncoder.ENCODER_32BIT_RGBA.EncodeColors(palette));
            }

            return pal;
        }
    }
}
