using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public record BinaryExpression(BinaryExpression.Operation BinaryOperation, IExpression Left, IExpression Right) : IExpression
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

    public record Operator(Operation Operation, BailaType LeftType, BailaType RightType, BailaType ResultType)
    {
        public static readonly List<Operator> All = [];

        public static readonly Operator IntIntAddition = Add(new Operator(Operation.Addition, BailaType.Int, BailaType.Int, BailaType.Int));

        private static Operator Add(Operator op) { All.Add(op); return op; }
    }

    public BailaType? GetBailaType()
    {
        var op = Operator.All.FirstOrDefault(op =>
            BinaryOperation == op.Operation &&
            op.LeftType == Left.GetBailaType() &&
            op.RightType == Right.GetBailaType());

        return op?.ResultType;
    }

    public string Stringify()
    {
        return $"{Left.Stringify()} {BinaryOperation} {Right.Stringify()}";
    }

    public override string ToString()
    {
        return $"BinaryExpression({Left} {BinaryOperation} {Right})";
    }
}