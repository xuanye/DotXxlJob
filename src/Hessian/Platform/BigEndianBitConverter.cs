namespace Hessian.Platform
{
    public class BigEndianBitConverter : EndianBitConverter
    {
        protected override long FromBytes(byte[] bytes, int offset, int count)
        {
            var result = 0L;
            for (var i = 0; i < count; ++i)
            {
                result = (result << 8) | bytes[offset + i];
            }
            return result;
        }

        protected override void CopyBytes(long source, byte[] buffer, int offset, int count)
        {
            var end = offset + count - 1;
            for (var i = 0; i < count; ++i)
            {
                buffer[end - i] = (byte)(source & 0xFF);
                source >>= 8;
            }
        }
    }
}
