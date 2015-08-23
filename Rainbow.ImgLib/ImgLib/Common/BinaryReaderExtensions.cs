using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Common
{
    public enum ByteOrder { LittleEndian, BigEndian }

    public static class BinaryReaderExtensions
    {
        private static byte[] buf = new byte[8];

        public static UInt16 ReadUInt16(this BinaryReader reader, ByteOrder order)
        {
            if (order == ByteOrder.LittleEndian)
                return reader.ReadUInt16();

            reader.Read(buf, 0, 2);
            Array.Reverse(buf, 0, 2);
            return BitConverter.ToUInt16(buf, 0);
        }

        public static Int16 ReadInt16(this BinaryReader reader, ByteOrder order)
        {
            if (order == ByteOrder.LittleEndian)
                return reader.ReadInt16();

            reader.Read(buf, 0, 2);
            Array.Reverse(buf, 0, 2);
            return BitConverter.ToInt16(buf, 0);
        }

        public static UInt32 ReadUInt32(this BinaryReader reader, ByteOrder order)
        {
            if (order == ByteOrder.LittleEndian)
                return reader.ReadUInt32();

            reader.Read(buf, 0, 4);
            Array.Reverse(buf, 0, 4);
            return BitConverter.ToUInt32(buf, 0);
        }

        public static Int32 ReadInt32(this BinaryReader reader, ByteOrder order)
        {
            if (order == ByteOrder.LittleEndian)
                return reader.ReadInt32();

            reader.Read(buf, 0, 4);
            Array.Reverse(buf, 0, 4);
            return BitConverter.ToInt32(buf, 0);
        }

        public static UInt64 ReadUInt64(this BinaryReader reader, ByteOrder order)
        {
            if (order == ByteOrder.LittleEndian)
                return reader.ReadUInt64();

            reader.Read(buf, 0, 8);
            Array.Reverse(buf, 0, 8);
            return BitConverter.ToUInt64(buf, 0);
        }

        public static Int64 ReadInt64(this BinaryReader reader, ByteOrder order)
        {
            if (order == ByteOrder.LittleEndian)
                return reader.ReadInt64();

            reader.Read(buf, 0, 8);
            Array.Reverse(buf, 0, 8);
            return BitConverter.ToInt64(buf, 0);
        }
    }
}
