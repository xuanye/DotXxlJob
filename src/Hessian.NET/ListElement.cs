using System;
using System.Collections.Generic;
using System.IO;


namespace Hessian.Net
{
    public class ListElement: ISerializationElement
    {
        private readonly IDictionary<Type, ISerializationElement> _catalog;
        private readonly IObjectSerializerFactory _factory;
        public ListElement(Type listType, IDictionary<Type, ISerializationElement> catalog, IObjectSerializerFactory factory)
        {
            this.ObjectType = listType;
            this._catalog = catalog;
            this._factory = factory;
        }
        private readonly Lazy<ListTypeResolver> listTypeResolver = new Lazy<ListTypeResolver>();

        public Type ObjectType { get; }
        public ISerializationElement ChildSerializationElement { get; }

        public void Serialize(HessianOutputWriter writer, object graph, HessianSerializationContext context)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(HessianInputReader reader, HessianSerializationContext context)
        {
            object ret=null;
            var preamble = reader.BeginList();
            switch (preamble)
            {
                case ObjectPreamble.FixList:
                    ret =ReadFixList(reader, context);
                    break;
                case ObjectPreamble.VarList:
                    ret = ReadVarList(reader, context);
                    break;
                case ObjectPreamble.FixListUntyped:
                    ret = ReadFixListUntyped(reader, context);
                    break;
                case ObjectPreamble.VarListUntyped:
                    ret = ReadVarListUntyped(reader, context);
                    break;
                case ObjectPreamble.CompactFixList:
                    ret = ReadCompactFixList(reader, context);
                    break;
                case ObjectPreamble.CompactFixListUntyped:
                    ret = ReadCompactFixListUntyped(reader, context);
                    break;
            }            
            
            reader.EndList();
            return ret;
        }
        
        
        private string ReadTypeName(HessianInputReader reader)
        {
         
            var tag = reader.Peek();
            
            // A type name is either a string, or an integer reference to a
            // string already read and stored in the type-name ref map.
            if ((tag >= 0x00 && tag < 0x20)
                || (tag >= 0x30 && tag < 0x34)
                || tag == 0x52
                || tag == 0x53) {

                var typeName = reader.ReadString();            
                return typeName;
            }

            reader.ReadInt32();
            return "";

        }

        #region List

        private IList<object> ReadVarList(HessianInputReader reader, HessianSerializationContext context)
        {
            var type = ReadTypeName(reader);
            return ReadListCore(reader, context, type: type);
        }

        private IList<object> ReadFixList(HessianInputReader reader, HessianSerializationContext context)
        {
            var type = ReadTypeName(reader);
            var length = reader.ReadInt32();
            return ReadListCore(reader, context, length, type);
        }

        private IList<object> ReadVarListUntyped(HessianInputReader reader, HessianSerializationContext context)
        {
            return ReadListCore(reader, context);
        }

        private IList<object> ReadFixListUntyped(HessianInputReader reader, HessianSerializationContext context)
        {
            var length = reader.ReadInt32();
            return ReadListCore(reader, context, length);
        }

        private IList<object> ReadCompactFixList(HessianInputReader reader, HessianSerializationContext context)
        {
            var tag = reader.LeadingByte.Data;
            var length = tag - 0x70;
            var type = ReadTypeName(reader);
            return ReadListCore(reader, context, length, type);
        }

        private IList<object> ReadCompactFixListUntyped(HessianInputReader reader, HessianSerializationContext context)
        {
            var tag = reader.LeadingByte.Data;
            var length = tag - 0x70;
            return ReadListCore(reader, context, length);
        }

        private IList<object> ReadListCore(HessianInputReader reader, HessianSerializationContext context, int? length = null, string type = null)
        {
            var list = GetListInstance(type, length);

            //objectRefs.Add(list);

            if (length.HasValue) {
                PopulateFixLengthList(reader, context, list, length.Value);
            } else {
                PopulateVarList(reader, context, list);
            }
            return list;
        }

        private IList<object> GetListInstance(string type, int? length = null)
        {
            IList<object> list;

            if (length.HasValue) {
                if (!listTypeResolver.Value.TryGetListInstance(type, length.Value, out list)) {
                    list = new List<object>(length.Value);
                }
            } else {
                if (!listTypeResolver.Value.TryGetListInstance(type, out list)) {
                    list = new List<object>();
                }
            }

            return list;
        }

        private void PopulateFixLengthList(HessianInputReader reader, HessianSerializationContext context, IList<object> list, int length)
        {
            var tag = reader.ReadByte(); //0x16
            
            for (var i = 0; i < length; ++i)
            {
                ObjectElement objectElement = new ObjectElement();
                var scheme = HessianSerializationScheme.CreateFromType(this.GetType(), this._factory);
                var obj = scheme.Deserialize(reader, context);
                list.Add(obj);
            }
        }

        private void PopulateVarList(HessianInputReader reader, HessianSerializationContext context, IList<object> list)
        {
            while (true) {
                var tag = reader.ReadByte();
                if (tag == 'Z') {
                    reader.ReadByte();
                    break;
                }
                list.Add(this.ChildSerializationElement.Deserialize(reader, context));
            }
        }

        #endregion
    }
}