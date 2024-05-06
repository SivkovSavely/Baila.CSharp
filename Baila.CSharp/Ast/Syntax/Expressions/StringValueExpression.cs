using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record StringValueExpression(
    string Value,
    Token? LiteralToken = null) : ValueExpression(LiteralToken ?? new Token("", SyntaxNodeSpan.Empty, TokenType.StringLiteral, ""))
{
    public override SyntaxNodeSpan Span { get; init; } = LiteralToken?.Span ?? SyntaxNodeSpan.Empty;
    
    public override BailaType GetBailaType() => BailaType.String;

    public override IValue Evaluate()
    {
        return new StringValue(Value);
    }

    public override void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitStringValueExpression(this);
    }

    public override string Stringify()
    {
        return Value;
    }

    public override string ToString()
    {
        return $"StringValueExpression({Value})";
    }
}