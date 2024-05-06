using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter.ControlFlowExceptions;

public class ControlFlowReturnException(IValue? value) : Exception
{
    public IValue? Value { get; } = value;
}