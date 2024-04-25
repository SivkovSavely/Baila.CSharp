using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public class WhileStatement(
    IExpression condition,
    IStatement body)
    : IStatement
{
    public IExpression Condition { get; } = condition;
    public IStatement Body { get; } = body;

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