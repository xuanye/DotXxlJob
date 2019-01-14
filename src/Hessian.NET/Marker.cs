namespace Hessian.Net
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Marker
    {
        public const byte True = (byte) 'T';//0x54;
        public const byte False = (byte) 'F';// 0x46;
        public const byte Null = (byte) 'N';//0x4E;
        public const byte BinaryNonfinalChunk = (byte) 'b';//0x41;
        public const byte BinaryFinalChunk = (byte) 'B';//0x42;
        public const byte ClassDefinition = (byte) 'C';//0x43;
        public const byte DateTimeLong = 0x4A;
        public const byte DateTimeCompact = 0x4B;
        public const byte Double = 0x5A;
        public const byte DoubleZero = 0x5B;
        public const byte DoubleOne = 0x5C;
        public const byte DoubleOctet = 0x5D;
        public const byte DoubleShort = 0x5E;
        public const byte DoubleFloat = 0x5F;
        public const byte UnpackedInteger = (byte) 'I';// 0x49;
        public const byte PackedLong = (byte) 'Y';// 0x59;
        public const byte UnpackedLong = (byte) 'L';// 0x4C;
        public const byte StringNonfinalChunk = 0x52;
        public const byte StringFinalChunk = 0x53;
        public const byte FixedLengthList = 0x56;
        public const byte ClassReference = (byte) 'O';//0x4F
        public const byte InstanceReference = (byte) 'Q'; //0x51;
    }
}