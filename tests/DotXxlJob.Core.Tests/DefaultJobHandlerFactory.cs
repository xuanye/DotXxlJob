using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotXxlJob.Core.Tests
{
    public class DefaultJobHandlerFactory
    {
        [Fact]
        public async Task Repeated_Job_Handler()
        {
            var services = new ServiceCollection();

            services.AddXxlJobExecutor(options => options.AdminAddresses = "http://localhost");

            services.AddDefaultXxlJobHandlers();

            services.AddJobHandler<TestJobHandler>();

            using (var provider = services.BuildServiceProvider())
            {
                Assert.Throws<ArgumentException>(() => provider.GetRequiredService<IJobHandlerFactory>());
            }
        }

        [JobHandler("simpleHttpJobHandler")]
        private class TestJobHandler : IJobHandler
        {
            public void Dispose()
            {
            }

            public Task<ReturnT> Execute(JobExecuteContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
