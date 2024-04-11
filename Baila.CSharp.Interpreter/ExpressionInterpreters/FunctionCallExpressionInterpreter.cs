using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class FunctionCallExpressionInterpreter : ExpressionInterpreterBase<FunctionCallExpression>
{
    public override IValue Interpret(FunctionCallExpression expression)
    {
        IValue value;

        if (expression.FunctionHolder is VariableExpression variableExpression)
        {
            var name = variableExpression.Name;
            var memory = NameTable.Get(name);
            value = memory.Value;
        }
        else
        {
            value = expression.FunctionHolder.InterpretEvaluate();
        }

        var functionValueType = value.GetBailaType();
        if (functionValueType != BailaType.Function) // TODO check for Type
        {
            throw new InvalidOperationException($"Cannot call {functionValueType.ClassName} as it is not a function");
        }

        List<FunctionOverload> availableOverloads = [];
        var functionValue = (value as FunctionValue)!;
        availableOverloads.AddRange(functionValue.Overloads);

        return new FunctionWithOverloads(availableOverloads).Call(expression.CallArgs); // TODO what to do with void functions?
    }
}