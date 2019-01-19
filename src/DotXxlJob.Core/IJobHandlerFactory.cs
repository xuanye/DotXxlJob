namespace DotXxlJob.Core
{
    public interface IJobHandlerFactory
    {
        IJobHandler GetJobHandler(string handlerName);
    }
}