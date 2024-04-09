using Baila.CSharp.Ast.Statements;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class NoOpStatementInterpreter : StatementInterpreterBase<NoOpStatement>
{
    public override void Interpret(NoOpStatement statement)
    {
        throw new NotImplementedException();
    }
}