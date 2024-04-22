using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class BinaryExpressionInterpreter : ExpressionInterpreterBase<BinaryExpression>
{
    private static readonly Dictionary<BinaryExpression.Operator, Func<IValue, IValue, IValue>> BinaryOperators = new()
    {
        [BinaryExpression.Operator.IntIntAddition] =
            (left, right) => new IntValue(left.GetAsInteger() + right.GetAsInteger()),
    };
    
    public override IValue Interpret(BinaryExpression expression)
    {
        var op = GetOperator(expression);
        var result = op.callback(expression.Left.InterpretEvaluate(), expression.Right.InterpretEvaluate());
        return result;
    }

    private (BinaryExpression.Operator op, Func<IValue, IValue, IValue> callback) GetOperator(BinaryExpression expression)
    {
        var (op, callback) = BinaryOperators.FirstOrDefault(op =>
            op.Key.Operation == expression.BinaryOperation &&
            op.Key.LeftType == expression.Left.GetBailaType() &&
            op.Key.RightType == expression.Right.GetBailaType());

        if (op is null)
        {
            throw new Exception($"Cannot use the operator '{expression.BinaryOperation.Op}' on operands of types '{expression.Left.GetBailaType()}' and '{expression.Right.GetBailaType()}'");
        }

        return (op, callback);
    }
}