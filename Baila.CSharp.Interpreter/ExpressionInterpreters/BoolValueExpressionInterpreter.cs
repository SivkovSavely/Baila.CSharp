using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class BoolValueExpressionInterpreter : ExpressionInterpreterBase<BoolValueExpression>
{
    public override IValue Interpret(BoolValueExpression expression)
    {
        return new BooleanValue(expression.Value);
    }
}