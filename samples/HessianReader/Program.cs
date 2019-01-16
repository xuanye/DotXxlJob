using System;
using System.Collections;
using System.IO;
using DotXxlJob.Core.Model;
using Hessian.Net;
using Newtonsoft.Json;

namespace HessianReader
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] myBinary = File.ReadAllBytes("1547621183.dat");

            foreach (var i in myBinary)
            {
                Console.Write("0x");
                Console.Write(i.ToString("x2"));
                Console.Write(",");
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("---------------------------------------------------------------");

            
            
            
            var serializer = new DataContractHessianSerializer(typeof (RpcRequest));

            using (var stream1 = new MemoryStream(myBinary))
            {
                var ds = new Hessian.Deserializer(stream1);

                Hessian.ClassDef def = ds.ReadClassDefinition();
                Console.WriteLine(JsonConvert.SerializeObject(def));
                Console.WriteLine(ds.ReadValue());
                //Console.WriteLine(ds.ReadLong());
                //Console.WriteLine(ds.ReadString());
                //Console.WriteLine(ds.ReadString());
                //Console.WriteLine(ds.ReadString());
                //Console.WriteLine(ds.ReadString());
                //Console.WriteLine(ds.ReadValue());
                //Console.WriteLine(ds.ReadValue());
                Console.WriteLine(JsonConvert.SerializeObject(def));
            }

            return;
            
            RpcRequest req = new RpcRequest {
                RequestId = "71565f61-94e8-4dcf-9760-f2fb73a6886a",
                CreateMillisTime = 1547621183585,
                AccessToken = "",
                ClassName = "com.xxl.job.core.biz.ExecutorBiz",
                MethodName = "run",
                ParameterTypes = new HessianArrayList {"class com.xxl.job.core.biz.model.TriggerParam"},
                Version = "null",
                Parameters = new HessianArrayList()
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

            byte[] distArray;
            
            using (MemoryStream stream = new MemoryStream())
            {   
              
                
                serializer.WriteObject(stream,req);
                //Console.WriteLine(Environment.NewLine);
                //Console.WriteLine("---------------------------"+  stream.Length+"------------------------------------");
                stream.Flush();
                distArray =stream.ToArray();
            }
            foreach (var j in distArray)
            {
                Console.Write("0x");
                Console.Write(j.ToString("x2"));
                Console.Write(",");
            }
            
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("---------------------------------------------------------------");
            
            using (var stream2 = new MemoryStream(distArray))
            {
             
                var instance = serializer.ReadObject(stream2) as RpcRequest;

                Console.WriteLine(JsonConvert.SerializeObject(instance));
            }
            /**
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