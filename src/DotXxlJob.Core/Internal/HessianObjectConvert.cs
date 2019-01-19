using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using DotXxlJob.Core.Model;
using Hessian;

namespace DotXxlJob.Core
{
    public class HessianObjectHelper
    {
        
        private static readonly Dictionary<string,Dictionary<string,PropertyInfo>> TransferObjCache 
            = new Dictionary<string, Dictionary<string, PropertyInfo>>();
        private static readonly Dictionary<string,Type> TransferTypeCache 
            = new Dictionary<string, Type>();
        static HessianObjectHelper()
        {
        
            InitProperties(typeof(RpcRequest));
            
            InitProperties(typeof(TriggerParam));
            
            InitProperties(typeof(RpcResponse));

            InitProperties(typeof(ReturnT));
            
            InitProperties(typeof(HandleCallbackParam));
            
            InitProperties(typeof(JavaClass));
            
            InitProperties(typeof(RegistryParam));
            
            InitProperties(typeof(LogResult));
        }
        private static void InitProperties(Type type)
        {
            var propertyInfos = new Dictionary<string, PropertyInfo>();
            var typeInfo = type.GetTypeInfo();
            var classAttr = type.GetCustomAttribute<DataContractAttribute>();
            if (classAttr == null)
            {
                return;
            }
            
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

                propertyInfos.Add(attribute.Name,property);
            }
            TransferTypeCache.Add(classAttr.Name,type);
            TransferObjCache.Add(classAttr.Name,propertyInfos);
        }

        public static object GetRealObjectValue(Deserializer deserializer,object value)
        {
            if (value == null || IsSimpleType(value.GetType()))
            {
                return value;
            }

            if (value is HessianObject hessianObject)
            {
                if(TransferObjCache.TryGetValue(hessianObject.TypeName,out var properties))
                {
                    var instance = Activator.CreateInstance(TransferTypeCache[hessianObject.TypeName]);
                    foreach (var (k, v) in hessianObject)
                    {
                        if (properties.TryGetValue(k, out var p))
                        {
                            p.SetValue(instance,GetRealObjectValue(deserializer,v));
                        }
                    }

                    return instance;
                }
            }

            if (value is ClassDef)
            {
                return GetRealObjectValue(deserializer, deserializer.ReadValue());
            }
            
            if (IsListType(value.GetType()))
            {
                var listData = new List<object>();
                
                var cList = value as List<object>;
                foreach (var cItem in cList)
                {
                   listData.Add(GetRealObjectValue(deserializer,cItem));
                }

                return listData;
            }
           
            throw new HessianException($"unknown item:{value.GetType()}");
        }
        
        private static bool IsListType(Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type);          
        }
        private static bool IsSimpleType(Type typeInfo)
        {
            if (typeInfo.IsValueType || typeInfo.IsEnum || typeInfo.IsPrimitive)
            {
                return true;
            }

            if (typeof (string) == typeInfo)
            {
                return true;
            }

            return false;
        }
        
    }
}