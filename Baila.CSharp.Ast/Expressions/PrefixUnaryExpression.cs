namespace Baila.CSharp.Ast.Expressions;

public class PrefixUnaryExpression(PrefixUnaryExpression.Operation operation, IExpression expr) : IExpression
{
    public readonly record struct Operation(string Op)
    {
        public static readonly Operation BitwiseNegation = new("~");
        public static readonly Operation LogicalNegation = new("!");
        public static readonly Operation Plus = new("+");
        public static readonly Operation Minus = new("-");

        public override string ToString() => Op;
    }

    public string Stringify()
    {
        return $"{operation} {expr.Stringify()}";
    }

    public override string ToString()
    {
        return $"PrefixUnaryExpression({operation} {expr})";
    }
}