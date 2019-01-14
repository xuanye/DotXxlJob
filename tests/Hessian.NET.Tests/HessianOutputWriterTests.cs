using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Hessian.Net;
using Xunit;

namespace Hessian.NET.Tests
{
    //
    // http://hessian.caucho.com/doc/hessian-serialization.html##date
    //
    public class HessianOutputWriterTests
    {
        [Fact]       
        public void BooleanTrue()
        {
            AssertOutput(new[] { (byte)'T' }, writer => writer.WriteBoolean(true));
        }

        [Fact]       
        public void BooleanFalse()
        {
            AssertOutput(new[] { (byte)'F' }, writer => writer.WriteBoolean(false));
        }

        [Fact]       
        public void BytesCompactEmpty()
        {
            AssertOutput(new[] { (byte)0x20 }, writer => writer.WriteBytes(new byte[0]));
        }

        [Fact]       
        public void BytesCompactOne()
        {
            AssertOutput(new[] { (byte)0x21, (byte)0x01 }, writer => writer.WriteBytes(new byte[] { 1 }));
        }

        [Fact]       
        public void BytesCompact15()
        {
            var value = new byte[15];

            for (var index = 0; index < value.Length; index++)
            {
                value[index] = (byte)index;
            }

            var expected = new byte[16];

            expected[0] = (byte)(0x20 + value.Length);
            value.CopyTo(expected, 1);

            AssertOutput(expected, writer => writer.WriteBytes(value));
        }

        [Fact]
        public void BytesSingleChunk()
        {
            var data = new byte[255];

            for (var index = 0; index < data.Length; index++)
            {
                data[index] = (byte)index;
            }

            var expected = new byte[3 + data.Length];

            WriteBytesLengthPrefix(expected, 0, data.Length, true);

            data.CopyTo(expected, 3);

            AssertOutput(expected, writer => writer.WriteBytes(data));
        }

        [Fact]
        public void BytesMultiChunk()
        {
            const int sigLength = 3;
            const int chunkSize = 0x8000;
            const int appendixSize = 3;
            var buffer = new byte[chunkSize + appendixSize];

            for (var index = 0; index < buffer.Length; index++)
            {
                buffer[index] = (byte)(index + 1);
            }

            var expected = new byte[sigLength + chunkSize + sigLength + appendixSize];

            WriteBytesLengthPrefix(expected, 0, chunkSize, false);
            CopyBytes(buffer, 0, expected, sigLength, chunkSize);
            WriteBytesLengthPrefix(expected, sigLength + chunkSize, appendixSize, true);
            CopyBytes(buffer, chunkSize, expected, sigLength + chunkSize + sigLength, appendixSize);

            AssertOutput(expected, writer => writer.WriteBytes(buffer));
        }

        [Fact]
        public void DateTimeShort()
        {
            var expected = new byte[] { 0x4B, 0x00, 0xE3, 0x83, 0x8F };
            AssertOutput(expected, writer => writer.WriteDateTime(new DateTime(1998, 5, 8, 9, 51, 00, DateTimeKind.Utc)));
        }

        [Fact]
        public void DateTimeFull()
        {
            var expected = new byte[] { 0x4A, 0x00, 0x00, 0x00, 0xD0, 0x4B, 0x92, 0x84, 0xB8 };
            AssertOutput(expected, writer => writer.WriteDateTime(new DateTime(1998, 5, 8, 9, 51, 31, DateTimeKind.Utc)));
        }

        [Fact]
        public void DoubleZero()
        {
            AssertOutput(new byte[] { 0x5B }, writer => writer.WriteDouble(0.0d));
        }

        [Fact]
        public void DoubleOne()
        {
            AssertOutput(new byte[] { 0x5C }, writer => writer.WriteDouble(1.0d));
        }

        [Fact]
        public void DoubleOctet()
        {
            var expected = new byte[] { 0x5D, 0x7B };
            AssertOutput(expected, writer => writer.WriteDouble(123.0d));
        }

        [Fact]
        public void DoubleShort()
        {
            AssertOutput(new byte[] { 0x5E, 0x80, 0x00 }, writer => writer.WriteDouble(-32768d));
            AssertOutput(new byte[] { 0x5E, 0x7F, 0xFF }, writer => writer.WriteDouble(32767d));
        }

        [Fact]
        public void DoubleFloat()
        {
            AssertOutput(new byte[] { 0x5F, 0xFF, 0x7F, 0xFF, 0xFF }, writer => writer.WriteDouble(Single.MinValue));
            AssertOutput(new byte[] { 0x5F, 0x7F, 0x7F, 0xFF, 0xFF }, writer => writer.WriteDouble(Single.MaxValue));
        }

        [Fact]
        public void DoubleFull()
        {
//            var expected = new byte[] { 0x5A, 0x40, 0x28, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var expected = new byte[] { 0x5A, 0x7F, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            AssertOutput(expected, writer => writer.WriteDouble(Double.MaxValue - 1));
        }

        [Fact]
        public void Int32SingleOctet()
        {
            AssertOutput(new byte[] { 0x80 }, writer => writer.WriteInt32(-16));
            AssertOutput(new byte[] { 0x90 }, writer => writer.WriteInt32(0));
            AssertOutput(new byte[] { 0xBF }, writer => writer.WriteInt32(47));
        }

        [Fact]
        public void Int32TwoOctets()
        {
            AssertOutput(new byte[] { 0xC0, 0x00 }, writer => writer.WriteInt32(-2048));
            AssertOutput(new byte[] { 0xC7, 0x00 }, writer => writer.WriteInt32(-256));
            AssertOutput(new byte[] { 0xCF, 0xFF }, writer => writer.WriteInt32(2047));
        }

        [Fact]
        public void Int32ThreeOctets()
        {
            AssertOutput(new byte[] { 0xD0, 0x00, 0x00 }, writer => writer.WriteInt32(-262144));
            AssertOutput(new byte[] { 0xD7, 0xFF, 0xFF }, writer => writer.WriteInt32(262143));
        }

        [Fact]
        public void Int32Full()
        {
            AssertOutput(new byte[] { 0x49, 0x00, 0x04, 0x00, 0x00 }, writer => writer.WriteInt32(262144));
        }

        [Fact]
        public void Int64SingleOctet()
        {
            AssertOutput(new byte[] { 0xD8 }, writer => writer.WriteInt64(-8));
            AssertOutput(new byte[] { 0xE0 }, writer => writer.WriteInt64(0));
            AssertOutput(new byte[] { 0xEF }, writer => writer.WriteInt64(15));
        }

        [Fact]
        public void Int64TwoOctets()
        {
            AssertOutput(new byte[] { 0xF0, 0x00 }, writer => writer.WriteInt64(-2048));
            AssertOutput(new byte[] { 0xF7, 0x00 }, writer => writer.WriteInt64(-256));
            AssertOutput(new byte[] { 0xFF, 0xFF }, writer => writer.WriteInt64(2047));
        }

        [Fact]
        public void Int64ThreeOctets()
        {
            AssertOutput(new byte[] { 0x38, 0x00, 0x00 }, writer => writer.WriteInt64(-262144));
            AssertOutput(new byte[] { 0x3F, 0xFF, 0xFF }, writer => writer.WriteInt64(262143));
        }

        [Fact]
        public void Int64Full()
        {
            var expected = new byte[] { 0x4C, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00 };
            AssertOutput(expected, writer => writer.WriteInt64(0x80000000L));
        }

        [Fact]
        public void StringEmpty()
        {
            AssertOutput(new byte[] {0x00}, writer => writer.WriteString(null));
            AssertOutput(new byte[] {0x00}, writer => writer.WriteString(String.Empty));
        }

        [Fact]
        public void StringShort()
        {
            AssertOutput(new byte[] { 0x01, 0xC3, 0x83 }, writer => writer.WriteString("\u00c3"));
            AssertOutput(new byte[] { 0x05, 0x68, 0x65, 0x6C, 0x6C, 0x6F }, writer => writer.WriteString("hello"));
        }

        [Fact]
        public void StringCompact()
        {
            const string text = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat";
            var expected = new byte[]
            {
                0x30, 0x8f, 0x4c, 0x6f, 0x72, 0x65, 0x6d, 0x20, 0x69, 0x70, 0x73, 0x75, 0x6d, 0x20, 0x64, 0x6f,
                0x6c, 0x6f, 0x72, 0x20, 0x73, 0x69, 0x74, 0x20, 0x61, 0x6d, 0x65, 0x74, 0x2c, 0x20, 0x63, 0x6f,
                0x6e, 0x73, 0x65, 0x63, 0x74, 0x65, 0x74, 0x75, 0x65, 0x72, 0x20, 0x61, 0x64, 0x69, 0x70, 0x69,
                0x73, 0x63, 0x69, 0x6e, 0x67, 0x20, 0x65, 0x6c, 0x69, 0x74, 0x2c, 0x20, 0x73, 0x65, 0x64, 0x20,
                0x64, 0x69, 0x61, 0x6d, 0x20, 0x6e, 0x6f, 0x6e, 0x75, 0x6d, 0x6d, 0x79, 0x20, 0x6e, 0x69, 0x62,
                0x68, 0x20, 0x65, 0x75, 0x69, 0x73, 0x6d, 0x6f, 0x64, 0x20, 0x74, 0x69, 0x6e, 0x63, 0x69, 0x64,
                0x75, 0x6e, 0x74, 0x20, 0x75, 0x74, 0x20, 0x6c, 0x61, 0x6f, 0x72, 0x65, 0x65, 0x74, 0x20, 0x64,
                0x6f, 0x6c, 0x6f, 0x72, 0x65, 0x20, 0x6d, 0x61, 0x67, 0x6e, 0x61, 0x20, 0x61, 0x6c, 0x69, 0x71,
                0x75, 0x61, 0x6d, 0x20, 0x65, 0x72, 0x61, 0x74, 0x20, 0x76, 0x6f, 0x6c, 0x75, 0x74, 0x70, 0x61,
                0x74
            };

            AssertOutput(expected, writer => writer.WriteString(text));
        }

        private static void AssertOutput(byte[] expected, Action<HessianOutputWriter> action)
        {
            var stream = new MemoryStream();
            byte[] bytes;

            using (var writer = new HessianOutputWriter(stream))
            {
                action(writer);
                bytes = stream.ToArray();
            }

            //WriteOutput(bytes);

            Assert.True(ByteArray.Equals(expected, bytes));
        }

        private static void WriteOutput(byte[] bytes)
        {
            for (var offset = 0; offset < bytes.Length;)
            {
                var count = Math.Min(bytes.Length - offset, 16);
                var line = new StringBuilder();

                line.AppendFormat("{0:X08}: ", offset);

                for (var position = 0; position < count; position++)
                {
                    line.AppendFormat("{0:x02} ", bytes[offset]);
                    offset++;
                }

                Debug.WriteLine(line.ToString());
            }
        }

        private static void WriteBytesLengthPrefix(byte[] buffer, int offset, int length, bool final)
        {
            buffer[offset] = final ? (byte)'B' : (byte)'b';
            buffer[offset + 1] = (byte) (length >> 8);
            buffer[offset + 2] = (byte) length;
        }

        private static void CopyBytes(byte[] source, int srcoffset, byte[] dest, int destoffset, int count)
        {
            for (var index = 0; index < count; index++)
            {
                dest[destoffset + index] = source[srcoffset + index];
            }
        }

        /*private static void AssertOutput(bool value, byte[] expected)
        {
            var stream = new MemoryStream();
            byte[] bytes;

            using (var writer = new HessianOutputWriter(stream))
            {
                writer.WriteBoolean(value);
                bytes = stream.ToArray();
            }

            Assert.IsTrue(ByteArray.Equals(expected, bytes));
        }*/

        /*private static void AssertOutput(byte[] value, byte[] expected)
        {
            var stream = new MemoryStream();
            byte[] bytes;

            using (var writer = new HessianOutputWriter(stream))
            {
                writer.WriteBytes(value);
                bytes = stream.ToArray();
            }

            Assert.IsTrue(ByteArray.Equals(expected, bytes));
        }*/
    }
}
