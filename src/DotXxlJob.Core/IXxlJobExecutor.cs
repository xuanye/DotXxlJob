using DotXxlJob.Core.Model;

namespace DotXxlJob.Core
{
    public interface IXxlJobExecutor
    {
        ReturnT Beat();


        ReturnT IdleBeat(int jobId);


        ReturnT Kill(int jobId);

        ReturnT Log(long logDateTim, int logId, int fromLineNum);


        ReturnT Run(TriggerParam triggerParam);
    }
}