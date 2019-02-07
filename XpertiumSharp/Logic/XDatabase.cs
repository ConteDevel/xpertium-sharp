using System.Collections.Generic;

namespace XpertiumSharp.Logic
{
    public class XDatabase
    {
        public List<XSignature> Signatures { get; private set; }
        public List<XClause> Clauses { get; private set; }

        public XDatabase()
        {
            Signatures = new List<XSignature>();
            Clauses = new List<XClause>();
        }

        public XDatabase(List<XSignature> signatures, List<XClause> clauses)
        {
            Signatures = signatures;
            Clauses = clauses;
        }
    }
}
