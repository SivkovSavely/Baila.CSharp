using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.ExpressionInterpreters;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class IfElseStatementInterpreter : StatementInterpreterBase<IfElseStatement>
{
    public override void Interpret(IfElseStatement statement)
    {
        if (ExpressionInterpreter.Interpret(statement.Condition).GetAsBoolean())
        {
            StatementInterpreter.Interpret(statement.TrueStatement);
        }
        else if (statement.FalseStatement != null)
        {
            StatementInterpreter.Interpret(statement.FalseStatement);
        }
    }
}