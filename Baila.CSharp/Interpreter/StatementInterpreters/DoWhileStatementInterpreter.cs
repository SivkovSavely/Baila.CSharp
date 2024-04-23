using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.ExpressionInterpreters;
using Baila.CSharp.Interpreter.Stdlib;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class DoWhileStatementInterpreter : StatementInterpreterBase<DoWhileStatement>
{
    public override void Interpret(DoWhileStatement statement)
    {
        NameTable.PushScope();

        var condition = statement.Condition.InterpretEvaluate();
        do
        {
            statement.Body.InterpretExecute();
        } while (condition.GetAsBoolean());
        
        NameTable.PopScope();
    }
}