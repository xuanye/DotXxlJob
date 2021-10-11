using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotXxlJob.Core.DefaultHandlers;
using DotXxlJob.Core.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotXxlJob.Core.Tests
{
    public class BeanTaskExecutorTest
    {
        [Fact]
        public async Task Repeated_Job_Handler()
        {
            var services = new ServiceCollection();

            services.AddScoped<ScopedService>();

            services.AddXxlJobExecutor(options => options.AdminAddresses = "http://localhost");

            var list = new List<object>();

            services.AddJobHandler<TestJobHandler>("test", list);

            using (var provider = services.BuildServiceProvider(true))
            {
                await provider.GetRequiredService<ITaskExecutor>()
                    .Execute(new TriggerParam {
                        ExecutorHandler = "test"
                    });
            }

            Assert.Single(list);
        }

        private class TestJobHandler : IJobHandler
        {
            private readonly List<object> _list;

            public TestJobHandler(List<object> list, ScopedService _) => _list = list;

            public void Dispose()
            {
            }

            public Task<ReturnT> Execute(JobExecuteContext context)
            {
                _list.Add(new object());

                return Task.FromResult(ReturnT.SUCCESS);
            }
        }

        private class ScopedService
        {
        }
    }
}
