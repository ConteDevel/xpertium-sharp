using System;
using System.Linq;

namespace XpertiumSharp.Logic
{
    [Serializable]
    public class XPredicate
    {
        public XSignature Signature { get; protected set; }
        public string Name { get => Signature.Name; }
        public XVar[] Vars { get; protected set; }

        public XPredicate(XSignature signature, params XVar[] vars)
        {
            Signature = signature;
            Vars = new XVar[vars.Length];
            Array.Copy(vars, Vars, vars.Length);
        }

        public XVar GetVar(int index)
        {
            return Vars[index];
        }

        public override bool Equals(object obj)
        {
            if (obj is XPredicate)
            {
                var signature = obj as XPredicate;
                return Signature == signature.Signature && Enumerable.SequenceEqual(Vars, signature.Vars);
            }

            return false;
        }

        public static bool operator ==(XPredicate lhs, XPredicate rhs)
        {
            return lhs.Signature == rhs.Signature && Enumerable.SequenceEqual(lhs.Vars, rhs.Vars);
        }

        public static bool operator !=(XPredicate lhs, XPredicate rhs)
        {
            return lhs.Signature != rhs.Signature || !Enumerable.SequenceEqual(lhs.Vars, rhs.Vars);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                // get hash code for all items in array
                foreach (var item in Vars)
                {
                    hash = hash * 23 + ((item != null) ? item.GetHashCode() : 0);
                }

                return hash + Signature.GetHashCode();
            }
        }

        public override string ToString()
        {
            return Signature.Name + "(" + string.Join(",", Vars) + ")";
        }
    }
}
