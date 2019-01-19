using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotXxlJob.Core.Model;

namespace DotXxlJob.Core.DefaultHandlers
{
    [JobHandler("simpleHttpJobHandler")]
    public class SimpleHttpJobHandler:AbsJobHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly Regex UrlPattern =
            new Regex(@"(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]");
        public SimpleHttpJobHandler(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }
        public async override Task<ReturnT> Execute(JobExecuteContext context)
        {
            if (string.IsNullOrEmpty(context.JobParameter))
            {
                return ReturnT.Failed("url is empty");
            }

            string url = context.JobParameter;

            if (!UrlPattern.IsMatch(url))
            {
                return ReturnT.Failed("url format is not valid");
            }
            
            using (var client = _httpClientFactory.CreateClient(Constants.DefaultHttpClientName))
            {
               var responseMessage =  await client.GetAsync(url);
            }
            
            //判断是否为单URL
            context.JobLogger.Log("Get Request Data:{0}",context.JobParameter);
            return ReturnT.SUCCESS;
        }
    }
    
}