using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.ControlFlowExceptions;

namespace Baila.CSharp.Ast.Statements;

public class ReturnStatement(IExpression? returnExpression = null) : IStatement
{
    public IExpression? ReturnExpression { get; } = returnExpression;

    public void Execute()
    {
        throw new ControlFlowReturnException(ReturnExpression?.Evaluate());
    }

    public override string ToString()
    {
        return ReturnExpression != null
            ? $"ReturnStatement(ReturnExpression={ReturnExpression})"
            : "ReturnStatement()";
    }
}