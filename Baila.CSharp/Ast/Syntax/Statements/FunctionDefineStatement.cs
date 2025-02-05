﻿using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Interpreter;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record FunctionDefineStatement(
    Token FunctionKeyword,
    Token NameIdentifier,
    Token? LeftParenthesisParameterListToken,
    List<FunctionParameter> Parameters,
    Token? RightParenthesisParameterListToken,
    BailaType? ReturnType,
    IStatement Body) : IStatement
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.MergeWithNulls(
        FunctionKeyword, NameIdentifier, LeftParenthesisParameterListToken, RightParenthesisParameterListToken, Body);

    public string Name { get; } = NameIdentifier.Value!;

    public void Execute()
    {
        // We define the function in the preprocessing stage.
        // When the code actually executes, the function define statement doesn't do anything
        // because otherwise it would try to add a function we already added in the preprocessing stage.
    }

    public void DefineFunction()
    {
        var overload = new FunctionOverload(new StatementCallable(Body), Parameters, ReturnType);

        if (!NameTable.Exists(Name))
        {
            var func = FunctionValue.WithOverload(overload, Name);
            NameTable.AddVariableInferred(Name, func);
        }
        else
        {
            var value = NameTable.Get(Name).Value;

            if (value is not FunctionValue func)
            {
                throw new Exception(
                    $"Cannot overload variable {Name} of type {value.GetBailaType()} as it is not a function");
            }

            if (FunctionValue.IsRequiredParameterAfterOptionalParameter(
                    overload,
                    out var requiredParameter,
                    out var optionalParameter))
            {
                throw new Exception($"In function '{Name}', " +
                                    $"required parameter '{requiredParameter!.Name}' " +
                                    $"cannot be after an optional parameter '{optionalParameter!.Name}'");
            }

            func.AddOverload(overload);
        }
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitFunctionDefineStatement(this);
    }

    public override string ToString()
    {
        return $"FunctionDefineStatement(Name={Name})";
    }
}