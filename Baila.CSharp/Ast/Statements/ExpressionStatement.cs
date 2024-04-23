using Baila.CSharp.Ast.Expressions;

namespace Baila.CSharp.Ast.Statements;

public class ExpressionStatement(IExpression expression) : IStatement
{
    public IExpression Expression { get; } = expression;

    public void Execute()
    {
        Expression.Evaluate();
    }

    public override string ToString()
    {
        return $"ExpressionStatement(Expression={Expression})";
    }
}