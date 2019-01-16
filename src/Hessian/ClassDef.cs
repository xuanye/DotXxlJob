using System;

namespace Hessian
{
    public class ClassDef : IEquatable<ClassDef>
    {
        public string Name { get; private set; }
        public string[] Fields { get; private set; }

        public ClassDef(string name, string[] fields)
        {
            Name = Conditions.CheckNotNull(name, "name");
            Fields = Conditions.CheckNotNull(fields, "fields");
        }

        public override int GetHashCode()
        {
            unchecked {
                const uint prime = 16777619;
                var hash = 2166136261;

                hash *= prime;
                hash ^= (uint)Name.GetHashCode();

                for (var i = 0; i < Fields.Length; ++i) {
                    hash *= prime;
                    hash ^= (uint)Fields[i].GetHashCode();
                }

                return (int)hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((ClassDef) obj);
        }

        public bool Equals(ClassDef other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return string.Equals(Name, other.Name) && Fields.Equals(other.Fields);
        }
    }
}
