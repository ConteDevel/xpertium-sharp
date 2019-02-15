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

        public bool Resolve2(XPredicate target, XAnd expression, out List<XPredicate> facts, out List<XPredicate> localFacts)
        {
            if (expression.Childs.Count > 0)
            {
                var exp = expression.Childs[0];
                facts = new List<XPredicate>();

                if (!Resolve(target, exp, out List<XPredicate> partialSolutions, out localFacts))
                {
                    return false;
                }

                if (exp.Type == XOperand.Predicate)
                {
                    var src = exp as XExpression;
                    expression.Childs.RemoveAt(0);

                    bool hasSolutions = false;

                    for (int i = 0; i < localFacts.Count; ++i)
                    {
                        var dest = localFacts[i];
                        var expCopy = expression.Clone();
                        Unify(src.Predicate, dest, expCopy);

                        ++logger.Indent;

                        if (Resolve2(partialSolutions[i], expCopy, out List<XPredicate> partialSolutions2, out _))
                        {
                            facts.AddRange(partialSolutions2);
                            hasSolutions = true;
                        }

                        --logger.Indent;
                    }

                    return hasSolutions;
                }

                return false;
            }
            
            facts = new List<XPredicate>() { target };
            localFacts = new List<XPredicate>();
            return true;
        }

        public bool Resolve(XPredicate target, IXExpression expression, out List<XPredicate> facts, out List<XPredicate> localFacts)
        {
            if (expression == null)
            {
                facts = new List<XPredicate>();
                localFacts = new List<XPredicate>();

                return true;
            }

            switch (expression.Type)
            {
                case XOperand.Predicate:
                    var predicateExp = expression as XExpression;
                    facts = new List<XPredicate>();

                    if (Run(predicateExp.Predicate, out localFacts))
                    {

                        foreach (var f in localFacts)
                        {
                            var clone = target.Clone();

                            for (int i = 0; i < f.Vars.Length; ++i)
                            {
                                var sArg = predicateExp.Predicate.Vars[i];
                                var dArg = f.Vars[i];

                                if (sArg.Type == XType.Var && dArg.Type == XType.Const)
                                {
                                    clone.Bind(sArg, dArg);
                                }
                            }

                            facts.Add(clone);
                        }

                        return true;
                    }

                    return false;
                case XOperand.Not:
                    var notExp = expression as XNot;
                    return !Resolve(target, notExp.Expression.Clone(), out facts, out localFacts);
                default:
                    var andExp = expression as XAnd;
                    return Resolve2(target, andExp, out facts, out localFacts);
            }
        }

        public bool Run(XPredicate target, out List<XPredicate> solutions)
        {
            logger.LogD("Target: {0}", target);
            solutions = new List<XPredicate>();

            if (!VerifySignature(target.Signature))
            {
                logger.LogD("FAILED");
                return false;
            }

            foreach (var c in Database.Clauses)
            {
                var clause = c.Bind(target.Clone());

                if (clause != null)
                {
                    logger.LogD("Suitable clause: {0}", clause);
                    ++logger.Indent;
                    var newTarget = clause.Predicate;

                    if (Resolve(newTarget, clause.Body, out List<XPredicate> partialSolutions, out _))
                    {
                        if (clause.Body == null && !solutions.Contains(newTarget))
                        {
                            solutions.Add(newTarget);
                        }
                        else
                        {
                            solutions.AddRange(partialSolutions);
                        }
                    }

                    --logger.Indent;
                }
            }
            
            if (solutions.Count != 0)
            {
                logger.LogD("SUCCESS");
                return true;
            }

            logger.LogD("FAILED");
            return false;
        }
    }
}
