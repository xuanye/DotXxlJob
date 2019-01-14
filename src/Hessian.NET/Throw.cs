using System;

namespace Hessian.Net
{
    internal static class Throw
    {
        public static void NotNull(object arg, string argname)
        {
            if (null == arg)
            {
                throw new ArgumentNullException(argname);
            }
        }

        public static void Validate(Func<bool> validator, string argname)
        {
            if (!validator())
            {
                throw new ArgumentException("", argname);
            }
        }
    }
}