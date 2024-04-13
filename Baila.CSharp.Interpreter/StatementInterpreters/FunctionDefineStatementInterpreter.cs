using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class FunctionDefineStatementInterpreter : StatementInterpreterBase<FunctionDefineStatement>
{
    public override void Interpret(FunctionDefineStatement statement)
    {
        var overload = new FunctionOverload(
            new StatementCallable(statement.Body),
            statement.Parameters.Select(par => new FunctionParameter(par.Name, par.Type, par.DefaultValue, par.Vararg)).ToList(),
            statement.ReturnType);

        if (!NameTable.Exists(statement.Name))
        {
            var func = FunctionValue.WithOverload(overload, statement.Name);
            NameTable.AddVariableInferred(statement.Name, func);
        }
        else
        {
            var value = NameTable.Get(statement.Name).Value;

            if (value is not FunctionValue func)
            {
                throw new Exception(
                    $"Cannot overload variable {statement.Name} of type {value.GetBailaType()} as it is not a function");
            }

            if (func.HasOverload(overload))
            {
                var pars = string.Join(", ", overload.Parameters.Select(x => x.ToString()));
                throw new Exception($"Overload with parameters {pars} already exists");
            }

            func.AddOverload(overload);
        }
    }
}