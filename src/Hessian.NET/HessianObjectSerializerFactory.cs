using System;
using System.Collections.Generic;

namespace Hessian.Net
{
    internal sealed partial class HessianObjectSerializerFactory : IObjectSerializerFactory
    {
        //private readonly HessianSerializationContext context;
        private Dictionary<Type, IObjectSerializer> cache;

        //public HessianObjectSerializerFactory(HessianSerializationContext context)
        //{
        //    this.context = context;
        //}

        public IObjectSerializer GetSerializer(Type target)
        {
            IObjectSerializer writer;

            EnsureCache();

            return cache.TryGetValue(target, out writer) ? writer : null;
        }

        private void EnsureCache()
        {
            if (null != cache)
            {
                return;
            }

            var dict = new Dictionary<Type, IObjectSerializer>
            {
                [typeof (bool)] = new BooleanSerializer(),
                [typeof (int)] = new Int32Serializer(),
                [typeof (long)] = new Int64Serializer(),
                [typeof (double)] = new DoubleSerializer(),
                [typeof (string)] = new StringSerializer(),
                [typeof (DateTime)] = new DateTimeSerializer()
            };
            
            cache = dict;
        }
    }
}