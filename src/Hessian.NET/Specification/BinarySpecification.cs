namespace Hessian.Net.Specification
{
    public abstract class BinarySpecification<TParam> : ISpecification<TParam>
    {
        public ISpecification<TParam> Left
        {
            get;
            protected set;
        }

        public ISpecification<TParam> Right
        {
            get;
            protected set;
        }

        protected BinarySpecification(ISpecification<TParam> left, ISpecification<TParam> right)
        {
            Left = left;
            Right = right;
        }

        public abstract bool IsSatisfied(TParam arg);
    }
}