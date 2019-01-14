using System;
using System.IO;
using Hessian.Net;
using Xunit;


namespace Hessian.NET.Tests
{
    public class HessianInputReaderTests
    {
        [Fact]
        public void BooleanTrue()
        {
            AssertInput(true, new[] { (byte)'T' }, reader => reader.ReadBoolean());
        }

        [Fact]
        public void BooleanFalse()
        {
            AssertInput(false, new[] { (byte)'F' }, reader => reader.ReadBoolean());
        }

        [Fact]
        public void BytesCompactEmpty()
        {
            AssertInput(null, new[] { (byte)'N' }, reader => reader.ReadBytes());
        }

        [Fact]
        public void BytesCompactOne()
        {
            AssertInput(new[] { (byte)0x01 }, new[] { (byte)0x21, (byte)0x01 }, reader => reader.ReadBytes());
        }

        [Fact]
        public void BytesCompact15()
        {
            AssertInput(
                new byte[] {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A},
                new byte[] {0x2A, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A},
                reader => reader.ReadBytes()
                );
        }

        [Fact]
        public void BytesSingleChunk()
        {
            AssertInput(
                new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x012, 0x13, 0x14, 0x15 },
                new byte[] {(byte)'B', 0x00, 0x15, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x012, 0x13, 0x14, 0x15 },
                reader => reader.ReadBytes()
                );
        }

        [Fact]
        public void BytesMultiChunk()
        {
            AssertInput(
                new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x012, 0x13, 0x14, 0x15 },
                new byte[] { (byte)'b', 0x00, 0x10, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, (byte)'B', 0x00, 0x05, 0x11, 0x012, 0x13, 0x14, 0x15 },
                reader => reader.ReadBytes()
                );
        }

        [Fact]
        public void Int32SingleOctet()
        {
            AssertInput(0, new[] { (byte)0x90 }, reader => reader.ReadInt32());
            AssertInput(-16, new[] { (byte)0x80 }, reader => reader.ReadInt32());
            AssertInput(47, new[] { (byte)0xBF }, reader => reader.ReadInt32());
        }

        [Fact]
        public void Int32TwoOctets()
        {
            AssertInput(-2048,new byte[] { 0xC0, 0x00 }, reader => reader.ReadInt32());
            AssertInput(-256, new byte[] { 0xC7, 0x00 }, reader => reader.ReadInt32());
            AssertInput(2047, new byte[] { 0xCF, 0xFF }, reader => reader.ReadInt32());
        }

        [Fact]
        public void Int32ThreeOctets()
        {
            AssertInput(-262144, new byte[] { 0xD0, 0x00, 0x00 }, reader => reader.ReadInt32());
            AssertInput(262143, new byte[] { 0xD7, 0xFF, 0xFF }, reader => reader.ReadInt32());
        }

        [Fact]
        public void Int32Full()
        {
            AssertInput(262144, new byte[] { 0x49, 0x00, 0x04, 0x00, 0x00 }, reader => reader.ReadInt32());
        }

        [Fact]
        public void Int64SingleOctet()
        {
            AssertInput(0L, new byte[] { 0xE0 }, reader => reader.ReadInt64());
            AssertInput(-8L, new byte[] { 0xD8 }, reader => reader.ReadInt64());
            AssertInput(15L, new byte[] { 0xEF }, reader => reader.ReadInt64());
        }

        [Fact]
        public void Int64TwoOctet()
        {
            AssertInput(-256L, new byte[] { 0xF7, 0x00 }, reader => reader.ReadInt64());
            AssertInput(-2048L, new byte[] { 0xF0, 0x00 }, reader => reader.ReadInt64());
            AssertInput(2047L, new byte[] { 0xFF, 0xFF }, reader => reader.ReadInt64());
        }

        [Fact]
        public void Int64ThreeOctet()
        {
            AssertInput(-262144L, new byte[] { 0x38, 0x00, 0x00 }, reader => reader.ReadInt64());
            AssertInput(262143L, new byte[] { 0x3F, 0xFF, 0xFF }, reader => reader.ReadInt64());
        }

        [Fact]
        public void Int64Full()
        {
            AssertInput(0x80000000L, new byte[] { 0x4C, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00 }, reader => reader.ReadInt64());
        }

        [Fact]
        public void DoubleZero()
        {
            AssertInput(0.0d, new byte[] { 0x5B }, reader => reader.ReadDouble());
        }

        [Fact]
        public void DoubleOne()
        {
            AssertInput(1.0d, new byte[] { 0x5C }, reader => reader.ReadDouble());
        }

        [Fact]
        public void DoubleOctet()
        {
            AssertInput(123.0d, new byte[] { 0x5D, 0x7B }, reader => reader.ReadDouble());
        }

        [Fact]
        public void DoubleShort()
        {
            AssertInput(-32768.0d, new byte[] { 0x5E, 0x80, 0x00 }, reader => reader.ReadDouble());
            AssertInput(32767.0d, new byte[] { 0x5E, 0x7F, 0xFF }, reader => reader.ReadDouble());
        }

        [Fact]
        public void DoubleFloat()
        {
            AssertInput(value => Assert.Equal(Single.MinValue, Convert.ToDouble(value)), new byte[] {0x5F, 0xFF, 0x7F, 0xFF, 0xFF}, reader => reader.ReadDouble());
            AssertInput(value => Assert.Equal(Single.MaxValue, Convert.ToDouble(value)), new byte[] {0x5F, 0x7F, 0x7F, 0xFF, 0xFF}, reader => reader.ReadDouble());
        }

        [Fact]
        public void DoubleFull()
        {
            AssertInput(Double.MaxValue - 1, new byte[] { 0x5A, 0x7F, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, reader => reader.ReadDouble());
        }

        [Fact]
        public void StringEmpty()
        {
            AssertInput(String.Empty, new byte[] {0x00}, reader => reader.ReadString());
        }

        [Fact]
        public void StringShort()
        {
            AssertInput("\u00c3", new byte[] {0x01, 0xC3, 0x83}, reader => reader.ReadString());
            AssertInput("hello", new byte[] {0x05, 0x68, 0x65, 0x6C, 0x6C, 0x6F}, reader => reader.ReadString());
        }

        [Fact]
        public void StringCompact()
        {
            const string expected = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat";
            var data = new byte[]
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

            AssertInput(expected, data, reader => reader.ReadString());
        }

        [Fact]
        public void DateTimeShort()
        {
            var data = new byte[] { 0x4B, 0x00, 0xE3, 0x83, 0x8F };
            AssertInput(new DateTime(1998, 5, 8, 9, 51, 00, DateTimeKind.Utc), data, reader => reader.ReadDateTime());
        }

        [Fact]
        public void DateTimeFull()
        {
            var data = new byte[] { 0x4A, 0x00, 0x00, 0x00, 0xD0, 0x4B, 0x92, 0x84, 0xB8 };
            AssertInput(new DateTime(1998, 5, 8, 9, 51, 31, DateTimeKind.Utc), data, reader => reader.ReadDateTime());
        }

        private static void AssertInput(Action<object> validator, byte[] input, Func<HessianInputReader, object> action)
        {
            var memoryStream = new MemoryStream(input);
            var reader = new HessianInputReader(memoryStream);

            validator(action(reader));
        }

        private static void AssertInput(object expected, byte[] input, Func<HessianInputReader, object> action)
        {
            var memoryStream = new MemoryStream(input);
            var reader = new HessianInputReader(memoryStream);

            Assert.Equal(expected, action(reader));
        }

        private static void AssertInput(byte[] expected, byte[] input, Func<HessianInputReader, byte[]> action)
        {
            var memoryStream = new MemoryStream(input);
            var reader = new HessianInputReader(memoryStream);
            var temp = action(reader);

            if (!ReferenceEquals(expected, temp))
            {
                
                Assert.Equal(expected.Length,temp.Length);
                

                for (var index = 0; index < expected.Length; index++)
                {
                   
                    Assert.Equal(expected[index], temp[index]);
                    
                }
            }
        }
    }
}