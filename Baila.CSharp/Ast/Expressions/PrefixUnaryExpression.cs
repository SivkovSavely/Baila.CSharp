using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Expressions;

public class PrefixUnaryExpression(PrefixUnaryExpression.Operation op, IExpression operandExpression) : IExpression
{
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
        public static readonly Operator IntBitwiseNegation = Add(new Operator(Operation.BitwiseNegation, BailaType.Int, BailaType.Int));

        public static readonly Operator BoolLogicalNegation = Add(new Operator(Operation.LogicalNegation, BailaType.Bool, BailaType.Bool));
        
        private static Operator Add(Operator op) { All.Add(op); return op; }
    }

    private static readonly Dictionary<Operator, Func<IValue, IValue>> UnaryOperators = new()
    {
        [Operator.IntPlus] = op => new IntValue(+op.GetAsInteger()),
        [Operator.IntMinus] = op => new IntValue(-op.GetAsInteger()),
        [Operator.IntBitwiseNegation] = op => new IntValue(~op.GetAsInteger()),
        [Operator.BoolLogicalNegation] = op => new BooleanValue(!op.GetAsBoolean()),
    };

    public PrefixUnaryExpression.Operation Op { get; } = op;
    public IExpression OperandExpression { get; } = operandExpression;

    public BailaType? GetBailaType()
    {
        return OperandExpression.GetBailaType();
    }

    public IValue Evaluate()
    {
        throw new NotImplementedException();
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitPrefixUnaryExpression(this);
    }

    public string Stringify()
    {
        return $"{Op} {OperandExpression.Stringify()}";
    }

    public override string ToString()
    {
        return $"PrefixUnaryExpression({Op} {OperandExpression})";
    }
}