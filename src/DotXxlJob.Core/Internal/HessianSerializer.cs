using System;
using System.IO;
using DotXxlJob.Core.Model;
using Hessian;

namespace DotXxlJob.Core
{
    public static class HessianSerializer
    {
     
        public static RpcRequest DeserializeRequest(Stream stream)
        {
            RpcRequest request = null;

            try
            {
                var deserializer = new Deserializer(stream);
                var classDef = deserializer.ReadValue() as ClassDef; 
                if (!Constants.RpcRequestJavaFullName.Equals(classDef.Name))
                {
                    throw  new HessianException($"unknown class :{classDef.Name}");
                }
                request = HessianObjectHelper.GetRealObjectValue(deserializer,deserializer.ReadValue()) as RpcRequest;
            }
            catch (EndOfStreamException)
            {
                //没有数据可读了
            }
            return request;

        }
     
        
        public static void SerializeRequest(Stream stream,RpcRequest req)
        {
            var serializer = new Serializer(stream);
            serializer.WriteObject(req);
        }
        
        public static void SerializeResponse(Stream stream,RpcResponse res)
        {
            var serializer = new Serializer(stream);
            serializer.WriteObject(res);
        }
        
  
        public static RpcResponse DeserializeResponse(Stream resStream)
        {
            RpcResponse rsp = null;

            try
            {
                var deserializer = new Deserializer(resStream);
                var classDef = deserializer.ReadValue() as ClassDef;
                if (!Constants.RpcResponseJavaFullName.Equals(classDef.Name))
                {
                    throw new HessianException($"unknown class :{classDef.Name}");
                }

                rsp = HessianObjectHelper.GetRealObjectValue(deserializer,deserializer.ReadValue()) as RpcResponse;

            }
            catch (EndOfStreamException)
            {
                //没有数据可读了
            }
            catch (Exception)
            {
                //TODO: do something?
            }

            return rsp;
        }
        
      
    }

  
}