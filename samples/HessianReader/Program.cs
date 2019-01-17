using System;
using System.Collections.Generic;
using System.IO;
using DotXxlJob.Core;
using DotXxlJob.Core.Model;
using Hessian;
using Newtonsoft.Json;

namespace HessianReader
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] myBinary = File.ReadAllBytes("log.dat");

            foreach (var i in myBinary)
            {
                Console.Write("0x");
                Console.Write(i.ToString("x2"));
                Console.Write(",");
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("---------------------{0}------------------------------------------",myBinary.Length);

          
            
            using (var stream1 = new MemoryStream(myBinary))
            {
                var s1 = HessianSerializer.DeserializeRequest(stream1);
                Console.WriteLine(JsonConvert.SerializeObject(s1));
            }
            
            Console.WriteLine("------------------------------------------------------------");
          

            RpcResponse response = new RpcResponse {
                RequestId = Guid.NewGuid().ToString("N"), Result = ReturnT.Failed("ABCDEFG")
            };
            using (var stream2 = new MemoryStream())
            {
                HessianSerializer.SerializeResponse(stream2,response);

                stream2.Position = 0;

                var s2 =HessianSerializer.DeserializeResponse(stream2);
                Console.WriteLine(JsonConvert.SerializeObject(s2));
            }
            Console.WriteLine("------------------------------------------------------------");
            Console.ReadKey();
            /**
             *
             * Console.WriteLine("---------------------------------------------------------------");
            RpcRequest req = new RpcRequest {
                RequestId = "71565f61-94e8-4dcf-9760-f2fb73a6886a",
                CreateMillisTime = 1547621183585,
                AccessToken = "",
                ClassName = "com.xxl.job.core.biz.ExecutorBiz",
                MethodName = "run",
                ParameterTypes = new List<object> {new JavaClass{ Name = "com.xxl.job.core.biz.model.TriggerParam"}},
                Version = null,
                Parameters = new List<object>()
            };

            var p =new TriggerParam {
                JobId=1,
                ExecutorHandler="demoJobHandler",
                ExecutorParams="111",
                ExecutorBlockStrategy="SERIAL_EXECUTION",
                ExecutorTimeout=0,
                LogId=5,
                LogDateTime=1547621183414L,
                GlueType="BEAN",
                GlueSource="",
                GlueUpdateTime=1541254891000,
                BroadcastIndex=0,
                BroadcastTotal=1
            };
            req.Parameters.Add(p);
            
            using (var stream2 = new MemoryStream())
            {
                var serializer = new Serializer(stream2);
                serializer.WriteObject(req);
                Console.WriteLine("-----------------------------序列化成功---{0}-------------------------------",stream2.Length);
                stream2.Position = 0;
                
                var s2 = HessianSerializer.DeserializeRequest(stream2);
                Console.WriteLine(JsonConvert.SerializeObject(s2));
            }
             * [{"Item1":"requestId","Item2":"71565f61-94e8-4dcf-9760-f2fb73a6886a"},{"Item1":"createMillisTime","Item2":1432957289},{"Item1":"accessToken","Item2":""},{"Item1":"className","Item2":"com.xxl.job.core.biz.ExecutorBiz"},{"Item1":"methodName","Item2":"run"},{"Item1":"version","Item2":null},{"Item1":"parameterT
ypes","Item2":[{"Name":"java.lang.Class","Fields":["name"]}]},{"Item1":"parameters","Item2":[{"Item1":"name","Item2":"com.xxl.job.core.biz.model.TriggerParam"}]}]
System.Collections.Generic.List`1[System.Object]
[{"Name":"com.xxl.job.core.biz.model.TriggerParam","Fields":["jobId","executorHandler","executorParams","executorBlockStrategy","executorTimeout","logId","logDateTim","glueType","glueSource","glueUpdatetime","broadcastIndex","broadcastTotal"]}]
                     Hessian.HessianObject
[{"Item1":"jobId","Item2":1},{"Item1":"executorHandler","Item2":"demoJobHandler"},{"Item1":"executorParams","Item2":"111"},{"Item1":"executorBlockStrategy","Item2":"SERIAL_EXECUTION"},{"Item1":"executorTimeout","Item2":0},{"Item1":"logId","Item2":5},{"Item1":"logDateTim","Item2":1432956926},{"Item1":"glueTy
pe","Item2":"BEAN"},{"Item1":"glueSource","Item2":""},{"Item1":"glueUpdatetime","Item2":-638368258},{"Item1":"broadcastIndex","Item2":0},{"Item1":"broadcastTotal","Item2":1}]

             * requestId='71565f61-94e8-4dcf-9760-f2fb73a6886a',
             * createMillisTime=1547621183585, 
             * accessToken='',
             * className='com.xxl.job.core.biz.ExecutorBiz',
             * methodName='run',
             * parameterTypes=[class com.xxl.job.core.biz.model.TriggerParam],
             * parameters=[
             * TriggerParam{
             * jobId=1,
             * executorHandler='demoJobHandler',
             * executorParams='111',
             * executorBlockStrategy='SERIAL_EXECUTION',
             * executorTimeout=0,
             * logId=5,
             * logDateTim=1547621183414,
             * glueType='BEAN',
             * glueSource='',
             * glueUpdatetime=1541254891000,
             * broadcastIndex=0,
             * broadcastTotal=1
             * }
             * ], version='null'
             * 
             */
        }
    }
}