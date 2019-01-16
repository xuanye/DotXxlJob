using Microsoft.AspNetCore.Builder;

namespace ASPNetCoreExecutor
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseXxlJobExecutor(this IApplicationBuilder @this)
        {
           return @this.UseMiddleware<XxlJobExecutorMiddleware>();
        }
    }
}