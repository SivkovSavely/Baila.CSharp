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
        
        var args = new BailaCallableArgs();
        for (var i = 0; i < arguments.Count; i++)
        {
            args.AddArgument($"par{i}", arguments[i].InterpretEvaluate());
        }

        var overloads = GetOverloads(args);
        if (!overloads.Any())
        {
            var argTypes = args.ArgsByIndex.Select(x => x.GetBailaType()).ToArray();
            throw new Exception($"Unable to find function overload: {string.Join(", ", argTypes.Select(x => x.ToString()))}");
        }
        var overload = overloads.First();
        
        // Load function arguments into the current scope
        args = new BailaCallableArgs();
        for (var i = 0; i < overload.Parameters.Count; i++)
        {
            var parameter = overload.Parameters[i];
            var value = arguments[i].InterpretEvaluate();
            args.AddArgument(parameter.Name, value);
            NameTable.AddVariable(parameter.Name, parameter.Type, value);
        }

        var callResult = overload.Callback.Call(args);

        NameTable.PopScope();

        return callResult;
    }

    private List<FunctionOverload> GetOverloads(BailaCallableArgs args)
    {
        var argTypes = args.ArgsByIndex.Select(x => x.GetBailaType()).ToArray();
        var found = new List<FunctionOverload>();

        foreach (var overload in Overloads)
        {
            // If we passed fewer arguments than the required parameters count, skip that overload
            if (argTypes.Length < overload.Parameters.Count(par => par.DefaultValue == null))
            {
                continue;
            }

            // If we passed more arguments than the total parameters count, skip that overload
            if (argTypes.Length > overload.Parameters.Count)
            {
                continue;
            }
            
            // Best match
            if (argTypes.SequenceEqual(overload.Parameters.Select(par => par.Type)))
            {
                found.Add(overload);
                break;
            }
        }

        return found;
    }
}