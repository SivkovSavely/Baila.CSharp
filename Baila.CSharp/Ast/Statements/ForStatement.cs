﻿using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public record ForStatement(
    string CounterVariable,
    IExpression InitialValue,
    IExpression FinalValue,
    IExpression StepValue,
    IStatement Body)
    : IStatement
{
    public void Execute()
    {
        NameTable.PushScope();

        var initial = InitialValue.Evaluate();
        var final = FinalValue.Evaluate();
        var step = StepValue.Evaluate();

        var loopType = initial.GetBailaType() == BailaType.Int &&
                       final.GetBailaType() == BailaType.Int &&
                       step.GetBailaType() == BailaType.Int
            ? BailaType.Int
            : BailaType.Float;

        NameTable.AddVariable(CounterVariable, loopType, initial);
        var v = NameTable.Get(CounterVariable);

        if (loopType == BailaType.Int)
        {
            if (initial.GetAsInteger() <= final.GetAsInteger())
            {
                var increment = step.GetAsInteger();
                while (v.Value.GetAsInteger() <= final.GetAsInteger())
                {
                    Body.Execute();
                    NameTable.Set(CounterVariable, new IntValue(v.Value.GetAsInteger() + increment));
                }
            }
            else
            {
                var increment = -step.GetAsInteger();
                while (v.Value.GetAsInteger() >= final.GetAsInteger())
                {
                    Body.Execute();
                    NameTable.Set(CounterVariable, new IntValue(v.Value.GetAsInteger() + increment));
                }
            }
        }
        else
        {
            if (initial.GetAsFloat() <= final.GetAsFloat())
            {
                var increment = step.GetAsFloat();
                while (v.Value.GetAsFloat() <= final.GetAsFloat())
                {
                    Body.Execute();
                    NameTable.Set(CounterVariable, new FloatValue(v.Value.GetAsFloat() + increment));
                }
            }
            else
            {
                var increment = -step.GetAsFloat();
                while (v.Value.GetAsFloat() >= final.GetAsFloat())
                {
                    Body.Execute();
                    NameTable.Set(CounterVariable, new FloatValue(v.Value.GetAsFloat() + increment));
                }
            }
        }

        NameTable.PopScope();
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitForStatement(this);
    }

    public override string ToString()
    {
        return $"ForStatement(" +
               $"CounterVariable={CounterVariable}, " +
               $"InitialValue={InitialValue}, " +
               $"FinalValue = {FinalValue}, " +
               $"StepValue = {StepValue}, " +
               $"Body = {Body})";
    }
}