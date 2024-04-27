using System.Globalization;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record FloatValueExpression(
    double Value,
    string Filename,
    SyntaxNodeSpan Span) : IExpression
{
    public BailaType GetBailaType() => BailaType.Float;

    public IValue Evaluate()
    {
        return new FloatValue(Value);
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitFloatValueExpression(this);
    }

    public string Stringify()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
        return $"FloatValueExpression({Value})";
    }
}