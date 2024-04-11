using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class IntValueExpressionInterpreter : ExpressionInterpreterBase<IntValueExpression>
{
    public override IValue Interpret(IntValueExpression expression)
    {
        return new IntValue(expression.Value);
    }
}