namespace XpertiumSharp.Logic
{
    public enum XOperand
    {
        Predicate,
        Not,
        And,
        Or
    }

    public interface IXExpression
    {
        XOperand Type { get; }
    }

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
    }

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
    }

    public class XAnd : IXExpression
    {
        public IXExpression Left { get; private set; }
        public IXExpression Right { get; private set; }
        public XOperand Type => XOperand.And;

        public XAnd(IXExpression left, IXExpression right)
        {
            Left = left;
            Right = right;
        }

        public override bool Equals(object obj)
        {
            if (obj is XAnd)
            {
                var exp = obj as XAnd;
                return Left.Equals(exp.Left) && Right.Equals(exp.Right);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Left.GetHashCode() + Right.GetHashCode() + (int)Type;
            }
        }

        public override string ToString()
        {
            return "and(" + Left.ToString() + "," + Right.ToString() + ")";
        }
    }

    public class XOr : IXExpression
    {
        public IXExpression Left { get; private set; }
        public IXExpression Right { get; private set; }
        public XOperand Type => XOperand.Or;

        public XOr(IXExpression left, IXExpression right)
        {
            Left = left;
            Right = right;
        }

        public override bool Equals(object obj)
        {
            if (obj is XAnd)
            {
                var exp = obj as XAnd;
                return Left.Equals(exp.Left) && Right.Equals(exp.Right);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Left.GetHashCode() + Right.GetHashCode() + (int)Type;
            }
        }

        public override string ToString()
        {
            return "or(" + Left.ToString() + "," + Right.ToString() + ")";
        }
    }
}
