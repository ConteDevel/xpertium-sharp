using System;
using System.Linq;

namespace XpertiumSharp.Logic
{
    public class XSignature
    {
        public string Name { get; protected set; }
        public XVar[] Vars { get; protected set; }

        public XSignature(string name, params XVar[] vars)
        {
            Name = name;
            Vars = new XVar[vars.Length];
            Array.Copy(vars, Vars, vars.Length);
        }

        public XVar GetVar(int index)
        {
            return Vars[index];
        }

        public override bool Equals(object obj)
        {
            if (obj is XSignature)
            {
                var signature = obj as XSignature;
                return Name.Equals(signature.Name) && Enumerable.SequenceEqual(Vars, signature.Vars);
            }

            return false;
        }

        public static bool operator ==(XSignature lhs, XSignature rhs)
        {
            return lhs.Name.Equals(rhs.Name) && Enumerable.SequenceEqual(lhs.Vars, rhs.Vars);
        }

        public static bool operator !=(XSignature lhs, XSignature rhs)
        {
            return !lhs.Name.Equals(rhs.Name) || !Enumerable.SequenceEqual(lhs.Vars, rhs.Vars);
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

                return hash;
            }
        }

        public override string ToString()
        {
            return "(" + string.Join(",", Vars) + ")";
        }
    }
}
