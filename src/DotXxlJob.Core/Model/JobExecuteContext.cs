using System.Threading;

namespace DotXxlJob.Core.Model
{
    public class JobExecuteContext
    {
        public JobExecuteContext(IJobLogger jobLogger, string jobParameter, CancellationToken cancellationToken)
        {
            this.JobLogger = jobLogger;
            this.JobParameter = jobParameter;
            this.cancellationToken = cancellationToken;
        }
        public string JobParameter { get; }
        public IJobLogger JobLogger { get; }
        public CancellationToken cancellationToken { get; }
    }
}