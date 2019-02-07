namespace XpertiumSharp.Logic
{
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
