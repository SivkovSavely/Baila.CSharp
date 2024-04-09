using Baila.CSharp.Ast.Statements;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class BlockStatementInterpreter : StatementInterpreterBase<BlockStatement>
{
    public override void Interpret(BlockStatement statement)
    {
        foreach (var stmt in statement.Statements)
        {
            StatementInterpreter.Interpret(stmt);
        }
    }
}