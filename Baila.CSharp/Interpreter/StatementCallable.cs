using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.ControlFlowExceptions;
using Baila.CSharp.Interpreter.StatementInterpreters;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Interpreter;

public class StatementCallable(IStatement statement) : IBailaCallable
{
    public IStatement Statement { get; } = statement;

    public IValue? Call(BailaCallableArgs args)
    {
        try
        {
            Statement.InterpretExecute();
        }
        catch (ControlFlowReturnException returnException)
        {
            return returnException.Value;
        }

        return null; // TODO return something on ReturnStatement
    }
}