using System;
using System.Threading.Tasks;
using DotXxlJob.Core.DefaultHandlers;
using DotXxlJob.Core.Model;
using Xunit;

namespace DotXxlJob.Core.Tests
{
    public class JobHandlerCacheTest
    {
        [Fact]
        public void Repeated_Job_Handler()
        {
            var cache = new JobHandlerCache();

            cache.AddJobHandler<SimpleHttpJobHandler>();

            Assert.Throws<ArgumentException>(() => cache.AddJobHandler("simpleHttpJobHandler", new TestJobHandler()));
        }

        private class TestJobHandler : IJobHandler
        {
            public void Dispose() { }

            public Task<ReturnT> Execute(JobExecuteContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}