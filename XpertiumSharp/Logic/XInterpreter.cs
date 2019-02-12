using System.Collections.Generic;

namespace XpertiumSharp.Logic
{
    public class XInterpreter
    {
        public XDatabase Database { get; private set; }

        public XInterpreter(XDatabase db)
        {
            Database = db;
        }

        private bool VerifySignature(XSignature signature)
        {
            foreach (var s in Database.Signatures)
            {
                if (signature.Name == s.Name)
                {
                    if (signature.Arity != s.Arity)
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool Run(XPredicate target, out List<XPredicate> solutions)
        {
            solutions = new List<XPredicate>();

            if (!VerifySignature(target.Signature))
            {
                return false;
            }

            foreach (var c in Database.Clauses)
            {
                var clause = c.Bind(target);

                if (clause != null)
                {
                    if (clause.Body == null)
                    {
                        solutions.Add(clause.Predicate);
                    }
                }
            }

            return solutions.Count != 0;
        }
    }
}
