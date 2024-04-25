using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Expressions;

public class BoolValueExpression(bool value) : IExpression
{
    public bool Value { get; } = value;

    public BailaType GetBailaType() => BailaType.Bool;

    public IValue Evaluate()
    {
        return new BooleanValue(Value);
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitBoolValueExpression(this);
    }

    public string Stringify()
    {
        return Value ? "true" : "false";
    }

    public override string ToString()
    {
        return $"BoolValueExpression({Value})";
    }
}