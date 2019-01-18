using DotXxlJob.Core.DefaultHandlers;

namespace DotXxlJob.Core
{
    public class DefaultJobHandlerFactory:IJobHandlerFactory
    {
        public IJobHandler GetJobHandler(string handlerName)
        {
            return new HttpJobHandler();
        }
    }
}