using Baila.CSharp.Ast.Expressions;

namespace Baila.CSharp.Ast.Statements;

public class ReturnStatement(IExpression? returnExpression = null) : IStatement
{
    public IExpression? ReturnExpression { get; } = returnExpression;

    public override string ToString()
    {
        return ReturnExpression != null
            ? $"ReturnStatement(ReturnExpression={ReturnExpression})"
            : "ReturnStatement()";
    }
}