using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Interpreter.ExpressionInterpreters;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Interpreter;

public class FunctionWithOverloads(List<FunctionOverload> overloads)
{
    public List<FunctionOverload> Overloads { get; } = overloads;

    public IValue? Call(List<IExpression> arguments)
    {
        NameTable.PushScope();

        var overload = Overloads.First();
        
        // Load function arguments into the current scope
        var args = new BailaCallableArgs();
        for (var i = 0; i < overload.Parameters.Count; i++)
        {
            var parameter = overload.Parameters[i];
            args.AddArgument(parameter.name, arguments[i].InterpretEvaluate());
        }

        var callResult = overload.Callback.Call(args);

        NameTable.PopScope();

        return callResult;
    }
}