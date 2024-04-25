﻿using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Interpreter;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public class FunctionDefineStatement(
    string name,
    List<FunctionParameter> parameters,
    IStatement body,
    BailaType? returnType) : IStatement
{
    public string Name { get; } = name;
    public List<FunctionParameter> Parameters { get; } = parameters;
    public IStatement Body { get; } = body;
    public BailaType? ReturnType { get; } = returnType;

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

            if (func.HasOverload(overload))
            {
                var pars = string.Join(", ", overload.Parameters.Select(x => x.ToString()));
                throw new Exception($"Overload with parameters {pars} already exists");
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