using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
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