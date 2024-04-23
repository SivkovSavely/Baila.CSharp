using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Ast.Statements;

public class ExpressionStatement(IExpression expression) : IStatement
{
    public IExpression Expression { get; } = expression;

    public IValue? Value { get; set; }

    public void Execute()
    {
        Value = Expression.Evaluate();
    }

    public override string ToString()
    {
        return $"ExpressionStatement(Expression={Expression})";
    }
}