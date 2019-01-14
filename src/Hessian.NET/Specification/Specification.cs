namespace Hessian.Net.Specification
{
    /// <summary>
    /// 
    /// </summary>
    public static class Specification
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="specification"></param>
        /// <returns></returns>
        public static ISpecification<TParam> Not<TParam>(ISpecification<TParam> specification)
        {
            return new NotSpecification<TParam>(specification);
        }
    }
}