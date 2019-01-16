using System;
using System.Collections.Generic;
using System.IO;

namespace Hessian.Net
{
    public class ListElement: ISerializationElement
    {
        public ListElement(Type listType)
        {
            this.ObjectType = listType.GetElementType();
        }
        
        public Type ObjectType { get; }
        public void Serialize(HessianOutputWriter writer, object graph, HessianSerializationContext context)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(HessianInputReader reader, HessianSerializationContext context)
        {
            var preamble = reader.BeginList();
            switch (preamble)
            {
                case ObjectPreamble.FixList:
                    break;
                case ObjectPreamble.VarList:
                    break;
                case ObjectPreamble.FixListUntyped:
                    break;
                case ObjectPreamble.VarListUntyped:
                    break;
                case ObjectPreamble.CompactFixList:
                    break;
                case ObjectPreamble.CompactFixListUntyped:
                    break;
            }
            
            
            reader.EndList();
        }
        
        
         private string ReadTypeName(HessianInputReader reader)
        {
            reader.re
            var tag = reader.Peek();

            if (!tag.HasValue) {
                throw new EndOfStreamException();
            }

            // A type name is either a string, or an integer reference to a
            // string already read and stored in the type-name ref map.
            if ((tag >= 0x00 && tag < 0x20)
                || (tag >= 0x30 && tag < 0x34)
                || tag == 0x52
                || tag == 0x53) {
                var typeName = ReadString();
                typeNameRefs.Add(typeName);
                return typeName;
            }

            return typeNameRefs.Get(ReadInteger());
        }

        #region List

        private IList<object> ReadVarList()
        {
            reader.ReadByte();
            var type = ReadTypeName();
            return ReadListCore(type: type);
        }

        private IList<object> ReadFixList()
        {
            reader.ReadByte();
            var type = ReadTypeName();
            var length = ReadInteger();
            return ReadListCore(length, type);
        }

        private IList<object> ReadVarListUntyped()
        {
            reader.ReadByte();
            return ReadListCore();
        }

        private IList<object> ReadFixListUntyped()
        {
            reader.ReadByte();
            var length = ReadInteger();
            return ReadListCore(length);
        }

        private IList<object> ReadCompactFixList()
        {
            var tag = reader.ReadByte();
            var length = tag - 0x70;
            var type = ReadTypeName();
            return ReadListCore(length, type);
        }

        private IList<object> ReadCompactFixListUntyped()
        {
            var tag = reader.ReadByte();
            var length = tag - 0x70;
            return ReadListCore(length);
        }

        private IList<object> ReadListCore(int? length = null, string type = null)
        {
            var list = GetListIntance(type, length);

            objectRefs.Add(list);

            if (length.HasValue) {
                PopulateFixLengthList(list, length.Value);
            } else {
                PopulateVarList(list);
            }
            return list;
        }

        private IList<object> GetListIntance(string type, int? length = null)
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

        private void PopulateFixLengthList(IList<object> list, int length)
        {
            for (var i = 0; i < length; ++i) {
                list.Add(ReadValue());
            }
        }

        private void PopulateVarList(IList<object> list)
        {
            while (true) {
                var tag = reader.Peek();
                if (!tag.HasValue) {
                    throw new EndOfStreamException();
                }
                if (tag == 'Z') {
                    reader.ReadByte();
                    break;
                }
                list.Add(ReadValue());
            }
        }

        #endregion
    }
}