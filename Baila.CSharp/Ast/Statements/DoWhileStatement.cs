using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public record DoWhileStatement(IExpression Condition, IStatement Body) : IStatement
{

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