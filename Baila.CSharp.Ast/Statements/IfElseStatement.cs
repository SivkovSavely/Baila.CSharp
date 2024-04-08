using Baila.CSharp.Ast.Expressions;

namespace Baila.CSharp.Ast.Statements;

public class IfElseStatement : IStatement
{
    public IExpression Condition { get; }
    public IStatement TrueStatement { get; }
    public IStatement? FalseStatement { get; }

    public IfElseStatement(
        IExpression condition,
        IStatement trueStatement,
        IStatement? falseStatement)
    {
        Condition = condition;
        TrueStatement = trueStatement;
        FalseStatement = falseStatement;
    }

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