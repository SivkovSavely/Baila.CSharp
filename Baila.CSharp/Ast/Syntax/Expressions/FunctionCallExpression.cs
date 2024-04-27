using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Interpreter;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record FunctionCallExpression(IExpression FunctionHolder, List<IExpression> CallArgs) : IExpression
{
    public BailaType? GetBailaType()
    {
        if (FunctionHolder is VariableExpression variableExpression)
        {
            var member = NameTable.Get(variableExpression.Name);
            var functionValue = member.Value as FunctionValue;
            var overload = FunctionValue.GetApplicableOverloads(
                functionValue.Overloads,
                CallArgs.Select(x => x.GetBailaType()).ToArray());
            return overload.First().ReturnType;
        }
        return FunctionHolder.GetBailaType();
    }

    public IValue Evaluate()
    {
        IValue value;

        if (FunctionHolder is VariableExpression variableExpression)
        {
            var name = variableExpression.Name;
            var memory = NameTable.Get(name);
            value = memory.Value;
        }
        else
        {
            value = FunctionHolder.Evaluate();
        }

        var functionValueType = value.GetBailaType();
        if (functionValueType != BailaType.Function) // TODO check for Type
        {
            throw new InvalidOperationException($"Cannot call {functionValueType.ClassName} as it is not a function");
        }

        List<FunctionOverload> availableOverloads = [];
        var functionValue = (value as FunctionValue)!;
        availableOverloads.AddRange(functionValue.Overloads);

        return new FunctionWithOverloads(availableOverloads).Call(CallArgs)!; // TODO what to do with void functions?
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitFunctionCallExpression(this);
    }

    public string Stringify()
    {
        return $"{FunctionHolder.Stringify()}({string.Join(", ", CallArgs.Select(x => x.Stringify()))})";
    }
}