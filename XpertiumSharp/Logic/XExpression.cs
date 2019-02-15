using System;
using System.Collections.Generic;
using System.Linq;

namespace XpertiumSharp.Logic
{
    public enum XOperand
    {
        Predicate,
        Not,
        And
    }
    
    public interface IXExpression
    {
        XOperand Type { get; }
        void Bind(XVar oldV, XVar newV);
    }

    [Serializable]
    public class XExpression : IXExpression
    {
        public XPredicate Predicate { get; private set; }
        public XOperand Type => XOperand.Predicate;

        public XExpression(XPredicate predicate)
        {
            Predicate = predicate;
        }

        public override bool Equals(object obj)
        {
            if (obj is XExpression)
            {
                var exp = obj as XExpression;
                return Predicate == exp.Predicate;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Predicate.GetHashCode() + (int)Type;
            }
        }

        public override string ToString()
        {
            return Predicate.ToString();
        }

        public void Bind(XVar oldV, XVar newV)
        {
            if (oldV.Type == XType.Var)
            {
                for (int i = 0; i < Predicate.Signature.Arity; ++i)
                {
                    if (Predicate.Vars[i] == oldV)
                    {
                        Predicate.Vars[i] = newV;
                    }
                }
            }
        }
    }

    [Serializable]
    public class XNot : IXExpression
    {
        public IXExpression Expression { get; private set; }
        public XOperand Type => XOperand.Not;

        public XNot(IXExpression expression)
        {
            Expression = expression;
        }

        public override bool Equals(object obj)
        {
            if (obj is XNot)
            {
                var exp = obj as XNot;
                return Expression.Equals(exp.Expression);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Expression.GetHashCode() + (int)Type;
            }
        }

        public override string ToString()
        {
            return "not(" + Expression.ToString() + ")";
        }

        public void Bind(XVar oldV, XVar newV)
        {
            Expression.Bind(oldV, newV);
        }
    }

    [Serializable]
    public class XAnd : IXExpression
    {
        public List<IXExpression> Childs { get; private set; }
        public XOperand Type => XOperand.And;

        public XAnd(params IXExpression[] childs)
        {
            Childs = new List<IXExpression>(childs);
        }

        public override bool Equals(object obj)
        {
            if (obj is XAnd)
            {
                var exp = obj as XAnd;
                return Enumerable.SequenceEqual(Childs, exp.Childs);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                // get hash code for all items in array
                foreach (var item in Childs)
                {
                    hash = hash * 23 + ((item != null) ? item.GetHashCode() : 0);
                }

                return hash + (int)Type;
            }
        }

        public override string ToString()
        {
            return "and(" + string.Join<IXExpression>(",", Childs) + ")";
        }

        public void Bind(XVar oldV, XVar newV)
        {
            foreach (var child in Childs)
            {
                child.Bind(oldV, newV);
            }
        }
    }
}
