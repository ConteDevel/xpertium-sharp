using XpertiumSharp.Core;

namespace XpertiumSharp.Logic
{
    public class XInterpreter
    {
        public XDatabase Database { get; private set; }

        public XInterpreter(XDatabase db)
        {
            Database = db;
        }

        public bool Run(XPredicate target)
        {
            var t = target.Clone();
            return false;
        }
    }
}
