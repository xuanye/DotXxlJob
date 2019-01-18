namespace DotXxlJob.Core.Model
{
    public class JobExecuteContext
    {
        public JobExecuteContext(IJobLogger jobLogger,string jobParameter)
        {
            this.JobLogger = jobLogger;
            this.JobParameter = jobParameter;
        }
        public string JobParameter { get; }
        public IJobLogger JobLogger { get;  }
    }
}