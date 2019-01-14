namespace DotXxlJob.Core
{
    public interface IJobHandlerFactory
    {
        //TODO: 获取实际执行的JobHandler
        IJobHandler GetJobHandler(string handlerName);
    }
}