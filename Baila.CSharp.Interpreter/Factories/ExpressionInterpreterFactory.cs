using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.ExpressionInterpreters;

namespace Baila.CSharp.Interpreter.Factories;

public class ExpressionInterpreterFactory
{
    public static ExpressionInterpreterBase Create(IExpression expression)
    {
        return expression switch
        {
            AssignmentExpression => new AssignmentExpressionInterpreter(),
            BinaryExpression => new BinaryExpressionInterpreter(),
            PrefixUnaryExpression => new PrefixUnaryExpressionInterpreter(),
            ValueExpression => new ValueExpressionInterpreter(),
            VariableExpression => new VariableExpressionInterpreter(),
            _ => throw new InvalidOperationException(
                $"Expression is of invalid type: {expression.GetType().FullName}")
        };
    }
}