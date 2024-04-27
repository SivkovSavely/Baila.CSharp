using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record StringValueExpression(
    string Value,
    string Filename,
    SyntaxNodeSpan Span) : IExpression
{
    public BailaType GetBailaType() => BailaType.String;

    public IValue Evaluate()
    {
        return new StringValue(Value);
    }

    public static StringValueExpression CreateVirtual(string value) => new(value, "", SyntaxNodeSpan.Empty);

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