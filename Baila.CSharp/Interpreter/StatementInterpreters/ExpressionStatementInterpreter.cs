using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.ExpressionInterpreters;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class ExpressionStatementInterpreter : StatementInterpreterBase<ExpressionStatement>
{
    public override void Interpret(ExpressionStatement statement)
    {
        var value = statement.Expression.InterpretEvaluate();
        //statement.EvaluatedExpression ??= value;
        // TODO where to store evaluated expression? we can't do this in statement itself because it's not a runtime entity
    }
}