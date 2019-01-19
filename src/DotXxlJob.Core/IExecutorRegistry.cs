using System.Threading;
using System.Threading.Tasks;

namespace DotXxlJob.Core
{
    public interface IExecutorRegistry
    {
        
        Task RegistryAsync(CancellationToken cancellationToken);

     
    }
}