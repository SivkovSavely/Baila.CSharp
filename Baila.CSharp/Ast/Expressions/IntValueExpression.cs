using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Expressions;

public record IntValueExpression(long Value) : IExpression
{
    public BailaType GetBailaType() => BailaType.Int;

    public IValue Evaluate()
    {
        return new IntValue(Value);
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitIntValueExpression(this);
    }

    public string Stringify()
    {
        return Value.ToString();
    }

    public override string ToString()
    {
        return $"IntValueExpression({Value})";
    }
}