using System;

namespace Rainbow.ImgLib.Encoding
{
    public abstract class IndexPacker
    {
        public virtual byte[] PackIndexes(int[] indexes)
        {
            return PackIndexes(indexes, 0, indexes.Length);
        }

        public abstract byte[] PackIndexes(int[] indexes, int start, int length);
        public abstract int BitDepth { get; }

        public static IndexPacker FromNumberOfColors(int colors)
        {
            if (colors <= 16)
                return new IndexPacker4Bpp();
            else if (colors <= 256)
                return new IndexPacker8Bpp();
            else
                throw new ArgumentException("Unsupported number of colors");
        }
    }
}
