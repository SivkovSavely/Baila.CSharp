using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Interpreter.ControlFlowExceptions;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter;

public class StatementCallable(IStatement statement) : IBailaCallable
{
    public IStatement Statement { get; } = statement;

    public IValue? Call(BailaCallableArgs args)
    {
        try
        {
            Statement.Execute();
        }
        catch (ControlFlowReturnException returnException)
        {
            return returnException.Value;
        }

        return null; // TODO return something on ReturnStatement
    }
}