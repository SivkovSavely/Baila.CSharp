using Baila.CSharp.Ast.Values;

namespace Baila.CSharp.Ast.Expressions;

public class BinaryExpression(BinaryExpression.Operation operation, IExpression left, IExpression right) : IExpression
{
    public readonly record struct Operation(string Op)
    {
        public static readonly Operation BitwiseOr = new("|");

        public static readonly Operation BitwiseXor = new("^");

        public static readonly Operation BitwiseAnd = new("&");

        public static readonly Operation Equality = new("==");
        public static readonly Operation Inequality = new("!=");

        public static readonly Operation LessThan = new("<");
        public static readonly Operation LessThanOrEqual = new("<=");
        public static readonly Operation GreaterThan = new(">");
        public static readonly Operation GreaterThanOrEqual = new(">=");

        public static readonly Operation Addition = new("+");
        public static readonly Operation Subtraction = new("-");

        public static readonly Operation FloatDivision = new("/");
        public static readonly Operation IntegerDivision = new("//");
        public static readonly Operation Multiplication = new("*");

        public static readonly Operation Power = new("**");

        public override string ToString() => Op;
    }

    public IValue Evaluate()
    {
        // TODO assign to variable
        return null;
    }

    public string Stringify()
    {
        return $"{left.Stringify()} {operation} {right.Stringify()}";
    }
}