using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record StringValueExpression(string Value) : IExpression
{
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