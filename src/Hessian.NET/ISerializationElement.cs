using System;

namespace Hessian.Net
{
    public interface ISerializationElement
    {
        Type ObjectType
        {
            get;
        }

        void Serialize(HessianOutputWriter writer, object graph, HessianSerializationContext context);

        object Deserialize(HessianInputReader reader, HessianSerializationContext context);
    }
}