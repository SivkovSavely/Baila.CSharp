using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.ExpressionInterpreters;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class ForStatementInterpreter : StatementInterpreterBase<ForStatement>
{
    public override void Interpret(ForStatement statement)
    {
        NameTable.PushScope();

        var initial = statement.InitialValue.InterpretEvaluate();
        var final = statement.FinalValue.InterpretEvaluate();
        var step = statement.StepValue.InterpretEvaluate();

        var loopType = initial.GetBailaType() == BailaType.Int && final.GetBailaType() == BailaType.Int &&
                       step.GetBailaType() == BailaType.Int
            ? BailaType.Int
            : BailaType.Float;

        NameTable.AddVariable(statement.CounterVariable, loopType, initial);
        var v = NameTable.Get(statement.CounterVariable);

        if (loopType == BailaType.Int)
        {
            if (initial.GetAsInteger() <= final.GetAsInteger())
            {
                var increment = step.GetAsInteger();
                while (v.Value.GetAsInteger() <= final.GetAsInteger())
                {
                    statement.Body.InterpretExecute();
                    NameTable.Set(statement.CounterVariable, new IntValue(v.Value.GetAsInteger() + increment));
                }
            }
            else
            {
                var increment = -step.GetAsInteger();
                while (v.Value.GetAsInteger() >= final.GetAsInteger())
                {
                    statement.Body.InterpretExecute();
                    NameTable.Set(statement.CounterVariable, new IntValue(v.Value.GetAsInteger() + increment));
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
                    statement.Body.InterpretExecute();
                    NameTable.Set(statement.CounterVariable, new FloatValue(v.Value.GetAsFloat() + increment));
                }
            }
            else
            {
                var increment = -step.GetAsFloat();
                while (v.Value.GetAsFloat() >= final.GetAsFloat())
                {
                    statement.Body.InterpretExecute();
                    NameTable.Set(statement.CounterVariable, new FloatValue(v.Value.GetAsFloat() + increment));
                }
            }
        }

        NameTable.PopScope();
    }
}