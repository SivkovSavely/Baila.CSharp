using Baila.CSharp.Ast.Expressions;

namespace Baila.CSharp.Ast.Statements;

public class IfElseStatement(
    IExpression condition,
    IStatement trueStatement,
    IStatement? falseStatement)
    : IStatement
{
    public IExpression Condition { get; } = condition;
    public IStatement TrueStatement { get; } = trueStatement;
    public IStatement? FalseStatement { get; } = falseStatement;

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

    public override string ToString()
    {
        return $"IfElseStatement(Condition={Condition}, TrueStatement={TrueStatement}" +
               (FalseStatement == null ? "" : $", FalseStatement={FalseStatement}") +
               ")";
    }
}