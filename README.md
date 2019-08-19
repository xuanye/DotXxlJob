# DotXxlJob
xxl-job的dotnet core 执行器实现，支持XXL-JOB 2.0+

## 1 XXL-JOB概述
[XXL-JOB][1]是一个轻量级分布式任务调度平台，其核心设计目标是开发迅速、学习简单、轻量级、易扩展。现已开放源代码并接入多家公司线上产品线，开箱即用。以下是它的架构图
![架构图](https://raw.githubusercontent.com/xuxueli/xxl-job/master/doc/images/img_Qohm.png)



## 2. 关于DotXxlJob产生
在工作中调研过多个任务调度平台，如Hangfire、基于Quatz.NET的第三方扩展，都与实际的需求有一点差距。 之前一直使用Hangfire，Hangfire的执行器在同步调用业务服务时，如果即时业务服务正在重新部署或者重启，有一定概率会出现死锁，导致CPU100%，后来全部调整为异步，但是这样就无法获得执行结果，这样的设计有蛮大问题，XxlJob的回调机制很好的解决了这个问题。本身如果通过http的方式调用，只要部署springbootd的一个执行器就可以解决问题，但是扩展性较差。所以萌生了实现DotNet版本的执行器的想法，为避免重复造轮子，开始之前也进行过调研，以下仓库[https://github.com/yuniansheng/xxl-job-dotnet][2]给了较大的启发，但是该库只支持1.9版本的xxljob，还有一些其他小问题，所以还是自力更生。

## 3. 如何使用

目前只实现了BEAN的方式，即直接实现IJobHandler调用的方式，Glue源码的方式实际上实现起来也并不复杂（有需求再说把），或者各位有需求Fork 实现一下

可参考sample

安装:

> dotnet add package DotXxlJob.Core 

### 3.1 在AspNetCore中使用

1. 声明一个AspNet的Middleware中间件,并扩展ApplicationBuilder，本质是拦截Post请求，解析Body中的流信息

```
public class XxlJobExecutorMiddleware
{
    private readonly IServiceProvider _provider;
    private readonly RequestDelegate _next;

    private readonly XxlRpcServiceHandler _rpcService;
    public XxlJobExecutorMiddleware(IServiceProvider provider, RequestDelegate next)
    {
        this._provider = provider;
        this._next = next;
        this._rpcService = _provider.GetRequiredService<XxlRpcServiceHandler>();
    }

    public async Task Invoke(HttpContext context)
    {

        if ("POST".Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase) && 
            "application/octet-stream".Equals(context.Request.ContentType, StringComparison.OrdinalIgnoreCase))
        {
            var rsp =  await _rpcService.HandlerAsync(context.Request.Body);
            context.Response.StatusCode = (int) HttpStatusCode.OK;
            context.Response.ContentType = "text/plain;utf-8";
            await context.Response.Body.WriteAsync(rsp,0,rsp.Length);
            return;
        }
        await _next.Invoke(context);
    }
}
```

扩展ApplicationBuilderExtensions,可根据实际情况绑定在特殊的Url Path上

```
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseXxlJobExecutor(this IApplicationBuilder @this)
    {
       return @this.UseMiddleware<XxlJobExecutorMiddleware>();
    }
}
```

在Startup中添加必要的引用,其中自动注册。

```
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }
    
    public void ConfigureServices(IServiceCollection services)
    {
      
        services.AddXxlJobExecutor(Configuration);
        services.AddSingleton<IJobHandler, DemoJobHandler>(); // 添加自定义的jobHandler
        services.AddAutoRegistry(); // 自动注册
    }


    public void Configure(IApplicationBuilder app,IHostingEnvironment env)
    {
        //启用XxlExecutor
        app.UseXxlJobExecutor();
    }
}
```

编写JobHandler,继承AbstractJobHandler或者直接实现接口IJobHandler，通过context.JobLogger 记录执行过程和结果，在AdminWeb上可查看的哦
```
[JobHandler("demoJobHandler")]
public class DemoJobHandler:AbstractJobHandler
{
    public override Task<ReturnT> Execute(JobExecuteContext context)
    {
        context.JobLogger.Log("receive demo job handler,parameter:{0}",context.JobParameter);

        return Task.FromResult(ReturnT.SUCCESS);
    }
}
```

## 3.2 配置信息
管理端地址和端口是必填信息，其他根据实际情况，选择配置，配置项说明见下代码中的注释

```
 public class XxlJobExecutorOptions
{
   
    /// <summary>
    /// 管理端地址，多个以;分隔
    /// </summary>
    public string AdminAddresses { get; set; }
    /// <summary>
    /// appName自动注册时要去管理端配置一致
    /// </summary>
    public string AppName { get; set; } = "xxl-job-executor-dotnet";
    /// <summary>
    /// 自动注册时提交的地址，为空会自动获取内网地址
    /// </summary>
    public string SpecialBindAddress { get; set; }
    /// <summary>
    /// 绑定端口
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// 是否自动注册
    /// </summary>
    public bool AutoRegistry { get; set; }
    /// <summary>
    /// 认证票据
    /// </summary>
    public string AccessToken { get; set; }
    /// <summary>
    /// 日志目录，默认为执行目录的logs子目录下，请配置绝对路径
    /// </summary>
    public string LogPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "./logs");
    /// <summary>
    /// 日志保留天数
    /// </summary>
    public int LogRetentionDays { get; set; } = 30;
}
```
## 在其他Http服务中使用

只需要实现Http请求的拦截，并判断post请求中content-Type="application/octet-stream",并使用XxlRpcServiceHandler来处理流 即可。

## 其他说明
XXL-JOB内置的RPC是使用Hessian协议，这个有点坑。很多都是java特有的属性和标识，比如类名什么的。在本项目中，并没有实现完整的Hessian2协议，只实现了使用到的类型，当然扩展起来也非常方便。如果有人要单独使用Hessian 这个类库的话，要特别注意这个问题。

有任何问题，可Issue反馈 ,最后感谢 xxl-job



  [1]: http://www.xuxueli.com/xxl-job
  [2]: https://github.com/yuniansheng/xxl-job-dotnet
