using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Hessian
{
    public class Serializer
    {
        static  readonly ConcurrentDictionary<Type,ClassElement> ClassDefCache =new ConcurrentDictionary<Type, ClassElement>();
        
        private readonly Stream _stream;
        private readonly HessianSerializationContext _context;

        public Serializer (Stream stream)
        {
            this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _context = new HessianSerializationContext();
        }

        public void WriteObject(object graph)
        {
            var objectType = graph.GetType();
            if (!ClassDefCache.TryGetValue(objectType, out var classDef))
            {
                classDef = GetClassDef(objectType.GetTypeInfo());
            }
          
            var index = this._context.Instances.IndexOf(graph);

            if (index > -1)
            {
                WriteInstanceReference(index);
                return;
            }

            this._context.Instances.Add(graph);

            index = this._context.Classes.IndexOf(objectType);

            if (index < 0)
            {
                BeginClassDefinition();
                WriteString(classDef.ClassName);
                WriteInt32(classDef.Fields.Count);

                foreach (var property in classDef.Fields)
                {
                    WriteString(property.Name);
                }
                EndClassDefinition();

                index = this._context.Classes.Count;

                this._context.Classes.Add(objectType);
            }
            WriteObjectReference(index);
            foreach (var item in classDef.Fields)
            {
                var value = item.PropertyInfo.GetValue(graph);
                WriteValue(value);
            }
        }

        public void WriteValue(object val)
        {
            if (val == null)
            {
                WriteNull();
                return;
            }
            var valType = val.GetType();
            var typeInfo = valType.GetTypeInfo();
            if (IsSimpleType(typeInfo))
            {
                WriteSimpleValue(val);
                return;
            }

            if (IsListType(typeInfo))
            {
                WriteListValue(val);
                return;
            }

            WriteObject(val);
        }

        private void WriteListValue(object val)
        {
            var tag = (int)Marker.CompactFixListStart;
            if (!(val is ICollection eVal))
            {
                throw new HessianException("write list data error");
            }
            tag += eVal.Count;
            if (tag > Marker.CompactFixListEnd)
            {
                throw new HessianException("write list data error,tag too large");
            }
            this._stream.WriteByte((byte)tag);
            int index = 1;
            foreach (var item in eVal)
            {
                if (index == 1)
                {
                    WriteString("["+GetItemTypeName(item.GetType()));
                }
                WriteValue(item);
            }
        }

        private string GetItemTypeName(Type type)
        {
            var classType = type.GetCustomAttribute<DataContractAttribute>();
            if (classType != null)
            {
                return classType.Name;
            }

            return type.Name;
        }
        private void WriteSimpleValue(object val)
        {
            var valType = val.GetType();
            if (valType == typeof(int))
            {
                WriteInt32((int)val);
            }
            else  if (valType == typeof(bool))
            {
                WriteBoolean((bool)val);
            }
            else  if (valType == typeof(long))
            {
                WriteInt64((long)val);
            }
            else  if (valType == typeof(double))
            {
                WriteDouble((double)val);
            }
            else  if (valType == typeof(string))
            {
                WriteString((string)val);
            }
            else  if (valType == typeof(DateTime))
            {
                WriteDateTime((DateTime)val);
            }
        }
        
        private ClassElement GetClassDef(TypeInfo typeInfo)
        {
          
           var classAttr = typeInfo.GetCustomAttribute<DataContractAttribute>();
           if (classAttr == null)
           {
               throw  new HessianException("DataContract must be set");
           }

           ClassElement ce = new ClassElement {ClassName = classAttr.Name, Fields = new List<PropertyElement>()};

           //ClassDef def = new ClassDef(classAttr.Name);
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
               PropertyElement p = new PropertyElement {
                   Name = attribute.Name,
                   Order =  attribute.Order,
                   PropertyInfo = property
               };
               ce.Fields .Add( p );
           }
           ce.Fields.Sort(new PropertyComparer());
           return ce;
        }

        /// <summary>
        /// Writes NULL token into stream
        /// </summary>
        public  void WriteNull()
        {
            this._stream.WriteByte(Marker.Null);
        }

        /// <summary>
        /// Writes <see cref="System.Boolean" /> value into output stream.
        /// </summary>
        /// <param name="value">The value.</param>
        public  void WriteBoolean(bool value)
        {
            this._stream.WriteByte(value ? Marker.True : Marker.False);
        }

        /// <summary>
        /// Writes array of <see cref="System.Byte" /> into output stream.
        /// </summary>
        /// <param name="buffer">The value.</param>
        public void WriteBytes(byte[] buffer)
        {
            if (null == buffer)
            {
                WriteNull();
                return;
            }

            WriteBytes(buffer, 0, buffer.Length);
        }

        public  void WriteBytes(byte[] buffer, int offset, int count)
        {
            if (offset < 0)
            {
                throw new ArgumentException("", nameof(offset));
            }

            if (null == buffer)
            {
                WriteNull();
                return;
            }

            if (count < 0x10)
            {
                this._stream.WriteByte((byte)(0x20 + (count & 0x0F)));
                this._stream.Write(buffer, offset, count);
                return;
            }

            const int chunkSize = 0x8000;

            while (count > chunkSize)
            {
                this._stream.WriteByte(Marker.BinaryNonfinalChunk);
                this._stream.WriteByte(chunkSize >> 8);
                this._stream.WriteByte(chunkSize & 0xFF);
                this._stream.Write(buffer, offset, chunkSize);

                count -= chunkSize;
                offset += chunkSize;
            }

            this._stream.WriteByte(Marker.BinaryFinalChunk);
            this._stream.WriteByte((byte)(count >> 8));
            this._stream.WriteByte((byte)(count & 0xFF));
            this._stream.Write(buffer, offset, count);
        }

        public  void WriteDateTime(DateTime value)
        {
            if (value.Second == 0)
            {
                var s = value.GetTotalMinutes();

                this._stream.WriteByte(Marker.DateTimeCompact);
                this._stream.WriteByte((byte)(s >> 24));
                this._stream.WriteByte((byte)(s >> 16));
                this._stream.WriteByte((byte)(s >> 8));
                this._stream.WriteByte((byte)s);

                return;
            }

            var dt = value.GetTotalMilliseconds();

            this._stream.WriteByte(Marker.DateTimeLong);
            this._stream.WriteByte((byte)(dt >> 56));
            this._stream.WriteByte((byte)(dt >> 48));
            this._stream.WriteByte((byte)(dt >> 40));
            this._stream.WriteByte((byte)(dt >> 32));
            this._stream.WriteByte((byte)(dt >> 24));
            this._stream.WriteByte((byte)(dt >> 16));
            this._stream.WriteByte((byte)(dt >> 8));
            this._stream.WriteByte((byte)dt);
        }

        public  void WriteDouble(double value)
        {
            if (value.Equals(0.0d))
            {
                this._stream.WriteByte(Marker.DoubleZero);
                return;
            }

            if (value.Equals(1.0d))
            {
                this._stream.WriteByte(Marker.DoubleOne);
                return;
            }

            var fraction = Math.Abs(value - Math.Truncate(value));

            if (Double.Epsilon >= fraction)
            {
                if (Byte.MinValue <= value && value <= Byte.MaxValue)
                {
                    this._stream.WriteByte(Marker.DoubleOctet);
                    this._stream.WriteByte(Convert.ToByte(value));

                    return;
                }

                if (Int16.MinValue <= value && value <= Int16.MaxValue)
                {
                    var val = Convert.ToInt16(value);

                    this._stream.WriteByte(Marker.DoubleShort);
                    this._stream.WriteByte((byte)(val >> 8));
                    this._stream.WriteByte((byte)val);

                    return;
                }
            }

            if (Single.MinValue <= value && value <= Single.MaxValue)
            {
                var bytes = BitConverter.GetBytes((float) value);

                this._stream.WriteByte(Marker.DoubleFloat);

                for (var index = bytes.Length - 1; index >= 0; index--)
                {
                    this._stream.WriteByte(bytes[index]);
                }

                return;
            }

            var temp = BitConverter.DoubleToInt64Bits(value);

            this._stream.WriteByte(Marker.Double);

            for (var index = 56; index >= 0; index -= 8)
            {
                this._stream.WriteByte((byte) (temp >> index));
            }
        }

        public  void WriteInt32(int value)
        {
            if (-16 <= value && value < 48)
            {
                this._stream.WriteByte((byte)(0x90 + value));
            }
            else if (-2048 <= value && value < 2048)
            {
                this._stream.WriteByte((byte)(0xC8 + (byte)(value >> 8)));
                this._stream.WriteByte((byte)value);
            }
            else if (-262144 <= value && value < 262144)
            {
                this._stream.WriteByte((byte)(0xD4 + (byte)(value >> 16)));
                this._stream.WriteByte((byte)(value >> 8));
                this._stream.WriteByte((byte)value);
            }
            else
            {
                this._stream.WriteByte(Marker.UnpackedInteger);
                this._stream.WriteByte((byte)(value >> 24));
                this._stream.WriteByte((byte)(value >> 16));
                this._stream.WriteByte((byte)(value >> 8));
                this._stream.WriteByte((byte)value);
            }
        }

        public  void WriteInt64(long value)
        {
            if (-8 <= value && value < 16)
            {
                this._stream.WriteByte((byte)(0xE0 + value));
            }
            else if (-2048 <= value && value < 2048)
            {
                this._stream.WriteByte((byte)(0xF8 + (byte)(value >> 8)));
                this._stream.WriteByte((byte)value);
            }
            else if (-262144 <= value && value < 262144)
            {
                this._stream.WriteByte((byte)(0x3C + (byte)(value >> 16)));
                this._stream.WriteByte((byte)(value >> 8));
                this._stream.WriteByte((byte)value);
            }
            else if (Int32.MinValue <= value && value <= Int32.MaxValue)
            {
                this._stream.WriteByte(Marker.PackedLong);
                this._stream.WriteByte((byte)(value >> 24));
                this._stream.WriteByte((byte)(value >> 16));
                this._stream.WriteByte((byte)(value >> 8));
                this._stream.WriteByte((byte)value);
            }
            else
            {
                this._stream.WriteByte(Marker.UnpackedLong);
                this._stream.WriteByte((byte)(value >> 56));
                this._stream.WriteByte((byte)(value >> 48));
                this._stream.WriteByte((byte)(value >> 40));
                this._stream.WriteByte((byte)(value >> 32));
                this._stream.WriteByte((byte)(value >> 24));
                this._stream.WriteByte((byte)(value >> 16));
                this._stream.WriteByte((byte)(value >> 8));
                this._stream.WriteByte((byte)value);
            }
        }

        public  void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                this._stream.WriteByte(0x00);
                return;
            }

            var length = value.Length;

            if (1024 > length)
            {
                var bytes = Encoding.UTF8.GetBytes(value.ToCharArray());

                if (32 > length)
                {
                    this._stream.WriteByte((byte) length);
                }
                else
                {
                    this._stream.WriteByte((byte) (0x30 + (byte) (length >> 8)));
                    this._stream.WriteByte((byte) length);
                }

                this._stream.Write(bytes, 0, bytes.Length);

                return;
            }

            const int maxChunkLength = 1024;
            var position = 0;

            while (position < length)
            {
                var count = Math.Min(length - position, maxChunkLength);
                var final = length == (position + count);
                var chunk = value.Substring(position, count);
                var bytes = Encoding.UTF8.GetBytes(chunk.ToCharArray());

                this._stream.WriteByte(final ? Marker.StringFinalChunk : Marker.StringNonfinalChunk);
                this._stream.WriteByte((byte)(count >> 8));
                this._stream.WriteByte((byte)count);
                this._stream.Write(bytes, 0, bytes.Length);

                position += count;
            }
        }
      
        public  void BeginClassDefinition()
        {
            this._stream.WriteByte(Marker.ClassDefinition);
        }

        public  void EndClassDefinition()
        {
        }

        public  void WriteObjectReference(int index)
        {
            if (index < 0x10)
            {
                this._stream.WriteByte((byte)(0x60 + index));
            }
            else
            {
                this._stream.WriteByte(Marker.ClassReference);
                WriteInt32(index);
            }
        }

        public  void WriteInstanceReference(int index)
        {
            this._stream.WriteByte(Marker.InstanceReference);
            WriteInt32(index);
        }
        
        
        private static bool IsSimpleType(TypeInfo typeInfo)
        {
            if (typeInfo.IsValueType || typeInfo.IsEnum || typeInfo.IsPrimitive)
            {
                return true;
            }

            if (typeof (String) == typeInfo.AsType())
            {
                return true;
            }

            return false;
        }

        private static bool IsListType(TypeInfo typeInfo)
        {
            return typeof(ICollection).IsAssignableFrom(typeInfo);          
        }
    }
}