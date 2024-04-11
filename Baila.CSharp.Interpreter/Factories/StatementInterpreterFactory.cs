using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.StatementInterpreters;

namespace Baila.CSharp.Interpreter.Factories;

public static class StatementInterpreterFactory
{
    public static StatementInterpreterBase Create(IStatement statement)
    {
        return statement switch
        {
            BlockStatement => new BlockStatementInterpreter(),
            DoWhileStatement => new DoWhileStatementInterpreter(),
            ForStatement => new ForStatementInterpreter(),
            IfElseStatement => new IfElseStatementInterpreter(),
            NoOpStatement => new NoOpStatementInterpreter(),
            WhileStatement => new WhileStatementInterpreter(),
            VariableDefineStatement => new VariableDefineStatementInterpreter(),
            ConstantDefineStatement => new ConstantDefineStatementInterpreter(),
            ExpressionStatement => new ExpressionStatementInterpreter(),
            _ => throw new InvalidOperationException(
                $"Statement is of invalid type: {statement.GetType().FullName}")
        };
    }
}