using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.Factories;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public abstract class StatementInterpreterBase
{
    public abstract void Interpret(IStatement statement);
}

public abstract class StatementInterpreterBase<TStatement> : StatementInterpreterBase
    where TStatement : IStatement
{
    public abstract void Interpret(TStatement statement);

    public override void Interpret(IStatement statement)
    {
        if (statement is not TStatement e)
        {
            throw new InvalidOperationException($"Statement should be of type {nameof(TStatement)}");
        }

        Interpret(e);
    }
}

public static class StatementInterpreter
{
    private static readonly Dictionary<Type, StatementInterpreterBase> InterpreterCache = new();

    public static void Interpret(IStatement statement)
    {
        var statementType = statement.GetType();
        if (!InterpreterCache.TryGetValue(statementType, out var value))
        {
            value = StatementInterpreterFactory.Create(statement);
            InterpreterCache[statementType] = value;
        }

        value.Interpret(statement);
    }
}

public static class StatementExtensions
{
    public static void InterpretExecute(this IStatement expression)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"[DEBUG] Exec {expression.GetType().Name} {expression}");
        Console.ResetColor();
        StatementInterpreter.Interpret(expression);
    }
}