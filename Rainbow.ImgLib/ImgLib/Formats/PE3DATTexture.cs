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
        private uint[] formats;
        private List<byte[]> imagesData;
        private List<Color[]> palettes;

        internal PE3DATTexture(BinaryReader reader,uint[] pos1,uint[] pos2,ushort[] w,ushort[] h,uint[] f)
        {
            positions1=pos1;
            positions2=pos2;
            widths=w;
            heights=h;
            formats=f;

            imagesData = new List<byte[]>(f.Length);
            palettes = new List<Color[]>(f.Length);

            
            for(int i=0;i<formats.Length;i++)
            {
                int count = widths[i] * heights[i];
                if (formats[i] != 2)
                    count /= 2;
                byte[] paletteData = new byte[formats[i] == 2 ? 4 * 256 : 4 * 16];
                byte[] imageData = new byte[count];

                reader.Read(paletteData, 0, paletteData.Length);
                palettes.Add(ColorDecoder.DECODER_32BIT_RGBA.DecodeColors(paletteData));

                reader.Read(imageData,0,count);

                imagesData.Add(new SwizzleFilter((int)widths[i],(int)heights[i],(int)formats[i]*4).Defilter(imageData));
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
            get { return formats.Length; }
        }

        public override int PalettesCount
        {
            get { return 1; }
        }

        public int Bpp
        {
            get
            {
                return formats[SelectedFrame] == 2 ? 8 : 4;
            }
        }

        protected override System.Drawing.Image GetImage(int activeFrame, int activePalette)
        {
            IndexRetriever retriever = null;
            if(formats[activeFrame]==2)
                retriever=new IndexRetriever8Bpp();
            else
                retriever=new IndexRetriever4Bpp();

            IndexedImageDecoder decoder = new IndexedImageDecoder(imagesData[activeFrame],
                                                                  widths[activeFrame],
                                                                  heights[activeFrame],
                                                                  retriever, palettes[activeFrame]);
            return decoder.DecodeImage();
        }
    }
}
