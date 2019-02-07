namespace XpertiumSharp.Logic
{
    public class XPredicate
    {
        public IXExpression Body { get; private set; }
        public XSignature Signature { get; private set; }

        public XPredicate(IXExpression body, XSignature signature)
        {
            Body = body;
            Signature = signature;
        }

        public XPredicate(IXExpression body, string name, params XVar[] vars) : this(body, new XSignature(name, vars)) { }

        public override string ToString()
        {
            return Signature.ToString() + ":-" + Body.ToString();
        }
    }
}
