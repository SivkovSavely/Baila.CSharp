using Baila.CSharp.Ast.Expressions;

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
        throw new NotImplementedException("For loop is not implemented yet");
    }

    public override string ToString()
    {
        return $"WhileStatement(" +
               $"Condition={Condition}, " +
               $"Body={Body})";
    }
}