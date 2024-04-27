using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record BinaryExpression(
    BinaryExpression.Operation BinaryOperation,
    IExpression Left,
    IExpression Right) : IExpression
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.Merge(Left, Right);
    
    public readonly record struct Operation(string Op)
    {
        public static readonly Operation BitwiseOr = new("|");

        public static readonly Operation BitwiseXor = new("^");

        public static readonly Operation BitwiseAnd = new("&");

        public static readonly Operation Equality = new("==");
        public static readonly Operation Inequality = new("!=");

        public static readonly Operation LogicalOr = new("||");

        public static readonly Operation LogicalAnd = new("&&");

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
        public static readonly Operator IntIntPower = Add(new Operator(Operation.Power, BailaType.Int, BailaType.Int, BailaType.Int));
        public static readonly Operator IntIntLessThan = Add(new Operator(Operation.LessThan, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntLessThanOrEqual = Add(new Operator(Operation.LessThanOrEqual, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntGreaterThan = Add(new Operator(Operation.GreaterThan, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntGreaterThanOrEqual = Add(new Operator(Operation.GreaterThanOrEqual, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntEquality = Add(new Operator(Operation.Equality, BailaType.Int, BailaType.Int, BailaType.Bool));
        public static readonly Operator IntIntInequality = Add(new Operator(Operation.Inequality, BailaType.Int, BailaType.Int, BailaType.Bool));

        public static readonly Operator AnyNumberAddition = Add(new Operator(Operation.Addition, BailaType.Number, BailaType.Number, BailaType.Number));
        public static readonly Operator AnyNumberSubtraction = Add(new Operator(Operation.Subtraction, BailaType.Number, BailaType.Number, BailaType.Number));
        public static readonly Operator AnyNumberMultiplication = Add(new Operator(Operation.Multiplication, BailaType.Number, BailaType.Number, BailaType.Number));
        public static readonly Operator AnyNumberIntegerDivision = Add(new Operator(Operation.IntegerDivision, BailaType.Number, BailaType.Number, BailaType.Int));
        public static readonly Operator AnyNumberFloatDivision = Add(new Operator(Operation.FloatDivision, BailaType.Number, BailaType.Number, BailaType.Number));
        public static readonly Operator AnyNumberPower = Add(new Operator(Operation.Power, BailaType.Number, BailaType.Number, BailaType.Number));
        public static readonly Operator AnyNumberLessThan = Add(new Operator(Operation.LessThan, BailaType.Number, BailaType.Number, BailaType.Bool));
        public static readonly Operator AnyNumberLessThanOrEqual = Add(new Operator(Operation.LessThanOrEqual, BailaType.Number, BailaType.Number, BailaType.Bool));
        public static readonly Operator AnyNumberGreaterThan = Add(new Operator(Operation.GreaterThan, BailaType.Number, BailaType.Number, BailaType.Bool));
        public static readonly Operator AnyNumberGreaterThanOrEqual = Add(new Operator(Operation.GreaterThanOrEqual, BailaType.Number, BailaType.Number, BailaType.Bool));
        public static readonly Operator AnyNumberEquality = Add(new Operator(Operation.Equality, BailaType.Number, BailaType.Number, BailaType.Bool));
        public static readonly Operator AnyNumberInequality = Add(new Operator(Operation.Inequality, BailaType.Number, BailaType.Number, BailaType.Bool));

        public static readonly Operator BoolBoolLogicalAnd = Add(new Operator(Operation.LogicalAnd, BailaType.Bool, BailaType.Bool, BailaType.Bool));
        public static readonly Operator BoolBoolLogicalOr = Add(new Operator(Operation.LogicalOr, BailaType.Bool, BailaType.Bool, BailaType.Bool));
        public static readonly Operator BoolBoolEquality = Add(new Operator(Operation.Equality, BailaType.Bool, BailaType.Bool, BailaType.Bool));
        public static readonly Operator BoolBoolInequality = Add(new Operator(Operation.Inequality, BailaType.Bool, BailaType.Bool, BailaType.Bool));

        public static readonly Operator StringAnyConcatenation = Add(new Operator(Operation.Addition, BailaType.String, BailaType.Any, BailaType.String));
        public static readonly Operator AnyStringConcatenation = Add(new Operator(Operation.Addition, BailaType.Any, BailaType.String, BailaType.String));
        public static readonly Operator StringIntRepetition = Add(new Operator(Operation.Multiplication, BailaType.String, BailaType.Int, BailaType.String));
        public static readonly Operator IntStringRepetition = Add(new Operator(Operation.Multiplication, BailaType.Int, BailaType.String, BailaType.String));
        public static readonly Operator StringStringEquality = Add(new Operator(Operation.Equality, BailaType.String, BailaType.String, BailaType.Bool));
        public static readonly Operator StringStringInequality = Add(new Operator(Operation.Inequality, BailaType.String, BailaType.String, BailaType.Bool));

        private static Operator Add(Operator op) { All.Add(op); return op; }
    }
    
    internal static readonly Dictionary<Operator, Func<IValue, IValue, IValue>> BinaryOperators = new()
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
        [Operator.IntIntPower] =
            (left, right) => new IntValue((int)Math.Pow(left.GetAsInteger(), right.GetAsInteger())),
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
        [Operator.AnyNumberAddition] =
            (left, right) => new FloatValue(left.GetAsFloat() + right.GetAsFloat()),
        [Operator.AnyNumberSubtraction] =
            (left, right) => new FloatValue(left.GetAsFloat() - right.GetAsFloat()),
        [Operator.AnyNumberMultiplication] =
            (left, right) => new FloatValue(left.GetAsFloat() * right.GetAsFloat()),
        [Operator.AnyNumberIntegerDivision] =
            (left, right) => new IntValue((int)(left.GetAsFloat() / right.GetAsFloat())),
        [Operator.AnyNumberFloatDivision] =
            (left, right) => new FloatValue(left.GetAsFloat() / right.GetAsFloat()),
        [Operator.AnyNumberPower] =
            (left, right) => new FloatValue(Math.Pow(left.GetAsFloat(), right.GetAsFloat())),
        [Operator.AnyNumberLessThan] =
            (left, right) => new BooleanValue(left.GetAsFloat() < right.GetAsFloat()),
        [Operator.AnyNumberLessThanOrEqual] =
            (left, right) => new BooleanValue(left.GetAsFloat() <= right.GetAsFloat()),
        [Operator.AnyNumberGreaterThan] =
            (left, right) => new BooleanValue(left.GetAsFloat() > right.GetAsFloat()),
        [Operator.AnyNumberGreaterThanOrEqual] =
            (left, right) => new BooleanValue(left.GetAsFloat() >= right.GetAsFloat()),
        [Operator.AnyNumberEquality] =
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            (left, right) => new BooleanValue(left.GetAsFloat() == right.GetAsFloat()),
        [Operator.AnyNumberInequality] =
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            (left, right) => new BooleanValue(left.GetAsFloat() != right.GetAsFloat()),
        [Operator.BoolBoolLogicalAnd] =
            (left, right) => new BooleanValue(left.GetAsBoolean() && right.GetAsBoolean()),
        [Operator.BoolBoolLogicalOr] =
            (left, right) => new BooleanValue(left.GetAsBoolean() || right.GetAsBoolean()),
        [Operator.BoolBoolEquality] =
            (left, right) => new BooleanValue(left.GetAsBoolean() == right.GetAsBoolean()),
        [Operator.BoolBoolInequality] =
            (left, right) => new BooleanValue(left.GetAsBoolean() != right.GetAsBoolean()),
        [Operator.StringAnyConcatenation] =
            (left, right) => new StringValue(left.GetAsString() + right.GetAsString()),
        [Operator.AnyStringConcatenation] =
            (left, right) => new StringValue(left.GetAsString() + right.GetAsString()),
        [Operator.StringIntRepetition] =
            (left, right) => StringValue.FromRepeated(left.GetAsString(), right.GetAsInteger()),
        [Operator.IntStringRepetition] =
            (left, right) => StringValue.FromRepeated(right.GetAsString(), left.GetAsInteger()),
        [Operator.StringStringEquality] =
            (left, right) => new BooleanValue(left.GetAsString() == right.GetAsString()),
        [Operator.StringStringInequality] =
            (left, right) => new BooleanValue(left.GetAsString() != right.GetAsString()),
    };

    public BailaType GetBailaType()
    {
        var (op, _) = GetOperator(this);
        return op.ResultType;
    }

    public IValue Evaluate()
    {
        var op = GetOperator(this);
        var result = op.callback(Left.Evaluate(), Right.Evaluate());
        return result;
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitBinaryExpression(this);
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
        var (op, callback) = BinaryOperators.FirstOrDefault(operatorCallbackPair =>
        {
            var (op, _) = operatorCallbackPair;
            return op.Operation == expression.BinaryOperation &&
                   expression.Left.GetBailaType()!.IsImplicitlyConvertibleTo(op.LeftType) &&
                   expression.Right.GetBailaType()!.IsImplicitlyConvertibleTo(op.RightType);
        });

        if (op is null)
        {
            throw new Exception($"Cannot use the operator '{expression.BinaryOperation.Op}' on operands of types '{expression.Left.GetBailaType()}' and '{expression.Right.GetBailaType()}'");
        }

        return (op, callback);
    }
}