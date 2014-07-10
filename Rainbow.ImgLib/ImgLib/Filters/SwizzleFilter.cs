using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Filters
{
    public class SwizzleFilter : Filter<byte>
    {
        private int width,height,bitDepth;

        public SwizzleFilter(int width,int height,int bitDepth)
        {
            this.width=width;
            this.height = height;
            this.bitDepth = bitDepth;
        }


        public override byte[] ApplyFilter(byte[] originalData, int index, int length)
        {
            byte[] Buf = new byte[length];
            int val = 16;
            int w = (this.width * bitDepth) / 8;
            int rowblocks = w / val;

            int totalBlocksx = w / val;
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
                                originalData[index + absolutex + absolutey * w];
                            
                        }
                }

            int start = totalBlocksy * rowblocks * val * 8;
            for (int i = start; i < length; i++)
                Buf[i] = originalData[i + index];

            return Buf;
        }

        public override byte[] Defilter(byte[] originalData, int index, int length)
        {
            byte[] Buf = new byte[length];
            int val = 16;
            int w = (this.width * bitDepth) / 8;
            int rowblocks = w / val;

            int totalBlocksx = w / val;
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
                           
                            Buf[absolutex + absolutey * w] =
                                  originalData[index + block_address + x + y * val];
                            
                        }
                }

            int start = totalBlocksy * rowblocks * val * 8;
            for (int i = start; i < length; i++)
                Buf[i] = originalData[i + index];

            return Buf;
        }
    }
}
