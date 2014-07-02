using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using nQuant;

namespace TIM2Conv
{
    class TIM2Exception : ApplicationException
    {
        public TIM2Exception(string msg)
            : base(msg)
        {

        }
    }
    class TIM2
    {
        private byte[] fullHeader = new byte[0x40];

        private uint dataSize, paletteSize;

        private Image image;

        public TIM2(Stream tim2Stream, bool swizzled = false)
        {
            Swizzled = swizzled;
            initTIM2(tim2Stream);
        }

        public TIM2(Stream pngStream, Stream tmexStream, bool swizzled = false)
        {
            Swizzled = swizzled;
            initPNG(pngStream, tmexStream);
        }

        public TIM2(string filename, bool swizzled = false)
        {
            Swizzled = swizzled;
            FileStream s = null;
            FileStream tmexStream = null;

            try
            {
                if (Path.GetExtension(filename) == ".tm2")
                    initTIM2(s = File.Open(filename, FileMode.Open));

                else if (Path.GetExtension(filename) == ".tmex" || Path.GetExtension(filename) == ".png")
                {
                    string tmexname = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".tmex");
                    string pngname = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".png");
                    initPNG(s = File.Open(pngname, FileMode.Open), tmexStream = File.Open(tmexname,FileMode.Open));

                }else
                    throw new TIM2Exception("Invalid input file!");
            }
            finally
            {
                if(s!=null) s.Close();
                if (tmexStream != null) tmexStream.Close();
            }
        }

        private void initPNG(Stream s, Stream tmex)
        {
            tmex.Read(fullHeader, 0, fullHeader.Length);
            FillHeader();

            Bitmap img = new Bitmap(s);

            IWuQuantizer quantizer = new WuQuantizer(ColorsCount);

            image = quantizer.QuantizeImage(img, 10, 70);

        }

        private void initTIM2(Stream stream)
        {
            stream.Read(fullHeader, 0, fullHeader.Length);

            FillHeader();

            byte[] imageData = new byte[dataSize];
            stream.Read(imageData, 0, (int)dataSize);

            byte[] paletteData = new byte[paletteSize];
            stream.Read(paletteData, 0, (int)paletteSize);
            
            if (Swizzled)
            {
                byte[] newdata = new byte[imageData.Length];
                unswizzle(imageData, imageData.Length, Width, Height, BitDepth, newdata);
                imageData = newdata;
            }

            if (BitDepth == 4)
            {
                invertNibbles(imageData);
            }

            image = ToImage(imageData, paletteData);
        }

        private void FillHeader()
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(fullHeader));

            char[] magic = reader.ReadChars(4);
            if (new string(magic) != "TIM2")
                throw new TIM2Exception("Invalid TIM2 image!");

            Version = reader.ReadUInt16();
            ImagesCount = reader.ReadUInt16();
            reader.BaseStream.Position += 8;

            uint totalSize = reader.ReadUInt32();
            paletteSize = reader.ReadUInt32();
            dataSize = reader.ReadUInt32();
            ushort unknown1 = reader.ReadUInt16();
            ColorsCount = reader.ReadUInt16();

            ThrowIfInvalidColors();
            uint unknown2 = reader.ReadUInt32();

            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();

            byte[] unknown3 = reader.ReadBytes(24);

            reader.Close();
        }

        private void invertNibbles(byte[] image_raw)
        {
            for (int i = 0; i < image_raw.Length; i++)
            {
                byte b = 0;
                b = (byte)(b | image_raw[i] & 0xF & 0xFF);
                b = (byte)(b << 4);
                b = (byte)(b | (image_raw[i] & 0xF0) >> 4 & 0xFF);
                image_raw[i] = b;
            }
        }

        private byte[] compressNibbles(byte[] image_raw)
        {
            byte[] newData = new byte[image_raw.Length / 2];
            for (int i = 0; i < image_raw.Length; i+=2)
            {
                byte b = (byte)(image_raw[i] & 0xF | (image_raw[i + 1] << 4) & 0xF0);
                newData[i / 2] = b;
            }
            return newData;
        }
        private void unswizzle(byte[] Swizzled, int Length, int w, int height, int bit_depth, byte[] Buf)
        {
            int val = 16;
            int width = bit_depth == 4 ? w / 2 : w * (bit_depth / 8);
            int rowblocks = width / val;

            int totalBlocksx = width/val;
            int totalBlocksy = height/8;

            for(int blocky=0; blocky < totalBlocksy ; blocky++)
                for (int blockx = 0; blockx < totalBlocksx; blockx++)
                {
                    int block_index = blockx + blocky * rowblocks;
                    int block_address = block_index * val * 8;

                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < val; x++)
                        {
                            int absolutex = x + blockx * val;
                            int absolutey = y + blocky * 8;

                            Buf[absolutex + absolutey * width] =
                                Swizzled[block_address + x + y * val];
                        }
                }

            int start = totalBlocksy * rowblocks * val * 8;
            for (int i = start; i < Swizzled.Length; i++)
                Buf[i] = Swizzled[i];  
        }

        private void swizzle(byte[] unSwizzled, int Length, int w, int height, int bit_depth, byte[] Buf)
        {
            int val = 16;
            int width = bit_depth == 4 ? w / 2 : w * (bit_depth / 8);
            int rowblocks = width / val;

            int totalBlocksx = width / val;
            int totalBlocksy = height / 8;

            for (int blocky = 0; blocky < totalBlocksy; blocky++)
                for (int blockx = 0; blockx < totalBlocksx; blockx++)
                {
                    int block_index = blockx + blocky * rowblocks;
                    int block_address = block_index * val * 8;

                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < val; x++)
                        {
                            int absolutex = x + blockx * val;
                            int absolutey = y + blocky * 8;

                            Buf[block_address + x + y * val] =
                                unSwizzled[absolutex + absolutey * width];
                        }
                }

            int start = totalBlocksy * rowblocks * val * 8;
            for (int i = start; i < unSwizzled.Length; i++)
                Buf[i] = unSwizzled[i];
        }
        private Image ToImage(byte[] imageData, byte[] paletteData)
        {
       
            Bitmap bitmap = new Bitmap(Width, Height, 
                                       GetStride(), 
                                       PixelFormat, 
                                       System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(imageData,0));

            ColorPalette pal = bitmap.Palette;
            for (int i = 0; i < ColorsCount; i++)
                pal.Entries[i] = Color.FromArgb(paletteData[i * 4 + 3], paletteData[i * 4], paletteData[i * 4 + 1], paletteData[i * 4 + 2]);

            bitmap.Palette = pal;

            return bitmap;
        }

        private int GetStride()
        {
            return (Width * BitDepth) / 8;
        }
        private void ThrowIfInvalidColors()
        {
            if (ColorsCount != 16 && ColorsCount != 256)
                throw new TIM2Exception("Unsupported number of colors!");
        }

        public int Version { get; private set; }

        public int ImagesCount { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int ColorsCount { get; private set; }

        public bool Swizzled { get; private set; }
        public int BitDepth
        {
            get
            {
                switch (ColorsCount)
                {
                    case 16:
                        return 4;
                    case 256:
                        return 8;
                    default:
                        throw new TIM2Exception("Unsupported number of colors!");
                }
            }
        }

        public PixelFormat PixelFormat
        {
            get
            {
                switch (BitDepth)
                {
                    case 4:
                        return PixelFormat.Format4bppIndexed;
                    case 8:
                        return PixelFormat.Format8bppIndexed;
                    default:
                        throw new TIM2Exception("Unsupported pixel format!");
                }
            }
        }
        public Image getImage()
        {
            return image;
        }

        public void Export(string filename)
        {
            if (Path.GetExtension(filename) == ".png")
            {
                Image img = getImage();

                img.Save(filename);

                string name = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".tmex");

                File.WriteAllBytes(name, fullHeader);
            }
            else if (Path.GetExtension(filename) == ".tm2")
            {
                Bitmap img = new Bitmap(getImage());

                var palette=new Dictionary<Color,int>();
                byte[] paletteData = new byte[ColorsCount * 4];

                byte[] image = new byte[Width * Height];

                int k = 0;
                for(int x=0;x<img.Width;x++)
                    for (int y = 0; y < img.Height; y++)
                    {
                        Color c = img.GetPixel(x, y);
                        int val;
                        if (!palette.TryGetValue(c, out val))
                        {
                            val = k++;
                            palette[c] = val;
                            paletteData[val * 4] = c.R;
                            paletteData[val * 4 + 1] = c.G;
                            paletteData[val * 4 + 2] = c.B;
                            paletteData[val * 4 + 3] = c.A;
                        }
                        image[x + y*Width] = (byte)val;
                    }

                if (BitDepth == 4)
                {
                    image=compressNibbles(image);
                }

                if(Swizzled)
                {
                    byte[] buf=new byte[image.Length];
                    swizzle(image, image.Length, Width, Height, BitDepth, buf);
                    image = buf;
                }
                FileStream outStream=File.Open(filename, FileMode.Create);
                outStream.Write(fullHeader, 0, fullHeader.Length);
                outStream.Write(image, 0, image.Length);
                outStream.Write(paletteData, 0, paletteData.Length);
                outStream.Close();

            }
            else
                throw new TIM2Exception("Invalid output file format!");
        }


    }
    class TIM2Utils
    {

    }
}
