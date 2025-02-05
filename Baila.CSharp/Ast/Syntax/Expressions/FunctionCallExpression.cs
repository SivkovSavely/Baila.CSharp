﻿using System.Diagnostics.CodeAnalysis;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Interpreter;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record FunctionCallExpression(
    IExpression FunctionHolder,
    Token LeftParenthesisToken,
    List<IExpression> CallArgs,
    Token? RightParenthesisToken) : IExpression
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.MergeWithNulls(
        [FunctionHolder, LeftParenthesisToken, ..CallArgs, RightParenthesisToken]);

    public BailaType? GetBailaType()
    {
        if (FunctionHolder is VariableExpression variableExpression)
        {
            /*var member = NameTable.Get(variableExpression.Name);
            var functionValue = member.Value as FunctionValue;
            var overload = FunctionValue.GetApplicableOverloads(
                functionValue.Overloads,
                CallArgs.Select(x => x.GetBailaType()).ToArray());
            return overload.First().ReturnType;*/
            var function = CompileTimeNameTable.GetFunction(variableExpression.Name);
            if (function == null) return null;
            var overload = FunctionValue.GetApplicableOverloads(
                function.Overloads,
                CallArgs.Select(arg => arg.GetBailaType()!).ToArray());
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

    [ExcludeFromCodeCoverage]
    public string Stringify()
    {
        return $"{FunctionHolder.Stringify()}({string.Join(", ", CallArgs.Select(x => x.Stringify()))})";
    }
}