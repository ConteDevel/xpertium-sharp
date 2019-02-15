using System.Collections.Generic;
using XpertiumSharp.Core;

namespace XpertiumSharp.Logic
{
    public class XInterpreter
    {
        private readonly IXLogger logger;

        public XDatabase Database { get; private set; }

        public XInterpreter(XDatabase db, IXLogger logger = null)
        {
            Database = db;
            this.logger = (logger == null) ? new XLogger() : logger;
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

        public void Unify(XPredicate src, XPredicate dest, IXExpression expression)
        {
            for (int i = 0; i < src.Signature.Arity; ++i)
            {
                var sArg = src.Vars[i];
                var dArg = dest.Vars[i];

                if (sArg.Type == XType.Var && dArg.Type == XType.Const)
                {
                    expression.Bind(sArg, dArg);
                }
            }
        }

        public bool Resolve(List<XPredicate> solutions, XAnd expression, out List<XPredicate> facts)
        {
            if (expression.Childs.Count > 0)
            {
                var target = expression.Childs[0];

                if (!Resolve(solutions, target, out facts))
                {
                    return false;
                }

                if (target.Type == XOperand.Predicate)
                {
                    var src = target as XExpression;
                    expression.Childs.RemoveAt(0);

                    for (int i = 0; i < facts.Count;)
                    {
                        var dest = facts[i];
                        var exp = expression.Clone();
                        Unify(src.Predicate, dest, exp);

                        if (Resolve(solutions, exp, out _))
                        {
                            return true;
                        }
                    }
                }

                facts = new List<XPredicate>();
                return false;
            }

            facts = new List<XPredicate>();
            return true;
        }

        public bool Resolve(List<XPredicate> solutions, IXExpression expression, out List<XPredicate> facts)
        {
            if (expression == null)
            {
                facts = new List<XPredicate>();
                return true;
            }

            switch (expression.Type)
            {
                case XOperand.Predicate:
                    var predicateExp = expression as XExpression;
                    return Run(predicateExp.Predicate, out facts);
                case XOperand.Not:
                    var notExp = expression as XNot;
                    var localSolutions = new List<XPredicate>();
                    return !Resolve(localSolutions, notExp.Expression.Clone(), out facts);
                default:
                    var andExp = expression as XAnd;
                    return Resolve(solutions, andExp, out facts);
            }
        }

        public bool Run(XPredicate target, out List<XPredicate> solutions)
        {
            logger.LogD("Target: {0}", target);
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
                    logger.LogD("Suitable clause: {0}", clause);
                    ++logger.Indent;
                    if (Resolve(solutions, clause.Body, out _) /*&& clause.Body == null*/ && !solutions.Contains(clause.Predicate))
                    {
                        solutions.Add(clause.Predicate);
                    }
                    --logger.Indent;
                }
            }

            return solutions.Count != 0;
        }
    }
}
