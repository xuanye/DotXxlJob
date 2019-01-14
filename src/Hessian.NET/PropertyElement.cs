using System;
using System.Reflection;
using System.Runtime.Serialization;
using Hessian.Net.Extension;

namespace Hessian.Net
{
    public class PropertyElement : ISerializationElement
    {
        private string propertyname;
        private int? propertyOrder;

        public Type ObjectType => Property.PropertyType;

        public PropertyInfo Property
        {
            get;
        }

        public ISerializationElement Element
        {
            get;
        }

        public int PropertyOrder
        {
            get
            {
                if (!propertyOrder.HasValue)
                {
                    var attribute = Property.GetCustomAttribute<DataMemberAttribute>();
                    propertyOrder = null == attribute ? 0 : attribute.Order;
                }

                return propertyOrder.Value;
            }
        }

        public string PropertyName
        {
            get
            {
                if (String.IsNullOrEmpty(propertyname))
                {
                    propertyname = Property
                        .GetCustomAttribute<DataMemberAttribute>()
                        .Unless(attribute => String.IsNullOrEmpty(attribute.Name))
                        .Return(attribute => attribute.Name, Property.Name);
                }

                return propertyname;
            }
        }

        public PropertyElement(PropertyInfo property, ISerializationElement element)
        {
            Property = property;
            Element = element;
        }

        public void Serialize(HessianOutputWriter writer, object graph, HessianSerializationContext context)
        {
            Element.Serialize(writer, graph, context);
        }

        public object Deserialize(HessianInputReader reader, HessianSerializationContext context)
        {
            return Element.Deserialize(reader, context);
        }
    }
}