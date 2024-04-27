using System.Globalization;
using Baila.CSharp.Lexer;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record FloatValueExpression(
    double Value,
    Token SourceLiteral) : IExpression
{
    public SyntaxNodeSpan Span { get; init; } = SourceLiteral.Span;
    
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