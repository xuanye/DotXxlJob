using System.Threading.Tasks;
using DotXxlJob.Core.Model;

namespace DotXxlJob.Core
{
    public interface ITaskExecutor
    {
        string GlueType { get; }

        Task<ReturnT> Execute(TriggerParam triggerParam);
    }
}