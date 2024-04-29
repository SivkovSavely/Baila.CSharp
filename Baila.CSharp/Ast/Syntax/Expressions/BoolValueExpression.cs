using Baila.CSharp.Lexer;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record BoolValueExpression(
    bool Value,
    Token SourceLiteral) : ValueExpression(SourceLiteral)
{
    public SyntaxNodeSpan Span { get; init; } = SourceLiteral.Span;
    
    public override BailaType GetBailaType() => BailaType.Bool;

    public override IValue Evaluate()
    {
        return new BooleanValue(Value);
    }

    public override void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitBoolValueExpression(this);
    }

    public override string Stringify()
    {
        return Value ? "true" : "false";
    }

    public override string ToString()
    {
        return $"BoolValueExpression({Value})";
    }
}