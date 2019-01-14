namespace Hessian.Net.Specification
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public interface ISpecification<in TParam>
    {
        /// <summary>
        /// Checks if current specification can be satisfied by given <paramref name="arg" />
        /// </summary>
        /// <param name="arg">The value to check specification</param>
        /// <returns>Flags if specification is satisfied successfuly</returns>
        bool IsSatisfied(TParam arg);
    }
}