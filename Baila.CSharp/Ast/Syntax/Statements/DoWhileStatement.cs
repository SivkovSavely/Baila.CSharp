using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record DoWhileStatement(
    Token DoToken,
    IStatement Body,
    Token WhileToken,
    IExpression Condition) : IStatement
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.Merge(
        DoToken, Body, WhileToken, Condition);

    public void Execute()
    {
        NameTable.PushScope();

        var condition = Condition.Evaluate();
        do
        {
            Body.Execute();
        } while (condition.GetAsBoolean());
        
        NameTable.PopScope();
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitDoWhileStatement(this);
    }

    public override string ToString()
    {
        return $"DoWhileStatement(" +
               $"Condition={Condition}, " +
               $"Body={Body})";
    }
}