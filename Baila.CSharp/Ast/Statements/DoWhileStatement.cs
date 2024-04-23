using Baila.CSharp.Ast.Expressions;

namespace Baila.CSharp.Ast.Statements;

public class DoWhileStatement(
    IExpression condition,
    IStatement body)
    : IStatement
{
    public IExpression Condition { get; } = condition;
    public IStatement Body { get; } = body;

    public override string ToString()
    {
        return $"DoWhileStatement(" +
               $"Condition={Condition}, " +
               $"Body={Body})";
    }
}