using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Interpreter.ControlFlowExceptions;
using Baila.CSharp.Lexer;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record ReturnStatement(
    Token ReturnToken,
    IExpression? ReturnExpression) : IStatement
{
    public SyntaxNodeSpan Span { get; init; } = ReturnExpression != null
        ? SyntaxNodeSpan.Merge(ReturnToken, ReturnExpression)
        : ReturnToken.Span;

    public void Execute()
    {
        throw new ControlFlowReturnException(ReturnExpression?.Evaluate());
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitReturnStatement(this);
    }

    public override string ToString()
    {
        return ReturnExpression != null
            ? $"ReturnStatement(ReturnExpression={ReturnExpression})"
            : "ReturnStatement()";
    }
}