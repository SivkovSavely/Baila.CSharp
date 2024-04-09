using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.StatementInterpreters;

namespace Baila.CSharp.Interpreter.Factories;

public class StatementInterpreterFactory
{
    public static StatementInterpreterBase Create(IStatement statement)
    {
        return statement switch
        {
            _ => throw new InvalidOperationException(
                $"Statement is of invalid type: {statement.GetType().FullName}")
        };
    }
}