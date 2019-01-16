using System;

namespace Hessian
{
    public class HessianException : ApplicationException
    {
        public HessianException()
        {
            
        }

        public HessianException(string message)
            : base(message)
        {
            
        }

        public HessianException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
    }
}
