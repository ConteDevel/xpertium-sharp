using System;
using System.Text;
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
                    for (int j = 0; j < copy.Predicate.Signature.Arity; ++j)
                    {
                        var tmpArg = copy.Predicate.Vars[j];

                        if (tmpArg == oArg)
                        {
                            copy.Predicate.Vars[j] = sArg;

                            if (copy.Body != null)
                            {
                                copy.Body.Bind(tmpArg, sArg);
                            }
                        }
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
            if (lhs is null || rhs is null)
            {
                return lhs is null && rhs is null;
            }

            return lhs.Predicate == rhs.Predicate && lhs.Body.Equals(rhs.Body);
        }

        public static bool operator !=(XClause lhs, XClause rhs)
        {
            if (lhs is null && rhs is null)
            {
                return false;
            }

            if (lhs is null || rhs is null)
            {
                return true;
            }

            return lhs.Predicate != rhs.Predicate || !lhs.Body.Equals(rhs.Body);
        }

        public override string ToString()
        {
            var sb = new StringBuilder(Predicate.ToString());

            if (Body != null)
            {
                sb.Append(" :- ");
                sb.Append(Body.ToString());
            }

            sb.Append('.');

            return sb.ToString();
        }
    }
}
