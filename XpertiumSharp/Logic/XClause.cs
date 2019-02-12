using System;
using XpertiumSharp.Core;

namespace XpertiumSharp.Logic
{
    [Serializable]
    public class XClause
    {
        public IXExpression Body { get; private set; }
        public XPredicate Predicate { get; private set; }

        public XClause(IXExpression body, XPredicate predicate)
        {
            Body = body;
            Predicate = predicate;
        }

        public XClause(IXExpression body, XSignature signature, params XVar[] vars) : this(body, new XPredicate(signature, vars)) { }

        public XClause Bind(XPredicate predicate)
        {
            if (predicate.Signature != Predicate.Signature)
            {
                return null;
            }

            var copy = this.Clone();

            for (int i = 0; i < predicate.Signature.Arity; ++i)
            {
                var sArg = predicate.Vars[i];
                var oArg = copy.Predicate.Vars[i];

                if (sArg.Type == XType.Const && oArg.Type == XType.Const && sArg.Value != oArg.Value)
                {
                    return null;
                }

                if (sArg != oArg && oArg.Type != XType.Const)
                {
                    copy.Predicate.Vars[i] = new XVar(sArg.Type, sArg.Value);

                    if (copy.Body != null)
                    {
                        copy.Body.Bind(oArg, sArg);
                    }
                }
            }

            return copy;
        }

        public override bool Equals(object obj)
        {
            if (obj is XClause)
            {
                var p = obj as XClause;
                return Predicate == p.Predicate && Body.Equals(p.Body);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Predicate.GetHashCode() + Body.GetHashCode();
            }
        }

        public static bool operator ==(XClause lhs, XClause rhs)
        {
            return lhs.Predicate == rhs.Predicate && lhs.Body.Equals(rhs.Body);
        }

        public static bool operator !=(XClause lhs, XClause rhs)
        {
            return lhs.Predicate != rhs.Predicate || !lhs.Body.Equals(rhs.Body);
        }

        public override string ToString()
        {
            return Predicate.ToString() + ":-" + Body.ToString();
        }
    }
}
