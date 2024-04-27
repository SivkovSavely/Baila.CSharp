using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record IfElseStatement(
    IExpression Condition,
    IStatement TrueStatement,
    IStatement? FalseStatement)
    : IStatement
{

    public void Execute()
    {
        if (Condition.Evaluate().GetAsBoolean())
        {
            TrueStatement.Execute();
        }
        else
        {
            FalseStatement?.Execute();
        }
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitIfElseStatement(this);
    }

    public override string ToString()
    {
        return $"IfElseStatement(Condition={Condition}, TrueStatement={TrueStatement}" +
               (FalseStatement == null ? "" : $", FalseStatement={FalseStatement}") +
               ")";
    }
}