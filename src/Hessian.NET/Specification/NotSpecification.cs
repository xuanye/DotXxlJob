namespace Hessian.Net.Specification
{
    public class NotSpecification<TParam> : ISpecification<TParam>
    {
        private readonly ISpecification<TParam> specification;

        public NotSpecification(ISpecification<TParam> specification)
        {
            this.specification = specification;
        }

        public bool IsSatisfied(TParam arg)
        {
            return !specification.IsSatisfied(arg);
        }
    }
}