﻿using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class VariableExpressionInterpreter : ExpressionInterpreterBase<VariableExpression>
{
    public override IValue Interpret(VariableExpression expression)
    {
        throw new NotImplementedException();
    }
}