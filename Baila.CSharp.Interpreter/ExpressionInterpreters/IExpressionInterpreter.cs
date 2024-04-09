using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Values;
using Baila.CSharp.Interpreter.Factories;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public abstract class ExpressionInterpreterBase
{
    public abstract IValue Interpret(IExpression expression);
}

public abstract class ExpressionInterpreterBase<TExpression> : ExpressionInterpreterBase
    where TExpression : IExpression
{
    public abstract IValue Interpret(TExpression expression);

    public override IValue Interpret(IExpression expression)
    {
        if (expression is not TExpression e)
        {
            throw new InvalidOperationException($"Expression should be of type {nameof(TExpression)}");
        }

        return Interpret(e);
    }
}

public static class ExpressionInterpreter
{
    private static readonly Dictionary<Type, ExpressionInterpreterBase> InterpreterCache = new();

    public static IValue Interpret(IExpression expression)
    {
        var expressionType = expression.GetType();
        if (InterpreterCache.TryGetValue(expressionType, out var value))
        {
            return value.Interpret(expression);
        }

        value = ExpressionInterpreterFactory.Create(expression);
        InterpreterCache[expressionType] = value;

        return value.Interpret(expression);
    }
}