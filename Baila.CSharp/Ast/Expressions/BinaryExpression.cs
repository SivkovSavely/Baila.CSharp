using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
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
        public static readonly Operator IntIntSubtraction = Add(new Operator(Operation.Subtraction, BailaType.Int, BailaType.Int, BailaType.Int));
        public static readonly Operator IntIntMultiplication = Add(new Operator(Operation.Multiplication, BailaType.Int, BailaType.Int, BailaType.Int));
        public static readonly Operator IntIntIntegerDivision = Add(new Operator(Operation.IntegerDivision, BailaType.Int, BailaType.Int, BailaType.Int));
        public static readonly Operator IntIntFloatDivision = Add(new Operator(Operation.FloatDivision, BailaType.Int, BailaType.Int, BailaType.Float));
        public static readonly Operator IntIntLessThan = Add(new Operator(Operation.LessThan, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntLessThanOrEqual = Add(new Operator(Operation.LessThanOrEqual, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntGreaterThan = Add(new Operator(Operation.GreaterThan, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntGreaterThanOrEqual = Add(new Operator(Operation.GreaterThanOrEqual, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntEquality = Add(new Operator(Operation.Equality, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntInequality = Add(new Operator(Operation.Inequality, BailaType.Int, BailaType.Int, BailaType.Bool));

        private static Operator Add(Operator op) { All.Add(op); return op; }
    }
    
    private static readonly Dictionary<Operator, Func<IValue, IValue, IValue>> BinaryOperators = new()
    {
        [Operator.IntIntAddition] =
            (left, right) => new IntValue(left.GetAsInteger() + right.GetAsInteger()),
        [Operator.IntIntSubtraction] =
            (left, right) => new IntValue(left.GetAsInteger() - right.GetAsInteger()),
        [Operator.IntIntMultiplication] =
            (left, right) => new IntValue(left.GetAsInteger() * right.GetAsInteger()),
        [Operator.IntIntIntegerDivision] =
            (left, right) => new IntValue(left.GetAsInteger() / right.GetAsInteger()),
        [Operator.IntIntFloatDivision] =
            (left, right) => new FloatValue((float)left.GetAsInteger() / right.GetAsInteger()),
        [Operator.IntIntLessThan] =
            (left, right) => new BooleanValue(left.GetAsInteger() < right.GetAsInteger()),
        [Operator.IntIntLessThanOrEqual] =
            (left, right) => new BooleanValue(left.GetAsInteger() <= right.GetAsInteger()),
        [Operator.IntIntGreaterThan] =
            (left, right) => new BooleanValue(left.GetAsInteger() > right.GetAsInteger()),
        [Operator.IntIntGreaterThanOrEqual] =
            (left, right) => new BooleanValue(left.GetAsInteger() >= right.GetAsInteger()),
        [Operator.IntIntEquality] =
            (left, right) => new BooleanValue(left.GetAsInteger() == right.GetAsInteger()),
        [Operator.IntIntInequality] =
            (left, right) => new BooleanValue(left.GetAsInteger() != right.GetAsInteger()),
    };

    public BailaType? GetBailaType()
    {
        var op = Operator.All.FirstOrDefault(op =>
            BinaryOperation == op.Operation &&
            op.LeftType == Left.GetBailaType() &&
            op.RightType == Right.GetBailaType());

        return op?.ResultType;
    }

    public IValue Evaluate()
    {
        var op = GetOperator(this);
        var result = op.callback(Left.Evaluate(), Right.Evaluate());
        return result;
    }

    public string Stringify()
    {
        return $"{Left.Stringify()} {BinaryOperation} {Right.Stringify()}";
    }

    public override string ToString()
    {
        return $"BinaryExpression({Left} {BinaryOperation} {Right})";
    }
    
    private (Operator op, Func<IValue, IValue, IValue> callback) GetOperator(BinaryExpression expression)
    {
        var (op, callback) = BinaryOperators.FirstOrDefault(op =>
            op.Key.Operation == expression.BinaryOperation &&
            op.Key.LeftType == expression.Left.GetBailaType() &&
            op.Key.RightType == expression.Right.GetBailaType());

        if (op is null)
        {
            throw new Exception($"Cannot use the operator '{expression.BinaryOperation.Op}' on operands of types '{expression.Left.GetBailaType()}' and '{expression.Right.GetBailaType()}'");
        }

        return (op, callback);
    }
}