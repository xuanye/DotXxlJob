using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Hessian.Net.Extension;

namespace Hessian.Net
{
    public class ObjectElement : ISerializationElement
    {
        private string classname;

        public Type ObjectType
        {
            get;
        }

        public string ClassName
        {
            get
            {
                if (String.IsNullOrEmpty(classname))
                {
                    classname = ObjectType
                        .GetTypeInfo()
                        .GetCustomAttribute<DataContractAttribute>()
                        .Unless(attribute => String.IsNullOrEmpty(attribute.Name))
                        .Return(attribute => attribute.Name, ObjectType.FullName);
                }

                return classname;
            }
        }

        public IList<PropertyElement> ObjectProperties
        {
            get;
        }

        public ObjectElement(Type objectType, IList<PropertyElement> objectProperties)
        {
            ObjectType = objectType;
            ObjectProperties = objectProperties;
        }

        public void Serialize(HessianOutputWriter writer, object graph, HessianSerializationContext context)
        {
            var index = context.Instances.IndexOf(graph);

            if (index > -1)
            {
                writer.WriteInstanceReference(index);
                return;
            }

            context.Instances.Add(graph);

            index = context.Classes.IndexOf(ObjectType);

            if (index < 0)
            {
                writer.BeginClassDefinition();
                writer.WriteString(ClassName);
                writer.WriteInt32(ObjectProperties.Count);

                foreach (var property in ObjectProperties)
                {
                    writer.WriteString(property.PropertyName);
                }
                
                writer.EndClassDefinition();

                index = context.Classes.Count;

                context.Classes.Add(ObjectType);
            }

            writer.WriteObjectReference(index);

            foreach (var item in ObjectProperties)
            {
                var value = item.Property.GetValue(graph);
                item.Serialize(writer, value, context);
            }
        }

        public object Deserialize(HessianInputReader reader, HessianSerializationContext context)
        {
            reader.BeginObject();

            if (reader.IsClassDefinition)
            {
                var className = reader.ReadString();
                var propertiesCount = reader.ReadInt32();

                if (!String.Equals(ClassName, className))
                {
                    throw new HessianSerializerException();
                }

                if (ObjectProperties.Count != propertiesCount)
                {
                    throw new HessianSerializerException();
                }

                for (var index = 0; index < propertiesCount; index++)
                {
                    var propertyName = reader.ReadString();
                    Console.WriteLine(propertyName);
                    var exists = ObjectProperties.Any(property => String.Equals(property.PropertyName, propertyName));

                    if (!exists)
                    {
                        throw new HessianSerializerException();
                    }
                }

                context.Classes.Add(ObjectType);

                reader.EndClassDefinition();
            }
            else if (reader.IsInstanceReference)
            {
                var index = reader.ReadInstanceReference();
                return context.Instances[index];
            }

            var number = reader.ReadObjectReference();
            var instance = Activator.CreateInstance(ObjectType);

            context.Instances.Add(instance);
            Console.WriteLine("===========================================");
            foreach (var item in ObjectProperties)
            {
                Console.WriteLine(item.PropertyName);
                var value = item.Deserialize(reader, context);
                Console.WriteLine(value);
                item.Property.SetValue(instance, value);
            }

            reader.EndObject();

            return instance;
        }
    }
}