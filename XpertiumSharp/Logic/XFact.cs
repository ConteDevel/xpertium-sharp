namespace XpertiumSharp.Logic
{
    public class XFact : XSignature
    {
        public XFact(string name, params XVar[] vars) : base(name, vars) { }
    }
}
