using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.ControlFlowExceptions;
using Baila.CSharp.Interpreter.ExpressionInterpreters;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class ReturnStatementInterpreter : StatementInterpreterBase<ReturnStatement>
{
    public override void Interpret(ReturnStatement statement)
    {
        throw new ControlFlowReturnException(statement.ReturnExpression?.InterpretEvaluate());
    }
}