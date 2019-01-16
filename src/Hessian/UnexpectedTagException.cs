namespace Hessian
{
    public class UnexpectedTagException : HessianException
    {
        public byte Tag { get; private set; }

        public UnexpectedTagException(byte tag, string expectedType)
            : base(FormatErrorMessage(tag, expectedType))
        {
            Tag = tag;
        }

        private static string FormatErrorMessage(byte tag, string expectedType)
        {
            return string.Format("{0:X} is not a valid {1} tag.", tag, expectedType);
        }
    }
}
