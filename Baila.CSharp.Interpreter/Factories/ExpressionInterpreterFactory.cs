using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.ExpressionInterpreters;

namespace Baila.CSharp.Interpreter.Factories;

public static class ExpressionInterpreterFactory
{
    public static ExpressionInterpreterBase Create(IExpression expression)
    {
        return expression switch
        {
            AssignmentExpression => new AssignmentExpressionInterpreter(),
            BinaryExpression => new BinaryExpressionInterpreter(),
            PrefixUnaryExpression => new PrefixUnaryExpressionInterpreter(),
            VariableExpression => new VariableExpressionInterpreter(),
            FunctionCallExpression => new FunctionCallExpressionInterpreter(),
            IntValueExpression => new IntValueExpressionInterpreter(),
            StringValueExpression => new StringValueExpressionInterpreter(),
            BoolValueExpression => new BoolValueExpressionInterpreter(),
            _ => throw new InvalidOperationException(
                $"Expression is of invalid type: {expression.GetType().FullName}")
        };
    }
}