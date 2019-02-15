using System;

namespace XpertiumSharp.Logic
{
    [Serializable]
    public struct XVar
    {
        public XType Type { get; private set; }
        public string Value { get; private set; }

        public XVar(XType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is XVar)
            {
                XVar other = (XVar)obj;
                return Type == other.Type && Value.Equals(other.Value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Value.GetHashCode() + Type.GetHashCode();
            }
        }

        public static bool operator ==(XVar lhs, XVar rhs)
        {
            return lhs.Type == rhs.Type && lhs.Value.Equals(rhs.Value);
        }

        public static bool operator !=(XVar lhs, XVar rhs)
        {
            return lhs.Type != rhs.Type || !lhs.Value.Equals(rhs.Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
