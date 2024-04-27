using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record WhileStatement(
    IExpression Condition,
    IStatement Body,
    string Filename,
    SyntaxNodeSpan Span) : IStatement
{
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