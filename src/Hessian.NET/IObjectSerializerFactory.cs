using System;

namespace Hessian.Net
{
    public interface IObjectSerializerFactory
    {
        IObjectSerializer GetSerializer(Type target);
    }
}