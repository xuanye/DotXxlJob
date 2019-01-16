using System;

namespace Hessian
{
    public class InvalidRefException : HessianException
    {
        public int RefId { get; private set; }

        public InvalidRefException(int refId)
            : base(String.Format("Invalid ref ID: {0}", refId))
        {
            RefId = refId;
        }
    }
}
