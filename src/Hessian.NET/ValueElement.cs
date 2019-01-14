using System;

namespace Hessian.Net
{
    public class ValueElement : ISerializationElement
    {
        public Type ObjectType
        {
            get;
        }

        public IObjectSerializer ObjectSerializer
        {
            get;
        }

        public ValueElement(Type objectType, IObjectSerializer objectSerializer)
        {
            ObjectType = objectType;
            ObjectSerializer = objectSerializer;
        }

        public void Serialize(HessianOutputWriter writer, object graph, HessianSerializationContext context)
        {
            ObjectSerializer.Serialize(writer, graph);
        }

        public object Deserialize(HessianInputReader reader, HessianSerializationContext context)
        {
            return ObjectSerializer.Deserialize(reader);
        }
    }
}