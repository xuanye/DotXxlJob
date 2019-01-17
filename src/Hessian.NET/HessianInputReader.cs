using System;
using System.IO;
using System.Text;
using Hessian.Net.Extension;

namespace Hessian.Net
{
    public class HessianInputReader : DisposableStreamHandler
    {
        private ObjectPreamble preamble;

        public bool IsClassDefinition => ObjectPreamble.ClassDefinition == preamble;

        public bool IsInstanceReference => ObjectPreamble.InstanceReference == preamble;

        public LeadingByte LeadingByte
        {
            get;
        }

        public HessianInputReader(Stream stream)
            : base(stream)
        {
            LeadingByte = new LeadingByte();
        }

        /// <summary>
        /// Reads <see cref="System.Boolean" /> value from the stream.
        /// </summary>
        /// <returns>The value</returns>
        public virtual bool ReadBoolean()
        {
            ReadLeadingByte();

            if (LeadingByte.IsTrue)
            {
                return true;
            }

            if (!LeadingByte.IsFalse)
            {
                throw new HessianSerializerException();
            }

            return false;
        }

        public virtual int ReadInt32()
        {
            ReadLeadingByte();

            if (LeadingByte.IsTinyInt32)
            {
                return LeadingByte.Data - 144;
            }

            if (LeadingByte.IsShortInt32)
            {
                return ReadShortInt32();
            }

            if (LeadingByte.IsCompactInt32)
            {
                return ReadCompactInt32();
            }

            if (!LeadingByte.IsUnpackedInt32)
            {
                throw new HessianSerializerException();
            }

            return ReadUnpackedInt32();
        }

        public virtual long ReadInt64()
        {
            ReadLeadingByte();

            if (LeadingByte.IsTinyInt64)
            {
                return LeadingByte.Data - 224;
            }

            if (LeadingByte.IsShortInt64)
            {
                return ReadShortInt64();
            }

            if (LeadingByte.IsCompactInt64)
            {
                return ReadCompactInt64();
            }

            if (LeadingByte.IsPackedInt64)
            {
                return ReadPackedInt64();
            }
           
            if (!LeadingByte.IsUnpackedInt64)
            {
                throw new HessianSerializerException();
            }

            return ReadUnpackedInt64();
        }

        /// <summary>
        /// Reads binary data from stream.
        /// </summary>
        /// <returns>The array of bytes.</returns>
        public virtual byte[] ReadBytes()
        {
            ReadLeadingByte();

            if (LeadingByte.IsNull)
            {
                return null;
            }

            if (LeadingByte.IsCompactBinary)
            {
                return ReadBinaryCompact15();
            }

            var count = 0;
            var buffer = new byte[0];

            while (LeadingByte.IsNonfinalChunkBinary || LeadingByte.IsFinalChunkBinary)
            {
                var length = GetChunkLength();

                if (buffer.Length < (count + length))
                {
                    var temp = new byte[count + length];

                    Buffer.BlockCopy(buffer, 0, temp, 0, buffer.Length);

                    buffer = temp;
                }

                if (Stream.Read(buffer, count, length) != length)
                {
                    throw new HessianSerializerException();
                }

                count += length;

                if (LeadingByte.IsFinalChunkBinary)
                {
                    break;
                }

                ReadLeadingByte();
            }

            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual double ReadDouble()
        {
            ReadLeadingByte();

            if (LeadingByte.IsZeroDouble)
            {
                return 0.0d;
            }

            if (LeadingByte.IsOneDouble)
            {
                return 1.0d;
            }

            if (LeadingByte.IsTinyDouble)
            {
                return Stream.ReadByte();
            }

            if (LeadingByte.IsShortDouble)
            {
                return ReadShortDouble();
            }

            if (LeadingByte.IsCompactDouble)
            {
                return ReadCompactDouble();
            }

            if (!LeadingByte.IsUnpackedDouble)
            {
                throw new HessianSerializerException();
            }

            return ReadUnpackedDouble();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string ReadString()
        {
            ReadLeadingByte();

            if (LeadingByte.IsTinyString)
            {
                return GetStringChunk(LeadingByte.Data);
            }

            if (LeadingByte.IsCompactString)
            {
                return ReadCompactString();
            }

            var builder = new StringBuilder();

            while (LeadingByte.IsNonfinalChunkString || LeadingByte.IsFinalChunkString)
            {
                var chunkLength = GetChunkLength();
                var chunk = GetStringChunk(chunkLength);

                builder.Append(chunk);

                if (LeadingByte.IsFinalChunkString)
                {
                    break;
                }

                ReadLeadingByte();
            }

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual DateTime ReadDateTime()
        {
            ReadLeadingByte();

            if (LeadingByte.IsCompactDateTime)
            {
                var minutes = ReadUnpackedInt32();
                return DateTimeExtension.FromMinutes(minutes);
            }

            if (!LeadingByte.IsUnpackedDateTime)
            {
                throw new HessianSerializerException();
            }

            return DateTimeExtension.FromMilliseconds(ReadUnpackedInt64());
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginObject()
        {
            ReadLeadingByte();

            if (LeadingByte.IsClassDefinition)
            {
                preamble = ObjectPreamble.ClassDefinition;
            }
            else if (LeadingByte.IsShortObjectReference || LeadingByte.IsLongObjectReference)
            {
                preamble = ObjectPreamble.ObjectReference;
            }
            else if (LeadingByte.IsInstanceReference)
            {
                preamble = ObjectPreamble.InstanceReference;
            }
            else
            {
                throw new HessianSerializerException();
            }
        }

        public void EndObject()
        {
            preamble = ObjectPreamble.None;
        }
        
        public ObjectPreamble BeginList()
        {
            ReadLeadingByte();

            if (LeadingByte.IsVarList)
            {
                preamble = ObjectPreamble.VarList;
            }
            else if (LeadingByte.IsFixedList)
            {
                preamble = ObjectPreamble.FixList;
            }
            else if (LeadingByte.IsVarListUntyped)
            {
                preamble = ObjectPreamble.VarListUntyped;
            }
            else if (LeadingByte.IsFixListUntyped)
            {
                preamble = ObjectPreamble.FixListUntyped;
            }
            else if (LeadingByte.IsCompactFixList)
            {
                preamble = ObjectPreamble.CompactFixList;
            }
            else if (LeadingByte.IsCompactFixListUntyped)
            {
                preamble = ObjectPreamble.CompactFixListUntyped;
            }
            else
            {
                throw new HessianSerializerException();
            }

            return preamble;

        }

        public void EndList()
        {
            preamble = ObjectPreamble.None;
        }

        public void EndClassDefinition()
        {
        }

        public int ReadObjectReference()
        {
            ReadLeadingByte();

            if (LeadingByte.IsShortObjectReference)
            {
                return LeadingByte.Data - 0x60;
            }

            if (!LeadingByte.IsLongObjectReference)
            {
                throw new HessianSerializerException();
            }

            return ReadInt32();
        }

        public int ReadInstanceReference()
        {
            return ReadInt32();
        }

        public byte Peek()
        {
            byte  value= (byte)this.Stream.ReadByte();
            this.Stream.Position--;
            return value;
        }
        public byte ReadByte()
        {
            return (byte)this.Stream.ReadByte();
        }
        protected void ReadLeadingByte()
        {
            var data = Stream.ReadByte();

            Console.WriteLine(data.ToString("x2"));
            if (-1 == data)
            {
                throw new HessianSerializerException();
            }

            LeadingByte.SetData((byte)data);
        }

        private int ReadShortInt32()
        {
            var data = Stream.ReadByte();
            return ((LeadingByte.Data - 200) << 8) | data;
        }

        private int ReadCompactInt32()
        {
            var value = (LeadingByte.Data - 212) << 16;

            value |= (Stream.ReadByte() << 8);
            value |= Stream.ReadByte();

            return value;
        }

        private int ReadUnpackedInt32()
        {
            var value = Stream.ReadByte() << 24;

            value |= (Stream.ReadByte() << 16);
            value |= (Stream.ReadByte() << 8);
            value |= Stream.ReadByte();

            return value;
        }

        private long ReadShortInt64()
        {
            var data = Stream.ReadByte();
            return ((LeadingByte.Data - 248) << 8) | data;
        }

        private long ReadCompactInt64()
        {
            var value = (LeadingByte.Data - 60) << 16;

            value |= (Stream.ReadByte() << 8);
            value |= Stream.ReadByte();

            return value;
        }
        private long ReadPackedInt64()
        {
            var value = Stream.ReadByte() << 24;

            value |= (Stream.ReadByte() << 16);
            value |= (Stream.ReadByte() << 8);
            value |= Stream.ReadByte();

            return value;
        }

        private long ReadUnpackedInt64()
        {
            var value = (long) Stream.ReadByte() << 56;

            value |= ((long) Stream.ReadByte() << 48);
            value |= ((long) Stream.ReadByte() << 40);
            value |= ((long) Stream.ReadByte() << 32);
            value |= ((long) Stream.ReadByte() << 24);
            value |= ((long) Stream.ReadByte() << 16);
            value |= ((long) Stream.ReadByte() << 8);
            value |= (uint)Stream.ReadByte();

            return value;
        }

        private byte[] ReadBinaryCompact15()
        {
            var size = LeadingByte.Data - 32;
            var temp = new byte[size];

            if (Stream.Read(temp, 0, size) != size)
            {
                throw new HessianSerializerException();
            }

            return temp;
        }

        private double ReadShortDouble()
        {
            var value = Stream.ReadByte() << 8;

            value |= Stream.ReadByte();

            return Convert.ToDouble((short)value);
        }

        private double ReadCompactDouble()
        {
            var buffer = new byte[4];

            for (var index = buffer.Length - 1; index >= 0; index--)
            {
                buffer[index] = (byte)Stream.ReadByte();
            }

            return BitConverter.ToSingle(buffer, 0);
        }

        private double ReadUnpackedDouble()
        {
            var value = 0L;

            for (var count = 8; count > 0; count--)
            {
                value <<= 8;
                value |= (uint)Stream.ReadByte();
            }

            return BitConverter.Int64BitsToDouble(value);
        }

        private string ReadCompactString()
        {
            var length = (LeadingByte.Data - 0x30) << 8;

            length |= Stream.ReadByte();

            return GetStringChunk(length);
        }

        private string GetStringChunk(int length)
        {
            var buffer = new StringBuilder();

            while (length-- > 0)
            {
                var ch = ReadChar();
                buffer.Append(ch);
            }

            return buffer.ToString();
        }

        private int GetChunkLength()
        {
            var b0 = Stream.ReadByte();
            var b1 = Stream.ReadByte();
            return (b0 << 8) + b1;
        }

        private char ReadChar()
        {
            var data = Stream.ReadByte();

            if (data < 0x80)
            {
                return (char)data;
            }

            if ((data & 0xE0) == 0xC0)
            {
                var b0 = Stream.ReadByte();
                return (char) (((data & 0x1F) << 6) + (b0 & 0x3F));
            }

            if ((data & 0xF0) == 0xE0)
            {
                var b0 = Stream.ReadByte();
                var b1 = Stream.ReadByte();
                return (char) (((data & 0x0F) << 12) + ((b0 & 0x3F) << 6) + (b1 & 0x3F));
            }

            throw new HessianSerializerException();
        }

        /*
         * hessian implementation
         * https://github.com/benjamin-bader/hessian/blob/master/Hessian/ValueReader.cs
        public uint ReadUtf8Codepoint ()
        {
            const uint replacementChar = 0xFFFD;

            byte b0, b1, b2, b3;
            b0 = ReadByte ();

            if (b0 < 0x80) {
                return b0;
            }
 
            if (b0 < 0xC2) {
                return replacementChar;
            }
           
            if (b0 < 0xE0) {
                b1 = ReadByte ();

                if ((b1 ^ 0x80) >= 0x40) {
                    return replacementChar;
                }

                return (b1 & 0x3Fu) | ((b0 & 0x1Fu) << 6);
            }

            if (b0 < 0xF0) {
                b1 = ReadByte ();
                b2 = ReadByte ();

                // Valid range: E0 A0..BF 80..BF
                if (b0 == 0xE0 && (b1 ^ 0xA0) >= 0x20) {
                    return replacementChar;
                }

                // Valid range: ED 80..9F 80..BF
                if (b0 == 0xED && (b1 ^ 0x80) >= 0x20) {
                    return replacementChar;
                }

                // Valid range: E1..EC 80..BF 80..BF
                if ((b1 ^ 0x80) >= 0x40 || (b2 ^ 0x80) >= 0x40) {
                    return replacementChar;
                }

                return (b2 & 0x3Fu)
                    | ((b1 & 0x3Fu) << 6)
                    | ((b0 & 0x0Fu) << 12);
            }

            if (b0 < 0xF1) {
                b1 = ReadByte();

                if ((b1 ^ 0x90) < 0x30) {
                    return replacementChar;
                }

                b2 = ReadByte();
                b3 = ReadByte();

                if ((b2 & 0xC0) != 0x80 || (b3 & 0xC0) != 0x80) {
                    return replacementChar;
                }

                return (b3 & 0x3Fu)
                    | ((b2 & 0x3Fu) << 6)
                    | ((b1 & 0x3Fu) << 12)
                    | ((b0 & 0x07u) << 18);
            }
            
            if (b0 < 0xF4) {
                b1 = ReadByte ();
                b2 = ReadByte ();
                b3 = ReadByte ();

                // Valid range: F1..F3 80..BF 80..BF 80..BF
                if ((b1 & 0xC0) != 0x80 || (b2 & 0xC0) != 0x80 || (b3 & 0xC0) != 0x80)
                {
                    return replacementChar;
                }

                return (b3 & 0x3Fu)
                    | ((b2 & 0x3Fu) << 6)
                    | ((b1 & 0x3Fu) << 12)
                    | ((b0 & 0x07u) << 18);
            }

            if (b0 < 0xF5) {
                b1 = ReadByte ();

                // Valid range: F4 80..8F 80..BF 80..BF
                if ((b1 ^ 0x80) >= 0x10) {
                    return replacementChar;
                }

                b2 = ReadByte();
                b3 = ReadByte();

                if ((b2 & 0xC0) != 0x80 || (b3 & 0xC0) != 0x80)
                {
                    return replacementChar;
                }

                return (b3 & 0x3Fu)
                    | ((b2 & 0x3Fu) << 6)
                    | ((b1 & 0x3Fu) << 12)
                    | ((b0 & 0x07u) << 18);
            }
            
            return replacementChar;
        }*/
    }
}