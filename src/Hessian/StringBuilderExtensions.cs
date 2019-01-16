using System;
using System.IO;
using System.Text;

namespace Hessian
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendCodepoint(this StringBuilder sb, uint codepoint)
        {
            if (codepoint < 0x10000) {
                return sb.Append((char)codepoint);
            }
            
            var n = codepoint - 0x10000;
            var high = (char)((n >> 10) + 0xD800);
            var low  = (char)((n & 0x3FF) + 0xDC00);

            AssertValidSurrogates(high, low);
            
            return sb
                .Append (high)
                .Append (low);
        }
        
        [System.Diagnostics.Conditional("DEBUG")]
        private static void AssertValidSurrogates (char high, char low)
        {
            if (!Char.IsHighSurrogate (high)) {
                throw new InvalidDataException ("Invalid high surrogate");
            }
            
            if (!Char.IsLowSurrogate (low)) {
                throw new InvalidDataException ("Invalid low surrogate");
            }
        }
    }
}

