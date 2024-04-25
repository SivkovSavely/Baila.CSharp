using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public class DoWhileStatement(
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