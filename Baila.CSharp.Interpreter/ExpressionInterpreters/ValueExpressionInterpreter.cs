using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class ValueExpressionInterpreter : ExpressionInterpreterBase<ValueExpression>
{
    public override IValue Interpret(ValueExpression expression)
    {
        return expression.Value;
    }
}