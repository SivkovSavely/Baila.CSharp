using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter;

public class FunctionWithOverloads(List<FunctionOverload> overloads)
{
    public List<FunctionOverload> Overloads { get; } = overloads;

    public IValue? Call(List<IExpression> arguments)
    {
        NameTable.PushScope();

        try
        {
            var args = new BailaCallableArgs();
            for (var i = 0; i < arguments.Count; i++)
            {
                args.AddArgument($"par{i}", arguments[i].Evaluate());
            }

            var overloads = FunctionValue.GetApplicableOverloads(Overloads, args.ArgumentTypes);
            if (!overloads.Any())
            {
                var argTypes = args.ArgsByIndex.Select(x => x.GetBailaType()).ToArray();
                throw new Exception(
                    $"Unable to find function overload: {string.Join(", ", argTypes.Select(x => x.ToString()))}");
            }

            var overload = overloads.First();

            // Load function arguments into the current scope
            args = new BailaCallableArgs();
            for (var i = 0; i < overload.Parameters.Count; i++)
            {
                var parameter = overload.Parameters[i];
                var value = i < arguments.Count ? arguments[i].Evaluate() : parameter.DefaultValue?.Evaluate();

                if (value == null)
                {
                    throw new Exception(
                        $"Argument for parameter {parameter.Name} is not supplied, " +
                        "and the parameter doesn't have a default value.");
                }
                
                args.AddArgument(parameter.Name, value);
                NameTable.AddVariable(parameter.Name, parameter.Type, value);
            }

            var callResult = overload.Callback.Call(args);
            return callResult;
        }
        finally
        {
            NameTable.PopScope();
        }
    }
}