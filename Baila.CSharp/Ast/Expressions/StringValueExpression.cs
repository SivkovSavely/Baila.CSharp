using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Expressions;

public class StringValueExpression(string value) : IExpression
{
    public string Value { get; } = value;

    public BailaType GetBailaType() => BailaType.String;

    public IValue Evaluate()
    {
        return new StringValue(Value);
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitStringValueExpression(this);
    }

    public string Stringify()
    {
        return Value;
    }

    public override string ToString()
    {
        return $"StringValueExpression({Value})";
    }
}