using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.ExpressionInterpreters;
using Baila.CSharp.Interpreter.Stdlib;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class WhileStatementInterpreter : StatementInterpreterBase<WhileStatement>
{
    public override void Interpret(WhileStatement statement)
    {
        NameTable.PushScope();

        var condition = statement.Condition.InterpretEvaluate();
        while (condition.GetAsBoolean())
        {
            statement.Body.InterpretExecute();
        }
        
        NameTable.PopScope();
    }
}