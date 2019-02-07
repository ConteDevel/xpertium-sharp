namespace XpertiumSharp.Logic
{
    public enum XOperand
    {
        Fact,
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
        public XFact Fact { get; private set; }
        public XOperand Type => XOperand.Fact;

        public XExpression(XFact fact)
        {
            Fact = fact;
        }

        public override string ToString()
        {
            return Fact.ToString();
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

        public override string ToString()
        {
            return "or(" + Left.ToString() + "," + Right.ToString() + ")";
        }
    }
}
