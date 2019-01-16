using System;
using System.Runtime.InteropServices;

namespace Hessian.Platform
{
    public abstract class EndianBitConverter
    {
        #region T -> byte[]

        public byte[] GetBytes(bool value)
        {
            // One byte, no endianness
            return BitConverter.GetBytes(value);
        }

        public byte[] GetBytes(char value)
        {
            return GetBytes(value, sizeof (char));
        }

        public byte[] GetBytes(short value)
        {
            return GetBytes(value, sizeof (short));
        }

        public byte[] GetBytes(ushort value)
        {
            return GetBytes(value, sizeof (ushort));
        }

        public byte[] GetBytes(int value)
        {
            return GetBytes(value, sizeof (int));
        }

        public byte[] GetBytes(uint value)
        {
            return GetBytes(value, sizeof (uint));
        }

        public byte[] GetBytes(long value)
        {
            return GetBytes(value, sizeof (long));
        }

        public byte[] GetBytes(ulong value)
        {
            return GetBytes((long)value, sizeof (ulong));
        }

        public byte[] GetBytes(float value)
        {
            return GetBytes(SingleToInt32(value), sizeof (int));
        }

        public byte[] GetBytes(double value)
        {
            return GetBytes(DoubleToInt64(value), sizeof (long));
        }

        private byte[] GetBytes(long value, int size)
        {
            var buffer = new byte[size];
            CopyBytes(value, buffer, 0, size);
            return buffer;
        }

        #endregion

        #region byte[] -> T

        public bool ToBoolean(byte[] value, int index)
        {
            // one byte, no endianness
            return BitConverter.ToBoolean(value, index);
        }

        public char ToChar(byte[] value, int index)
        {
            return (char) FromBytes(value, index, sizeof (char));
        }

        public short ToInt16(byte[] value, int index)
        {
            return (short) FromBytes(value, index, sizeof (short));
        }

        public ushort ToUInt16(byte[] value, int index)
        {
            return (ushort) FromBytes(value, index, sizeof (ushort));
        }

        public int ToInt32(byte[] value, int index)
        {
            return (int) FromBytes(value, index, sizeof (int));
        }

        public uint ToUInt32(byte[] value, int index)
        {
            return (uint) FromBytes(value, index, sizeof (uint));
        }

        public long ToInt64(byte[] value, int index)
        {
            return FromBytes(value, index, sizeof (long));
        }

        public ulong ToUInt64(byte[] value, int index)
        {
            return (ulong) FromBytes(value, index, sizeof (ulong));
        }

        public float ToSingle(byte[] value, int index)
        {
            var int32 = (int) FromBytes(value, index, sizeof (int));
            return Int32ToSingle(int32);
        }

        public double ToDouble(byte[] value, int index)
        {
            var int64 = FromBytes(value, index, sizeof (long));
            return Int64ToDouble(int64);
        }

        #endregion

        protected abstract long FromBytes(byte[] bytes, int offset, int count);

        protected abstract void CopyBytes(long source, byte[] buffer, int index, int count);

        private static int SingleToInt32(float value)
        {
            return new JonSkeetUnion32(value).AsInt;
        }

        private static float Int32ToSingle(int value)
        {
            return new JonSkeetUnion32(value).AsFloat;
        }

        private static long DoubleToInt64(double value)
        {
            return new JonSkeetUnion64(value).AsLong;
        }

        private static double Int64ToDouble(long value)
        {
            return new JonSkeetUnion64(value).AsDouble;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct JonSkeetUnion32
        {
            [FieldOffset(0)]
            private readonly int i;

            [FieldOffset(0)]
            private readonly float f;

            public int AsInt
            {
                get { return i; }
            }

            public float AsFloat
            {
                get { return f; }
            }

            public JonSkeetUnion32(int value)
            {
                f = 0;
                i = value;
            }

            public JonSkeetUnion32(float value)
            {
                i = 0;
                f = value;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct JonSkeetUnion64
        {
            [FieldOffset(0)]
            private readonly long l;

            [FieldOffset(0)]
            private readonly double d;

            public long AsLong
            {
                get { return l; }
            }

            public double AsDouble
            {
                get { return d; }
            }

            public JonSkeetUnion64(long value)
            {
                d = 0;
                l = value;
            }

            public JonSkeetUnion64(double value)
            {
                l = 0;
                d = value;
            }
        }
    }
}
