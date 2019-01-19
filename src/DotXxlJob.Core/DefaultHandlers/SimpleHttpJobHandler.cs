using System;
using System.Net;
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
        public override async Task<ReturnT> Execute(JobExecuteContext context)
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
            context.JobLogger.Log("Get Request Data:{0}",context.JobParameter);
            using (var client = this._httpClientFactory.CreateClient(Constants.DefaultHttpClientName))
            {
                try
                {
                    var response =  await client.GetAsync(url);
                    if (response == null)
                    {
                        context.JobLogger.Log("call remote error,response is null");
                        return ReturnT.Failed("call remote error,response is null");
                    }

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        context.JobLogger.Log("call remote error,response statusCode ={0}",response.StatusCode);
                        return ReturnT.Failed("call remote error,response statusCode ="+response.StatusCode);
                    }

                    string body = await response.Content.ReadAsStringAsync();
                    context.JobLogger.Log("<br/> call remote success ,response is : <br/> {0}",body);
                    return ReturnT.SUCCESS;
                }
                catch (Exception ex)
                {
                    context.JobLogger.LogError(ex);
                    return ReturnT.Failed(ex.Message);
                }
              
            }
        }
    }
    
}