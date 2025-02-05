﻿using System.Diagnostics.CodeAnalysis;
using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
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

    [ExcludeFromCodeCoverage]
    public string Stringify()
    {
        return $"({Expression.Stringify()})";
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"ParenthesizedExpression({Expression})";
    }
}