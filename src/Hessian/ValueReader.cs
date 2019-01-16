using System;
using System.IO;

namespace Hessian
{
    public class ValueReader
    {
        private byte[] buffer = new byte[8];
        private PeekStream stream;

        public ValueReader (Stream stream)
        {
            this.stream = stream as PeekStream ?? new PeekStream(stream);
        }

        public byte? Peek ()
        {
            return stream.Peek ();
        }

        public short ReadShort ()
        {
            Read (buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        public int ReadInt()
        {
            Read (buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

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
        }

        public byte ReadByte()
        {
            var b = stream.ReadByte();
            if (b == -1) throw new EndOfStreamException();
            return (byte)b;
        }

        public void Read(byte[] buffer, int count)
        {
            Read (buffer, 0, count);
        }

        private void Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = stream.Read (buffer, offset, count);
            if (bytesRead != count) throw new EndOfStreamException();
        }
    }
}

