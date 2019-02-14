using System;

namespace XpertiumSharp.Logic
{
    [Serializable]
    public class XSignature
    {
        public readonly string Name;
        public readonly int Arity;

        public XSignature(string name, int arity)
        {
            Name = name;
            Arity = arity;
        }

        public override bool Equals(object obj)
        {
            if (obj is XSignature)
            {
                var sign = obj as XSignature;
                return Arity == sign.Arity && Name == sign.Name;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Name.GetHashCode() + Arity;
            }
        }

        public static bool operator ==(XSignature lhs, XSignature rhs)
        {
            if (lhs is null || rhs is null)
            {
                return lhs is null && rhs is null;
            }

            return lhs.Arity == rhs.Arity && lhs.Name == rhs.Name;
        }

        public static bool operator !=(XSignature lhs, XSignature rhs)
        {
            if (lhs is null && rhs is null)
            {
                return false;
            }

            if (lhs is null || rhs is null)
            {
                return true;
            }

            return lhs.Arity != rhs.Arity || lhs.Name != rhs.Name;
        }

        public override string ToString()
        {
            return Name + "(" + Arity + ")";
        }
    }
}
