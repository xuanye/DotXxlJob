using Hessian.Net.Specification;

namespace Hessian.Net
{
    public class LeadingByte
    {
        public byte Data
        {
            get;
            private set;
        }

        public bool IsNull => Check(If.Marker.Equals(Marker.Null));

        public bool IsTrue => Check(If.Marker.Equals(Marker.True));

        public bool IsFalse => Check(If.Marker.Equals(Marker.False));

        public bool IsTinyInt32 => Check(If.Marker.Between(0x80).And(0xBF));

        public bool IsShortInt32 => Check(If.Marker.Between(0xC0).And(0xCF));

        public bool IsCompactInt32 => Check(If.Marker.Between(0xD0).And(0xD7));

        public bool IsUnpackedInt32 => Check(If.Marker.Equals(Marker.UnpackedInteger));

        public bool IsTinyInt64 => Check(If.Marker.Between(0xD8).And(0xEF));

        public bool IsShortInt64 => Check(If.Marker.Between(0xF0).And(0xFF));

        public bool IsCompactInt64 => Check(If.Marker.Between(0x38).And(0x3F));

        public bool IsPackedInt64 => Check(If.Marker.Equals(Marker.PackedLong));

        public bool IsUnpackedInt64 => Check(If.Marker.Equals(Marker.UnpackedLong));

        public bool IsCompactBinary => Check(If.Marker.Between(0x20).And(0x2f));

        public bool IsNonfinalChunkBinary => Check(If.Marker.Equals(Marker.BinaryNonfinalChunk));

        public bool IsFinalChunkBinary => Check(If.Marker.Equals(Marker.BinaryFinalChunk));

        public bool IsZeroDouble => Check(If.Marker.Equals(Marker.DoubleZero));

        public bool IsOneDouble => Check(If.Marker.Equals(Marker.DoubleOne));

        public bool IsTinyDouble => Check(If.Marker.Equals(Marker.DoubleOctet));

        public bool IsShortDouble => Check(If.Marker.Equals(Marker.DoubleShort));

        public bool IsCompactDouble => Check(If.Marker.Equals(Marker.DoubleFloat));

        public bool IsUnpackedDouble => Check(If.Marker.Equals(Marker.Double));

        public bool IsTinyString => Check(If.Marker.Between(0x00).And(0x1F));

        public bool IsCompactString => Check(If.Marker.Between(0x30).And(0x33));

        public bool IsNonfinalChunkString => Check(If.Marker.Equals(Marker.StringNonfinalChunk));

        public bool IsFinalChunkString => Check(If.Marker.Equals(Marker.StringFinalChunk));

        public bool IsCompactDateTime => Check(If.Marker.Equals(Marker.DateTimeCompact));

        public bool IsUnpackedDateTime => Check(If.Marker.Equals(Marker.DateTimeLong));

        public bool IsClassDefinition => Check(If.Marker.Equals(Marker.ClassDefinition));

        public bool IsShortObjectReference => Check(If.Marker.Between(0x60).And(0x6F));

        public bool IsLongObjectReference => Check(If.Marker.Equals(Marker.ClassReference));

        public bool IsInstanceReference => Check(If.Marker.Equals(Marker.InstanceReference));
        
        public bool IsVarList  => Check(If.Marker.Equals(Marker.VarList));
        
        public bool IsFixedList  => Check(If.Marker.Equals(Marker.FixedList));
        public bool IsVarListUntyped  => Check(If.Marker.Equals(Marker.VarListUntyped));
        public bool IsFixListUntyped  => Check(If.Marker.Equals(Marker.FixListUntyped));
        public bool IsCompactFixList  => Check(If.Marker.Between(Marker.CompactFixListStart).And(Marker.CompactFixListEnd));
        public bool IsCompactFixListUntyped => Check(If.Marker.Between(Marker.CompactFixListUntypedStart).And(Marker.CompactFixListUntypedEnd));
        
     

        public void SetData(byte value)
        {
            Data = value;
        }

        private bool Check(ISpecification<byte> specification)
        {
            return specification.IsSatisfied(Data);
        }

        private static class If
        {
            internal static class Marker
            {
                public static MarkerValue Equals(byte value)
                {
                    return new MarkerValue(value);
                }

                public static MarkerMinValue Between(byte value)
                {
                    return new MarkerMinValue(value);
                }

                internal abstract class MarkerSpecification : ISpecification<byte>
                {
                    public abstract bool IsSatisfied(byte arg);
                }

                internal class MarkerValue : MarkerSpecification
                {
                    protected readonly byte value;

                    public MarkerValue(byte value)
                    {
                        this.value = value;
                    }

                    public override bool IsSatisfied(byte arg)
                    {
                        return value == arg;
                    }
                }

                internal class MarkerMinValue : MarkerValue
                {
                    public MarkerMinValue(byte value)
                        : base(value)
                    {
                    }

                    public override bool IsSatisfied(byte arg)
                    {
                        return value <= arg;
                    }

                    public ISpecification<byte> And(byte max)
                    {
                        return this.And(new MarkerMaxValue(max));
                    }
                }

                private class MarkerMaxValue : MarkerValue
                {
                    public MarkerMaxValue(byte value)
                        : base(value)
                    {
                    }

                    public override bool IsSatisfied(byte arg)
                    {
                        return value >= arg;
                    }
                }
            }
        }
    }
}