namespace Hessian.Net.Specification
{
    internal class AndSpecification<TParam> : BinarySpecification<TParam>
    {
        public AndSpecification(ISpecification<TParam> left, ISpecification<TParam> right) 
            : base(left, right)
        {
        }

        public override bool IsSatisfied(TParam arg)
        {
            return Left.IsSatisfied(arg) && Right.IsSatisfied(arg);
        }
    }
}