namespace XpertiumSharp.Fuzzy
{
    public struct XPhase
    {
        public readonly string Name;
        public readonly IXFunction Membership;
        
        public XPhase(string name, IXFunction membership)
        {
            Name = name;
            Membership = membership;
        }

        public double Hit(double x)
        {
            return Membership.Invoke(x);
        }
    }
}
