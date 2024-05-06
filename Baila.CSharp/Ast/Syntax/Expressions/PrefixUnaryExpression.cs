using System.Diagnostics.CodeAnalysis;
using Baila.CSharp.Lexer;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record PrefixUnaryExpression(
    PrefixUnaryExpression.Operation Op,
    Token OperatorToken,
    IExpression OperandExpression) : IExpression
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.Merge(OperatorToken, OperandExpression);

    public readonly record struct Operation(string Op)
    {
        public static readonly Operation BitwiseNegation = new("~");
        public static readonly Operation LogicalNegation = new("!");
        public static readonly Operation Plus = new("+");
        public static readonly Operation Minus = new("-");

        public override string ToString() => Op;
    }

    public record Operator(Operation Operation, BailaType OperandType, BailaType ResultType)
    {
        public static readonly List<Operator> All = [];
        
        public static readonly Operator IntPlus = Add(new Operator(Operation.Plus, BailaType.Int, BailaType.Int));
        public static readonly Operator IntMinus = Add(new Operator(Operation.Minus, BailaType.Int, BailaType.Int));
        public static readonly Operator AnyNumberPlus = Add(new Operator(Operation.Plus, BailaType.Number, BailaType.Number));
        public static readonly Operator AnyNumberMinus = Add(new Operator(Operation.Minus, BailaType.Number, BailaType.Number));
        public static readonly Operator IntBitwiseNegation = Add(new Operator(Operation.BitwiseNegation, BailaType.Int, BailaType.Int));

        public static readonly Operator BoolLogicalNegation = Add(new Operator(Operation.LogicalNegation, BailaType.Bool, BailaType.Bool));
        
        private static Operator Add(Operator op) { All.Add(op); return op; }
    }

    internal static readonly Dictionary<Operator, Func<IValue, IValue>> UnaryOperators = new()
    {
        [Operator.IntPlus] = op => new IntValue(+op.GetAsInteger()),
        [Operator.IntMinus] = op => new IntValue(-op.GetAsInteger()),
        [Operator.AnyNumberPlus] = op => new FloatValue(+op.GetAsFloat()),
        [Operator.AnyNumberMinus] = op => new FloatValue(-op.GetAsFloat()),
        [Operator.IntBitwiseNegation] = op => new IntValue(~op.GetAsInteger()),
        [Operator.BoolLogicalNegation] = op => new BooleanValue(!op.GetAsBoolean()),
    };

    public BailaType? GetBailaType()
    {
        return OperandExpression.GetBailaType();
    }

    public IValue Evaluate()
    {
        var op = GetOperator(this);
        var result = op.callback(OperandExpression.Evaluate());
        return result;
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitPrefixUnaryExpression(this);
    }

    [ExcludeFromCodeCoverage]
    public string Stringify()
    {
        return $"{Op} {OperandExpression.Stringify()}";
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"PrefixUnaryExpression({Op} {OperandExpression})";
    }

    private (Operator op, Func<IValue, IValue> callback) GetOperator(PrefixUnaryExpression expression)
    {
        var (op, callback) = UnaryOperators.FirstOrDefault(operatorCallbackPair =>
        {
            var (op, _) = operatorCallbackPair;
            return op.Operation == expression.Op &&
                   expression.OperandExpression.GetBailaType()!.IsImplicitlyConvertibleTo(op.OperandType);
        });

        if (op is null)
        {
            throw new Exception($"Cannot use the operator '{expression.Op.Op}' on an operand of type '{expression.OperandExpression.GetBailaType()}'");
        }

        return (op, callback);
    }
}