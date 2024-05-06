using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record WhileStatement(
    Token WhileToken,
    IExpression Condition,
    IStatement Body) : IStatement
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.Merge(WhileToken, Condition, Body); 

    public void Execute()
    {
        NameTable.PushScope();

        var condition = Condition.Evaluate();
        while (condition.GetAsBoolean())
        {
            Body.Execute();
        }
        
        NameTable.PopScope();
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitWhileStatement(this);
    }

    public override string ToString()
    {
        return $"WhileStatement(" +
               $"Condition={Condition}, " +
               $"Body={Body})";
    }
}