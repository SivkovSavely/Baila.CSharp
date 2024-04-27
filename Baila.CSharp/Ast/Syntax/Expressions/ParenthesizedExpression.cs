using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Lexer;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record ParenthesizedExpression(
    Token LeftParenthesisToken,
    IExpression Expression,
    Token RightParenthesisToken) : IExpression
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.Merge(
        LeftParenthesisToken.Span, RightParenthesisToken.Span);

    public BailaType? GetBailaType()
    {
        return Expression.GetBailaType();
    }

    public IValue Evaluate()
    {
        return Expression.Evaluate();
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitParenthesizedExpression(this);
    }

    public string Stringify()
    {
        return $"({Expression.Stringify()})";
    }

    public override string ToString()
    {
        return $"ParenthesizedExpression({Expression})";
    }
}