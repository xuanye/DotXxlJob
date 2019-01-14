namespace Hessian.Net.Specification
{
    /// <summary>
    /// 
    /// </summary>
    public static class SpecificationExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static ISpecification<TParam> And<TParam>(this ISpecification<TParam> left, ISpecification<TParam> right)
        {
            return new AndSpecification<TParam>(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static ISpecification<TParam> Or<TParam>(this ISpecification<TParam> left, ISpecification<TParam> right)
        {
            return new OrSpecification<TParam>(left, right);
        }
    }
}