using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Serialization;
using DotXxlJob.Core.Model;
using Hessian;

namespace DotXxlJob.Core
{
    public static class HessianSerializer
    {
        private static readonly Dictionary<string, PropertyInfo> requestProperties =
            new Dictionary<string, PropertyInfo>();
        
        private static readonly Dictionary<string, PropertyInfo> triggerProperties =
            new Dictionary<string, PropertyInfo>();
        
        private static readonly Dictionary<string, PropertyInfo> responseProperties =
            new Dictionary<string, PropertyInfo>();
        
        private static readonly Dictionary<string, PropertyInfo> returnProperties =
            new Dictionary<string, PropertyInfo>();
        static HessianSerializer()
        {
            var typeInfo = typeof(RpcRequest).GetTypeInfo();
            foreach (var property in typeInfo.DeclaredProperties)
            {
                var attribute = property.GetCustomAttribute<DataMemberAttribute>();

                if (null == attribute)
                {
                    continue;
                }

                if (!property.CanRead || !property.CanWrite)
                {
                    continue;
                }

                requestProperties.Add(attribute.Name,property);
            }
            
            var triggerTypeInfo = typeof(TriggerParam).GetTypeInfo();
            foreach (var property in triggerTypeInfo.DeclaredProperties)
            {
                var attribute = property.GetCustomAttribute<DataMemberAttribute>();

                if (null == attribute)
                {
                    continue;
                }

                if (!property.CanRead || !property.CanWrite)
                {
                    continue;
                }

                triggerProperties.Add(attribute.Name,property);
            }
            
            var rspTypeInfo = typeof(RpcResponse).GetTypeInfo();
            foreach (var property in rspTypeInfo.DeclaredProperties)
            {
                var attribute = property.GetCustomAttribute<DataMemberAttribute>();

                if (null == attribute)
                {
                    continue;
                }

                if (!property.CanRead || !property.CanWrite)
                {
                    continue;
                }

                responseProperties.Add(attribute.Name,property);
            }
            
            var retTypeInfo = typeof(ReturnT).GetTypeInfo();
            foreach (var property in retTypeInfo.DeclaredProperties)
            {
                var attribute = property.GetCustomAttribute<DataMemberAttribute>();

                if (null == attribute)
                {
                    continue;
                }

                if (!property.CanRead || !property.CanWrite)
                {
                    continue;
                }

                returnProperties.Add(attribute.Name,property);
            }
        }
        
        public static RpcRequest DeserializeRequest(Stream stream)
        {
            RpcRequest request = new RpcRequest();

            try
            {
                var deserializer = new Deserializer(stream);
                var classDef = deserializer.ReadValue() as ClassDef; 
                if (!Constants.RpcRequestJavaFullName.Equals(classDef.Name))
                {
                    throw  new HessianException($"unknown class :{classDef.Name}");
                }
                if (requestProperties.Count != classDef.Fields.Length)
                {
                    throw  new HessianException($"unknown class :{classDef.Name}, field count not match ${requestProperties.Count} !={classDef.Fields.Length}");
                }
           
                //obj serialize
                if (deserializer.ReadValue() is HessianObject hessianObject)
                {
                    foreach (var item in hessianObject)
                    {
                        if (requestProperties.TryGetValue(item.Item1, out var p))
                        {
                            if (IsSimpleType(p.PropertyType.GetTypeInfo()))
                            {
                                p.SetValue(request,item.Item2);
                            }
                            else
                            {
                                if (item.Item1 == "parameterTypes")
                                {
                                    request.ParameterTypes = item.Item2 as List<object>;
                                }
                                else if (item.Item2 is HessianObject ) 
                                {
                                    request.Parameters = new List<object>();
                                    
                                    var defList = deserializer.ReadValue() as List<object>;

                                    foreach (var li in defList)
                                    {
                                        ReadParameters(deserializer,request.Parameters, li);
                                    }
                                }
                                else
                                {
                                    throw  new HessianException($"unknown item :{item.Item1},{item.Item2.GetType()}");
                                }
                            }
                        }
                        
                    }
                }
             
                
            }
            catch (EndOfStreamException)
            {
                //没有数据可读了
            }

            return request;

        }

        private static void ReadParameters(Deserializer deserializer,IList<object> list, object item)
        {
            var itemType = item.GetType();
            if (IsSimpleType(itemType.GetTypeInfo()))
            {
                list.Add(item);
                return;
            }

            if (itemType == typeof(ClassDef))
            {
                var triggerClass = item as ClassDef;
                //TODO:这里要做成动态的话 ，可以注册所有的实体到对应的字典中，不过这里只有这个类型哦
                if (triggerClass.Name != "com.xxl.job.core.biz.model.TriggerParam")
                {
                    throw new HessianException($"not expected parameter type [{triggerClass.Name}]");
                }

                if (!(deserializer.ReadValue() is HessianObject triggerData))
                {
                    throw new HessianException("not expected parameter type ,data is null");
                }
                TriggerParam param = new TriggerParam();
                foreach (var field in triggerData)
                {
                    if (triggerProperties.TryGetValue(field.Item1, out var tgPropertyInfo))
                    {
                        tgPropertyInfo.SetValue(param,field.Item2);
                    }
                }
            }
            else
            {
                throw new HessianException($"unsupported list item type =[{itemType}]");
            }
            
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
        
        private static bool IsSimpleType(TypeInfo typeInfo)
        {
            if (typeInfo.IsValueType || typeInfo.IsEnum || typeInfo.IsPrimitive)
            {
                return true;
            }

            if (typeof (string) == typeInfo.AsType())
            {
                return true;
            }

            return false;
        }

        public static RpcResponse DeserializeResponse(Stream resStream)
        {
            var rsp = new RpcResponse();

            try
            {
                var deserializer = new Deserializer(resStream);
                var classDef = deserializer.ReadValue() as ClassDef; 
                if (!Constants.RpcResponseJavaFullName.Equals(classDef.Name))
                {
                    throw  new HessianException($"unknown class :{classDef.Name}");
                }
                if (responseProperties.Count != classDef.Fields.Length)
                {
                    throw  new HessianException($"unknown class :{classDef.Name}, field count not match ${responseProperties.Count} !={classDef.Fields.Length}");
                }
           
                //obj serialize
                if (deserializer.ReadValue() is HessianObject hessianObject)
                {
                    foreach (var item in hessianObject)
                    {
                        if (responseProperties.TryGetValue(item.Item1, out var p))
                        {
                            if (IsSimpleType(p.PropertyType.GetTypeInfo()))
                            {
                                p.SetValue(rsp,item.Item2);
                            }
                            else
                            {
                                if (item.Item2 is ClassDef resultClassDef )
                                {
                                  
                                    //TODO:这里要做成动态的话 ，可以注册所有的实体到对应的字典中，不过这里只有这个类型哦
                                    if (resultClassDef.Name != "com.xxl.job.core.biz.model.ReturnT")
                                    {
                                        throw new HessianException($"not expected parameter type [{resultClassDef.Name}]");
                                    }

                                    if (!(deserializer.ReadValue() is HessianObject resultData))
                                    {
                                        throw new HessianException("not expected parameter type ,data is null");
                                    }
                                    ReturnT data = new ReturnT();
                                    foreach (var field in resultData)
                                    {
                                        if (returnProperties.TryGetValue(field.Item1, out var tgPropertyInfo))
                                        {
                                            tgPropertyInfo.SetValue(data,field.Item2);
                                        }
                                    }

                                    rsp.Result = data;
                                }
                                else
                                {
                                    throw  new HessianException($"unknown item :{item.Item1},{item.Item2.GetType()}");
                                }
                            }
                        }
                        
                    }
                }
             
                
            }
            catch (EndOfStreamException)
            {
                //没有数据可读了
            }

            return rsp;
        }
    }

  
}