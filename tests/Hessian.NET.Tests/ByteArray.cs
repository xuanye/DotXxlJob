namespace Hessian.NET.Tests
{
    internal static class ByteArray
    {
        public static bool Equals(byte[] expect, byte[] value)
        {
            if (null == expect)
            {
                return null == value;
            }

            if (null == value)
            {
                return false;
            }

            if (expect.Length != value.Length)
            {
                return false;
            }

            for (var index = 0; index < expect.Length; index++)
            {
                if (!expect[index].Equals(value[index]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
